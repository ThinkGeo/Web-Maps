using System.Collections.ObjectModel;
using System.Globalization;
using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;

namespace CSharp_HowDoISamples
{
    public partial class SpatialFunctionsController : Controller
    {
        //
        // GET: /ZoomInToAFeatureClicked/

        public ActionResult ZoomInToAFeatureClicked()
        {
            return View();
        }

        [MapActionFilter]
        public string ZoomToShape(Map map, GeoCollection<object> args)
        {
            PointShape position = new PointShape(double.Parse(args[0].ToString()), double.Parse(args[1].ToString()));

            LayerOverlay staticOverlay = (LayerOverlay)map.CustomOverlays["CountryOverlay"];
            ShapeFileFeatureLayer worldLayer = (ShapeFileFeatureLayer)staticOverlay.Layers["WorldLayer"];

            LayerOverlay dynamicOverlay = (LayerOverlay)map.CustomOverlays["HighlightOverlay"];
            InMemoryFeatureLayer highlightLayer = (InMemoryFeatureLayer)dynamicOverlay.Layers["HighlighLayer"];

            highlightLayer.InternalFeatures.Clear();
            worldLayer.Open();
            Collection<Feature> features = worldLayer.QueryTools.GetFeaturesContaining(position, ReturningColumnsType.AllColumns);
            worldLayer.Close();

            string dataTableHtml = string.Empty;
            RectangleShape extent = map.CurrentExtent;
            if (features.Count > 0)
            {
                highlightLayer.InternalFeatures.Add(features[0].Id, features[0]);
                map.CurrentExtent = ExtentHelper.GetDrawingExtent(features[0].GetBoundingBox(), (float)map.WidthInPixels, (float)map.HeightInPixels);

                extent = features[0].GetBoundingBox();

                string content = string.Empty;
                foreach (string key in features[0].ColumnValues.Keys)
                {
                    content += string.Format("<tr><td style=\"border:1px solid #cccccc;\">{0}</td><td style=\"border:1px solid #cccccc;\">{1}</td></tr>", key, features[0].ColumnValues[key]);
                }
                dataTableHtml = string.Format("<table id=\"dataInfo\" cellspacing=\"0\" style=\"border: 1px solid #cccccc;\"><tr><td style=\"border: 1px solid #cccccc; background: #bbd7ed;\">Column Name</td><td style=\"border: 1px solid #cccccc; background: #bbd7ed;\">Value</td></tr>{0}</table>", content);
            }

            string extentString = string.Format(CultureInfo.InvariantCulture, "{0},{1},{2},{3}", extent.LowerLeftPoint.X, extent.LowerLeftPoint.Y, extent.UpperRightPoint.X, extent.UpperRightPoint.Y);

            return extentString + "|" + dataTableHtml;
        }

    }
}
