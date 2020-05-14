using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Layers;

namespace CSharp_HowDoISamples
{
    public partial class LayersFeatureSourcesController : Controller
    {
        //
        // GET: /CreateaScaleLineAdormentLayer/

        public ActionResult CreateaScaleLineAdormentLayer()
        {
            return View();
        }

        [MapActionFilter]
        public void SetScaleLineLocation(Map map, GeoCollection<object> args)
        {
            string placementString = args[0].ToString();
            AdornmentLocation placement;
            switch (placementString)
            {
                case "Center":
                    placement = AdornmentLocation.Center;
                    break;
                case "CenterLeft":
                    placement = AdornmentLocation.CenterLeft;
                    break;
                case "CenterRight":
                    placement = AdornmentLocation.CenterRight;
                    break;
                case "LowerCenter":
                    placement = AdornmentLocation.LowerCenter;
                    break;
                case "LowerLeft":
                    placement = AdornmentLocation.LowerLeft;
                    break;
                case "LowerRight":
                    placement = AdornmentLocation.LowerRight;
                    break;
                case "UpperCenter":
                    placement = AdornmentLocation.UpperCenter;
                    break;
                case "UpperLeft":
                    placement = AdornmentLocation.UpperLeft;
                    break;
                case "UpperRight":
                    placement = AdornmentLocation.UpperRight;
                    break;
                default:
                    placement = AdornmentLocation.CenterRight;
                    break;
            }
            LayerOverlay dynamicOverlay = (LayerOverlay)map.CustomOverlays["DynamicOverlay"];

            ScaleLineAdornmentLayer currentScaleLineAdornmentLayer = (ScaleLineAdornmentLayer)dynamicOverlay.Layers["ScaleLineAdornmentLayer"];
            currentScaleLineAdornmentLayer.Location = placement;
            dynamicOverlay.Redraw();
        }
    }
}
