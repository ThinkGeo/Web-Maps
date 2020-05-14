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
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI
{
    public partial class ConvertScreenCoordinatesToWorld : System.Web.UI.Page
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

                InMemoryMarkerOverlay inMemoryMarkerOverlay = new InMemoryMarkerOverlay("InMemoryMarkerOverlay");
                inMemoryMarkerOverlay.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                Map1.CustomOverlays.Add(inMemoryMarkerOverlay);
            }
        }

        protected void btnConvert_Click(object sender, EventArgs e)
        {
            float screenX, screenY;
            if (float.TryParse(ScreenXTextBox.Text, out screenX) && float.TryParse(ScreenYTextBox.Text, out screenY))
            {
                PointShape worldCoordinate = Map1.ToWorldCoordinate(screenX, screenY);
                InMemoryMarkerOverlay inMemoryMarkerOverlay = (InMemoryMarkerOverlay)Map1.CustomOverlays["InMemoryMarkerOverlay"];
                inMemoryMarkerOverlay.FeatureSource.InternalFeatures.Clear();
                inMemoryMarkerOverlay.FeatureSource.InternalFeatures.Add("marker", new Feature(worldCoordinate));

                Label1.Text = string.Format("The converted world coordinates are <br/><span style='color:red;font-size:13;font-weight:bolder;'>({0}, {1})</span>.", worldCoordinate.X, worldCoordinate.Y);
            }
        }
    }
}
