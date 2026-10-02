using System.Globalization;
using System.Text.Json;
using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI.Samples
{
    /// <summary>
    /// ThinkGeo Cloud Services: the browser asks the server, the server asks the cloud with its
    /// client id and secret, and the answer comes back as GeoJSON the map draws. The keys stay
    /// on the server.
    /// </summary>
    public class Cloud : ISampleGroup
    {
        private static readonly int[] ServiceAreaMinutes = { 15, 30, 45, 60 };

        public void Register(OverlayCatalog catalog)
        {
            // The census block groups the Colour Utilities page classes.
            catalog.Vector("housing", () =>
            {
                var overlay = new VectorTileOverlay();
                overlay.FeatureSources.Add("housing", SampleData.Frisco("Frisco 2010 Census Housing Units.shp"));
                return overlay;
            });
        }

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            // An address or a place name goes to GeocodingCloudClient.SearchAsync; every match
            // comes back as a point, a bounding box and its properties.
            app.MapPost("/samples/cloud/geocode", async (HttpRequest request) =>
            {
                var body = await Body(request);
                var client = new GeocodingCloudClient(GlobalSettings.ThinkGeoCloudClientId, GlobalSettings.ThinkGeoCloudClientSecret);
                var options = new CloudGeocodingOptions
                {
                    MaxResults = body.TryGetProperty("maxResults", out var max) ? max.GetInt32() : 10,
                    ResultProjectionInSrid = 3857,
                    BBox = body.TryGetProperty("bbox", out var bbox) && bbox.ValueKind == JsonValueKind.String ? GeoJson.Extent(bbox.GetString()) : null,
                };
                var result = await client.SearchAsync(body.GetProperty("text").GetString(), options);
                if (result.Exception != null) return Results.Problem(result.Exception.Message, statusCode: 502);
                return Results.Json(new
                {
                    locations = (result.Locations ?? Enumerable.Empty<CloudGeocodingLocation>()).Select(location => new
                    {
                        name = location.LocationName,
                        type = location.LocationType,
                        point = GeoJson.LngLat(location.LocationPoint),
                        bounds = location.BoundingBox != null && location.BoundingBox.Width > 1 && location.BoundingBox.Height > 1 ? GeoJson.Bounds(location.BoundingBox) : null,
                    }),
                });
            });

            // A point and a search radius go to ReverseGeocodingCloudClient.SearchPointAsync,
            // which answers with the best match and what else lies within the radius.
            app.MapPost("/samples/cloud/reverse-geocode", async (HttpRequest request) =>
            {
                var body = await Body(request);
                var point = GeoJson.Point(body.GetProperty("lng").GetDouble(), body.GetProperty("lat").GetDouble());
                var radius = body.GetProperty("radius").GetInt32();
                var client = new ReverseGeocodingCloudClient(GlobalSettings.ThinkGeoCloudClientId, GlobalSettings.ThinkGeoCloudClientSecret);
                var result = await client.SearchPointAsync(point.X, point.Y, 3857, radius, DistanceUnit.Meter, new CloudReverseGeocodingOptions { MaxResults = body.GetProperty("maxResults").GetInt32() });
                if (result.Exception != null) return Results.Problem(result.Exception.Message, statusCode: 502);

                object best = null;
                if (result.BestMatchLocation != null)
                {
                    var shape = result.BestMatchLocation.LocationFeature.GetShape();
                    var at = shape.GetClosestPointTo(point, GeographyUnit.Meter) ?? shape.GetCenterPoint();
                    best = new { text = string.IsNullOrEmpty(result.BestMatchLocation.Address) ? result.BestMatchLocation.LocationName : result.BestMatchLocation.Address, at = GeoJson.LngLat(at) };
                }
                return Results.Json(new
                {
                    circle = JsonDocument.Parse(GeoJson.Geometry(new EllipseShape(point, radius))).RootElement,
                    best,
                    nearby = (result.NearbyLocations ?? Enumerable.Empty<CloudReverseGeocodingLocation>()).Select(location => new
                    {
                        text = string.IsNullOrEmpty(location.Address) ? location.LocationName : location.Address,
                        type = location.LocationType,
                        category = location.LocationCategory,
                        distance = location.DistanceFromQueryFeature,
                        direction = location.DirectionFromQueryFeature,
                        geometry = JsonDocument.Parse(GeoJson.Geometry(location.LocationFeature.GetShape())).RootElement,
                    }),
                });
            });

            // Stops go to RoutingCloudClient.GetRouteAsync, or GetOptimizedRouteAsync when the
            // order is theirs to choose; the route and its turn-by-turn segments come back.
            app.MapPost("/samples/cloud/route", async (HttpRequest request) =>
            {
                var body = await Body(request);
                var waypoints = body.GetProperty("stops").EnumerateArray().Select(stop => GeoJson.Point(stop[0].GetDouble(), stop[1].GetDouble())).ToList();
                var optimize = body.TryGetProperty("optimize", out var flag) && flag.GetBoolean();
                var client = new RoutingCloudClient(GlobalSettings.ThinkGeoCloudClientId, GlobalSettings.ThinkGeoCloudClientSecret);

                IEnumerable<CloudRoutingRoute> routes;
                var stops = new List<Feature>();
                if (optimize)
                {
                    var options = new CloudRoutingOptimizationOptions { TurnByTurn = true, Roundtrip = true, Source = CloudRoutingTspFixSourceCoordinate.Any, Destination = CloudRoutingTspFixDestinationCoordinate.Any };
                    var result = await client.GetOptimizedRouteAsync(waypoints, 3857, options);
                    if (result.Exception != null) return Results.Problem(result.Exception.Message, statusCode: 502);
                    routes = result.TspResult.Routes;
                    var sequence = result.TspResult.VisitSequences;
                    for (var order = 0; order < sequence.Count; order++)
                    {
                        if (order == sequence.Count - 1 && sequence[order] == sequence[0]) break;
                        stops.Add(new Feature(waypoints[sequence[order]], new Dictionary<string, string> { ["order"] = (order + 1).ToString(), ["label"] = order == 0 ? "Start and end" : "Stop " + order }));
                    }
                }
                else
                {
                    var result = await client.GetRouteAsync(waypoints, 3857, new CloudRoutingGetRouteOptions { TurnByTurn = true });
                    if (result.Exception != null) return Results.Problem(result.Exception.Message, statusCode: 502);
                    routes = result.RouteResult.Routes;
                    var index = 0;
                    foreach (var waypoint in result.RouteResult.Waypoints)
                    {
                        var label = index == 0 ? "Start" : index == result.RouteResult.Waypoints.Count - 1 ? "End" : "Stop " + index;
                        stops.Add(new Feature(new PointShape(waypoint.Coordinate), new Dictionary<string, string> { ["order"] = (index + 1).ToString(), ["label"] = label }));
                        index++;
                    }
                }

                var parts = routes.ToList();
                var bounds = MapUtil.GetBoundingBoxOfItems(parts.Select(part => part.Shape));
                bounds.ScaleUp(20);
                return Results.Json(new
                {
                    route = JsonDocument.Parse(GeoJson.Collection(parts.Select(part => part.Shape))).RootElement,
                    stops = JsonDocument.Parse(GeoJson.Collection(stops)).RootElement,
                    segments = parts.SelectMany(part => part.Segments).Select(segment => new
                    {
                        instruction = segment.Instruction,
                        distance = segment.Distance,
                        seconds = segment.Duration.TotalSeconds,
                        geometry = JsonDocument.Parse(GeoJson.Geometry(segment.Shape)).RootElement,
                        bounds = GeoJson.Bounds(Padded(segment.Shape.GetBoundingBox())),
                    }),
                    meters = parts.Sum(part => part.Distance),
                    seconds = parts.Sum(part => part.Duration.TotalSeconds),
                    bounds = GeoJson.Bounds(bounds),
                });
            });

            // How far 15, 30, 45 and 60 minutes of driving reach from a point: one polygon per
            // limit from RoutingCloudClient.GetServiceAreaAsync.
            app.MapPost("/samples/cloud/service-area", async (HttpRequest request) =>
            {
                var body = await Body(request);
                var point = GeoJson.Point(body.GetProperty("lng").GetDouble(), body.GetProperty("lat").GetDouble());
                var client = new RoutingCloudClient(GlobalSettings.ThinkGeoCloudClientId, GlobalSettings.ThinkGeoCloudClientSecret);
                var limits = ServiceAreaMinutes.Select(minutes => TimeSpan.FromMinutes(minutes)).ToList();
                var result = await client.GetServiceAreaAsync(point, 3857, limits, new CloudRoutingGetServiceAreaOptions { DistanceUnit = DistanceUnit.Meter });
                if (result.Exception != null) return Results.Problem(result.Exception.Message, statusCode: 502);

                var polygons = result.ServiceAreaResult.ServiceAreas;
                var areas = new List<Feature>();
                for (var i = 0; i < polygons.Count && i < ServiceAreaMinutes.Length; i++)
                {
                    areas.Add(new Feature(polygons[i], new Dictionary<string, string> { ["minutes"] = ServiceAreaMinutes[i].ToString(CultureInfo.InvariantCulture) }));
                }
                var bounds = MapUtil.GetBoundingBoxOfItems(polygons);
                bounds.ScaleUp(10);
                return Results.Json(new { areas = JsonDocument.Parse(GeoJson.Collection(areas)).RootElement, count = polygons.Count, bounds = GeoJson.Bounds(bounds) });
            });

            // A point, a line or an area is sampled by ElevationCloudClient; a line or an area
            // every few meters, and each sample comes back with its height in feet.
            app.MapPost("/samples/cloud/elevation", async (HttpRequest request) =>
            {
                var body = await Body(request);
                var shape = GeoJson.Shape(body.GetProperty("geometry"));
                var interval = body.GetProperty("interval").GetInt32();
                if (shape is LineShape line && line.GetLength(GeographyUnit.Meter, DistanceUnit.Kilometer) > 5) return Results.Problem("That line is longer than 5 km; draw a shorter one.", statusCode: 400);
                if (shape is PolygonShape polygon && polygon.GetArea(GeographyUnit.Meter, AreaUnit.SquareKilometers) > 5) return Results.Problem("That area is larger than 5 square km; draw a smaller one.", statusCode: 400);

                var client = new ElevationCloudClient(GlobalSettings.ThinkGeoCloudClientId, GlobalSettings.ThinkGeoCloudClientSecret);
                List<CloudElevationPointResult> samples;
                double average, highest, lowest;
                try
                {
                    switch (shape)
                    {
                        case PointShape point:
                            var feet = await client.GetElevationOfPointAsync(point.X, point.Y, 3857);
                            samples = new List<CloudElevationPointResult> { new CloudElevationPointResult(feet, point) };
                            average = highest = lowest = feet;
                            break;
                        case LineShape lineShape:
                            var alongLine = await client.GetElevationOfLineAsync(lineShape, 3857, interval, DistanceUnit.Meter);
                            samples = alongLine.ElevationPoints.ToList();
                            average = alongLine.AverageElevation;
                            highest = alongLine.HighestElevationPoint.Elevation;
                            lowest = alongLine.LowestElevationPoint.Elevation;
                            break;
                        case AreaBaseShape area:
                            var acrossArea = await client.GetElevationOfAreaAsync(area, 3857, interval, DistanceUnit.Meter);
                            samples = acrossArea.ElevationPoints.ToList();
                            average = acrossArea.AverageElevation;
                            highest = acrossArea.HighestElevationPoint.Elevation;
                            lowest = acrossArea.LowestElevationPoint.Elevation;
                            break;
                        default:
                            return Results.Problem("Draw a point, a line or an area.", statusCode: 400);
                    }
                }
                catch (Exception exception)
                {
                    return Results.Problem(exception.Message, statusCode: 502);
                }
                return Results.Json(new
                {
                    samples = samples.Select(sample => new { at = GeoJson.LngLat(sample.Point), feet = sample.Elevation }),
                    average,
                    highest,
                    lowest,
                });
            });

            // ColorCloudClient hands back a family of colours that go together, from a colour
            // given or one it picks. A family with a base colour gives several sets - one per
            // colour of the family - and the sets are read in order; a hue or quality family is
            // one set.
            app.MapPost("/samples/cloud/colors", async (HttpRequest request) =>
            {
                var body = await Body(request);
                var family = body.GetProperty("family").GetString();
                var count = body.GetProperty("count").GetInt32();
                var colour = body.TryGetProperty("color", out var given) && given.ValueKind == JsonValueKind.String ? GeoColor.FromHtml(given.GetString()) : null;
                var client = new ColorCloudClient(GlobalSettings.ThinkGeoCloudClientId, GlobalSettings.ThinkGeoCloudClientSecret);
                try
                {
                    IEnumerable<GeoColor> colours;
                    switch (family)
                    {
                        case "Hue": colours = colour == null ? await client.GetColorsInHueFamilyAsync(count) : await client.GetColorsInHueFamilyAsync(colour, count); break;
                        case "Quality": colours = colour == null ? await client.GetColorsInQualityFamilyAsync(count) : await client.GetColorsInQualityFamilyAsync(colour, count); break;
                        case "Analogous": colours = Flat(colour == null ? await client.GetColorsInAnalogousFamilyAsync(count) : await client.GetColorsInAnalogousFamilyAsync(colour, count)); break;
                        case "Complementary": colours = Flat(colour == null ? await client.GetColorsInComplementaryFamilyAsync(count) : await client.GetColorsInComplementaryFamilyAsync(colour, count)); break;
                        case "Contrasting": colours = Flat(colour == null ? await client.GetColorsInContrastingFamilyAsync(count) : await client.GetColorsInContrastingFamilyAsync(colour, count)); break;
                        case "Tetrad": colours = Flat(colour == null ? await client.GetColorsInTetradFamilyAsync(count) : await client.GetColorsInTetradFamilyAsync(colour, count)); break;
                        default: colours = Flat(colour == null ? await client.GetColorsInTriadFamilyAsync(count) : await client.GetColorsInTriadFamilyAsync(colour, count)); break;
                    }
                    return Results.Json(new { colors = colours.Take(count).Select(c => $"#{c.R:X2}{c.G:X2}{c.B:X2}") });
                }
                catch (Exception exception)
                {
                    return Results.Problem(exception.Message, statusCode: 502);
                }
            });

        }

        private static async Task<JsonElement> Body(HttpRequest request)
        {
            using (var document = await JsonDocument.ParseAsync(request.Body))
            {
                return document.RootElement.Clone();
            }
        }

        private static RectangleShape Padded(RectangleShape bounds)
        {
            bounds.ScaleUp(60);
            if (bounds.Width < 400 || bounds.Height < 400)
            {
                var center = bounds.GetCenterPoint();
                bounds = new RectangleShape(center.X - 200, center.Y + 200, center.X + 200, center.Y - 200);
            }
            return bounds;
        }

        private static IEnumerable<GeoColor> Flat(Dictionary<GeoColor, System.Collections.ObjectModel.Collection<GeoColor>> sets) => sets.Values.SelectMany(set => set);
    }
}
