using System.Diagnostics;
using System.Globalization;
using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI.Samples
{
    /// <summary>
    /// Cache tiles: the same layers served as raster tiles drawn on the server or as vector tiles
    /// cut on the server, each with or without a cache on the server's disk, and with or without
    /// letting the browser keep what it was sent.
    /// </summary>
    public class Caching : ISampleGroup
    {
        private static readonly string Folder = Path.Combine(Path.GetTempPath(), "howdoi-tile-cache");
        private static readonly FileRasterTileCache RasterCache = new FileRasterTileCache(Path.Combine(Folder, "raster"), "frisco");
        private static readonly FileTileCache VectorCache = new FileTileCache(Path.Combine(Folder, "vector"));

        public void Register(OverlayCatalog catalog)
        {
            catalog.Raster("cache-raster-on", () => Raster(RasterCache));
            catalog.Raster("cache-raster-off", () => Raster(null));
            catalog.Vector("cache-vector-on", () => Vector(VectorCache));
            catalog.Vector("cache-vector-off", () => Vector(null));
        }

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            // The Server-Timing header says whether the server's cache had the tile and how long
            // the server took; Cache-Control says whether the browser may keep it.
            app.MapGet("/samples/caching/{kind}/{serverCache:bool}/{browserCache:bool}/{z:int}/{x:int}/{y:int}", async (string kind, bool serverCache, bool browserCache, int z, int x, int y, OverlayCatalog catalog, HttpResponse response, CancellationToken cancellation) =>
            {
                var id = $"cache-{kind}-{(serverCache ? "on" : "off")}";
                var watch = Stopwatch.StartNew();
                bool hit;
                byte[] bytes;
                if (kind == "raster")
                {
                    var gate = catalog.LockFor(id);
                    await gate.WaitAsync(cancellation);
                    try
                    {
                        hit = serverCache && RasterCache.GetTile(z, x, y) is RasterTile tile && tile.Content?.Length > 0;
                        watch.Restart();
                        bytes = await catalog.GetRaster(id).GetTileImageAsync(z, x, y, cancellation);
                    }
                    finally
                    {
                        gate.Release();
                    }
                }
                else
                {
                    var overlay = catalog.GetVector(id);
                    if (z > overlay.MaxDataZoom) return Results.NoContent();
                    hit = serverCache && (await VectorCache.GetTileAsync(z, x, y, "frisco", cancellation))?.Content?.Length > 0;
                    watch.Restart();
                    bytes = await overlay.GetTileAsync(z, x, y, cancellation);
                    if (bytes == null) return Results.NoContent();
                    response.Headers.ContentEncoding = "gzip";
                }

                response.Headers.CacheControl = browserCache ? "public, max-age=300" : "no-store";
                response.Headers["Server-Timing"] = $"tile;desc={(hit ? "cache" : "made")};dur={watch.Elapsed.TotalMilliseconds.ToString("0.0", CultureInfo.InvariantCulture)}";
                return Results.Bytes(bytes, kind == "raster" ? "image/png" : "application/vnd.mapbox-vector-tile");
            });

            app.MapPost("/samples/caching/clear", () =>
            {
                RasterCache.ClearCache();
                VectorCache.ClearCache();
                return Results.Ok();
            });
        }

        private static LayerOverlay Raster(FileRasterTileCache cache)
        {
            var overlay = new LayerOverlay { TileCache = cache };
            foreach (var layer in Styling.Migration.ClassicLayers())
            {
                overlay.Layers.Add(layer.Name, layer);
            }
            return overlay;
        }

        // The cutter keeps recent tiles in memory too; that is turned off so the disk cache is the
        // only cache the server has.
        private static VectorTileOverlay Vector(FileTileCache cache)
        {
            var tiles = new FeatureSourceVectorTileSource { SourceId = "frisco", VectorTileCache = cache, MaxCachedCutTiles = 0 };
            foreach (var layer in Styling.Migration.ClassicLayers())
            {
                tiles.FeatureSources.Add(layer.Name, layer.FeatureSource);
            }
            return new VectorTileOverlay(tiles);
        }
    }
}
