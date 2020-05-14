/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Web.UI;
using System.Collections.ObjectModel;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples.Querying_Vector_Layers
{
    public partial class SpatialQueryAFeatureLayer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-7601183.6904228, 4461367.090647, 8610804.1706833, -3943036.9966736);

                // Please input your ThinkGeo Cloud API Key to enable the background map.
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                Map1.CustomOverlays.Add(backgroundOverlay);

                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02_3857.shp"));
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.SimpleColors.Transparent, GeoColor.FromArgb(100, GeoColor.SimpleColors.Green));
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                InMemoryFeatureLayer rectangleLayer = new InMemoryFeatureLayer();
                rectangleLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(new GeoColor(50, 100, 100, 200)));
                rectangleLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.StandardColors.DarkBlue;
                rectangleLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                BaseShape baseShape = BaseShape.CreateShapeFromWellKnownData("POLYGON((-5565974.53966368 -2273030.92698769,-5565974.53966368 2273030.92698769,5565974.53966368 2273030.92698769,5565974.53966368 -2273030.92698769,-5565974.53966368 -2273030.92698769))");
                baseShape.Id = "Rectangle";
                rectangleLayer.InternalFeatures.Add("Rectangle", new Feature(baseShape));

                InMemoryFeatureLayer spatialQueryResultLayer = new InMemoryFeatureLayer();
                spatialQueryResultLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(GeoColor.FromArgb(200, GeoColor.SimpleColors.PastelRed)));
                spatialQueryResultLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.StandardColors.Red;
                spatialQueryResultLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                LayerOverlay staticOverlay = new LayerOverlay("StaticOverlay");
                staticOverlay.IsBaseOverlay = false;
                Map1.CustomOverlays.Add(staticOverlay);

                staticOverlay.Layers.Add("WorldLayer", worldLayer);
                staticOverlay.Layers.Add("RectangleLayer", rectangleLayer);
                staticOverlay.Layers.Add("SpatialQueryResultLayer", spatialQueryResultLayer);
                
                ddlSpatialQuery.SelectedIndex = 0;
            }
        }

        protected void ddlSpatialQuery_SelectedIndexChanged(object sender, EventArgs e)
        {
            LayerOverlay staticOverlay = (LayerOverlay)Map1.CustomOverlays["StaticOverlay"];
            ShapeFileFeatureLayer worldLayer = (ShapeFileFeatureLayer)staticOverlay.Layers["WorldLayer"];
            InMemoryFeatureLayer rectangleLayer = (InMemoryFeatureLayer)staticOverlay.Layers["RectangleLayer"];
            InMemoryFeatureLayer spatialQueryResultLayer = (InMemoryFeatureLayer)staticOverlay.Layers["SpatialQueryResultLayer"];

            Feature rectangleFeature = rectangleLayer.InternalFeatures["Rectangle"];
            Collection<Feature> spatialQueryResults;
            worldLayer.Open();
            switch (ddlSpatialQuery.SelectedValue.Trim())
            {
                case "Within":
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesWithin(rectangleFeature, new string[0]);
                    break;
                case "Containing":
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesContaining(rectangleFeature, new string[0]);
                    break;
                case "Disjointed":
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesDisjointed(rectangleFeature, new string[0]);
                    break;
                case "Intersecting":
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesIntersecting(rectangleFeature, new string[0]);
                    break;
                case "Overlapping":
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesOverlapping(rectangleFeature, new string[0]);
                    break;
                case "TopologicalEqual":
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesTopologicalEqual(rectangleFeature, new string[0]);
                    break;
                case "Touching":
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesTouching(rectangleFeature, new string[0]);
                    break;
                default:
                    spatialQueryResults = worldLayer.QueryTools.GetFeaturesWithin(rectangleFeature, new string[0]);
                    break;
            }
            worldLayer.Close();

            spatialQueryResultLayer.InternalFeatures.Clear();
            spatialQueryResultLayer.Open();
            foreach (Feature feature in spatialQueryResults)
            {
                spatialQueryResultLayer.InternalFeatures.Add(feature.Id, feature);
            }
            spatialQueryResultLayer.Close();
            staticOverlay.Redraw();
        }
    }
}
