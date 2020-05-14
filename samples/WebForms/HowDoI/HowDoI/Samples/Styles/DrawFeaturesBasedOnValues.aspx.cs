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
    public partial class DrawFeaturesBasedOnValues : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-6021077.450489, 7063895.0152425, 10190910.410617, -1340509.0720781);

                // Please input your ThinkGeo Cloud API Key to enable the background map. 
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                Map1.CustomOverlays.Add(backgroundOverlay);

                ShapeFileFeatureLayer citiesLayer = new ShapeFileFeatureLayer(Server.MapPath("~/SampleData/World/capital.shp"));
                citiesLayer.FeatureSource.Projection = new Proj4Projection(4326, 3857);
                citiesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                // Draw features based on values
                ValueStyle valueStyle = new ValueStyle();
                valueStyle.ColumnName = "POP_RANK";
                valueStyle.ValueItems.Add(new ValueItem("1", PointStyles.CreateCompoundPointStyle(PointSymbolType.Square, GeoColor.StandardColors.White, GeoColor.StandardColors.Black, 1F, 10F, PointSymbolType.Square, GeoColor.StandardColors.Navy, GeoColor.StandardColors.Transparent, 0F, 6F)));
                valueStyle.ValueItems.Add(new ValueItem("2", PointStyles.CreateCompoundPointStyle(PointSymbolType.Square, GeoColor.StandardColors.White, GeoColor.StandardColors.Black, 1F, 6F, PointSymbolType.Square, GeoColor.StandardColors.Maroon, GeoColor.StandardColors.Transparent, 0F, 2F)));
                valueStyle.ValueItems.Add(new ValueItem("3", PointStyles.CreateSimpleCircleStyle(GeoColor.StandardColors.White, 3.2F, GeoColor.StandardColors.Black, 1F)));
                citiesLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(valueStyle);

                LayerOverlay staticOverlay = new LayerOverlay();
                staticOverlay.Layers.Add("CitiesLayer", citiesLayer);
                staticOverlay.IsBaseOverlay = false;
                Map1.CustomOverlays.Add(staticOverlay);
            }
        }
    }
}
