/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Collections.ObjectModel;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples.NavigateTheMap
{
    public partial class ZoomToAClickedFeature : System.Web.UI.Page
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

                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02_3857.shp"));
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.SimpleColors.Transparent, GeoColor.FromArgb(100, GeoColor.SimpleColors.Green));
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                InMemoryFeatureLayer highlightLayer = new InMemoryFeatureLayer();
                highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(100, 60, 180, 60), GeoColor.GeographicColors.DeepOcean);
                highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = WorldStreetsTextStyles.GeneralPurpose("CNTRY_NAME",8);
                highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                LayerOverlay staticOverlay = new LayerOverlay("StaticOverlay");
                staticOverlay.IsBaseOverlay = false;
                staticOverlay.Layers.Add("WorldLayer", worldLayer);
                Map1.CustomOverlays.Add(staticOverlay);

                LayerOverlay dynamicOverlay = new LayerOverlay("DynamicOverlay");
                dynamicOverlay.IsBaseOverlay = false;
                dynamicOverlay.TileType = TileType.SingleTile;
                dynamicOverlay.Layers.Add("HighlightLayer", highlightLayer);
                Map1.CustomOverlays.Add(dynamicOverlay);
            }
        }

        protected void Map1_Click(object sender, MapClickedEventArgs e)
        {
            LayerOverlay staticOverlay = (LayerOverlay)Map1.CustomOverlays["StaticOverlay"];
            ShapeFileFeatureLayer worldLayer = (ShapeFileFeatureLayer)staticOverlay.Layers["WorldLayer"];

            LayerOverlay dynamicOverlay = (LayerOverlay)Map1.CustomOverlays["DynamicOverlay"];
            InMemoryFeatureLayer highlightLayer = (InMemoryFeatureLayer)dynamicOverlay.Layers["HighlightLayer"];

            highlightLayer.InternalFeatures.Clear();
            worldLayer.Open();
            Collection<Feature> features = worldLayer.QueryTools.GetFeaturesContaining(e.Position, new string[1] { "CNTRY_NAME" });
            worldLayer.Close();

            if (features.Count > 0)
            {
                highlightLayer.InternalFeatures.Add(features[0].Id, features[0]);
                Map1.CurrentExtent = ExtentHelper.GetDrawingExtent(features[0].GetBoundingBox(), (float)Map1.WidthInPixels, (float)Map1.HeightInPixels);
            }

            dynamicOverlay.Redraw();
        }
    }
}
