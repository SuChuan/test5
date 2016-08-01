using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System.IO;
using ESRI.ArcGIS.DataSourcesGDB;

namespace pixChange
{
    public partial class ProListView : Form
    {
        public DataTable attributeTable;

        private IMapControl3 m_mapControl = MainFrom.m_mapControl;
        private IDisplayTable pDisplayTable;
        private string layerName;
        public ProListView()
        {
            InitializeComponent();
        }

        #region 根据图层字段创建一个只含字段的空DataTable
        /// <summary>
        /// 根据图层字段创建一个只含字段的空DataTable
        /// </summary>
        /// <param name="pLayer"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        private static DataTable CreateDataTableByLayer(ILayer pLayer, string tableName)
        {
            //创建一个DataTable表
            DataTable pDataTable = new DataTable(tableName);

            //取得ITable接口
            ITable pTable = pLayer as ITable;
            IField pField = null;
            DataColumn pDataColumn;
            //根据每个字段的属性建立DataColumn对象
            for (int i = 0; i < pTable.Fields.FieldCount; i++)
            {
                pField = pTable.Fields.get_Field(i);
                //新建一个DataColumn并设置其属性
                pDataColumn = new DataColumn(pField.Name);
                if (pField.Name == pTable.OIDFieldName)
                {
                    pDataColumn.Unique = true;//字段值是否唯一
                }
                //字段值是否允许为空
                pDataColumn.AllowDBNull = pField.IsNullable;
                //字段别名
                pDataColumn.Caption = pField.AliasName;
                //字段数据类型
                pDataColumn.DataType = System.Type.GetType(ParseFieldType(pField.Type));
                //字段默认值
                pDataColumn.DefaultValue = pField.DefaultValue;

                //当字段为String类型是设置字段长度
                if (pField.VarType == 8)
                {
                    pDataColumn.MaxLength = pField.Length;
                }
                //字段添加到表中
                pDataTable.Columns.Add(pDataColumn);
                pField = null;
                pDataColumn = null;
            }
            return pDataTable;
        }

        //因为GeoDatabase的数据类型与.NET的数据类型不同，故要进行转换。转换函数如下：
        /// <summary>
        /// 将GeoDatabase字段类型转换成.Net相应的数据类型
        /// </summary>
        /// <param name="fieldType">字段类型</param>
        /// <returns></returns>
        public static string ParseFieldType(esriFieldType fieldType)
        {
            switch (fieldType)
            {
                case esriFieldType.esriFieldTypeBlob:
                    return "System.String";
                case esriFieldType.esriFieldTypeDate:
                    return "System.DateTime";
                case esriFieldType.esriFieldTypeDouble:
                    return "System.Double";
                case esriFieldType.esriFieldTypeGeometry:
                    return "System.String";
                case esriFieldType.esriFieldTypeGlobalID:
                    return "System.String";
                case esriFieldType.esriFieldTypeGUID:
                    return "System.String";
                case esriFieldType.esriFieldTypeInteger:
                    return "System.Int32";
                case esriFieldType.esriFieldTypeOID:
                    return "System.String";
                case esriFieldType.esriFieldTypeRaster:
                    return "System.String";
                case esriFieldType.esriFieldTypeSingle:
                    return "System.Single";
                case esriFieldType.esriFieldTypeSmallInteger:
                    return "System.Int32";
                case esriFieldType.esriFieldTypeString:
                    return "System.String";
                default:
                    return "System.String";
            }
        }
        #endregion

        #region 填充DataTable中的数据
        /// <summary> 
        /// 填充DataTable中的数据
        /// </summary>
        /// <param name="pLayer"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataTable CreateDataTable(ILayer pLayer, string tableName)
        {
            //创建空DataTable,并确定表头的名称
            DataTable pDataTable = CreateDataTableByLayer(pLayer, tableName);

            //取得图层类型,如果是shape字段,则表的数据里放该类型名称
            string shapeType = getShapeType(pLayer);

            //创建DataTable的行对象
            DataRow pDataRow = null;
            //从ILayer查询到ITable
            ITable pTable = pLayer as ITable;
            ICursor pCursor = pTable.Search(null, false);
            //取得ITable中的行信息
            IRow pRow = pCursor.NextRow();
            int n = 0;
            while (pRow != null)
            {
                //新建DataTable的行对象
                pDataRow = pDataTable.NewRow();
                for (int i = 0; i < pRow.Fields.FieldCount; i++)
                {
                    //如果字段类型为esriFieldTypeGeometry，则根据图层类型设置字段值
                    if (pRow.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        pDataRow[i] = shapeType;
                    }
                    //当图层类型为Anotation时，要素类中会有esriFieldTypeBlob类型的数据，
                    //其存储的是标注内容，如此情况需将对应的字段值设置为Element
                    else if (pRow.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeBlob)
                    {
                        pDataRow[i] = "Element";
                    }
                    else
                    {
                        pDataRow[i] = pRow.get_Value(i);
                    }
                }
                //添加DataRow到DataTable
                pDataTable.Rows.Add(pDataRow);
                pDataRow = null;
                n++;
                //为保证效率，一次只装载最多条记录
                if (n == 2000)
                {
                    pRow = null;
                }
                else
                {
                    pRow = pCursor.NextRow();
                }
            }

            //开始链接外部数据
            DataSet MyDs = new DataSet();
            if (getOutData(tableName, "ID", ref MyDs) == true)
            {
                DataTable MyJionedTbl;
                MyJionedTbl = JionOutData(pDataTable, MyDs.Tables[0]);
                return MyJionedTbl;
            }
            else
            {
                return pDataTable;
            }
        }


        //进行外部链接
        private static DataTable JionOutData(DataTable InTable, DataTable OutTable)
        {
            DataTable JoinedTable;
            MegerTable MyMerger = new MegerTable();
            string[] keyFields = new string[1];
            string[] megerFields = new string[3];
            int i;
            keyFields[0] = "ID";

            for (i = 1; i < OutTable.Columns.Count; i++)
            {
                megerFields[i - 1] = OutTable.Columns[i].ColumnName;
            }
            JoinedTable = MyMerger.LeftJoinTabel(InTable, OutTable, keyFields, megerFields);

            return JoinedTable;
        }



        //寻找链接的外部数据
        //Join(flyr.Name, Application.StartupPath, "DBPoint.mdb", "ID"))
        private static bool getOutData(string strTblName, string JoinFieldName, ref DataSet ReturnDs)
        {
            ReturnDs = new DataSet();
            if (strTblName == "包头" && JoinFieldName == "ID")
            {
                string sqlstr;
                WinInrush.Class.Database MyDB = new WinInrush.Class.Database();
                DataSet MyDs = new DataSet();
                sqlstr = "SELECT * FROM " + strTblName + " ORDER BY " + JoinFieldName;

                ReturnDs = MyDB.GetDataFromDB(sqlstr);

                return true;
            }
            else
            {
                return false;
            }
        }



        //上面的代码中涉及到一个获取图层类型的函数getShapeType，此函数是通过ILayer判断图层类型的，代码如下：
        /// <summary>
        /// 获得图层的Shape类型
        /// </summary>
        /// <param name="pLayer">图层</param>
        /// <returns></returns>
        public static string getShapeType(ILayer pLayer)
        {
            IFeatureLayer pFeatLyr = (IFeatureLayer)pLayer;
            switch (pFeatLyr.FeatureClass.ShapeType)
            {
                case esriGeometryType.esriGeometryPoint:
                    return "Point";
                case esriGeometryType.esriGeometryPolyline:
                    return "Polyline";
                case esriGeometryType.esriGeometryPolygon:
                    return "Polygon";
                default:
                    return "";
            }
        }
        #endregion

        #region 绑定DataTable到DataGridView
        ///<summary> 
        /// 绑定DataTable到DataGridView
        /// </summary>
        /// <param name="player"></param>
        public void CreateAttributeTable(ILayer player)
        {
            string tableName;
            tableName = getValidFeatureClassName(player.Name);
            attributeTable = CreateDataTable(player, tableName);

            //gridview的东西不允许用户进行修改
            attributeTable.DefaultView.AllowNew = false;

            //设置数据源
            this.DataGrdView.DataSource = attributeTable;

            this.Text = "属性表[" + tableName + "] " + "记录数：" + attributeTable.Rows.Count.ToString();
        }

        //因为DataTable的表名不允许含有“.”，因此我们用“_”替换。函数如下：
        /// <summary>
        /// 替换数据表名中的点
        /// </summary>
        /// <param name="FCname"></param>
        /// <returns></returns>
        public static string getValidFeatureClassName(string FCname)
        {
            int dot = FCname.IndexOf(".");
            if (dot != -1)
            {
                return FCname.Replace(".", "_");
            }
            return FCname;
        }
        #endregion

        public void showTable(DataTable pDataTable)
        {
            this.DataGrdView.DataSource = pDataTable;
        }
        public bool Join(String sLayerName, String sFilePath, String sFileName, String sFieldName)
        {
            IWorkspaceFactory pWorkspaceFactory;
            IWorkspace pWorkspace;
            IFeatureWorkspace pFeatureWorkspace;
            IFeatureLayer pFeatureLayer;
            IFeatureClass pFeatureClass;
            ITable pPrimaryTable;//    'shp主表
            ITable pForeignTable;//    '外部表
            IDisplayRelationshipClass pDisplayRelationshipC;
            IMemoryRelationshipClassFactory pMemoryRelationshipCF;
            IRelationshipClass pRelationshipClass;

            int nNumber;
            String sForeignFile;
            try
            {
                sForeignFile = sFilePath + "\\" + sFileName;
                if (!File.Exists(sForeignFile))
                {
                    MessageBox.Show("外部数据库不存在！");
                    return false;
                }
                String dname = sFilePath + "\\" + sFileName;
                pWorkspaceFactory = new AccessWorkspaceFactory();
                pWorkspace = pWorkspaceFactory.OpenFromFile(dname, 0);
                pFeatureWorkspace = pWorkspace as IFeatureWorkspace;
                //pForeignTable = pFeatureWorkspace.OpenTable("外部数据");
                pForeignTable = pFeatureWorkspace.OpenTable("包头");
                string values = "";
                for (int i = 0; i < pForeignTable.Fields.FieldCount; i++)
                {
                    values += pForeignTable.Fields.get_Field(i).Name;

                }
                //MessageBox.Show(values);
                //DataGrdView.DataSource = pForeignTable;//added 
                pFeatureLayer = new FeatureLayer();
                for (nNumber = 0; nNumber < m_mapControl.LayerCount; nNumber++)
                {
                    String tmpLayerName = m_mapControl.get_Layer(nNumber).Name;
                    if (tmpLayerName == sLayerName)
                    {
                        pFeatureLayer = (IFeatureLayer)(m_mapControl.get_Layer(nNumber));
                        break;
                    }
                }
                if (pFeatureLayer == null)
                {
                    MessageBox.Show("No Layer's Name is " + sLayerName);
                    return false;

                }

                pDisplayTable = (IDisplayTable)pFeatureLayer;
                pFeatureClass = (IFeatureClass)pDisplayTable.DisplayTable;
                pPrimaryTable = (ITable)pFeatureClass;

                pMemoryRelationshipCF = new MemoryRelationshipClassFactory();
                pRelationshipClass = pMemoryRelationshipCF.Open("TabletoLayer", pPrimaryTable as IObjectClass, sFieldName, pForeignTable as IObjectClass, sFieldName, "forward", "backward", esriRelCardinality.esriRelCardinalityOneToOne);
                pDisplayRelationshipC = pFeatureLayer as IDisplayRelationshipClass;
                pDisplayRelationshipC.DisplayRelationshipClass(pRelationshipClass, esriJoinType.esriLeftOuterJoin);
                //added function to show the displayTable in this dataGridView
                setDataSource(pDisplayTable);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void setDataSource(IDisplayTable pDisplayTable)
        {
            DataTable pTable = new DataTable();
            IField pField = null;
            DataColumn pColumn;
            for (int i = 0; i < pDisplayTable.DisplayTable.Fields.FieldCount; i++)
            {
                pField = pDisplayTable.DisplayTable.Fields.get_Field(i);
                string name = getRealName(pField);
                pColumn = new DataColumn(name);//delete table name
                //
                MessageBox.Show("Field name:" + name + " " + pField.Name + "; Field aliasName: " + pField.AliasName);
                pColumn.AllowDBNull = pField.IsNullable;
                if (pTable.Columns.Contains(pColumn.ColumnName))
                {
                    pColumn.ColumnName += "1";
                    pTable.Columns.Add(pColumn);
                }
                else
                {
                    pTable.Columns.Add(pColumn);
                }
            }
            ICursor pcusor = pDisplayTable.SearchDisplayTable(null, false);
            IRow pRow;
            pRow = pcusor.NextRow();
            DataRow pDataRow;
            while (pRow != null)
            {
                pDataRow = pTable.NewRow();
                for (int i = 0; i < pRow.Fields.FieldCount; i++)
                {
                    if (pRow.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                    {
                        pDataRow[i] = "shapeType";//ensure the type, need to be changed
                    }
                    else if (pRow.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeBlob)
                    {
                        pDataRow[i] = "Element";
                    }
                    else
                    {
                        pDataRow[i] = pRow.get_Value(i);
                    }
                }
                pTable.Rows.Add(pDataRow);
                pRow = pcusor.NextRow();
            }
            DataGrdView.DataSource = pTable;
            DataGrdView.Refresh();
        }
        private void joinBtn_Click(object sender, EventArgs e)
        {
            if (layerName != "")
            {
                Join(layerName, Application.StartupPath, "DBPoint.mdb", "ID");
            }
        }
        public void getLayerName(string name)
        {
            layerName = name;
            //MessageBox.Show(name);
        }
        private string getRealName(IField pfield)
        {
            string pname = pfield.Name;
            string name;
            int dot = pname.IndexOf(".");
            if (dot != -1)
            {
                name = pname.Substring(dot + 1);
                return name;
            }
            else
            {
                return pname;
            }
        }

        private void FrmSelectByLocaResult_Load(object sender, EventArgs e)
        {

        }
        private void FrmSelectByLocaResult_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DataGrdView.DataSource = null;
            this.DataGrdView.Refresh();
        }
    
    }
}
