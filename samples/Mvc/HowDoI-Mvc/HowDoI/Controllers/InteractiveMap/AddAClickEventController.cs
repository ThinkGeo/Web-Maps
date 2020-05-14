/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;

namespace CSharp_HowDoISamples
{
    public partial class InteractiveMapController : Controller
    {
        //
        // GET: /AddAClickEvent/

        public ActionResult AddAClickEvent()
        {
            Map map = new Map("Map1", new Unit(100, UnitType.Percentage), 510);
            map.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
            map.MapUnit = GeographyUnit.Meter;
            map.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            map.CurrentExtent = new RectangleShape(-13086298.60, 7339062.72, -8111177.75, 2853137.62);

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            map.CustomOverlays.Add(backgroundOverlay);

            InMemoryMarkerOverlay markerOverlay = new InMemoryMarkerOverlay("MarkerOverlay");
            markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.ImageWidth = 21;
            markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.ImageHeight = 25;
            markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.ImageOffsetX = -10.5f;
            markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.ImageOffsetY = -25f;
            markerOverlay.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            map.CustomOverlays.Add(markerOverlay);

            return View(map);
        }

        [MapActionFilter]
        public void ClickEvent(Map map, GeoCollection<object> args)
        {
            PointShape position = new PointShape(Convert.ToDouble(args[0]), Convert.ToDouble(args[1]));

            InMemoryMarkerOverlay markerOverlay = (InMemoryMarkerOverlay)map.CustomOverlays["MarkerOverlay"];
            markerOverlay.FeatureSource.InternalFeatures.Add("marker" + Guid.NewGuid().ToString(), new Feature(position));
        }
    }
}
