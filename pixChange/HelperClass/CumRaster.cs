using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.DataSourcesRaster;
using ESRI.ArcGIS.Geometry;

namespace pixChange.HelperClass
{
    public class CumRaster
    {
        /// <summary>
        ///栅格行数
        /// </summary>
        public int dHeight { get; set; }
        /// <summary>
        /// 栅格列数
        /// </summary>
        public int dWidth { get; set; }
        /// <summary>
        /// 栅格宽度
        /// </summary>
        public double dx { get; set; }
        /// <summary>
        /// 栅格高度
        /// </summary>
        public double dy { get; set; }
        /// <summary>
        /// 栅格范围
        /// </summary>
        public IEnvelope extent { get; set; }
       /// <summary>
       /// 波段信息
       /// </summary>
        public IRasterBandCollection pRsBandCol { set; get; }

       

    }
}
