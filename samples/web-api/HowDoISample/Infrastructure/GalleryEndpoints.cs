using System.Text.Json;
using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI
{
    /// <summary>
    /// The routes every sample shares: the tiles of a vector overlay, the tiles of a raster
    /// overlay, the image of an adornment overlay, a file a sample made, and the sources the
    /// code pane shows.
    /// </summary>
    public static class GalleryEndpoints
    {
        public static void MapGalleryEndpoints(this WebApplication app)
        {
            // A vector tile, cut on the server or read from an archive, gzipped as the cutter
            // hands it out; the browser unpacks it. Nothing there is 204, which the map treats as
            // an empty tile.
            app.MapGet("/tiles/{overlay}/{z:int}/{x:int}/{y:int}.mvt", async (string overlay, int z, int x, int y, OverlayCatalog catalog, HttpResponse response, CancellationToken cancellation) =>
            {
                var vector = catalog.GetVector(overlay);
                if (vector == null) return Results.NotFound();
                if (z > vector.MaxDataZoom) return Results.NoContent();

                var bytes = await vector.GetTileAsync(z, x, y, cancellation);
                if (bytes == null) return Results.NoContent();

                response.Headers.ContentEncoding = "gzip";
                response.Headers.CacheControl = "public, max-age=300";
                return Results.Bytes(bytes, "application/vnd.mapbox-vector-tile");
            });

            // A raster tile drawn by classic layers, or by CpuTileRenderer from a style.json a page
            // handed over. Neither is drawn from two requests at once, so one overlay draws one
            // tile at a time.
            app.MapGet("/raster/{overlay}/{z:int}/{x:int}/{y:int}.png", async (string overlay, int z, int x, int y, OverlayCatalog catalog, HttpResponse response, CancellationToken cancellation) =>
            {
                var styled = catalog.GetStyled(overlay);
                var raster = styled == null ? catalog.GetRaster(overlay) : null;
                if (styled == null && raster == null) return Results.NotFound();

                var gate = catalog.LockFor(overlay);
                await gate.WaitAsync(cancellation);
                try
                {
                    response.Headers.CacheControl = "public, max-age=300";
                    var bytes = styled != null ? await styled.RenderTileAsync(z, x, y, cancellation) : await raster.GetTileImageAsync(z, x, y, cancellation);
                    return Results.Bytes(bytes, "image/png");
                }
                finally
                {
                    gate.Release();
                }
            });

            // The adornments drawn for the map's size and extent, asked for again whenever the
            // view settles.
            app.MapGet("/adornments/{overlay}", async (string overlay, int width, int height, string bbox, OverlayCatalog catalog, CancellationToken cancellation) =>
            {
                var adornment = catalog.GetAdornment(overlay);
                if (adornment == null) return Results.NotFound();
                if (width <= 0 || height <= 0 || string.IsNullOrEmpty(bbox)) return Results.BadRequest();

                var gate = catalog.LockFor(overlay);
                await gate.WaitAsync(cancellation);
                try
                {
                    return Results.Bytes(await adornment.GetImageAsync(width, height, GeoJson.Extent(bbox), cancellation), "image/png");
                }
                finally
                {
                    gate.Release();
                }
            });

            // A file a sample made - a PDF, a picture - fetched by the token the sample got.
            app.MapGet("/export/{id}", (string id) =>
                ExportStore.TryGet(id, out var bytes, out var contentType) ? Results.Bytes(bytes, contentType) : Results.NotFound());

            // The code pane: the sample's page as written, and the server source it runs.
            app.MapGet("/source/{sample}", async (string sample, IWebHostEnvironment environment) =>
            {
                var menu = await MenuOf(sample, environment);
                if (menu == null) return Results.NotFound();

                var pagePath = Path.Combine(environment.WebRootPath, "samples", sample + ".html");
                var serverPath = Path.Combine(environment.ContentRootPath, "Samples", menu.Value.Source + ".cs");
                return Results.Json(new
                {
                    client = File.Exists(pagePath) ? await File.ReadAllTextAsync(pagePath) : "",
                    server = File.Exists(serverPath) ? await File.ReadAllTextAsync(serverPath) : "",
                    serverFile = menu.Value.Source + ".cs",
                });
            });
        }

        private static async Task<(string Id, string Source)?> MenuOf(string sample, IWebHostEnvironment environment)
        {
            var menusPath = Path.Combine(environment.WebRootPath, "menus.json");
            using var document = JsonDocument.Parse(await File.ReadAllTextAsync(menusPath));
            foreach (var entry in document.RootElement.EnumerateArray())
            {
                if (string.Equals(entry.GetProperty("id").GetString(), sample, StringComparison.OrdinalIgnoreCase))
                {
                    return (sample, entry.TryGetProperty("source", out var source) ? source.GetString() : "");
                }
            }
            return null;
        }
    }
}
