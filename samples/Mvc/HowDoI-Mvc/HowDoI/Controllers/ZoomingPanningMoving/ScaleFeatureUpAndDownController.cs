using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Mvc;

namespace CSharp_HowDoISamples
{
    public partial class ZoomingPanningMovingController : Controller
    {
        //
        // GET: /ScaleFeatureUpAndDown/

        public ActionResult ScaleFeatureUpAndDown()
        {
            return View();
        }

        [MapActionFilter]
        public void Scale(Map map, GeoCollection<object> args)
        {
            string dir = args[0] as string;

            switch (dir)
            {
                case "up":
                    UpdateFeatureByScale(map, 20, true);
                    break;
                case "down":
                    UpdateFeatureByScale(map, 20, false);
                    break;
            }
        }

        private void UpdateFeatureByScale(Map map, double percentage, bool isScaleUp)
        {
            InMemoryFeatureLayer mapShapeLayer = (InMemoryFeatureLayer)((LayerOverlay)map.CustomOverlays["InMemoryFeatureLayer"]).Layers[0];
            mapShapeLayer.Open();
            mapShapeLayer.EditTools.BeginTransaction();
            if (isScaleUp)
            {
                mapShapeLayer.EditTools.ScaleUp("Rectangle", percentage);
            }
            else
            {
                mapShapeLayer.EditTools.ScaleDown("Rectangle", percentage);
            }
            mapShapeLayer.EditTools.CommitTransaction();
            mapShapeLayer.Close();
            ((LayerOverlay)map.CustomOverlays["InMemoryFeatureLayer"]).Redraw();
        }
    }
}
