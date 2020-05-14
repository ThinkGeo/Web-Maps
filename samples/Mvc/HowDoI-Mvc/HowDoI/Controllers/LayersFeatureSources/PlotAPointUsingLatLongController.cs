using System;
using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Layers;

namespace CSharp_HowDoISamples
{
    public partial class LayersFeatureSourcesController : Controller
    {
        public ActionResult PlotAPointUsingLatLong()
        {
            return View();
        }

        [MapActionFilter]
        public void PlotPoint(Map map, GeoCollection<object> args)
        {
            if (map != null)
            {
                double x = Convert.ToDouble(args["x"]);
                double y = Convert.ToDouble(args["y"]);
                LayerOverlay dynamicOverlay = (LayerOverlay)map.CustomOverlays["DynamicOverlay"];
                InMemoryFeatureLayer shapeLayer = (InMemoryFeatureLayer)dynamicOverlay.Layers["ShapeLayer"];

                Proj4Projection proj4 = new Proj4Projection(4326,3857);
                proj4.Open();
                PointShape pointShape = (PointShape)proj4.ConvertToExternalProjection(new PointShape(x, y));

                Feature pointFeature = new Feature(pointShape);
                shapeLayer.InternalFeatures.Add(pointFeature.Id, pointFeature);
            }
        }
    }
}