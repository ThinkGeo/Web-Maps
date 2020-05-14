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
    public partial class ClickEventOnAMarker : System.Web.UI.Page
    {
        private readonly string[] markerIcons = new string[] { "marker_blue.gif", "marker.gif", "marker_gold.gif", "marker_green.gif" };

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
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage = new WebImage("../../theme/default/img/marker_blue.gif", 21, 25);
                markerOverlay.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                markerOverlay.Click += new EventHandler<MarkerOverlayClickEventArgs>(markerOverlay_Click);
                Map1.CustomOverlays.Add(markerOverlay);
            }
        }

        void markerOverlay_Click(object sender, MarkerOverlayClickEventArgs e)
        {
            int times;
            if (ViewState["ClickTimes"] != null)
            {
                times = (int)ViewState["ClickTimes"];
                times++;
            }
            else
            {
                times = 1;
            }

            int iconIndex = times % 4 + 1;
            if (iconIndex >= 4)
            {
                iconIndex -= 4;
            }

            string iconPath = "../../theme/default/img/" + markerIcons[iconIndex];
            InMemoryMarkerOverlay markerOverlay = (InMemoryMarkerOverlay)Map1.CustomOverlays["MarkerOverlay"];
            markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.ImageVirtualPath = iconPath;

            Label1.Text = String.Format("You have clicked the marker <span style='color:red;font-weight:bolder;font-size:15;'>{0}</span> time(s)", times);
            ViewState["ClickTimes"] = times;
        }
    }
}
