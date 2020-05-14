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
    public partial class DifferentProjectionsWithRouting : System.Web.UI.Page
    {
        private static RoutingEngine routingEngine;
        private static RoutingSource RoutingSourceForShortest;
        private static RoutingSource RoutingSourceForFastest;
        private static string rootPath;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                rootPath = Path.Combine(MapPath("~"), ConfigurationManager.AppSettings["RootDirectory"]);
                ShapeFileFeatureSource featureSource = new ShapeFileFeatureSource(Path.Combine(rootPath, "Austinstreets.shp"));

                RoutingSourceForShortest = new RtgRoutingSource(Path.Combine(rootPath, "Austinstreets.rtg"));
                RoutingSourceForFastest = new RtgRoutingSource(Path.Combine(rootPath, "AustinStreetsForFastest.rtg"));
                routingEngine = new RoutingEngine(RoutingSourceForShortest, featureSource);
                routingEngine.GeographyUnit = GeographyUnit.Meter;

                RenderMap();

                FindPath();
            }
        }

        private void RenderMap()
        {
            Map1.MapUnit = GeographyUnit.Meter;
            Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
            Map1.CurrentExtent = new RectangleShape(-10884474.4001792, 3542175.58734002, -10877416.7249544, 3537139.84069311);

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            Map1.BackgroundOverlay = backgroundOverlay;

            ShapeFileFeatureSource featureSource = new ShapeFileFeatureSource(Path.Combine(rootPath, "Austinstreets.shp"));
            featureSource.Open();
            RoutingLayer routingLayer = new RoutingLayer();
            routingLayer.StartPoint = featureSource.GetFeatureById(txtStartFeatureId.Value, ReturningColumnsType.NoColumns).GetShape().GetCenterPoint();
            routingLayer.EndPoint = featureSource.GetFeatureById(txtEndFeatureId.Value, ReturningColumnsType.NoColumns).GetShape().GetCenterPoint();
            Map1.DynamicOverlay.Layers.Add("RoutingLayer", routingLayer);

            InMemoryFeatureLayer routingExtentLayer = new InMemoryFeatureLayer();
            routingExtentLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoColor.SimpleColors.Green));
            routingExtentLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            routingExtentLayer.InternalFeatures.Add(new Feature(new RectangleShape(-10888761.5216158, 3547722.87454731, -10871238.72057, 3531592.55348582)));
            Map1.DynamicOverlay.Layers.Add("RoutingExtentLayer", routingExtentLayer);
        }

        private void FindPath()
        {
            RoutingLayer routingLayer = (RoutingLayer)Map1.DynamicOverlay.Layers["RoutingLayer"];

            routingLayer.Routes.Clear();
            routingLayer.Routes.Add(routingEngine.GetRoute(txtStartFeatureId.Value, txtEndFeatureId.Value).Route);
            Map1.DynamicOverlay.Redraw();
        }

        protected void rbtShortest_CheckedChanged(object sender, EventArgs e)
        {
            routingEngine.RoutingSource = RoutingSourceForShortest;
            FindPath();
        }

        protected void rbtFastest_CheckedChanged(object sender, EventArgs e)
        {
            routingEngine.RoutingSource = RoutingSourceForFastest;
            FindPath();
        }
    }
}
