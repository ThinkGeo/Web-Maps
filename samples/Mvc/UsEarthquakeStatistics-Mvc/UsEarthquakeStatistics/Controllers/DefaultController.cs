/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace ThinkGeo.MapSuite.EarthquakeStatistics.Controllers
{
    public class DefaultController : Controller
    {
        public ActionResult Index()
        {
            // Initialize map for the page
            Map map = InitializeMap();
            return View(map);
        }

        [MapActionFilter]
        public void ClearAllShapes(Map map, GeoCollection<object> args)
        {
            // clear the tracked shapes.
            LayerOverlay trackShapeOverlay = map.CustomOverlays["TrackShapeOverlay"] as LayerOverlay;
            InMemoryFeatureLayer trackShapeLayer = trackShapeOverlay.Layers["TrackShapeLayer"] as InMemoryFeatureLayer;
            trackShapeLayer.InternalFeatures.Clear();
            trackShapeOverlay.Redraw();

            LayerOverlay queryResultMarkerOverlay = map.CustomOverlays["QueryResultMarkerOverlay"] as LayerOverlay;
            // clear the queried result markers from map.
            InMemoryFeatureLayer markerMemoryLayer = queryResultMarkerOverlay.Layers["MarkerMemoryLayer"] as InMemoryFeatureLayer;
            markerMemoryLayer.InternalFeatures.Clear();
            // clear the highlighted result markers.
            InMemoryFeatureLayer markerMemoryHighLightLayer = queryResultMarkerOverlay.Layers["MarkerMemoryHighLightLayer"] as InMemoryFeatureLayer;
            markerMemoryHighLightLayer.InternalFeatures.Clear();
            queryResultMarkerOverlay.Redraw();
        }

        [MapActionFilter]
        public JsonResult GetQueryingFeatures(Map map, GeoCollection<object> args)
        {
            string featuresInJson = string.Empty;
            if (map != null)
            {
                Collection<EarthquakeQueryConfiguration> configurations = JsonSerializer.Deserialize<Collection<EarthquakeQueryConfiguration>>(args[0].ToString());
                Collection<Feature> selectedFeatures = FilterEarthquakePoints(map, configurations);
                featuresInJson = InternalHelper.ConvertFeaturesToJson(selectedFeatures);
                Session["QueryConfiguration"] = configurations;
            }

            return Json(new { features = featuresInJson });
        }

        [MapActionFilter]
        public JsonResult GetTrackFeatures(Map map, GeoCollection<object> args)
        {
            string featuresInJson = string.Empty;
            if (map != null)
            {
                // convert wkt to thinkgeo features
                Feature[] trackedFeatrues = args.Select((t) =>
                {
                    return new Feature(t.ToString());
                }).OfType<Feature>().ToArray();

                // add those features into edit overlay
                map.EditOverlay.Features.Clear();
                foreach (Feature item in trackedFeatrues)
                {
                    map.EditOverlay.Features.Add(item);
                }

                // restore other query conditions from session.
                Collection<EarthquakeQueryConfiguration> queryConfigurations = new Collection<EarthquakeQueryConfiguration>();
                if (Session["QueryConfiguration"] != null)
                {
                    queryConfigurations = Session["QueryConfiguration"] as Collection<EarthquakeQueryConfiguration>;
                }

                Collection<Feature> selectedFeatures = FilterEarthquakePoints(map, queryConfigurations);
                featuresInJson = InternalHelper.ConvertFeaturesToJson(selectedFeatures);
            }
            return Json(new { features = featuresInJson });
        }

        [MapActionFilter]
        public JsonResult ZoomToFeature(Map map, GeoCollection<object> args)
        {
            string featureId = args[0].ToString();

            LayerOverlay queryResultMarkerOverlay = map.CustomOverlays["QueryResultMarkerOverlay"] as LayerOverlay;
            InMemoryFeatureLayer markerMemoryLayer = queryResultMarkerOverlay.Layers["MarkerMemoryLayer"] as InMemoryFeatureLayer;
            InMemoryFeatureLayer markerMemoryHighLightLayer = queryResultMarkerOverlay.Layers["MarkerMemoryHighLightLayer"] as InMemoryFeatureLayer;

            Feature currentFeature = markerMemoryLayer.InternalFeatures.FirstOrDefault(f => f.Id == featureId);
            markerMemoryHighLightLayer.InternalFeatures.Clear();
            markerMemoryHighLightLayer.InternalFeatures.Add(currentFeature);
            PointShape center = currentFeature.GetShape() as PointShape;

            return Json(new { x = center.X, y = center.Y });
        }

        [MapActionFilter]
        public void SwitchMapType(Map map, GeoCollection<object> args)
        {
            if (map != null)
            {
                string mapType = args[0].ToString();

                LayerOverlay earthquakeOverlay = map.CustomOverlays["EarthquakeOverlay"] as LayerOverlay;
                foreach (Layer layer in earthquakeOverlay.Layers)
                {
                    layer.IsVisible = false;
                }
                Layer selectedLayer = earthquakeOverlay.Layers[mapType];
                selectedLayer.IsVisible = true;
                earthquakeOverlay.Redraw();

                // if Isoline layer, then display its legend.
                if (mapType.Equals("IsoLines Map"))
                {
                    map.AdornmentOverlay.Layers["IsoLineLevelLegendLayer"].IsVisible = true;
                }
                else
                {
                    map.AdornmentOverlay.Layers["IsoLineLevelLegendLayer"].IsVisible = false;
                }
            }
        }

        private Map InitializeMap()
        {
            Map Map1 = new Map("Map1");
            Map1.Width = new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage);
            Map1.Height = new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage);
            Map1.MapUnit = GeographyUnit.Meter;
            Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            Map1.MapTools.OverlaySwitcher.Enabled = true;
            Map1.MapTools.OverlaySwitcher.BaseOverlayTitle = "ThinkGeo Cloud Maps:";

            // add base layers.
            AddBaseMapLayers(Map1);
            // add the earthquake layer to as the data source.
            AddEarthquakeLayers(Map1);
            // add query shape layers, like track layer, marker layer and highlight layer etc.
            AddQueryResultLayers(Map1);
            // add adorment layers
            AddAdormentLayers(Map1);

            Map1.CurrentExtent = new RectangleShape(-14503631.6805645, 7498410.41581975, -7928840.70035357, 4171879.26511785);
            Map1.OnClientTrackShapeFinished = "trackShapeFinished";

            return Map1;
        }

        private void AddBaseMapLayers(Map Map1)
        {
            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay lightMapOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            lightMapOverlay.Name = "Light";
            lightMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Light;
            Map1.CustomOverlays.Add(lightMapOverlay);

            ThinkGeoCloudRasterMapsOverlay darkMapOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            darkMapOverlay.Name = "Dark";
            darkMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Dark;
            Map1.CustomOverlays.Add(darkMapOverlay);

            ThinkGeoCloudRasterMapsOverlay aerialMapOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            aerialMapOverlay.Name = "Aerial";
            aerialMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Aerial;
            Map1.CustomOverlays.Add(aerialMapOverlay);

            ThinkGeoCloudRasterMapsOverlay hybridMapOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            hybridMapOverlay.Name = "Hybrid";
            hybridMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Hybrid;
            Map1.CustomOverlays.Add(hybridMapOverlay);
        }

        private void AddEarthquakeLayers(Map Map1)
        {
            LayerOverlay earthquakeOverlay = new LayerOverlay("EarthquakeOverlay");
            //earthquakeOverlay.TileType = TileType.SingleTile;
            earthquakeOverlay.IsVisibleInOverlaySwitcher = false;
            Map1.CustomOverlays.Add(earthquakeOverlay);

            Proj4Projection proj4 = new Proj4Projection();
            proj4.InternalProjectionParametersString = Proj4Projection.GetDecimalDegreesParametersString();
            proj4.ExternalProjectionParametersString = Proj4Projection.GetSphericalMercatorParametersString();

            string dataShapefileFilePath = Server.MapPath(ConfigurationManager.AppSettings["statesPathFileName"]);

            EarthquakeHeatFeatureLayer heatLayer = new EarthquakeHeatFeatureLayer(new ShapeFileFeatureSource(dataShapefileFilePath));
            heatLayer.HeatStyle = new HeatStyle(10, 180, "MAGNITUDE", 0, 12, 100, DistanceUnit.Kilometer);
            heatLayer.FeatureSource.Projection = proj4;
            earthquakeOverlay.Layers.Add("Heat Map", heatLayer);

            ShapeFileFeatureLayer pointLayer = new ShapeFileFeatureLayer(dataShapefileFilePath);
            pointLayer.FeatureSource.Projection = proj4;
            pointLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.StandardColors.Red, 6, GeoColor.StandardColors.White, 1);
            pointLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            pointLayer.IsVisible = false;
            earthquakeOverlay.Layers.Add("Regular Point Map", pointLayer);

            EarthquakeIsoLineFeatureLayer isoLineLayer = new EarthquakeIsoLineFeatureLayer(new ShapeFileFeatureSource(dataShapefileFilePath));
            isoLineLayer.FeatureSource.Projection = proj4;
            isoLineLayer.IsVisible = false;
            earthquakeOverlay.Layers.Add("IsoLines Map", isoLineLayer);
        }

        private void AddQueryResultLayers(Map Map1)
        {
            // define the track layer.
            LayerOverlay trackResultOverlay = new LayerOverlay("TrackShapeOverlay");
            trackResultOverlay.IsVisibleInOverlaySwitcher = false;
            trackResultOverlay.TileType = TileType.SingleTile;
            Map1.CustomOverlays.Add(trackResultOverlay);

            InMemoryFeatureLayer trackResultLayer = new InMemoryFeatureLayer();
            trackResultLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(80, GeoColor.SimpleColors.LightGreen), GeoColor.SimpleColors.White, 2);
            trackResultLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.SimpleColors.Orange, 2, true);
            trackResultLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.SimpleColors.Orange, 10);
            trackResultLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            trackResultOverlay.Layers.Add("TrackShapeLayer", trackResultLayer);

            // define the marker and highlight layer for markers.
            LayerOverlay queryResultMarkerOverlay = new LayerOverlay("QueryResultMarkerOverlay");
            queryResultMarkerOverlay.IsBaseOverlay = false;
            queryResultMarkerOverlay.IsVisibleInOverlaySwitcher = false;
            queryResultMarkerOverlay.TileType = TileType.SingleTile;
            Map1.CustomOverlays.Add(queryResultMarkerOverlay);

            InMemoryFeatureLayer markerMemoryLayer = new InMemoryFeatureLayer();
            markerMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.SimpleColors.Gold, 8, GeoColor.SimpleColors.Orange, 1);
            markerMemoryLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            queryResultMarkerOverlay.Layers.Add("MarkerMemoryLayer", markerMemoryLayer);

            InMemoryFeatureLayer markerMemoryHighLightLayer = new InMemoryFeatureLayer();
            PointStyle highLightStyle = new PointStyle();
            highLightStyle.CustomPointStyles.Add(PointStyles.CreateSimpleCircleStyle(GeoColor.FromArgb(50, GeoColor.SimpleColors.Blue), 20, GeoColor.SimpleColors.LightBlue, 1));
            highLightStyle.CustomPointStyles.Add(PointStyles.CreateSimpleCircleStyle(GeoColor.SimpleColors.LightBlue, 9, GeoColor.SimpleColors.Blue, 1));
            markerMemoryHighLightLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = highLightStyle;
            markerMemoryHighLightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            queryResultMarkerOverlay.Layers.Add("MarkerMemoryHighLightLayer", markerMemoryHighLightLayer);
        }

        private void AddAdormentLayers(Map Map1)
        {
            // ScaleBar
            ScaleBarAdornmentLayer scaleBarAdormentLayer = new ScaleBarAdornmentLayer();
            scaleBarAdormentLayer.XOffsetInPixel = 10;
            scaleBarAdormentLayer.XOffsetInPixel = 5;
            scaleBarAdormentLayer.UnitFamily = UnitSystem.Metric;
            Map1.AdornmentOverlay.Layers.Add("ScaleBarAdormentLayer", scaleBarAdormentLayer);

            // Isoline legend adorment layer
            LegendAdornmentLayer isoLevelLegendLayer = new LegendAdornmentLayer();
            isoLevelLegendLayer.IsVisible = false;
            isoLevelLegendLayer.Width = 85;
            isoLevelLegendLayer.Height = 320;
            isoLevelLegendLayer.Location = AdornmentLocation.LowerRight;
            isoLevelLegendLayer.ContentResizeMode = LegendContentResizeMode.Fixed;

            LegendItem legendTitle = new LegendItem();
            legendTitle.TextStyle = new TextStyle("Magnitude", new GeoFont("Arial", 10), new GeoSolidBrush(GeoColor.StandardColors.Black));
            legendTitle.TextLeftPadding = -20;
            isoLevelLegendLayer.LegendItems.Add(legendTitle);   // add legend title

            // Legend items
            LayerOverlay earthquakeOverlay = Map1.CustomOverlays["EarthquakeOverlay"] as LayerOverlay;
            EarthquakeIsoLineFeatureLayer isolineLayer = earthquakeOverlay.Layers["IsoLines Map"] as EarthquakeIsoLineFeatureLayer;
            for (int i = 0; i < isolineLayer.IsoLineLevels.Count; i++)
            {
                LegendItem legendItem = new LegendItem();
                legendItem.TextStyle = new TextStyle(isolineLayer.IsoLineLevels[i].ToString("f2"), new GeoFont("Arial", 10), new GeoSolidBrush(GeoColor.StandardColors.Black));
                legendItem.ImageStyle = isolineLayer.LevelClassBreakStyle.ClassBreaks[i].DefaultAreaStyle;
                legendItem.ImageWidth = 25;

                isoLevelLegendLayer.LegendItems.Add(legendItem);
            }

            Map1.AdornmentOverlay.Layers.Add("IsoLineLevelLegendLayer", isoLevelLegendLayer);
        }

        private Collection<Feature> FilterEarthquakePoints(Map Map1, Collection<EarthquakeQueryConfiguration> queryConfigurations)
        {
            IEnumerable<Feature> allFeatures = new Collection<Feature>();

            if (Map1.EditOverlay.Features.Count > 0)
            {
                Feature queryFeature = Feature.Union(Map1.EditOverlay.Features);
                BaseShape queryShape = queryFeature.GetShape();

                FeatureLayer currentEarthquakeLayer = (Map1.CustomOverlays["EarthquakeOverlay"] as LayerOverlay).Layers[0] as FeatureLayer;
                currentEarthquakeLayer.Open();
                allFeatures = currentEarthquakeLayer.FeatureSource.GetFeaturesWithinDistanceOf(queryShape, Map1.MapUnit, DistanceUnit.Meter, 1, ReturningColumnsType.AllColumns);
                currentEarthquakeLayer.Close();
            }

            // filter the feature based on the query configuration.
            allFeatures = allFeatures.Where((f) =>
            {
                bool isIncluded = true;
                foreach (EarthquakeQueryConfiguration item in queryConfigurations)
                {
                    double columnValue = double.Parse(f.ColumnValues[item.Parameter]);
                    if ((columnValue > item.Maximum || columnValue < item.Minimum) && columnValue > 0) // nagetive means no record.
                    {
                        isIncluded = false;
                        break;
                    }
                }
                return isIncluded;
            }).ToList();

            // clear the original markers and add new markers.
            LayerOverlay queryResultMarkerOverlay = Map1.CustomOverlays["QueryResultMarkerOverlay"] as LayerOverlay;
            InMemoryFeatureLayer markerMemoryLayer = queryResultMarkerOverlay.Layers["MarkerMemoryLayer"] as InMemoryFeatureLayer;
            markerMemoryLayer.InternalFeatures.Clear();
            foreach (Feature item in allFeatures)
            {
                markerMemoryLayer.InternalFeatures.Add(item);
            }
            queryResultMarkerOverlay.Redraw();

            LayerOverlay trackShapeOverlay = Map1.CustomOverlays["TrackShapeOverlay"] as LayerOverlay;
            // clear the original track shapes and add the new shapes.
            InMemoryFeatureLayer trackShapeLayer = trackShapeOverlay.Layers["TrackShapeLayer"] as InMemoryFeatureLayer;
            trackShapeLayer.InternalFeatures.Clear();
            foreach (Feature item in Map1.EditOverlay.Features)
            {
                trackShapeLayer.InternalFeatures.Add(item);
            }
            trackShapeOverlay.Redraw();

            return new Collection<Feature>(allFeatures.ToList());
        }
    }
}
