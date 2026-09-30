using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;
using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI.Samples
{
    /// <summary>
    /// The map itself as a file, with no page furniture on it: a picture of any size, or a PDF
    /// of vector paths and glyphs. Both are drawn on the server with no map control at all and
    /// kept for a few minutes at export/{id}. A sheet with a title and a legend on it is
    /// <see cref="PrinterLayout"/>.
    /// </summary>
    public class Printing : ISampleGroup
    {
        // The papers a PDF page can be, portrait, in points.
        private static readonly Dictionary<string, (float Width, float Height)> Papers = new Dictionary<string, (float, float)>(StringComparer.OrdinalIgnoreCase)
        {
            ["AnsiA"] = (612, 792),
            ["AnsiB"] = (792, 1224),
            ["A4"] = (595, 842),
            ["A3"] = (842, 1191),
        };

        public void Register(OverlayCatalog catalog)
        {
        }

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            // The view drawn onto a PDF page as vector: the basemap's tiles styled by
            // MvtTilesAsyncLayer and the zoning by the style.json the page sends, both onto a
            // PdfGeoCanvas, so every road, parcel and label is a path or a glyph. The map fills
            // the page; nothing else is on it.
            app.MapPost("/samples/printing/pdf", async (HttpRequest request) =>
            {
                using var document = await JsonDocument.ParseAsync(request.Body);
                var body = document.RootElement;
                var extent = GeoJson.Extent(body.GetProperty("bbox").GetString());
                var paper = Papers.TryGetValue(body.TryGetProperty("paper", out var asked) ? asked.GetString() ?? "" : "", out var size) ? size : Papers["AnsiA"];
                var landscape = !body.TryGetProperty("orientation", out var turned) || turned.GetString() != "Portrait";
                var (pageWidth, pageHeight) = landscape ? (paper.Height, paper.Width) : (paper.Width, paper.Height);
                try
                {
                    var basemap = await BasemapAsync();
                    var zoning = Zoning(body.GetProperty("layers").GetRawText());
                    zoning.Open();

                    // The page shows what the browser shows, widened or heightened to the page's shape.
                    var pageExtent = MapUtil.GetDrawingExtent(extent, pageWidth, pageHeight);
                    var pageScale = MapUtil.GetScale(GeographyUnit.Meter, pageExtent, pageWidth, pageHeight);
                    var features = await basemap.GetFeaturesInsideBoundingBoxAsync(pageExtent, pageScale, CancellationToken.None);

                    using var stream = new MemoryStream();
                    var canvas = new PdfGeoCanvas { PageWidth = pageWidth, PageHeight = pageHeight };
                    canvas.BeginDrawing(stream, pageExtent, GeographyUnit.Meter);
                    var labels = new Collection<SimpleCandidate>();
                    basemap.Draw(canvas, features, labels);
                    canvas.Flush();
                    zoning.Draw(canvas, labels);
                    canvas.EndDrawing();
                    zoning.Close();
                    await basemap.CloseAsync();

                    var bytes = stream.ToArray();
                    return Results.Json(new
                    {
                        url = "export/" + ExportStore.Put(bytes, "application/pdf"),
                        kilobytes = bytes.Length / 1024,
                        page = FormattableString.Invariant($"{pageWidth:0} by {pageHeight:0} points"),
                    });
                }
                catch (Exception exception)
                {
                    return Results.Problem(exception.Message, statusCode: 500);
                }
            });

            // The same view drawn into a GeoImage instead, at the width asked for; the height
            // follows the extent. No map control, no browser: this is the call a scheduled job
            // or a report generator makes.
            app.MapPost("/samples/printing/image", async (HttpRequest request) =>
            {
                using var document = await JsonDocument.ParseAsync(request.Body);
                var body = document.RootElement;
                var size = Math.Clamp(body.TryGetProperty("size", out var asked) ? asked.GetInt32() : 800, 200, 4000);
                var bbox = body.TryGetProperty("bbox", out var box) ? box.GetString() : null;
                try
                {
                    var zoning = Zoning(body.GetProperty("layers").GetRawText());
                    zoning.Open();

                    var basemap = await BasemapAsync();
                    var extent = zoning.GetBoundingBox();
                    var parts = (bbox ?? "").Split(',');
                    if (parts.Length == 4 && parts.All(part => double.TryParse(part, NumberStyles.Float, CultureInfo.InvariantCulture, out _)))
                    {
                        var v = parts.Select(part => double.Parse(part, CultureInfo.InvariantCulture)).ToArray();
                        extent = new RectangleShape(v[0], v[3], v[2], v[1]);
                    }
                    var width = size;
                    var height = Math.Clamp((int)Math.Round(size * extent.Height / extent.Width), 1, 4000);
                    using var picture = new GeoImage(width, height);
                    var drawingExtent = MapUtil.GetDrawingExtent(extent, width, height);
                    var features = await basemap.GetFeaturesInsideBoundingBoxAsync(drawingExtent, MapUtil.GetScale(GeographyUnit.Meter, drawingExtent, width, height), CancellationToken.None);
                    var canvas = GeoCanvas.CreateDefaultGeoCanvas();
                    canvas.BeginDrawing(picture, drawingExtent, GeographyUnit.Meter);
                    var labels = new Collection<SimpleCandidate>();
                    basemap.Draw(canvas, features, labels);
                    canvas.Flush();
                    zoning.Draw(canvas, labels);
                    canvas.EndDrawing();
                    zoning.Close();
                    await basemap.CloseAsync();
                    return Results.Json(new { url = "export/" + ExportStore.Put(picture.GetImageBytes(GeoImageFormat.Png), "image/png"), width, height });
                }
                catch (Exception exception)
                {
                    return Results.Problem(exception.Message, statusCode: 500);
                }
            });
        }

        // ThinkGeo Cloud's vector tiles styled by MvtTilesAsyncLayer; the style asks for its tiles
        // with a {key} placeholder in their address, which the key fills.
        private static async Task<MvtTilesAsyncLayer> BasemapAsync()
        {
            var basemap = new MvtTilesAsyncLayer(ThinkGeoVectorStyles.Light) { TimeoutInSeconds = 60 };
            basemap.SendingHttpRequest += (_, args) =>
            {
                var address = args.HttpRequestMessage.RequestUri.ToString().Replace("{key}", GlobalSettings.ThinkGeoApiKey).Replace("%7Bkey%7D", GlobalSettings.ThinkGeoApiKey);
                args.HttpRequestMessage.RequestUri = new Uri(address);
            };
            await basemap.OpenAsync();
            return basemap;
        }

        // Frisco's zoning, read from its shapefile in Texas state plane feet and drawn in the map's
        // meters by the style.json given: the same document the browser draws it with.
        private static StyledLayer Zoning(string layers) =>
            new StyledLayer(layers, new[] { new KeyValuePair<string, FeatureSource>("zoning", SampleData.Frisco("Zoning.shp")) });
    }
}
