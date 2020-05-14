/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Styles;

namespace CSharp_HowDoISamples
{
    public partial class PrintingController : Controller
    {
        public ActionResult PrintAMap()
        {
            Map map1 = new Map("Map1", new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage), 510);
            map1.MapUnit = GeographyUnit.Meter;
            map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            map1.CurrentExtent = new RectangleShape(-14607343.5818934, 7371576.14679691, -6014592.08756057, 1910351.0222467);

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            map1.CustomOverlays.Add(backgroundOverlay);

            ShapeFileFeatureLayer usStatesLayer = new ShapeFileFeatureLayer(Server.MapPath("~/App_Data/STATES.SHP"));
            usStatesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.StandardColors.Transparent, GeoColor.FromArgb(255, 156, 155, 154), 1);
            usStatesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.StartCap = DrawingLineCap.Round;
            usStatesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay staticOverlay = new LayerOverlay();
            staticOverlay.IsBaseOverlay = false;
            staticOverlay.Layers.Add(usStatesLayer);

            map1.CustomOverlays.Add(staticOverlay);

            return View(map1);
        }
    }
}