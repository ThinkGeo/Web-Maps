using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Layers;
using System.Globalization;

namespace CSharp_HowDoISamples
{
    public partial class SpatialFunctionsController : Controller
    {
        //
        // GET: /FindFeatureById/

        public ActionResult FindFeatureById()
        {
            return View();
        }

        [MapActionFilter]
        public string FindFeature(Map map, GeoCollection<object> args)
        {
            string id = args[0] as string;
            FeatureLayer shapeFileLayer = (FeatureLayer)((LayerOverlay)map.CustomOverlays["WorldLayer"]).Layers[0];
            InMemoryFeatureLayer mapShapeLayer = (InMemoryFeatureLayer)((LayerOverlay)map.CustomOverlays["InMemoryFeatureLayer"]).Layers[0];

            shapeFileLayer.Open();
            Feature feature = shapeFileLayer.FeatureSource.GetFeatureById(id, new string[] { "CNTRY_NAME" });
            shapeFileLayer.Close();

            mapShapeLayer.InternalFeatures.Clear();
            mapShapeLayer.InternalFeatures.Add(feature.Id, feature);


            return string.Format(CultureInfo.InvariantCulture, "{0}^{1}", feature.GetBoundingBox().GetCenterPoint().ToString(), feature.ColumnValues["CNTRY_NAME"]);
        }
    }
}
