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
        // GET: /DrawEditShapes/

        public ActionResult DrawEditShapes()
        {
            return View();
        }

        [MapActionFilter]
        public void SaveMap(Map map, GeoCollection<object> args)
        {
            Collection<Feature> features = MapHelper.ConvertJsonToFeatures(args[0].ToString());

            LayerOverlay dynamicOverlay = (LayerOverlay)map.CustomOverlays["DynamicOverlay"];
            InMemoryFeatureLayer shapeLayer = (InMemoryFeatureLayer)dynamicOverlay.Layers["shapeLayer"];

            foreach (Feature feature in features)
            {
                if (!shapeLayer.InternalFeatures.Contains(feature.Id))
                {
                    shapeLayer.InternalFeatures.Add(feature.Id, feature);
                }
            }

            map.EditOverlay.Features.Clear();
        }


        [MapActionFilter]
        public string EditShape(Map map, GeoCollection<object> args)
        {
            LayerOverlay dynamicOverlay = (LayerOverlay)map.CustomOverlays["DynamicOverlay"];
            InMemoryFeatureLayer shapeLayer = (InMemoryFeatureLayer)dynamicOverlay.Layers["shapeLayer"];

            string featuresJson = MapHelper.ConvertFeaturesToJson(shapeLayer.InternalFeatures);

            shapeLayer.InternalFeatures.Clear();

            return featuresJson;
        }
    }
}
