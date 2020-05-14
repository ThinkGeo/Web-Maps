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
    public partial class AutoRefreshOverlay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-10612429.957618, 4717746.4367209, -10596597.938222, 4709539.0108543);

                // Please input your ThinkGeo Cloud API Key to enable the background map.
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                Map1.CustomOverlays.Add(backgroundOverlay);

                SimpleMarkerOverlay markerOverlay = new SimpleMarkerOverlay("MarkerOverlay");
                markerOverlay.AutoRefreshInterval = TimeSpan.FromMilliseconds(3000);
                markerOverlay.Tick += new EventHandler<EventArgs>(markerOverlay_Tick);
                markerOverlay.Markers.Add(new Marker(-10604400.4464835, 4712974.81950668, new WebImage(21, 25, -10.5f, -25f)));
                markerOverlay.Markers.Add(new Marker(-10604390.4277293, 4717312.24123085, new WebImage(21, 25, -10.5f, -25f)));
                markerOverlay.Markers.Add(new Marker(-10604204.5241797, 4717028.74910805, new WebImage(21, 25, -10.5f, -25f)));
                markerOverlay.Markers.Add(new Marker(-10607696.6166059, 4715518.3564733, new WebImage(21, 25, -10.5f, -25f)));
                markerOverlay.Markers.Add(new Marker(-10600568.8296104, 4713478.60961773, new WebImage(21, 25, -10.5f, -25f)));
                markerOverlay.Markers.Add(new Marker(-10605393.4163414, 4712409.51663098, new WebImage(21, 25, -10.5f, -25f)));

                Map1.CustomOverlays.Add(markerOverlay);
            }
        }

        void markerOverlay_Tick(object sender, EventArgs e)
        {
            SimpleMarkerOverlay markerOverlay = (SimpleMarkerOverlay)Map1.CustomOverlays["MarkerOverlay"];
            foreach (Marker marker in markerOverlay.Markers)
            {
                double lon = marker.Position.X + new Random(Guid.NewGuid().GetHashCode()).Next(-2000000, 2000000) / 5000.0;
                double lat = marker.Position.Y + new Random(Guid.NewGuid().GetHashCode()).Next(-2000000, 2000000) / 5000.0;
                marker.Position = new PointShape(lon, lat);
            }
        }
    }
}
