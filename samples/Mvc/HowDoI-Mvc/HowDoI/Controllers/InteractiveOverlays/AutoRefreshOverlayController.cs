using System;
using System.Collections.ObjectModel;
using System.Web.Mvc;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;

namespace CSharp_HowDoISamples
{
    public partial class InteractiveOverlaysController : Controller
    {
        //
        // GET: /AutoRefreshOverlay/

        public ActionResult AutoRefreshOverlay()
        {
            return View();
        }

        [MapActionFilter]
        public void RefreshMarkerOverlay(Map map, Collection<object> args)
        {
            InMemoryMarkerOverlay markerOverlay = (InMemoryMarkerOverlay)map.CustomOverlays["MarkerOverlay"];
            for (int i = 0; i < markerOverlay.FeatureSource.InternalFeatures.Count; i++)
            {
                PointShape point = markerOverlay.FeatureSource.InternalFeatures[i].GetShape() as PointShape;

                double lon = point.X + new Random(Guid.NewGuid().GetHashCode()).Next(-2000000, 2000000) / 5000.0;
                double lat = point.Y + new Random(Guid.NewGuid().GetHashCode()).Next(-2000000, 2000000) / 5000.0;

                markerOverlay.FeatureSource.InternalFeatures[i] = new Feature(lon, lat);

            }
        }
    }
}
