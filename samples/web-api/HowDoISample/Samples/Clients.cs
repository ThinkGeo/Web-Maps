using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI.Samples
{
    /// <summary>
    /// Leaflet and OpenLayers: the server hands out PNG tiles, vector tiles and GeoJSON, and any
    /// map library that reads those can show them.
    /// </summary>
    public class Clients : ISampleGroup
    {
        public void Register(OverlayCatalog catalog)
        {
            // Frisco's zoning, trails, streets and schools drawn on the server; the parks go on top
            // as data the browser draws.
            catalog.Raster("clients-frisco", () =>
            {
                var overlay = new LayerOverlay();
                foreach (var layer in Styling.Migration.ClassicLayers().Where(layer => layer.Name != "parks"))
                {
                    overlay.Layers.Add(layer.Name, layer);
                }
                return overlay;
            });
        }

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            app.MapGet("/samples/clients/parks", () =>
            {
                var parks = SampleData.Frisco("Parks.shp");
                parks.Open();
                var features = parks.GetAllFeatures(new[] { "NAME" });
                parks.Close();
                return Results.Text(GeoJson.Collection(features), "application/json");
            });
        }
    }
}
