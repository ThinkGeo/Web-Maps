/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples
{
    public partial class LoadAHeatLayer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-10228034.814086, 5337883.48150295, -7779006.01663396, 3485566.81262866);

                // Please input your ThinkGeo Cloud API Key to enable the background map. 
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                Map1.CustomOverlays.Add(backgroundOverlay);

                ShapeFileFeatureSource featureSource = new ShapeFileFeatureSource(MapPath("~/SampleData/USA/quksigx020.shp"));

                HeatLayer heatLayer = new HeatLayer(featureSource);
                heatLayer.HeatStyle = new HeatStyle(180, "other_mag1", 0, 12);

                LayerOverlay heatlayerOverlay = new LayerOverlay();
                heatlayerOverlay.IsBaseOverlay = false;
                heatlayerOverlay.Layers.Add(heatLayer);
                Map1.CustomOverlays.Add(heatlayerOverlay);
            }
        }
    }
}
