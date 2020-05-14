/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Collections.ObjectModel;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples.Features
{
    public partial class EnvelopeOfAFeature : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-8956259.3203343, 7303601.534613, 7255728.5407719, -1100802.5527076);

                // Please input your ThinkGeo Cloud API Key to enable the background map. 
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                Map1.CustomOverlays.Add(backgroundOverlay);

                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02_3857.shp"));
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.SimpleColors.Transparent, GeoColor.FromArgb(100, GeoColor.SimpleColors.Green));
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                // Setup the BoundingBox InMemoryFeatureLayer.
                InMemoryFeatureLayer boundingBoxLayer = new InMemoryFeatureLayer();
                boundingBoxLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.FromArgb(100, 200, 255, 200);
                boundingBoxLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.GeographicColors.DeepOcean;
                boundingBoxLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                LayerOverlay staticOverlay = new LayerOverlay();
                LayerOverlay dynamicOverlay = new LayerOverlay();

                staticOverlay.Layers.Add("WorldLayer", worldLayer);
                staticOverlay.IsBaseOverlay = false;
                dynamicOverlay.Layers.Add("BoundingBoxLayer", boundingBoxLayer);
                dynamicOverlay.TileType = TileType.SingleTile;
                dynamicOverlay.IsBaseOverlay = false;

                Map1.CustomOverlays.Add(staticOverlay);
                Map1.CustomOverlays.Add(dynamicOverlay);                
            }
        }

        protected void Map1_Click(object sender, ThinkGeo.MapSuite.WebForms.MapClickedEventArgs e)
        {
            ShapeFileFeatureLayer worldLayer = (ShapeFileFeatureLayer)((LayerOverlay)Map1.CustomOverlays[1]).Layers["WorldLayer"];
            InMemoryFeatureLayer boundingBoxLayer = (InMemoryFeatureLayer)((LayerOverlay)Map1.CustomOverlays[2]).Layers["BoundingBoxLayer"];
            boundingBoxLayer.InternalFeatures.Clear();

            worldLayer.Open();
            Collection<Feature> selectedFeatures = worldLayer.QueryTools.GetFeaturesContaining(e.Position, new string[0]);
            worldLayer.Close();

            if (selectedFeatures.Count > 0)
            {
                AreaBaseShape areaShape = (AreaBaseShape)selectedFeatures[0].GetShape();
                boundingBoxLayer.InternalFeatures.Add("BoundingBox", new Feature(areaShape.GetBoundingBox()));
            }
            ((LayerOverlay)Map1.CustomOverlays[2]).Redraw();
        }
    }
}
