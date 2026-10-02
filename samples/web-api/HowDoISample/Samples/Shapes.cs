using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.Json;
using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI.Samples
{
    /// <summary>
    /// Shapes and Spatial Queries: geometry done on the server - buffers, unions, measurements,
    /// spatial relations, topology rules - and handed to the browser as GeoJSON to draw.
    /// </summary>
    public class Shapes : ISampleGroup
    {
        private const string WestRegionWkt =
            "POLYGON((-10780139 3918539, -10780206 3915600, -10780037 3913978, -10779176 3913336, " +
            "-10778280 3911934, -10778263 3910684, -10778382 3909569, -10778280 3907356, " +
            "-10785595 3904045, -10786034 3904822, -10786001 3908150, -10785933 3909315, " +
            "-10786001 3911275, -10785832 3914485, -10785832 3917728, -10785460 3919012, " +
            "-10782233 3918995, -10780139 3918539))";
        private static readonly string[] PieceColors = { "rgba(255,200,120,0.5)", "rgba(147,112,219,0.5)" };
        private static readonly PointShape Stadium = new PointShape(-10779651.5, 3915933.0);

        // The city limits, read once: the pieces the operations work on.
        private static readonly Lazy<(Collection<Feature> Pieces, RectangleShape Bounds)> CityLimits = new Lazy<(Collection<Feature>, RectangleShape)>(() =>
        {
            var file = SampleData.Frisco("FriscoCityLimitsDivided.shp");
            file.Open();
            var pieces = file.GetAllFeatures(ReturningColumnsType.NoColumns);
            var bounds = file.GetBoundingBox();
            file.Close();
            return (pieces, bounds);
        });

        public void Register(OverlayCatalog catalog)
        {
            catalog.Vector("parks", () =>
            {
                var overlay = new VectorTileOverlay();
                overlay.FeatureSources.Add("parks", SampleData.Frisco("Parks.shp"));
                return overlay;
            });
            catalog.Vector("trails", () =>
            {
                var overlay = new VectorTileOverlay();
                overlay.FeatureSources.Add("trails", SampleData.Frisco("Hike_Bike.shp"));
                return overlay;
            });
        }

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            // One of the shape operations run on the first piece of the city limits: orange is
            // the shape read from the file, green what the operation makes of it, blue the area
            // the paired operations use.
            app.MapGet("/samples/shapes/operation", (string name, double first, double second, bool merged) =>
            {
                var (pieces, bounds) = CityLimits.Value;
                var shape = pieces[0].GetShape();
                var westRegion = new PolygonShape(WestRegionWkt);
                var union = name == "Union of all the pieces";
                var paired = name == "Intersection with the blue area" || name == "Difference from the blue area";
                var source = new List<Feature>();
                BaseShape region = paired ? westRegion : null;
                BaseShape result;
                switch (name)
                {
                    case "Buffer": result = shape.Buffer(first, GeographyUnit.Meter, DistanceUnit.Meter); break;
                    case "Simplify": result = AreaBaseShape.Simplify((AreaBaseShape)shape, first, SimplificationType.DouglasPeucker); break;
                    case "Rotate": result = BaseShape.Rotate(shape, shape.GetCenterPoint(), (float)first); break;
                    case "Scale": result = BaseShape.ScaleTo(shape, first / 100.0); break;
                    case "Translate": result = BaseShape.TranslateByOffset(shape, first, second, GeographyUnit.Meter, DistanceUnit.Meter); break;
                    case "Convex hull": result = pieces[0].GetConvexHull().GetShape(); break;
                    case "Envelope": result = shape.GetBoundingBox(); break;
                    case "Split by a line":
                    {
                        // A line through the shape's centre at the angle given, and the half-plane on
                        // one side of it as a polygon far bigger than the shape: the shape is split by
                        // that area, and each piece comes back in a colour of its own.
                        var centre = shape.GetCenterPoint();
                        var reach = Math.Max(bounds.Width, bounds.Height) * 4;
                        var dx = Math.Cos(first * Math.PI / 180) * reach;
                        var dy = Math.Sin(first * Math.PI / 180) * reach;
                        var cut = new LineShape(new Collection<Vertex> { new Vertex(centre.X - dx, centre.Y - dy), new Vertex(centre.X + dx, centre.Y + dy) });
                        var side = new PolygonShape(new RingShape(new Collection<Vertex>
                        {
                            new Vertex(centre.X - dx, centre.Y - dy),
                            new Vertex(centre.X + dx, centre.Y + dy),
                            new Vertex(centre.X + dx - dy, centre.Y + dy + dx),
                            new Vertex(centre.X - dx - dy, centre.Y - dy + dx),
                            new Vertex(centre.X - dx, centre.Y - dy),
                        }));
                        var halves = AreaBaseShape.Split((AreaBaseShape)shape, side);
                        for (var i = 0; i < halves.Count; i++)
                        {
                            source.Add(new Feature(halves[i], new Dictionary<string, string> { ["color"] = PieceColors[i % PieceColors.Length] }));
                        }
                        result = cut;
                        break;
                    }
                    case "Snap a line to it":
                    {
                        // A short line starting a gap east of the shape. The reach is 400 metres:
                        // while the shape is within it, the line is moved so that it starts on the
                        // shape's closest point, the classic snap; the circle is the reach.
                        const double reachInMeters = 400;
                        var box = shape.GetBoundingBox();
                        var start = new PointShape(box.MaxX + first, box.GetCenterPoint().Y);
                        var probe = new LineShape(new Collection<Vertex> { new Vertex(start), new Vertex(start.X + 1500, start.Y + 600) });
                        region = start.Buffer(reachInMeters, GeographyUnit.Meter, DistanceUnit.Meter);
                        if (shape.GetDistanceTo(start, GeographyUnit.Meter, DistanceUnit.Meter) <= reachInMeters)
                        {
                            var closest = shape.GetClosestPointTo(start, GeographyUnit.Meter);
                            probe = new LineShape(new Collection<Vertex> { new Vertex(closest), new Vertex(closest.X + 1500, closest.Y + 600) });
                        }
                        result = probe;
                        break;
                    }
                    case "Intersection with the blue area": result = ((AreaBaseShape)shape).GetIntersection(westRegion); break;
                    case "Difference from the blue area": result = ((AreaBaseShape)shape).GetDifference(westRegion); break;
                    default: result = merged ? AreaBaseShape.Union(pieces) : null; break;
                }

                if (union && !merged)
                {
                    for (var i = 0; i < pieces.Count; i++)
                    {
                        source.Add(new Feature(pieces[i].GetShape(), new Dictionary<string, string> { ["color"] = PieceColors[i % PieceColors.Length] }));
                    }
                }
                else if (!union && source.Count == 0)
                {
                    source.Add(new Feature(shape, new Dictionary<string, string> { ["color"] = PieceColors[0] }));
                }
                var polygons = result is MultipolygonShape multipolygon ? multipolygon.Polygons.Count : result == null ? 0 : 1;
                var framed = new RectangleShape(bounds.UpperLeftPoint, bounds.LowerRightPoint);
                framed.ScaleUp(50);
                return Results.Json(new
                {
                    source = Json(GeoJson.Collection(source)),
                    region = region == null ? (JsonElement?)null : Json(GeoJson.Geometry(region)),
                    result = result == null ? (JsonElement?)null : Json(GeoJson.Geometry(result)),
                    note = union ? (merged ? pieces.Count + " shapes became " + polygons + " - one color, one outline" : pieces.Count + " separate shapes, each its own color") : "",
                    bounds = GeoJson.Bounds(framed),
                });
            });

            // Area, length, centre, the shortest line to the stadium, or a stretch along a
            // trail, measured on the feature nearest the click - or the biggest one. Every
            // measurement is taken in the Texas state plane feet the data is recorded in, never
            // in the Web Mercator the map is drawn in, which stretches everything by
            // 1/cos(latitude) - a fifth too long and nearly half again too large at Frisco.
            app.MapPost("/samples/shapes/measure", async (HttpRequest request) =>
            {
                var body = await Body(request);
                var measurement = body.GetProperty("measurement").GetString();
                var lines = measurement == "Length" || measurement == "A line along a line";
                // The file itself, in its own feet; a converter takes the click in and the answer out.
                var data = new ShapeFileFeatureSource(SampleData.Path("Shapefile/" + (lines ? "Hike_Bike.shp" : "Parks.shp")));
                var toFeet = new ProjectionConverter(3857, 2276);
                toFeet.Open();
                data.Open();
                Feature feature;
                RectangleShape bounds = null;
                if (body.TryGetProperty("lng", out var lng) && lng.ValueKind == JsonValueKind.Number)
                {
                    var at = (PointShape)toFeet.ConvertToExternalProjection(GeoJson.Point(lng.GetDouble(), body.GetProperty("lat").GetDouble()));
                    feature = data.GetFeaturesNearestTo(at, GeographyUnit.Feet, 1, ReturningColumnsType.NoColumns).FirstOrDefault();
                }
                else
                {
                    feature = data.GetAllFeatures(ReturningColumnsType.NoColumns).OrderByDescending(candidate => { var box = candidate.GetBoundingBox(); return box.Width + box.Height; }).FirstOrDefault();
                    bounds = feature == null ? null : (RectangleShape)toFeet.ConvertToInternalProjection(feature.GetBoundingBox());
                    bounds?.ScaleUp(60);
                }
                data.Close();
                if (feature == null) return Results.NotFound();

                var shape = feature.GetShape();
                var stadium = (PointShape)toFeet.ConvertToExternalProjection(Stadium);
                var drawn = new List<BaseShape>();
                string result;
                switch (measurement)
                {
                    case "Length":
                        drawn.Add(shape);
                        result = ((LineBaseShape)shape).GetLength(GeographyUnit.Feet, DistanceUnit.Kilometer).ToString("f3", CultureInfo.InvariantCulture) + " km";
                        break;
                    case "Center point":
                        var center = shape.GetCenterPoint();
                        drawn.Add(shape);
                        drawn.Add(center);
                        var onMap = (PointShape)toFeet.ConvertToInternalProjection(center);
                        result = onMap.X.ToString("f0", CultureInfo.InvariantCulture) + ", " + onMap.Y.ToString("f0", CultureInfo.InvariantCulture);
                        break;
                    case "Shortest line to the stadium":
                        var line = shape.GetShortestLineTo(stadium, GeographyUnit.Feet);
                        drawn.Add(shape);
                        drawn.Add(line);
                        result = line.GetLength(GeographyUnit.Feet, DistanceUnit.Kilometer).ToString("f3", CultureInfo.InvariantCulture) + " km to the stadium";
                        break;
                    case "A line along a line":
                        var trail = shape as LineShape ?? ((MultilineShape)shape).Lines.First();
                        var startingPoint = body.GetProperty("startingPoint").GetString() == "LastPoint" ? StartingPoint.LastPoint : StartingPoint.FirstPoint;
                        var stretch = trail.GetLineOnALine(startingPoint, body.GetProperty("startingOffset").GetDouble(), body.GetProperty("runLength").GetDouble(), GeographyUnit.Feet, DistanceUnit.Meter);
                        var got = stretch?.GetLength(GeographyUnit.Feet, DistanceUnit.Meter) ?? 0;
                        if (got > 0)
                        {
                            drawn.Add(stretch);
                            result = got.ToString("f0", CultureInfo.InvariantCulture) + " m of it";
                        }
                        else
                        {
                            result = "Nothing there - this trail is " + trail.GetLength(GeographyUnit.Feet, DistanceUnit.Meter).ToString("f0", CultureInfo.InvariantCulture) + " m long";
                        }
                        break;
                    default:
                        drawn.Add(shape);
                        result = ((AreaBaseShape)shape).GetArea(GeographyUnit.Feet, AreaUnit.SquareKilometers).ToString("f3", CultureInfo.InvariantCulture) + " sq km";
                        break;
                }
                // Measured in feet, drawn in the map's metres.
                var onScreen = drawn.Select(piece => toFeet.ConvertToInternalProjection(piece)).ToList();
                toFeet.Close();
                return Results.Json(new
                {
                    drawn = Json(GeoJson.Collection(onScreen)),
                    stadium = GeoJson.LngLat(Stadium),
                    result,
                    bounds = bounds == null ? null : GeoJson.Bounds(bounds),
                });
            });

            // The zoning parcels that contain, cross, are disjoint from, intersect, overlap,
            // touch, lie within or lie within a distance of a shape: QueryTools on the source.
            app.MapPost("/samples/shapes/relation", async (HttpRequest request) =>
            {
                var body = await Body(request);
                var relation = body.GetProperty("relation").GetString();
                var radius = body.TryGetProperty("radius", out var r) ? r.GetDouble() : 500;
                var shape = body.TryGetProperty("geometry", out var geometry) && geometry.ValueKind == JsonValueKind.Object ? GeoJson.Shape(geometry) : Seed(relation);

                if (relation == "Touching")
                {
                    // Touching is exact, so it is asked in the file's own plane and the answer converted out.
                    var native = new ShapeFileFeatureSource(SampleData.Path("Shapefile/Zoning.shp"));
                    var outward = new ProjectionConverter(2276, 3857);
                    outward.Open();
                    var asked = geometry.ValueKind == JsonValueKind.Object ? outward.ConvertToInternalProjection(shape) : shape;
                    native.Open();
                    var touching = new QueryTools(native).GetFeaturesTouching(asked, ReturningColumnsType.NoColumns).ToList();
                    native.Close();
                    var response = new
                    {
                        query = Json(GeoJson.Geometry(outward.ConvertToExternalProjection(asked))),
                        matches = Json(GeoJson.Collection(touching.Select(feature => outward.ConvertToExternalProjection(feature)))),
                        result = touching.Count + " features touching the query shape",
                    };
                    outward.Close();
                    return Results.Json(response);
                }

                var zoning = SampleData.Frisco("Zoning.shp");
                zoning.Open();
                var query = new QueryTools(zoning);
                IEnumerable<Feature> found;
                string verb;
                switch (relation)
                {
                    case "Crossing": found = query.GetFeaturesCrossing(shape, ReturningColumnsType.NoColumns); verb = "crossing"; break;
                    case "Disjoint": found = query.GetFeaturesDisjointed(shape, ReturningColumnsType.NoColumns); verb = "disjoint from"; break;
                    case "Intersecting": found = query.GetFeaturesIntersecting(shape, ReturningColumnsType.NoColumns); verb = "intersecting"; break;
                    case "Overlapping": found = query.GetFeaturesOverlapping(shape, ReturningColumnsType.NoColumns); verb = "overlapping"; break;
                    case "Within the shape": found = query.GetFeaturesWithin(shape, ReturningColumnsType.NoColumns); verb = "within"; break;
                    case "Within a distance": found = query.GetFeaturesWithinDistanceOf(shape, GeographyUnit.Meter, DistanceUnit.Meter, radius, ReturningColumnsType.NoColumns); verb = "within " + (int)radius + " m of"; break;
                    default: found = query.GetFeaturesContaining(shape, ReturningColumnsType.NoColumns); verb = "containing"; break;
                }
                var list = found.ToList();
                zoning.Close();
                return Results.Json(new
                {
                    query = Json(GeoJson.Geometry(shape)),
                    matches = Json(GeoJson.Collection(list)),
                    result = list.Count + " features " + verb + " the query shape",
                });
            });

            // The park under a click, with every column its record holds: a point-in-polygon
            // query asking for all columns.
            app.MapPost("/samples/shapes/feature-at", async (HttpRequest request) =>
            {
                var body = await Body(request);
                var parks = SampleData.Frisco("Parks.shp");
                parks.Open();
                var park = new QueryTools(parks).GetFeaturesContaining(GeoJson.Point(body.GetProperty("lng").GetDouble(), body.GetProperty("lat").GetDouble()), ReturningColumnsType.AllColumns).FirstOrDefault();
                parks.Close();
                if (park == null) return Results.Json(new { found = false });
                return Results.Json(new
                {
                    found = true,
                    at = GeoJson.LngLat(park.GetShape().GetCenterPoint()),
                    columns = park.ColumnValues.Select(column => new { name = column.Key, value = column.Value }),
                });
            });

            // A filled circle with a white rim, drawn by a PointStyle: the picture the pages pin.
            app.MapGet("/samples/shapes/circle.png", (string fill, int size) =>
                Results.Bytes(Pictures.OfStyle(PointStyle.CreateSimpleCircleStyle(GeoColor.FromHtml("#" + fill), size, GeoColors.White, 2), size + 6, size + 6), "image/png"));

            // One topology rule checked on crafted shapes: blue is what is checked against,
            // green what the rule checks, red what breaks it.
            app.MapGet("/samples/shapes/topology", (string rule) =>
            {
                var chosen = Topology.Rules.FirstOrDefault(candidate => candidate.Name == rule) ?? Topology.Rules[0];
                var subject = Topology.Wkt(chosen.Subject);
                var counterpart = chosen.Against == null ? new Collection<Feature>() : Topology.Wkt(chosen.Against);
                var invalid = chosen.Broken(subject, counterpart);
                var bounds = MapUtil.GetBoundingBoxOfItems(counterpart.Concat(subject).Concat(invalid).Select(feature => feature.GetShape()));
                bounds.ScaleUp(50);
                return Results.Json(new
                {
                    rules = Topology.Rules.Select(candidate => new { group = candidate.Group, name = candidate.Name }),
                    against = Json(GeoJson.Collection(counterpart)),
                    @checked = Json(GeoJson.Collection(subject)),
                    broken = Json(GeoJson.Collection(invalid)),
                    result = invalid.Count == 0 ? "Nothing breaks this rule. " + chosen.Note : invalid.Count + " in red. " + chosen.Note,
                    bounds = GeoJson.Bounds(bounds),
                });
            });
        }

        // Where each relation starts: a shape that has something to find.
        private static BaseShape Seed(string relation)
        {
            switch (relation)
            {
                case "Crossing": return new LineShape("LINESTRING(-10774628 3914024,-10776902 3915582,-10778030 3914368,-10778708 3914445)");
                case "Disjoint": return new PolygonShape("POLYGON((-10780418 3915973,-10780428 3913422,-10775737 3913413,-10775612 3915954,-10780418 3915973))");
                case "Intersecting": return new PolygonShape("POLYGON((-10778718 3914865,-10778746 3913709,-10777103 3913766,-10777179 3914942,-10778718 3914865))");
                case "Overlapping": return new PolygonShape("POLYGON((-10779549 3915352,-10777495 3915859,-10776214 3914827,-10776081 3913384,-10777906 3912553,-10779702 3914110,-10779549 3915352))");
                case "Touching":
                    // One edge of the first parcel, in the file's own plane.
                    var native = new ShapeFileFeatureSource(SampleData.Path("Shapefile/Zoning.shp"));
                    native.Open();
                    var first = (MultipolygonShape)native.GetAllFeatures(ReturningColumnsType.NoColumns).First().GetShape();
                    native.Close();
                    var vertices = first.Polygons.First().OuterRing.Vertices;
                    return new LineShape(new Collection<Vertex> { vertices[0], vertices[1] });
                case "Within the shape": return new PolygonShape("POLYGON((-10779148 3916088,-10779960 3913862,-10777189 3911913,-10777179 3915754,-10779148 3916088))");
                default: return new PointShape(-10779430, 3914970);
            }
        }

        private static JsonElement Json(string text) => JsonDocument.Parse(text).RootElement;

        private static async Task<JsonElement> Body(HttpRequest request)
        {
            using (var document = await JsonDocument.ParseAsync(request.Body))
            {
                return document.RootElement.Clone();
            }
        }

        /// <summary>Twenty-five topology rules, each with the shapes that show it.</summary>
        private static class Topology
        {
            public sealed class Rule
            {
                public string Group;
                public string Name;
                public string[] Subject;
                public string[] Against;
                public Func<Collection<Feature>, Collection<Feature>, Collection<Feature>> Broken;
                public string Note;
            }

            public static readonly Rule[] Rules =
            {
                new Rule { Group = "Points", Name = "Must touch lines", Subject = new[] { "POINT(0 0)", "POINT(50 0)", "POINT(150 0)" }, Against = new[] { "LINESTRING(0 0,100 0)" },
                    Broken = (s, a) => TopologyValidator.PointsMustTouchLines(s, a).InvalidFeatures, Note = "Points that miss the line are red." },
                new Rule { Group = "Points", Name = "Must touch line endpoints", Subject = new[] { "POINT(0 0)", "POINT(50 0)", "POINT(100 0)" }, Against = new[] { "LINESTRING(0 0,100 0,100 100,0 100)" },
                    Broken = (s, a) => TopologyValidator.PointsMustTouchLineEndpoints(s, a).InvalidFeatures, Note = "A point on the line but not at an end is still wrong." },
                new Rule { Group = "Points", Name = "Must touch polygon boundaries", Subject = new[] { "POINT(150 0)", "POINT(50 50)", "POINT(0 0)" }, Against = new[] { "POLYGON((0 0,100 0,100 100,0 100,0 0))" },
                    Broken = (s, a) => TopologyValidator.PointsMustTouchPolygonBoundaries(s, a).InvalidFeatures, Note = "Inside is not on the boundary; nor is outside." },
                new Rule { Group = "Points", Name = "Must be within polygons", Subject = new[] { "POINT(150 0)", "POINT(0 0)", "POINT(50 50)" }, Against = new[] { "POLYGON((0 0,100 0,100 100,0 100,0 0))" },
                    Broken = (s, a) => TopologyValidator.PointsMustBeWithinPolygons(s, a).InvalidFeatures, Note = "A point on the corner is not within." },

                new Rule { Group = "Lines", Name = "Endpoints must touch points", Subject = new[] { "LINESTRING(0 0,100 0,100 50)" }, Against = new[] { "POINT(0 0)" },
                    Broken = (s, a) => TopologyValidator.LineEndPointsMustTouchPoints(s, a).InvalidFeatures, Note = "One end has its point, the other does not." },
                new Rule { Group = "Lines", Name = "Must overlap polygon boundaries", Subject = new[] { "LINESTRING(-50 0,150 0)" }, Against = new[] { "POLYGON((0 0,100 0,100 100,0 100,0 0))" },
                    Broken = (s, a) => TopologyValidator.LinesMustOverlapPolygonBoundaries(s, a).InvalidFeatures, Note = "Only the stretch that runs along the boundary counts." },
                new Rule { Group = "Lines", Name = "Must be covered by other lines", Subject = new[] { "LINESTRING(0 0,100 0,100 100,0 100)" }, Against = new[] { "LINESTRING(0 -50,50 0,100 0,100 150)" },
                    Broken = (s, a) => TopologyValidator.LinesMustBeCoveredByLines(a, s).InvalidFeatures, Note = "The parts with nothing under them are red." },
                new Rule { Group = "Lines", Name = "Must be a single part", Subject = new[] { "MULTILINESTRING((0 -50,100 -50,100 -100,0 -100))", "MULTILINESTRING((0 0,100 0),(100 50,0 50))" },
                    Broken = (s, a) => TopologyValidator.LinesMustBeSinglePart(s).InvalidFeatures, Note = "One multiline is really one strand; the other is two." },
                new Rule { Group = "Lines", Name = "Must form a closed polygon", Subject = new[] { "LINESTRING(0 0,100 0,100 100,20 100)", "LINESTRING(0 0,-50 0,-50 100,0 100)" },
                    Broken = (s, a) => TopologyValidator.LinesMustFormClosedPolygon(s).InvalidFeatures, Note = "The ends that do not meet are red." },
                new Rule { Group = "Lines", Name = "Must not have pseudonodes", Subject = new[] { "LINESTRING(0 0,50 0,50 50,0 0)", "LINESTRING(-50 0,-50 50)", "LINESTRING(-100 0,-50 50)", "LINESTRING(-50 -50,-50 -100)", "LINESTRING(-100 -50,-50 -100)", "LINESTRING(-50 -100,0 -100)" },
                    Broken = (s, a) => TopologyValidator.LinesMustNotHavePseudonodes(s).InvalidFeatures, Note = "A node joining exactly two lines is a pseudonode." },
                new Rule { Group = "Lines", Name = "Must not intersect", Subject = new[] { "LINESTRING(0 0,100 0,100 100)", "LINESTRING(0 -50,30 0,60 0,100 50)", "LINESTRING(20 50,20 -50)" },
                    Broken = (s, a) => TopologyValidator.LinesMustNotIntersect(s).InvalidFeatures, Note = "Each crossing comes back as a point." },
                new Rule { Group = "Lines", Name = "Must not self-intersect or touch", Subject = new[] { "LINESTRING(0 0,100 0,100 100)", "LINESTRING(0 -50,30 0,60 0,100 50)", "LINESTRING(20 50,20 -50)" },
                    Broken = (s, a) => TopologyValidator.LinesMustNotSelfIntersectOrTouch(s).InvalidFeatures, Note = "Crossings and shared stretches both count." },
                new Rule { Group = "Lines", Name = "Must not overlap", Subject = new[] { "LINESTRING(0 0,100 0,100 100)", "LINESTRING(0 -50,30 0,60 0,100 50)", "LINESTRING(20 50,20 -50)" },
                    Broken = (s, a) => TopologyValidator.LinesMustNotOverlap(s).InvalidFeatures, Note = "Only the shared stretches, not the crossings." },
                new Rule { Group = "Lines", Name = "Must not overlap other lines", Subject = new[] { "LINESTRING(150 0,100 30,100 60,150 100)" }, Against = new[] { "LINESTRING(0 0,100 0,100 100,0 100)" },
                    Broken = (s, a) => TopologyValidator.LinesMustNotOverlapLines(a, s).InvalidFeatures, Note = "The stretch lying on the blue line is red." },
                new Rule { Group = "Lines", Name = "Must not self-intersect", Subject = new[] { "LINESTRING(0 0,100 0,100 100,50 100,50 -50)" },
                    Broken = (s, a) => TopologyValidator.LinesMustNotSelfIntersect(s).InvalidFeatures, Note = "One line, crossing itself once." },
                new Rule { Group = "Lines", Name = "Must not self-overlap", Subject = new[] { "LINESTRING(0 0,100 0,100 100,0 100,20 0,40 0,40 -50)" },
                    Broken = (s, a) => TopologyValidator.LinesMustNotSelfOverlap(s).InvalidFeatures, Note = "One line, doubling back along itself." },

                new Rule { Group = "Polygons", Name = "Boundaries must overlap boundaries", Subject = new[] { "POLYGON((0 0,50 0,50 50,0 50,0 0))" }, Against = new[] { "POLYGON((0 0,100 0,100 100,0 100,0 0))" },
                    Broken = (s, a) => TopologyValidator.PolygonBoundariesMustOverlapPolygonBoundaries(a, s).InvalidFeatures, Note = "Two corners are shared; the rest of the boundary is not." },
                new Rule { Group = "Polygons", Name = "Boundaries must overlap lines", Subject = new[] { "POLYGON((0 0,100 0,100 100,0 100,0 0))" }, Against = new[] { "LINESTRING(-50 0,100 0,100 150)" },
                    Broken = (s, a) => TopologyValidator.PolygonBoundariesMustOverlapLines(s, a).InvalidFeatures, Note = "The line covers two sides; the other two are red." },
                new Rule { Group = "Polygons", Name = "Must overlap other polygons", Subject = new[] { "POLYGON((25 25,50 25,50 50,25 50,25 25))", "POLYGON((75 25,125 25,125 75,75 75,75 25))", "POLYGON((150 25,200 25,200 75,150 75,150 25))" }, Against = new[] { "POLYGON((0 0,100 0,100 100,0 100,0 0))" },
                    Broken = (s, a) => TopologyValidator.PolygonsMustOverlapPolygons(a, s).InvalidFeatures, Note = "The one that never meets the blue square is red." },
                new Rule { Group = "Polygons", Name = "Must be within other polygons", Subject = new[] { "POLYGON((25 25,50 25,50 50,25 50,25 25))", "POLYGON((75 25,125 25,125 75,75 75,75 25))", "POLYGON((150 25,200 25,200 75,150 75,150 25))" }, Against = new[] { "POLYGON((0 0,100 0,100 100,0 100,0 0))" },
                    Broken = (s, a) => TopologyValidator.PolygonsMustBeWithinPolygons(a, s).InvalidFeatures, Note = "Overlapping is not enough - it has to be all the way inside." },
                new Rule { Group = "Polygons", Name = "Must contain points", Subject = new[] { "POLYGON((150 0,250 0,250 100,150 100,150 0))", "POLYGON((0 0,100 0,100 100,0 100,0 0))" }, Against = new[] { "POINT(50 50)" },
                    Broken = (s, a) => TopologyValidator.PolygonsMustContainPoint(s, a).InvalidFeatures, Note = "The empty polygon is red." },
                new Rule { Group = "Polygons", Name = "Must overlap each other", Subject = new[] { "POLYGON((0 0,100 0,100 100,0 100,0 0))", "POLYGON((-50 -50,50 -50,50 50,-50 50,-50 -50))" },
                    Broken = (s, a) => TopologyValidator.PolygonsMustOverlapEachOther(Wkt(s[0].GetWellKnownText()), Wkt(s[1].GetWellKnownText())).InvalidFeatures, Note = "This one reports from both sets: everything outside the shared corner." },
                new Rule { Group = "Polygons", Name = "Must not leave gaps", Subject = new[] { "POLYGON((0 0,40 0,40 40,0 40,0 0))", "POLYGON((30 30,70 30,70 70,30 70,30 30))", "POLYGON((60 0,100 0,100 40,60 40,60 0))", "POLYGON((30 10,70 10,70 -30,30 -30,30 10))" },
                    Broken = (s, a) => TopologyValidator.PolygonsMustNotHaveGaps(s).InvalidFeatures, Note = "The hole left between them comes back as its own shape." },
                new Rule { Group = "Polygons", Name = "Must not overlap", Subject = new[] { "POLYGON((25 25,50 25,50 50,25 50,25 25))", "POLYGON((75 25,125 25,125 75,75 75,75 25))", "POLYGON((150 25,200 25,200 75,150 75,150 25))", "POLYGON((0 0,100 0,100 100,0 100,0 0))" },
                    Broken = (s, a) => TopologyValidator.PolygonsMustNotOverlap(s).InvalidFeatures, Note = "Red is the overlap itself, not the polygons that made it." },
                new Rule { Group = "Polygons", Name = "Must not overlap other polygons", Subject = new[] { "POLYGON((25 25,50 25,50 50,25 50,25 25))", "POLYGON((75 25,125 25,125 75,75 75,75 25))", "POLYGON((150 25,200 25,200 75,150 75,150 25))" }, Against = new[] { "POLYGON((0 0,100 0,100 100,0 100,0 0))" },
                    Broken = (s, a) => TopologyValidator.PolygonsMustNotOverlapPolygons(a, s).InvalidFeatures, Note = "The reverse of the rule above: overlapping is the fault." },
            };

            public static Collection<Feature> Wkt(params string[] wellKnownText)
            {
                var features = new Collection<Feature>();
                foreach (var text in wellKnownText)
                {
                    features.Add(new Feature(text));
                }
                return features;
            }
        }
    }
}
