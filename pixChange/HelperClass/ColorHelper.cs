using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Display;

namespace pixChange.HelperClass
{
 public  class ColorHelper
    {
     public static IRgbColor GetRGBColor(int red, int green, int blue)
     {
         IRgbColor rGBColor = new RgbColor();
         rGBColor.Red = red;
         rGBColor.Green = green;
         rGBColor.Blue = blue;
         return rGBColor;
     }
    }
}
