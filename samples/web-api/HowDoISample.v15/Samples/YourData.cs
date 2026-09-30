using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI.Samples
{
    /// <summary>
    /// Your Data: the vector files, raster files, databases and tile archives the SDK reads.
    /// A vector source is cut into vector tiles as the browser asks for them; a raster layer is
    /// drawn tile by tile on the server; an archive is handed out as it holds its tiles.
    /// </summary>
    public class YourData : ISampleGroup
    {
        public static readonly string[] VectorFormats = { "Shapefile", "GeoJSON", "KML", "GPX", "MapInfo TAB", "TinyGeo", "WKB", "In memory" };
        public static readonly string[] RasterFormats = { "GeoTIFF - NAIP aerial", "Plain JPEG + world file", "MrSID - aerial in state plane feet", "JPEG2000 - world", "ECW - world imagery", "Anything else GDAL reads" };
        public static readonly string[] Databases = { "SQLite", "GeoPackage", "Esri File Geodatabase", "PostgreSQL / PostGIS", "SQL Server" };
        public static readonly string[] Archives = { "pmtiles", "mbtiles" };

        public void Register(OverlayCatalog catalog)
        {
            foreach (var format in VectorFormats)
            {
                var name = format;
                catalog.Vector("vector-" + Slug(name), () =>
                {
                    var source = OpenVector(name);
                    // The cutter reads a source from several threads at once, and the source says
                    // through its ConcurrentAccess whether it can be: features built in memory are
                    // read as they are, a file is copied once per thread.
                    var overlay = new VectorTileOverlay();
                    overlay.FeatureSources.Add("features", source);
                    return overlay;
                });
            }

            foreach (var format in RasterFormats)
            {
                var name = format;
                catalog.Raster("raster-" + Slug(name), () =>
                {
                    var overlay = new LayerOverlay();
                    overlay.Layers.Add(OpenRaster(name));
                    return overlay;
                });
            }

            foreach (var database in Databases)
            {
                var name = database;
                catalog.Vector("db-" + Slug(name), () =>
                {
                    var overlay = new VectorTileOverlay();
                    overlay.FeatureSources.Add("data", OpenDatabase(name));
                    return overlay;
                });
            }

            foreach (var archive in Archives)
            {
                var name = archive;
                catalog.Vector("offline-" + name, () => new VectorTileOverlay { TileSource = OpenArchive(name) });
            }
        }

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            // Where an overlay's data is, for the page to zoom to when it is chosen. A fresh
            // source or layer is opened for the question, so the served one is left to its tiles.
            app.MapGet("/samples/your-data/bounds/{overlay}", (string overlay) =>
            {
                try
                {
                    return Results.Json(GeoJson.Bounds(Bounds(overlay)));
                }
                catch (Exception exception)
                {
                    while (exception.InnerException != null) exception = exception.InnerException;
                    return Results.Problem(exception.Message.Split('\n')[0], statusCode: 503);
                }
            });

            // An archive's own style.json and a bare one made from the layers its metadata
            // lists, each pointing at the overlay that serves the archive's tiles.
            app.MapGet("/samples/your-data/offline/{archive}", async (string archive) =>
            {
                var tiles = OpenArchive(archive);
                await tiles.OpenAsync();
                var bounds = archive == "mbtiles" ? await ((MbTilesVectorTileSource)tiles).GetBoundingBoxAsync() : ((PmTilesVectorTileSource)tiles).GetBoundingBox();
                var metadata = archive == "mbtiles" ? ((MbTilesVectorTileSource)tiles).MetadataJson : ((PmTilesVectorTileSource)tiles).MetadataJson;
                var own = await File.ReadAllTextAsync(SampleData.Path(archive == "mbtiles" ? "Mbtiles/style.json" : "Pmtiles/style.json"));
                var result = new
                {
                    kind = tiles.GetType().Name,
                    minzoom = tiles.MinDataZoom,
                    maxzoom = tiles.MaxDataZoom,
                    bounds = GeoJson.Bounds(bounds),
                    tiles = "tiles/offline-" + archive + "/{z}/{x}/{y}.mvt",
                    style = own,
                    bare = BareStyle(metadata),
                };
                ((IDisposable)tiles).Dispose();
                return Results.Json(result);
            });
        }

        // Eight formats. Every source hands out Web Mercator meters: the ones stored otherwise
        // carry a ProjectionConverter.
        private static FeatureSource OpenVector(string format)
        {
            switch (format)
            {
                case "Shapefile": return SampleData.Frisco("Hike_Bike.shp");
                case "GeoJSON": return new GeoJsonFeatureSource(SampleData.Path("GeoJson/pittsburghpacity-designated-historic-districts.geojson")) { ProjectionConverter = new ProjectionConverter(4326, 3857) };
                case "KML": return new KmlGdalFeatureSource(SampleData.Path("Kml/Frisco.kml"));   // written in the map's own meters
                case "GPX": return new GpxFeatureSource(SampleData.Path("Gpx/Hike_Bike.gpx")) { ProjectionConverter = new ProjectionConverter(4326, 3857) };
                case "MapInfo TAB": return new TabFeatureSource(SampleData.Path("Tab/City_ETJ.tab")) { ProjectionConverter = new ProjectionConverter(2276, 3857) };
                case "TinyGeo": return new TinyGeoFeatureSource(SampleData.Path("TinyGeo/Zoning.tgeo")) { ProjectionConverter = new ProjectionConverter(2276, 3857) };
                case "WKB": return new WkbFileFeatureSource(SampleData.Path("Wkb/USStates.wkb"));   // the US states, written in the map's own meters
                default: return MosquitoTraps();
            }
        }

        // An in-memory source filled from a CSV: a count per mosquito trap, at x, y in the map's meters.
        private static InMemoryFeatureSource MosquitoTraps()
        {
            var features = new List<Feature>();
            foreach (var line in File.ReadLines(SampleData.Path("Csv/Frisco_Mosquitos.csv")))
            {
                var parts = line.Split(',');
                if (parts.Length < 3) continue;
                var x = double.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture);
                var y = double.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture);
                features.Add(new Feature(new PointShape(x, y), new Dictionary<string, string> { ["Count"] = parts[2] }));
            }
            return new InMemoryFeatureSource(new[] { new FeatureSourceColumn("Count") }, features);
        }

        // Each format has a raster layer named after it, and anything else GDAL reads goes through
        // the base class. A file in another projection gets a GdalProjectionConverter and is
        // warped as it is drawn.
        private static RasterLayer OpenRaster(string name)
        {
            switch (name)
            {
                case "Plain JPEG + world file": return new GdalRasterLayer(SampleData.Path("Jpg/m_3309650_sw_14_1_20160911_20161121.jpg"));
                // The MrSID's own projection is read from its .prj; the converter warps every tile of it into the map's meters.
                case "MrSID - aerial in state plane feet": return new MrSidGdalRasterLayer(SampleData.Path("MrSid/US380AndGeeRoad.sid")) { ImageSource = { ProjectionConverter = new GdalProjectionConverter(Projection.ConvertWktToProjString(File.ReadAllText(SampleData.Path("MrSid/US380AndGeeRoad.prj"))), Projection.GetGoogleMapProjString()) } };
                case "JPEG2000 - world": return new Jpeg2000GdalRasterLayer(SampleData.Path("Jpeg2000/World.jp2"), SampleData.Path("Jpeg2000/World.j2w"));   // placed by its world file, already in the map's meters
                case "ECW - world imagery": return new EcwGdalRasterLayer(SampleData.Path("World.ecw")) { ImageSource = { ProjectionConverter = new GdalProjectionConverter(4326, 3857) } };
                case "Anything else GDAL reads": return new GdalRasterLayer(SampleData.Path("GeoTiff/World.tif")) { ImageSource = { ProjectionConverter = new GdalProjectionConverter(4326, 3857) } };
                default: return new GeoTiffRasterLayer(SampleData.Path("GeoTiff/m_3309650_sw_14_1_20160911_20161121.tif"));
            }
        }

        // The first three are databases carried as a file, the last two are servers connected to.
        // The connection is the only line that differs.
        private static FeatureSource OpenDatabase(string name)
        {
            switch (name)
            {
                case "GeoPackage": return new GdalFeatureSource(SampleData.Path("GeoPackage/mora_surficial_geology.gpkg")) { ProjectionConverter = new ProjectionConverter(26910, 3857) };
                case "Esri File Geodatabase": return new FileGeoDatabaseFeatureSource(SampleData.Path("FileGeoDatabase/zoning.gdb"), "zoning") { ProjectionConverter = new ProjectionConverter(2276, 3857) };
                case "PostgreSQL / PostGIS": return new PostgreSqlFeatureSource("User ID=ThinkGeoTest;Password=ThinkGeoTestPassword;Host=demodb.thinkgeo.com;Port=5432;Database=postgres;Pooling=true;", "countries", "gid", 4326) { ProjectionConverter = new ProjectionConverter(4326, 3857) };
                case "SQL Server": return new SqlServerFeatureSource("Server=demodb.thinkgeo.com;Database=thinkgeo;User Id=ThinkGeoTest;Password=ThinkGeoTestPassword;TrustServerCertificate=True;", "frisco_coyote_sightings", "id") { ProjectionConverter = new ProjectionConverter(2276, 3857) };
                default: return new SqliteFeatureSource("Data Source=" + SampleData.Path("SQLite/frisco-restaurants.sqlite") + ";", "restaurants", "id", "geometry") { ProjectionConverter = new ProjectionConverter(2276, 3857) };
            }
        }

        // Vector tiles kept in a file on the server: an MBTiles or a PMTiles archive.
        private static IVectorTileSource OpenArchive(string archive)
        {
            return archive == "mbtiles"
                ? new MbTilesVectorTileSource(SampleData.Path("Mbtiles/maplibre.mbtiles"))
                : new PmTilesVectorTileSource(SampleData.Path("Pmtiles/frisco.pmtiles"));
        }

        private static RectangleShape Bounds(string overlay)
        {
            var dash = overlay.IndexOf('-');
            var kind = overlay.Substring(0, dash);
            var slug = overlay.Substring(dash + 1);
            RectangleShape bounds;
            if (kind == "raster")
            {
                var layer = OpenRaster(RasterFormats.First(candidate => Slug(candidate) == slug));
                layer.Open();
                bounds = layer.GetBoundingBox();
                layer.Close();
            }
            else
            {
                var source = kind == "db" ? OpenDatabase(Databases.First(candidate => Slug(candidate) == slug)) : OpenVector(VectorFormats.First(candidate => Slug(candidate) == slug));
                source.Open();
                // GDAL clamps a KML's declared extent to +-180 degrees; the features say where they are.
                bounds = slug == "kml" ? MapUtil.GetBoundingBoxOfItems(source.GetAllFeatures(ReturningColumnsType.NoColumns)) : source.GetBoundingBox();
                source.Close();
                bounds.ScaleUp(20);
            }
            return bounds;
        }

        // One fill, line and circle layer per layer the archive's metadata declares.
        private static string BareStyle(string metadata)
        {
            var layers = new StringBuilder(@"{ ""version"": 8, ""sources"": {}, ""layers"": [
    { ""id"": ""background"", ""type"": ""background"", ""paint"": { ""background-color"": ""#F0F0EC"" } }");
            using (var document = JsonDocument.Parse(metadata))
            {
                foreach (var declared in document.RootElement.GetProperty("vector_layers").EnumerateArray())
                {
                    var name = declared.GetProperty("id").GetString();
                    layers.Append(@",
    { ""id"": ""LAYER-fill"", ""type"": ""fill"", ""source"": ""archive"", ""source-layer"": ""LAYER"", ""filter"": [""=="", [""geometry-type""], ""Polygon""],
      ""paint"": { ""fill-color"": ""rgba(90,140,200,0.35)"", ""fill-outline-color"": ""#2A4A7A"" } },
    { ""id"": ""LAYER-line"", ""type"": ""line"", ""source"": ""archive"", ""source-layer"": ""LAYER"", ""filter"": [""=="", [""geometry-type""], ""LineString""],
      ""paint"": { ""line-color"": ""#2A4A7A"", ""line-width"": 1 } },
    { ""id"": ""LAYER-point"", ""type"": ""circle"", ""source"": ""archive"", ""source-layer"": ""LAYER"", ""filter"": [""=="", [""geometry-type""], ""Point""],
      ""paint"": { ""circle-radius"": 4, ""circle-color"": ""#C0392B"", ""circle-stroke-color"": ""#FFFFFF"", ""circle-stroke-width"": 1.5 } }".Replace("LAYER", name));
                }
            }
            return layers.Append(" ] }").ToString();
        }

        /// <summary>A name as it appears in a route: lower case, words joined by dashes.</summary>
        public static string Slug(string name) => Regex.Replace(name.ToLowerInvariant(), "[^a-z0-9]+", "-").Trim('-');
    }
}
