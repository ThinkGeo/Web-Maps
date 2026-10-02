using System.Globalization;
using System.Text.Json.Nodes;
using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI.Samples
{
    /// <summary>
    /// Map Navigation: what the server holds for the pages that move the map - a GPS route to
    /// drive, and an OpenDRIVE lane map cut into tiles from memory. The camera itself is the
    /// browser's.
    /// </summary>
    public class Navigation : ISampleGroup
    {
        private static readonly Lazy<LaneMap> Lanes = new Lazy<LaneMap>(() => LaneMap.Load(SampleData.Path("OpenDrive/XodrWolfsburgLanes.geojson"), SampleData.Path("OpenDrive/XodrWolfsburgRoute.json")));

        public void Register(OverlayCatalog catalog)
        {
            // The lane map is read into memory once and cut into tiles from there: eleven
            // thousand lane pieces, one source per layer the file names.
            catalog.Vector("lanes", () =>
            {
                var overlay = new VectorTileOverlay();
                foreach (var pair in Lanes.Value.Sources)
                {
                    overlay.FeatureSources.Add(pair.Key, pair.Value);
                }
                return overlay;
            });
        }

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            // The route through the lane map: a trace along it in the map's meters, the lane
            // instructions it passes, and what the map is.
            app.MapGet("/samples/navigation/lane-route", () =>
            {
                var lanes = Lanes.Value;
                return Results.Json(new
                {
                    trace = lanes.RouteTrace.Select(vertex => new[] { vertex.X, vertex.Y }),
                    legs = lanes.RouteLegs.Select(leg => new { label = leg.Label, startDistance = leg.StartDistance, instruction = leg.Instruction, lane = leg.Lane }),
                    overviewBounds = GeoJson.Bounds(lanes.OverviewExtent),
                    routeLength = lanes.RouteLength,
                    groundScale = lanes.GroundScale,
                    markingCount = lanes.MarkingCount,
                    title = lanes.Title,
                    attribution = lanes.Attribution,
                });
            });

            // A car seen from above, drawn on the server with a GeoCanvas: a pale body, a dark
            // cabin, a windscreen, headlights.
            app.MapGet("/samples/navigation/vehicle.png", () =>
            {
                using (var image = new GeoImage(60, 100))
                {
                    var canvas = GeoCanvas.CreateDefaultGeoCanvas();
                    canvas.BeginDrawing(image, new RectangleShape(0, 100, 60, 0), GeographyUnit.Meter);
                    var body = new EllipseShape(new PointShape(30, 50), 27, 48, GeographyUnit.Meter, DistanceUnit.Meter);
                    canvas.DrawArea(body, new GeoPen(GeoColor.FromHtml("#1b1f24"), 2), new GeoSolidBrush(GeoColor.FromHtml("#f3f5f7")), DrawingLevel.LevelOne);
                    canvas.DrawArea(new RectangleShape(10, 74, 50, 30), new GeoPen(GeoColor.FromHtml("#1b1f24"), 1), new GeoSolidBrush(GeoColor.FromHtml("#2c3440")), DrawingLevel.LevelOne);
                    canvas.DrawArea(new RectangleShape(12, 80, 48, 70), new GeoSolidBrush(GeoColor.FromHtml("#7ef2d9")), DrawingLevel.LevelOne);
                    canvas.DrawEllipse(new PointShape(16, 92), 8, 5, new GeoSolidBrush(GeoColor.FromHtml("#fff3b0")), DrawingLevel.LevelOne);
                    canvas.DrawEllipse(new PointShape(44, 92), 8, 5, new GeoSolidBrush(GeoColor.FromHtml("#fff3b0")), DrawingLevel.LevelOne);
                    canvas.EndDrawing();
                    return Results.Bytes(image.GetImageBytes(GeoImageFormat.Png), "image/png");
                }
            });
        }

        /// <summary>
        /// The OpenDRIVE lanes as GeoJSON, one in-memory source per layer named in the file, and
        /// the route as a trace along it with the lane instructions the route file gives.
        /// </summary>
        private sealed class LaneMap
        {
            public Dictionary<string, InMemoryFeatureSource> Sources { get; private set; }
            public IReadOnlyList<Vertex> RouteTrace { get; private set; }
            public IReadOnlyList<RouteLeg> RouteLegs { get; private set; }
            public RectangleShape OverviewExtent { get; private set; }
            public double RouteLength { get; private set; }
            public int MarkingCount { get; private set; }
            public string Title { get; private set; }
            public string Attribution { get; private set; }
            public double GroundScale { get; private set; }

            public static LaneMap Load(string lanesFile, string routeFile)
            {
                var lanes = JsonNode.Parse(File.ReadAllText(lanesFile));
                var byLayer = new Dictionary<string, List<Feature>>(StringComparer.Ordinal);
                var columnsByLayer = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal);
                foreach (var node in lanes["features"].AsArray())
                {
                    var properties = node["properties"].AsObject();
                    var layer = properties["layer"].GetValue<string>();
                    var geometry = node["geometry"];
                    var values = new Dictionary<string, string>(StringComparer.Ordinal);
                    foreach (var pair in properties)
                    {
                        values[pair.Key] = pair.Value?.ToString() ?? string.Empty;
                    }
                    BaseShape shape = geometry["type"].GetValue<string>() == "Polygon"
                        ? Polygon(Project(geometry["coordinates"][0].AsArray()))
                        : new LineShape(Project(geometry["coordinates"].AsArray()));
                    if (!byLayer.TryGetValue(layer, out var list))
                    {
                        byLayer[layer] = list = new List<Feature>();
                        columnsByLayer[layer] = new HashSet<string>(StringComparer.Ordinal);
                    }
                    columnsByLayer[layer].UnionWith(values.Keys);
                    list.Add(new Feature(shape, values));
                }

                var sources = new Dictionary<string, InMemoryFeatureSource>(StringComparer.Ordinal);
                foreach (var pair in byLayer)
                {
                    var source = new InMemoryFeatureSource(columnsByLayer[pair.Key].Select(column => new FeatureSourceColumn(column)).ToArray(), pair.Value);
                    source.BuildIndex();
                    sources.Add(pair.Key, source);
                }

                var routeJson = JsonNode.Parse(File.ReadAllText(routeFile));
                var points = new List<Vertex>();
                var ordinals = new List<int>();
                foreach (var point in routeJson["points"].AsArray())
                {
                    points.Add(Project(point[0].GetValue<double>(), point[1].GetValue<double>()));
                    ordinals.Add(point[2].GetValue<int>());
                }
                var laneInfo = routeJson["lanes"].AsArray()
                    .Select(lane => (Index: lane["index"].GetValue<int>(), Count: lane["count"].GetValue<int>(), Intersection: lane["intersection"].GetValue<bool>()))
                    .ToList();

                var legs = new List<RouteLeg>();
                string previousLabel = null;
                for (var i = 0; i < points.Count; i++)
                {
                    var lane = laneInfo[ordinals[i]];
                    var label = lane.Intersection ? "Intersection" : string.Format(CultureInfo.InvariantCulture, "Lane {0} of {1}", lane.Index, lane.Count);
                    if (label == previousLabel) continue;
                    previousLabel = label;
                    var position = lane.Count <= 1 ? 0.5 : (lane.Index - 1) / (double)(lane.Count - 1);
                    var side = lane.Intersection ? string.Empty : position < 0.34 ? "left" : position > 0.66 ? "right" : "center";
                    var instruction = legs.Count == 0 ? "Start in " + label.ToLowerInvariant()
                        : lane.Intersection ? "Cross the intersection"
                        : "Keep to " + label.ToLowerInvariant();
                    legs.Add(new RouteLeg(label, Length(points.Take(i + 1).ToList()), instruction, side));
                }

                var trace = Densify(points, 8);
                var midY = trace[trace.Count / 2].Y;
                var name = Path.GetFileNameWithoutExtension(lanesFile).Replace("Lanes", string.Empty).Replace("Xodr", string.Empty);
                return new LaneMap
                {
                    Sources = sources,
                    RouteTrace = trace,
                    RouteLegs = legs,
                    OverviewExtent = ExtentOf(trace, 220),
                    RouteLength = Length(trace),
                    MarkingCount = byLayer.Where(pair => pair.Key.StartsWith("lane_marking", StringComparison.Ordinal)).Sum(pair => pair.Value.Count),
                    Title = name + ", OpenDRIVE HD map",
                    Attribution = routeJson["attribution"]?.GetValue<string>() ?? string.Empty,
                    // Metres on the ground per metre of Spherical Mercator here.
                    GroundScale = 1.0 / Math.Cosh(midY / 6378137.0)
                };
            }

            private static List<Vertex> Project(JsonArray coordinates)
            {
                return coordinates.Select(c => Project(c[0].GetValue<double>(), c[1].GetValue<double>())).ToList();
            }

            private static Vertex Project(double longitude, double latitude)
            {
                const double radius = 6378137.0;
                var lat = Math.Max(-85.05112878, Math.Min(85.05112878, latitude));
                return new Vertex(radius * longitude * Math.PI / 180.0, radius * Math.Log(Math.Tan(Math.PI / 4.0 + lat * Math.PI / 360.0)));
            }

            private static PolygonShape Polygon(List<Vertex> vertices)
            {
                if (vertices.Count > 0 && (vertices[0].X != vertices[vertices.Count - 1].X || vertices[0].Y != vertices[vertices.Count - 1].Y))
                {
                    vertices.Add(vertices[0]);
                }
                return new PolygonShape(new RingShape(vertices));
            }

            private static List<Vertex> Densify(IReadOnlyList<Vertex> points, double spacing)
            {
                var result = new List<Vertex>();
                for (var i = 0; i < points.Count - 1; i++)
                {
                    var steps = Math.Max(1, (int)Math.Ceiling(Distance(points[i], points[i + 1]) / spacing));
                    for (var s = 0; s < steps; s++)
                    {
                        var t = s / (double)steps;
                        result.Add(new Vertex(points[i].X + (points[i + 1].X - points[i].X) * t, points[i].Y + (points[i + 1].Y - points[i].Y) * t));
                    }
                }
                result.Add(points[points.Count - 1]);
                return result;
            }

            private static double Length(IReadOnlyList<Vertex> points)
            {
                var total = 0d;
                for (var i = 0; i < points.Count - 1; i++)
                {
                    total += Distance(points[i], points[i + 1]);
                }
                return total;
            }

            private static double Distance(Vertex a, Vertex b) => Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));

            private static RectangleShape ExtentOf(IEnumerable<Vertex> points, double padding)
            {
                var array = points.ToArray();
                return new RectangleShape(array.Min(p => p.X) - padding, array.Max(p => p.Y) + padding, array.Max(p => p.X) + padding, array.Min(p => p.Y) - padding);
            }
        }

        private readonly struct RouteLeg
        {
            public RouteLeg(string label, double startDistance, string instruction, string lane)
            {
                Label = label;
                StartDistance = startDistance;
                Instruction = instruction;
                Lane = lane;
            }

            public string Label { get; }
            public double StartDistance { get; }
            public string Instruction { get; }
            public string Lane { get; }
        }
    }
}
