using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI.Samples
{
    /// <summary>
    /// Fields interpolated and a century of population - computed on the server, drawn in
    /// the browser.
    /// </summary>
    public class Dynamic : ISampleGroup
    {
        private const int FieldResolution = 120;
        private const int MaxDrawnDataPoints = 2000;
        private const int FirstYear = 1900;
        private const int LastYear = 2024;

        private static readonly Lazy<Dictionary<PointShape, double>> Mosquitos = new Lazy<Dictionary<PointShape, double>>(() =>
        {
            var data = new Dictionary<PointShape, double>();
            foreach (var line in File.ReadLines(SampleData.Path("Csv/Frisco_Mosquitos.csv")))
            {
                var parts = line.Split(',');
                if (parts.Length < 3) continue;
                data[new PointShape(double.Parse(parts[0], CultureInfo.InvariantCulture), double.Parse(parts[1], CultureInfo.InvariantCulture))] = double.Parse(parts[2], CultureInfo.InvariantCulture);
            }
            return data;
        });

        public void Register(OverlayCatalog catalog)
        {
            catalog.Vector("us-states", () =>
            {
                var tiles = new FeatureSourceVectorTileSource();
                tiles.FeatureSources.Add("states", new ShapeFileFeatureSource(SampleData.Path("Shapefile/USStates_3857.shp")));
                return new VectorTileOverlay(tiles);
            });

            // A point at the middle of each state, with one column per year of population.
            catalog.Vector("population", () =>
            {
                var tiles = new FeatureSourceVectorTileSource();
                tiles.FeatureSources.Add("population", Population());
                return new VectorTileOverlay(tiles);
            });
        }

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            // The field is interpolated on the server - inverse distance weighting over a grid
            // across the data - and traced into isoline features there; the browser draws the
            // features, and reads their level for the colour, the width and the label.
            app.MapGet("/samples/dynamic/isolines", (int points, int levelCount, bool fillBands, double power) =>
            {
                var seed = Mosquitos.Value;
                var data = points == 0 ? seed : Synthetic(seed, points);
                var dataBounds = new RectangleShape(seed.Keys.Min(p => p.X), seed.Keys.Max(p => p.Y), seed.Keys.Max(p => p.X), seed.Keys.Min(p => p.Y));
                var levels = IsoLineLayer.GetIsoLineLevels(data, Math.Clamp(levelCount, 2, 60)).Distinct().OrderBy(level => level).ToList();
                var colors = GeoColor.GetColorsInQualityFamily(GeoColors.Blue, GeoColors.Red, levels.Count, ColorWheelDirection.Clockwise).ToList();
                var type = fillBands ? IsoLineType.ClosedLinesAsPolygons : IsoLineType.LinesOnly;
                // Closing the lines into band polygons costs far more than tracing them; the bands
                // read a coarser grid.
                var resolution = fillBands ? FieldResolution / 2 : FieldResolution;
                var extent = (RectangleShape)dataBounds.CloneDeep();
                extent.ScaleUp(20);

                var watch = Stopwatch.StartNew();
                var grid = GridFeatureSource.GenerateGridMatrix(new GridDefinition(extent, extent.Width / resolution, -9999, data), new InverseDistanceWeightedGridInterpolationModel(Math.Clamp(power, 1, 4), double.MaxValue));
                var features = IsoLineLayer.GetIsoFeatures(grid, levels, "level", type, -9999);
                watch.Stop();

                // Every second level carries its value as a label.
                foreach (var feature in features)
                {
                    var level = double.Parse(feature.ColumnValues["level"], CultureInfo.InvariantCulture);
                    var index = levels.FindIndex(candidate => Math.Abs(candidate - level) < 1e-9);
                    feature.ColumnValues["level"] = level.ToString(CultureInfo.InvariantCulture);
                    feature.ColumnValues["label"] = index % 2 == 0 ? level.ToString("0.#", CultureInfo.InvariantCulture) : "";
                }
                return Results.Json(new
                {
                    isolines = JsonDocument.Parse(GeoJson.Collection(features)).RootElement,
                    points = JsonDocument.Parse(GeoJson.Collection(data.Keys.Take(MaxDrawnDataPoints).Cast<BaseShape>())).RootElement,
                    levels,
                    colors = colors.Select(color => $"#{color.R:X2}{color.G:X2}{color.B:X2}"),
                    count = data.Count,
                    featureCount = features.Count,
                    resolution,
                    ms = watch.ElapsedMilliseconds,
                    bounds = GeoJson.Bounds(dataBounds),
                });
            });
        }

        // Synthetic points: a smooth field drawn from the real ones, plus noise.
        private static Dictionary<PointShape, double> Synthetic(Dictionary<PointShape, double> seedPoints, int count)
        {
            var random = new Random(42);
            var seeds = seedPoints.ToList();
            var bounds = new RectangleShape(seedPoints.Keys.Min(p => p.X), seedPoints.Keys.Max(p => p.Y), seedPoints.Keys.Max(p => p.X), seedPoints.Keys.Min(p => p.Y));
            var synthetic = new Dictionary<PointShape, double>(count);
            while (synthetic.Count < count)
            {
                var x = bounds.MinX + random.NextDouble() * bounds.Width;
                var y = bounds.MinY + random.NextDouble() * bounds.Height;
                double top = 0, bottom = 0;
                foreach (var seed in seeds)
                {
                    var dx = x - seed.Key.X;
                    var dy = y - seed.Key.Y;
                    var weight = 1.0 / (dx * dx + dy * dy + 1.0);
                    top += weight * seed.Value;
                    bottom += weight;
                }
                synthetic[new PointShape(x, y)] = top / bottom + (random.NextDouble() - 0.5) * 12.0;
            }
            return synthetic;
        }

        private static FeatureSource Population()
        {
            var byStateAndYear = new Dictionary<(string, int), string>();
            foreach (var line in File.ReadLines(SampleData.Path("Csv/historical_state_population_by_year.csv")))
            {
                var parts = line.Split(',');
                if (parts.Length == 3 && int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var y)) byStateAndYear[(parts[0], y)] = parts[2];
            }

            var columns = new Collection<FeatureSourceColumn> { new FeatureSourceColumn("STATE_ABBR", "String", 2) };
            for (var y = FirstYear; y <= LastYear; y++)
            {
                columns.Add(new FeatureSourceColumn("y" + y, "Integer", 12));
            }

            var shapefile = new ShapeFileFeatureSource(SampleData.Path("Shapefile/USStates_3857.shp"));
            shapefile.Open();
            var features = new Collection<Feature>();
            foreach (var state in shapefile.GetAllFeatures(new[] { "STATE_ABBR" }))
            {
                var abbreviation = state.ColumnValues["STATE_ABBR"];
                var values = new Dictionary<string, string> { ["STATE_ABBR"] = abbreviation };
                for (var y = FirstYear; y <= LastYear; y++)
                {
                    values["y" + y] = byStateAndYear.TryGetValue((abbreviation, y), out var population) ? population : "0";
                }
                features.Add(new Feature(state.GetShape().GetCenterPoint(), values));
            }
            shapefile.Close();
            return new InMemoryFeatureSource(columns, features);
        }
    }
}
