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

namespace HowDoI.Samples.Markers
{
    public partial class MarkerWithContextMenu : System.Web.UI.Page
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

                ContextMenu menuOnMarker = new ContextMenu("kansas", 180);
                ContextMenuItem redirectItem = new ContextMenuItem("<a href='http://en.wikipedia.org/wiki/Lawrence%2C_Kansas' target='_blank'>Lawrence<a>");
                ContextMenuItem showPositionItem = new ContextMenuItem("CurrentPosition(Server Event)");
                ContextMenuItem zoomOutItem = new ContextMenuItem("<div onclick='Map1.ZoomIn();'>Zoom in</div>");
                ContextMenuItem zoomInItem = new ContextMenuItem("<div onclick='Map1.ZoomOut();'>Zoom out</div>");
                ContextMenuItem centerItem = new ContextMenuItem("<div onclick='Map1.PanToWorldCoordinate(-94.558, 39.078);'>Center map here</div>");
                showPositionItem.Click += new EventHandler<ContextMenuItemClickEventArgs>(showPosition_Click);
                
                menuOnMarker.MenuItems.Add(redirectItem);
                menuOnMarker.MenuItems.Add(showPositionItem);
                menuOnMarker.MenuItems.Add(zoomOutItem);
                menuOnMarker.MenuItems.Add(zoomInItem);
                menuOnMarker.MenuItems.Add(centerItem);

                BaseShape kansas = new PointShape(-10526148.4104304, 4732850.5697907);
                InMemoryMarkerOverlay markerOverlay = new InMemoryMarkerOverlay("MarkerOverlay");
                markerOverlay.FeatureSource.InternalFeatures.Add("kansasCity", new Feature(kansas));
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.ContextMenu = menuOnMarker;
                markerOverlay.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                Map1.CustomOverlays.Add(markerOverlay);
            }
        }

        private void showPosition_Click(object sender, ContextMenuItemClickEventArgs e)
        {
            Longitude.Value = e.Location.X.ToString();
            Latitude.Value = e.Location.Y.ToString();
        }
    }
}
