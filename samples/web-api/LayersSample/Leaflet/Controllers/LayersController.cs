using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using ThinkGeo.Core;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.UI.WebApi;

namespace Layers.Controllers
{
    [Route("Layers")]
    public class LayersController : ControllerBase
    {
        private static readonly string baseDirectory = null;
        private static ProjectionConverter wgs84ToGoogleProjectionConverter;
        private static ProjectionConverter feetToGoogleProjectionConverter;
        private static readonly NoaaRadarRasterLayer noaaRadarRasterLayer;
        private static readonly NoaaWeatherStationFeatureLayer noaaWeatherStationFeatureLayer;

        static LayersController()
        {
            baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "App_Data");
            InitializeProjection();

            // Noaa layers need to download the imformantion from server, we can open to start downloading data when application starting.
            noaaRadarRasterLayer = new NoaaRadarRasterLayer();
            noaaRadarRasterLayer.Name = "noaaRadar";
            noaaRadarRasterLayer.ImageSource.ProjectionConverter = wgs84ToGoogleProjectionConverter;
            NoaaRadarMonitor.RadarUpdated += (sender, args) => NoaaRadarMonitor.StopMonitoring();
            noaaRadarRasterLayer.Open();

            noaaWeatherStationFeatureLayer = new NoaaWeatherStationFeatureLayer();
            noaaWeatherStationFeatureLayer.Name = "noaaWeather";
            noaaWeatherStationFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new NoaaWeatherStationStyle());
            noaaWeatherStationFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            noaaWeatherStationFeatureLayer.FeatureSource.ProjectionConverter = wgs84ToGoogleProjectionConverter;
            NoaaWeatherStationMonitor.StationsUpdated += (sender, args) => NoaaWeatherStationMonitor.StopMonitoring();
            noaaWeatherStationFeatureLayer.Open();
        }

        /// <summary>
        /// Load ShapeFileFeatureLayer.
        /// </summary>
        [Route("VectorLayers/shapeFile/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult LoadShapeFile(int z, int x, int y)
        {
            string shpFilePathName = string.Format(@"{0}/ShapeFile/USStates.shp", baseDirectory);
            ShapeFileFeatureLayer shapeFileFeatureLayer = new ShapeFileFeatureLayer(shpFilePathName);
            shapeFileFeatureLayer.Name = "shapeFile";
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush = new GeoSolidBrush(new GeoColor(50, GeoColors.Orange));
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColors.Black);
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(shapeFileFeatureLayer);

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Load TabFeatureLayer.
        /// </summary>
        [Route("VectorLayers/tab/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult LoadTabFile(int z, int x, int y)
        {
            string tabFilePathName = string.Format(@"{0}/TAB/USStates.tab", baseDirectory);
            TabFeatureLayer tabFeatureLayer = new TabFeatureLayer(tabFilePathName);
            tabFeatureLayer.StylingType = TabStylingType.StandardStyling;
            tabFeatureLayer.Name = "tab";
            tabFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush = new GeoSolidBrush(new GeoColor(50, GeoColors.Orange));
            tabFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColors.Black);
            tabFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            tabFeatureLayer.FeatureSource.ProjectionConverter = wgs84ToGoogleProjectionConverter;
            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(tabFeatureLayer);

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Load TinyGeoFeatureLayer
        /// </summary>
        [Route("VectorLayers/tinyGeo/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult LoadTinyGeo(int z, int x, int y)
        {
            string tinyGeoFilePathName = Path.Combine(baseDirectory, "TinyGeo", "USStates.tgeo");

            TinyGeoFeatureLayer tinyGeoFeatureLayer = new TinyGeoFeatureLayer(tinyGeoFilePathName);
            tinyGeoFeatureLayer.Name = "tinyGeo";
            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush = new GeoSolidBrush(new GeoColor(50, GeoColors.Orange));
            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColors.Black);
            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(tinyGeoFeatureLayer);

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Load FileGeoDatabaseFeatureLayer
        /// </summary>
        [Route("VectorLayers/fileGeoDatabase/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult LoadFileGeoDatabase(int z, int x, int y)
        {
            string fileGeoDatabaeFilePathName = string.Format(@"{0}/FileGeodatabase/USStates.gdb", baseDirectory);
            FileGeoDatabaseFeatureLayer fileGeoDatabaseLayer = new FileGeoDatabaseFeatureLayer(fileGeoDatabaeFilePathName);
            fileGeoDatabaseLayer.Name = "fileGeoDatabase";
            fileGeoDatabaseLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush = new GeoSolidBrush(new GeoColor(50, GeoColors.Orange));
            fileGeoDatabaseLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColors.Black);
            fileGeoDatabaseLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            fileGeoDatabaseLayer.FeatureSource.ProjectionConverter = wgs84ToGoogleProjectionConverter;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(fileGeoDatabaseLayer);

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Load SqliteFeatureLayer.
        /// </summary>
        [Route("VectorLayers/sqlite/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult LoadSqlite(int z, int x, int y)
        {
            string fileConnectionStr = string.Format(@"Data Source={0}/Sqlite/USStates.sqlite;", baseDirectory);

            // Please add  the ‘System.Data.SQLite Core (x86/x64)’ NuGet package to remove this build error.
            SqliteFeatureLayer sqliteFeatureLayer = new SqliteFeatureLayer(fileConnectionStr, "table_name", "id", "geometry");
            sqliteFeatureLayer.Name = "sqlite";
            sqliteFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush = new GeoSolidBrush(new GeoColor(50, GeoColors.Orange));
            sqliteFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColors.Black);
            sqliteFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(sqliteFeatureLayer);

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Load WkbFileFeatureLayer.
        /// </summary>
        [Route("VectorLayers/wkb/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult LoadWkbFile(int z, int x, int y)
        {
            string wkbFilePathName = Path .Combine (baseDirectory, "Wkb", "USStates.wkb") ;
            WkbFileFeatureLayer wkbFileFeatureLayer = new WkbFileFeatureLayer(wkbFilePathName);
            wkbFileFeatureLayer.Name = "wkb";
            wkbFileFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush = new GeoSolidBrush(new GeoColor(50, GeoColors.Orange));
            wkbFileFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColors.Black);
            wkbFileFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(wkbFileFeatureLayer);

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Load MapShapeLayer.
        /// </summary>
        [Route("VectorLayers/mapShape/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult LoadMapShapeLayer(int z, int x, int y)
        {
            MapShapeLayer mapShapeLayer = new MapShapeLayer();
            mapShapeLayer.Name = "mapShape";

            TextStyle textStyle = new TextStyle("name", (new GeoFont("Arial", 12)), new GeoSolidBrush(GeoColors.DarkOliveGreen));
            textStyle.HaloPen = new GeoPen(GeoColors.FloralWhite, 5);
            textStyle.SplineType = SplineType.ForceSplining;
            GeoPen pointPen = new GeoPen(GeoColor.FromArgb(255, GeoColors.Green), 8);
            LineStyle lineStyle = new LineStyle(new GeoPen(GeoColors.Orange, 5));

            int polygonCount = 1;
            IEnumerable<Feature> mapShapeLayerFeatures = SampleHelper.GetFeatures("MapShapeLayer");
            // According to the feature type setting style.
            foreach (var feature in mapShapeLayerFeatures)
            {
                MapShape mapShape = new MapShape(feature);
                switch (feature.GetWellKnownType())
                {
                    case WellKnownType.Point:
                        mapShape.ZoomLevels.ZoomLevel01.DefaultPointStyle.OutlinePen = pointPen;
                        mapShape.ZoomLevels.ZoomLevel01.DefaultTextStyle = textStyle;
                        mapShape.ZoomLevels.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                        mapShapeLayer.MapShapes.Add(feature.Id, mapShape);
                        break;
                    case WellKnownType.Line:
                        mapShape.ZoomLevels.ZoomLevel01.DefaultLineStyle = lineStyle;
                        mapShape.ZoomLevels.ZoomLevel01.DefaultTextStyle = textStyle;
                        mapShape.ZoomLevels.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                        mapShapeLayer.MapShapes.Add(feature.Id, mapShape);
                        break;
                    case WellKnownType.Polygon:
                        // Set up different styles based on polygon feature order.
                        if (polygonCount == 1)
                            mapShape.ZoomLevels.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoColors.Transparent, 0), new GeoSolidBrush(new GeoColor(255, 211, 226, 190)));

                        if (polygonCount == 2)
                            mapShape.ZoomLevels.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoColors.Transparent, 0), new GeoSolidBrush(new GeoColor(255, 240, 240, 216)));

                        if (polygonCount == 3)
                            mapShape.ZoomLevels.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoColors.Transparent, 0), new GeoSolidBrush(new GeoColor(255, 160, 207, 235)));
                        mapShape.ZoomLevels.ZoomLevel01.DefaultTextStyle = textStyle;
                        mapShape.ZoomLevels.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColors.Black);
                        mapShape.ZoomLevels.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                        mapShapeLayer.MapShapes.Add(feature.Id, mapShape);
                        polygonCount++;

                        break;
                }
            }

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(mapShapeLayer);

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Load InMemoryFeatureLayer
        /// </summary>
        [Route("VectorLayers/inMemoryFeature/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult LoadInMemoryLayer(int z, int x, int y)
        {
            InMemoryFeatureLayer inMemoryFeatureLayer = new InMemoryFeatureLayer();
            inMemoryFeatureLayer.Name = "inMemoryFeature";
            // Get features for inMemoryFeature sample.
            IEnumerable<Feature> inMemoryLayerFeatures = SampleHelper.GetFeatures("InMemoryLayer");
            foreach (var feature in inMemoryLayerFeatures)
            {
                inMemoryFeatureLayer.InternalFeatures.Add(feature);
            }

            (inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush as GeoSolidBrush).Color = GeoColor.FromArgb(100, GeoColors.RoyalBlue);
            inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColors.Blue;
            inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.OuterPen = new GeoPen(GeoColor.FromArgb(200, GeoColors.Red), 5);
            inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.OutlinePen = new GeoPen(GeoColor.FromArgb(255, GeoColors.Green), 8);
            TextStyle textStyle = new TextStyle("name", (new GeoFont("Arial", 12)), new GeoSolidBrush(GeoColors.DarkOliveGreen));
            textStyle.HaloPen = new GeoPen(GeoColors.FloralWhite, 3);
            inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = textStyle;
            inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(inMemoryFeatureLayer);

            return DrawTileImage(layerOverlay, z, x, y);
        }


        /// <summary>
        /// Load MrSidRasterLayer.
        /// </summary>
        [Route("RasterLayers/mrsid/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult LoadMrsidFile(int z, int x, int y)
        {
            LayerOverlay layerOverlay = new LayerOverlay();

            string mrSidFilePathName = Path.Combine(baseDirectory, "MrSid", "US380AndGeeRoad.sid");
            MrSidRasterLayer mrsidRasterLayer = new MrSidRasterLayer(mrSidFilePathName);
            mrsidRasterLayer.Name = "mrsid";
            mrsidRasterLayer.ImageSource.ProjectionConverter = feetToGoogleProjectionConverter;
            layerOverlay.Layers.Add(mrsidRasterLayer);

            // Attach road layer for display road and information.
            ShapeFileFeatureLayer txlka40FeatureLayer = new ShapeFileFeatureLayer(string.Format(@"{0}/MrSid/TXlkaA40.shp", baseDirectory));
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel17.DefaultLineStyle = new LineStyle(new GeoPen(new GeoColor(255, 255, 255, 255), 8) { StartCap = DrawingLineCap.Round, EndCap = DrawingLineCap.Round });
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle = new TextStyle("[fedirp] [fename] [fetype] [fedirs]", new GeoFont("Verdana", 10), new GeoSolidBrush(new GeoColor(255, 102, 102, 102)))
            {
                HaloPen = new GeoPen(new GeoColor(200, 255, 255, 255), 3),
                DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels,
                ForceLineCarriage = true,
                OverlappingRule = LabelOverlappingRule.NoOverlapping,
                PolygonLabelingLocationMode = PolygonLabelingLocationMode.Centroid,
                GridSize = 0,
                FittingPolygon = true,
                SplineType = SplineType.StandardSplining,
                TextPlacement = TextPlacement.Center
            };

            layerOverlay.Layers.Add("TXlkaA40", txlka40FeatureLayer);

            ShapeFileFeatureLayer txlkaA20FeatureLayer = new ShapeFileFeatureLayer(string.Format(@"{0}/MrSid/TXlkaA20.shp", baseDirectory));
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel17.DefaultLineStyle = new LineStyle(new GeoPen(new GeoColor(255, 255, 222, 190), 10) { StartCap = DrawingLineCap.Round, EndCap = DrawingLineCap.Round });
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle = new TextStyle("[fedirp] [fename] [fetype] [fedirs]", new GeoFont("Verdana", 10, DrawingFontStyles.Bold), new GeoSolidBrush(new GeoColor(255, 102, 102, 102)))
            {
                DuplicateRule = LabelDuplicateRule.OneDuplicateLabelPerQuadrant,
                ForceLineCarriage = true,
                OverlappingRule = LabelOverlappingRule.NoOverlapping,
                SplineType = SplineType.None,
                TextLineSegmentRatio = 4,
                GridSize = 0,
                ForceHorizontalLabelForLine = true,
                //MaskMargin = 2,
                MaskType = MaskType.RoundedCorners,
                SuppressPartialLabels = true,
                Mask = new AreaStyle(new GeoPen(new GeoColor(255, 153, 153, 153), 2)) { DrawingLevel = DrawingLevel.LabelLevel }
            };
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add("TXlkaA20", txlkaA20FeatureLayer);

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Load Jpeg2000RasterLayer.
        /// </summary>
        [Route("RasterLayers/jpeg2000/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult LoadJpeg2000(int z, int x, int y)
        {
            string jpeg2000FilePathName = string.Format(@"{0}/Jpeg2000/World.jp2", baseDirectory);
            string jpeg2000WorldFilePathName = Path.ChangeExtension(jpeg2000FilePathName, ".j2w");
            Jpeg2000RasterLayer jpeg2000RasterLayer = new Jpeg2000RasterLayer(jpeg2000FilePathName, jpeg2000WorldFilePathName);
            jpeg2000RasterLayer.Name = "jpeg2000";

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(jpeg2000RasterLayer);

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Load GeoTiffRasterLayer.
        /// </summary>
        [Route("RasterLayers/geotiff/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult LoadGeoTiff(int z, int x, int y)
        {
            string tiffFilePathName = string.Format(@"{0}/Tiff/World.tif", baseDirectory);
            string tiffWorldFilePathName = Path.ChangeExtension(tiffFilePathName, "tfw");
            GeoTiffRasterLayer geoTiffRasterLayer = new GeoTiffRasterLayer(tiffFilePathName, tiffWorldFilePathName);
            geoTiffRasterLayer.Name = "geotiff";

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(geoTiffRasterLayer);

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Load GdiPlusRasterLayer.
        /// </summary>
        [Route("RasterLayers/nativeImage/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult LoadNativeImage(int z, int x, int y)
        {
            NativeImageRasterLayer gdiPlusRasterLayer = new NativeImageRasterLayer(string.Format(@"{0}/PNG/ThinkGeoLogo.png", baseDirectory),
                new RectangleShape(-10776876.6105256, 3912354.07403825, -10776796.6105256, 3912334.07403825));
            gdiPlusRasterLayer.Name = "nativeImage";

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(gdiPlusRasterLayer);

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Load NoaaRadarRasterLayer.
        /// </summary>
        [Route("WebServiceLayers/noaaRadar/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult LoadNoaaRadar(int z, int x, int y)
        {
            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(noaaRadarRasterLayer);
            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Load NoaaWeatherStationFeatureLayer.
        /// </summary>
        [Route("WebServiceLayers/noaaWeather/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult LoadNoaaWeatherStation(int z, int x, int y)
        {
            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(noaaWeatherStationFeatureLayer);

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Load WmsRasterLayer.
        /// </summary>
        [Route("WebServiceLayers/wms/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult LoadWms(int z, int x, int y)
        {
            WmsRasterLayer wmsLayer = new WmsRasterLayer();
            wmsLayer.Name = "wms";
            wmsLayer.Crs = "EPSG:900913";
            wmsLayer.Uri = new Uri("https://cloud.thinkgeo.com/api/v1/maps/wms");
            wmsLayer.ActiveLayerNames.Add("WorldStreets");
            wmsLayer.Parameters.Add("STYLES", "Light");
            wmsLayer.Parameters.Add("apikey", "PIbGd76RyHKod99KptWTeb-Jg9JUPEPUBFD3SZJYLDE~");

              LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(wmsLayer);

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Load CustomizedLayers.
        /// </summary>
        [Route("CustomizedLayers/customized/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult LoadCustomized(int z, int x, int y)
        {
            ThinkGeoHeadquartersFeatureLayer thinkGeoHeadquartersFeatureLayer = new ThinkGeoHeadquartersFeatureLayer();
            thinkGeoHeadquartersFeatureLayer.Name = "CustomizedLayer";

            TextStyle textStyle = new TextStyle("name", (new GeoFont("Arial", 12)), new GeoSolidBrush(GeoColors.DarkOliveGreen));
            textStyle.HaloPen = new GeoPen(GeoColors.FloralWhite, 5);
            textStyle.SplineType = SplineType.ForceSplining;

            thinkGeoHeadquartersFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush = new GeoSolidBrush(new GeoColor(50, GeoColors.Orange));
            thinkGeoHeadquartersFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColors.Black);
            thinkGeoHeadquartersFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = new LineStyle(new GeoPen(GeoColors.Orange, 5));
            thinkGeoHeadquartersFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.OutlinePen = new GeoPen(GeoColor.FromArgb(255, GeoColors.Green), 8);
            thinkGeoHeadquartersFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = textStyle;
            thinkGeoHeadquartersFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(thinkGeoHeadquartersFeatureLayer);

            return DrawTileImage(layerOverlay, z, x, y);
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

        /// <summary>
        /// Initializes projection.
        /// </summary>
        private static void InitializeProjection()
        {
            wgs84ToGoogleProjectionConverter = new ProjectionConverter(4326, 3857);

            feetToGoogleProjectionConverter = new ProjectionConverter();
            string projStr = System.IO.File.ReadAllText(string.Format(@"{0}/MrSid/US380AndGeeRoad.prj", baseDirectory));
            feetToGoogleProjectionConverter.InternalProjection = new Projection(projStr);
            feetToGoogleProjectionConverter.ExternalProjection = new Projection(Projection.GetGoogleMapProjString());
        }
    }
}
