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

namespace CSharp_HowDoISamples
{
    public partial class BackgroundMapsController : Controller
    {
        public ActionResult DisplayASimpleMap()
        {
            Map map = new Map("Map1",
                new Unit(100, UnitType.Percentage),
                new Unit(100, UnitType.Percentage));

            map.MapUnit = GeographyUnit.Meter;
            map.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            map.CurrentExtent = new RectangleShape(-13939426.6371, 6701997.4056, -7812401.86, 2626987.386962);

            map.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
            // Please input your ThinkGeo Cloud API Key to enable the background map.
            map.CustomOverlays.Add(new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key"));

            return View(map);
        }
    }
}