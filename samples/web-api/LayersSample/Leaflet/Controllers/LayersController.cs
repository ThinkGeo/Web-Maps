using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebApi;

namespace Layers.Controllers
{
    [RoutePrefix("Layers")]
    public class LayersController : ApiController
    {
        private static readonly string baseDirectory;
        private static Proj4Projection wgs84ToGoogleProjection;
        private static Proj4Projection feetToGoogleProjection;
        private static readonly NoaaRadarRasterLayer noaaRadarRasterLayer;
        private static readonly NoaaWeatherStationFeatureLayer noaaWeatherStationFeatureLayer;

        static LayersController()
        {
            baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
 
            InitializeProjection();

            // Noaa layers need to download the imformantion from server, we can open to start downloading data when application starting.
            noaaRadarRasterLayer = new NoaaRadarRasterLayer();
            noaaRadarRasterLayer.Name = "noaaRadar";
            noaaRadarRasterLayer.ImageSource.Projection = wgs84ToGoogleProjection;
            NoaaRadarMonitor.RadarUpdated += (sender, args) => NoaaRadarMonitor.StopMonitoring();
            noaaRadarRasterLayer.Open();

            noaaWeatherStationFeatureLayer = new NoaaWeatherStationFeatureLayer();
            noaaWeatherStationFeatureLayer.Name = "noaaWeather";
            noaaWeatherStationFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new NoaaWeatherStationStyle());
            noaaWeatherStationFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            noaaWeatherStationFeatureLayer.FeatureSource.Projection = wgs84ToGoogleProjection;
            NoaaWeatherStationMonitor.StationsUpdated += (sender, args) => NoaaWeatherStationMonitor.StopMonitoring();
            noaaWeatherStationFeatureLayer.Open();
        }

        /// <summary>
        /// Load ShapeFileFeatureLayer.
        /// </summary>
        [Route("VectorLayers/shapeFile/{z}/{x}/{y}")]
        [HttpGet]
        public HttpResponseMessage LoadShapeFile(int z, int x, int y)
        {
            string shpFilePathName = string.Format(@"{0}/ShapeFile/USStates.shp", baseDirectory);
            ShapeFileFeatureLayer shapeFileFeatureLayer = new ShapeFileFeatureLayer(shpFilePathName);
            shapeFileFeatureLayer.Name = "shapeFile";
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush = new GeoSolidBrush(new GeoColor(50, GeoColor.SimpleColors.Orange));
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColor.SimpleColors.Black);
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
        public HttpResponseMessage LoadTabFile(int z, int x, int y)
        {
			string tabFilePathName = string.Format(@"{0}/TAB/USStates.tab", baseDirectory);
            TabFeatureLayer tabFeatureLayer = new TabFeatureLayer(tabFilePathName);
            tabFeatureLayer.StylingType = TabStylingType.StandardStyling;
            tabFeatureLayer.Name = "tab";
            tabFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush = new GeoSolidBrush(new GeoColor(50, GeoColor.SimpleColors.Orange));
            tabFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColor.SimpleColors.Black);
            tabFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            tabFeatureLayer.FeatureSource.Projection = wgs84ToGoogleProjection;
            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(tabFeatureLayer);

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Load TinyGeoFeatureLayer
        /// </summary>
        [Route("VectorLayers/tinyGeo/{z}/{x}/{y}")]
        [HttpGet]
        public HttpResponseMessage LoadTinyGeo(int z, int x, int y)
        {
            string tinyGeoFilePathName = string.Format(@"{0}/TinyGeo/USStates.tgeo", baseDirectory);

            TinyGeoFeatureLayer tinyGeoFeatureLayer = new TinyGeoFeatureLayer(tinyGeoFilePathName);
            tinyGeoFeatureLayer.Name = "tinyGeo";
            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush = new GeoSolidBrush(new GeoColor(50, GeoColor.SimpleColors.Orange));
            tinyGeoFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColor.SimpleColors.Black);
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
        public HttpResponseMessage LoadFileGeoDatabase(int z, int x, int y)
        {
			string fileGeoDatabaeFilePathName = string.Format(@"{0}/FileGeodatabase/USStates.gdb", baseDirectory);
            FileGeoDatabaseFeatureLayer fileGeoDatabaseLayer = new FileGeoDatabaseFeatureLayer(fileGeoDatabaeFilePathName, "USStates");
            fileGeoDatabaseLayer.Name = "fileGeoDatabase";
            fileGeoDatabaseLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush = new GeoSolidBrush(new GeoColor(50, GeoColor.SimpleColors.Orange));
            fileGeoDatabaseLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColor.SimpleColors.Black);
            fileGeoDatabaseLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            fileGeoDatabaseLayer.FeatureSource.Projection = wgs84ToGoogleProjection;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(fileGeoDatabaseLayer);

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Load SqliteFeatureLayer.
        /// </summary>
        [Route("VectorLayers/sqlite/{z}/{x}/{y}")]
        [HttpGet]
        public HttpResponseMessage LoadSqlite(int z, int x, int y)
        {
            string fileConnectionStr = string.Format(@"Data Source={0}/Sqlite/USStates.sqlite;Version=3;", baseDirectory);

            // Please add  the ‘System.Data.SQLite Core (x86/x64)’ NuGet package to remove this build error.
            SqliteFeatureLayer sqliteFeatureLayer = new SqliteFeatureLayer(fileConnectionStr, "table_name", "id", "geometry");
            sqliteFeatureLayer.Name = "sqlite";
            sqliteFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush = new GeoSolidBrush(new GeoColor(50, GeoColor.SimpleColors.Orange));
            sqliteFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColor.SimpleColors.Black);
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
        public HttpResponseMessage LoadWkbFile(int z, int x, int y)
        {
            string mrSidFilePathName = string.Format(@"{0}/Wkb/USStates.wkb", baseDirectory);
            WkbFileFeatureLayer wkbFileFeatureLayer = new WkbFileFeatureLayer(mrSidFilePathName);
            wkbFileFeatureLayer.Name = "wkb";
            wkbFileFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush = new GeoSolidBrush(new GeoColor(50, GeoColor.SimpleColors.Orange));
            wkbFileFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColor.SimpleColors.Black);
            wkbFileFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(wkbFileFeatureLayer);

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Load KmlFeatureLayer.
        /// </summary>
        [Route("VectorLayers/kml/{z}/{x}/{y}")]
        [HttpGet]
        public HttpResponseMessage LoadKmlFile(int z, int x, int y)
        {
            string kmlFilePathName = string.Format(@"{0}/KML/ThinkGeoHeadquarters.kml", baseDirectory);
            KmlFeatureLayer kmlFeatureLayer = new KmlFeatureLayer(kmlFilePathName, KmlStylingType.StandardStyling);
            kmlFeatureLayer.Name = "kml";

            TextStyle textStyle = new TextStyle("name", (new GeoFont("Arial", 12)), new GeoSolidBrush(GeoColor.StandardColors.DarkOliveGreen));
            textStyle.HaloPen = new GeoPen(GeoColor.StandardColors.FloralWhite, 5);
            textStyle.SplineType = SplineType.ForceSplining;

            // Set area style,line style,point style and text style.
            kmlFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush = new GeoSolidBrush(new GeoColor(50, GeoColor.SimpleColors.Orange));
            kmlFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColor.SimpleColors.Black);
            kmlFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = new LineStyle(new GeoPen(GeoColor.SimpleColors.Orange, 5));
            kmlFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.SymbolPen = new GeoPen(GeoColor.FromArgb(255, GeoColor.StandardColors.Green), 8);
            kmlFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = textStyle;
            kmlFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(kmlFeatureLayer);

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Load MapShapeLayer.
        /// </summary>
        [Route("VectorLayers/mapShape/{z}/{x}/{y}")]
        [HttpGet]
        public HttpResponseMessage LoadMapShapeLayer(int z, int x, int y)
        {
            MapShapeLayer mapShapeLayer = new MapShapeLayer();
            mapShapeLayer.Name = "mapShape";

            TextStyle textStyle = new TextStyle("name", (new GeoFont("Arial", 12)), new GeoSolidBrush(GeoColor.StandardColors.DarkOliveGreen));
            textStyle.HaloPen = new GeoPen(GeoColor.StandardColors.FloralWhite, 5);
            textStyle.SplineType = SplineType.ForceSplining;
            GeoPen pointPen = new GeoPen(GeoColor.FromArgb(255, GeoColor.StandardColors.Green), 8);
            LineStyle lineStyle = new LineStyle(new GeoPen(GeoColor.SimpleColors.Orange, 5));

            int polygonCount = 1;
            IEnumerable<Feature> mapShapeLayerFeatures = SampleHelper.GetFeatures("MapShapeLayer");
            // According to the feature type setting style.
            foreach (var feature in mapShapeLayerFeatures)
            {
                MapShape mapShape = new MapShape(feature);
                switch (feature.GetWellKnownType())
                {
                    case WellKnownType.Point:
                        mapShape.ZoomLevels.ZoomLevel01.DefaultPointStyle.SymbolPen = pointPen;
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
                            mapShape.ZoomLevels.ZoomLevel01.DefaultAreaStyle = WorldStreetsAreaStyles.Park();

                        if (polygonCount == 2)
                            mapShape.ZoomLevels.ZoomLevel01.DefaultAreaStyle = WorldStreetsAreaStyles.School();

                        if (polygonCount == 3)
                            mapShape.ZoomLevels.ZoomLevel01.DefaultAreaStyle = WorldStreetsAreaStyles.Water();
                        mapShape.ZoomLevels.ZoomLevel01.DefaultTextStyle = textStyle;
                        mapShape.ZoomLevels.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColor.SimpleColors.Black);
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
        public HttpResponseMessage LoadInMemoryLayer(int z, int x, int y)
        {
            InMemoryFeatureLayer inMemoryFeatureLayer = new InMemoryFeatureLayer();
            inMemoryFeatureLayer.Name = "inMemoryFeature";
            // Get features for inMemoryFeature sample.
            IEnumerable<Feature> inMemoryLayerFeatures = SampleHelper.GetFeatures("InMemoryLayer");
            foreach (var feature in inMemoryLayerFeatures)
            {
                inMemoryFeatureLayer.InternalFeatures.Add(feature);
            }

            inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.FromArgb(100, GeoColor.StandardColors.RoyalBlue);
            inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.StandardColors.Blue;
            inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.OuterPen = new GeoPen(GeoColor.FromArgb(200, GeoColor.StandardColors.Red), 5);
            inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.SymbolPen = new GeoPen(GeoColor.FromArgb(255, GeoColor.StandardColors.Green), 8);
            TextStyle textStyle = new TextStyle("name", (new GeoFont("Arial", 12)), new GeoSolidBrush(GeoColor.StandardColors.DarkOliveGreen));
            textStyle.HaloPen = new GeoPen(GeoColor.StandardColors.FloralWhite, 3);
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
        public HttpResponseMessage LoadMrsidFile(int z, int x, int y)
        {
            LayerOverlay layerOverlay = new LayerOverlay();

			string mrSidFilePathName = string.Format(@"{0}/MrSid/US380AndGeeRoad.sid", baseDirectory);
            MrSidRasterLayer mrsidRasterLayer = new MrSidRasterLayer(mrSidFilePathName);
            mrsidRasterLayer.Name = "mrsid";
            mrsidRasterLayer.ImageSource.Projection = feetToGoogleProjection;
            layerOverlay.Layers.Add(mrsidRasterLayer);

            // Attach road layer for display road and information.
			ShapeFileFeatureLayer txlka40FeatureLayer = new ShapeFileFeatureLayer(string.Format(@"{0}/MrSid/TXlkaA40.shp", baseDirectory));
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel17.DefaultLineStyle = WorldStreetsLineStyles.RoadFill(8);
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle = WorldStreetsTextStyles.GeneralPurpose("[fedirp] [fename] [fetype] [fedirs]", 10f);
            txlka40FeatureLayer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add("TXlkaA40", txlka40FeatureLayer);

            ShapeFileFeatureLayer txlkaA20FeatureLayer = new ShapeFileFeatureLayer(string.Format(@"{0}/MrSid/TXlkaA20.shp", baseDirectory));
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel17.DefaultLineStyle = WorldStreetsLineStyles.Highway(10);
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel17.DefaultTextStyle = WorldStreetsTextStyles.TrunkRoadSheild("[fedirp] [fename] [fetype] [fedirs]", 10f);
            txlkaA20FeatureLayer.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add("TXlkaA20", txlkaA20FeatureLayer);

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Load Jpeg2000RasterLayer.
        /// </summary>
        [Route("RasterLayers/jpeg2000/{z}/{x}/{y}")]
        [HttpGet]
        public HttpResponseMessage LoadJpeg2000(int z, int x, int y)
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
        public HttpResponseMessage LoadGeoTiff(int z, int x, int y)
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
        public HttpResponseMessage LoadNativeImage(int z, int x, int y)
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
        public HttpResponseMessage LoadNoaaRadar(int z, int x, int y)
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
        public HttpResponseMessage LoadNoaaWeatherStation(int z, int x, int y)
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
        public HttpResponseMessage LoadWms(int z, int x, int y)
        {
            WmsRasterLayer wmsLayer = new WmsRasterLayer();
            wmsLayer.Name = "wms";
            wmsLayer.Crs = "EPSG:900913";
            wmsLayer.Uri = new Uri("http://howdoiwms.thinkgeo.com/WmsServer.aspx");
            wmsLayer.ActiveLayerNames.Add("COUNTRIES02");
            wmsLayer.ActiveLayerNames.Add("USSTATES");
            wmsLayer.ActiveLayerNames.Add("USMAJORCITIES");
            wmsLayer.Parameters.Add("STYLES", "SIMPLE");

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(wmsLayer);

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Load CustomizedLayers.
        /// </summary>
        [Route("CustomizedLayers/customized/{z}/{x}/{y}")]
        [HttpGet]
        public HttpResponseMessage LoadCustomized(int z, int x, int y)
        {
            ThinkGeoHeadquartersFeatureLayer thinkGeoHeadquartersFeatureLayer = new ThinkGeoHeadquartersFeatureLayer();
            thinkGeoHeadquartersFeatureLayer.Name = "CustomizedLayer";

            TextStyle textStyle = new TextStyle("name", (new GeoFont("Arial", 12)), new GeoSolidBrush(GeoColor.StandardColors.DarkOliveGreen));
            textStyle.HaloPen = new GeoPen(GeoColor.StandardColors.FloralWhite, 5);
            textStyle.SplineType = SplineType.ForceSplining;

            thinkGeoHeadquartersFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush = new GeoSolidBrush(new GeoColor(50, GeoColor.SimpleColors.Orange));
            thinkGeoHeadquartersFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColor.SimpleColors.Black);
            thinkGeoHeadquartersFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = new LineStyle(new GeoPen(GeoColor.SimpleColors.Orange, 5));
            thinkGeoHeadquartersFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.SymbolPen = new GeoPen(GeoColor.FromArgb(255, GeoColor.StandardColors.Green), 8);
            thinkGeoHeadquartersFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = textStyle;
            thinkGeoHeadquartersFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(thinkGeoHeadquartersFeatureLayer);

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Initializes projection.
        /// </summary>
        private static void InitializeProjection()
        {
            wgs84ToGoogleProjection = new Proj4Projection();
            wgs84ToGoogleProjection.InternalProjectionParametersString = Proj4Projection.GetWgs84ParametersString();
            wgs84ToGoogleProjection.ExternalProjectionParametersString = Proj4Projection.GetGoogleMapParametersString();
            wgs84ToGoogleProjection.Open();

            string projWkt = File.ReadAllText(string.Format(@"{0}/MrSid/US380AndGeeRoad.prj", baseDirectory));
            feetToGoogleProjection = new Proj4Projection();
            feetToGoogleProjection.InternalProjectionParametersString = Proj4Projection.ConvertPrjToProj4(projWkt);
            feetToGoogleProjection.ExternalProjectionParametersString = Proj4Projection.GetGoogleMapParametersString();
            feetToGoogleProjection.Open();
        }

        /// <summary>
        /// Draw the map and return the image back to client in an HttpResponseMessage.
        /// </summary>
        private HttpResponseMessage DrawTileImage(LayerOverlay layerOverlay, int z, int x, int y)
        {
            using (Bitmap bitmap = new Bitmap(256, 256))
            {
                PlatformGeoCanvas geoCanvas = new PlatformGeoCanvas();
                RectangleShape boundingBox = WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, GeographyUnit.Meter);
                geoCanvas.BeginDrawing(bitmap, boundingBox, GeographyUnit.Meter);
                layerOverlay.Draw(geoCanvas);
                geoCanvas.EndDrawing();

                MemoryStream ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Png);

                HttpResponseMessage msg = new HttpResponseMessage(HttpStatusCode.OK);
                msg.Content = new ByteArrayContent(ms.ToArray());
                msg.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

                return msg;
            }
        }
    }
}
