/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI
{
    public partial class UsingPredefinedStyle : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-13064290.945669, 6003560.5647606, -9011293.9803927, 3902459.5429304);

                // Please input your ThinkGeo Cloud API Key to enable the background map.
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                Map1.CustomOverlays.Add(backgroundOverlay);

                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02_3857.shp"));
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                LayerOverlay staticOverlay = new LayerOverlay();
                staticOverlay.Layers.Add("WorldLayer", worldLayer);
                staticOverlay.IsBaseOverlay = false;
                Map1.CustomOverlays.Add(staticOverlay);
            }
        }

        protected void ddlPreDefinedStyles_SelectedIndexChanged(object sender, EventArgs e)
        {
            FeatureLayer worldLayer = (FeatureLayer)((LayerOverlay)Map1.CustomOverlays[1]).Layers["WorldLayer"];
            switch (ddlPreDefinedStyles.SelectedValue)
            {
                case "AreaStyles.Country1":
                    worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
                    break;
                case "AreaStyles.Swamp1":
                    worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateHatchStyle(GeoHatchStyle.Percent05, GeoColor.StandardColors.Green, GeoColor.FromArgb(180, 220, 224, 204));
                    break;
                case "AreaStyles.Grass1":
                    worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(150, 216, 221, 188));
                    break;
                case "AreaStyles.Sand1":
                    worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateHatchStyle(GeoHatchStyle.SmallConfetti, GeoColor.StandardColors.LightGoldenrodYellow, GeoColor.FromArgb(255, 255, 238, 208));
                    break;
                case "AreaStyles.Crop1":
                    worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.StandardColors.LightGreen);
                    break;
                default:
                    break;
            }

            ((LayerOverlay)Map1.CustomOverlays[1]).ClientCache.CacheId = ddlPreDefinedStyles.SelectedValue;
            // ((LayerOverlay)Map1.CustomOverlays[1]).ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path + "/" + ddlPreDefinedStyles.SelectedValue);
            ((LayerOverlay)Map1.CustomOverlays[1]).Redraw();
        }
    }
}
