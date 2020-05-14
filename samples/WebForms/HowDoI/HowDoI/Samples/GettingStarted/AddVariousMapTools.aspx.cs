/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Web.UI.WebControls;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples
{
    public partial class AddVariousMappingControls : System.Web.UI.Page
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
                backgroundOverlay.Name = "ThinkGeoCloudMap";
                Map1.CustomOverlays.Add(backgroundOverlay);
            }
        }

        protected void lsbControls_SelectedIndexChanged(object sender, EventArgs e)
        {
            EnableControls(false);
            foreach (ListItem listItem in lsbControls.Items)
            {
                if (listItem.Selected)
                {
                    switch (listItem.Value)
                    {
                        case "PanZoom":
                            Map1.MapTools.PanZoom.Enabled = true;
                            break;
                        case "PanZoomBar":
                            Map1.MapTools.PanZoomBar.Enabled = true;
                            break;
                        case "MouseCoordinateLonLat":
                            Map1.MapTools.MouseCoordinate.MouseCoordinateType = MouseCoordinateType.LongitudeLatitude;
                            Map1.MapTools.MouseCoordinate.Enabled = true;
                            break;
                        case "MouseCoordinateLatLon":
                            Map1.MapTools.MouseCoordinate.MouseCoordinateType = MouseCoordinateType.LatitudeLongitude;
                            Map1.MapTools.MouseCoordinate.Enabled = true;
                            break;
                        case "MouseCoordinateDms":
                            Map1.MapTools.MouseCoordinate.MouseCoordinateType = MouseCoordinateType.DegreesMinutesSeconds;
                            Map1.MapTools.MouseCoordinate.Enabled = true;
                            break;
                        case "MiniMap":
                            Map1.MapTools.MiniMap.Enabled = true;
                            break;
                        case "ScaleLine":
                            Map1.MapTools.ScaleLine.Enabled = true;
                            break;
                        case "OverlaySwitcher":
                            Map1.MapTools.OverlaySwitcher.Enabled = true;
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        protected void btnSelectAll_Click(object sender, EventArgs e)
        {
            EnableControls(true);
            foreach (ListItem listItem in lsbControls.Items)
            {
                listItem.Selected = true;
            }
        }

        protected void btnNoSelect_Click(object sender, EventArgs e)
        {
            EnableControls(false);
            foreach (ListItem listItem in lsbControls.Items)
            {
                listItem.Selected = false;
            }
        }

        private void EnableControls(bool isEnable)
        {
            Map1.MapTools.PanZoom.Enabled = isEnable;
            Map1.MapTools.PanZoomBar.Enabled = isEnable;
            Map1.MapTools.MouseCoordinate.Enabled = isEnable;
            Map1.MapTools.MiniMap.Enabled = isEnable;
            Map1.MapTools.ScaleLine.Enabled = isEnable;
            Map1.MapTools.OverlaySwitcher.Enabled = isEnable;
        }
    }
}
