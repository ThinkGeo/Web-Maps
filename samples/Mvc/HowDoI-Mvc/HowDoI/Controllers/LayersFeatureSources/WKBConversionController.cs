using System;
using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Layers;
using System.Text;

namespace CSharp_HowDoISamples
{
    public partial class LayersFeatureSourcesController : Controller
    {
        //
        // GET: /WKBConvertion/

        public ActionResult WKBConversion()
        {
            return View();
        }

        [MapActionFilter]
        public string ConvertWkb(Map map, GeoCollection<object> args)
        {
            InMemoryFeatureLayer mapShapeLayer = (InMemoryFeatureLayer)(((LayerOverlay)map.CustomOverlays[1]).Layers["InMemoryFeatureLayer"]);
            string btnConvertText = args[0].ToString();
            string txtWKBText = args[1].ToString();
            if (btnConvertText == "WKB  to  Feature")
            {
                byte[] wellKnownBinary = Convert.FromBase64String(txtWKBText);
                Feature feature = new Feature(wellKnownBinary);

                mapShapeLayer.InternalFeatures.Add("feature", feature);

                txtWKBText = string.Empty;
            }
            else
            {
                byte[] wellKnownBinary = mapShapeLayer.InternalFeatures["feature"].GetWellKnownBinary();

                mapShapeLayer.InternalFeatures.Clear();

                txtWKBText = Convert.ToBase64String(wellKnownBinary);
            }

            ((LayerOverlay)map.CustomOverlays[1]).Redraw();

            return txtWKBText;
        }
    }
}
