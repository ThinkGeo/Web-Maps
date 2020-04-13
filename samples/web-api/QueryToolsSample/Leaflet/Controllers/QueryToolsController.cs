using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using ThinkGeo.Core;
using ThinkGeo.UI.WebApi;

namespace QueryTools.Controllers
{
    [Route("QueryTools")]
    public class QueryToolsController : ControllerBase
    {
        private readonly FeatureLayer sourceLayer;
        private static readonly string baseDirectory;
        private static InMemoryFeatureLayer sqlQueryResultLayer;
        private static InMemoryFeatureLayer spatialQueryTargetLayer;

        static QueryToolsController()
        {
            baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot","App_Data");


            // Create query result layer.
            sqlQueryResultLayer = new InMemoryFeatureLayer();
            sqlQueryResultLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(new GeoColor(200, GeoColors.PastelBlue)));
            sqlQueryResultLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.Blue;
            sqlQueryResultLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Create spatial query layer.
            spatialQueryTargetLayer = new InMemoryFeatureLayer();
            spatialQueryTargetLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(new GeoColor(50, GeoColors.LightBlue)));
            spatialQueryTargetLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.DarkBlue;
            spatialQueryTargetLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        }

        public QueryToolsController()
        {
            // Create the source layer for displaying countries.
            sourceLayer = GetSourceLayer();
            sourceLayer.Open();
        }

        /// <summary>
        /// Initialize the query source layer, all sample use same source layer, this layer only need to be initialized once.. 
        /// </summary>
        [Route("InitializeSourceLayer/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult InitializeSourceLayer(int z, int x, int y)
        {
            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(sourceLayer);
            return DrawTileImage(layerOverlay, z, x, y);
        }

        [Route("SpatialQuery/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult SpatialQuery(int z, int x, int y, string queryType = "Within")
        {
            // Get the spatial query result features by query type.
            Collection<Feature> spatialQueryResults = GetSpatialQueryResultsByQueryType(queryType, sourceLayer);

            // Create the spatial query result layer.
            InMemoryFeatureLayer spatialQueryResultLayer = new InMemoryFeatureLayer();
            spatialQueryResultLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush = new GeoSolidBrush(new GeoColor(40, GeoColors.Orange));
            spatialQueryResultLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColors.Orange);
            spatialQueryResultLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            foreach (Feature feature in spatialQueryResults)
            {
                spatialQueryResultLayer.InternalFeatures.Add(feature);
            }

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(spatialQueryResultLayer);
            layerOverlay.Layers.Add(spatialQueryTargetLayer);
            return DrawTileImage(layerOverlay, z, x, y);
        }

        [Route("QueryFeaturesWithinDistance/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult QueryFeaturesWithinDistance(int z, int x, int y, double distance = 1000, double lng = 1115369.11, double lat = 1937220.04)
        {
            Feature targetPointFeature = new Feature(lng, lat);

            // Query features within distance.
            Collection<Feature> spatialQueryResults = sourceLayer.QueryTools.GetFeaturesWithinDistanceOf(targetPointFeature, GeographyUnit.Meter, DistanceUnit.Kilometer, distance, ReturningColumnsType.NoColumns);

            // Create the spatial query result layer.
            InMemoryFeatureLayer queryResultLayer = new InMemoryFeatureLayer();
            queryResultLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(new GeoColor(200, GeoColors.PastelBlue)));
            queryResultLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.Blue;
            PointStyle pinImageStyle = new PointStyle(new GeoImage(Path.Combine(baseDirectory, "pin.png")));
            pinImageStyle.YOffsetInPixel = -(pinImageStyle.Image.Width / 2);
            queryResultLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = pinImageStyle;
            queryResultLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            foreach (Feature feature in spatialQueryResults)
            {
                queryResultLayer.InternalFeatures.Add(feature);
            }

            queryResultLayer.InternalFeatures.Add(targetPointFeature);

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(queryResultLayer);
            return DrawTileImage(layerOverlay, z, x, y);
        }

        [Route("QueryAllFeaturesDataRequests")]
        [HttpGet]
        public string QueryAllFeaturesDataRequests()
        {
            StringBuilder stringBuilder = new StringBuilder();

            bool isHeader = true;
            // Get all features with all columns.
            Collection<Feature> allFeatures = sourceLayer.FeatureSource.GetAllFeatures(ReturningColumnsType.AllColumns);

            // Get all columns format it to HTML content.
            foreach (Feature feature in allFeatures)
            {
                int columnIndex = feature.ColumnValues.Count - 1;

                if (isHeader)
                {
                    // Add table header.
                    stringBuilder.Append("<tr><th></th>");
                    for (int i = columnIndex; i >= 0; i--)
                    {
                        stringBuilder.Append("<th>" + feature.ColumnValues.Keys.ElementAt(i) + "</th>");
                    }
                    stringBuilder.Append("</tr>");
                    isHeader = false;
                }
                // Add zoom to extent function by HTML centent.
                stringBuilder.Append("<tr><td><a style='width:14px;display:block;'><img alt='Search' title='Zoom to country' src='Images/find.png'");
                PointShape latlng = feature.GetShape().GetCenterPoint();
                stringBuilder.Append("onclick='zoomToExtent(" + latlng.X + "," + latlng.Y + "," + feature.Id + ")'");
                stringBuilder.Append("/></a></td>");
                // Add column values to HTML centent.
                for (int i = columnIndex; i >= 0; i--)
                {
                    stringBuilder.Append("<td>" + feature.ColumnValues.Values.ElementAt(i) + "</td>");
                }
                stringBuilder.Append("</tr>");
            }

            return stringBuilder.ToString();
        }

        [Route("QueryAllFeatures/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult QueryAllFeatures(int z, int x, int y, string featureId)
        {
            return DrawTileImage(GetHighlightOverlayByFeatureId(featureId), z, x, y);
        }

        [Route("QueryColumnDataDataRequests")]
        [HttpGet]
        public string QueryColumnDataDataRequests(double lng = 1115369.11, double lat = 1937220.04)
        {
            // Get all of the features that contain the target shape.
            Collection<Feature> selectedFeatures = sourceLayer.QueryTools.GetFeaturesContaining(new PointShape(lng, lat), ReturningColumnsType.AllColumns);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<tr><th>Key</th><th>Value</th></tr>");
            if (selectedFeatures.Count > 0)
            {
                // Add feature's column keys and values to HTML centent. 
                foreach (var column in selectedFeatures[0].ColumnValues)
                {
                    stringBuilder.Append(" <tr><td>" + column.Key + "</td><td>" + column.Value + "</td></tr>");
                }
            }
            return stringBuilder.ToString();
        }

        [Route("QueryColumnData/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult QueryColumnData(int z, int x, int y, double lng = 1115369.11, double lat = 1937220.04)
        {
            return DrawTileImage(GetHighlightOverlayByLatLng(lng, lat), z, x, y);
        }

        [Route("SQLQueryDataRequests")]
        [HttpGet]
        public string SQLQueryDataRequests(string sqlString = "Select CNTRY_NAME,POP_CNTRY from Countries Where LANDLOCKED='Y'")
        {
            sourceLayer.Open();
            // Get SQL query result and format it to HTML centent.
            DataTable dataTable = sourceLayer.QueryTools.ExecuteQuery(sqlString);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("<tr>");
            foreach (var column in dataTable.Columns)
            {
                stringBuilder.Append("<th>" + column + "</th>");
            }
            stringBuilder.Append("</tr>");


            foreach (DataRow row in dataTable.Rows)
            {
                stringBuilder.Append("<tr>");

                foreach (var column in dataTable.Columns)
                {
                    stringBuilder.Append("<td>" + row[column.ToString()]);
                }
                stringBuilder.Append("</tr>");
            }

            // Update sql query result layer.
            sqlQueryResultLayer.InternalFeatures.Clear();
            var features = sourceLayer.FeatureSource.GetFeaturesByColumnValue("LANDLOCKED", "Y");
            foreach (Feature feature in features)
            {
                sqlQueryResultLayer.InternalFeatures.Add(feature.Id, feature);
            }
            return stringBuilder.ToString();
        }

        [Route("SQLQuery/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult SQLQuery(int z, int x, int y)
        {
            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(sqlQueryResultLayer);
            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Gets source layer for query sample.
        /// </summary>
        private static ShapeFileFeatureLayer GetSourceLayer()
        {
            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "Countries.shp"));
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColors.Transparent, GeoColor.FromArgb(100, GeoColors.Green));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            return worldLayer;
        }

        /// <summary>
        /// Gets the highlight overlay by given geographic coordinates.
        /// </summary>
        private LayerOverlay GetHighlightOverlayByLatLng(double lng, double lat)
        {
            LayerOverlay layerOverlay = new LayerOverlay();
            PointShape targetPointShape = new PointShape(lng, lat);

            Collection<Feature> queryResults = sourceLayer.QueryTools.GetFeaturesContaining(targetPointShape, ReturningColumnsType.NoColumns);
            InMemoryFeatureLayer featureLayer = new InMemoryFeatureLayer();
            featureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(new GeoColor(200, GeoColors.PastelBlue)));
            featureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.Blue;
            featureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            foreach (Feature feature in queryResults)
            {
                featureLayer.InternalFeatures.Add(feature);
            }
            layerOverlay.Layers.Add(featureLayer);

            return layerOverlay;
        }


        /// <summary>
        /// Gets the highlight overlay by given feature id.
        /// </summary>
        private LayerOverlay GetHighlightOverlayByFeatureId(string featureId)
        {
            Feature feature = sourceLayer.QueryTools.GetFeatureById(featureId, ReturningColumnsType.NoColumns);

            InMemoryFeatureLayer featureLayer = new InMemoryFeatureLayer();
            featureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(new GeoColor(200, GeoColors.PastelBlue)));
            featureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.Blue;
            featureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            featureLayer.InternalFeatures.Add(feature);

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(featureLayer);

            return layerOverlay;
        }


        /// <summary>
        /// Queryes features by different query type.
        /// </summary>
        private Collection<Feature> GetSpatialQueryResultsByQueryType(string queryType, FeatureLayer sourceLayer)
        {
            Feature targetFeature = new Feature(new RectangleShape(-5565974.53966368, 2273030.92698769, 5565974.53966368, -2273030.92698769));
            Collection<Feature> spatialQueryResults = null;
            switch ((QueryType)Enum.Parse(typeof(QueryType), queryType))
            {
                case QueryType.Within:
                    spatialQueryResults = sourceLayer.QueryTools.GetFeaturesWithin(targetFeature, ReturningColumnsType.NoColumns);
                    break;
                case QueryType.Contains:
                    targetFeature = new Feature(new RectangleShape(1856497.90132799, 1915249.75262707, 2443532.81027539, 1582596.63755688));
                    spatialQueryResults = sourceLayer.QueryTools.GetFeaturesContaining(targetFeature, ReturningColumnsType.NoColumns);
                    break;
                case QueryType.Disjoint:
                    spatialQueryResults = sourceLayer.QueryTools.GetFeaturesDisjointed(targetFeature, ReturningColumnsType.NoColumns);
                    break;
                case QueryType.Intersects:
                    spatialQueryResults = sourceLayer.QueryTools.GetFeaturesIntersecting(targetFeature, ReturningColumnsType.NoColumns);
                    break;
                case QueryType.Overlaps:
                    spatialQueryResults = sourceLayer.QueryTools.GetFeaturesOverlapping(targetFeature, ReturningColumnsType.NoColumns);
                    break;
                case QueryType.Touches:
                    targetFeature = new Feature(new RectangleShape(-3893986.7364521623, 2705846.1871381178, 7585780.5470837476, -714646.63483386114));
                    spatialQueryResults = sourceLayer.QueryTools.GetFeaturesTouching(targetFeature, ReturningColumnsType.NoColumns);
                    break;
                default:
                    break;
            }

            // Refresh target feature layer.
            lock (spatialQueryTargetLayer)
            {
                spatialQueryTargetLayer.InternalFeatures.Clear();
                spatialQueryTargetLayer.InternalFeatures.Add(targetFeature);
            }

            return spatialQueryResults;
        }

        /// <summary>
        /// Draw the map and return the image back to client in an IActionResult. 
        /// </summary>
        private IActionResult DrawTileImage(LayerOverlay layerOverlay, int z, int x, int y)
        {
            using (GeoImage image = new GeoImage(256, 256))
            {
                GeoCanvas geoCanvas = GeoCanvas.CreateDefaultGeoCanvas();
                RectangleShape boundingBox = WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, GeographyUnit.Meter);
                geoCanvas.BeginDrawing(image, boundingBox, GeographyUnit.Meter);
                layerOverlay.Draw(geoCanvas);
                geoCanvas.EndDrawing();

                byte[] imageBytes = image.GetImageBytes(GeoImageFormat.Png);

                return File(imageBytes, "image/png");
            }
        }
    }
}
