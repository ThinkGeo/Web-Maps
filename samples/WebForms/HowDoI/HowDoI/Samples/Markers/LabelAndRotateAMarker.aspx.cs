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
    public partial class LabelAndRotateAMarker : System.Web.UI.Page
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
                markerOverlay.FeatureSource.InternalFeatures.Add("Kansas", new Feature(-10517734.8833162, 4687318.87110816));
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage = new WebImage("../../theme/default/samplepic/Vehicle.png", 48, 48);
                markerOverlay.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                Map1.CustomOverlays.Add(markerOverlay);
            }
        }

        protected void ShowTextCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            InMemoryMarkerOverlay markerOverlay = (InMemoryMarkerOverlay)Map1.CustomOverlays["MarkerOverlay"];
            if (ShowTextCheckBox.Checked)
            {
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.Text = "Vehicle";
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.FontColor = GeoColor.StandardColors.OrangeRed;
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.FontStyle = new GeoFont("Arail Black", 12, DrawingFontStyles.Bold);
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.TextOffsetX = 7F;
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.TextOffsetY = 3F;
            }
            else
            {
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.Text = string.Empty;
            }
        }

        protected void RotateButton_Click(object sender, EventArgs e)
        {
            InMemoryMarkerOverlay markerOverlay = (InMemoryMarkerOverlay)Map1.CustomOverlays["MarkerOverlay"];
            markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.RotationAngle += 30;
        }
    }
}
