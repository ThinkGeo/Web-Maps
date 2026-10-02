using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI.Samples
{
    /// <summary>
    /// The smallest map this Web API serves: one shapefile, read on the server and cut into
    /// vector tiles as the browser asks for them, drawn by the browser with one style layer.
    /// </summary>
    public class GettingStarted : ISampleGroup
    {
        public void Register(OverlayCatalog catalog)
        {
            // Frisco's parks, stored in Texas state plane feet and handed out in the map's meters.
            catalog.Vector("getting-started", () =>
            {
                var overlay = new VectorTileOverlay();
                overlay.FeatureSources.Add("parks", SampleData.Frisco("Parks.shp"));
                return overlay;
            });
        }

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            // Where the parks are: the map starts there, and can be kept there.
            app.MapGet("/samples/getting-started/bounds", () =>
            {
                var parks = SampleData.Frisco("Parks.shp");
                parks.Open();
                var bounds = parks.GetBoundingBox();
                parks.Close();
                bounds.ScaleUp(10);
                return Results.Json(GeoJson.Bounds(bounds));
            });
        }
    }
}
