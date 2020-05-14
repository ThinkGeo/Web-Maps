using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Mvc;

namespace CSharp_HowDoISamples_for_Debug
{
    public partial class StylesController : Controller
    {
        //
        // GET: /GeographicSimpleStandardColors/

        public ActionResult GeographicSimpleStandardColors()
        {
            return View();
        }

        [MapActionFilter]
        public void StandardColorChanged(Map map, GeoCollection<object> args)
        {
            string standardColor = args[0] as string;
            GeoColor borderColor = GetStandardColor(standardColor);
            FeatureLayer worldLayer = (FeatureLayer)map.StaticOverlay.Layers["WorldLayer"];
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = borderColor;

            //Map1.StaticOverlay.ClientCache.CacheId = ddlStandardColor.SelectedValue + ddlGeofraphicColor.SelectedValue;
            //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + ddlStandardColor.SelectedValue + "/" + ddlGeofraphicColor.SelectedValue);
            map.StaticOverlay.Redraw();
        }

        [MapActionFilter]
        public void GeographicColorChanged(Map map, GeoCollection<object> args)
        {
            string geographicColor = args[0] as string;
            GeoColor areaColor = GetGeographyColor(geographicColor);
            FeatureLayer worldLayer = (FeatureLayer)map.StaticOverlay.Layers["WorldLayer"];
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color = areaColor;

            //Map1.StaticOverlay.ClientCache.CacheId  = ddlStandardColor.SelectedValue + ddlGeofraphicColor.SelectedValue;
            //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + ddlStandardColor.SelectedValue + "/" + ddlGeofraphicColor.SelectedValue);
            map.StaticOverlay.Redraw();
        }

        private GeoColor GetGeographyColor(string geographicColor)
        {
            GeoColor geoColor = GeoColor.GeographicColors.Sand;
            switch (geographicColor)
            {
                case "Dirt":
                    geoColor = GeoColor.GeographicColors.Dirt;
                    break;
                case "Sand":
                    geoColor = GeoColor.GeographicColors.Sand;
                    break;
                case "Swamp":
                    geoColor = GeoColor.GeographicColors.Swamp;
                    break;
                case "LightGreen":
                    geoColor = GeoColor.StandardColors.LightGreen;
                    break;
                case "LightPink":
                    geoColor = GeoColor.StandardColors.LightPink;
                    break;
                case "DarkBlue":
                    geoColor = GeoColor.SimpleColors.DarkBlue;
                    break;
                default:
                    break;
            }
            return geoColor;
        }
        
        private GeoColor GetStandardColor(string standardColor)
        {
            GeoColor geoColor = GeoColor.StandardColors.Gray;
            switch (standardColor)
            {
                case "LightGray":
                    geoColor = GeoColor.StandardColors.LightGray;
                    break;
                case "Pink":
                    geoColor = GeoColor.StandardColors.Pink;
                    break;
                case "Teal":
                    geoColor = GeoColor.StandardColors.Teal;
                    break;
                case "Plum":
                    geoColor = GeoColor.StandardColors.Plum;
                    break;
                case "DarkGreen":
                    geoColor = GeoColor.SimpleColors.DarkGreen;
                    break;
                default:
                    break;
            }
            return geoColor;
        }
    }
}
