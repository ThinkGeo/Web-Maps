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
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Routing;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace ThinkGeo.MapSuite.RoutingSamples
{
    public partial class IntermediatePointsSupport : System.Web.UI.Page
    {
        private static string rootPath;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                rootPath = Path.Combine(MapPath("~"), ConfigurationManager.AppSettings["RootDirectory"]);
                RenderMap();
            }
        }

        protected void chkAddIntermediate_CheckedChanged(object sender, EventArgs e)
        {
            RoutingLayer routingLayer = (RoutingLayer)Map1.DynamicOverlay.Layers["RoutingLayer"];
            Route();
            Map1.DynamicOverlay.Redraw();
        }

        private void Route()
        {
            FeatureSource featureSource = new ShapeFileFeatureSource(Path.Combine(rootPath, "Austinstreets.shp"));
            RoutingSource routingSource = new RtgRoutingSource(Path.Combine(rootPath, "Austinstreets.rtg"));
            RoutingEngine routingEngine = new RoutingEngine(routingSource, featureSource);
            routingEngine.GeographyUnit = GeographyUnit.Meter;
            routingEngine.SearchRadiusInMeters = 100;

            RoutingLayer routingLayer = (RoutingLayer)Map1.DynamicOverlay.Layers["RoutingLayer"];
            Collection<LineShape> paths = new Collection<LineShape>();
            if (chkAddIntermediate.Checked)
            {
                paths.Add(routingEngine.GetRoute(routingLayer.StartPoint, routingLayer.StopPoints[0]).Route);
                paths.Add(routingEngine.GetRoute(routingLayer.StopPoints[0], routingLayer.EndPoint).Route);
            }
            else
            {
                paths.Add(routingEngine.GetRoute(routingLayer.StartPoint, routingLayer.EndPoint).Route);
            }

            routingLayer.Routes.Clear();
            foreach (LineShape item in paths)
            {
                routingLayer.Routes.Add(item);
            }
        }

        private void RenderMap()
        {
            Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
            Map1.MapUnit = GeographyUnit.Meter;
            Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            Map1.CurrentExtent = new RectangleShape(-10883440.1761007, 3546134.00558888, -10876369.7409161, 3540290.49754335);

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            Map1.BackgroundOverlay = backgroundOverlay;

            Proj4Projection proj4 = new Proj4Projection(4326, 3857);
            proj4.Open();

            RoutingLayer routingLayer = new RoutingLayer();
            string[] startCoordinates = txtStartPoint.Value.Split(',');
            PointShape startPoint = new PointShape(double.Parse(startCoordinates[0], CultureInfo.InvariantCulture), double.Parse(startCoordinates[1], CultureInfo.InvariantCulture));
            startPoint = (PointShape)proj4.ConvertToExternalProjection(startPoint);
            routingLayer.StartPoint = startPoint;

            string[] endCoordinates = txtEndPoint.Value.Split(',');
            PointShape endPoint = new PointShape(double.Parse(endCoordinates[0], CultureInfo.InvariantCulture), double.Parse(endCoordinates[1], CultureInfo.InvariantCulture));
            endPoint = (PointShape)proj4.ConvertToExternalProjection(endPoint);
            routingLayer.EndPoint = endPoint;

            string[] stopCoordinates = txtIntermediatePoint.Value.Split(',');
            PointShape stopPoint = new PointShape(double.Parse(stopCoordinates[0], CultureInfo.InvariantCulture), double.Parse(stopCoordinates[1], CultureInfo.InvariantCulture));
            stopPoint = (PointShape)proj4.ConvertToExternalProjection(stopPoint);
            routingLayer.StopPoints.Add(stopPoint);

            Map1.DynamicOverlay.Layers.Add("RoutingLayer", routingLayer);

            InMemoryFeatureLayer routingExtentLayer = new InMemoryFeatureLayer();
            routingExtentLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoColor.SimpleColors.Green));
            routingExtentLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            routingExtentLayer.InternalFeatures.Add(new Feature(new RectangleShape(-10888761.5216158, 3551192.52708513, -10871238.72057, 3531592.55348582)));
            Map1.DynamicOverlay.Layers.Add("RoutingExtentLayer", routingExtentLayer);

            Route();
        }
    }
}
