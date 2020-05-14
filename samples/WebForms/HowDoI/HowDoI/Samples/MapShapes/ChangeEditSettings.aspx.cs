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

namespace HowDoI.Samples.Features
{
    public partial class ChangeEditSettings : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-4893709.134029, 7937434.644296, 11318278.727077, -466969.44302459);

                // Please input your ThinkGeo Cloud API Key to enable the background map.
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                Map1.CustomOverlays.Add(backgroundOverlay);

                Map1.EditOverlay.Features.Add(new Feature(new RectangleShape(-1113194.90793274, 6446275.84101716, 6679169.44759641, 1118889.97485796)));          
            }
        }

        protected void CheckBoxChanged(object sender, EventArgs args)
        {
            Map1.EditOverlay.TrackMode = TrackMode.Edit;
            Map1.EditOverlay.EditSettings.IsDraggable = CheckBoxDrag.Checked;
            Map1.EditOverlay.EditSettings.IsReshapable = CheckBoxReshape.Checked;
            Map1.EditOverlay.EditSettings.IsResizable = CheckBoxResize.Checked;
            Map1.EditOverlay.EditSettings.IsRotatable = CheckBoxRotate.Checked;
        }
    }
}
