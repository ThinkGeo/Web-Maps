using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Styles;

namespace CSharp_HowDoISamples_for_Debug
{
    public partial class StylesController : Controller
    {
        //
        // GET: /PredefinedStyles/

        public ActionResult PredefinedStyles()
        {

            return View();
        }

        [MapActionFilter]
        public void PreDefinedStyles_SelectedIndexChanged(Map map, GeoCollection<object> args)
        {
            if (null != map)
            {
                string optionString = args[0] as string;
                FeatureLayer worldLayer = (FeatureLayer)((LayerOverlay)map.CustomOverlays[1]).Layers["WorldLayer"];
                switch (optionString)
                {
                    case "AreaStyles.Country1":
                        worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69)); ;
                        break;
                    case "AreaStyles.Swamp1":
                        worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateHatchStyle(GeoHatchStyle.Percent05, GeoColor.StandardColors.Green, GeoColor.FromArgb(180, 220, 224, 204)); 
                        break;
                    case "AreaStyles.Grass1":
                        worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(150, 216, 221, 188)); ;
                        break;
                    case "AreaStyles.Sand1":
                        worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateHatchStyle(GeoHatchStyle.SmallConfetti, GeoColor.StandardColors.LightGoldenrodYellow, GeoColor.FromArgb(255, 255, 238, 208)); ;
                        break;
                    case "AreaStyles.Crop1":
                        worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.StandardColors.LightGreen); 
                        break;
                    default:
                        break;
                }

                ((LayerOverlay)map.CustomOverlays[1]).ClientCache.CacheId = optionString;
                // ((LayerOverlay)Map1.CustomOverlays[1]).ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path + "/" + ddlPreDefinedStyles.SelectedValue);
                ((LayerOverlay)map.CustomOverlays[1]).Redraw();
            }
        }
    }
}
