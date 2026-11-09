using System.Text.Json;
using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI.Samples
{
    /// <summary>
    /// A style.json drawn on the server: the browser hands over the layers it draws, and the
    /// server draws the same document over the same data into raster tiles. One style model,
    /// two deliveries.
    /// </summary>
    public class Tiles : ISampleGroup
    {
        public void Register(OverlayCatalog catalog)
        {
        }

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            // The layers a page holds, and any image they name, over the feature sources of the
            // vector overlay named. The style is registered under a name made from its content,
            // so the same document asked for twice is the same tiles, and the reply is the address
            // the browser lays on its map: raster/{id}/{z}/{x}/{y}.png, drawn by CpuTileRenderer.
            app.MapPost("/samples/tiles/style", async (HttpRequest request, OverlayCatalog catalog) =>
            {
                using var body = await JsonDocument.ParseAsync(request.Body);
                var source = body.RootElement.GetProperty("source").GetString() ?? "";
                var layers = body.RootElement.GetProperty("layers").GetRawText();
                if (!catalog.HasVector(source)) return Results.NotFound();

                var images = new Dictionary<string, GeoImage>();
                if (body.RootElement.TryGetProperty("images", out var pictures) && pictures.ValueKind == JsonValueKind.Object)
                {
                    foreach (var picture in pictures.EnumerateObject())
                    {
                        var data = picture.Value.GetString() ?? "";
                        var comma = data.IndexOf(',');
                        images[picture.Name] = new GeoImage(Convert.FromBase64String(comma < 0 ? data : data.Substring(comma + 1)));
                    }
                }
                var id = StyledTiles.IdOf(source, layers, images.Keys);
                catalog.Styled(id, () => new StyledTiles(source, catalog.FreshSources(source), layers, images));
                return Results.Json(new { id });
            });
        }
    }
}
