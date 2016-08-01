using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using pixChange.HelperClass;

namespace pixChange
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void AddRaster_Click(object sender, EventArgs e)
        {
            try
            {

                OpenFileDialog openFile = new OpenFileDialog();

                string fileName;



                openFile.Title = "添加栅格数据";

                openFile.Filter = "IMG图像(*.img)|*.img|TIFF图像(*.tif)|*.tif|所有文件(*.*)|*.*";

                openFile.ShowDialog();

                fileName = openFile.FileName;


                //这里将RasterLayerClass改为RasterLayer  即可以嵌入互操作类型  下同
                IRasterLayer rasterLayer = new RasterLayer();

                rasterLayer.CreateFromFilePath(fileName);

                axMapControl1.AddLayer(rasterLayer, 0);

            }

            catch
            {

                MessageBox.Show("添加栅格数据错误!");

            }

        }

        private void RasterExtent_Click(object sender, EventArgs e)
        {
            ILayer pLayer = axMapControl1.get_Layer(0);//假定第一个图层为Raster数据

            IRasterLayer rasterLayer = (IRasterLayer)pLayer;

            IRaster pRaster = rasterLayer.Raster;

            IRasterProps pRasterProps = (IRasterProps)pRaster;

            MessageBox.Show("Raster数据的范围为：/nXmax=" + pRasterProps.Extent.XMax.ToString()

                             + "/nXmin=" + pRasterProps.Extent.XMin.ToString()

                             + "/nYmax=" + pRasterProps.Extent.YMax.ToString()

                             + "/nYmin=" + pRasterProps.Extent.YMin.ToString());

        }

        private void getPix_Click(object sender, EventArgs e)
        {
            ILayer pLayer = axMapControl1.get_Layer(0);//假定第一个图层为Raster数据

            IRasterLayer rasterLayer = (IRasterLayer)pLayer;

            IRaster pRaster = rasterLayer.Raster;

            IRasterProps pRasterProps = (IRasterProps)pRaster;



            IPnt pnt = new Pnt();

            pnt.SetCoords(10, 5);



            IPnt pntSize = new Pnt();

            pntSize.SetCoords(1, 1);



            IPixelBlock pixelBlock = pRaster.CreatePixelBlock(pntSize);



            pRaster.Read(pnt, pixelBlock);

            object obj = pixelBlock.GetVal(0, 0, 0);

            MessageBox.Show(Convert.ToUInt32(obj).ToString());

        }
        //栅格属性
        private void button1_Click(object sender, EventArgs e)
        {
            ILayer pLayer = axMapControl1.get_Layer(0);//假定第一个图层为Raster数据

            IRasterLayer rasterLayer = (IRasterLayer)pLayer;
          //  IRasterda
                //   IRaster2 pRaster2 = (IRaster2)rasterLayer;
            IRaster pRaster = rasterLayer.Raster;
         //   IRasterDataset2 pRaster2 = rasterLayer.CreateFromRaster(pRaster)
            //RasterHelper  rh=new RasterHelper();
            //rh.GetRasterProps(pRaster);
            //var tt = rh.cumRaster;
            // rh.BandsHande(pRaster);
            reclass(pRaster,4);
           // ChangeRasterValue(pRaster2);
            //  funColorForRaster_Classify(rasterLayer);
            // UniqueValueRender(rasterLayer);
        }
        //分级渲染
        public void funColorForRaster_Classify(IRasterLayer pRasterLayer)
        {
            IRasterClassifyColorRampRenderer pRClassRend = new RasterClassifyColorRampRendererClass();
            IRasterRenderer pRRend = pRClassRend as IRasterRenderer;


            IRaster pRaster = pRasterLayer.Raster;
            IRasterBandCollection pRBandCol = pRaster as IRasterBandCollection;
            IRasterBand pRBand = pRBandCol.Item(0);
            if (pRBand.Histogram == null)
            {
                pRBand.ComputeStatsAndHist();
            }
            pRRend.Raster = pRaster;
            pRClassRend.ClassCount = 5;
            pRRend.Update();


            IRgbColor pFromColor = new RgbColorClass();
            pFromColor.Red = 135;//天蓝色
            pFromColor.Green = 206;
            pFromColor.Blue = 235;
            IRgbColor pToColor = new RgbColorClass();
            pToColor.Red = 255;//草坪绿
            pToColor.Green = 0;
            pToColor.Blue = 0;


            IAlgorithmicColorRamp colorRamp = new AlgorithmicColorRampClass();
            //决定了颜色可以分的级别
            colorRamp.Size = 5;
            colorRamp.FromColor = pFromColor;
            colorRamp.ToColor = pToColor;
            bool createColorRamp;
            colorRamp.CreateRamp(out createColorRamp);


            IFillSymbol fillSymbol = new SimpleFillSymbolClass();


            //for (int ii = 0; ii < pRows; ii++)
            //{
            //    for (int jj = 0; jj < pColumns; jj++)
            //    {
            //        if (isFlood[ii, jj] == true)
            //        {
            //            fillSymbol.Color = colorRamp.get_Color(0);
            //            pRClassRend.set_Symbol(0, fillSymbol as ISymbol);
            //            pRClassRend.set_Label(0, pRClassRend.get_Break(0).ToString("0.00"));
            //        }
            //        if (isFlood[ii, jj] == false)
            //        {
            //            fillSymbol.Color = colorRamp.get_Color(1);
            //            pRClassRend.set_Symbol(1, fillSymbol as ISymbol);
            //            pRClassRend.set_Label(1, pRClassRend.get_Break(1).ToString("0.00"));
            //        }
            //    }
            //}
            for (int i = 0; i < pRClassRend.ClassCount; i++)
            {


                fillSymbol.Color = colorRamp.get_Color(i);
                pRClassRend.set_Symbol(i, fillSymbol as ISymbol);
                pRClassRend.set_Label(i, pRClassRend.get_Break(i).ToString("0.00"));
            }
            pRasterLayer.Renderer = pRRend;
            axMapControl1.AddLayer(pRasterLayer);
             MessageBox.Show("ok");
        }
        //唯一值渲染
        public void UniqueValueRender(IRasterLayer rasterLayer, string renderfiled = "Value")
        {
            try
            {
                IRasterUniqueValueRenderer uniqueValueRenderer = new RasterUniqueValueRendererClass();
                IRasterRenderer pRasterRenderer = uniqueValueRenderer as IRasterRenderer;
                pRasterRenderer.Raster = rasterLayer.Raster;
                pRasterRenderer.Update();
                IUniqueValues uniqueValues = new UniqueValuesClass();
                IRasterCalcUniqueValues calcUniqueValues = new RasterCalcUniqueValuesClass();
                calcUniqueValues.AddFromRaster(rasterLayer.Raster, 0, uniqueValues);//iBand=0  
                IRasterRendererUniqueValues renderUniqueValues = uniqueValueRenderer as IRasterRendererUniqueValues;
                renderUniqueValues.UniqueValues = uniqueValues;
                uniqueValueRenderer.Field = renderfiled;
                /////////////////////////IColorRamp不包含FromColor和ToColor方法////////////////////
                //IRgbColor pFromColor = new RgbColorClass();
                //pFromColor.Red = 135;//天蓝色
                //pFromColor.Green = 206;
                //pFromColor.Blue = 235;
                //IRgbColor pToColor = new RgbColorClass();
                //pToColor.Red = 124;//草坪绿
                //pToColor.Green = 252;
                //pToColor.Blue = 0;
                IColorRamp colorRamp = new AlgorithmicColorRampClass();
                //colorRamp.FromColor = pFromColor;
                //colorRamp.ToColor = pToColor;


                colorRamp.Size = uniqueValues.Count;


                uniqueValueRenderer.HeadingCount = 1;
                uniqueValueRenderer.set_Heading(0, "All Data Value");
                uniqueValueRenderer.set_ClassCount(0, uniqueValues.Count);
                bool pOk;
                colorRamp.CreateRamp(out pOk);
                IRasterRendererColorRamp pRasterRendererColorRamp = uniqueValueRenderer as IRasterRendererColorRamp;
                pRasterRendererColorRamp.ColorRamp = colorRamp;
                for (int i = 0; i < uniqueValues.Count; i++)
                {
                    uniqueValueRenderer.AddValue(0, i, uniqueValues.get_UniqueValue(i));
                    uniqueValueRenderer.set_Label(0, i, uniqueValues.get_UniqueValue(i).ToString());
                    IFillSymbol fs = new SimpleFillSymbol();
                    fs.Color = colorRamp.get_Color(i);
                    uniqueValueRenderer.set_Symbol(0, i, fs as ISymbol);
                }
                pRasterRenderer.Update();
                rasterLayer.Renderer = pRasterRenderer;
                this.axMapControl1.AddLayer(rasterLayer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void reclass(IRaster pRaster, double weight)
        {
            //int trueCount = 0;
            //int falseCount = 0;
            IRasterProps rasterProps = (IRasterProps)pRaster;
            //设置栅格数据起始点
            IPnt pBlockSize = new Pnt();
            //这一句太耗内存记得改
            pBlockSize.SetCoords(rasterProps.Width, rasterProps.Height);
            //获取整个范围
            IPixelBlock pPixelBlock = pRaster.CreatePixelBlock(pBlockSize);
           //  IPixelBlock3 pPixelBlock2 = (IPixelBlock3)pRaster.CreatePixelBlock(pBlockSize);
            //左上点坐标
            IPnt tlp = new PntClass();
            tlp.SetCoords(1, 1);
            //读入栅格
            IRasterBandCollection pRasterBands = pRaster as IRasterBandCollection;
            for (int i = 0; i < pRasterBands.Count; i++)
            {
                IRasterBand pRasterBand = pRasterBands.Item(i);
                IRawPixels pRawPixels = pRasterBands.Item(i) as IRawPixels;
                pRawPixels.Read(tlp, pPixelBlock);
                //将pixel的值组成数组
                System.Array pSafeArray = pPixelBlock.get_SafeArray(i) as System.Array;

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
                        //if (isFlood[y, x] == true)
                        if (x < rasterProps.Width / 2)
                        {
                            pSafeArray.SetValue((Byte)1, x, y);

                            //pSafeArray[x, y] = 1;
                            //trueCount++;
                        }
                        else
                        {
                            pSafeArray.SetValue((Byte)0, x, y);
                            //falseCount++;
                        }
                    }
                }
                pPixelBlock.set_SafeArray(i, pSafeArray);
                //编辑raster,将更新的值写入raster中
                IRasterEdit rasterEdit = pRaster as IRasterEdit;

                rasterEdit.Write(tlp, pPixelBlock);
                rasterEdit.Refresh();
            }
            //IRasterBand pRasterBand = pRasterBands.Item(0);
            //IRawPixels pRawPixels = pRasterBands.Item(0) as IRawPixels;
            //pRawPixels.Read(tlp, pPixelBlock);
            ////将pixel的值组成数组
            //System.Array pSafeArray = pPixelBlock.get_SafeArray(0) as System.Array;
            
            //for (int y = 0; y < rasterProps.Height; y++)
            //{
            //    for (int x = 0; x < rasterProps.Width; x++)
            //    {
            //        //int value = Convert.ToInt32(pSafeArray.GetValue(x, y));
            //        //Byte value = Convert.ToByte(pSafeArray.GetValue(x, y));
            //        //if (value != 0 && isFlood[y, x] == false)
            //        //{
            //        //    pSafeArray.SetValue((Byte)(value * weight), x, y);
            //        //}
            //        //if (isFlood[y, x] == true)
            //        if (x<rasterProps.Width/2)
            //        {
            //            pSafeArray.SetValue((Byte)1, x, y);
                        
            //            //pSafeArray[x, y] = 1;
            //            //trueCount++;
            //        }
            //        else
            //        {
            //            pSafeArray.SetValue((Byte)0, x, y);
            //            //falseCount++;
            //        }
            //    }
            //}
            //pPixelBlock.set_SafeArray(0, pSafeArray);
            ////编辑raster,将更新的值写入raster中
            //IRasterEdit rasterEdit = pRaster as IRasterEdit;
           
            //rasterEdit.Write(tlp, pPixelBlock);
           // System.Runtime.InteropServices.Marshal.ReleaseComObject(rasterEdit);
         //   rasterEdit.Refresh();

            int Fcount = 0;
            //m_map.AddLayer
            //MessageBox.Show("ok");
            //double result = Convert.ToDouble(pSafeArray.GetValue(100, 99));
            this.axMapControl1.Refresh();


            //ILayer pLayer = axMapControl1.get_Layer(0);//假定第一个图层为Raster数据

            //IRasterLayer rasterLayer = (IRasterLayer)pLayer;
            // IRaster pRaster = rasterLayer.Raster;
           
            //reclass(pRaster, 4);

          //  axMapControl1.AddLayer();
            MessageBox.Show("ok");
        }

        public void ChangeRasterValue(IRaster2 pRasterDatset)
        {
            ILayer pLayer = axMapControl1.get_Layer(0);//假定第一个图层为Raster数据

            IRasterLayer rasterLayer = (IRasterLayer)pLayer;

            IRaster pRaster = rasterLayer.Raster;

           // IRaster2 pRaster2 = pRasterDatset.CreateFullRaster() as IRaster2;
            IRaster2 pRaster2 = pRaster as IRaster2;

            IPnt pPntBlock = new PntClass();

            pPntBlock.X = 128;

            pPntBlock.Y = 128;
            IRasterCursor pRasterCursor = pRaster2.CreateCursorEx(pPntBlock);
            IRasterEdit pRasterEdit = pRaster2 as IRasterEdit;

            if (pRasterEdit.CanEdit())
            {
                IRasterBandCollection pBands = pRasterDatset as IRasterBandCollection; IPixelBlock3 pPixelblock3 = null; int pBlockwidth = 0; int pBlockheight = 0; System.Array pixels; IPnt pPnt = null; object pValue;
                long pBandCount = pBands.Count;

                //获取Nodata 
                //IRasterProps pRasterPro = pRaster2 as IRasterProps; 

                //object pNodata = pRasterPro.NoDataValue; 

                do
                {
                    pPixelblock3 = pRasterCursor.PixelBlock as IPixelBlock3; pBlockwidth = pPixelblock3.Width; pBlockheight = pPixelblock3.Height;

                    for (int k = 0; k < pBandCount; k++)
                    {
                        pixels = (System.Array)pPixelblock3.get_PixelData(k); for (int i = 0; i < pBlockwidth; i++)
                        {
                            for (int j = 0; j < pBlockheight; j++)
                            {

                                pValue = pixels.GetValue(i, j);

                                if (Convert.ToInt32(pValue) == 0)
                                { //pixels.SetValue(Convert.ToByte(50), i, j); 
                                    pixels.SetValue(Convert.ToByte(0), i, j);
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
            }

            MessageBox.Show("变换完成！");

        }

       
        private void classy_click(object sender, EventArgs e)
        {
            ILayer pLayer = axMapControl1.get_Layer(0);//假定第一个图层为Raster数据

            IRasterLayer rasterLayer = (IRasterLayer)pLayer;

            funColorForRaster_Classify(rasterLayer);
        }

      

    }
}