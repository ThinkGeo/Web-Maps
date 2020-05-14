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
    public partial class GetRoadInformationByRoadId : System.Web.UI.Page
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

        protected void btnGetRouteInformation_Click(object sender, EventArgs e)
        {
            ShapeFileFeatureLayer austinstreetsLayer = new ShapeFileFeatureLayer(Path.Combine(rootPath, "Austinstreets.shp"));
            austinstreetsLayer.Open();

            RtgRoutingSource routingSource = new RtgRoutingSource(Path.Combine(rootPath, "Austinstreets.rtg"));
            routingSource.ReadEndPoints = true;
            routingSource.Open();
            RouteSegment road = routingSource.GetRouteSegmentByFeatureId(txtId.Value);
            // render routeSegment information
            RenderRoadInformation(austinstreetsLayer, road);
            // render adjacent routeSegments information
            RenderAdjacentRoadsInformation(austinstreetsLayer, road);

            austinstreetsLayer.Close();
            routingSource.Close();
            Map1.DynamicOverlay.Redraw();
        }

        private void RenderRoadInformation(ShapeFileFeatureLayer austinstreetsLayer, RouteSegment road)
        {
            InMemoryFeatureLayer currentRoadLayer = Map1.DynamicOverlay.Layers[0] as InMemoryFeatureLayer;
            currentRoadLayer.InternalFeatures.Clear();

            string featureId = road.FeatureId;
            Feature currentRoadFeature = austinstreetsLayer.FeatureSource.GetFeatureById(featureId, ReturningColumnsType.AllColumns);
            currentRoadLayer.InternalFeatures.Add(currentRoadFeature);

            txtStartPoint.Value = String.Format("{0}, {1}", road.StartPoint.X.ToString("F4", CultureInfo.InvariantCulture), road.StartPoint.Y.ToString("F4", CultureInfo.InvariantCulture));
            txtEndPoint.Value = String.Format("{0}, {1}", road.EndPoint.X.ToString("F4", CultureInfo.InvariantCulture), road.EndPoint.Y.ToString("F4", CultureInfo.InvariantCulture));
            LineShape line = ((MultilineShape)currentRoadFeature.GetShape()).Lines[0];
            txtLength.Value = Math.Round(line.GetLength(Map1.MapUnit, DistanceUnit.Meter), 4).ToString(CultureInfo.InvariantCulture);

            switch (road.RouteSegmentType)
            {
                case 0:
                    txtRoadType.Value = "Local Road";
                    break;
                case 1:
                    txtRoadType.Value = "Major Road";
                    break;
                case 2:
                    txtRoadType.Value = "High Way";
                    break;
                default:
                    break;
            }
        }

        private void RenderAdjacentRoadsInformation(ShapeFileFeatureLayer austinstreetsLayer, RouteSegment road)
        {
            InMemoryFeatureLayer adjacentRoadsLayer = Map1.DynamicOverlay.Layers[1] as InMemoryFeatureLayer;
            Collection<string> adjacentIds = road.StartPointAdjacentIds;
            foreach (string id in road.EndPointAdjacentIds)
            {
                adjacentIds.Add(id);
            }
            Collection<Feature> features = austinstreetsLayer.FeatureSource.GetFeaturesByIds(adjacentIds, ReturningColumnsType.AllColumns);
            adjacentRoadsLayer.InternalFeatures.Clear();

            foreach (Feature feature in features)
            {
                adjacentRoadsLayer.InternalFeatures.Add(feature);
            }
        }

        private void RenderMap()
        {
            Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
            Map1.MapUnit = GeographyUnit.Meter;
            Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            Map1.CurrentExtent = new RectangleShape(-10878061.296784, 3537709.26889763, -10877458.7683626, 3537178.96343149);

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            Map1.BackgroundOverlay = backgroundOverlay;

            InMemoryFeatureLayer currentRoadLayer = new InMemoryFeatureLayer();
            currentRoadLayer.Open();
            currentRoadLayer.Columns.Add(new FeatureSourceColumn("FENAME"));
            currentRoadLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = new LineStyle(new GeoPen(GeoColor.SimpleColors.LightRed, 6));
            currentRoadLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = TextStyles.MajorRoad1("FENAME");
            currentRoadLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            currentRoadLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            Map1.DynamicOverlay.Layers.Add("currentRoadLayer", currentRoadLayer);

            InMemoryFeatureLayer adjacentRoadsLayer = new InMemoryFeatureLayer();
            adjacentRoadsLayer.Open();
            adjacentRoadsLayer.Columns.Add(new FeatureSourceColumn("FENAME"));
            adjacentRoadsLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = new LineStyle(new GeoPen(GeoColor.SimpleColors.LightGreen, 6));
            adjacentRoadsLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = TextStyles.MajorRoad1("FENAME");
            adjacentRoadsLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            adjacentRoadsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            Map1.DynamicOverlay.Layers.Add("adjacentRoadsLayer", adjacentRoadsLayer);
        }
    }
}
