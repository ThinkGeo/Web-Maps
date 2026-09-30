using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;
using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI.Samples
{
    /// <summary>
    /// The map as a picture of any size, drawn on the server with no map control at all and
    /// kept for a few minutes at export/{id}. This is the call a scheduled job or a report
    /// generator makes. A page to print, with a title block and a legend on it, is
    /// <see cref="PrinterLayout"/>.
    /// </summary>
    public class Printing : ISampleGroup
    {
        public void Register(OverlayCatalog catalog)
        {
        }

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            // The view drawn into a GeoImage at the width asked for; the height follows the
            // extent. The basemap's vector tiles underneath, the zoning on top by the style.json
            // the page sends.
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
