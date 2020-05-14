using System.Collections.ObjectModel;
using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Layers;

namespace CSharp_HowDoISamples
{
    public partial class SpatialFunctionsController : Controller
    {
        //
        // GET: /ExecuteSpatialQuery/

        public ActionResult ExecuteSpatialQuery()
        {
            return View();
        }

        [MapActionFilter]
        public void SpatialQuery(Map map, GeoCollection<object> args)
        {
            LayerOverlay staticOverlay = (LayerOverlay)map.CustomOverlays["StaticOverlay"];
            ShapeFileFeatureLayer worldLayer = (ShapeFileFeatureLayer)staticOverlay.Layers["WorldLayer"];
            InMemoryFeatureLayer rectangleLayer = (InMemoryFeatureLayer)staticOverlay.Layers["RectangleLayer"];
            InMemoryFeatureLayer spatialQueryResultLayer = (InMemoryFeatureLayer)staticOverlay.Layers["SpatialQueryResultLayer"];

            Feature rectangleFeature = rectangleLayer.InternalFeatures["Rectangle"];
            Collection<Feature> spatialQueryResults;
            string spatialQueryType = args[0].ToString();
            worldLayer.Open();
            switch (spatialQueryType)
            {
                case "Within":
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesWithin(rectangleFeature, new string[0]);
                    break;
                case "Containing":
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesContaining(rectangleFeature, new string[0]);
                    break;
                case "Disjointed":
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesDisjointed(rectangleFeature, new string[0]);
                    break;
                case "Intersecting":
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesIntersecting(rectangleFeature, new string[0]);
                    break;
                case "Overlapping":
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesOverlapping(rectangleFeature, new string[0]);
                    break;
                case "TopologicalEqual":
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesTopologicalEqual(rectangleFeature, new string[0]);
                    break;
                case "Touching":
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesTouching(rectangleFeature, new string[0]);
                    break;
                default:
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesWithin(rectangleFeature, new string[0]);
                    break;
            }
            worldLayer.Close();

            spatialQueryResultLayer.InternalFeatures.Clear();
            spatialQueryResultLayer.Open();
            foreach (Feature feature in spatialQueryResults)
            {
                spatialQueryResultLayer.InternalFeatures.Add(feature.Id, feature);
            }
            spatialQueryResultLayer.Close();
            staticOverlay.Redraw();
        }
    }
}
