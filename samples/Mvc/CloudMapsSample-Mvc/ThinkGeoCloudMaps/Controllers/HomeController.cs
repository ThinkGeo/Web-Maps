/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System.Web.Mvc;
using System.Web.UI.WebControls;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;

namespace ThinkGeoCloudMapsSample
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Map map = new Map("Map1", new Unit(100, UnitType.Percentage), new Unit(100, UnitType.Percentage));

            map.MapUnit = GeographyUnit.Meter;
            map.ZoomLevelSet =new ThinkGeoCloudMapsZoomLevelSet();
            map.MapTools.OverlaySwitcher.Enabled = true;
            map.MapTools.OverlaySwitcher.BaseOverlayTitle = "ThinkGeo Cloud Maps:";
            map.MapTools.OverlaySwitcher.BackgroundColor = GeoColor.StandardColors.DarkSlateGray;

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay lightMap = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            lightMap.Name = "Light";
            lightMap.WrapDateline = WrapDatelineMode.WrapDateline;
            lightMap.MapType = ThinkGeoCloudRasterMapsMapType.Light;
            map.CustomOverlays.Add(lightMap);

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay darkMap = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            darkMap.Name = "Dark";
            darkMap.WrapDateline = WrapDatelineMode.WrapDateline;
            darkMap.MapType = ThinkGeoCloudRasterMapsMapType.Dark;
            map.CustomOverlays.Add(darkMap);

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay aerialMap = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            aerialMap.Name = "Aerial";
            aerialMap.WrapDateline = WrapDatelineMode.WrapDateline;
            aerialMap.MapType = ThinkGeoCloudRasterMapsMapType.Aerial;
            map.CustomOverlays.Add(aerialMap);

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay hybridMap = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            hybridMap.Name = "Hybrid";
            hybridMap.WrapDateline = WrapDatelineMode.WrapDateline;
            hybridMap.MapType = ThinkGeoCloudRasterMapsMapType.Hybrid;
            map.CustomOverlays.Add(hybridMap);

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay transparentBackgroundMap = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            transparentBackgroundMap.Name = "TransparentBackground";
            transparentBackgroundMap.WrapDateline = WrapDatelineMode.WrapDateline;
            transparentBackgroundMap.MapType = ThinkGeoCloudRasterMapsMapType.TransparentBackground;
            map.CustomOverlays.Add(transparentBackgroundMap);

            map.CurrentExtent = new RectangleShape(-13086298.60, 7339062.72, -8111177.75, 2853137.62);

            return View(map);
        }
    }
}