using System.Collections.Concurrent;
using System.Data;
using System.Globalization;
using System.Text.Json;
using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI.Samples
{
    /// <summary>
    /// A page laid out on the server with printer layout layers -
    /// the sheet, a map of the world's countries, a title, and the scale line, scale bar,
    /// compass image, data grid and note space you choose to add - drawn as a picture and
    /// exported as PNG or PDF. Each page keeps its own settings under an access id.
    /// </summary>
    public class PrinterLayout : ISampleGroup
    {
        private sealed class PrintingInfo
        {
            public RectangleShape MapExtent = new RectangleShape(-20037508.2314698, 18418382.1370691, 20037508.2314698, -20037508.2314698);
            public PrinterPageSize PaperSize = PrinterPageSize.AnsiA;
            public PrinterOrientation Orientation = PrinterOrientation.Portrait;
            public bool LabelPrinterLayer;
            public bool ImagePrinterLayer;
            public bool ScaleLinePrinterLayer;
            public bool ScaleBarPrinterLayer;
            public bool DataGridPrinterLayer;
        }

        private static readonly ConcurrentDictionary<string, PrintingInfo> Settings = new ConcurrentDictionary<string, PrintingInfo>(StringComparer.Ordinal);

        public void Register(OverlayCatalog catalog)
        {
        }

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            // One change to the page's settings - a layer added or removed, the paper, the
            // orientation, a zoom or the extent the map was dragged or zoomed to - and the page
            // drawn again. The answer says where the map sits on the picture and what it shows,
            // which is what the browser needs to turn a drag into an extent.
            app.MapPost("/samples/printer-layout/update/{accessId}", async (string accessId, HttpRequest request) =>
            {
                using var document = await JsonDocument.ParseAsync(request.Body);
                var body = document.RootElement;
                var info = Settings.GetOrAdd(accessId, _ => new PrintingInfo());
                if (body.TryGetProperty("key", out var keyProperty))
                {
                    var key = keyProperty.GetString();
                    var value = body.TryGetProperty("value", out var valueProperty) ? valueProperty.ToString() : "";
                    switch (key)
                    {
                        case "PaperSize": info.PaperSize = Enum.Parse<PrinterPageSize>(value); break;
                        case "Orientation": info.Orientation = Enum.Parse<PrinterOrientation>(value); break;
                        case "LabelPrinterLayer": info.LabelPrinterLayer = value == "true"; break;
                        case "ImagePrinterLayer": info.ImagePrinterLayer = value == "true"; break;
                        case "ScaleLinePrinterLayer": info.ScaleLinePrinterLayer = value == "true"; break;
                        case "ScaleBarPrinterLayer": info.ScaleBarPrinterLayer = value == "true"; break;
                        case "DataGridPrinterLayer": info.DataGridPrinterLayer = value == "true"; break;
                        case "Percentage": info.MapExtent = Zoomed(info.MapExtent, int.Parse(value)); break;
                        case "Extent":
                            var v = value.Split(',').Select(part => double.Parse(part, CultureInfo.InvariantCulture)).ToArray();
                            if (v.Length == 4) info.MapExtent = new RectangleShape(v[0], v[3], v[2], v[1]);
                            break;
                    }
                }
                try
                {
                    var (bytes, width, height, map, extent) = await PictureAsync(info);
                    return Results.Json(new
                    {
                        url = "export/" + ExportStore.Put(bytes, "image/png"),
                        width,
                        height,
                        map = new { x = map.X, y = map.Y, width = map.Width, height = map.Height },
                        extent = new[] { extent.MinX, extent.MinY, extent.MaxX, extent.MaxY },
                    });
                }
                catch (Exception exception)
                {
                    return Results.Problem(exception.Message, statusCode: 500);
                }
            });

            // The page as a file: the picture as PNG, or the layout written straight to a PDF
            // page of the paper's size.
            app.MapGet("/samples/printer-layout/export/{accessId}", async (string accessId, string type) =>
            {
                var info = Settings.GetOrAdd(accessId, _ => new PrintingInfo());
                try
                {
                    if (type == "pdf")
                    {
                        var layout = Layout(info);
                        using var stream = new MemoryStream();
                        var canvas = new PdfGeoCanvas();
                        canvas.SetPageSize(layout.Pages[0].Page);
                        canvas.BeginDrawing(stream, layout.Pages[0].Page.GetPosition(PrintingUnit.Point), GeographyUnit.Meter);
                        await layout.DrawAsync(canvas);
                        canvas.EndDrawing();
                        return Results.Json(new { url = "export/" + ExportStore.Put(stream.ToArray(), "application/pdf"), name = "Printing Sample.pdf" });
                    }
                    var (bytes, _, _, _, _) = await PictureAsync(info);
                    return Results.Json(new { url = "export/" + ExportStore.Put(bytes, "image/png"), name = "Printing Sample.png" });
                }
                catch (Exception exception)
                {
                    return Results.Problem(exception.Message, statusCode: 500);
                }
            });
        }

        // The whole sheet drawn into a picture, one pixel per point; with it, the map's place on
        // the picture in pixels from the top left, and the extent the map shows once fitted to
        // its frame.
        private static async Task<(byte[] Bytes, int Width, int Height, (double X, double Y, double Width, double Height) Map, RectangleShape Extent)> PictureAsync(PrintingInfo info)
        {
            var layout = Layout(info, out var map);
            layout.IncludePageBackground = true;
            var sheet = layout.Pages[0].Page.GetPosition(PrintingUnit.Point);
            var width = (int)sheet.Width;
            var height = (int)sheet.Height;
            using var image = new GeoImage(width, height);
            var canvas = GeoCanvas.CreateDefaultGeoCanvas();
            canvas.BeginDrawing(image, sheet, GeographyUnit.Meter);
            await layout.DrawAsync(canvas);
            canvas.EndDrawing();
            // A layer answers for its position only while open; the layout closes it after the draw.
            if (!map.IsOpen) await map.OpenAsync();
            var frame = map.GetPosition(PrintingUnit.Point);
            await map.CloseAsync();
            // Pixels count down the picture from its top left; the page's points count up from its bottom left.
            var onPicture = (X: frame.MinX - sheet.MinX, Y: sheet.MaxY - frame.MaxY, Width: frame.Width, Height: frame.Height);
            var shown = MapUtil.GetDrawingExtent(info.MapExtent, (float)frame.Width, (float)frame.Height);
            return (image.GetImageBytes(GeoImageFormat.Png), width, height, onPicture, shown);
        }

        private static PrinterLayoutDocument Layout(PrintingInfo info) => Layout(info, out _);

        // The layout: the sheet, the map, the title, and the layers the page asked for, each
        // placed in inches from the sheet's centre.
        private static PrinterLayoutDocument Layout(PrintingInfo info, out MapPrinterLayoutAsyncLayer map)
        {
            var sheet = new PagePrinterLayoutAsyncLayer(info.PaperSize, info.Orientation);
            var page = new PrinterLayoutPage(sheet);
            var center = sheet.GetPosition(PrintingUnit.Inch).GetCenterPoint();

            map = new MapPrinterLayoutAsyncLayer(new LayerBase[] { Countries() }, info.MapExtent, GeographyUnit.Meter)
            {
                BackgroundMask = new AreaStyle(new GeoPen(GeoColors.Transparent, 0), new GeoSolidBrush(new GeoColor(255, 160, 207, 235))),
            };
            map.SetPosition(8, 7, center.X, center.Y, PrintingUnit.Inch);
            page.Layers.Add(map);

            var title = new LabelPrinterLayoutAsyncLayer("Population > 150 Million", new GeoFont("Arial", 10, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColors.Black)) { PrinterWrapMode = PrinterWrapMode.AutoSizeText };
            title.SetPosition(5, 1, center.X, center.Y + 3.8, PrintingUnit.Inch);
            page.Layers.Add(title);

            if (info.ScaleBarPrinterLayer)
            {
                var scaleBar = new ScaleBarPrinterLayoutAsyncLayer(map) { MapUnit = GeographyUnit.Meter };
                scaleBar.SetPosition(1.25, .25, center.X - 3.332, center.Y - 2.75, PrintingUnit.Inch);
                page.Layers.Add(scaleBar);
            }
            if (info.ImagePrinterLayer)
            {
                var compass = new ImagePrinterLayoutAsyncLayer(new GeoImage(SampleData.Path("Images/Compass.png")));
                compass.SetPosition(.75, .75, center.X + 3.5, center.Y - 3, PrintingUnit.Inch);
                page.Layers.Add(compass);
            }
            if (info.ScaleLinePrinterLayer)
            {
                var scaleLine = new ScaleLinePrinterLayoutAsyncLayer(map) { MapUnit = GeographyUnit.Meter };
                scaleLine.SetPosition(1.25, .25, center.X - 3.25, center.Y - 3.25, PrintingUnit.Inch);
                page.Layers.Add(scaleLine);
            }
            if (info.DataGridPrinterLayer)
            {
                var grid = new DataGridPrinterLayoutAsyncLayer(BigCountries(), new GeoFont("Arial", 8)) { TextHorizontalAlignment = PrinterTextHorizontalAlignment.Left };
                grid.SetPosition(8, 1.75, center.X, center.Y - 4.5, PrintingUnit.Inch);
                page.Layers.Add(grid);
            }
            if (info.LabelPrinterLayer)
            {
                var notes = new LabelPrinterLayoutAsyncLayer("Notes: " + string.Concat(Enumerable.Repeat("_ ", 130)), new GeoFont("Arial", 14), new GeoSolidBrush(GeoColors.Black)) { PrinterWrapMode = PrinterWrapMode.WrapText };
                notes.SetPosition(3.6, 2, center.X + 2, center.Y - 4.5, PrintingUnit.Inch);
                page.Layers.Add(notes);
            }

            var layout = new PrinterLayoutDocument();
            layout.Pages.Add(page);
            return layout;
        }

        // The countries of the world, in degrees, drawn in the map's meters by a style.json of
        // one fill layer: a StyledLayer draws a style on any canvas, the printer's included.
        private const string CountriesStyle = "[{\"type\":\"fill\",\"source-layer\":\"countries\",\"paint\":{\"fill-color\":\"#FAF7F3\",\"fill-outline-color\":\"#808080\"}}]";

        private static StyledLayer Countries() =>
            new StyledLayer(CountriesStyle, new[] { new KeyValuePair<string, FeatureSource>("countries", new ShapeFileFeatureSource(SampleData.Path("Shapefile/Countries02.shp")) { ProjectionConverter = new ProjectionConverter(4326, 3857) }) });

        // The countries of more than 150 million people, eleven at most, as the grid's rows.
        private static DataTable BigCountries()
        {
            var table = new DataTable();
            foreach (var column in new[] { "FIPS_CNTRY", "CNTRY_NAME", "LONG_NAME", "POP_CNTRY" })
            {
                table.Columns.Add(column);
            }
            var countries = new ShapeFileFeatureSource(SampleData.Path("Shapefile/Countries02.shp"));
            countries.Open();
            foreach (var feature in countries.GetAllFeatures(ReturningColumnsType.AllColumns))
            {
                if (table.Rows.Count >= 11 || !long.TryParse(feature.ColumnValues["POP_CNTRY"], out var population) || population <= 150_000_000) continue;
                table.Rows.Add(feature.ColumnValues["FIPS_CNTRY"], feature.ColumnValues["CNTRY_NAME"], feature.ColumnValues["LONG_NAME"], feature.ColumnValues["POP_CNTRY"]);
            }
            countries.Close();
            return table;
        }

        private static RectangleShape Zoomed(RectangleShape extent, int percentage)
        {
            if (percentage > 100) return MapUtil.ZoomOut(extent, percentage - 100);
            if (percentage < 100) return MapUtil.ZoomIn(extent, 100 - percentage);
            return extent;
        }
    }
}
