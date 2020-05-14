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

namespace HowDoI.Samples
{
    public partial class AddAClickEvent : System.Web.UI.Page
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
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.ImageWidth = 21;
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.ImageHeight = 25;
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.ImageOffsetX = -10.5f;
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.ImageOffsetY = -25f;
                markerOverlay.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                Map1.CustomOverlays.Add(markerOverlay);    
            }
        }

        protected void Map1_Click(object sender, MapClickedEventArgs e)
        {
            InMemoryMarkerOverlay markerOverlay = (InMemoryMarkerOverlay)Map1.CustomOverlays["MarkerOverlay"];
            markerOverlay.FeatureSource.InternalFeatures.Add("marker" + DateTime.Now.ToString("mmssms"), new Feature(e.Position));
        }
    }
}
