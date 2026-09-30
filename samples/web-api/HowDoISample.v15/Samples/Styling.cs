using System.Collections.ObjectModel;
using System.Globalization;
using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI.Samples
{
    /// <summary>
    /// Styling: the data behind the style lessons, cut into tiles on the server; the style is
    /// the browser's, edited beside the map.
    /// </summary>
    public class Styling : ISampleGroup
    {
        private static readonly (string Name, string Column, string Unit, double Divisor)[] CensusCategories =
        {
            ("Population", "Population", "people", 1),
            ("Population density", "PopDensity", "per sq mi", 1),
            ("Land area", "AREALAND", "sq km", 1_000_000),
            ("Water area", "AREAWATR", "sq km", 1_000_000),
        };

        public void Register(OverlayCatalog catalog)
        {
            // Three shapefiles in Texas North Central feet (EPSG:2276), reprojected to the map's
            // 3857 as they are cut. Each is the source-layer the style names.
            catalog.Vector("frisco", () =>
            {
                var overlay = new VectorTileOverlay();
                overlay.FeatureSources.Add("Parks", SampleData.Frisco("Parks.shp"));
                overlay.FeatureSources.Add("Streets", SampleData.Frisco("Streets.shp"));
                overlay.FeatureSources.Add("Hotels", SampleData.Frisco("Hotels.shp"));
                return overlay;
            });
            catalog.Vector("countries", () =>
            {
                var overlay = new VectorTileOverlay();
                overlay.FeatureSources.Add("Countries02", new ShapeFileFeatureSource(SampleData.Path("Shapefile/Countries02.shp")) { ProjectionConverter = new ProjectionConverter(4326, 3857) });
                return overlay;
            });
            catalog.Vector("states", () =>
            {
                var overlay = new VectorTileOverlay();
                overlay.FeatureSources.Add("states", new ShapeFileFeatureSource(SampleData.Path("usStatesCensus2010.shp")));
                return overlay;
            });
            catalog.Vector("coyotes", () =>
            {
                var overlay = new VectorTileOverlay();
                overlay.FeatureSources.Add("Frisco_Coyote_Sightings", SampleData.Frisco("Frisco_Coyote_Sightings.shp"));
                return overlay;
            });
            // The label lessons' data, crafted in code rather than loaded.
            catalog.Vector("lessons", () =>
            {
                var overlay = new VectorTileOverlay();
                overlay.FeatureSources.Add("lessons", LabelLessons());
                return overlay;
            });
            catalog.Vector("world", () =>
            {
                var overlay = new VectorTileOverlay();
                overlay.FeatureSources.Add("Countries02", new ShapeFileFeatureSource(SampleData.Path("Shapefile/Countries02.shp")) { ProjectionConverter = new ProjectionConverter(4326, 3857) });
                overlay.FeatureSources.Add("WorldCapitals", new ShapeFileFeatureSource(SampleData.Path("Shapefile/WorldCapitals.shp")) { ProjectionConverter = new ProjectionConverter(4326, 3857) });
                return overlay;
            });
            // The states and, scattered inside each, one dot per 25,000 housing units of each
            // kind: the dots are generated once as data, and the style only colours them.
            catalog.Vector("dots", () =>
            {
                var states = new ShapeFileFeatureSource(SampleData.Path("Shapefile/USStates_3857.shp"));
                states.Open();
                var features = states.GetAllFeatures(new[] { "OWNER_OCC", "RENTER_OCC" });
                states.Close();
                var overlay = new VectorTileOverlay();
                overlay.FeatureSources.Add("USStates_3857", new ShapeFileFeatureSource(SampleData.Path("Shapefile/USStates_3857.shp")));
                overlay.FeatureSources.Add("owner", Scatter(features, "OWNER_OCC", seed: 1));
                overlay.FeatureSources.Add("renter", Scatter(features, "RENTER_OCC", seed: 2));
                return overlay;
            });

            // Five classic FeatureLayers with their ZoomLevelSet styles, drawn on the server;
            // and the same layers put through FeatureLayerTranslator, cut into vector tiles for
            // the browser to draw from the style.json it wrote.
            catalog.Raster("classic-layers", () =>
            {
                var overlay = new LayerOverlay();
                foreach (var layer in Migration.ClassicLayers())
                {
                    overlay.Layers.Add(layer.Name, layer);
                }
                return overlay;
            });
            catalog.Vector("translated", () =>
            {
                var overlay = new VectorTileOverlay();
                foreach (var layer in Migration.ClassicLayers())
                {
                    overlay.FeatureSources.Add(layer.Name, layer.FeatureSource);
                }
                return overlay;
            });
        }

        // One dot per 25,000 units, dropped at random inside the state until enough have landed.
        private static FeatureSource Scatter(IEnumerable<Feature> states, string column, int seed)
        {
            const int unitsPerDot = 25_000;
            var dots = new Collection<Feature>();
            var stateIndex = 0;
            foreach (var state in states)
            {
                stateIndex++;
                if (!(state.GetShape() is AreaBaseShape area) ||
                    !state.ColumnValues.TryGetValue(column, out var raw) ||
                    !double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var units))
                {
                    continue;
                }

                var wanted = (int)Math.Round(units / unitsPerDot);
                var box = area.GetBoundingBox();
                var random = new Random((seed * 7919) + stateIndex);
                var placed = 0;
                for (var attempts = 0; placed < wanted && attempts < wanted * 60; attempts++)
                {
                    var candidate = new PointShape(box.LowerLeftPoint.X + (random.NextDouble() * box.Width), box.LowerLeftPoint.Y + (random.NextDouble() * box.Height));
                    if (!area.Contains(candidate)) continue;
                    dots.Add(new Feature(candidate));
                    placed++;
                }
            }
            return new InMemoryFeatureSource(Array.Empty<FeatureSourceColumn>(), dots);
        }

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            // Simulated sightings, a gaussian scatter tens of kilometres wide around each of
            // nine metros, handed to the browser whole for it to cluster.
            app.MapGet("/samples/styling/sightings", () =>
            {
                var metros = new (double X, double Y)[] { (-8235000, 4970000), (-13158000, 4035000), (-9757000, 5138000), (-10617000, 3455000), (-10778000, 3866000), (-8926000, 2968000), (-13617000, 6042000), (-11686000, 4832000), (-9391000, 3993000) };
                var random = new Random(42);
                double Gaussian() { var u1 = 1.0 - random.NextDouble(); var u2 = 1.0 - random.NextDouble(); return Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Cos(2.0 * Math.PI * u2); }
                var points = new List<BaseShape>();
                foreach (var (mx, my) in metros)
                {
                    var here = 40 + random.Next(45);
                    for (var i = 0; i < here; i++)
                    {
                        points.Add(new PointShape(mx + Gaussian() * 90000, my + Gaussian() * 90000));
                    }
                }
                return Results.Text(GeoJson.Collection(points), "application/json");
            });

            // The marker symbols the Two Ways page pins, each a PointStyle drawn once.

            // Every hatch there is, and one of them as a 32-pixel pattern drawn by AreaStyle.CreateHatchStyle.
            app.MapGet("/samples/styling/hatches", () => Results.Json(Enum.GetNames(typeof(GeoHatchStyle)).OrderBy(name => name, StringComparer.OrdinalIgnoreCase)));
            // The pattern the map fills with is see-through, so the parks show what is under
            // them; the swatch the page picks from asks for the same hatch on white, where the
            // lines read at a glance.
            app.MapGet("/samples/styling/hatch.png", (string name, bool onWhite = false) =>
            {
                if (!Enum.TryParse<GeoHatchStyle>(name, out var hatch)) return Results.NotFound();
                var style = AreaStyle.CreateHatchStyle(hatch, GeoColor.FromHtml("#3F7F45"), GeoColor.FromArgb(110, 180, 224, 182));
                return Results.Bytes(Pictures.OfStyle(style, 32, 32, onWhite ? GeoColors.White : null), "image/png");
            });

            // The style.json FeatureLayerTranslator wrote for the five classic layers, what it
            // could not carry over, and the images it drew for their symbols.
            app.MapGet("/samples/styling/translated-style", () =>
            {
                var (style, warnings) = Migration.Translated.Value;
                return Results.Json(new { style = System.Text.Json.JsonDocument.Parse(style.ToJson()).RootElement, warnings, images = style.ImageEntries.Keys });
            });
            app.MapGet("/samples/styling/translated-image/{name}", (string name) =>
            {
                var (style, _) = Migration.Translated.Value;
                return style.ImageEntries.TryGetValue(name, out var entry) ? Results.Bytes(entry.Image.GetImageBytes(GeoImageFormat.Png), "image/png") : Results.NotFound();
            });

            // The 2010 census columns a choropleth can class, each with the data's own quintiles
            // as its class breaks, so each class holds about a fifth of the states however the
            // values are spread. Read once from the 51 rows.
            app.MapGet("/samples/styling/census-breaks", () =>
            {
                var reader = new ShapeFileFeatureSource(SampleData.Path("usStatesCensus2010.shp"));
                reader.Open();
                var features = reader.GetAllFeatures(ReturningColumnsType.AllColumns);
                reader.Close();
                return Results.Json(CensusCategories.Select(category =>
                {
                    var sorted = features
                        .Select(feature => feature.ColumnValues.TryGetValue(category.Column, out var value) && double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var number) ? number : 0)
                        .OrderBy(value => value)
                        .ToArray();
                    return new
                    {
                        name = category.Name,
                        column = category.Column,
                        unit = category.Unit,
                        divisor = category.Divisor,
                        breaks = new[] { 0.2, 0.4, 0.6, 0.8 }.Select(q => sorted[(int)Math.Round(q * (sorted.Length - 1))]),
                    };
                }));
            });
        }

        // The lessons' data: winding lines for the text to bend along, a grid of points whose
        // positions match the anchors they name, a crowd for priority to thin, long names to
        // wrap, close-set places for text-optional, and a north-running line with a Chinese name.
        private static InMemoryFeatureSource LabelLessons()
        {
            var features = new List<Feature>();

            void Add(BaseShape shape, string kind, string name, string rank = "", string anchor = "")
            {
                var feature = new Feature(shape);
                feature.ColumnValues["KIND"] = kind;
                feature.ColumnValues["NAME"] = name;
                feature.ColumnValues["RANK"] = rank;
                feature.ColumnValues["ANCHOR"] = anchor;
                features.Add(feature);
            }

            // Curved text: two parkways winding east-west.
            Add(Winding(-10786000, 3917500, 1500, 0), "curve", "Meandering Creek Parkway");
            Add(Winding(-10786000, 3914000, 1800, 1.2), "curve", "Old Mill Heritage Trail");

            // Anchors: each point sits in the grid where its own anchor points.
            var anchors = new[]
            {
                new[] { "top-left", "top", "top-right" },
                new[] { "left", "center", "right" },
                new[] { "bottom-left", "bottom", "bottom-right" },
            };
            for (var row = 0; row < 3; row++)
            {
                for (var col = 0; col < 3; col++)
                {
                    var anchor = anchors[row][col];
                    Add(new PointShape(-10770000 + ((col - 1) * 1400), 3916000 - ((row - 1) * 1100)), "anchor", anchor, anchor: anchor);
                }
            }

            // Priority: a spiral of ranked labels, far too many for the space.
            for (var i = 1; i <= 12; i++)
            {
                var angle = i * 2.399963;
                var radius = 350 + (i * 230);
                Add(new PointShape(-10786000 + (radius * Math.Cos(angle)), 3904000 + (radius * Math.Sin(angle))), "rank", "Rank " + i, rank: i.ToString(CultureInfo.InvariantCulture));
            }

            // Wrapping: names longer than anyone wants on one line, and one that is not.
            Add(new PointShape(-10772000, 3904800), "wrap", "Frisco Heritage Botanical Conservatory and Community Arboretum");
            Add(new PointShape(-10768000, 3903600), "wrap", "Panther Creek Environmental Education Center");
            Add(new PointShape(-10770000, 3901800), "wrap", "City Hall");

            // text-optional: places set close enough that not every name can fit.
            var pois = new[] { "Coffee Roastery", "Corner Bakery", "Book Cellar", "Night Market", "Tea House", "Vinyl Shop", "Print Studio", "Flower Cart", "Cheese Shop" };
            for (var i = 0; i < pois.Length; i++)
            {
                var angle = i * 2.399963;
                var radius = 200 + (i * 160);
                Add(new PointShape(-10786000 + (radius * Math.Cos(angle)), 3892000 + (radius * Math.Sin(angle))), "poi", pois[i]);
            }

            // CJK vertical: the line runs north, so the name writes downward.
            Add(WindingNorth(-10770000, 3892000), "cjk", "银杏河滨绿道");

            var columns = new[] { new FeatureSourceColumn("NAME"), new FeatureSourceColumn("KIND"), new FeatureSourceColumn("RANK"), new FeatureSourceColumn("ANCHOR") };
            return new InMemoryFeatureSource(columns, features);
        }

        private static LineShape Winding(double centerX, double centerY, double amplitude, double phase)
        {
            var vertices = new Collection<Vertex>();
            for (var i = 0; i <= 48; i++)
            {
                var t = i / 48.0;
                vertices.Add(new Vertex(centerX - 4200 + (t * 8400), centerY + (amplitude * Math.Sin(phase + (t * Math.PI * 3)))));
            }
            return new LineShape(vertices);
        }

        private static LineShape WindingNorth(double centerX, double centerY)
        {
            var vertices = new Collection<Vertex>();
            for (var i = 0; i <= 48; i++)
            {
                var t = i / 48.0;
                vertices.Add(new Vertex(centerX + (1200 * Math.Sin(t * Math.PI * 2.5)), centerY - 4200 + (t * 8400)));
            }
            return new LineShape(vertices);
        }

        /// <summary>Five classic layers with ZoomLevelSet styles, and their translation into a style document.</summary>
        internal static class Migration
        {
            // Each map reads the shapefiles through layers of its own: a feature source is not
            // shared between a server-side draw and the tile cutter.
            public static IEnumerable<FeatureLayer> ClassicLayers()
            {
                ShapeFileFeatureLayer Layer(string file, string name) =>
                    new ShapeFileFeatureLayer(SampleData.Path("Shapefile/" + file)) { Name = name, FeatureSource = { ProjectionConverter = new ProjectionConverter(2276, 3857) } };

                var zoning = Layer("Zoning.shp", "zoning");
                zoning.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new ValueStyle("ZONING", new Collection<ValueItem>
                {
                    new ValueItem("C-1", new AreaStyle(new GeoSolidBrush(GeoColor.FromHtml("#F2C9B4")))),
                    new ValueItem("I", new AreaStyle(new GeoSolidBrush(GeoColor.FromHtml("#CBC3D8")))),
                    new ValueItem("AG", new AreaStyle(new GeoSolidBrush(GeoColor.FromHtml("#D7E3BE")))),
                    new ValueItem("O-1", new AreaStyle(new GeoSolidBrush(GeoColor.FromHtml("#F4E2A6")))),
                    new ValueItem("O-2", new AreaStyle(new GeoSolidBrush(GeoColor.FromHtml("#F4E2A6")))),
                })
                {
                    DefaultStyle = new AreaStyle(new GeoSolidBrush(GeoColor.FromHtml("#E9E4DA"))),
                });
                zoning.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                var parks = Layer("Parks.shp", "parks");
                parks.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(AreaStyle.CreateHatchStyle(GeoHatchStyle.DiagonalCross, GeoColor.FromHtml("#5E9E62"), GeoColor.FromArgb(110, 180, 224, 182), GeoColor.FromHtml("#3F7F45")));
                parks.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new TextStyle("NAME", new GeoFont("Arial", 12, DrawingFontStyles.Bold | DrawingFontStyles.Underline), new GeoSolidBrush(GeoColors.DarkGreen)) { HaloPen = new GeoPen(GeoColors.White, 2) });
                parks.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                var trails = Layer("Hike_Bike.shp", "trails");
                trails.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new LineStyle(new GeoPen(GeoColor.FromHtml("#8C5A2B"), 2) { DashStyle = LineDashStyle.Dash }));
                trails.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                var streets = Layer("Streets.shp", "streets");
                streets.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new LineStyle(new GeoPen(GeoColors.DimGray, 6), new GeoPen(GeoColors.WhiteSmoke, 4)));
                streets.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                streets.ZoomLevelSet.ZoomLevel17.CustomStyles.Add(new TextStyle("FULL_NAME", new GeoFont("Arial", 11, DrawingFontStyles.Bold | DrawingFontStyles.Strikeout), new GeoSolidBrush(GeoColors.MidnightBlue)) { HaloPen = new GeoPen(GeoColors.White, 2) });
                streets.ZoomLevelSet.ZoomLevel17.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                var schools = Layer("Schools.shp", "schools");
                schools.ZoomLevelSet.ZoomLevel15.CustomStyles.Add(new PointStyle(PointSymbolType.Star, 14, new GeoSolidBrush(GeoColor.FromHtml("#F5B301")), new GeoPen(GeoColor.FromHtml("#8A5A00"), 1)));
                schools.ZoomLevelSet.ZoomLevel15.CustomStyles.Add(new TextStyle("NAME", new GeoFont("Arial", 11, DrawingFontStyles.Bold | DrawingFontStyles.Underline), new GeoSolidBrush(GeoColor.FromHtml("#8A5A00"))) { TextPlacement = TextPlacement.Lower, YOffsetInPixel = 2, HaloPen = new GeoPen(GeoColors.White, 2) });
                schools.ZoomLevelSet.ZoomLevel15.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                yield return zoning;
                yield return parks;
                yield return trails;
                yield return streets;
                yield return schools;
            }

            // One tile source over every layer's data. Each layer translates against it, so the
            // style layers all name the one source the overlay cuts tiles from.
            public static readonly Lazy<(MapStyle Style, List<string> Warnings)> Translated = new Lazy<(MapStyle, List<string>)>(() =>
            {
                var source = new FeatureSourceVectorTileSource { SourceId = "features" };
                var layers = ClassicLayers().ToList();
                foreach (var layer in layers)
                {
                    source.FeatureSources.Add(layer.Name, layer.FeatureSource);
                }
                var style = new MapStyle().SetBackground(GeoColor.FromHtml("#EAE8E2"));
                var warnings = new List<string>();
                var zoomScales = new ZoomLevelSet().GetScales();
                foreach (var layer in layers)
                {
                    var translation = FeatureLayerTranslator.Translate(layer, source, zoomScales);
                    style.AddFeatureLayer(translation);
                    warnings.AddRange(translation.Warnings);
                    warnings.AddRange(translation.JsonWarnings);
                }
                return (style, warnings);
            });
        }
    }
}
