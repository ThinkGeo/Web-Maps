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
    public partial class RouteAvoidingCertainArea : System.Web.UI.Page
    {
        private static FeatureSource featureSource;
        private static PolygonShape avoidableArea;
        private static RoutingEngine routingEngine;
        private static Collection<string> avoidableFeatureIds;
        private static EventHandler<FindingRouteRoutingAlgorithmEventArgs> findingRoute;
        private static string rootPath;


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                rootPath = Path.Combine(MapPath("~"), ConfigurationManager.AppSettings["RootDirectory"]);
                featureSource = new ShapeFileFeatureSource(Path.Combine(rootPath, "Austinstreets.shp"));

                avoidableArea = new PolygonShape(txtAvoidWKT.Text);
                Proj4Projection proj4 = new Proj4Projection(4326, 3857);
                proj4.Open();
                avoidableArea = (PolygonShape)proj4.ConvertToExternalProjection(avoidableArea);

                featureSource.Open();
                Collection<Feature> features = featureSource.SpatialQuery(avoidableArea, QueryType.Within, ReturningColumnsType.NoColumns);
                featureSource.Close();
                avoidableFeatureIds = new Collection<string>();
                foreach (Feature item in features)
                {
                    avoidableFeatureIds.Add(item.Id);
                }

                RoutingSource routingSource = new RtgRoutingSource(Path.Combine(rootPath, "Austinstreets.rtg"));
                routingEngine = new RoutingEngine(routingSource, new AStarRoutingAlgorithm(), featureSource);
                routingEngine.GeographyUnit = GeographyUnit.Meter;
                RenderMap();
                findingRoute = new EventHandler<FindingRouteRoutingAlgorithmEventArgs>(Algorithm_FindingPath);
            }
        }

        protected void idAvoidArea_CheckedChanged(object sender, EventArgs e)
        {
            if (idAvoidArea.Checked)
            {
                routingEngine.RoutingAlgorithm.FindingRoute += findingRoute;
            }
            else
            {
                routingEngine.RoutingAlgorithm.FindingRoute -= findingRoute;
            }
            Route();
        }

        private void Algorithm_FindingPath(object sender, FindingRouteRoutingAlgorithmEventArgs e)
        {
            Collection<string> beContainedFeatureIds = new Collection<string>();

            featureSource.Open();
            Collection<string> startPointAdjacentIds = e.RouteSegment.StartPointAdjacentIds;
            Collection<string> endPointAdjacentIds = e.RouteSegment.EndPointAdjacentIds;
            foreach (string id in startPointAdjacentIds)
            {
                if (avoidableFeatureIds.Contains(id))
                {
                    beContainedFeatureIds.Add(id);
                }
            }
            foreach (string id in endPointAdjacentIds)
            {
                if (avoidableFeatureIds.Contains(id))
                {
                    beContainedFeatureIds.Add(id);
                }
            }

            // Remove the ones that be contained in the avoidable area
            foreach (string id in beContainedFeatureIds)
            {
                if (e.RouteSegment.StartPointAdjacentIds.Contains(id))
                {
                    e.RouteSegment.StartPointAdjacentIds.Remove(id);
                }
                if (e.RouteSegment.EndPointAdjacentIds.Contains(id))
                {
                    e.RouteSegment.EndPointAdjacentIds.Remove(id);
                }
            }
        }

        private void Route()
        {
            RoutingResult routingResult = routingEngine.GetRoute(txtStartId.Value, txtEndId.Value);

            RoutingLayer routingLayer = (RoutingLayer)Map1.DynamicOverlay.Layers["RoutingLayer"];
            routingLayer.Routes.Clear();
            routingLayer.Routes.Add(routingResult.Route);

            Map1.DynamicOverlay.Redraw();
        }

        private void RenderMap()
        {
            Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
            Map1.MapUnit = GeographyUnit.Meter;
            Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            Map1.CurrentExtent = new RectangleShape(-10882970.1251073, 3542132.85056385, -10877292.6084378, 3536925.3352412);

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            Map1.BackgroundOverlay = backgroundOverlay;

            featureSource = new ShapeFileFeatureSource(Path.Combine(rootPath, "Austinstreets.shp"));
            featureSource.Open();

            RoutingLayer routingLayer = new RoutingLayer();
            routingLayer.StartPoint = featureSource.GetFeatureById(txtStartId.Value, ReturningColumnsType.NoColumns).GetShape().GetCenterPoint();
            routingLayer.EndPoint = featureSource.GetFeatureById(txtEndId.Value, ReturningColumnsType.NoColumns).GetShape().GetCenterPoint();
            Map1.DynamicOverlay.Layers.Add("RoutingLayer", routingLayer);

            InMemoryFeatureLayer avoidableAreaLayer = new InMemoryFeatureLayer();
            avoidableAreaLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush = new GeoSolidBrush(GeoColor.FromArgb(100, GeoColor.StandardColors.Blue));
            avoidableAreaLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            avoidableAreaLayer.InternalFeatures.Add("avoidableArea", new Feature(avoidableArea));
            Map1.DynamicOverlay.Layers.Add("avoidableAreaLayer", avoidableAreaLayer);

            InMemoryFeatureLayer routingExtentLayer = new InMemoryFeatureLayer();
            routingExtentLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoColor.SimpleColors.Green));
            routingExtentLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            routingExtentLayer.InternalFeatures.Add(new Feature(new RectangleShape(-10888761.5216158, 3551192.52708513, -10871238.72057, 3531592.55348582)));
            Map1.DynamicOverlay.Layers.Add("RoutingExtentLayer", routingExtentLayer);

            Route();
        }
    }
}
