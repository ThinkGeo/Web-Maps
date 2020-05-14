using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Mvc;

namespace CSharp_HowDoISamples
{
    public partial class SpatialFunctionsController : Controller
    {
        //
        // GET: /GetFeaturesCount/

        public ActionResult GetFeaturesCount()
        {
            return View();
        }

        [MapActionFilter]
        public string GetCount(Map map, GeoCollection<object> args)
        {
            LayerOverlay staticOverlay = (LayerOverlay)map.CustomOverlays["StaticOverlay"];
            FeatureLayer worldLayer = (FeatureLayer)staticOverlay.Layers["WorldLayer"];
            worldLayer.Open();
            string result = string.Format("There are <span style='color:red;font-size:15;font-weight:bolder;'>{0}</span> records in this vector layer.", worldLayer.QueryTools.GetCount());
            worldLayer.Close();

            return result;
        }
    }
}
