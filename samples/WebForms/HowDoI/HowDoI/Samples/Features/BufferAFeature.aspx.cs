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

namespace HowDoI.Samples.Features
{
    public partial class BufferAFeature : System.Web.UI.Page
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

                Proj4Projection proj4 = new Proj4Projection(4326, 3857);
                proj4.Open();

                ShapeFileFeatureLayer citiesLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/USA/cities_a.shp"));
                citiesLayer.FeatureSource.Projection = proj4;
                citiesLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateCompoundCircleStyle(GeoColor.StandardColors.White, 6F, GeoColor.StandardColors.Black, 1F, GeoColor.StandardColors.Black, 3F);
                citiesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                citiesLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = new TextStyle("AREANAME", new GeoFont("Verdana", 9), new GeoSolidBrush(GeoColor.StandardColors.Black));
                citiesLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle.HaloPen = new GeoPen(GeoColor.StandardColors.White, 2);

                InMemoryFeatureLayer mapShapeLayer = new InMemoryFeatureLayer();
                mapShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.FromArgb(150, 60, 180, 60);
                mapShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.GeographicColors.DeepOcean;
                mapShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                mapShapeLayer.InternalFeatures.Add("POLYGON", new Feature(BaseShape.CreateShapeFromWellKnownData("POLYGON((-12190788.7671462 4089686.76137006,-12190788.7671462 5302895.27431239,-10067673.8694971 5302895.27431239,-10067673.8694971 4089686.76137006,-12190788.7671462 4089686.76137006))")));

                InMemoryFeatureLayer bufferLayer = new InMemoryFeatureLayer();
                bufferLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.FromArgb(100, 200, 255, 200);
                bufferLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.StandardColors.LightGreen;
                bufferLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                LayerOverlay staticOverlay = new LayerOverlay();
                staticOverlay.IsBaseOverlay = false;
                staticOverlay.Layers.Add("CitiesLayer", citiesLayer);
                Map1.CustomOverlays.Add(staticOverlay);

                LayerOverlay dynamicOverlay = new LayerOverlay();
                dynamicOverlay.TileType = TileType.SingleTile;
                dynamicOverlay.IsBaseOverlay = false;
                dynamicOverlay.Layers.Add("InMemoryFeatureLayer", mapShapeLayer);
                dynamicOverlay.Layers.Add("BufferLayer", bufferLayer);
                Map1.CustomOverlays.Add(dynamicOverlay);
            }
        }

        protected void btnBuffer_Click(object sender, EventArgs e)
        {
            InMemoryFeatureLayer mapShapeLayer = (InMemoryFeatureLayer)((LayerOverlay)(Map1.CustomOverlays[2])).Layers["InMemoryFeatureLayer"];
            InMemoryFeatureLayer bufferLayer = (InMemoryFeatureLayer)((LayerOverlay)(Map1.CustomOverlays[2])).Layers["BufferLayer"];

            AreaBaseShape baseShape = (AreaBaseShape)mapShapeLayer.InternalFeatures["POLYGON"].GetShape();
            MultipolygonShape bufferedShape = baseShape.Buffer(50000, 8, BufferCapType.Round, GeographyUnit.Meter, DistanceUnit.Meter);
            Feature bufferFeature = new Feature(bufferedShape);

            bufferLayer.InternalFeatures.Clear();
            bufferLayer.InternalFeatures.Add("BufferFeature", bufferFeature);
            ((LayerOverlay)(Map1.CustomOverlays[2])).Redraw();
        }
    }
}
