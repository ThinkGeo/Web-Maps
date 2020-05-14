/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Web.UI.WebControls;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Routing;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace ThinkGeo.MapSuite.RoutingSamples
{
    public partial class RoutingDirections : System.Web.UI.Page
    {
        private static DataTable dataTable;
        private static string rootPath;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                rootPath = Path.Combine(MapPath("~"), ConfigurationManager.AppSettings["RootDirectory"]);
                RenderMap();

                ShowTurnByTurnDirections(new Collection<RouteSegment>(), new Collection<Feature>());
            }
        }

        protected void btnRoute_Click(object sender, EventArgs e)
        {
            FeatureSource featureSource = new ShapeFileFeatureSource(Path.Combine(rootPath, "Austinstreets.shp"));
            RoutingSource routingSource = new RtgRoutingSource(Path.Combine(rootPath, "Austinstreets.rtg"));
            RoutingEngine routingEngine = new RoutingEngine(routingSource, featureSource);
            routingEngine.GeographyUnit = GeographyUnit.Meter;
            RoutingResult routingResult = routingEngine.GetRoute(txtStartId.Value, txtEndId.Value);

            RoutingLayer routingLayer = (RoutingLayer)Map1.DynamicOverlay.Layers["RoutingLayer"];
            routingLayer.Routes.Clear();
            routingLayer.Routes.Add(routingResult.Route);

            ShowTurnByTurnDirections(routingResult.RouteSegments, routingResult.Features);

            Map1.DynamicOverlay.Redraw();
        }

        private void ShowTurnByTurnDirections(Collection<RouteSegment> roads, Collection<Feature> features)
        {
            dataTable = new DataTable();
            dataTable.Locale = CultureInfo.InvariantCulture;

            dataTable.Columns.Add("RoadName");
            dataTable.Columns.Add("Direction");
            dataTable.Columns.Add("Length(Meter)");

            for (int i = 0; i < roads.Count; i++)
            {
                DataRow dataRow = dataTable.NewRow();
                dataRow["RoadName"] = features[i].ColumnValues["FENAME"];
                dataRow["Direction"] = roads[i].DrivingDirection;
                dataRow["Length(Meter)"] = Math.Round(((LineBaseShape)features[i].GetShape()).GetLength(GeographyUnit.Meter, DistanceUnit.Meter), 2);
                dataTable.Rows.Add(dataRow);
            }
            gvDirections.DataSource = dataTable;
            gvDirections.DataBind();
        }

        protected void gvDirections_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDirections.PageIndex = e.NewPageIndex;

            gvDirections.DataSource = dataTable;
            gvDirections.DataBind();
        }

        private void RenderMap()
        {
            Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
            Map1.MapUnit = GeographyUnit.Meter;
            Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();

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
            Map1.CurrentExtent = new RectangleShape(-10880028.5234687, 3543521.87388826, -10878260.9254813, 3542061.05121684);
        }
    }
}
