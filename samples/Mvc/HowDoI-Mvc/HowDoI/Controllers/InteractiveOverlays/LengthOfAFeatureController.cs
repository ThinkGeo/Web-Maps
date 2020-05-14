using System.Collections.ObjectModel;
using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Layers;
using System;

namespace CSharp_HowDoISamples
{
    public partial class InteractiveOverlaysController : Controller
    {
        //
        // GET: /LengthOfAFeature/
        public ActionResult LengthOfAFeature()
        {
            return View();
        }

        [MapActionFilter]
        public void GetLengthOfFeature(Map map, GeoCollection<object> args)
        {
            PointShape point = new PointShape(Convert.ToDouble(args[0]), Convert.ToDouble(args[1]));

            FeatureLayer worldLayer = (FeatureLayer)map.StaticOverlay.Layers["RoadLayer"];
            InMemoryFeatureLayer streetLayer = (InMemoryFeatureLayer)map.DynamicOverlay.Layers["StreetLayer"];
            streetLayer.InternalFeatures.Clear();

            worldLayer.Open();
            Collection<Feature> selectedFeatures = worldLayer.QueryTools.GetFeaturesNearestTo(point, GeographyUnit.DecimalDegree, 1, new string[1] { "FENAME" });
            worldLayer.Close();

            map.Popups.Clear();

            CloudPopup popup = new CloudPopup("information");
            popup.Position = point;
            popup.AutoSize = true;

            string popupContentHtml = string.Empty;
            if (selectedFeatures.Count > 0)
            {
                LineBaseShape lineShape = (LineBaseShape)selectedFeatures[0].GetShape();
                double length = lineShape.GetLength(GeographyUnit.DecimalDegree, DistanceUnit.Meter);
                string contentHtml = "<span style='color:red'>{0}</span> has a length of <span style='color:red'>{1:N0}</span> meters.";
                string information = string.Format(contentHtml, selectedFeatures[0].ColumnValues["FENAME"].Trim(), length);
                popupContentHtml = "<div style='font-size:10px; font-family:verdana; padding:4px;'>" + information + "</div>";

                streetLayer.InternalFeatures.Add("Street", new Feature(lineShape));
            }
            popup.ContentHtml = popupContentHtml;
            map.Popups.Add(popup);
        }
    }
}
