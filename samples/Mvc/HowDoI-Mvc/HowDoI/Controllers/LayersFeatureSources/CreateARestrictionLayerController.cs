using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Styles;

namespace CSharp_HowDoISamples
{
    public partial class LayersFeatureSourcesController : Controller
    {
        //
        // GET: /CreateARestrictionLayer/

        public ActionResult CreateARestrictionLayer()
        {
            return View();
        }

        [MapActionFilter]
        public void SetRestrictionStyle(Map map, GeoCollection<object> args)
        {
            LayerOverlay staticOverlay = (LayerOverlay)map.CustomOverlays["StaticOverlay"];
            RestrictionLayer currentRestrictionLayer = (RestrictionLayer)staticOverlay.Layers["RestrictionLayer"];

            if (currentRestrictionLayer.CustomStyles.Count > 0)
            {
                currentRestrictionLayer.CustomStyles.Clear();
            }

            string selectItem = args[0].ToString();
            switch (selectItem)
            {
                case "HatchPattern":
                    currentRestrictionLayer.RestrictionStyle = RestrictionStyle.HatchPattern;
                    break;
                case "CircleWithSlashImage":
                    currentRestrictionLayer.RestrictionStyle = RestrictionStyle.CircleWithSlashImage;
                    break;
                case "UseCustomStyles":
                    currentRestrictionLayer.RestrictionStyle = RestrictionStyle.UseCustomStyles;
                    Style customStyle = new AreaStyle(new GeoSolidBrush(new GeoColor(150, GeoColor.StandardColors.Gray)));
                    currentRestrictionLayer.CustomStyles.Add(customStyle);
                    break;
                default:
                    break;
            }

        }

        [MapActionFilter]
        public string ShowZones(Map map, GeoCollection<object> args)
        {
            LayerOverlay staticOverlay = (LayerOverlay)map.CustomOverlays["StaticOverlay"];
            RestrictionLayer currentRestrictionLayer = (RestrictionLayer)staticOverlay.Layers["RestrictionLayer"];
            currentRestrictionLayer.RestrictionMode = RestrictionMode.ShowZones;
            return "You can see only Africa because we have added a RestrictionLayer and its mode is ShowZones.";
        }

        [MapActionFilter]
        public string HideZones(Map map, GeoCollection<object> args)
        {
            LayerOverlay staticOverlay = (LayerOverlay)map.CustomOverlays["StaticOverlay"];
            RestrictionLayer currentRestrictionLayer = (RestrictionLayer)staticOverlay.Layers["RestrictionLayer"];
            currentRestrictionLayer.RestrictionMode = RestrictionMode.HideZones;
            return "You can not see Africa because we have added a RestrictionLayer and its mode is HideZones.";
        }
    }
}
