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
    public partial class SetTheZoomLevelsOnAMarker : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.MapTools.PanZoomBar.Enabled = true;
                Map1.MapTools.MouseCoordinate.Enabled = true;
                Map1.CurrentExtent = new RectangleShape(-13064290.945669, 6003560.5647606, -9011293.9803927, 3902459.5429304);

                // Please input your ThinkGeo Cloud API Key to enable the background map. 
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                Map1.CustomOverlays.Add(backgroundOverlay);

                FeatureSourceMarkerOverlay markerOverlay = new FeatureSourceMarkerOverlay("Markers");
                markerOverlay.FeatureSource = new ShapeFileFeatureSource(MapPath("~/SampleData/USA/cities_a.shp"));
                markerOverlay.FeatureSource.Projection = new Proj4Projection(4326, 3857);

                MarkerClassBreak classBreak1 = new MarkerClassBreak(double.MinValue);
                classBreak1.DefaultMarkerStyle.WebImage = new WebImage("../../theme/default/samplepic/Industrial.png", 20, 19);
                classBreak1.DefaultMarkerStyle.Popup.ContentHtml = "<span class='popCss'>Population in 2000 is <br/><span style='color:red'>[#POP2000#]</span></span>";
                classBreak1.DefaultMarkerStyle.Popup.AutoSize = true;
                classBreak1.DefaultMarkerStyle.Popup.BorderColor = GeoColor.FromHtml("#CCCCCC");
                classBreak1.DefaultMarkerStyle.Popup.BorderWidth = 1;

                MarkerClassBreak classBreak2 = new MarkerClassBreak(400000);
                classBreak2.DefaultMarkerStyle.WebImage = new WebImage("../../theme/default/samplepic/Industrial.png", 40, 38);
                classBreak2.DefaultMarkerStyle.Popup.ContentHtml = "<span class='popCss'>Population in 2000 is <br/><span style='color:red'>[#POP2000#]</span></span>";
                classBreak2.DefaultMarkerStyle.Popup.AutoSize = true;
                classBreak2.DefaultMarkerStyle.Popup.BorderColor = GeoColor.FromHtml("#CCCCCC");
                classBreak2.DefaultMarkerStyle.Popup.BorderWidth = 1;

                MarkerClassBreak classBreak3 = new MarkerClassBreak(600000);
                classBreak3.DefaultMarkerStyle.WebImage = new WebImage("../../theme/default/samplepic/Industrial.png", 60, 56);
                classBreak3.DefaultMarkerStyle.Popup.ContentHtml = "<span class='popCss'>Population in 2000 is <br/><span style='color:red'>[#POP2000#]</span></span>";
                classBreak3.DefaultMarkerStyle.Popup.AutoSize = true;
                classBreak3.DefaultMarkerStyle.Popup.BorderColor = GeoColor.FromHtml("#CCCCCC");
                classBreak3.DefaultMarkerStyle.Popup.BorderWidth = 1;

                ClassBreakMarkerStyle classBreakStyle = new ClassBreakMarkerStyle("POP2000");
                classBreakStyle.ClassBreaks.Add(classBreak1);
                classBreakStyle.ClassBreaks.Add(classBreak2);
                classBreakStyle.ClassBreaks.Add(classBreak3);

                markerOverlay.ZoomLevelSet.ZoomLevel04.DefaultMarkerStyle.WebImage = new WebImage("../../theme/default/samplepic/circle.png");
                markerOverlay.ZoomLevelSet.ZoomLevel04.DefaultMarkerStyle.Popup.ContentHtml = "<div class='popCss'><span style='color:#ff6500;'><b>[#AREANAME#]</b></span> city with <span style='color:#ff6500;'><b>[#POP2000#]</b></span> thousand people.</div>";
                markerOverlay.ZoomLevelSet.ZoomLevel04.DefaultMarkerStyle.Popup.AutoSize = true;
                markerOverlay.ZoomLevelSet.ZoomLevel04.DefaultMarkerStyle.Popup.BorderColor = GeoColor.StandardColors.Gray;
                markerOverlay.ZoomLevelSet.ZoomLevel04.DefaultMarkerStyle.Popup.BorderWidth = 1;
                markerOverlay.ZoomLevelSet.ZoomLevel04.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level05;
                markerOverlay.ZoomLevelSet.ZoomLevel06.CustomMarkerStyle = classBreakStyle;
                markerOverlay.ZoomLevelSet.ZoomLevel06.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level10;
                Map1.CustomOverlays.Add(markerOverlay);
            }
        }
    }
}
