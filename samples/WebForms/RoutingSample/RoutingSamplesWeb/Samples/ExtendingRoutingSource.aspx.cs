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
    public partial class ExtendingRoutingSource : System.Web.UI.Page
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

        private void RenderMap()
        {
            Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
            Map1.MapUnit = GeographyUnit.Meter;
            Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
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

        protected void btnRoute_Click(object sender, EventArgs e)
        {
            FeatureSource featureSource = new ShapeFileFeatureSource(Path.Combine(rootPath, "Austinstreets.shp"));
            RoutingSource routingSource = new CustomRoutingSource(featureSource);
            RoutingEngine routingEngine = new RoutingEngine(routingSource, featureSource);
            routingEngine.GeographyUnit = GeographyUnit.Meter;
            RoutingResult routingResult = routingEngine.GetRoute(txtStartId.Value, txtEndId.Value);

            RoutingLayer routingLayer = (RoutingLayer)Map1.DynamicOverlay.Layers["RoutingLayer"];
            routingLayer.Routes.Clear();
            routingLayer.Routes.Add(routingResult.Route);

            Map1.DynamicOverlay.Redraw();
        }
    }

    /// <summary>
    /// It shows how to exploler custom RoutingSource
    /// </summary>
    public class CustomRoutingSource : RoutingSource
    {
        private FeatureSource featureSource;

        public CustomRoutingSource()
            : this(null)
        {
        }

        public CustomRoutingSource(FeatureSource featureSource)
            : base()
        {
            this.featureSource = featureSource;
        }

        protected override int GetRouteSegmentCountCore()
        {
            return featureSource.GetCount();
        }

        protected override void OpenCore()
        {
            base.OpenCore();
            featureSource.Open();
        }

        protected override void CloseCore()
        {
            base.CloseCore();
            featureSource.Close();
        }

        protected override RouteSegment GetRouteSegmentByFeatureIdCore(string roadId)
        {
            Feature feature = featureSource.GetFeatureById(roadId, ReturningColumnsType.NoColumns);

            LineBaseShape shape = feature.GetShape() as LineBaseShape;
            LineShape sourceShape = ConvertLineBaseShapeToLines(shape)[0];
            sourceShape.Id = feature.Id;
            Collection<string> startIds = new Collection<string>();
            Collection<string> endIds = new Collection<string>();
            GetAdjacentFeataureIds(featureSource, sourceShape, startIds, endIds);
            PointShape startPoint = new PointShape(sourceShape.Vertices[0]);
            PointShape endPoint = new PointShape(sourceShape.Vertices[sourceShape.Vertices.Count - 1]);
            float length = (float)sourceShape.GetLength(GeographyUnit.Meter, DistanceUnit.Meter);
            RouteSegment road = new RouteSegment(feature.Id, 0, length, startPoint, startIds, endPoint, endIds);

            return road;
        }

        private static void GetAdjacentFeataureIds(FeatureSource featureSource, LineShape sourceShape, Collection<string> startIds, Collection<string> endIds)
        {
            Vertex startVertex = sourceShape.Vertices[0];
            Vertex endVertex = sourceShape.Vertices[sourceShape.Vertices.Count - 1];
            Collection<Feature> tempfeatures = featureSource.GetFeaturesInsideBoundingBox(sourceShape.GetBoundingBox(), ReturningColumnsType.NoColumns);
            foreach (Feature tempFeature in tempfeatures)
            {
                LineShape tempShape = ((MultilineShape)tempFeature.GetShape()).Lines[0];
                if (sourceShape.Id == tempFeature.Id)
                {
                    continue;
                }

                if (tempShape.Vertices.Contains(startVertex))
                {
                    startIds.Add(tempFeature.Id);
                }
                else if (tempShape.Vertices.Contains(endVertex))
                {
                    endIds.Add(tempFeature.Id);
                }
            }
        }

        private static Collection<LineShape> ConvertLineBaseShapeToLines(LineBaseShape sourceShape)
        {
            Collection<LineShape> lines = null;
            LineShape lineShape = sourceShape as LineShape;
            if (lineShape != null)
            {
                lines = new Collection<LineShape>();
                lines.Add(lineShape);
            }
            else
            {
                lines = ((MultilineShape)sourceShape).Lines;
            }
            return lines;
        }
    }
}
