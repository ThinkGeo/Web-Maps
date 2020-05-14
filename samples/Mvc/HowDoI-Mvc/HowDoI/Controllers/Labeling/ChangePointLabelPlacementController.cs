using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Styles;

namespace CSharp_HowDoISamples
{
    public partial class LabelingController : Controller
    {
        //
        // GET: /ChangePointLabelPlacement/

        public ActionResult ChangePointLabelPlacement()
        {
            return View();
        }

        [MapActionFilter]
        public void SetPointPlacement(Map map, GeoCollection<object> args)
        {
            string placementString = args[0].ToString();
            PointPlacement placement;
            switch (placementString)
            {
                case "Center":
                    placement = PointPlacement.Center;
                    break;
                case "CenterLeft":
                    placement = PointPlacement.CenterLeft;
                    break;
                case "CenterRight":
                    placement = PointPlacement.CenterRight;
                    break;
                case "LowerCenter":
                    placement = PointPlacement.LowerCenter;
                    break;
                case "LowerLeft":
                    placement = PointPlacement.LowerLeft;
                    break;
                case "LowerRight":
                    placement = PointPlacement.LowerRight;
                    break;
                case "UpperCenter":
                    placement = PointPlacement.UpperCenter;
                    break;
                case "UpperLeft":
                    placement = PointPlacement.UpperLeft;
                    break;
                case "UpperRight":
                    placement = PointPlacement.UpperRight;
                    break;
                default:
                    placement = PointPlacement.CenterRight;
                    break;
            }
            FeatureLayer labelPlacementLayer = (FeatureLayer)map.DynamicOverlay.Layers["MajorCitiesLabels"];
            labelPlacementLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle.PointPlacement = placement;
            map.DynamicOverlay.Redraw();
        }
    }
}
