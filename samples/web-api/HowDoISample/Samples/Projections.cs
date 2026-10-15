using System.Collections.ObjectModel;
using System.Text.Json;
using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI.Samples
{
    /// <summary>
    /// Projections: features and rasters converted between projections on the server. The
    /// browser's map is Web Mercator; what is drawn in another projection is drawn on the server
    /// into a picture.
    /// </summary>
    public class Projections : ISampleGroup
    {
        // Greenland in degrees, read once.
        private static readonly Lazy<MultipolygonShape> Greenland = new Lazy<MultipolygonShape>(() =>
        {
            var source = new ShapeFileFeatureSource(SampleData.Path("Shapefile/Countries02.shp"));
            source.Open();
            var found = source.GetAllFeatures(new[] { "CNTRY_NAME" }).First(candidate => candidate.ColumnValues["CNTRY_NAME"].Trim() == "Greenland");
            source.Close();
            return (MultipolygonShape)found.GetShape();
        });

        // An equal area projection centred where it is told; the page asks for the same string
        // with its own latitude in it, so the two sides move Greenland in exactly the same way.
        private static string EqualArea(object latitude, double longitude)
            => FormattableString.Invariant($"+proj=laea +lat_0={latitude} +lon_0={longitude} +x_0=0 +y_0=0 +datum=WGS84 +units=m +no_defs");

        // Projections the world can be drawn in, as proj strings; null is the map's own. Every
        // parameter is written out: the browser's proj4js fills in no defaults.
        public static readonly Dictionary<string, string> WorldProjections = new Dictionary<string, string>
        {
            ["Spherical Mercator"] = null,
            ["US National Atlas Equal Area"] = "+proj=laea +lat_0=45 +lon_0=-100 +x_0=0 +y_0=0 +a=6370997 +b=6370997 +units=m +no_defs",
            ["Robinson"] = "+proj=robin +lon_0=0 +x_0=0 +y_0=0 +datum=WGS84 +units=m +no_defs",
            ["Mollweide"] = "+proj=moll +lon_0=0 +x_0=0 +y_0=0 +datum=WGS84 +units=m +no_defs",
            ["Equal Earth"] = "+proj=eqearth +lon_0=0 +x_0=0 +y_0=0 +datum=WGS84 +units=m +no_defs",
            ["Equal Area - Cylindrical"] = "+proj=cea +lon_0=0 +lat_ts=0 +x_0=0 +y_0=0 +datum=WGS84 +units=m +no_defs",
            ["Equal Area - Sinusoidal"] = "+proj=sinu +lon_0=0 +x_0=0 +y_0=0 +datum=WGS84 +units=m +no_defs",
        };

        public void Register(OverlayCatalog catalog)
        {
            // The world in every projection listed, from two sources: ThinkGeo Cloud's aerial
            // tiles, which come in Spherical Mercator and are warped tile by tile by a projection
            // converter on the layer, and a GeoTIFF stored in WGS84, warped by a
            // GdalProjectionConverter on its image source. Each is drawn into tiles of a grid laid
            // over the projection's world extent, which the browser's map asks for by z/x/y.
            foreach (var pair in WorldProjections)
            {
                var target = pair.Value;
                var slug = YourData.Slug(pair.Key);
                catalog.Raster("any-projection-aerial-" + slug, () =>
                {
                    var aerial = new AerialTilesLayer(GlobalSettings.ThinkGeoApiKey);
                    if (target != null)
                    {
                        aerial.ProjectionConverter = Converter(3857, target);
                        aerial.ProjectionConverter.Open();
                    }
                    var overlay = new LayerOverlay();
                    overlay.Layers.Add(aerial);
                    return overlay;
                });
                catalog.Raster("any-projection-geotiff-" + slug, () =>
                {
                    var geotiff = new GeoTiffRasterLayer(SampleData.Path("GeoTiff/World.tif"));
                    geotiff.ImageSource.ProjectionConverter = Converter(4326, target ?? "EPSG:3857");
                    var overlay = new LayerOverlay();
                    overlay.Layers.Add(geotiff);
                    return overlay;
                });
            }

            // The shapefile is in Texas state plane feet (EPSG:2276). Its ProjectionConverter says
            // so, and the source hands out every feature in the map's meters - the tiles are cut
            // as if the file had always been Web Mercator.
            catalog.Vector("zoning", () =>
            {
                var tiles = new FeatureSourceVectorTileSource();
                tiles.FeatureSources.Add("zoning", SampleData.Frisco("Zoning.shp"));
                return new VectorTileOverlay(tiles);
            });

            // A Los Angeles to Shanghai route stored with continuous longitudes past -180, its
            // corridor, and the airports: three in-memory sources cut into one set of tiles. The
            // cut folds the far side back into the world, so the line draws whole either side of
            // the seam.
            catalog.Vector("flight", () =>
            {
                var tiles = new FeatureSourceVectorTileSource();
                var flight = new Flight();
                tiles.FeatureSources.Add("route", new InMemoryFeatureSource(Array.Empty<FeatureSourceColumn>(), new[] { new Feature(flight.RouteLine()) }));
                tiles.FeatureSources.Add("corridor", new InMemoryFeatureSource(Array.Empty<FeatureSourceColumn>(), new[] { new Feature(flight.CorridorEdges()) }));
                tiles.FeatureSources.Add("airports", new InMemoryFeatureSource(new[] { new FeatureSourceColumn("LABEL") }, flight.Airports()));
                return new VectorTileOverlay(tiles);
            });
        }

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            app.MapGet("/samples/projections/zoning-bounds", () =>
            {
                var zoning = SampleData.Frisco("Zoning.shp");
                zoning.Open();
                var bounds = zoning.GetBoundingBox();
                zoning.Close();
                bounds.ScaleUp(50);
                return Results.Json(GeoJson.Bounds(bounds));
            });

            // Well known text in one projection, each line a feature, converted to the map's by
            // a ProjectionConverter: back as the meters the map works in, and as GeoJSON to draw.
            app.MapPost("/samples/projections/reproject", async (HttpRequest request) =>
            {
                var body = await JsonDocument.ParseAsync(request.Body);
                var epsg = body.RootElement.GetProperty("epsg").GetInt32();
                var features = new List<Feature>();
                foreach (var line in body.RootElement.GetProperty("wkt").GetString().Split('\n'))
                {
                    if (line.Trim().Length == 0) continue;
                    try
                    {
                        features.Add(new Feature(line.Trim()));
                    }
                    catch (Exception exception)
                    {
                        return Results.Problem(exception.Message.Split('\n')[0], statusCode: 400);
                    }
                }

                var converter = new ProjectionConverter(epsg, 3857);
                converter.Open();
                var converted = converter.ConvertToExternalProjection(features);
                converter.Close();

                var bounds = MapUtil.GetBoundingBoxOfItems(converted);
                return Results.Json(new
                {
                    features = JsonDocument.Parse(GeoJson.Collection(converted)).RootElement,
                    converted = converted.Select(feature => feature.GetWellKnownText()),
                    bounds = GeoJson.Bounds(bounds),
                });
            });

            // The tile grid of a projection: its world extent made square, and the resolution of
            // each zoom level, 512-pixel tiles from the top left. The browser builds the same grid
            // from this answer, so both sides mean the same tile by z/x/y.
            app.MapGet("/samples/projections/grid", (string projection) =>
            {
                var target = WorldProjections.TryGetValue(projection ?? "", out var found) ? found : null;
                var grid = GridOf(target);
                var world = WorldBoundsOf(target);
                return Results.Json(new
                {
                    proj4 = target,
                    extent = new[] { grid.MinX, grid.MinY, grid.MaxX, grid.MaxY },
                    worldExtent = new[] { world.MinX, world.MinY, world.MaxX, world.MaxY },
                    outline = WorldOutlineOf(target).Select(vertex => new[] { vertex.X, vertex.Y }),
                    resolutions = Enumerable.Range(0, 9).Select(zoom => grid.Width / 512 / Math.Pow(2, zoom)),
                    tileSize = 512,
                });
            });

            // One tile of the world in a projection, drawn on the server by the overlay for that
            // source and projection at the tile's extent in the grid above.
            app.MapGet("/samples/projections/tile/{source}/{slug}/{z:int}/{x:int}/{y:int}.png", async (string source, string slug, int z, int x, int y, OverlayCatalog catalog, CancellationToken cancellation) =>
            {
                var name = WorldProjections.Keys.FirstOrDefault(candidate => YourData.Slug(candidate) == slug);
                var id = "any-projection-" + source + "-" + slug;
                var overlay = name == null ? null : catalog.GetRaster(id);
                if (overlay == null) return Results.NotFound();

                var grid = GridOf(WorldProjections[name]);
                var size = grid.Width / Math.Pow(2, z);
                var extent = new RectangleShape(grid.MinX + x * size, grid.MaxY - y * size, grid.MinX + (x + 1) * size, grid.MaxY - (y + 1) * size);
                var gate = catalog.LockFor(id);
                await gate.WaitAsync(cancellation);
                try
                {
                    using (var image = new GeoImage(512, 512))
                    {
                        var canvas = GeoCanvas.CreateDefaultGeoCanvas();
                        canvas.BeginDrawing(image, extent, GeographyUnit.Meter);
                        await overlay.DrawAsync(canvas, cancellation);
                        canvas.EndDrawing();
                        return Results.Bytes(image.GetImageBytes(GeoImageFormat.Png), "image/png");
                    }
                }
                finally
                {
                    gate.Release();
                }
            });

            // A star drawn by a PointStyle: the picture a point is drawn from in the browser.
            // Greenland flattened about its own middle by an equal area projection, handed over
            // once. Those metres are what the page puts back down at whatever latitude it is
            // dragged to, through the same projection re-centred there, so the copy always covers
            // the same ground and only Web Mercator's opinion of it changes. The outline is
            // thinned to what a world view can show and the area sent with it is that thinned
            // outline's own, so the page's figures always describe the shape it is drawing.
            app.MapGet("/samples/projections/greenland", () =>
            {
                var shape = Greenland.Value;
                var home = shape.GetCenterPoint();
                var converter = new ProjectionConverter(new Projection(4326), new Projection(EqualArea(home.Y, home.X)));
                converter.Open();
                var flat = (MultipolygonShape)converter.ConvertToExternalProjection(shape);
                converter.Close();
                var thin = flat.Simplify(4000, SimplificationType.DouglasPeucker);

                // Islands smaller than a pixel at world zoom are not worth sending.
                var rings = thin.Polygons
                    .Select(polygon => polygon.OuterRing)
                    .Where(ring => Math.Abs(ring.GetArea(GeographyUnit.Meter, AreaUnit.SquareKilometers)) > 500)
                    .Select(ring => ring.Vertices.Select(vertex => new[] { (long)Math.Round(vertex.X), (long)Math.Round(vertex.Y) }).ToArray())
                    .ToArray();
                var kept = new MultipolygonShape(rings.Select(ring => new PolygonShape(new RingShape(ring.Select(point => new Vertex(point[0], point[1]))))));

                return Results.Json(new
                {
                    home = Math.Round(home.Y, 4),
                    projection = EqualArea("{lat}", home.X),
                    ground = kept.GetArea(GeographyUnit.Meter, AreaUnit.SquareKilometers),
                    rings,
                });
            });

            app.MapGet("/samples/projections/star.png", () =>
                Results.Bytes(Pictures.OfStyle(new PointStyle(PointSymbolType.Star, 24, GeoBrushes.MediumPurple, GeoPens.Purple), 32, 32), "image/png"));
        }

        // The world's extent in a projection, made square about its middle so the tiles stay
        // square at every zoom.
        private static RectangleShape GridOf(string target)
        {
            var world = WorldBoundsOf(target);
            var side = Math.Max(world.Width, world.Height);
            var center = world.GetCenterPoint();
            return new RectangleShape(center.X - side / 2, center.Y + side / 2, center.X + side / 2, center.Y - side / 2);
        }

        // The bounds of the world's outline in a projection, which the browser's map needs as
        // the projection's own extent.
        private static RectangleShape WorldBoundsOf(string target)
        {
            var outline = WorldOutlineOf(target);
            return new RectangleShape(outline.Min(v => v.X), outline.Max(v => v.Y), outline.Max(v => v.X), outline.Min(v => v.Y));
        }

        // The world's outline in a projection: the convex hull of the whole globe, a lattice of
        // points every degree, put through the converter. The hull rather than the dateline and
        // the poles because in an azimuthal projection those run through the middle of the disc,
        // whose rim is the one point opposite the centre. Kept per projection: every tile asks
        // for its grid.
        private static readonly Dictionary<string, Collection<Vertex>> outlines = new Dictionary<string, Collection<Vertex>>();
        private static Collection<Vertex> WorldOutlineOf(string target)
        {
            lock (outlines)
            {
                if (outlines.TryGetValue(target ?? "", out var known)) return known;

                Collection<Vertex> outline;
                if (target == null)
                {
                    var world = MaxExtents.SphericalMercator;
                    outline = new Collection<Vertex> { new Vertex(world.MinX, world.MaxY), new Vertex(world.MaxX, world.MaxY), new Vertex(world.MaxX, world.MinY), new Vertex(world.MinX, world.MinY) };
                }
                else
                {
                    var lattice = new Collection<Vertex>();
                    for (var lat = -90.0; lat <= 90.0; lat += 1)
                        for (var lon = -180.0; lon <= 180.0; lon += 1)
                            lattice.Add(new Vertex(lon, lat));
                    var converter = Converter(4326, target);
                    converter.Open();
                    var projected = converter.ConvertToExternalProjection(lattice);
                    converter.Close();
                    var points = projected.Where(v => !double.IsNaN(v.X) && !double.IsNaN(v.Y) && !double.IsInfinity(v.X) && !double.IsInfinity(v.Y)).Select(v => new PointShape(v));
                    outline = new MultipointShape(points).ConvexHull().Vertices;
                }
                outlines[target ?? ""] = outline;
                return outline;
            }
        }

        private static GdalProjectionConverter Converter(int fromEpsg, string target)
        {
            return target.StartsWith("EPSG:", StringComparison.OrdinalIgnoreCase)
                ? new GdalProjectionConverter(fromEpsg, int.Parse(target.Substring(5)))
                : new GdalProjectionConverter(fromEpsg, target);
        }

        // ThinkGeo Cloud's aerial tiles by API key - any XYZ raster service is drawn the same way.
        private sealed class AerialTilesLayer : WebRasterXyzTileAsyncLayer
        {
            private readonly string apiKey;

            public AerialTilesLayer(string apiKey)
                : base(512, GeographyUnit.Meter, MaxExtents.SphericalMercator)
            {
                this.apiKey = apiKey;
            }

            protected override Task<string> GetImageUriAsyncCore(int zoomLevel, long x, long y, float resolutionFactor)
            {
                return Task.FromResult($"https://cloud.thinkgeo.com/api/v2/maps/raster/aerial/x1/3857/512/{zoomLevel}/{x}/{y}.jpeg?apikey={apiKey}");
            }
        }

        // A great-circle route through the antimeridian, with its 100-nautical-mile corridor,
        // written the way a navigator writes it: continuous longitudes, -118 at Los Angeles past
        // -180 to -238 (122 E) at Shanghai.
        private sealed class Flight
        {
            private const double HalfWorld = 20037508.342789244;
            private const double EarthRadius = 6378137.0;
            private const double CorridorHalfWidthMeters = 100 * 1852.0;

            private static readonly (string Code, string Name, double Lon, double Lat) Origin = ("LAX", "Los Angeles", -118.408, 33.942);
            private static readonly (string Code, string Name, double Lon, double Lat) Destination = ("PVG", "Shanghai Pudong", 121.805, 31.143);

            private readonly (double Lon, double Lat)[] route = GreatCircle(Origin.Lon, Origin.Lat, Destination.Lon, Destination.Lat, 360);

            public Feature[] Airports() => new[] { Airport(Origin), Airport(Destination) };

            public LineShape RouteLine()
            {
                var line = new LineShape();
                foreach (var (lon, lat) in route)
                {
                    var (x, y) = Meters(lon, lat);
                    line.Vertices.Add(new Vertex(x, y));
                }
                return line;
            }

            // The two edges of a band 100 nautical miles either side of the route. The offsets are
            // geodesic and the longitudes stay continuous, like the route's.
            public MultilineShape CorridorEdges()
            {
                var left = new LineShape();
                var right = new LineShape();
                for (var i = 0; i < route.Length; i++)
                {
                    var here = route[i];
                    var heading = i < route.Length - 1 ? Bearing(here, route[i + 1]) : Bearing(route[i - 1], here);
                    var l = Along(here, heading - 90, CorridorHalfWidthMeters);
                    var (lx, ly) = Meters(l.Lon, l.Lat);
                    left.Vertices.Add(new Vertex(lx, ly));
                    var r = Along(here, heading + 90, CorridorHalfWidthMeters);
                    var (rx, ry) = Meters(r.Lon, r.Lat);
                    right.Vertices.Add(new Vertex(rx, ry));
                }
                return new MultilineShape(new[] { left, right });
            }

            private static Feature Airport((string Code, string Name, double Lon, double Lat) airport)
            {
                var (x, y) = Meters(airport.Lon, airport.Lat);
                var feature = new Feature(new PointShape(x, y));
                feature.ColumnValues["LABEL"] = airport.Code + "  " + airport.Name;
                return feature;
            }

            // Points along the great circle, evenly spaced in arc length, with longitudes made
            // continuous from the origin onward (no jump at the antimeridian).
            private static (double Lon, double Lat)[] GreatCircle(double aLon, double aLat, double bLon, double bLat, int count)
            {
                var p = ToUnit(aLon, aLat);
                var q = ToUnit(bLon, bLat);
                var dot = Math.Max(-1, Math.Min(1, p.X * q.X + p.Y * q.Y + p.Z * q.Z));
                var omega = Math.Acos(dot);
                var sinOmega = Math.Sin(omega);
                var result = new (double Lon, double Lat)[count + 1];
                var previousLon = aLon;
                for (var i = 0; i <= count; i++)
                {
                    var t = (double)i / count;
                    var k1 = Math.Sin((1 - t) * omega) / sinOmega;
                    var k2 = Math.Sin(t * omega) / sinOmega;
                    var x = k1 * p.X + k2 * q.X;
                    var y = k1 * p.Y + k2 * q.Y;
                    var z = k1 * p.Z + k2 * q.Z;
                    var lat = Math.Asin(Math.Max(-1, Math.Min(1, z))) * 180 / Math.PI;
                    var lon = Math.Atan2(y, x) * 180 / Math.PI;
                    while (lon - previousLon > 180) lon -= 360;
                    while (lon - previousLon < -180) lon += 360;
                    result[i] = (lon, lat);
                    previousLon = lon;
                }
                return result;
            }

            private static (double X, double Y, double Z) ToUnit(double lon, double lat)
            {
                var la = lat * Math.PI / 180;
                var lo = lon * Math.PI / 180;
                return (Math.Cos(la) * Math.Cos(lo), Math.Cos(la) * Math.Sin(lo), Math.Sin(la));
            }

            private static double Bearing((double Lon, double Lat) a, (double Lon, double Lat) b)
            {
                var la1 = a.Lat * Math.PI / 180;
                var la2 = b.Lat * Math.PI / 180;
                var dLon = (b.Lon - a.Lon) * Math.PI / 180;
                var y = Math.Sin(dLon) * Math.Cos(la2);
                var x = Math.Cos(la1) * Math.Sin(la2) - Math.Sin(la1) * Math.Cos(la2) * Math.Cos(dLon);
                return (Math.Atan2(y, x) * 180 / Math.PI + 360) % 360;
            }

            // The point a distance along a bearing, longitude kept continuous with the start.
            private static (double Lon, double Lat) Along((double Lon, double Lat) from, double bearingDegrees, double meters)
            {
                var la1 = from.Lat * Math.PI / 180;
                var brng = bearingDegrees * Math.PI / 180;
                var delta = meters / EarthRadius;
                var la2 = Math.Asin(Math.Sin(la1) * Math.Cos(delta) + Math.Cos(la1) * Math.Sin(delta) * Math.Cos(brng));
                var dLon = Math.Atan2(Math.Sin(brng) * Math.Sin(delta) * Math.Cos(la1), Math.Cos(delta) - Math.Sin(la1) * Math.Sin(la2));
                return (from.Lon + dLon * 180 / Math.PI, la2 * 180 / Math.PI);
            }

            // Web Mercator meters; a longitude past +/-180 lands past the world edge on purpose.
            private static (double X, double Y) Meters(double lon, double lat)
            {
                var clamped = Math.Max(-85.05, Math.Min(85.05, lat));
                return (lon / 180 * HalfWorld, EarthRadius * Math.Log(Math.Tan(Math.PI / 4 + clamped * Math.PI / 360)));
            }
        }
    }
}
