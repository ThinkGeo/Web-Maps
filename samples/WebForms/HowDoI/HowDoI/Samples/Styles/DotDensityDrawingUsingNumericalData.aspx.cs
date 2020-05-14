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
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;


namespace HowDoI
{
    public partial class DotDensityDrawingUsingNumericalData : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-13064290.945669, 6003560.5647606, -9011293.9803927, 3902459.5429304);

                // Please input your ThinkGeo Cloud API Key to enable the background map. 
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                Map1.CustomOverlays.Add(backgroundOverlay);

                ShapeFileFeatureLayer statesLayer = new ShapeFileFeatureLayer(Server.MapPath("~/SampleData/USA/states.shp"));
                statesLayer.FeatureSource.Projection = new Proj4Projection(4326, 3857);
                statesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                DotDensityStyle dotDensityStyle = new DotDensityStyle();
                dotDensityStyle.ColumnName = "POP1990";
                dotDensityStyle.PointToValueRatio = 0.00002;
                dotDensityStyle.CustomPointStyle = new PointStyle(PointSymbolType.Circle, new GeoSolidBrush(GeoColor.FromArgb(180, GeoColor.StandardColors.OrangeRed)), 4);
                statesLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(dotDensityStyle);

                LayerOverlay staticOverlay = new LayerOverlay();
                staticOverlay.Layers.Add("States", statesLayer);
                staticOverlay.IsBaseOverlay = false;
                Map1.CustomOverlays.Add(staticOverlay);
            }
        }
    }
}
