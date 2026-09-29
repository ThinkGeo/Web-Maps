using System.Globalization;
using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI.Samples
{
    /// <summary>
    /// An S-57 chart of Chicago Harbor: drawn by the NauticalChartsFeatureLayer with its embedded
    /// S-52 styling on the server, tile by tile; and read by the server for a ship under way, which
    /// asks at every step what the chart says about the water it is in - the depth area, the aids
    /// to navigation and the hazards nearby, the restricted areas - and gets its under-keel
    /// clearance and a warning back. The browser only has the picture; the chart's objects and
    /// attributes live on the server.
    /// </summary>
    public class NauticalCharts : ISampleGroup
    {
        private static readonly HashSet<string> Aids = new HashSet<string> { "LIGHTS", "BOYLAT", "BOYSAW", "BOYSPP", "BOYCAR", "BOYISD", "BCNLAT", "BCNSPP", "BCNCAR", "BCNISD", "DAYMAR", "LNDMRK" };
        private static readonly HashSet<string> Hazards = new HashSet<string> { "UWTROC", "WRECKS", "OBSTRN" };

        // The chart, open once for the questions the helm asks; a source answers one at a time.
        private static readonly Lazy<NauticalChartsFeatureSource> Chart = new Lazy<NauticalChartsFeatureSource>(() =>
        {
            var chart = new NauticalChartsFeatureSource(SampleData.Path("Legacy/NauticalCharts/US4IL10M.000")) { ProjectionConverter = new ProjectionConverter(4326, 3857) };
            chart.Open();
            return chart;
        });
        private static readonly object Helm = new object();

        public void Register(OverlayCatalog catalog)
        {
            catalog.Raster("nautical-chart", () =>
            {
                var chart = new NauticalChartsFeatureLayer(SampleData.Path("Legacy/NauticalCharts/US4IL10M.000"))
                {
                    IsDepthContourTextVisible = true,
                    IsLightDescriptionVisible = true,
                    StylingType = NauticalChartsStylingType.EmbeddedStyling,
                    SymbolTextDisplayMode = NauticalChartsSymbolTextDisplayMode.None,
                    DisplayCategory = NauticalChartsDisplayCategory.All,
                    DefaultColorSchema = NauticalChartsDefaultColorSchema.DayBright,
                    SymbolDisplayMode = NauticalChartsSymbolDisplayMode.Simplified,
                    BoundaryDisplayMode = NauticalChartsBoundaryDisplayMode.Plain,
                    DrawingMode = NauticalChartsDrawingMode.Optimized,
                    IsFullLightLineVisible = true,
                    IsMetaObjectsVisible = false,
                };
                // The depths that decide the colour of the water, in meters.
                chart.SafetyDepthInMeter = NauticalChartsFeatureLayer.ConvertDistanceToMeters(28, NauticalChartsDepthUnit.Meter);
                chart.ShallowDepthInMeter = NauticalChartsFeatureLayer.ConvertDistanceToMeters(3, NauticalChartsDepthUnit.Meter);
                chart.DeepDepthInMeter = NauticalChartsFeatureLayer.ConvertDistanceToMeters(10, NauticalChartsDepthUnit.Meter);
                chart.SafetyContourDepthInMeter = NauticalChartsFeatureLayer.ConvertDistanceToMeters(10, NauticalChartsDepthUnit.Meter);
                // The chart is in latitude and longitude; the map is in meters.
                chart.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);
                var overlay = new LayerOverlay();
                overlay.Layers.Add(chart);
                return overlay;
            });
        }

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            // What the chart says about one place, for a ship of the draft given: the depth area
            // it is in, the under-keel clearance that leaves, the aids to navigation within 800 m
            // and the hazards within 500 m, each with its distance, and any restricted area it is
            // in. Every object comes off the chart's own S-57 data, named by the S-57 catalogue.
            app.MapGet("/samples/nautical/helm", (double lng, double lat, double draft) =>
            {
                var here = GeoJson.Point(lng, lat);
                lock (Helm)
                {
                    var chart = Chart.Value;
                    var around = chart.GetFeaturesWithinDistanceOf(here, GeographyUnit.Meter, DistanceUnit.Meter, 800, ReturningColumnsType.AllColumns);
                    double? shallowest = null, deepest = null;
                    var restricted = new List<object>();
                    var aids = new List<object>();
                    var hazards = new List<object>();
                    var nearestHazard = double.MaxValue;
                    foreach (var feature in around)
                    {
                        var kind = feature.ColumnValues.TryGetValue("OBJCLS", out var acronym) ? acronym : "";
                        var shape = feature.GetShape();
                        if (kind == "DEPARE" && shape is AreaBaseShape area && area.Contains(here))
                        {
                            if (Number(feature, "DRVAL1") is double low && (shallowest == null || low < shallowest)) shallowest = low;
                            if (Number(feature, "DRVAL2") is double high && (deepest == null || high > deepest)) deepest = high;
                        }
                        else if (kind == "RESARE" && shape is AreaBaseShape zone && zone.Contains(here))
                        {
                            restricted.Add(new { name = Name(chart, feature), details = Details(chart, feature) });
                        }
                        else if (Aids.Contains(kind))
                        {
                            aids.Add(new { name = Name(chart, feature), distance = Math.Round(here.GetDistanceTo(shape, GeographyUnit.Meter, DistanceUnit.Meter)) });
                        }
                        else if (Hazards.Contains(kind))
                        {
                            var distance = here.GetDistanceTo(shape, GeographyUnit.Meter, DistanceUnit.Meter);
                            if (distance > 500) continue;
                            nearestHazard = Math.Min(nearestHazard, distance);
                            hazards.Add(new { name = Name(chart, feature), distance = Math.Round(distance) });
                        }
                    }
                    var underKeel = shallowest == null ? (double?)null : Math.Round(shallowest.Value - draft, 1);
                    var status = shallowest == null ? "unknown" : underKeel < 0 ? "danger" : underKeel < 2 || nearestHazard < 300 ? "caution" : "ok";
                    return Results.Json(new
                    {
                        status,
                        depth = shallowest == null ? null : new { min = shallowest, max = deepest },
                        underKeel,
                        aids = aids.OrderBy(aid => ((dynamic)aid).distance).Take(3),
                        hazards = hazards.OrderBy(hazard => ((dynamic)hazard).distance).Take(3),
                        restricted,
                    });
                }
            });

            // Where the chart is, for the map to start at.
            app.MapGet("/samples/nautical/bounds", () =>
            {
                lock (Helm)
                {
                    return Results.Json(GeoJson.Bounds(Chart.Value.GetBoundingBox()));
                }
            });
        }

        private static double? Number(Feature feature, string column) =>
            feature.ColumnValues.TryGetValue(column, out var text) && double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ? value : (double?)null;

        // What a restricted area restricts, and the rule behind it, in the catalogue's words.
        private static string Details(NauticalChartsFeatureSource chart, Feature feature)
        {
            var description = chart.GetFeatureDescription(feature);
            if (description == null) return "";
            var told = new[] { "CATREA", "RESTRN", "INFORM" };
            return string.Join("; ", description.Attributes.Where(attribute => told.Contains(attribute.Acronym) && !string.IsNullOrWhiteSpace(attribute.Value)).Select(attribute => attribute.Name + ": " + attribute.Value));
        }

        // The object's class name from the S-57 catalogue - "Light", "Lateral buoy", "Underwater
        // rock" - and its own name when the chart gives it one.
        private static string Name(NauticalChartsFeatureSource chart, Feature feature)
        {
            var description = chart.GetFeatureDescription(feature);
            var kind = description?.ObjectClassName ?? (feature.ColumnValues.TryGetValue("OBJCLS", out var acronym) ? acronym : "Object");
            return feature.ColumnValues.TryGetValue("OBJNAM", out var own) && !string.IsNullOrWhiteSpace(own) ? kind + " " + own.Trim() : kind;
        }
    }
}
