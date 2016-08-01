using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

namespace pixChange.HelperClass
{
public    class LayerMange
    {
    

/// <summary>
          /// 创建要素类  现在暂时没用 suchuan
          /// </summary>
          /// <param name="pObject">IWorkspace或者IFeatureDataset对象</param>
          /// <param name="pName">要素类名称</param>
         /// <param name="pSpatialReference">空间参考</param>
         /// <param name="pFeatureType">要素类型</param>
         /// <param name="pGeometryType">几何类型</param>
         /// <param name="pFields">字段集</param>
         /// <param name="pUidClsId">CLSID值</param>
         /// <param name="pUidClsExt">EXTCLSID值</param>
             /// <param name="pConfigWord">配置信息关键词</param>
         /// <returns>返回IFeatureClass</returns>
         public static IFeatureClass CreateFeatureClass(object pObject, string pName, ISpatialReference pSpatialReference, esriFeatureType pFeatureType,
                                        esriGeometryType pGeometryType, IFields pFields, UID pUidClsId, UID pUidClsExt, string pConfigWord)
         {
           #region 错误检测
           if (pObject == null)
           {
                throw (new Exception("[pObject] 不能为空!"));
            }
            if (!((pObject is IFeatureWorkspace) || (pObject is IFeatureDataset)))
            {
                 throw (new Exception("[pObject] 必须为IFeatureWorkspace 或者 IFeatureDataset"));
             }
             if (pName.Length == 0)
             {
                 throw (new Exception("[pName] 不能为空!"));
             }
            if ((pObject is IWorkspace) && (pSpatialReference == null))
             {
                 throw (new Exception("[pSpatialReference] 不能为空(对于单独的要素类)"));
           }
           #endregion

            #region pUidClsID字段为空时
             if (pUidClsId == null)
            {
               pUidClsId = new UIDClass();
                 switch (pFeatureType)
               {
                    case (esriFeatureType.esriFTSimple):
                       if (pGeometryType == esriGeometryType.esriGeometryLine)
                           pGeometryType = esriGeometryType.esriGeometryPolyline;
                       pUidClsId.Value = "{52353152-891A-11D0-BEC6-00805F7C4268}";
                      break;
                    case (esriFeatureType.esriFTSimpleJunction):
                       pGeometryType = esriGeometryType.esriGeometryPoint;
                      pUidClsId.Value = "{CEE8D6B8-55FE-11D1-AE55-0000F80372B4}";
                      break;
                   case (esriFeatureType.esriFTComplexJunction):
                        pUidClsId.Value = "{DF9D71F4-DA32-11D1-AEBA-0000F80372B4}";
                       break;
                    case (esriFeatureType.esriFTSimpleEdge):
                        pGeometryType = esriGeometryType.esriGeometryPolyline;
                      pUidClsId.Value = "{E7031C90-55FE-11D1-AE55-0000F80372B4}";
                         break;
                   case (esriFeatureType.esriFTComplexEdge):
                       pGeometryType = esriGeometryType.esriGeometryPolyline;
                        pUidClsId.Value = "{A30E8A2A-C50B-11D1-AEA9-0000F80372B4}";
                       break;
                   case (esriFeatureType.esriFTAnnotation):
                         pGeometryType = esriGeometryType.esriGeometryPolygon;
                         pUidClsId.Value = "{E3676993-C682-11D2-8A2A-006097AFF44E}";
                        break;
                   case (esriFeatureType.esriFTDimension):
                       pGeometryType = esriGeometryType.esriGeometryPolygon;
                        pUidClsId.Value = "{496764FC-E0C9-11D3-80CE-00C04F601565}";
                         break;
               }
           }
            #endregion

            #region pUidClsExt字段为空时
            if (pUidClsExt == null)
            {
               switch (pFeatureType)
               {
                    case esriFeatureType.esriFTAnnotation:
                       pUidClsExt = new UIDClass();
                         pUidClsExt.Value = "{24429589-D711-11D2-9F41-00C04F6BC6A5}";
                         break;
                     case esriFeatureType.esriFTDimension:
                       pUidClsExt = new UIDClass();
                         pUidClsExt.Value = "{48F935E2-DA66-11D3-80CE-00C04F601565}";
                       break;
               }
           }
           #endregion
 
         #region 字段集合为空时
             if (pFields == null)
            {
                //实倒化字段集合对象
                pFields = new FieldsClass();
                IFieldsEdit tFieldsEdit = (IFieldsEdit)pFields;
 
                //创建几何对象字段定义
                IGeometryDef tGeometryDef = new GeometryDefClass();
                IGeometryDefEdit tGeometryDefEdit = tGeometryDef as IGeometryDefEdit;

               //指定几何对象字段属性值
               tGeometryDefEdit.GeometryType_2 = pGeometryType;
                tGeometryDefEdit.GridCount_2 = 1;
                tGeometryDefEdit.set_GridSize(0, 1000);
               if (pObject is IWorkspace)
                {
                    tGeometryDefEdit.SpatialReference_2 = pSpatialReference;
                }

                //创建OID字段
                IField fieldOID = new FieldClass();
                IFieldEdit fieldEditOID = fieldOID as IFieldEdit;
               fieldEditOID.Name_2 = "OBJECTID";
                fieldEditOID.AliasName_2 = "OBJECTID";
             fieldEditOID.Type_2 = esriFieldType.esriFieldTypeOID;               tFieldsEdit.AddField(fieldOID);

                //创建几何字段
                IField fieldShape = new FieldClass();
                IFieldEdit fieldEditShape = fieldShape as IFieldEdit;
                fieldEditShape.Name_2 = "SHAPE";
                fieldEditShape.AliasName_2 = "SHAPE";
               fieldEditShape.Type_2 = esriFieldType.esriFieldTypeGeometry;
                fieldEditShape.GeometryDef_2 = tGeometryDef;
                tFieldsEdit.AddField(fieldShape);
            }
            #endregion

            //几何对象字段名称         
             string strShapeFieldName = "";
           for (int i = 0; i < pFields.FieldCount; i++)
           {
                if (pFields.get_Field(i).Type == esriFieldType.esriFieldTypeGeometry)
                {
                   strShapeFieldName = pFields.get_Field(i).Name;
                    break;
                }
           }

            if (strShapeFieldName.Length == 0)
            {
               throw (new Exception("字段集中找不到几何对象定义"));
            }

            IFeatureClass tFeatureClass = null;
            if (pObject is IWorkspace)
            {
                //创建独立的FeatureClass
                IWorkspace tWorkspace = pObject as IWorkspace;
               IFeatureWorkspace tFeatureWorkspace = tWorkspace as IFeatureWorkspace;
                tFeatureClass = tFeatureWorkspace.CreateFeatureClass(pName, pFields, pUidClsId, pUidClsExt, pFeatureType, strShapeFieldName, pConfigWord);
            }
            else if (pObject is IFeatureDataset)
            {
                //在要素集中创建FeatureClass
                IFeatureDataset tFeatureDataset = (IFeatureDataset)pObject;
               tFeatureClass = tFeatureDataset.CreateFeatureClass(pName, pFields, pUidClsId, pUidClsExt, pFeatureType, strShapeFieldName, pConfigWord);
            }

            return tFeatureClass;
        }

    //根据图层名找到图层
    public static ILayer RetuenLayerByLayerNameLayer(IMapControl3 mainMap,string layerName)
    {
        ILayer player = null;
        ILayer temp = null;
        int layerCount = mainMap.LayerCount;
        for (int i = 0; i < layerCount; i++)
        {
            temp = mainMap.get_Layer(i);
            if (temp is GroupLayer)
            {
                
            }
            else if (temp.Name == layerName)
            {
                player = temp;
            }
        }
        return player;

    }
    /// <summary>
    /// 根据图层名称获得IFeatureLayer
    /// tips：  后面写成泛型 suchuan
    /// </summary>
    /// <param name="mapControl"></param>
    /// <param name="layerName"></param>
    /// <returns></returns>
    public static IFeatureLayer ReturnFeatureLayerFromName(AxMapControl mapControl, string layerName)
    {
        IFeatureLayer layer = null;
        for (int i = 0; i < mapControl.LayerCount; i++)
        {
            ILayer layers = mapControl.get_Layer(i);
            if (layers is GroupLayer || layers is ICompositeLayer)   //判断是否是groupLayer
            {
                layer = getSubLayer(layers, layerName);  //递归的思想
                if (layer != null)
                {
                    break;
                }
            }
            else
            {
                if (mapControl.get_Layer(i).Name.Equals(layerName))
                {
                    layer = layers as IFeatureLayer;
                    break;
                }
            }
        }
        return layer;
    }

    //从groupLayer中查找FeatureLayer

    public static IFeatureLayer getSubLayer(ILayer layers, string layerName)
    {
        IFeatureLayer l = null;
        ICompositeLayer compositeLayer = layers as ICompositeLayer;
        for (int i = 0; i < compositeLayer.Count; i++)
        {
            ILayer layer = compositeLayer.Layer[i];   //递归
            if (layer is GroupLayer || layer is ICompositeLayer)
            {
                l = getSubLayer(layer, layerName);
                if (l != null)
                {
                    break;
                }
            }
            else
            {
                while (layer.Name.Equals(layerName))
                {
                    l = layer as IFeatureLayer;
                    break;
                }
            }
        }
        return l;
    }


    }
}
