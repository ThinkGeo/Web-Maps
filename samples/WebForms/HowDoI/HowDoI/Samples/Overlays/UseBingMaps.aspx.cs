using System;
using System.Configuration;
using System.Web.UI.WebControls;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples.Layers
{
    public partial class UseBingMaps : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-13939426.6371, 6701997.4056, -7812401.86, 2626987.386962);
                Map1.MapUnit = GeographyUnit.Meter;

                BingMapsOverlay bingMapsOverlay = new BingMapsOverlay("BingMapsMap");
                bingMapsOverlay.MapType = BingMapsStyle.Road;
                Map1.CustomOverlays.Add(bingMapsOverlay);
            }
        }

        protected void MapTypeChanged(object sender, EventArgs e)
        {
            Button button = (Button)sender;

            var bingMapsOverlay = Map1.CustomOverlays["BingMapsMap"] as BingMapsOverlay;
            bingMapsOverlay.MapType = (BingMapsStyle)Enum.Parse(typeof(BingMapsStyle), button.Text);
        }
    }
}
