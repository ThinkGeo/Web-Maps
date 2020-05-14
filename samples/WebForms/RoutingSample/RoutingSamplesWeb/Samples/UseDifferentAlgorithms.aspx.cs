/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
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
    public partial class UseDifferentAlgorithms : System.Web.UI.Page
    {
        private static string rootPath;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                rootPath = Path.Combine(MapPath("~"), ConfigurationManager.AppSettings["RootDirectory"]);
                RenderMap();
                GetRoute();
                //this.ddlAlgorithm.Text = "Dijkstra";
            }
        }

        protected void ddlAlgorithm_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetRoute();
        }

        private void GetRoute()
        {
            FeatureSource featureSource = new ShapeFileFeatureSource(Path.Combine(rootPath, "Austinstreets.shp"));
            RoutingSource routingSource = new RtgRoutingSource(Path.Combine(rootPath, "Austinstreets.rtg"));
            RoutingEngine routingEngine = new RoutingEngine(routingSource, GetAlgorithm(), featureSource);
            routingEngine.GeographyUnit = GeographyUnit.Meter;
            RoutingResult routingResult = routingEngine.GetRoute(txtStartId.Value, txtEndId.Value);

            RoutingLayer routingLayer = (RoutingLayer)Map1.DynamicOverlay.Layers["RoutingLayer"];
            routingLayer.Routes.Clear();
            routingLayer.Routes.Add(routingResult.Route);

            Map1.DynamicOverlay.Redraw();
        }

        private RoutingAlgorithm GetAlgorithm()
        {
            RoutingAlgorithm algorithm = null;
            switch (ddlAlgorithm.SelectedValue)
            {
                case "AStar":
                    algorithm = new AStarRoutingAlgorithm();
                    break;
                case "Dijkstra":
                    algorithm = new DijkstraRoutingAlgorithm();
                    break;
                case "Bidirectional":
                    algorithm = new BidirectionalRoutingAlgorithm();
                    break;
                default:
                    break;
            }

            return algorithm;
        }

        private void RenderMap()
        {
            Map1.MapUnit = GeographyUnit.Meter;
            Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
            Map1.CurrentExtent = new RectangleShape(-10882970.1251073, 3542132.85056385, -10877292.6084378, 3536925.3352412);

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            Map1.BackgroundOverlay = backgroundOverlay;

            ShapeFileFeatureLayer austinstreetsLayer = new ShapeFileFeatureLayer(Path.Combine(rootPath, "Austinstreets.shp"));
            austinstreetsLayer.Open();
            RoutingLayer routingLayer = new RoutingLayer();
            routingLayer.StartPoint = austinstreetsLayer.FeatureSource.GetFeatureById(txtStartId.Value, ReturningColumnsType.NoColumns).GetShape().GetCenterPoint();
            routingLayer.EndPoint = austinstreetsLayer.FeatureSource.GetFeatureById(txtEndId.Value, ReturningColumnsType.NoColumns).GetShape().GetCenterPoint();
            austinstreetsLayer.Close();
            Map1.DynamicOverlay.Layers.Add("RoutingLayer", routingLayer);

            InMemoryFeatureLayer routingExtentLayer = new InMemoryFeatureLayer();
            routingExtentLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoColor.SimpleColors.Green));
            routingExtentLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            routingExtentLayer.InternalFeatures.Add(new Feature(new RectangleShape(-10888761.5216158, 3551192.52708513, -10871238.72057, 3531592.55348582)));
            Map1.DynamicOverlay.Layers.Add("RoutingExtentLayer", routingExtentLayer);
        }
    }
}
