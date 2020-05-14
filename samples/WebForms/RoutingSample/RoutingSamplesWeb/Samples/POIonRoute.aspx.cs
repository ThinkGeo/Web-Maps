/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Routing;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace ThinkGeo.MapSuite.RoutingSamples
{
    public partial class POIonRoute : System.Web.UI.Page
    {
        private static RoutingEngine routingEngine;
        private static RoutingSource RoutingSourceForShortest;
        private static string rootPath;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                rootPath = Path.Combine(MapPath("~"), ConfigurationManager.AppSettings["RootDirectory"]);
                ShapeFileFeatureSource featureSource = new ShapeFileFeatureSource(Path.Combine(rootPath, "Austinstreets.shp"));

                RoutingSourceForShortest = new RtgRoutingSource(Path.Combine(rootPath, "Austinstreets.rtg"));
                routingEngine = new RoutingEngine(RoutingSourceForShortest, featureSource);
                routingEngine.GeographyUnit = GeographyUnit.Meter;

                RenderMap();
            }
        }

        private void RenderMap()
        {
            Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
            Map1.MapUnit = GeographyUnit.Meter;
            Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            Map1.CurrentExtent = new RectangleShape(-10882214.9336817, 3542789.51736921, -10878207.4320132, 3539128.19368793);

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            Map1.BackgroundOverlay = backgroundOverlay;

            ShapeFileFeatureSource featureSource = new ShapeFileFeatureSource(Path.Combine(rootPath, "Austinstreets.shp"));
            featureSource.Open();
            RoutingLayer routingLayer = new RoutingLayer();
            routingLayer.StartPoint = featureSource.GetFeatureById(txtStartFeatureId.Value, ReturningColumnsType.NoColumns).GetShape().GetCenterPoint();
            routingLayer.EndPoint = featureSource.GetFeatureById(txtEndFeatureId.Value, ReturningColumnsType.NoColumns).GetShape().GetCenterPoint();
            Map1.DynamicOverlay.Layers.Add("RoutingLayer", routingLayer);

            ShapeFileFeatureLayer poiLayer = new ShapeFileFeatureLayer(Path.Combine(rootPath, "poi.shp"));
            poiLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle(new GeoImage(Server.MapPath(@"../theme/default/samplepic/Station.png")));
            poiLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            Map1.DynamicOverlay.Layers.Add("POILayer", poiLayer);

            InMemoryFeatureLayer routingExtentLayer = new InMemoryFeatureLayer();
            routingExtentLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoColor.SimpleColors.Green));
            routingExtentLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            routingExtentLayer.InternalFeatures.Add(new Feature(new RectangleShape(-10886222.4353503, 3546451.90190406, -10874199.9303446, 3535467.92934678)));
            Map1.DynamicOverlay.Layers.Add("RoutingExtentLayer", routingExtentLayer);
        }

        protected void btnRoute_Click(object sender, EventArgs e)
        {
            RoutingLayer routingLayer = (RoutingLayer)Map1.DynamicOverlay.Layers["RoutingLayer"];
            routingLayer.Routes.Clear();
            routingLayer.Routes.Add(routingEngine.GetRoute(txtStartFeatureId.Value, txtEndFeatureId.Value).Route);

            InMemoryFeatureLayer POIsOnRouteLayer = new InMemoryFeatureLayer();
            POIsOnRouteLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle(new GeoImage(Server.MapPath(@"../theme/default/samplepic/Gas Station.png")));
            POIsOnRouteLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            Map1.DynamicOverlay.Layers.Add(POIsOnRouteLayer);

            ShapeFileFeatureLayer poiLayer = (ShapeFileFeatureLayer)Map1.DynamicOverlay.Layers["POILayer"];
            poiLayer.Open();
            Collection<Feature> features = poiLayer.QueryTools.GetFeaturesWithinDistanceOf(routingLayer.Routes[0], GeographyUnit.Meter, DistanceUnit.Meter,
                                            100, ReturningColumnsType.NoColumns);
            poiLayer.Close();
            POIsOnRouteLayer.Open();
            POIsOnRouteLayer.EditTools.BeginTransaction();
            foreach (Feature feature in features)
            {
                POIsOnRouteLayer.EditTools.Add(feature);
            }
            POIsOnRouteLayer.EditTools.CommitTransaction();
            POIsOnRouteLayer.Close();
            Map1.DynamicOverlay.Redraw();
        }
    }
}
