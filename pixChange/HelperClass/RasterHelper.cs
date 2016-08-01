using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.SpatialAnalystTools;

namespace pixChange.HelperClass
{
 public class RasterHelper
 {
     public  CumRaster cumRaster;

     public RasterHelper()
     {
         cumRaster=new CumRaster();
     }
     public void GetRasterProps(IRaster pRaster)
     {
         //IRasterProps pRasterPros = pRaster as IRasterProps;

         //int pH = pRasterPros.Height;//3973 

         //int pW = pRasterPros.Width;//5629  
         IRasterProps rasterProps = (IRasterProps)pRaster;
         cumRaster.dHeight = rasterProps.Height;//当前栅格数据集的行数
         cumRaster.dWidth = rasterProps.Width; //当前栅格数据集的列数
         cumRaster.dx = rasterProps.MeanCellSize().X; //栅格的宽度
         cumRaster.dy= rasterProps.MeanCellSize().Y; //栅格的高度
         cumRaster.extent = rasterProps.Extent; //当前栅格数据集的范围
        
     }

     public void BandsHande(IRaster pRaster)
     {
         IRasterBandCollection pBandColl = (IRasterBandCollection)pRaster;
         for (int i = 0; i < pBandColl.Count; i++)
         {
             IRasterBand pRasterBand = pBandColl.Item(i);
           var t=  pRasterBand.AttributeTable;
             pRasterBand.ComputeStatsAndHist();
         } 

     }


     /// <summary>
     /// 采用矢量数据裁剪栅格
     /// </summary>
     /// <param name="sender"></param>
     /// <param name="e"></param>
     private void MnuClipRasterByVector_Click(object sender, EventArgs e)
     {
         Geoprocessor GP;
         GP = new Geoprocessor();
         ExtractByMask extractbymask1 = new ExtractByMask();//实例化extract by mask工具类
         extractbymask1.in_raster = Application.StartupPath + "\\mapdata\\baotouDEM.img";
         extractbymask1.out_raster = Application.StartupPath + "\\temp\\maskDEM.img";
         extractbymask1.in_mask_data = Application.StartupPath + "\\mapdata\\New_Shapefile.shp";
         GP.Execute(extractbymask1, null);

         MessageBox.Show("成功导出！");
     }
    }
}
