using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geometry;

namespace pixChange.HelperClass
{
 public  class ChangPixHelper
    {
     public void ChangeRasterValue(IRasterDataset2 pRasterDatset)
     {
         IRaster2 pRaster2 = pRasterDatset.CreateFullRaster() as IRaster2;
         IPnt pPntBlock = new PntClass();


         pPntBlock.X = 128;
         pPntBlock.Y = 128;


         IRasterCursor pRasterCursor = pRaster2.CreateCursorEx(pPntBlock);


         IRasterEdit pRasterEdit = pRaster2 as IRasterEdit;
         if (pRasterEdit.CanEdit())
         {
             IRasterBandCollection pBands = pRasterDatset as IRasterBandCollection;
             IPixelBlock3 pPixelblock3 = null;
             int pBlockwidth = 0;
             int pBlockheight = 0;
             System.Array pixels;
             IPnt pPnt = null;
             object pValue;
             long pBandCount = pBands.Count;


             //获取Nodata
             //IRasterProps pRasterPro = pRaster2 as IRasterProps;


             //object pNodata = pRasterPro.NoDataValue;
             do
             {
                 pPixelblock3 = pRasterCursor.PixelBlock as IPixelBlock3;
                 pBlockwidth = pPixelblock3.Width;
                 pBlockheight = pPixelblock3.Height;


                 for (int k = 0; k < pBandCount; k++)
                 {
                     pixels = (System.Array)pPixelblock3.get_PixelData(k);
                     for (int i = 0; i < pBlockwidth; i++)
                     {
                         for (int j = 0; j < pBlockheight; j++)
                         {
                             pValue = pixels.GetValue(i, j);


                             if (Convert.ToInt32(pValue) == 0 && isFlood[j, i] == true)
                             {
                                 pixels.SetValue(Convert.ToByte(50), i, j);
                             }
                         }
                     }
                     pPixelblock3.set_PixelData(k, pixels);
                 }
                 pPnt = pRasterCursor.TopLeft;
                 pRasterEdit.Write(pPnt, (IPixelBlock)pPixelblock3);
             }
             while (pRasterCursor.Next());
             System.Runtime.InteropServices.Marshal.ReleaseComObject(pRasterEdit);
             MessageBox.Show("done");
         }
     }
     public void reclass(IRaster pRaster, double weight)
        {
            //int trueCount = 0;
            //int falseCount = 0;
            IRasterProps rasterProps = (IRasterProps)pRaster;
            //设置栅格数据起始点
            IPnt pBlockSize = new Pnt();
            pBlockSize.SetCoords(rasterProps.Width, rasterProps.Height);
            //获取整个范围
            IPixelBlock pPixelBlock = pRaster.CreatePixelBlock(pBlockSize);
            // IPixelBlock3 pPixelBlock = (IPixelBlock3)pRaster.CreatePixelBlock(pBlockSize);
            //左上点坐标
            IPnt tlp = new Pnt();
            tlp.SetCoords(0, 0);
            //读入栅格
            IRasterBandCollection pRasterBands = pRaster as IRasterBandCollection;
            IRasterBand pRasterBand = pRasterBands.Item(0);
            IRawPixels pRawPixels = pRasterBands.Item(0) as IRawPixels;
            pRawPixels.Read(tlp, pPixelBlock);
            //将pixel的值组成数组
            System.Array pSafeArray = pPixelBlock.get_SafeArray(0) as System.Array;
            for (int y = 0; y < rasterProps.Height; y++)
            {
                for (int x = 0; x < rasterProps.Width; x++)
                {
                    //int value = Convert.ToInt32(pSafeArray.GetValue(x, y));
                    //Byte value = Convert.ToByte(pSafeArray.GetValue(x, y));
                    //if (value != 0 && isFlood[y, x] == false)
                    //{
                    //    pSafeArray.SetValue((Byte)(value * weight), x, y);
                    //}
                    if (isFlood[y, x] == true)
                    {
                        pSafeArray.SetValue(1, x, y);
                        //trueCount++;
                    }
                    else
                    {
                        pSafeArray.SetValue(0, x, y);
                        //falseCount++;
                    }
                }
            }
            pPixelBlock.set_SafeArray(0, pSafeArray);
            //编辑raster,将更新的值写入raster中
            IRasterEdit rasterEdit = pRaster as IRasterEdit;
            rasterEdit.Write(tlp, pPixelBlock);
            rasterEdit.Refresh();
            int Fcount=0;
            //m_map.AddLayer
            //MessageBox.Show("ok");
            //double result = Convert.ToDouble(pSafeArray.GetValue(100, 99));
           
        }
              IRasterBandCollection rasterbands = (IRasterBandCollection)pRasterDataset;
                IRasterBand rasterband = rasterbands.Item(0);
                IRawPixels rawpixels = (IRawPixels)rasterbands.Item(0);
                IRasterProps rasterpro = (IRasterProps)rasterband;




                IRasterDataset2 rasterDataset2 = (IRasterDataset2)pRasterDataset;
                IRaster raster = rasterDataset2.CreateFullRaster();
                IRaster2 raster2 = (IRaster2)raster;


                IPnt pBlockSize1 = new PntClass();
                IEnvelope envelope = rasterpro.Extent;
                pBlockSize1.SetCoords(envelope.Width, envelope.Height);


                IPixelBlock pixelBlock = raster2.CreateCursorEx(pBlockSize1).PixelBlock;
                int w = pixelBlock.Width;
                int h = pixelBlock.Height;
                //read the first pixel block
                IPnt topleftCorner = new PntClass();
                topleftCorner.SetCoords(0, 0);
                raster.Read(topleftCorner, pixelBlock);
                //modify one pixel value at location (assume the raster has a pixel type of uchar)
                IPixelBlock3 pixelBlock3 = (IPixelBlock3)pixelBlock;
                System.Array pixels = (System.Array)pixelBlock3.get_PixelData(0);
                for (int finalX = 0; finalX < pRows; finalX++)
                {
                    for (int finalY = 0; finalY < pColumns; finalY++)
                    {
                        if (isFlood[finalX, finalY] == true)
                        {
                            pixels.SetValue(Convert.ToByte(100), finalY, finalX);
                        }
                        //else
                        //{
                        //    pPixelData.SetValue(0, finalY, finalX);
                        //}
                    }
                }
                pixelBlock3.set_PixelData(0, (System.Object)pixels);
                //write the modified pixel block to the raster dataset
                IRasterEdit rasterEdit = (IRasterEdit)raster;
                rasterEdit.Write(topleftCorner, pixelBlock);


    }
}
