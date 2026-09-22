using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json.Nodes;
using ThinkGeo.Core;
using ThinkGeo.UI.Blazor;
using ThinkGeo.UI.Blazor.HowDoI.CustomerApp.Classes;
using ThinkGeo.UI.Blazor.HowDoI.CustomerApp.SharedProperties;
using ThinkGeo.UI.Blazor.HowDoI.CustomerApp.Utilities;

namespace ThinkGeo.UI.Blazor.HowDoI.CustomerApp
{
    /// <summary>
    /// The application's own data, built the way the application builds it: its drawn and placed
    /// feature layers from its Map.xml, through its own utilities and its own classic styles. Both
    /// halves of the side by side are given exactly this, so the only difference between the two
    /// pictures is where the drawing happens.
    /// </summary>
    public static class CustomerData
    {
        /// <summary>
        /// The classic layers, styled as the application styles them: what the application ships
        /// today, drawn on the server by a LayerOverlay.
        /// </summary>
        public static GeoCollection<LayerBase> BuildClassicLayers(MapView mapView, string webRootPath, out string summary)
        {
            var runtimeState = new MapRuntimeState { ActiveMapView = mapView };
            var geoUtilities = new MapGeographicUtilities { RuntimeState = runtimeState };
            var measurementUtilities = new MapMeasurementUtilities { RuntimeState = runtimeState, MapGeoUtilities = geoUtilities };
            var layerUtilities = new MapLayerUtilities { RuntimeState = runtimeState };
            var imageUtilities = new MapGeoImageUtilities { RuntimeState = runtimeState };

            // Every utility shares the runtime state and the geographic utility, as the application's
            // MapUtilities wires them; without it a placed feature converts against a map in degrees
            // and lands next to Null Island.
            var conversion = new MapFeatureConversionUtilities
            {
                RuntimeState = runtimeState,
                MapGeoUtilities = geoUtilities,
                MapMeasurementUtilities = measurementUtilities,
                MapGeoImageUtilities = imageUtilities
            };

            imageUtilities.LoadGeoImages(MapConstants.IconsDirectoryName, runtimeState.IconGeoImageIndexMap, runtimeState.IconGeoImageList);
            layerUtilities.CreateDrawnFeaturesLayer();
            layerUtilities.CreatePlacedFeaturesLayer();

            var drawn = runtimeState.DrawnFeaturesLayer;
            var placed = runtimeState.PlacedFeaturesLayer;
            drawn.Name = "drawn";
            placed.Name = "placed";

            var mapPath = Path.Combine(webRootPath, MapConstants.AppDataDirectoryName, MapConstants.MapFileName);
            var saved = XmlUtilities.LoadFromXml<Map>(mapPath) ?? new Map();
            runtimeState.ActiveMapClass = saved;

            foreach (var item in saved.MapDrawnFeatures ?? new Collection<MapDrawnFeature>())
            {
                Add(drawn, conversion.ConvertMapDrawnFeatureToMapFeature(item));
            }
            foreach (var item in saved.MapPlacedFeatures ?? new Collection<MapPlacedFeature>())
            {
                Add(placed, conversion.ConvertMapPlacedFeatureToMapFeature(item));
            }

            summary = drawn.InternalFeatures.Count + " drawn + " + placed.InternalFeatures.Count + " placed";
            return new GeoCollection<LayerBase> { { "drawn", drawn }, { "placed", placed } };
        }

        /// <summary>
        /// The same features as feature sources, keyed by the name the style's layers use - what the
        /// new way draws from. A label point per drawn shape comes along, because a label on a shape
        /// is placed once per shape, not once per tile the shape is cut into.
        /// </summary>
        public static GeoCollection<FeatureSource> BuildSources(GeoCollection<LayerBase> classicLayers)
        {
            var drawn = (InMemoryFeatureLayer)classicLayers["drawn"];
            var placed = (InMemoryFeatureLayer)classicLayers["placed"];

            var labels = new InMemoryFeatureLayer();
            labels.Open();
            foreach (var feature in drawn.InternalFeatures)
            {
                var shape = feature.GetShape();
                if (shape is PointShape || shape is LineShape || shape is MultilineShape) continue;
                labels.InternalFeatures.Add(feature.Id, new Feature(shape.GetCenterPoint(), feature.ColumnValues));
            }
            labels.Close();

            return new GeoCollection<FeatureSource>
            {
                { "drawn", drawn.FeatureSource },
                { "drawn_labels", labels.FeatureSource },
                { "placed", placed.FeatureSource }
            };
        }

        /// <summary>A circle that carries a gradient: its ramp painted, and the ground it covers.</summary>
        public sealed class GradientDisc
        {
            public string Id { get; set; }
            public GeoImage Image { get; set; }
            public RectangleShape Extent { get; set; }
        }

        /// <summary>
        /// The drawn circles, each with the application's own radial brush painted into a picture
        /// the size of its bounding square. The brush is the one the application's value style
        /// gives a circle; it is drawn by the classic canvas, so the ramp is exactly the one the
        /// classic map shows. A circle left out by name gets no ramp.
        /// </summary>
        public static IEnumerable<GradientDisc> GradientDiscs(GeoCollection<LayerBase> classicLayers, string hiddenShapeName = null)
        {
            var drawn = (InMemoryFeatureLayer)classicLayers["drawn"];
            var brush = CircleBrush(drawn);
            var discs = new List<GradientDisc>();
            if (brush == null) return discs;

            foreach (var feature in drawn.InternalFeatures)
            {
                if (!feature.ColumnValues.TryGetValue(MapConstants.IsCircleColumn, out var isCircle) || isCircle != "1") continue;
                if (hiddenShapeName != null && feature.ColumnValues.TryGetValue("Name", out var name) && name == hiddenShapeName) continue;

                var bounds = feature.GetBoundingBox();
                var side = System.Math.Max(bounds.Width, bounds.Height);
                var centre = bounds.GetCenterPoint();
                var square = new RectangleShape(centre.X - side / 2, centre.Y + side / 2, centre.X + side / 2, centre.Y - side / 2);

                // The ramp, painted by the brush the application paints it with: a disc filling
                // the square, transparent around it.
                const int pixels = 256;
                var image = new GeoImage(pixels, pixels);
                var canvas = GeoCanvas.CreateDefaultGeoCanvas();
                canvas.BeginDrawing(image, square, GeographyUnit.Meter);
                var disc = new Feature(new EllipseShape(centre, side / 2, side / 2, GeographyUnit.Meter, DistanceUnit.Meter));
                new AreaStyle(brush).Draw(new[] { disc }, canvas, new Collection<SimpleCandidate>(), new Collection<SimpleCandidate>());
                canvas.EndDrawing();

                discs.Add(new GradientDisc { Id = feature.Id, Image = image, Extent = square });
            }

            return discs;
        }

        /// <summary>The brush the application's value style gives a circle, when it is a gradient.</summary>
        private static GeoBrush CircleBrush(InMemoryFeatureLayer drawn)
        {
            foreach (var style in drawn.ZoomLevelSet.ZoomLevel01.CustomStyles)
            {
                if (!(style is ValueStyle value) || value.ColumnName != MapConstants.IsCircleColumn) continue;
                foreach (var item in value.ValueItems)
                {
                    if (item.Value == "1" && item.DefaultAreaStyle?.FillBrush is GeoRadialGradientBrush radial) return radial;
                }
            }
            return null;
        }

        /// <summary>The picture names the placed features use, each once.</summary>
        public static IEnumerable<string> IconNames(GeoCollection<LayerBase> classicLayers)
        {
            var placed = (InMemoryFeatureLayer)classicLayers["placed"];
            var names = new HashSet<string>();
            foreach (var feature in placed.InternalFeatures)
            {
                if (feature.ColumnValues.TryGetValue(MapConstants.IconNameColumn, out var name) && !string.IsNullOrEmpty(name))
                {
                    names.Add(name);
                }
            }
            return names;
        }

        /// <summary>
        /// The style, said in C#: what the application's classic styles say, in the words the browser
        /// reads. A shape hidden is a filter on the layers that draw it - the data is not touched.
        /// </summary>
        /// <param name="iconsPath">The application's icons folder; each placed feature names its picture by file name.</param>
        /// <param name="iconNames">The picture names the placed features use.</param>
        /// <param name="hiddenShapeName">A drawn shape to leave out, by name; null for none.</param>
        /// <param name="discs">The circles that carry a gradient, each with its ramp painted; see <see cref="GradientDiscs"/>.</param>
        public static StyleDocument BuildStyle(string iconsPath, IEnumerable<string> iconNames, IEnumerable<GradientDisc> discs, string hiddenShapeName = null)
        {
            var polygons = StyleExpressions.GeometryType("Polygon");
            var lines = StyleExpressions.GeometryType("LineString");
            JsonNode keep = hiddenShapeName == null ? null : new JsonArray("!=", StyleExpressions.Get("Name"), hiddenShapeName);
            var drawnPolygons = keep == null ? polygons : StyleExpressions.All(polygons, keep);
            var notACircle = new JsonArray("!=", StyleExpressions.Text("IsCircle"), "1");
            var flatFilled = StyleExpressions.All(drawnPolygons, notACircle);

            var white = GeoColor.FromArgb(255, 255, 255, 255);
            var shadow = GeoColor.FromArgb(153, 0, 0, 0);
            var circleOrBox = StyleExpressions.ColorByValue("IsCircle",
                new[] { new KeyValuePair<string, GeoColor>("True", GeoColor.FromArgb(64, 220, 40, 40)) },
                GeoColor.FromArgb(77, 60, 90, 220));
            var circleOrBoxEdge = StyleExpressions.ColorByValue("IsCircle",
                new[] { new KeyValuePair<string, GeoColor>("True", GeoColor.FromArgb(255, 208, 40, 40)) },
                GeoColor.FromArgb(255, 96, 64, 224));

            var style = new StyleDocument()
                .SetGlyphs("https://cdn.thinkgeo.com/glyphs/1.0.0/{fontstack}/{range}.pbf")
                .AddSource("app", new JsonObject { ["type"] = "vector" })
                // The edge of a polygon is a line layer over the fill, from the same tiles. A
                // fill's own outline strokes every edge of the piece a tile holds, the cut
                // included - a line across every polygon spanning two tiles - where a line
                // layer keeps to the shape's real edges: the tiles are cut with a margin past
                // their border, and a line is clipped at the margin, off the map.
                .AddFillLayer("drawn-fill", "app", "drawn", GeoColor.FromArgb(77, 60, 90, 220), filter: flatFilled,
                    paint: new JsonObject { ["fill-color"] = circleOrBox })
                .AddLineLayer("drawn-outline", "app", "drawn", GeoColor.FromArgb(255, 96, 64, 224), 1.5f, filter: drawnPolygons,
                    paint: new JsonObject { ["line-color"] = circleOrBoxEdge }, slot: StyleLayerSlot.Top)
                .AddLineLayer("drawn-line", "app", "drawn", white, 1.2f, filter: lines)
                .AddSymbolLayer("drawn-line-label", "app", "drawn", "LineLabelName", font: new GeoFont("Noto Sans", 11),
                    textColor: white, haloColor: shadow, haloWidth: 1f, alongLine: true, filter: lines,
                    layout: new JsonObject { ["text-field"] = StyleExpressions.FirstOf("LineLabelName", "LabelName") })
                .AddSymbolLayer("drawn-label", "app", "drawn_labels", "LabelName", font: new GeoFont("Noto Sans", 12),
                    textColor: white, haloColor: shadow, haloWidth: 1f, filter: keep,
                    layout: new JsonObject { ["text-field"] = StyleExpressions.FirstOf("LabelName", "Name") })
                .AddSymbolLayer("placed-icon", "app", "placed", "Name", imageName: "placeholder", font: new GeoFont("Noto Sans", 11),
                    textColor: white, haloColor: shadow, haloWidth: 1f, allowOverlap: true,
                    layout: new JsonObject
                    {
                        // The picture is the one the feature names, the application's own PNG.
                        ["icon-image"] = StyleExpressions.Get("IconName"),
                        ["text-field"] = StyleExpressions.FirstOf("LabelName", "Name")
                    });

            // The circles carry a radial gradient the way the application draws them, and the
            // style specification has no gradient fill: each circle gets its ramp as a picture
            // pinned to its ground, painted by the application's own brush, and the flat fill
            // steps aside where the picture lies.
            foreach (var disc in discs ?? System.Array.Empty<GradientDisc>())
            {
                style.AddImageLayer("ramp-" + disc.Id, disc.Image, disc.Extent, slot: StyleLayerSlot.AboveFills);
            }

            foreach (var name in iconNames ?? System.Array.Empty<string>())
            {
                var path = Path.Combine(iconsPath ?? string.Empty, name + MapConstants.MapImageFileExtension);
                if (File.Exists(path))
                {
                    style.Images.Add(name, new GeoImage(path));
                }
            }

            return style;
        }

        private static void Add(InMemoryFeatureLayer layer, Feature feature)
        {
            if (feature == null || feature.GetShape() == null) return;

            var id = string.IsNullOrEmpty(feature.Id) ? System.Guid.NewGuid().ToString() : feature.Id;
            if (layer.InternalFeatures.Contains(id)) return;

            layer.InternalFeatures.Add(id, feature);
        }

        /// <summary>Where the data lives: it crosses the dateline, so its bounding box is no guide.</summary>
        public static PointShape Center => new PointShape(20037508, 5500000);

        public const int Zoom = 3;
    }
}
