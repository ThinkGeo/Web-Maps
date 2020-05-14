/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples.Querying_Vector_Layers
{
    public partial class FindFeaturesWithinDistance : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-7601183.6904228, 5461367.090647, 8610804.1706833, -2943036.9966736);

                // Please input your ThinkGeo Cloud API Key to enable the background map. 
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                Map1.CustomOverlays.Add(backgroundOverlay);

                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02_3857.shp"));
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.SimpleColors.Transparent, GeoColor.FromArgb(100, GeoColor.SimpleColors.Green));
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                LayerOverlay staticOverlay = new LayerOverlay("StaticOverlay");
                staticOverlay.IsBaseOverlay = false;
                staticOverlay.Layers.Add("WorldLayer", worldLayer);
                Map1.CustomOverlays.Add(staticOverlay);

                InMemoryFeatureLayer highlightLayer = new InMemoryFeatureLayer();
                highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateCompoundPointStyle(PointSymbolType.Square, GeoColor.StandardColors.White, GeoColor.StandardColors.Black, 1F, 8F, PointSymbolType.Square, GeoColor.StandardColors.Navy, GeoColor.StandardColors.Transparent, 0F, 4F);
                highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(150, 154, 205, 50));
                highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                LayerOverlay dynamicOverlay = new LayerOverlay("DynamicOverlay");
                dynamicOverlay.IsBaseOverlay = false;
                dynamicOverlay.TileType = TileType.SingleTile;
                dynamicOverlay.Layers.Add("HighlightLayer", highlightLayer);
                Map1.CustomOverlays.Add(dynamicOverlay);
            }
        }

        protected void DistanceDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ViewState["CenterPoint"] != null)
            {
                FindWithinDistanceFeatures();
            }
        }

        protected void Map1_Click(object sender, MapClickedEventArgs e)
        {
            ViewState["CenterPoint"] = e.Position;
            FindWithinDistanceFeatures();
        }

        private void FindWithinDistanceFeatures()
        {
            PointShape pointShape = (PointShape)ViewState["CenterPoint"];

            LayerOverlay staticOverlay = (LayerOverlay)Map1.CustomOverlays["StaticOverlay"];
            LayerOverlay dynamicOverlay = (LayerOverlay)Map1.CustomOverlays["DynamicOverlay"];

            FeatureLayer worldLayer = (FeatureLayer)staticOverlay.Layers["WorldLayer"];
            InMemoryFeatureLayer highlightLayer = (InMemoryFeatureLayer)dynamicOverlay.Layers["HighlightLayer"];

            // Find the countries within special distance.
            double distance = Convert.ToDouble(DistanceDropDownList.SelectedItem.ToString(), CultureInfo.InvariantCulture);
            worldLayer.Open();
            Collection<Feature> selectedFeatures = worldLayer.QueryTools.GetFeaturesWithinDistanceOf(pointShape, GeographyUnit.Meter, DistanceUnit.Kilometer, distance, new string[0]);
            worldLayer.Close();

            if (highlightLayer.InternalFeatures.Count > 0)
            {
                highlightLayer.InternalFeatures.Clear();
            }

            highlightLayer.InternalFeatures.Add("Point", new Feature(pointShape));
            foreach (Feature feature in selectedFeatures)
            {
                highlightLayer.InternalFeatures.Add(feature.Id, feature);
            }

            dynamicOverlay.Redraw();
        }
    }
}
