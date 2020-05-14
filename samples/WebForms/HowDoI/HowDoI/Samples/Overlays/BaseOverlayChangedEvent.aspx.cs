using System;
using System.Configuration;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples.Layers
{
    public partial class BaseOverlayChangedEvent : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-13939426.6371, 6701997.4056, -7812401.86, 2626987.386962);
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.MapTools.OverlaySwitcher.Enabled = true;

                OpenStreetMapOverlay osmOverlay = new OpenStreetMapOverlay("OSM Map");
                osmOverlay.MapType = OpenStreetMapType.Standard;
                Map1.CustomOverlays.Add(osmOverlay);
                plControls.Visible = true;

                BingMapsOverlay bingMapsOverlay = new BingMapsOverlay("Bing Map");
                bingMapsOverlay.MapType = BingMapsStyle.Road;
                Map1.CustomOverlays.Add(bingMapsOverlay);

                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02.shp"));
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 243, 239, 228), GeoColor.FromArgb(255, 218, 193, 163), 1);
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                Proj4Projection proj4 = new Proj4Projection();
                proj4.InternalProjectionParametersString = Proj4Projection.GetEpsgParametersString(4326);
                proj4.ExternalProjectionParametersString = Proj4Projection.GetGoogleMapParametersString();
                worldLayer.FeatureSource.Projection = proj4;

                LayerOverlay worldOverlay = new LayerOverlay("WorldOverlay");
                worldOverlay.Layers.Add("WorldLayer", worldLayer);
                worldOverlay.Name = "ThinkGeoMap";
                Map1.CustomOverlays.Add(worldOverlay);

                // The following two lines of code enable the client and server caching.
                // If you enable these features it will greatly increase the scalability of your
                // mapping application however there some side effects that may be counter intuitive.
                // Please read the white paper on web caching or the documentation regarding these methods.

                //worldOverlay.ClientCache.CacheId = "WorldOverlay";
                //worldOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/BaseOverlayChangedEvent");
            }
        }

        protected void Map1_BaseOverlayChanged(object sender, BaseOverlayChangedEventArgs e)
        {
            if (e.CurrentBaseOverlayId == "OSM Map")
            {
                plControls.Visible = true;
            }
            else
            {
                plControls.Visible = false;
            }
        }
    }
}
