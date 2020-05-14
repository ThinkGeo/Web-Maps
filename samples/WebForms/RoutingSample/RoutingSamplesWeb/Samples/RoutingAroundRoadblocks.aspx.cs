/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Web.UI;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Routing;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace ThinkGeo.MapSuite.RoutingSamples
{
    public partial class RoutingAroundRoadblocks : System.Web.UI.Page
    {
        private static RoutingEngine routingEngine;
        private static RoutingSource routingSource;
        private static ShapeFileFeatureSource featureSource;
        private static InMemoryFeatureLayer roadblocksLayer;

        private static bool isAddingRoadblocks;
        private static Collection<string> avoidableFeatureIds;

        private static Feature startFeature;
        private static Feature endFeature;

        private static string rootPath;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                rootPath = Path.Combine(MapPath("~"), ConfigurationManager.AppSettings["RootDirectory"]);
                featureSource = new ShapeFileFeatureSource(Path.Combine(rootPath, "Austinstreets.shp"));
                routingSource = new RtgRoutingSource(Path.Combine(rootPath, "Austinstreets.rtg"));
                routingEngine = new RoutingEngine(routingSource, featureSource);
                routingEngine.GeographyUnit = GeographyUnit.Meter;
                routingEngine.RoutingAlgorithm.FindingRoute += new EventHandler<FindingRouteRoutingAlgorithmEventArgs>(RoutingAlgorithm_FindingRoute);

                isAddingRoadblocks = false;
                avoidableFeatureIds = new Collection<string>();
                RenderMap();
                Route();
            }
        }

        protected void btnGetRoute_Click(object sender, EventArgs e)
        {
            Route();
        }

        private void Route()
        {
            RoutingLayer routingLayer = (RoutingLayer)Map1.DynamicOverlay.Layers["RoutingLayer"];

            avoidableFeatureIds.Clear();
            foreach (Feature feature in roadblocksLayer.InternalFeatures)
            {
                avoidableFeatureIds.Add(feature.Id);
            }

            routingLayer.Routes.Clear();
            RoutingResult routingResult = routingEngine.GetRoute(routingLayer.StartPoint, routingLayer.EndPoint);
            if (routingResult.Features.Count == 0)
            {
                ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "messageBox", "window.alert('No routes exist!')", true);
            }
            else
            {
                routingLayer.Routes.Add(routingResult.Route);
            }

            Map1.DynamicOverlay.Redraw();
        }

        void RoutingAlgorithm_FindingRoute(object sender, FindingRouteRoutingAlgorithmEventArgs e)
        {
            Collection<string> startPointAdjacentIds = e.RouteSegment.StartPointAdjacentIds;
            Collection<string> endPointAdjacentIds = e.RouteSegment.EndPointAdjacentIds;

            Collection<string> beContainedFeatureIds = new Collection<string>();
            foreach (string featureId in startPointAdjacentIds)
            {
                if (avoidableFeatureIds.Contains(featureId))
                {
                    beContainedFeatureIds.Add(featureId);
                }
            }
            foreach (string featureId in endPointAdjacentIds)
            {
                if (avoidableFeatureIds.Contains(featureId))
                {
                    beContainedFeatureIds.Add(featureId);
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

        protected void Map1_Click(object sender, MapClickedEventArgs e)
        {
            if (isAddingRoadblocks)
            {
                featureSource.Open();
                Collection<Feature> closestFeatures = featureSource.GetFeaturesNearestTo(e.Position, Map1.MapUnit, 1, ReturningColumnsType.NoColumns);
                if (closestFeatures.Count > 0)
                {
                    PointShape position = ((LineBaseShape)closestFeatures[0].GetShape()).GetCenterPoint();
                    Feature feature = new Feature(position.GetWellKnownBinary(), closestFeatures[0].Id);
                    if (feature.Id != startFeature.Id && feature.Id != endFeature.Id)
                    {
                        roadblocksLayer.InternalFeatures.Add(feature);
                    }
                    btnGetRoute_Click(null, null);
                }
                Route();
            }
        }

        protected void btnBegin_Click(object sender, EventArgs e)
        {
            isAddingRoadblocks = true;
        }

        protected void btnStop_Click(object sender, EventArgs e)
        {
            isAddingRoadblocks = false;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            roadblocksLayer.InternalFeatures.Clear();
            Route();
        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            int featureCount = roadblocksLayer.InternalFeatures.Count;
            if (featureCount > 0)
            {
                roadblocksLayer.InternalFeatures.RemoveAt(roadblocksLayer.InternalFeatures.Count - 1);
            }
            Route();
        }

        private void RenderMap()
        {
            Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
            Map1.MapUnit = GeographyUnit.Meter;
            Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            Map1.CurrentExtent = new RectangleShape(-10878225.2183823, 3539294.69393627, -10876723.9417247, 3538109.53891539);

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            Map1.BackgroundOverlay = backgroundOverlay;

            Proj4Projection proj4 = new Proj4Projection(4326, 3857);
            proj4.Open();

            RoutingLayer routingLayer = new RoutingLayer();
            string[] startCoordinates = txtStart.Value.Split(',');
            var startPoint = new PointShape(double.Parse(startCoordinates[0], CultureInfo.InvariantCulture), double.Parse(startCoordinates[1], CultureInfo.InvariantCulture));
            startPoint = (PointShape)proj4.ConvertToExternalProjection(startPoint);
            routingLayer.StartPoint = startPoint;

            string[] endCoordinates = txtEnd.Value.Split(',');
            var endPoint = new PointShape(double.Parse(endCoordinates[0], CultureInfo.InvariantCulture), double.Parse(endCoordinates[1], CultureInfo.InvariantCulture));
            endPoint = (PointShape)proj4.ConvertToExternalProjection(endPoint);
            routingLayer.EndPoint = endPoint;
            Map1.DynamicOverlay.Layers.Add("RoutingLayer", routingLayer);

            featureSource.Open();
            startFeature = featureSource.GetFeaturesNearestTo(routingLayer.StartPoint, Map1.MapUnit, 1, ReturningColumnsType.NoColumns)[0];
            endFeature = featureSource.GetFeaturesNearestTo(routingLayer.EndPoint, Map1.MapUnit, 1, ReturningColumnsType.NoColumns)[0];

            roadblocksLayer = new InMemoryFeatureLayer();
            roadblocksLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle(new GeoImage(Server.MapPath(@"../theme/default/samplepic/roadblock.png")));
            roadblocksLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            Map1.DynamicOverlay.Layers.Add("roadblocksLayer", roadblocksLayer);

            InMemoryFeatureLayer routingExtentLayer = new InMemoryFeatureLayer();
            routingExtentLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoColor.SimpleColors.Green));
            routingExtentLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            routingExtentLayer.InternalFeatures.Add(new Feature(new RectangleShape(-10888761.5216158, 3551192.52708513, -10871238.72057, 3531592.55348582)));
            Map1.DynamicOverlay.Layers.Add("RoutingExtentLayer", routingExtentLayer);
        }
    }
}
