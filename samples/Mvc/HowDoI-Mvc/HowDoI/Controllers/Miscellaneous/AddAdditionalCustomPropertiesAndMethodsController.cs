using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Layers;

namespace CSharp_HowDoISamples
{
    public partial class MiscellaneousController : Controller
    {
        //
        // GET: /AddAdditionalCustomPropertiesAndMethods/

        public ActionResult AddAdditionalCustomPropertiesAndMethods()
        {
            return View();
        }

        [MapActionFilter]
        public void SetSecurityLevel(Map map, GeoCollection<object> args)
        {
            LayerOverlay staticOverlay = (LayerOverlay)map.CustomOverlays["StaticOverlay"];
            string selectedValue = args[0].ToString();
            foreach (Layer layer in staticOverlay.Layers)
            {
                if (layer.Name != "WorldMapKitLayer")
                {
                    layer.IsVisible = true;
                    SecurityLevel securityLevel = ((AdministrationShapeFileFeatureLayer)layer).SecurityLevel;

                    if (selectedValue == "AverageUsageLevel1" && securityLevel == SecurityLevel.AverageUsageLevel2)
                    {
                        layer.IsVisible = false;
                    }
                    else if (selectedValue == "AverageUsageLevel2" && securityLevel == SecurityLevel.AverageUsageLevel1)
                    {
                        layer.IsVisible = false;
                    }
                }
            }
            staticOverlay.Redraw();
        }
    }
}
