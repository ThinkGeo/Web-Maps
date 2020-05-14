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
    public partial class AddClickEventToHighlightOverlay : System.Web.UI.Page
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

                Feature feature = new Feature(new RectangleShape(-12269634.2752346, 6446275.84101716, -8908898.84818568, 3503549.84350437));
                Map1.HighlightOverlay.HighlightStyle = Map1.HighlightOverlay.Style;
                Map1.HighlightOverlay.Features.Add("feature", feature);
                Map1.HighlightOverlay.Click += new EventHandler<HighlightFeatureOverlayClickEventArgs>(EventLayer_Click);
            }
        }

        protected void EventLayer_Click(object sender, HighlightFeatureOverlayClickEventArgs e)
        {
            CloudPopup popup;
            if (Map1.Popups.Contains("Popup"))
            {
                popup = (CloudPopup)Map1.Popups["Popup"];
                popup.Position = e.Location;
            }
            else{
                popup = new CloudPopup("Popup", e.Location, "<span class='popup'>Inside the area</span>");
                popup.AutoSize = true;
                Map1.Popups.Add(popup);
            }

            this.Title = "Title changed";
        }
    }
}
