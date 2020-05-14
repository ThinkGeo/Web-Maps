using System;
using System.Globalization;
using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;

namespace CSharp_HowDoISamples
{
    public partial class SpatialFunctionsController : Controller
    {
        private static AreaBaseShape areaBaseShape;

        //
        // GET: /UseMapSimplification/

        public ActionResult UseMapSimplification()
        {
            return View();
        }

        [MapActionFilter]
        public void SimplifyClick(Map map, GeoCollection<object> args)
        {
            if (null == areaBaseShape)
            { 
                areaBaseShape = ((InMemoryFeatureLayer)map.StaticOverlay.Layers["SimplificationLayer"]).InternalFeatures[0].GetShape() as AreaBaseShape;
            }
            string toleranceString = args[0] as string;
            string simplificationTypeString = args[1] as string;
            InMemoryFeatureLayer simplificationLayer = (InMemoryFeatureLayer)map.StaticOverlay.Layers["SimplificationLayer"];

            double tolerance = Convert.ToDouble(toleranceString, CultureInfo.InvariantCulture);
            SimplificationType simplificationType = SimplificationType.DouglasPeucker;
            switch (simplificationTypeString)
            {
                case "TopologyPreserving":
                    simplificationType = SimplificationType.TopologyPreserving;
                    break;
                case "DouglasPeucker":
                    simplificationType = SimplificationType.DouglasPeucker;
                    break;
            }

            MultipolygonShape multipolygonShape = areaBaseShape.Simplify(tolerance, simplificationType);
            simplificationLayer.InternalFeatures.Clear();
            simplificationLayer.InternalFeatures.Add(new Feature(multipolygonShape));
        }
    }
}
