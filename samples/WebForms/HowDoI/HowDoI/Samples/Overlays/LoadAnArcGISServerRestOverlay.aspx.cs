using System;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI
{
    public partial class LoadAnArcGISServerRestOverlay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapBackground = new GeoSolidBrush(GeoColor.StandardColors.White);
                Map1.CurrentExtent = new RectangleShape(-174, 71, -63.5, 18.4);
                Map1.MapUnit = GeographyUnit.DecimalDegree;

                ArcGISServerRestOverlay arcGISServerRestOverlay = new ArcGISServerRestOverlay("ArcGIS Overlay");
                arcGISServerRestOverlay.Parameters.Add("format", "jpeg");
                arcGISServerRestOverlay.ServerUri = new Uri("http://sampleserver1.arcgisonline.com/ArcGIS/rest/services/Specialty/ESRI_StateCityHighway_USA/MapServer/export");

                Map1.CustomOverlays.Add(arcGISServerRestOverlay);
            }
        }
    }
}
