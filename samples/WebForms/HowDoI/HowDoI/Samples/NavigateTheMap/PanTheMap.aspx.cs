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
    public partial class PanTheMap : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-13064290.945669, 6003560.5647606, -9011293.9803927, 3902459.5429304);

                Map1.MapTools.PanZoomBar.Enabled = true;
                Map1.MapTools.MouseMapTool.Enabled = false;

                // Please input your ThinkGeo Cloud API Key to enable the background map.
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                Map1.CustomOverlays.Add(backgroundOverlay);
            }
        }

        protected void btnPanLeft_Click(object sender, EventArgs e)
        {
            Map1.Pan(PanDirection.Left, 10);
        }

        protected void btnPanRight_Click(object sender, EventArgs e)
        {
            Map1.Pan(PanDirection.Right, 10);
        }

        protected void btnPanUp_Click(object sender, EventArgs e)
        {
            Map1.Pan(PanDirection.Up, 10);
        }

        protected void btnPanDown_Click(object sender, EventArgs e)
        {
            Map1.Pan(PanDirection.Down, 10);
        }

        protected void EnableMousePanCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Map1.MapTools.MouseMapTool.Enabled = EnableMousePanCheckBox.Checked;
            DisableMouseZoomCheckBox.Enabled = EnableMousePanCheckBox.Checked;
        }

        protected void EnableKeyboardPanCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Map1.MapTools.KeyboardMapTool.Enabled = EnableKeyboardPanCheckBox.Checked;
        }

        protected void DisableMouseZoomCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Map1.MapTools.MouseMapTool.IsMouseWheelDisabled = DisableMouseZoomCheckBox.Checked;
        }
    }
}
