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
    public partial class UseDraggableMarkers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.CurrentExtent = new RectangleShape(-13939426.6371, 6701997.4056, -7812401.86, 2626987.386962);
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.MapTools.PanZoomBar.Enabled = true;
                Map1.MapTools.MouseCoordinate.Enabled = true;

                // Please input your ThinkGeo Cloud API Key to enable the background map.
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                Map1.CustomOverlays.Add(backgroundOverlay);

                SimpleMarkerOverlay markerOverlay = new SimpleMarkerOverlay("MarkerOverlay");
                markerOverlay.DragMode = MarkerDragMode.Drag;
                markerOverlay.Markers.Add(new Marker(-8922952.93266, 2984101.58384, new WebImage(21, 25, -10.5f, -25f)));
                markerOverlay.Markers.Add(new Marker(-10830821.09801, 4539747.98328, new WebImage(21, 25, -10.5f, -25f)));
                markerOverlay.Markers.Add(new Marker(-12454955.13517, 4980025.26614, new WebImage(21, 25, -10.5f, -25f)));
                markerOverlay.Markers.Add(new Marker(-10772117.52067, 3864656.14956, new WebImage(21, 25, -10.5f, -25f)));
                markerOverlay.Markers.Add(new Marker(-13164290.75755, 4035875.09290, new WebImage(21, 25, -10.5f, -25f)));
                markerOverlay.Markers.Add(new Marker(-9754587.80028, 5156136.17929, new WebImage(21, 25, -10.5f, -25f)));

                Map1.CustomOverlays.Add(markerOverlay);
            }
        }
    }
}
