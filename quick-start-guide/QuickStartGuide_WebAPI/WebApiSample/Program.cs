using ThinkGeo.Core;
using ThinkGeo.UI.WebApi;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();

// Read decimal degrees and reproject to Web Mercator for vector tiles.
var capitals = new FeatureSourceVectorTileSource();
capitals.FeatureSources.Add(new ShapeFileFeatureSource(
    Path.Combine(AppContext.BaseDirectory, "Data", "WorldCapitals.shp"))
{
    ProjectionConverter = new ProjectionConverter(4326, 3857)
});
var capitalsOverlay = new VectorTileOverlay("capitals", capitals);

app.MapGet("/tiles/capitals/{z:int}/{x:int}/{y:int}", async (
    int z, int x, int y, HttpResponse response, CancellationToken cancellationToken) =>
{
    var tile = await capitalsOverlay.GetTileAsync(z, x, y, cancellationToken);
    if (tile == null) return Results.NoContent();
    response.Headers.ContentEncoding = "gzip";
    return Results.Bytes(tile, "application/vnd.mapbox-vector-tile");
});
app.Run();
