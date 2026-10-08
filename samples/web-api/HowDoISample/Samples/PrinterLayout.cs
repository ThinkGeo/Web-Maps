using System.Collections.Concurrent;
using System.Data;
using System.Globalization;
using System.Text;
using System.Text.Json;
using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI.Samples
{
    /// <summary>
    /// A zoning plan sheet laid out on the server: printer layout layers place the map, the
    /// title block, the legend, the scale bar, the north arrow, the parcel table and the notes
    /// in inches on the paper, so they keep their size and their place whatever paper is
    /// chosen. The map is ThinkGeo Cloud's vector tiles with the zoning over them, both drawn
    /// onto the sheet. It is drawn as a picture for the browser to show, and written to a PDF
    /// page of the paper's own size, where every road, parcel and label is a path or a glyph.
    /// </summary>
    public class PrinterLayout : ISampleGroup
    {
        // The zoning classes the sheet draws and lists, in the order the legend reads.
        private static readonly (string Code, string Name, string Colour)[] Classes =
        {
            ("SF-7", "Single family, 7,000 sq ft", "#F3E1B0"),
            ("SF-8.5", "Single family, 8,500 sq ft", "#EDD48A"),
            ("SF-10", "Single family, 10,000 sq ft", "#E5C566"),
            ("TH", "Townhome", "#E0B183"),
            ("MF-19", "Multifamily", "#D89393"),
            ("C-1", "Commercial", "#E7A0A0"),
            ("O-1", "Office", "#C6B4DC"),
            ("I", "Industrial", "#B2A3C4"),
            ("AG", "Agricultural", "#D7E3BE"),
            ("PH", "Planned development", "#BCD7C1"),
        };
        private const string OtherColour = "#E9E4DA";

        private sealed class SheetInfo
        {
            public RectangleShape MapExtent = ZoningExtent.Value;
            public PrinterPageSize PaperSize = PrinterPageSize.AnsiA;
            public PrinterOrientation Orientation = PrinterOrientation.Landscape;
            public bool Basemap = true;
            public bool TitleBlock = true;
            public bool Legend = true;
            public bool ScaleBar = true;
            public bool NorthArrow = true;
            public bool ParcelTable = true;
            public bool Notes = true;
        }

        private static readonly ConcurrentDictionary<string, SheetInfo> Sheets = new ConcurrentDictionary<string, SheetInfo>(StringComparer.Ordinal);

        // Where the zoning is, read once from the shapefile.
        private static readonly Lazy<RectangleShape> ZoningExtent = new Lazy<RectangleShape>(() =>
        {
            var source = SampleData.Frisco("Zoning.shp");
            source.Open();
            var bounds = source.GetBoundingBox();
            source.Close();
            bounds.ScaleUp(6);
            return bounds;
        });

        // What each class covers, read from the shapefile in its own state plane feet so the
        // acres are the county's own.
        private static readonly Lazy<DataTable> Parcels = new Lazy<DataTable>(() =>
        {
            var table = new DataTable();
            foreach (var column in new[] { "Zoning", "Description", "Parcels", "Acres" }) table.Columns.Add(column);
            var source = new ShapeFileFeatureSource(SampleData.Path("Shapefile/Zoning.shp"));
            source.Open();
            var features = source.GetAllFeatures(new[] { "ZONING" });
            source.Close();
            foreach (var (code, name, _) in Classes)
            {
                var mine = features.Where(feature => feature.ColumnValues.TryGetValue("ZONING", out var value) && value.Trim() == code).ToList();
                if (mine.Count == 0) continue;
                var acres = mine.Sum(feature => ((AreaBaseShape)feature.GetShape()).GetArea(GeographyUnit.Feet, AreaUnit.Acres));
                table.Rows.Add(code, name, mine.Count.ToString(CultureInfo.InvariantCulture), acres.ToString("N0", CultureInfo.InvariantCulture));
            }
            return table;
        });

        public void Register(OverlayCatalog catalog)
        {
        }

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            // One change to the sheet - a layer on or off, the paper, the orientation, or the
            // extent the map was dragged or zoomed to - and it is laid out and drawn again. The
            // answer says where the map sits on the picture and what it shows, which is what the
            // browser needs to turn a drag into an extent.
            app.MapPost("/samples/printer-layout/update/{accessId}", async (string accessId, HttpRequest request) =>
            {
                using var document = await JsonDocument.ParseAsync(request.Body);
                var body = document.RootElement;
                var info = Sheets.GetOrAdd(accessId, _ => new SheetInfo());
                if (body.TryGetProperty("key", out var keyProperty))
                {
                    var key = keyProperty.GetString();
                    var value = body.TryGetProperty("value", out var valueProperty) ? valueProperty.ToString() : "";
                    switch (key)
                    {
                        case "PaperSize": info.PaperSize = Enum.Parse<PrinterPageSize>(value); break;
                        case "Orientation": info.Orientation = Enum.Parse<PrinterOrientation>(value); break;
                        case "Basemap": info.Basemap = value == "true"; break;
                        case "TitleBlock": info.TitleBlock = value == "true"; break;
                        case "Legend": info.Legend = value == "true"; break;
                        case "ScaleBar": info.ScaleBar = value == "true"; break;
                        case "NorthArrow": info.NorthArrow = value == "true"; break;
                        case "ParcelTable": info.ParcelTable = value == "true"; break;
                        case "Notes": info.Notes = value == "true"; break;
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

            // The sheet written straight to a PDF page of the paper's size: every layer draws
            // itself onto a PdfGeoCanvas, so the map is paths and the title block is text.
            app.MapGet("/samples/printer-layout/export/{accessId}", async (string accessId) =>
            {
                var info = Sheets.GetOrAdd(accessId, _ => new SheetInfo());
                try
                {
                    var (layout, _) = await LayoutAsync(info);
                    using var stream = new MemoryStream();
                    await layout.ExportPdfAsync(stream);
                    var bytes = stream.ToArray();
                    return Results.Json(new { url = "export/" + ExportStore.Put(bytes, "application/pdf"), name = "Frisco Zoning Plan.pdf", kilobytes = bytes.Length / 1024 });
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
        private static async Task<(byte[] Bytes, int Width, int Height, (double X, double Y, double Width, double Height) Map, RectangleShape Extent)> PictureAsync(SheetInfo info)
        {
            var (layout, map) = await LayoutAsync(info);
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

        // The sheet: a title block across the top, the map filling what is left of the paper, a
        // legend down the right, the scale bar and north arrow under the map, and the parcel
        // table and notes along the bottom. Every position is in inches on the paper, so the
        // same layout holds on any sheet size.
        private static async Task<(PrinterLayoutDocument Layout, MapPrinterLayoutAsyncLayer Map)> LayoutAsync(SheetInfo info)
        {
            var sheet = new PagePrinterLayoutAsyncLayer(info.PaperSize, info.Orientation);
            var page = new PrinterLayoutPage(sheet);
            var paper = sheet.GetPosition(PrintingUnit.Inch);

            const double margin = 0.45, gap = 0.16, titleHeight = 0.72, legendWidth = 2.1, bottomHeight = 1.5, barHeight = 0.42;
            var left = paper.MinX + margin;
            var right = paper.MaxX - margin;
            var top = paper.MaxY - margin;
            var bottom = paper.MinY + margin;

            // The title block: what makes the sheet a document rather than a picture of a map.
            if (info.TitleBlock)
            {
                var title = new LabelPrinterLayoutAsyncLayer("City of Frisco, Texas\nZoning Plan", new GeoFont("Arial", 15, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColors.Black))
                {
                    PrinterWrapMode = PrinterWrapMode.WrapText,
                };
                title.SetPosition(right - left, titleHeight, (left + right) / 2, top - titleHeight / 2, PrintingUnit.Inch);
                page.Layers.Add(title);
            }

            var mapTop = info.TitleBlock ? top - titleHeight - gap : top;
            var mapRight = info.Legend ? right - legendWidth - gap : right;
            var mapBottom = bottom + (info.ParcelTable || info.Notes ? bottomHeight + gap : 0) + (info.ScaleBar || info.NorthArrow ? barHeight + gap : 0);

            // The layers of the map itself, bottom first: the basemap's vector tiles, then the
            // zoning over them. Both draw onto whatever canvas the sheet is drawn to, so both
            // are paths and glyphs on the PDF.
            var inside = new List<LayerBase>();
            if (info.Basemap) inside.Add(await BasemapAsync());
            inside.Add(Zoning(info.Basemap));
            var map = new MapPrinterLayoutAsyncLayer(inside, info.MapExtent, GeographyUnit.Meter)
            {
                BackgroundMask = new AreaStyle(new GeoPen(GeoColors.DimGray, 1), new GeoSolidBrush(GeoColor.FromHtml("#FBFAF7"))),
            };
            map.SetPosition(mapRight - left, mapTop - mapBottom, (left + mapRight) / 2, (mapTop + mapBottom) / 2, PrintingUnit.Inch);
            page.Layers.Add(map);

            if (info.Legend)
            {
                var legend = new LegendPrinterLayoutAsyncLayer
                {
                    Title = new LegendItem { TextStyle = new TextStyle("Zoning", new GeoFont("Arial", 10, DrawingFontStyles.Bold), GeoBrushes.Black) },
                    BackgroundMask = new AreaStyle(new GeoPen(GeoColors.DimGray, 1), new GeoSolidBrush(GeoColors.White)),
                };
                foreach (var (code, name, colour) in Classes)
                {
                    legend.LegendItems.Add(new LegendItem
                    {
                        ImageStyle = new AreaStyle(new GeoPen(GeoColors.DimGray, 1), new GeoSolidBrush(GeoColor.FromHtml(colour))),
                        TextStyle = new TextStyle(code + "  " + name, new GeoFont("Arial", 7), GeoBrushes.Black),
                    });
                }
                legend.SetPosition(legendWidth, mapTop - mapBottom, right - legendWidth / 2, (mapTop + mapBottom) / 2, PrintingUnit.Inch);
                page.Layers.Add(legend);
            }

            if (info.ScaleBar)
            {
                var scaleBar = new ScaleBarPrinterLayoutAsyncLayer(map) { MapUnit = GeographyUnit.Meter };
                scaleBar.SetPosition(1.6, barHeight, left + 0.8, mapBottom - gap - barHeight / 2, PrintingUnit.Inch);
                page.Layers.Add(scaleBar);
            }
            if (info.NorthArrow)
            {
                var compass = new ImagePrinterLayoutAsyncLayer(new GeoImage(SampleData.Path("Images/Compass.png")));
                compass.SetPosition(barHeight, barHeight, mapRight - barHeight / 2, mapBottom - gap - barHeight / 2, PrintingUnit.Inch);
                page.Layers.Add(compass);
            }
            if (info.ParcelTable)
            {
                var width = info.Notes ? (right - left) * 0.62 : right - left;
                var grid = new DataGridPrinterLayoutAsyncLayer(Parcels.Value, new GeoFont("Arial", 7)) { TextHorizontalAlignment = PrinterTextHorizontalAlignment.Left };
                grid.SetPosition(width, bottomHeight, left + width / 2, bottom + bottomHeight / 2, PrintingUnit.Inch);
                page.Layers.Add(grid);
            }
            if (info.Notes)
            {
                var width = info.ParcelTable ? (right - left) * 0.36 : right - left;
                var notes = new LabelPrinterLayoutAsyncLayer(Note(), new GeoFont("Arial", 7), new GeoSolidBrush(GeoColors.Black))
                {
                    PrinterWrapMode = PrinterWrapMode.WrapText,
                    BackgroundMask = new AreaStyle(new GeoPen(GeoColors.DimGray, 1), new GeoSolidBrush(GeoColors.White)),
                };
                notes.SetPosition(width, bottomHeight, right - width / 2, bottom + bottomHeight / 2, PrintingUnit.Inch);
                page.Layers.Add(notes);
            }

            var layout = new PrinterLayoutDocument();
            layout.Pages.Add(page);
            return (layout, map);
        }

        // ThinkGeo Cloud's vector tiles styled by MvtTilesAsyncLayer; the style asks for its
        // tiles with a {key} placeholder in their address, which the key fills.
        private static async Task<MvtTilesAsyncLayer> BasemapAsync()
        {
            var basemap = new MvtTilesAsyncLayer(ThinkGeoVectorStyles.Light) { TimeoutInSeconds = 60 };
            basemap.SendingHttpRequest += (_, args) =>
            {
                var address = args.HttpRequestMessage.RequestUri.ToString().Replace("{key}", GlobalSettings.ThinkGeoApiKey).Replace("%7Bkey%7D", GlobalSettings.ThinkGeoApiKey);
                args.HttpRequestMessage.RequestUri = new Uri(address);
            };
            await basemap.OpenAsync();
            return basemap;
        }

        // Frisco's zoning, drawn by a style.json of one fill layer whose colour comes from the
        // parcel's own class - the same colours the legend lists.
        private static StyledLayer Zoning(bool overBasemap)
        {
            var match = new StringBuilder("[\"match\",[\"get\",\"ZONING\"]");
            foreach (var (code, _, colour) in Classes) match.Append(",\"" + code + "\",\"" + colour + "\"");
            match.Append(",\"" + OtherColour + "\"]");
            // Over the basemap the fills let the streets through; on bare paper they are solid.
            var opacity = overBasemap ? "0.6" : "1";
            var layers = "[{\"type\":\"fill\",\"source-layer\":\"zoning\",\"paint\":{\"fill-color\":" + match + ",\"fill-opacity\":" + opacity + ",\"fill-outline-color\":\"#8C8C8C\"}}]";
            return new StyledLayer(layers, new[] { new KeyValuePair<string, FeatureSource>("zoning", SampleData.Frisco("Zoning.shp")) });
        }

        private static string Note() =>
            "NOTES\n1. Zoning as adopted; see the zoning ordinance for the controlling text.\n" +
            "2. Parcel boundaries are for reference and are not a survey.\n" +
            "3. Areas are in acres, computed in Texas State Plane North Central (EPSG:2276).\n" +
            "4. Sheet drawn " + DateTime.Now.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + ".";
    }
}
