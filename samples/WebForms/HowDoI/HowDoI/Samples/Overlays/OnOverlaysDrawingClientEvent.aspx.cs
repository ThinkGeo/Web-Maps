using System;
using System.Configuration;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples.Overlays
{
    public partial class OnOverlaysDrawingClientEvent : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-10620313.58252, 4795964.90206, -10431514.12268, 4667244.94644);
                Map1.MapUnit = GeographyUnit.Meter;

                Map1.MapTools.OverlaySwitcher.Enabled = true;
                Map1.MapTools.MouseCoordinate.Enabled = true;

                GoogleMapsLayer google = new GoogleMapsLayer();
                LayerOverlay staticOverlay = new LayerOverlay("Google Map");
                staticOverlay.Layers.Add(google);
                Map1.CustomOverlays.Add(staticOverlay);
            }
        }
    }
}
