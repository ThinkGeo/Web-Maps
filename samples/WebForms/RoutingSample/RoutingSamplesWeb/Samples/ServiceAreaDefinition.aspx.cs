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
    public partial class ServiceAreaDefinition : System.Web.UI.Page
    {
        private static string rootPath;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                rootPath = Path.Combine(MapPath("~"), ConfigurationManager.AppSettings["RootDirectory"]);
                RenderMap();
                Route();
            }
        }

        protected void btnRoute_Click(object sender, EventArgs e)
        {
            Route();
        }

        private void Route()
        {
            RtgRoutingSource routingSource = new RtgRoutingSource(Path.Combine(rootPath, "Austinstreets.rtg"));
            FeatureSource featureSource = new ShapeFileFeatureSource(Path.Combine(rootPath, "Austinstreets.shp"));
            RoutingEngine routingEngine = new RoutingEngine(routingSource, featureSource);
            routingEngine.GeographyUnit = GeographyUnit.Meter;

            float averageSpeed = float.Parse(txtSpeed.Value);
            int drivingMinutes = int.Parse(txtDrivingTime.Value);
            SpeedUnit speedUnit = GetSpeedUnit();
            PolygonShape polygonShape = routingEngine.GenerateServiceArea(txtSourceFeatureId.Value, new TimeSpan(0, drivingMinutes, 0), averageSpeed, speedUnit);

            InMemoryFeatureLayer routingLayer = (InMemoryFeatureLayer)Map1.DynamicOverlay.Layers["RoutingLayer"];
            routingLayer.InternalFeatures.Remove("ServiceArea");
            if (polygonShape.Validate(ShapeValidationMode.Simple).IsValid)
            {
                routingLayer.InternalFeatures.Add("ServiceArea", new Feature(polygonShape));
                routingLayer.Open();
                Map1.CurrentExtent = routingLayer.GetBoundingBox();
                routingLayer.Close();
            }

            Map1.DynamicOverlay.Redraw();
        }

        private SpeedUnit GetSpeedUnit()
        {
            SpeedUnit speedUnit = SpeedUnit.Kph;
            switch (ddlSpeedUnit.SelectedValue)
            {
                case "Mile":
                    speedUnit = SpeedUnit.Mph;
                    break;
                default:
                    break;
            }
            return speedUnit;
        }

        private void RenderMap()
        {
            Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
            Map1.MapUnit = GeographyUnit.Meter;
            Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            Map1.CurrentExtent = new RectangleShape(-10882481.0639857, 3541063.02410643, -10875410.6288813, 3535221.85707249);

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            Map1.BackgroundOverlay = backgroundOverlay;

            ShapeFileFeatureLayer austinstreetsLayer = new ShapeFileFeatureLayer(Path.Combine(rootPath, "Austinstreets.shp"));
            austinstreetsLayer.Open();
            InMemoryFeatureLayer routingLayer = new InMemoryFeatureLayer();
            routingLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimpleStarStyle(GeoColor.SimpleColors.Red, 30);
            routingLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush = new GeoSolidBrush(GeoColor.FromArgb(100, GeoColor.StandardColors.Blue));
            routingLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.OuterPen.Color = GeoColor.StandardColors.Red;
            routingLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            Map1.DynamicOverlay.Layers.Add("RoutingLayer", routingLayer);

            InMemoryFeatureLayer routingExtentLayer = new InMemoryFeatureLayer();
            routingExtentLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoColor.SimpleColors.Green));
            routingExtentLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            routingExtentLayer.InternalFeatures.Add(new Feature(new RectangleShape(-10888761.5216158, 3551192.52708513, -10871238.72057, 3531592.55348582)));
            Map1.DynamicOverlay.Layers.Add("RoutingExtentLayer", routingExtentLayer);

            austinstreetsLayer.Open();
            Feature startRoad = austinstreetsLayer.FeatureSource.GetFeatureById(txtSourceFeatureId.Value, ReturningColumnsType.NoColumns);
            Feature startPoint = new Feature(startRoad.GetShape().GetCenterPoint().GetWellKnownBinary(), txtSourceFeatureId.Value);
            routingLayer.InternalFeatures.Add(txtSourceFeatureId.Value, startPoint);
            austinstreetsLayer.Close();
        }
    }
}
