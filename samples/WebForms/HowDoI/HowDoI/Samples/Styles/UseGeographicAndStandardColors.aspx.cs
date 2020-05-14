using System;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
 

namespace HowDoI
{
    public partial class UseGeographicAndStandardColors : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-140, 60, 140, -60);
                Map1.MapUnit = GeographyUnit.DecimalDegree;

                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02.shp"));
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.GeographicColors.Sand;
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.StandardColors.Pink;
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                Map1.StaticOverlay.Layers.Add("WorldLayer", worldLayer);

                // The following two lines of code enable the client and server caching.
                // If you enable these features it will greatly increase the scalability of your
                // mapping application however there some side effects that may be counter intuitive.
                // Please read the white paper on web caching or the documentation regarding these methods.

                //Map1.StaticOverlay.ClientCache.CacheId = "WorldOverlay";
                //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path);
            }
        }

        protected void ddlStandardColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            GeoColor borderColor = GetStandardColor();
            FeatureLayer worldLayer = (FeatureLayer)Map1.StaticOverlay.Layers["WorldLayer"];
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = borderColor;

            //Map1.StaticOverlay.ClientCache.CacheId = ddlStandardColor.SelectedValue + ddlGeofraphicColor.SelectedValue;
            //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + ddlStandardColor.SelectedValue + "/" + ddlGeofraphicColor.SelectedValue);
            Map1.StaticOverlay.Redraw();
        }

        protected void ddlGeofraphicColor_SelectedIndexChanged(object sender, EventArgs e)
        {
            GeoColor areaColor = GetGeographyColor();
            FeatureLayer worldLayer = (FeatureLayer)Map1.StaticOverlay.Layers["WorldLayer"];
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color = areaColor;

            //Map1.StaticOverlay.ClientCache.CacheId  = ddlStandardColor.SelectedValue + ddlGeofraphicColor.SelectedValue;
            //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + ddlStandardColor.SelectedValue + "/" + ddlGeofraphicColor.SelectedValue);
            Map1.StaticOverlay.Redraw();
        }

        private GeoColor GetGeographyColor()
        {
            GeoColor geoColor = GeoColor.GeographicColors.Sand;
            switch (ddlGeofraphicColor.SelectedValue)
            {
                case "GeoColor.GeographicColors.Dirt":
                    geoColor = GeoColor.GeographicColors.Dirt;
                    break;
                case "GeoColor.GeographicColors.Sand":
                    geoColor = GeoColor.GeographicColors.Sand;
                    break;
                case "GeoColor.GeographicColors.Swamp":
                    geoColor = GeoColor.GeographicColors.Swamp;
                    break;
                case "GeoColor.StandardColors.LightGreen":
                    geoColor = GeoColor.StandardColors.LightGreen;
                    break;
                case "GeoColor.StandardColors.LightPink":
                    geoColor = GeoColor.StandardColors.LightPink;
                    break;
                case "GeoColor.SimpleColors.DarkBlue":
                    geoColor = GeoColor.SimpleColors.DarkBlue;
                    break;
                default:
                    break;
            }
            return geoColor;
        }

        private GeoColor GetStandardColor()
        {
            GeoColor geoColor = GeoColor.StandardColors.Gray;
            switch (ddlStandardColor.SelectedValue)
            {
                case "GeoColor.StandardColors.LightGray":
                    geoColor = GeoColor.StandardColors.LightGray;
                    break;
                case "GeoColor.StandardColors.Pink":
                    geoColor = GeoColor.StandardColors.Pink;
                    break;
                case "GeoColor.StandardColors.Teal":
                    geoColor = GeoColor.StandardColors.Teal;
                    break;
                case "GeoColor.StandardColors.Plum":
                    geoColor = GeoColor.StandardColors.Plum;
                    break;
                case "GeoColor.SimpleColors.DarkGreen":
                    geoColor = GeoColor.SimpleColors.DarkGreen;
                    break;
                default:
                    break;
            }
            return geoColor;
        }
    }
}
