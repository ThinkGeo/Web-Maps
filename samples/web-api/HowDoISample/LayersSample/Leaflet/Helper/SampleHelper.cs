using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ThinkGeo.Core;

namespace ThinkGeo.MapSuite.Layers
{
    public static class SampleHelper
    {
        private static readonly string baseDirectory = null;

        static SampleHelper()
        {
            baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "App_Data");
        }

        /// <summary>
        /// Get Features from XML file by layer type.
        /// </summary>
        /// <param name="layerType">layer ype :MapShapeLayer,CustomizedLayer,InMemoryLayer</param>
        /// <returns></returns>
        public static Collection<Feature> GetFeatures(string layerType)
        {
            Collection<Feature> features = new Collection<Feature>();
            XElement layerData = XElement.Load(Path.Combine(baseDirectory, "LayerData.xml"));
            XElement featureElements = layerData.Elements("Layer").FirstOrDefault(a => a.Attribute("type").Value.Contains(layerType));
            if (featureElements != null)
            {
                foreach (var featureItem in featureElements.Elements("Feature"))
                {
                    Feature feature = new Feature(featureItem.Value);
                    XAttribute featureAttribute = featureItem.FirstAttribute;
                    feature.ColumnValues[featureAttribute.Name.ToString()] = featureAttribute.Value;

                    features.Add(feature);
                }
            }
            return features;
        }
    }
}