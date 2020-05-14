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
    public partial class OptimizeRoadType : System.Web.UI.Page
    {
        private static RoutingEngine routingEngine;
        private static FeatureSource featureSource;
        private static string routeType;
        private static string rootPath;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                rootPath = Path.Combine(MapPath("~"), ConfigurationManager.AppSettings["RootDirectory"]);
                RenderMap();

                featureSource = new ShapeFileFeatureSource(Path.Combine(rootPath, "Austinstreets.shp"));
                featureSource.Open();
                RoutingSource routingSource = new RtgRoutingSource(Path.Combine(rootPath, "HighwayFirst.rtg"));
                routingEngine = new RoutingEngine(routingSource, featureSource);
                routingEngine.GeographyUnit = GeographyUnit.Meter;
                routingEngine.RoutingAlgorithm.FindingRoute += new EventHandler<FindingRouteRoutingAlgorithmEventArgs>(Algorithm_FindingPath);
                Route();
            }
        }

        protected void ddlRouteType_SelectedIndexChanged(object sender, EventArgs e)
        {
            Route();
        }

        private void Route()
        {
            routeType = ddlRouteType.SelectedValue;
            RoutingResult routingResult = routingEngine.GetRoute(txtStartFeatureId.Value, txtEndFeatureId.Value);
            RoutingLayer routingLayer = (RoutingLayer)Map1.DynamicOverlay.Layers["RoutingLayer"];
            routingLayer.Routes.Clear();
            routingLayer.Routes.Add(routingResult.Route);

            Map1.DynamicOverlay.Redraw();
        }

        void Algorithm_FindingPath(object sender, FindingRouteRoutingAlgorithmEventArgs e)
        {
            Collection<string> localRoads = new Collection<string>();
            Collection<string> highways = new Collection<string>();

            foreach (string item in e.RouteSegment.StartPointAdjacentIds)
            {
                RouteSegment road = routingEngine.RoutingSource.GetRouteSegmentByFeatureId(item);
                if (road.RouteSegmentType == 2)
                {
                    highways.Add(item);
                }
                else
                {
                    localRoads.Add(item);
                }
            }

            foreach (string item in e.RouteSegment.EndPointAdjacentIds)
            {
                RouteSegment road = routingEngine.RoutingSource.GetRouteSegmentByFeatureId(item);
                if (road.RouteSegmentType == 2)
                {
                    highways.Add(item);
                }
                else
                {
                    localRoads.Add(item);
                }
            }

            if (routeType == "On Foot")
            {
                if (localRoads.Count > 0)
                {
                    foreach (string item in highways)
                    {
                        e.RouteSegment.StartPointAdjacentIds.Remove(item);
                        e.RouteSegment.EndPointAdjacentIds.Remove(item);
                    }
                }
            }
        }

        private void RenderMap()
        {
            Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
            Map1.MapUnit = GeographyUnit.Meter;
            Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            Map1.CurrentExtent = new RectangleShape(-10882957.8938171, 3539283.97140315, -10879422.6768379, 3536363.44924643);

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            Map1.BackgroundOverlay = backgroundOverlay;

            FeatureSource featureSource = new ShapeFileFeatureSource(Path.Combine(rootPath, "Austinstreets.shp"));
            featureSource.Open();
            RoutingLayer routingLayer = new RoutingLayer();
            routingLayer.StartPoint = featureSource.GetFeatureById(txtStartFeatureId.Value, ReturningColumnsType.NoColumns).GetShape().GetCenterPoint();
            routingLayer.EndPoint = featureSource.GetFeatureById(txtEndFeatureId.Value, ReturningColumnsType.NoColumns).GetShape().GetCenterPoint();
            featureSource.Close();
            Map1.DynamicOverlay.Layers.Add("RoutingLayer", routingLayer);

            InMemoryFeatureLayer routingExtentLayer = new InMemoryFeatureLayer();
            routingExtentLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoColor.SimpleColors.Green));
            routingExtentLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            routingExtentLayer.InternalFeatures.Add(new Feature(new RectangleShape(-10888761.5216158, 3551192.52708513, -10871238.72057, 3531592.55348582)));
            Map1.DynamicOverlay.Layers.Add("RoutingExtentLayer", routingExtentLayer);
        }
    }
}
