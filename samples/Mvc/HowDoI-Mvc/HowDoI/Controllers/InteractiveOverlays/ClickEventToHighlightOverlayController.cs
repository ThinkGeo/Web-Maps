/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;

namespace CSharp_HowDoISamples
{
    public partial class InteractiveOverlaysController : Controller
    {
        //
        // GET: /ClickEventToHighlightOverlay/

        public ActionResult ClickEventToHighlightOverlay()
        {
            Map map = new Map("Map1", new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage), 510);
            map.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
            map.MapUnit = GeographyUnit.Meter;
            map.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            map.CurrentExtent = new RectangleShape(-14607343.5818934, 7371576.14679691, -6014592.08756057, 1910351.0222467);

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            map.CustomOverlays.Add(backgroundOverlay);

            Feature feature = new Feature(new RectangleShape(-12269634.2752346, 6446275.84101716, -8908898.84818568, 3503549.84350437));
            map.HighlightOverlay.HighlightStyle = map.HighlightOverlay.Style;
            map.HighlightOverlay.Features.Add("feature", feature);

            map.Popups.Add(new CloudPopup("Popup") { AutoSize = true });

            return View(map);
        }

        [MapActionFilter]
        public void ClickEvent(Map map, GeoCollection<object> args)
        {
            double x = Convert.ToDouble(args[0]);
            double y = Convert.ToDouble(args[1]);

            PointShape location = new PointShape(x, y);


            map.Popups.Clear();

            CloudPopup popup = new CloudPopup("information");
            popup.Position = location;
            popup.AutoSize = true;

            popup.ContentHtml = "<span class='popup'>Inside the area</span>";
            map.Popups.Add(popup);
        }
    }
}
