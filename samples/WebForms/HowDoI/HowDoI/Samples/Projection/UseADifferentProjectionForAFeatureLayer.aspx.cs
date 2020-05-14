using System;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples.Projection
{
    public partial class UseADifferentProjectionForAFeatureLayer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#94aac6"));
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.CurrentExtent = new RectangleShape(-19268509.29874, 13535292.38285, 20656089.34576, -14435365.7673);

                // If want to know more srids, please refer Projections.rtf in Documentation folder.
                Proj4Projection proj4Projection = new Proj4Projection();
                proj4Projection.InternalProjectionParametersString = Proj4Projection.GetEpsgParametersString(4326);
                proj4Projection.ExternalProjectionParametersString = Proj4Projection.GetEpsgParametersString(2163);

                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02.shp"));
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 243, 239, 228), GeoColor.FromArgb(255, 218, 193, 163), 1);
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                worldLayer.FeatureSource.Projection = proj4Projection;
                Map1.StaticOverlay.Layers.Add("WorldLayer", worldLayer);
                Map1.StaticOverlay.TileType = TileType.SingleTile;

                // The following two lines of code enable the client and server caching.
                // If you enable these features it will greatly increase the scalability of your
                // mapping application however there some side effects that may be counter intuitive.
                // Please read the white paper on web caching or the documentation regarding these methods.

                //Map1.StaticOverlay.ClientCache.CacheId = "WorldOverlay";
                //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path);
            }
        }
    }
}
