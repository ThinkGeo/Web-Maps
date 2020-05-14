using System;
using System.Configuration;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples.Layers
{
    public partial class UseGoogleMap : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-13939426.6371, 6701997.4056, -7812401.86, 2626987.386962);
                Map1.MapUnit = GeographyUnit.Meter;

                Map1.MapTools.OverlaySwitcher.Enabled = true;
                Map1.MapTools.MouseCoordinate.Enabled = true;

                GoogleOverlay google = new GoogleOverlay("Google Map");
                google.JavaScriptLibraryUri = new Uri(ConfigurationManager.AppSettings["GoogleUriV3"]);
                google.GoogleMapType = GoogleMapType.Normal;

                Map1.CustomOverlays.Add(google);
            }
        }
    }
}
