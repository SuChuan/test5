using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;

namespace pixChange
{
    public partial class LayerMangerView : Form
    {
        private List<string> LayerNameList;
        private List<string> LayerPathList; 
        public LayerMangerView()
        {
            InitializeComponent();
            LayerNameList=new List<string>();
            getallLayers();
            exCheckedListBox1.DataSource = LayerNameList;
        }

        public LayerMangerView(ref List<string> LayerPathList)
        {
            InitializeComponent();
            LayerNameList = new List<string>();
            getallLayers();
            exCheckedListBox1.DataSource = LayerNameList;
            this.LayerPathList = LayerPathList;
        }

        private void getallLayers()
        {
            string path = @"..\..\Rources\LayersData";

            DirectoryInfo folder = new DirectoryInfo(path);
            FileInfo[] fileInfos = folder.GetFiles();
            
            List<string>fileType=new List<string>();
            fileType.Add(".jpg");
            fileType.Add(".shp");
            fileType.Add(".img");
            fileType.Add(".tif");
            foreach (FileInfo file in fileInfos)
            {
                //string name = file.Name;
                //这里需要加上判断是否可以成为图层数据源的代码  如：图片，矢量文件
                string ardess = file.Name.Substring(file.Name.IndexOf("."));
                if (fileType.Contains(ardess))
                { LayerNameList.Add(file.Name);}
            }  
        }

        private void ok_Click(object sender, EventArgs e)
        {
            foreach (var item in exCheckedListBox1.CheckedItems)
            {
              //  LayerPathList.Add(@"..\..\Rources\LayersData\"+item.ToString());
                string fullPath= @"..\..\Rources\LayersData\"+ item.ToString();
                string ss = fullPath.Substring(fullPath.LastIndexOf("."));
                if (fullPath.Substring(fullPath.LastIndexOf(".")) != ".shp")//栅格数据  
                {
                    //这里将RasterLayerClass改为RasterLayer  即可以嵌入互操作类型  下同
                    IRasterLayer rasterLayer = new RasterLayer();

                    rasterLayer.CreateFromFilePath(fullPath);
                    MainFrom.m_mapControl.AddLayer(rasterLayer, 0);
                    MainFrom.m_pTocControl.Update();
                }
                else//矢量数据
                {
                    //利用"\\"将文件路径分成两部分 
                    int Position = fullPath.LastIndexOf("\\");
                    //文件目录
                    string FilePath = fullPath.Substring(0, Position);
                    //
                    string ShpName = fullPath.Substring(Position + 1);
                    IWorkspaceFactory pWF;
                    pWF = new ShapefileWorkspaceFactory();
                    IFeatureWorkspace pFWS;
                    pFWS = (IFeatureWorkspace)pWF.OpenFromFile(FilePath, 0);
                    IFeatureClass pFClass;
                  //  String fname;
                 //   fname = strPath.Substring(loc1 + 1, loc2 - loc1 - 1);
                    pFClass = pFWS.OpenFeatureClass(ShpName);

                    IFeatureLayer pFLayer;
                    pFLayer = new FeatureLayer();
                    pFLayer.FeatureClass = pFClass;
                    pFLayer.Name = pFClass.AliasName;

                    MainFrom.m_mapControl.AddLayer(pFLayer, 0);
                    //选择数据源
                    MainFrom.toolComboBox.Items.Add(pFLayer.Name);
                    MainFrom.m_pTocControl.Update();
                }
              //  MainFrom.m_mapControl.Refresh(esriViewDrawPhase.esriViewGeography, null, null);
             //
                MainFrom.m_pTocControl.Update();
            }
           // exCheckedListBox1.CheckedItems
          
           this.Close();
        }

        private void exCheckedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
