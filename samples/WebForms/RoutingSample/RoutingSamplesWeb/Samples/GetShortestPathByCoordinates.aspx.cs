/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Routing;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace ThinkGeo.MapSuite.RoutingSamples.Samples
{
    public partial class GetShortestPathByCoordinates : System.Web.UI.Page
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

        protected void btnGetRoute_Click(object sender, EventArgs e)
        {
            RoutingSource routingSource = new RtgRoutingSource(Path.Combine(rootPath, "Austinstreets.rtg"));
            ShapeFileFeatureSource featureSource = new ShapeFileFeatureSource(Path.Combine(rootPath, "Austinstreets.shp"));

            RoutingEngine routingEngine = new RoutingEngine(routingSource, featureSource);
            routingEngine.GeographyUnit = GeographyUnit.Meter;
            routingEngine.SearchRadiusInMeters = 50;

            RoutingLayer routingLayer = (RoutingLayer)Map1.DynamicOverlay.Layers["RoutingLayer"];
            RoutingResult routingResult = routingEngine.GetRoute(routingLayer.StartPoint, routingLayer.EndPoint);
            routingLayer.Routes.Clear();
            routingLayer.Routes.Add(routingResult.Route);

            Map1.DynamicOverlay.Redraw();
        }

        private void RenderMap()
        {
            Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
            Map1.MapUnit = GeographyUnit.Meter;
            Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            Map1.CurrentExtent = new RectangleShape(-10881291.0598132, 3542021.52884823, -10876470.7477693, 3537778.35831031);

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            Map1.BackgroundOverlay = backgroundOverlay;

            Proj4Projection proj4 = new Proj4Projection(4326, 3857);
            proj4.Open();

            RoutingLayer routingLayer = new RoutingLayer();
            string[] startCoordinates = txtStart.Value.Split(',');
            var startPoint = new PointShape(double.Parse(startCoordinates[0], CultureInfo.InvariantCulture), double.Parse(startCoordinates[1], CultureInfo.InvariantCulture));
            routingLayer.StartPoint = (PointShape)proj4.ConvertToExternalProjection(startPoint);
            string[] endCoordinates = txtEnd.Value.Split(',');
            var endPoint = new PointShape(double.Parse(endCoordinates[0], CultureInfo.InvariantCulture), double.Parse(endCoordinates[1], CultureInfo.InvariantCulture));
            routingLayer.EndPoint = (PointShape)proj4.ConvertToExternalProjection(endPoint);
            Map1.DynamicOverlay.Layers.Add("RoutingLayer", routingLayer);

            InMemoryFeatureLayer routingExtentLayer = new InMemoryFeatureLayer();
            routingExtentLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoColor.SimpleColors.Green));
            routingExtentLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            routingExtentLayer.InternalFeatures.Add(new Feature(new RectangleShape(-10888761.5216158, 3551192.52708513, -10871238.72057, 3531592.55348582)));
            Map1.DynamicOverlay.Layers.Add("RoutingExtentLayer", routingExtentLayer);
        }
    }
}
