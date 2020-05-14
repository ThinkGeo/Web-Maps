using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Layers;

namespace CSharp_HowDoISamples
{
    public partial class LayersFeatureSourcesController : Controller
    {
        //
        // GET: /WKTConvertion/

        public ActionResult WKTConversion()
        {
            return View();
        }

        [MapActionFilter]
        public string ConvertWkt(Map map, GeoCollection<object> args)
        {
            InMemoryFeatureLayer mapShapeLayer = (InMemoryFeatureLayer)(((LayerOverlay)map.CustomOverlays[1]).Layers["InMemoryFeatureLayer"]);
            string btnConvertText = args[0].ToString();
            string txtWKTText = args[1].ToString();
            if (btnConvertText == "WKT  to  Feature")
            {
                Feature feature = new Feature(txtWKTText);

                mapShapeLayer.InternalFeatures.Add("feature", feature);

                txtWKTText = string.Empty;
            }
            else
            {
                txtWKTText = mapShapeLayer.InternalFeatures["feature"].GetWellKnownText();

                mapShapeLayer.InternalFeatures.Clear();
            }

            ((LayerOverlay)map.CustomOverlays[1]).Redraw();

            return txtWKTText;
        }
    }
}
