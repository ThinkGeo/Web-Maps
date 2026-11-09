using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI.Samples
{
    /// <summary>
    /// Map Tools and Adornments: classic adornment layers - legend, scale line and bar, compass -
    /// drawn on the server as one picture the size of the map, laid over it and drawn again
    /// whenever the view settles; a graticule drawn as tiles; and the popups the browser keeps.
    /// </summary>
    public class Tools : ISampleGroup
    {
        public static readonly (double From, string Colour)[] PopulationClasses = { (0, "#B0C4DE"), (2_000_000, "#87CEFA"), (6_000_000, "#4169E1"), (12_000_000, "#00008B") };

        public void Register(OverlayCatalog catalog)
        {
            // A classic LegendAdornmentLayer, one item per class of the style, with the same colours.
            catalog.Adornment("legend", () =>
            {
                var legend = new LegendAdornmentLayer
                {
                    Title = new LegendItem { TextStyle = new TextStyle("Population 1990", new GeoFont("Arial", 10, DrawingFontStyles.Bold), GeoBrushes.Black) },
                    Location = AdornmentLocation.LowerRight,
                    YOffsetInPixel = -28,   // clear of the map's attribution line
                };
                for (var i = 0; i < PopulationClasses.Length; i++)
                {
                    var text = i == PopulationClasses.Length - 1
                        ? FormattableString.Invariant($"over {PopulationClasses[i].From / 1_000_000:0}M")
                        : FormattableString.Invariant($"{PopulationClasses[i].From / 1_000_000:0}M to {PopulationClasses[i + 1].From / 1_000_000:0}M");
                    legend.LegendItems.Add(new LegendItem
                    {
                        ImageStyle = new AreaStyle(GeoPens.DimGray, new GeoSolidBrush(GeoColor.FromHtml(PopulationClasses[i].Colour))),
                        TextStyle = new TextStyle(text, new GeoFont("Arial", 9), GeoBrushes.Black),
                    });
                }
                var overlay = new AdornmentOverlay();
                overlay.Layers.Add("legend", legend);
                return overlay;
            });

            // The scale line and scale bar in the lower left, one overlay per way the line can
            // read: each unit system, with or without text styles of its own.
            foreach (var unitSystem in Enum.GetValues<ScaleLineUnitSystem>())
            {
                foreach (var styled in new[] { false, true })
                {
                    var units = unitSystem;
                    var withStyles = styled;
                    catalog.Adornment("scale-" + units.ToString().ToLowerInvariant() + (withStyles ? "-styled" : ""), () =>
                    {
                        var projection = new Projection(3857);
                        var scaleLine = new ScaleLineAdornmentLayer
                        {
                            Projection = projection,
                            BackgroundMask = AreaStyle.CreateSimpleAreaStyle(GeoColors.LightBlue, GeoColors.Red),
                            UnitSystem = units,
                        };
                        if (withStyles)
                        {
                            scaleLine.AboveLabelTextStyle = new TextStyle(string.Empty, new GeoFont("Arial", 16, DrawingFontStyles.Italic), GeoBrushes.Blue) { TextPlacement = TextPlacement.Left, YOffsetInPixel = -2 };
                            scaleLine.BelowLabelTextStyle = new TextStyle(string.Empty, new GeoFont("Arial", 16), GeoBrushes.Red) { TextPlacement = TextPlacement.Left, YOffsetInPixel = 2 };
                        }
                        var scaleBar = new ScaleBarAdornmentLayer
                        {
                            YOffsetInPixel = -50,
                            Projection = projection,
                            BackgroundMask = AreaStyle.CreateSimpleAreaStyle(GeoColors.LightBlue, GeoColors.Red),
                        };
                        var overlay = new AdornmentOverlay();
                        overlay.Layers.Add("line", scaleLine);
                        overlay.Layers.Add("bar", scaleBar);
                        return overlay;
                    });
                }
            }

            // True north, magnetic north and the declination between them for the middle of the
            // map, from the World Magnetic Model. Drawn in the middle of a small picture of its
            // own, which the browser pins to a corner and turns with the map, so true north on the
            // picture is true north on the map however the map is turned.
            catalog.Adornment("compass", () =>
            {
                var declination = new MagneticDeclinationAdornmentLayer(AdornmentLocation.Center) { Projection = new Projection(3857) };
                declination.TrueNorthPointStyle.SymbolSize = 25;
                declination.TrueNorthLineStyle.InnerPen.Width = 2f;
                declination.TrueNorthLineStyle.OuterPen.Width = 5f;
                declination.MagneticNorthLineStyle.InnerPen.Width = 2f;
                declination.MagneticNorthLineStyle.OuterPen.Width = 5f;
                var overlay = new AdornmentOverlay();
                overlay.Layers.Add("declination", declination);
                return overlay;
            });

            // A graticule is a feature layer, its lines and labels made for each extent it is
            // asked for; it is drawn tile by tile like any layer.
            catalog.Raster("graticule", () =>
            {
                var graticule = new GraticuleFeatureLayer
                {
                    GraticuleLineStyle = new LineStyle(new GeoPen(GeoColor.FromArgb(150, GeoColors.Navy), 1)),
                    GraticuleTextFont = new GeoFont("Times", 12, DrawingFontStyles.Bold),
                };
                graticule.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);
                var overlay = new LayerOverlay();
                overlay.Layers.Add(graticule);
                return overlay;
            });
        }

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            // The hotels of Frisco, read from a shapefile: one popup each.
            app.MapGet("/samples/tools/hotels", () =>
            {
                var source = SampleData.Frisco("Hotels.shp");
                source.Open();
                var hotels = source.GetAllFeatures(new[] { "NAME" });
                source.Close();
                return Results.Text(GeoJson.Collection(hotels), "application/json");
            });
        }
    }
}
