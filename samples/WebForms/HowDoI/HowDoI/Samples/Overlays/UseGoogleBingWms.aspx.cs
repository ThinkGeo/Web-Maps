using System;
using System.Configuration;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI
{
    public partial class UseGoogleBingWms : System.Web.UI.Page
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
                Map1.MapTools.PanZoomBar.Enabled = true;
                Map1.MapTools.OverlaySwitcher.OnClientBaseOverlayChanged = "onLayerChanged";

                WmsOverlay wms = new WmsOverlay("WMS Overlay");
                wms.Parameters.Add("layers", "Countries02,USSTATES,USMAJORCITIES");
                wms.Parameters.Add("STYLE", "SIMPLE");
                wms.ServerUris.Add(new Uri("http://wmssamples.thinkgeo.com/WmsServer.aspx"));
                wms.SetBaseEpsgProjection("EPSG:900913");

                GoogleOverlay google = new GoogleOverlay("Google Map");
                google.GoogleMapType = GoogleMapType.Normal;
                google.JavaScriptLibraryUri = new Uri(ConfigurationManager.AppSettings["GoogleUriV3"]);

                OpenStreetMapOverlay osmOverlay = new OpenStreetMapOverlay("Open Street Map");

                BingMapsOverlay bing = new BingMapsOverlay("Bing Map");
                bing.MapType = ThinkGeo.MapSuite.WebForms.BingMapsStyle.Road;

                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02_3857.shp"));
                worldLayer.ZoomLevelSet = new SphericalMercatorZoomLevelSet();
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 243, 239, 228), GeoColor.FromArgb(255, 218, 193, 163), 1);
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                Proj4Projection proj4 = new Proj4Projection();
                proj4.InternalProjectionParametersString = Proj4Projection.GetEpsgParametersString(4326);
                proj4.ExternalProjectionParametersString = Proj4Projection.GetGoogleMapParametersString();
                worldLayer.FeatureSource.Projection = proj4;

                LayerOverlay worldOverlay = new LayerOverlay("WorldOverlay");
                worldOverlay.Layers.Add(worldLayer);

                Map1.CustomOverlays.Add(wms);
                Map1.CustomOverlays.Add(osmOverlay);
                Map1.CustomOverlays.Add(google);
                Map1.CustomOverlays.Add(bing);
                Map1.CustomOverlays.Add(worldOverlay);

                Map1.ZoomLevelSet = worldLayer.ZoomLevelSet;
            }
        }
    }
}
