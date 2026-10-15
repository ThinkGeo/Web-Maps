using System.Collections.ObjectModel;
using System.Globalization;
using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI.Samples
{
    /// <summary>Extending the SDK: a feature source of your own, cut into tiles like any other.</summary>
    public class Extending : ISampleGroup
    {
        public void Register(OverlayCatalog catalog)
        {
            catalog.Vector("csv-route", () =>
            {
                var tiles = new FeatureSourceVectorTileSource();
                tiles.FeatureSources.Add("route", new SimpleCsvFeatureSource(SampleData.Path("Csv/vehicle-route.csv")) { ProjectionConverter = new ProjectionConverter(4326, 3857) });
                return new VectorTileOverlay(tiles);
            });
        }

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            app.MapGet("/samples/extending/csv-route", () =>
            {
                var route = new SimpleCsvFeatureSource(SampleData.Path("Csv/vehicle-route.csv")) { ProjectionConverter = new ProjectionConverter(4326, 3857) };
                route.Open();
                var bounds = route.GetBoundingBox();
                var count = route.GetCount();
                route.Close();
                bounds.ScaleUp(50);
                return Results.Json(new { count, bounds = GeoJson.Bounds(bounds) });
            });
        }

        /// <summary>
        /// A FeatureSource over a CSV of "latitude,longitude" lines. Reading them is the whole
        /// job; the base class does the rest - the projection converter, the bounding box, the
        /// tile cutting - so the map treats it like any shapefile.
        /// </summary>
        public class SimpleCsvFeatureSource : FeatureSource
        {
            private readonly string path;
            private Collection<Feature> features;

            public SimpleCsvFeatureSource(string csvPath)
            {
                path = csvPath;
            }

            protected override Collection<Feature> GetAllFeaturesCore(IEnumerable<string> returningColumnNames)
            {
                if (features != null) return features;

                features = new Collection<Feature>();
                foreach (var line in File.ReadAllLines(path))
                {
                    var parts = line.Split(',');
                    if (parts.Length < 2) continue;
                    features.Add(new Feature(double.Parse(parts[1], CultureInfo.InvariantCulture), double.Parse(parts[0], CultureInfo.InvariantCulture)));
                }
                return features;
            }
        }
    }
}
