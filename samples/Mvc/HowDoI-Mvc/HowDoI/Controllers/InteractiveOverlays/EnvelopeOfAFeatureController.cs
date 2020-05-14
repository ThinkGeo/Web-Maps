using System.Collections.ObjectModel;
using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Layers;
using System;

namespace CSharp_HowDoISamples
{
    public partial class InteractiveOverlaysController : Controller
    {
        //
        // GET: /EnvelopeOfAFeature/

        public ActionResult EnvelopeOfAFeature()
        {
            return View();
        }

        [MapActionFilter]
        public void ClickOnFeature(Map map, GeoCollection<object> args)
        {
            PointShape point = new PointShape(Convert.ToDouble(args[0]), Convert.ToDouble(args[1]));

            ShapeFileFeatureLayer worldLayer = (ShapeFileFeatureLayer)((LayerOverlay)map.CustomOverlays[1]).Layers["WorldLayer"];
            InMemoryFeatureLayer boundingBoxLayer = (InMemoryFeatureLayer)((LayerOverlay)map.CustomOverlays[2]).Layers["BoundingBoxLayer"];
            boundingBoxLayer.InternalFeatures.Clear();

            worldLayer.Open();
            Collection<Feature> selectedFeatures = worldLayer.QueryTools.GetFeaturesContaining(point, new string[0]);
            worldLayer.Close();

            if (selectedFeatures.Count > 0)
            {
                AreaBaseShape areaShape = (AreaBaseShape)selectedFeatures[0].GetShape();
                boundingBoxLayer.InternalFeatures.Add("BoundingBox", new Feature(areaShape.GetBoundingBox()));
            }
            ((LayerOverlay)map.CustomOverlays[2]).Redraw();
        }
    }
}
