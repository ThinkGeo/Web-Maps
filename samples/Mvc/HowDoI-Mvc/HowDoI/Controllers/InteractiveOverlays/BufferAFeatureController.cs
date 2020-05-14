using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;

namespace CSharp_HowDoISamples
{
    public partial class InteractiveOverlaysController : Controller
    {
        //
        // GET: /BufferAFeature/

        public ActionResult BufferAFeature()
        {
            return View();
        }

        [MapActionFilter]
        public void BufferFeature(Map map, GeoCollection<object> args)
        {
            if (null != map)
            {
                InMemoryFeatureLayer mapShapeLayer = (InMemoryFeatureLayer)((LayerOverlay)(map.CustomOverlays["BufferLayerOverLayer"])).Layers["InMemoryFeatureLayer"];
                InMemoryFeatureLayer bufferLayer = (InMemoryFeatureLayer)((LayerOverlay)(map.CustomOverlays["BufferLayerOverLayer"])).Layers["BufferLayer"];

                AreaBaseShape baseShape = (AreaBaseShape)mapShapeLayer.InternalFeatures["POLYGON"].GetShape();
                MultipolygonShape bufferedShape = baseShape.Buffer(1000, 8, BufferCapType.Round, GeographyUnit.Meter, DistanceUnit.Kilometer);
                Feature bufferFeature = new Feature(bufferedShape);

                bufferLayer.InternalFeatures.Clear();
                bufferLayer.InternalFeatures.Add("BufferFeature", bufferFeature);
                ((LayerOverlay)(map.CustomOverlays["BufferLayerOverLayer"])).Redraw();
            }
        }
    }
}
