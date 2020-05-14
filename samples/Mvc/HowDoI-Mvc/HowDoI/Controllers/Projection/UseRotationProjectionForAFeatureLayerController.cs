using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Layers;

namespace CSharp_HowDoISamples
{
    public partial class ProjectionController : Controller
    {
        //
        // GET: /UseRotationProjectionForAFeatureLayer/

        public ActionResult UseRotationProjectionForAFeatureLayer()
        {
            return View();
        }

        [MapActionFilter]
        public void RotateCounterclockwise(Map map, GeoCollection<object> args)
        {
            RotationProjection rotateProjection = (RotationProjection)((FeatureLayer)map.StaticOverlay.Layers[0]).FeatureSource.Projection;
            rotateProjection.Angle += 45;

            //Map1.StaticOverlay.ClientCache.CacheId = rotateProjection.Angle.ToString();
            //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path + "/" + rotateProjection.Angle.ToString());
            map.StaticOverlay.Redraw();
        }

        [MapActionFilter]
        public void RotateClockwise(Map map, GeoCollection<object> args)
        {
            RotationProjection rotateProjection = (RotationProjection)((FeatureLayer)map.StaticOverlay.Layers[0]).FeatureSource.Projection;
            rotateProjection.Angle -= 45;

            //Map1.StaticOverlay.ClientCache.CacheId = rotateProjection.Angle.ToString();
            //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path + "/" + rotateProjection.Angle.ToString());
            map.StaticOverlay.Redraw();
        }
    }
}
