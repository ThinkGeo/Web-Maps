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
    public partial class LoadAWmsOverlay : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapBackground = new GeoSolidBrush(GeoColor.StandardColors.White);
                Map1.CurrentExtent = new RectangleShape(-131.22, 55.05, -54.03, 16.91);
                Map1.MapUnit = GeographyUnit.DecimalDegree;

                WmsOverlay wms = new WmsOverlay("WMS Overlay");
                wms.Parameters.Add("layers", "OSM-WMS");
                wms.Parameters.Add("STYLE", "default");
                wms.ServerUris.Add(new Uri("http://ows.mundialis.de/services/service"));
                wms.TileType = TileType.MultipleTile;
                wms.TileHeight = 256;
                wms.TileWidth = 256;

                Map1.CustomOverlays.Add(wms);
            }
        }
    }
}
