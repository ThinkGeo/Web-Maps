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

namespace HowDoI
{
    public partial class AddAMarker : System.Web.UI.Page
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

                InMemoryMarkerOverlay markerOverlay = new InMemoryMarkerOverlay("MarkerOverlay");
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.ContentHtml = "<div style='font-size:11px'>Kansas City</Div>";
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.HasCloseButton = true;
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.Width = 100;
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.Height = 30;
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.BackgroundColor = GeoColor.StandardColors.LightBlue;
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.BorderColor = GeoColor.StandardColors.Black;
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.BorderWidth = 1;
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.ImageOffsetX = -10.5f;
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.ImageOffsetY = -25f;
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.ImageWidth = 21;
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.ImageHeight = 25;
                markerOverlay.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                Map1.CustomOverlays.Add(markerOverlay);    
            }
        }

        protected void Button1_Click(object sender, EventArgs args)
        {
            InMemoryMarkerOverlay markerOverlay = (InMemoryMarkerOverlay)Map1.CustomOverlays["MarkerOverlay"];
            if (!markerOverlay.FeatureSource.InternalFeatures.Contains("Kansas"))
            {
                markerOverlay.FeatureSource.InternalFeatures.Add("Kansas", new Feature(-10526148.4104304, 4732850.5697907));
            }
        }
    }
}
