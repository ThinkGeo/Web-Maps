using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace TopologyValidation
{
    public static class TopologyHelper
    {
        private static readonly XElement toolElement;

        static TopologyHelper()
        {
            toolElement = XElement.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data/Source.xml"));
        }

        /// <summary>
        /// Gets source data by Id.
        /// </summary>
        public static SourceDataItem GetSourceDataById(string id)
        {
            SourceDataItem sourceData = new SourceDataItem();
            sourceData.Id = id;
            XElement topologyElement = toolElement.Elements("Topology").FirstOrDefault(a => a.Attribute("id").Value.Contains(id));

            if (topologyElement != null)
            {
                var shapesWkt = topologyElement.Elements("FirstInputShapes").Elements("WKT");

                if (shapesWkt != null)
                {
                    foreach (var item in shapesWkt)
                    {
                        sourceData.FirstInputFeatures.Add(new Feature(item.Value));
                    }
                }

                var secondShapesWkt = topologyElement.Elements("SecondInputShapes").Elements("WKT");
                if (secondShapesWkt != null)
                {
                    foreach (var item in secondShapesWkt)
                    {
                        sourceData.SecondInputFeatures.Add(new Feature(item.Value));
                    }
                }

                var comment = topologyElement.Element("Description");
                if (comment != null)
                {
                    sourceData.Comment = comment.Value;
                }
            }

            return sourceData;
        }
    }
}
