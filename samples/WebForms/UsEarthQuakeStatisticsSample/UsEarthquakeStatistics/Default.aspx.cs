/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace ThinkGeo.MapSuite.EarthquakeStatistics
{
    public partial class _Default : Page, ICallbackEventHandler
    {
        private string callbackResult;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializeMap();

                // display an empty query result table.
                DisplayQueryResult(new Collection<Feature>());
            }
        }

        protected void ChangeTrackShapeMode_Command(object sender, CommandEventArgs e)
        {
            switch (e.CommandArgument.ToString())
            {
                case "DrawCircle":
                    Map1.EditOverlay.TrackMode = TrackMode.Circle;
                    break;

                case "DrawRectangle":
                    Map1.EditOverlay.TrackMode = TrackMode.Rectangle;
                    break;

                case "DrawPolygon":
                    Map1.EditOverlay.TrackMode = TrackMode.Polygon;
                    break;

                case "ClearAll":
                    ClearQueryResult();
                    break;

                case "Pan":
                default:
                    Map1.EditOverlay.TrackMode = TrackMode.None;
                    break;
            }
        }

        protected void rptQueryResultItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var featureID = ((DataRowView)e.Item.DataItem)[0].ToString();

                ImageButton update = (ImageButton)e.Item.FindControl("find");
                update.CommandArgument = featureID;
            }
        }

        protected void ibtnZoomToFeature(object source, RepeaterCommandEventArgs e)
        {
            string featureId = ((ImageButton)(((RepeaterCommandEventArgs)(e)).CommandSource)).CommandArgument;
            ZoomToFeature(featureId);
        }

        protected void Map1_TrackShapeFinished(object sender, EventArgs e)
        {
            Collection<EarthquakeQueryConfiguration> queryConfigurations = new Collection<EarthquakeQueryConfiguration>();
            if (Session["QueryConfiguration"] != null)
            {
                queryConfigurations = Session["QueryConfiguration"] as Collection<EarthquakeQueryConfiguration>;
            }

            Collection<Feature> selectedFeatures = FilterEarthquakePoints(queryConfigurations);
            DisplayQueryResult(selectedFeatures);
        }

        public string GetCallbackResult()
        {
            return callbackResult;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            CallbackRequest callbackRequest = JsonSerializer.Deserialize<CallbackRequest>(eventArgument);
            switch (callbackRequest.Command)
            {
                case "Query":
                    IEnumerable<Feature> selectedFeatures = FilterEarthquakePoints(callbackRequest.QueryConfigurations);
                    Session["QueryConfiguration"] = callbackRequest.QueryConfigurations;
                    callbackResult = InternalHelper.ConvertFeaturesToJson(selectedFeatures);
                    break;

                case "ZoomToFeature":
                    callbackResult = ZoomToFeature(callbackRequest.FeatureId);
                    break;

                case "ChangeType":
                    ChangeMapDisplayType(callbackRequest.MapDisplayType);
                    break;

                default:
                    break;
            }
        }

        private void ChangeMapDisplayType(string mapType)
        {
            LayerOverlay earthquakeOverlay = Map1.CustomOverlays["EarthquakeOverlay"] as LayerOverlay;
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
                Map1.AdornmentOverlay.Layers["IsoLineLevelLegendLayer"].IsVisible = true;
            }
            else
            {
                Map1.AdornmentOverlay.Layers["IsoLineLevelLegendLayer"].IsVisible = false;
            }
        }

        private void ClearQueryResult()
        {
            // clear the query result table.
            DisplayQueryResult(new Collection<Feature>());

            // clear the tracked shapes.
            LayerOverlay trackShapeOverlay = Map1.CustomOverlays["TrackShapeOverlay"] as LayerOverlay;
            InMemoryFeatureLayer trackShapeLayer = trackShapeOverlay.Layers["TrackShapeLayer"] as InMemoryFeatureLayer;
            trackShapeLayer.InternalFeatures.Clear();
            trackShapeOverlay.Redraw();

            LayerOverlay queryResultMarkerOverlay = Map1.CustomOverlays["QueryResultMarkerOverlay"] as LayerOverlay;
            // clear the queried result markers from map.
            InMemoryFeatureLayer markerMemoryLayer = queryResultMarkerOverlay.Layers["MarkerMemoryLayer"] as InMemoryFeatureLayer;
            markerMemoryLayer.InternalFeatures.Clear();
            // clear the highlighted result markers.
            InMemoryFeatureLayer markerMemoryHighLightLayer = queryResultMarkerOverlay.Layers["MarkerMemoryHighLightLayer"] as InMemoryFeatureLayer;
            markerMemoryHighLightLayer.InternalFeatures.Clear();
            queryResultMarkerOverlay.Redraw();

            // make map back to normal mode.
            Map1.EditOverlay.Features.Clear();
            Map1.EditOverlay.TrackMode = TrackMode.None;
        }

        private string ZoomToFeature(string featureId)
        {
            LayerOverlay queryResultMarkerOverlay = Map1.CustomOverlays["QueryResultMarkerOverlay"] as LayerOverlay;
            InMemoryFeatureLayer markerMemoryLayer = queryResultMarkerOverlay.Layers["MarkerMemoryLayer"] as InMemoryFeatureLayer;
            InMemoryFeatureLayer markerMemoryHighLightLayer = queryResultMarkerOverlay.Layers["MarkerMemoryHighLightLayer"] as InMemoryFeatureLayer;

            Feature currentFeature = markerMemoryLayer.InternalFeatures.FirstOrDefault(f => f.Id == featureId);
            markerMemoryHighLightLayer.InternalFeatures.Clear();
            markerMemoryHighLightLayer.InternalFeatures.Add(currentFeature);

            PointShape center = currentFeature.GetShape() as PointShape;
            Map1.ZoomTo(center, Map1.ZoomLevelSet.GetZoomLevels()[16].Scale);

            queryResultMarkerOverlay.Redraw();

            return currentFeature.GetBoundingBox().GetWellKnownText();
        }

        private Collection<Feature> FilterEarthquakePoints(Collection<EarthquakeQueryConfiguration> queryConfigurations)
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

        private void DisplayQueryResult(IEnumerable<Feature> features)
        {
            DataTable resultTable = InternalHelper.GetQueriedResultTableDefination();
            foreach (Feature point in features)
            {
                DataRow dataRow = resultTable.NewRow();
                dataRow["id"] = point.Id;
                dataRow["year"] = point.ColumnValues["YEAR"];
                dataRow["longitude"] = point.ColumnValues["LONGITUDE"];
                dataRow["latitude"] = point.ColumnValues["LATITIUDE"];
                dataRow["depth_km"] = point.ColumnValues["DEPTH_KM"];
                dataRow["magnitude"] = point.ColumnValues["MAGNITUDE"];
                dataRow["location"] = point.ColumnValues["LOCATION"];
                resultTable.Rows.Add(dataRow);
            }

            rptQueryResult.DataSource = resultTable;
            rptQueryResult.DataBind();
        }

        private void InitializeMap()
        {
            Map1.MapUnit = GeographyUnit.Meter;
            Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            Map1.MapTools.OverlaySwitcher.Enabled = true;
            Map1.MapTools.OverlaySwitcher.BaseOverlayTitle = "ThinkGeo Cloud Maps: ";

            // add base layers.
            AddBaseMapLayers();
            // add the earthquake layer to as the data source.
            AddEarthquakeLayers();
            // add query shape layers, like track layer, marker layer and highlight layer etc.
            AddQueryResultLayers();
            // add adorment layers
            AddAdormentLayers();

            Map1.CurrentExtent = new RectangleShape(-14503631.6805645, 7498410.41581975, -7928840.70035357, 4171879.26511785);
        }

        private void AddBaseMapLayers()
        {
            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay thinkgeoCloudLightMapOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            thinkgeoCloudLightMapOverlay.Name = "Light";
            thinkgeoCloudLightMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Light;
            Map1.CustomOverlays.Add(thinkgeoCloudLightMapOverlay);

            ThinkGeoCloudRasterMapsOverlay thinkgeCloudDardMapOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            thinkgeCloudDardMapOverlay.Name = "Dark";
            thinkgeCloudDardMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Dark;
            Map1.CustomOverlays.Add(thinkgeCloudDardMapOverlay);

            ThinkGeoCloudRasterMapsOverlay thinkgeoCloudAerialMapOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            thinkgeoCloudAerialMapOverlay.Name = "Aerial";
            thinkgeoCloudAerialMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Aerial;
            Map1.CustomOverlays.Add(thinkgeoCloudAerialMapOverlay);

            ThinkGeoCloudRasterMapsOverlay thinkgeoCloudHybridMapOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            thinkgeoCloudHybridMapOverlay.Name = "Hybrid";
            thinkgeoCloudHybridMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Hybrid;
            Map1.CustomOverlays.Add(thinkgeoCloudHybridMapOverlay);
        }

        private void AddEarthquakeLayers()
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

        private void AddQueryResultLayers()
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

        private void AddAdormentLayers()
        {
            // ScaleBar
            ScaleBarAdornmentLayer scaleBarAdormentLayer = new ScaleBarAdornmentLayer();
            scaleBarAdormentLayer.XOffsetInPixel = 10;
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
            EarthquakeIsoLineFeatureLayer isoLineLayer = earthquakeOverlay.Layers["IsoLines Map"] as EarthquakeIsoLineFeatureLayer;
            for (int i = 0; i < isoLineLayer.IsoLineLevels.Count; i++)
            {
                LegendItem legendItem = new LegendItem();
                legendItem.TextStyle = new TextStyle(isoLineLayer.IsoLineLevels[i].ToString("f2"), new GeoFont("Arial", 10), new GeoSolidBrush(GeoColor.StandardColors.Black));
                legendItem.ImageStyle = isoLineLayer.LevelClassBreakStyle.ClassBreaks[i].CustomStyles[0];
                legendItem.ImageWidth = 25;

                isoLevelLegendLayer.LegendItems.Add(legendItem);
            }

            Map1.AdornmentOverlay.Layers.Add("IsoLineLevelLegendLayer", isoLevelLegendLayer);
        }
    }
}