using System.Collections.ObjectModel;
using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Layers;

namespace CSharp_HowDoISamples
{
    public partial class InteractiveOverlaysController : Controller
    {
        //
        // GET: /TrackShapeFinishedEvent/

        public ActionResult TrackShapeFinishedEvent()
        {
            return View();
        }

        [MapActionFilter]
        public void UpdateEditedShape(Map map, GeoCollection<object> args)
        {
            string json = args[0] as string;
            Collection<Feature> features = MapHelper.ConvertJsonToFeatures(json);

            LayerOverlay shapeOverlay = map.CustomOverlays["ShapeOverlay"] as LayerOverlay;
            InMemoryFeatureLayer shapeLayer = shapeOverlay.Layers[0] as InMemoryFeatureLayer;

            foreach (Feature feature in features)
            {
                shapeLayer.InternalFeatures.Add(feature.Id, feature);
            }

            map.EditOverlay.Features.Clear();
        }
    }
}
