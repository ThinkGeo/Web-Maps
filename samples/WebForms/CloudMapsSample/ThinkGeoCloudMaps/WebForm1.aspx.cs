/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.WebForms;

namespace ThinkGeoCloudMapsSample
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.MapTools.OverlaySwitcher.Enabled = true;
                Map1.MapTools.OverlaySwitcher.BaseOverlayTitle = "ThinkGeo Cloud Maps:";
                Map1.MapTools.OverlaySwitcher.BackgroundColor = GeoColor.StandardColors.DarkSlateGray;

                // Please input your ThinkGeo Cloud API Key to enable the background map.
                ThinkGeoCloudRasterMapsOverlay lightMap = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                lightMap.Name = "Light";
                lightMap.MapType = ThinkGeoCloudRasterMapsMapType.Light;
                lightMap.WrapDateline = WrapDatelineMode.WrapDateline;
                Map1.CustomOverlays.Add(lightMap);

                // Please input your ThinkGeo Cloud API Key to enable the background map.
                ThinkGeoCloudRasterMapsOverlay darkMap = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                darkMap.Name = "Dark";
                darkMap.MapType = ThinkGeoCloudRasterMapsMapType.Dark;
                darkMap.WrapDateline = WrapDatelineMode.WrapDateline;
                Map1.CustomOverlays.Add(darkMap);

                // Please input your ThinkGeo Cloud API Key to enable the background map.
                ThinkGeoCloudRasterMapsOverlay aerialMap = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                aerialMap.Name = "Aerial";
                aerialMap.MapType = ThinkGeoCloudRasterMapsMapType.Aerial;
                aerialMap.WrapDateline = WrapDatelineMode.WrapDateline;
                Map1.CustomOverlays.Add(aerialMap);

                // Please input your ThinkGeo Cloud API Key to enable the background map.
                ThinkGeoCloudRasterMapsOverlay hybridMap = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                hybridMap.Name = "Hybrid";
                hybridMap.MapType = ThinkGeoCloudRasterMapsMapType.Hybrid;
                hybridMap.WrapDateline = WrapDatelineMode.WrapDateline;
                Map1.CustomOverlays.Add(hybridMap);

                // Please input your ThinkGeo Cloud API Key to enable the background map.
                ThinkGeoCloudRasterMapsOverlay transparentBackgroundMap = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                transparentBackgroundMap.Name = "Transparent Background";
                transparentBackgroundMap.MapType = ThinkGeoCloudRasterMapsMapType.TransparentBackground;
                transparentBackgroundMap.WrapDateline = WrapDatelineMode.WrapDateline;
                Map1.CustomOverlays.Add(transparentBackgroundMap);

                Map1.CurrentExtent = new RectangleShape(-13086298.60, 7339062.72, -8111177.75, 2853137.62);
            }
        }
    }
}