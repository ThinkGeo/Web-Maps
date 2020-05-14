using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Mvc;

namespace CSharp_HowDoISamples
{
    public partial class InteractiveOverlaysController : Controller
    {
        public ActionResult SetBackgroundColor()
        {
            return View();
        }

        [MapActionFilter]
        public void UpdateBackgound(Map map, GeoCollection<object> args)
        {
            GeoColor backcolor = GeoColor.FromHtml(args[0].ToString());
            map.MapBackground = new GeoSolidBrush(backcolor);
        }
    }
}