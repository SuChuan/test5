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
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.SystemUI;
using pixChange.HelperClass;

namespace pixChange
{
    public partial class MainFrom : Form
    {

        //公共变量用于表示整个系统都能访问的图层控件和环境变量
      //  public static SpatialAnalysisOption SAoption;
        public static IMapControl3 m_mapControl = null;
        public static ITOCControl2 m_pTocControl = null;
        public static ToolStripComboBox toolComboBox = null;
        String m_mapDocumentName = "";

        IToolbarMenu m_pMenuLayer;
        //用于判断当前鼠标点击的菜单命令,以备在地图控件中判断操作
        static public CustomTool m_cTool;
        IScreenDisplay m_focusScreenDisplay;// For 平移

        //NA分析变量

        //For 放大,缩小，平移
        INewEnvelopeFeedback m_feedBack;//  '拉框
        IPoint m_mouseDownPoint;
        bool m_isMouseDown;
        public bool frmAttriQueryisOpen = false;

    
        //当前窗体实例
        public MainFrom pCurrentWin = null;
        //当前主地图控件实例
        public AxMapControl pCurrentMap = null;
        //当前鹰眼控件实例
        public AxMapControl pCurrentSmallMap = null;
        //当前TOC控件实例
        public AxTOCControl pCurrentTOC = null;
        public enum CustomTool
        {
            None = 0,
            ZoomIn = 1,
            ZoomOut = 2,
            Pan = 3,
            RuleMeasure = 4,
            AreaMeasure = 5,
            PointSelect = 6,
            RectSelect = 7,
            PolygonSelect = 8,
            CircleSelect = 9,
            NAanalysis = 10,
            StartEditing = 11,
            SelectFeature = 12,
            MoveFeature = 13,
            EditVertex = 14,
            EditUndo = 15,
            EditRedo = 16,
            EditDeleteFeature = 17,
            EditAttribute = 18,
        };
        public MainFrom()
        {
            InitializeComponent();
        }

        private void axTOCControl1_OnMouseDown(object sender, ESRI.ArcGIS.Controls.ITOCControlEvents_OnMouseDownEvent e)
        {

        }

        private void 图层管理_Click(object sender, EventArgs e)
        {
            List<string>LayerPathList=new List<string>();
            LayerMangerView lm = new LayerMangerView(ref LayerPathList);
            lm.Show();
        
        }

        private void MainFrom_Load(object sender, EventArgs e)
        {
            //将地图控件赋给变量，这样就可以使用接口所暴露的属性和方法了
            //axMapControl1属于主框架的私有控件，外部不能访问，所以采用这种模式可以通过公共变量的形式操作
            m_mapControl = (IMapControl3)axMapControl1.Object;
            m_pTocControl = (ITOCControl2)axTOCControl1.Object;

            toolComboBox = this.toolStripComboBox1;
            //TOC控件绑定地图控件
            m_pTocControl.SetBuddyControl(m_mapControl);

        }

        private void ToolButtonZoomIn_Click(object sender, EventArgs e)
        {
            ICommand zoomIn;
            zoomIn = new ControlsMapZoomInTool();
            zoomIn.OnCreate(m_mapControl);
            m_mapControl.CurrentTool = zoomIn as ITool;
            m_cTool = CustomTool.ZoomIn;
        }

        private void ToolButtonZoomOut_Click(object sender, EventArgs e)
        {
            ICommand zoomOut;
            zoomOut = new ControlsMapZoomOutTool();
            zoomOut.OnCreate(m_mapControl);
            m_cTool = CustomTool.ZoomOut;
            m_mapControl.CurrentTool = zoomOut as ITool;
        }

        private void ToolButtonPan_Click(object sender, EventArgs e)
        {
            ICommand pan;
            pan = new ControlsMapPanTool();
            pan.OnCreate(m_mapControl);
            m_cTool = CustomTool.Pan;
            m_mapControl.CurrentTool = pan as ITool;
        }

        private void ToolButtonFull_Click(object sender, EventArgs e)
        {
            m_mapControl.Extent = m_mapControl.FullExtent;
        }

        private void axTOCControl1_OnMouseMove(object sender, ITOCControlEvents_OnMouseMoveEvent e)
        {
            //鼠标未落下,退出
            IPoint pt = new ESRI.ArcGIS.Geometry.Point();
            pt.PutCoords(e.x, e.y);

            switch (m_cTool)
            {
                case CustomTool.ZoomIn:
                case CustomTool.ZoomOut:
                    //'Get 
                  

                    break;
                case CustomTool.Pan:
                    m_focusScreenDisplay = m_mapControl.ActiveView.ScreenDisplay;
                    m_focusScreenDisplay.PanMoveTo(pt);
                    break;
              
               

            }
        }

        private void axTOCControl1_OnKeyDown(object sender, ITOCControlEvents_OnKeyDownEvent e)
        {
           






        }

        private void axMapControl1_OnKeyUp(object sender, IMapControlEvents2_OnKeyUpEvent e)
        {
            //鼠标未落下,退出
            if (m_isMouseDown == false) return;

            IActiveView pActiveView = m_mapControl.ActiveView;
            IEnvelope pEnvelope;

            switch (m_cTool)
            {
                case CustomTool.ZoomIn:

                    ////If an envelope has not been tracked

                    if (m_feedBack == null)
                    {
                        //Zoom in from mouse click
                        pEnvelope = pActiveView.Extent;
                        pEnvelope.CenterAt(m_mouseDownPoint);
                        pEnvelope.Expand(0.5, 0.5, true);


                    }
                    else
                    {
                        //Stop the envelope feedback
                        pEnvelope = m_feedBack.Stop();

                        //Exit if the envelope height or width is 0
                        if (pEnvelope.Width == 0 || pEnvelope.Height == 0)
                        {
                            m_feedBack = null;
                            m_isMouseDown = false;
                        }
                    }
                    //Set the new extent
                    pActiveView.Extent = pEnvelope;
                    break;

                case CustomTool.ZoomOut:
                    IEnvelope pFeedEnvelope;

                    double NewWidth, NewHeight;

                    //If an envelope has not been tracked
                    if (m_feedBack == null)
                    {
                        //Zoom out from the mouse click
                        pEnvelope = pActiveView.Extent;
                        pEnvelope.Expand(2, 2, true);
                        pEnvelope.CenterAt(m_mouseDownPoint);
                    }
                    else
                    {
                        //Stop the envelope feedback
                        pFeedEnvelope = m_feedBack.Stop();

                        //Exit if the envelope height or width is 0
                        if (pFeedEnvelope.Width == 0 || pFeedEnvelope.Height == 0)
                        {
                            m_feedBack = null;
                            m_isMouseDown = false;

                        }

                        NewWidth = pActiveView.Extent.Width*(pActiveView.Extent.Width/pFeedEnvelope.Width);
                        NewHeight = pActiveView.Extent.Height*(pActiveView.Extent.Height/pFeedEnvelope.Height);

                        //Set the new extent coordinates
                        pEnvelope = new Envelope() as IEnvelope;
                        pEnvelope.PutCoords(
                            pActiveView.Extent.XMin -
                            ((pFeedEnvelope.XMin - pActiveView.Extent.XMin)*
                             (pActiveView.Extent.Width/pFeedEnvelope.Width)),
                            pActiveView.Extent.YMin -
                            ((pFeedEnvelope.YMin - pActiveView.Extent.YMin)*
                             (pActiveView.Extent.Height/pFeedEnvelope.Height)),
                            (pActiveView.Extent.XMin -
                             ((pFeedEnvelope.XMin - pActiveView.Extent.XMin)*
                              (pActiveView.Extent.Width/pFeedEnvelope.Width))) + NewWidth,
                            (pActiveView.Extent.YMin -
                             ((pFeedEnvelope.YMin - pActiveView.Extent.YMin)*
                              (pActiveView.Extent.Height/pFeedEnvelope.Height))) + NewHeight);

                    }


                    //Set the new extent
                    pActiveView.Extent = pEnvelope;
                    break;

                case CustomTool.Pan:
                    pEnvelope = m_focusScreenDisplay.PanStop();

                    //Check if user dragged a rectangle or just clicked.
                    //If a rectangle was dragged, m_ipFeedback in non-NULL
                    if (pEnvelope != null)
                    {
                        m_focusScreenDisplay.DisplayTransformation.VisibleBounds = pEnvelope;
                        m_focusScreenDisplay.Invalidate(null, true, (short) esriScreenCache.esriAllScreenCaches);

                    }
                    m_cTool = CustomTool.None;
                    break;
            }



        }

        private void axMapControl1_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            IFeatureLayer pFeatureLayer;
            IFeatureClass pFeatureClass ;
            //新建一个空间过滤器
            ISpatialFilter pSpatialFilter;
            IQueryFilter pFilter;

            IFeatureCursor pCursor;

            //定义各种空间类型数据的符号
            ISimpleMarkerSymbol simplePointSymbol;
            ISimpleFillSymbol simpleFillSymbol;
            ISimpleLineSymbol simpleLineSymbol;
            //用于闪烁的符号
            ISymbol symbol;

            IFeature pFeature;
            switch (m_cTool)
            {
                case CustomTool.ZoomIn:
                case CustomTool.ZoomOut:
                    //case CustomTool.Pan:
                    //Create a point in map coordinates
                    IPoint pPoint = new ESRI.ArcGIS.Geometry.Point();
                    m_mouseDownPoint = new ESRI.ArcGIS.Geometry.Point();
                    pPoint.X = e.mapX;
                    pPoint.Y = e.mapY;
                    m_mouseDownPoint = pPoint;
                    m_isMouseDown = true;
                    break;
                case CustomTool.RuleMeasure://测距离
                    IPolyline plinemeasure;
                    plinemeasure = (IPolyline)m_mapControl.TrackLine();
                    ISpatialReferenceFactory spatialReferenceFactory;
                    spatialReferenceFactory = new SpatialReferenceEnvironment();

                    IProjectedCoordinateSystem pPCS;
                    pPCS = spatialReferenceFactory.CreateProjectedCoordinateSystem((int)esriSRProjCSType.esriSRProjCS_WGS1984N_AsiaAlbers);
                    plinemeasure.Project(pPCS);

                    m_mapControl.MapUnits = esriUnits.esriKilometers;

                    IGeometry input_geometry;
                    input_geometry = plinemeasure.FromPoint;
                    IProximityOperator proOperator = (IProximityOperator)input_geometry;
                    double check;
                    check = proOperator.ReturnDistance(plinemeasure.ToPoint);

                    MessageBox.Show("所测距离为：" + check.ToString("#######.##") + "米");
                    m_cTool = CustomTool.None;
                    break;             

            
                case CustomTool.RectSelect:
                    pFeatureLayer = (IFeatureLayer)m_mapControl.get_Layer(toolComboBox.SelectedIndex);
                    pFeatureClass = pFeatureLayer.FeatureClass;
                    IEnvelope pRect = new Envelope() as IEnvelope;
                    pRect = m_mapControl.TrackRectangle();

                    //新建一个空间过滤器
                    pSpatialFilter = new SpatialFilter();
                    pSpatialFilter.Geometry = pRect;
                    //依据被选择的要素类的类型不同，设置不同的空间过滤关系
                    switch (pFeatureClass.ShapeType)
                    {
                        case esriGeometryType.esriGeometryPoint:
                        case esriGeometryType.esriGeometryMultipoint:
                            pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;
                            break;
                        case esriGeometryType.esriGeometryPolyline:
                        case esriGeometryType.esriGeometryLine:
                            pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelCrosses;
                            break;
                        case esriGeometryType.esriGeometryPolygon:
                        case esriGeometryType.esriGeometryEnvelope:
                            pSpatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
                            break;
                    }


                    pSpatialFilter.GeometryField = pFeatureClass.ShapeFieldName;
                    pFilter = pSpatialFilter;
                    //通过空间关系查询
                    pCursor = pFeatureLayer.Search(pFilter, false);

                    //定义各种空间类型数据的符号
                    simplePointSymbol = new SimpleMarkerSymbol();
                    simpleFillSymbol = new SimpleFillSymbol();
                    simpleLineSymbol = new SimpleLineSymbol();
                    simplePointSymbol.Style = esriSimpleMarkerStyle.esriSMSCircle;
                    simplePointSymbol.Size = 5;
                    simplePointSymbol.Color =ColorHelper.GetRGBColor(255, 0, 0);

                    simpleLineSymbol.Width = 2;
                    simpleLineSymbol.Color = ColorHelper.GetRGBColor(255, 0, 99);
                    simpleFillSymbol.Outline = simpleLineSymbol;
                    simpleFillSymbol.Color = ColorHelper.GetRGBColor(222, 222, 222);
                    //用于闪烁的符号
                    pFeature = pCursor.NextFeature();
                    DataTable pDataTable = createDataTableByLayer(pFeatureLayer as ILayer);
                    while (pFeature != null)
                    {
                         axMapControl1.Map.SelectFeature(pFeatureLayer,pFeature);  //这句话可能不需要，但是书中留了这行代码，
                        IGeometry pShape;
                        pShape = pFeature.Shape;
                        ITable pTable = pFeature as ITable;
                        DataRow pDataRow = pDataTable.NewRow();
                        for (int i = 0; i < pFeature.Fields.FieldCount; i++)
                        {

                            if (pFeature.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                            {
                                pDataRow[i] = getShapeType(pFeatureLayer as ILayer);
                            }
                            else if (pFeature.Fields.get_Field(i).Type == esriFieldType.esriFieldTypeBlob)
                            {
                                pDataRow[i] = "Element";
                            }
                            else
                            {
                                pDataRow[i] = pFeature.get_Value(i);
                            }

                        }
                        pDataTable.Rows.Add(pDataRow);
                        switch (pFeatureClass.ShapeType)
                        {
                            case esriGeometryType.esriGeometryPoint:
                            case esriGeometryType.esriGeometryMultipoint:
                                symbol = (ISymbol)simplePointSymbol;
                                m_mapControl.FlashShape(pShape, 5, 100, symbol);
                                break;
                            case esriGeometryType.esriGeometryPolyline:
                            case esriGeometryType.esriGeometryLine:
                                symbol = (ISymbol)simpleLineSymbol;
                                m_mapControl.FlashShape(pShape, 5, 100, symbol);
                                break;
                            case esriGeometryType.esriGeometryPolygon:
                            case esriGeometryType.esriGeometryEnvelope:
                                symbol = (ISymbol)simpleFillSymbol;
                                m_mapControl.FlashShape(pShape, 5, 100, symbol);
                                break;
                        }

                        pFeature = pCursor.NextFeature();
                    }
                    m_cTool = CustomTool.None;
               
                    ProListView result=new ProListView();
                    result.showTable(pDataTable);

                    result.getLayerName(pFeatureLayer.Name);
                    result.Show();
                  
                    break;
              
                case CustomTool.Pan:
                    pPoint = new ESRI.ArcGIS.Geometry.Point();
                    pPoint.X = e.mapX;
                    pPoint.Y = e.mapY;
                    m_mouseDownPoint = pPoint;
                    m_isMouseDown = true;
                    m_focusScreenDisplay = m_mapControl.ActiveView.ScreenDisplay;
                    m_focusScreenDisplay.PanStart(m_mouseDownPoint);
                    break;
          

            }

        }

        #region get dataTable by Ilayer
        private DataTable createDataTableByLayer(ILayer player)
        {
            DataTable dataTable = new DataTable();
            ITable ptable = player as ITable;
            IField pField;
            DataColumn pDataColumn;
            for (int i = 0; i < ptable.Fields.FieldCount; i++)
            {
                pField = ptable.Fields.get_Field(i);
                pDataColumn = new DataColumn(pField.Name);
                dataTable.Columns.Add(pDataColumn);
                pField = null;
                pDataColumn = null;
            }
            return dataTable;
        }
        private DataTable createDataTable()
        {
            DataTable dataTable = new DataTable();
            //ITable ptable = player as ITable;
            //IField pField;
            //DataRow pDataRow;
            DataColumn pColumn1 = new DataColumn("Feilds");
            DataColumn pColumn2 = new DataColumn("Value");
            dataTable.Columns.Add(pColumn1);
            dataTable.Columns.Add(pColumn2);
            /*
            for (int i = 0; i < ptable.Fields.FieldCount; i++)
               {
                    pField = ptable.Fields.get_Field(i);
                    pDataRow = new DataRow();
                }
            */
            return dataTable;
        }
        private string getShapeType(ILayer player)
        {
            IFeatureLayer pFlayer = (IFeatureLayer)player;
            switch (pFlayer.FeatureClass.ShapeType)
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

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (toolComboBox.SelectedIndex < 0)
            {
                MessageBox.Show("请先选择操作图层");
            }
            //改变鼠标形状
            m_mapControl.MousePointer = esriControlsMousePointer.esriPointerArrow;
            m_mapControl.CurrentTool = null;
            m_cTool = CustomTool.RectSelect;
            //将mapcontrol的tool设为nothing，不然会影响效果
        }

       
    }
}
