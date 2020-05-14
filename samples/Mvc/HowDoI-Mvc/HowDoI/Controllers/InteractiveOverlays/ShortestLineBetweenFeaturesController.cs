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
        // GET: /ShortestLineBetweenFeatures/

        public ActionResult ShortestLineBetweenFeatures()
        {
            return View();
        }

        [MapActionFilter]
        public void ShortestLineFeature(Map map, GeoCollection<object> args)
        {
            if (null != map)
            {
                InMemoryFeatureLayer mapShapeLayer = (InMemoryFeatureLayer)((LayerOverlay)map.CustomOverlays[1]).Layers["InMemoryFeatureLayer"];
                InMemoryFeatureLayer shortestLineLayer = (InMemoryFeatureLayer)((LayerOverlay)map.CustomOverlays[1]).Layers["ShortestLineLayer"];

                BaseShape areaShape1 = mapShapeLayer.InternalFeatures["AreaShape1"].GetShape();
                BaseShape areaShape2 = mapShapeLayer.InternalFeatures["AreaShape2"].GetShape();
                MultilineShape multiLineShape = areaShape1.GetShortestLineTo(areaShape2, GeographyUnit.Meter);

                shortestLineLayer.InternalFeatures.Clear();
                shortestLineLayer.InternalFeatures.Add("ShortestLine", new Feature(multiLineShape));
                ((LayerOverlay)map.CustomOverlays[1]).Redraw();
            }
        }
    }
}
