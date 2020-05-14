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

namespace HowDoI
{
    public partial class AddProjectedMarkers : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.CurrentExtent = new RectangleShape(-13939426.6371, 6701997.4056, -7812401.86, 2626987.386962);
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.MapTools.PanZoomBar.Enabled = true;
                Map1.MapTools.MouseCoordinate.Enabled = true;

                // Please input your ThinkGeo Cloud API Key to enable the background map. 
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                Map1.CustomOverlays.Add(backgroundOverlay);

                Proj4Projection proj4 = new Proj4Projection();
                proj4.InternalProjectionParametersString = Proj4Projection.GetEpsgParametersString(4326);
                proj4.ExternalProjectionParametersString = Proj4Projection.GetGoogleMapParametersString();

                FeatureSourceMarkerOverlay markerOverlay = new FeatureSourceMarkerOverlay("Markers");
                markerOverlay.FeatureSource = new ShapeFileFeatureSource(MapPath("~/SampleData/USA/cities_a.shp"));
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage = new WebImage("../../theme/default/samplepic/circle.png");
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.ContentHtml = "<span class='popup'>[#AREANAME#]</span>";
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.AutoSize = true;
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.BackgroundColor = GeoColor.StandardColors.White;
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.BorderColor = GeoColor.StandardColors.Black;
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.BorderWidth = 1;
                markerOverlay.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                markerOverlay.FeatureSource.Projection = proj4;
                Map1.CustomOverlays.Add(markerOverlay);
            }
        }
    }
}
