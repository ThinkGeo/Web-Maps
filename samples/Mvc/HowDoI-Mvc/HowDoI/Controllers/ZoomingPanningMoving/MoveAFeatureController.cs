using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;

namespace CSharp_HowDoISamples
{
    public partial class ZoomingPanningMovingController : Controller
    {
        //
        // GET: /MoveAFeature/

        public ActionResult MoveAFeature()
        {
            return View();
        }

        [MapActionFilter]
        public void Move(Map map, GeoCollection<object> args)
        {
            string dir = args[0] as string;
            switch (dir)
            {
                case "right":
                    TranslateByOffset(map, 1000000, 0);
                    break;
                case "left":
                    TranslateByOffset(map, -1000000, 0);
                    break;
                case "up":
                    TranslateByOffset(map, 0, 1000000);
                    break;
                case "down":
                    TranslateByOffset(map, 0, -1000000);
                    break;
            }
        }

        private void TranslateByOffset(Map map, double xOffset, double yOffset)
        {
            InMemoryFeatureLayer mapShapeLayer = (InMemoryFeatureLayer)((LayerOverlay)map.CustomOverlays["mapShapeLayer"]).Layers[0];
            mapShapeLayer.Open();
            mapShapeLayer.EditTools.BeginTransaction();
            mapShapeLayer.EditTools.TranslateByOffset("MutlipointShape", xOffset, yOffset, GeographyUnit.Meter, DistanceUnit.Meter);
            mapShapeLayer.EditTools.CommitTransaction();
            mapShapeLayer.Close();
            ((LayerOverlay)map.CustomOverlays[1]).Redraw();
        }
    }
}
