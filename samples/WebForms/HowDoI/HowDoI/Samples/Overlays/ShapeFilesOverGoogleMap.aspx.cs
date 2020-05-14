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
    public partial class ShapeFilesOverGoogleMap : System.Web.UI.Page
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

                ShapeFileFeatureLayer shapeFileFeatureLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/USA/STATES.shp"));
                shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(100,212,220,184), GeoColor.FromArgb(255,132,132,154), 1);
                shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                Proj4Projection proj4 = new Proj4Projection();
                proj4.InternalProjectionParametersString = Proj4Projection.GetEpsgParametersString(4326);
                proj4.ExternalProjectionParametersString = Proj4Projection.GetGoogleMapParametersString();
                shapeFileFeatureLayer.FeatureSource.Projection = proj4;

                LayerOverlay shapeOverlay = new LayerOverlay("Shape Overlay", false, TileType.SingleTile);
                shapeOverlay.Layers.Add(shapeFileFeatureLayer);
                shapeOverlay.TransitionEffect = TransitionEffect.None;

                Map1.CustomOverlays.Add(google);
                Map1.CustomOverlays.Add(shapeOverlay);
            }
        }
    }
}
