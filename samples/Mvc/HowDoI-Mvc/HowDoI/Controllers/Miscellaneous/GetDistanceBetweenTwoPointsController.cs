/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Styles;

namespace CSharp_HowDoISamples
{
    public partial class MiscellaneousController : Controller
    {
        //
        // GET: /GetDistanceBetweenTwoPoints/

        public ActionResult GetDistanceBetweenTwoPoints()
        {
            Map map = new Map("Map1",
                 new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage),
                 new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage));
            map.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
            map.MapUnit = GeographyUnit.Meter;
            map.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            map.CurrentExtent = new RectangleShape(-14607343.5818934, 7371576.14679691, -6014592.08756057, 1910351.0222467);

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            map.CustomOverlays.Add(backgroundOverlay);

            InMemoryFeatureLayer pointShapeLayer = new InMemoryFeatureLayer();
            pointShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimplePointStyle(PointSymbolType.Circle, GeoColor.StandardColors.Red, 8);
            pointShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            InMemoryFeatureLayer lineShapeLayer = new InMemoryFeatureLayer();
            lineShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = new LineStyle(new GeoPen(GeoColor.StandardColors.Red, 3));
            lineShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay dynamicOverlay = new LayerOverlay("DynamicOverlay");
            dynamicOverlay.IsBaseOverlay = false;
            dynamicOverlay.TileType = TileType.SingleTile;
            dynamicOverlay.Layers.Add("pointShapeLayer", pointShapeLayer);
            dynamicOverlay.Layers.Add("lineShapeLayer", lineShapeLayer);
            map.CustomOverlays.Add(dynamicOverlay);

            map.Popups.Add(new CloudPopup("information") { AutoSize = true, IsVisible = false });

            return View(map);
        }

        [MapActionFilter]
        public void GetDistance(Map map, GeoCollection<object> args)
        {
            PointShape pointShape = new PointShape(string.Format("POINT ({0} {1})", args[0], args[1]));
            LayerOverlay dynamicOverlay = (LayerOverlay)map.CustomOverlays["DynamicOverlay"];
            InMemoryFeatureLayer pointShapeLayer = (InMemoryFeatureLayer)dynamicOverlay.Layers["pointShapeLayer"];
            Feature newPoint = new Feature(pointShape);
            InMemoryFeatureLayer lineShapeLayer = (InMemoryFeatureLayer)dynamicOverlay.Layers["lineShapeLayer"];

            if (Session["StartPoint"] == null)
            {
                pointShapeLayer.InternalFeatures.Clear();
                lineShapeLayer.InternalFeatures.Clear();
            }

            pointShapeLayer.InternalFeatures.Add(newPoint.Id, newPoint);

            CloudPopup tempPopup = (CloudPopup)map.Popups[0];
            string popupContentHtml = string.Empty; 
            if (Session["StartPoint"] != null)
            {
                PointShape startPoint = (PointShape)Session["StartPoint"];
                MultilineShape line = startPoint.GetShortestLineTo(pointShape, GeographyUnit.Meter);

                Feature lineFeature = new Feature(line);
                string distanceValue = String.Format("<span class='popup'>{0} Mile</span>", line.GetLength(GeographyUnit.Meter, DistanceUnit.Mile).ToString("N2"));
                lineShapeLayer.InternalFeatures.Add(lineFeature.Id, lineFeature);

                popupContentHtml = distanceValue;
                tempPopup.Position = pointShape;
                tempPopup.IsVisible = true;

                Session["StartPoint"] = null;
            }
            else
            {
                tempPopup.IsVisible = false;
                Session["StartPoint"] = pointShape;
            }

            tempPopup.ContentHtml = popupContentHtml;
        }
    }
}
