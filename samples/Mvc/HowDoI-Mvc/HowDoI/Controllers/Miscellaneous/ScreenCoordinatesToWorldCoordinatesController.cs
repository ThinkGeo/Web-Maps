using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;

namespace CSharp_HowDoISamples
{
    public partial class MiscellaneousController : Controller
    {
        //
        // GET: /ScreenCoordinatesToWorldCoordinates/

        public ActionResult ScreenCoordinatesToWorldCoordinates()
        {
            return View();
        }

        [MapActionFilter]
        public string ConvertCoordinate(Map map, GeoCollection<object> args)
        {
            string result = string.Empty;
            float screenX, screenY;
            string x = args[0].ToString();
            string y = args[1].ToString();
            if (float.TryParse(x, out screenX) && float.TryParse(y, out screenY))
            {
                PointShape worldCoordinate = map.ToWorldCoordinate(screenX, screenY);
                InMemoryMarkerOverlay inMemoryMarkerOverlay = (InMemoryMarkerOverlay)map.CustomOverlays["InMemoryMarkerOverlay"];
                inMemoryMarkerOverlay.FeatureSource.InternalFeatures.Clear();
                inMemoryMarkerOverlay.FeatureSource.InternalFeatures.Add("marker", new Feature(worldCoordinate));

                result = string.Format("The converted world coordinates are <br/><span style='color:red;font-size:13;font-weight:bolder;'>({0}, {1})</span>.", worldCoordinate.X, worldCoordinate.Y);
            }
            return result;
        }
    }
}
