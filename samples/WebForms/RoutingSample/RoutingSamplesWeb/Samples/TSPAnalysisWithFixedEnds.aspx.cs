/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Routing;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace ThinkGeo.MapSuite.RoutingSamples
{
    public partial class TSPAnalysisWithFixedEnds : System.Web.UI.Page
    {
        private static RoutingEngine routingEngine;
        private static RoutingSource routingSource;
        private static string rootPath;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                rootPath = Path.Combine(MapPath("~"), ConfigurationManager.AppSettings["RootDirectory"]);
                RenderMap();

                ShapeFileFeatureSource featureSource = new ShapeFileFeatureSource(Path.Combine(rootPath, "Austinstreets.shp"));
                routingSource = new RtgRoutingSource(Path.Combine(rootPath, "Austinstreets.rtg"));
                routingEngine = new RoutingEngine(routingSource, new AStarRoutingAlgorithm(), featureSource);
                routingEngine.GeographyUnit = GeographyUnit.Meter;
                routingEngine.SearchRadiusInMeters = 100;
            }
        }

        protected void btnRoute_Click(object sender, EventArgs e)
        {
            RoutingLayer routingLayer = (RoutingLayer)Map1.DynamicOverlay.Layers["RoutingLayer"];
            routingLayer.ShowStopOrder = true;

            Stopwatch watch = new Stopwatch();
            watch.Start();
            //New API for GetRouteViaVisitstops wher you can have a start and an end point.
            RoutingResult routingResult = routingEngine.GetRoute(routingLayer.StartPoint, routingLayer.EndPoint, routingLayer.StopPoints, int.Parse(txtIterations.Value));
            watch.Stop();
            // Render the route
            routingLayer.Routes.Clear();
            routingLayer.Routes.Add(routingResult.Route);
            routingLayer.StopPoints.Clear();

            // Show the visit sequence
            foreach (PointShape stop in routingResult.OrderedStops)
            {
                routingLayer.StopPoints.Add(stop);
            }

            Map1.DynamicOverlay.Redraw();
            txtTime.Text = watch.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture) + " ms";
            txtDistance.Value = routingResult.Distance.ToString("F4", CultureInfo.InvariantCulture) + " Meters";
        }

        private void RenderMap()
        {
            Map1.MapUnit = GeographyUnit.Meter;
            Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
            Map1.CurrentExtent = new RectangleShape(-10879472.0974036, 3541166.40710249, -10875936.8795289, 3538245.45113881);

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            Map1.BackgroundOverlay = backgroundOverlay;

            RoutingLayer routingLayer = new RoutingLayer();
            Map1.DynamicOverlay.Layers.Add("RoutingLayer", routingLayer);

            Proj4Projection proj4 = new Proj4Projection(4326, 3857);
            proj4.Open();

            //Collection<PointShape>  stops = new Collection<PointShape>();
            string[] startCoordinates = txtStartId.Value.Split(',');
            var startPoint = new PointShape(double.Parse(startCoordinates[0], CultureInfo.InvariantCulture), double.Parse(startCoordinates[1], CultureInfo.InvariantCulture));
            startPoint = (PointShape)proj4.ConvertToExternalProjection(startPoint);
            routingLayer.StartPoint = startPoint;

            string[] endCoordinates = txtEndId.Value.Split(',');
            var endPoint = new PointShape(double.Parse(endCoordinates[0], CultureInfo.InvariantCulture), double.Parse(endCoordinates[1], CultureInfo.InvariantCulture));
            endPoint = (PointShape)proj4.ConvertToExternalProjection(endPoint);
            routingLayer.EndPoint = endPoint;

            foreach (object item in lsbLocations.Items)
            {
                string[] coordinate = item.ToString().Split(',');
                PointShape pointNeedVisit = new PointShape(double.Parse(coordinate[0], CultureInfo.InvariantCulture), double.Parse(coordinate[1], CultureInfo.InvariantCulture));
                pointNeedVisit = (PointShape)proj4.ConvertToExternalProjection(pointNeedVisit);
                routingLayer.StopPoints.Add(pointNeedVisit);
                //stops.Add(pointNeedVisit);
            }
            routingLayer.ShowStopOrder = false;

            InMemoryFeatureLayer routingExtentLayer = new InMemoryFeatureLayer();
            routingExtentLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoColor.SimpleColors.Green));
            routingExtentLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            routingExtentLayer.InternalFeatures.Add(new Feature(new RectangleShape(-10888761.5216158, 3551192.52708513, -10871238.72057, 3531592.55348582)));
            Map1.DynamicOverlay.Layers.Add("RoutingExtentLayer", routingExtentLayer);
        }
    }
}
