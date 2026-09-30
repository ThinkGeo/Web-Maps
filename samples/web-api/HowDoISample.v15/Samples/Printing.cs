using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;
using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI.Samples
{
    /// <summary>
    /// Printing and Export: pages and pictures made on the server - a PDF with the map as the
    /// browser's picture or as vector paths, a GeoImage drawn with no map control at all -
    /// kept for a few minutes and fetched from export/{id}.
    /// </summary>
    public class Printing : ISampleGroup
    {
        // The papers a page can be, portrait, in points.
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
            // Two ways onto a page. Raster: the map exactly as the browser draws it, sent up as a
            // picture and placed on the page - pixels. Vector: the extent drawn again on the
            // server into a PdfGeoCanvas - the ThinkGeo Cloud vector tiles styled by
            // MvtTilesAsyncLayer, the zoning from its shapefile by the style.json the page sends,
            // the title and footer as text - so every road, parcel and label is a path or a glyph
            // that prints sharp at any size.
            app.MapPost("/samples/printing/pdf", async (HttpRequest request) =>
            {
                using var document = await JsonDocument.ParseAsync(request.Body);
                var body = document.RootElement;
                var kind = body.GetProperty("kind").GetString();
                var extent = GeoJson.Extent(body.GetProperty("bbox").GetString());
                var title = body.GetProperty("title").GetString();
                // The paper and the way it is turned, in points.
                var paper = Papers.TryGetValue(body.TryGetProperty("paper", out var asked) ? asked.GetString() ?? "" : "", out var size) ? size : Papers["AnsiA"];
                var landscape = !body.TryGetProperty("orientation", out var turned) || turned.GetString() != "Portrait";
                var (PageWidth, PageHeight) = landscape ? (paper.Height, paper.Width) : (paper.Width, paper.Height);
                try
                {
                    using var stream = new MemoryStream();
                    var canvas = new PdfGeoCanvas { PageWidth = PageWidth, PageHeight = PageHeight };
                    if (kind == "raster")
                    {
                        var picture = new GeoImage(Convert.FromBase64String(body.GetProperty("image").GetString()));
                        canvas.BeginDrawing(stream, new RectangleShape(0, PageHeight, PageWidth, 0), GeographyUnit.Meter);
                        var scale = Math.Min((PageWidth - 40) / picture.Width, (PageHeight - 90) / picture.Height);
                        var width = picture.Width * scale;
                        var height = picture.Height * scale;
                        canvas.DrawScreenImage(picture, PageWidth / 2, 54 + (height / 2), width, height, DrawingLevel.LevelOne, 0, 0, 0);
                        Frame(canvas, extent, title, PageWidth, PageHeight);
                        canvas.EndDrawing();
                    }
                    else
                    {
                        var basemap = await BasemapAsync();
                        var zoning = Zoning(body.GetProperty("layers").GetRawText());
                        zoning.Open();

                        // The page shows what the browser shows, widened or heightened to the page's shape.
                        var pageExtent = MapUtil.GetDrawingExtent(extent, PageWidth, PageHeight);
                        var pageScale = MapUtil.GetScale(GeographyUnit.Meter, pageExtent, PageWidth, PageHeight);
                        var features = await basemap.GetFeaturesInsideBoundingBoxAsync(pageExtent, pageScale, CancellationToken.None);

                        canvas.BeginDrawing(stream, pageExtent, GeographyUnit.Meter);
                        var labels = new Collection<SimpleCandidate>();
                        basemap.Draw(canvas, features, labels);
                        canvas.Flush();
                        zoning.Draw(canvas, labels);
                        canvas.Flush();
                        Frame(canvas, extent, title, PageWidth, PageHeight);
                        canvas.EndDrawing();
                        zoning.Close();
                        await basemap.CloseAsync();
                    }
                    var bytes = stream.ToArray();
                    return Results.Json(new { url = "export/" + ExportStore.Put(bytes, "application/pdf"), kilobytes = bytes.Length / 1024, kind, page = FormattableString.Invariant($"{PageWidth:0} by {PageHeight:0} points") });
                }
                catch (Exception exception)
                {
                    return Results.Problem(exception.Message, statusCode: 500);
                }
            });

            // No map control at all: the layers are drawn straight into a GeoImage on the server
            // with a GeoCanvas - the basemap's vector tiles underneath, Frisco's zoning on top by
            // the style.json the page sends - over the extent the browser asks for in the map's
            // own metres, or the zoning's own, at the width asked for; the height follows the extent.
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

        // A title band across the top and a footer across the bottom, over whatever the page holds.
        private static void Frame(GeoCanvas canvas, RectangleShape extent, string title, float PageWidth, float PageHeight)
        {
            Band(canvas, 0, 44, PageWidth);
            canvas.DrawTextWithScreenCoordinate(title, new GeoFont("Arial", 20, DrawingFontStyles.Bold), GeoBrushes.Black, PageWidth / 2, 24, DrawingLevel.LevelFour);
            Band(canvas, PageHeight - 26, PageHeight, PageWidth);
            var centre = ProjectionConverter.Convert(3857, 4326, extent.GetCenterPoint());
            var footer = FormattableString.Invariant($"Centre {centre.Y:0.0000}, {centre.X:0.0000}   -   {extent.Width / 1000:0.0} km across   -   {DateTime.Now:yyyy-MM-dd HH:mm}");
            canvas.DrawTextWithScreenCoordinate(footer, new GeoFont("Arial", 9), GeoBrushes.DimGray, PageWidth / 2, PageHeight - 12, DrawingLevel.LevelFour);
        }

        private static void Band(GeoCanvas canvas, float top, float bottom, float PageWidth)
        {
            var corners = new[] { new ScreenPointF(0, top), new ScreenPointF(PageWidth, top), new ScreenPointF(PageWidth, bottom), new ScreenPointF(0, bottom), new ScreenPointF(0, top) };
            canvas.DrawArea(new[] { corners }, null, new GeoSolidBrush(GeoColor.FromArgb(225, GeoColors.White)), DrawingLevel.LevelFour, 0, 0, PenBrushDrawingOrder.BrushFirst);
        }
    }
}
