using PdfSharp;
using PdfSharp.Pdf;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Xml.Linq;
using System.Xml.XPath;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebApi;

namespace Printing.Controllers
{
    [RoutePrefix("Printing")]
    public class PrintingController : ApiController
    {
        private static readonly string baseDirectory;

        static PrintingController()
        {
            baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
        }

        /// <summary>
        /// Readys image for a new access.
        /// </summary>
        [Route("PrepareImage/{accessId}")]
        [HttpGet]
        public void PrepareImage(string accessId)
        {
            // Get printing information with default value.
            PrintingInfo printingInfo = GetPrintingInfo(accessId);

            LayerOverlay printerInteractiveOverlay = GetPrinterOverlay(printingInfo);

            Bitmap bitmap = DrawOverlayToBitmap(printerInteractiveOverlay);

            // Save printing layers bitmap.
            string accessIdFolder = Path.Combine(baseDirectory, "Temp", accessId);
            if (!Directory.Exists(accessIdFolder)) Directory.CreateDirectory(accessIdFolder);

            string imagePath = Path.Combine(accessIdFolder, DateTime.Now.ToString("yyyy-MM-dd_hh_mm_ss.fff") + ".png");
            bitmap.Save(imagePath, ImageFormat.Png);
        }

        /// <summary>
        /// Loads printer overlay for printing.
        /// </summary>
        [Route("LoadPrinterOverlay/{z}/{x}/{y}/{accessId}")]
        [HttpGet]
        public HttpResponseMessage LoadPrinterOverlay(int z, int x, int y, string accessId)
        {
            PrintingInfo printingInfo = GetPrintingInfo(accessId);

            PagePrinterLayer pagePrinterLayer = new PagePrinterLayer(printingInfo.PaperSize, printingInfo.Orientation);
            pagePrinterLayer.Open();

            string folder = Path.Combine(baseDirectory, "Temp", accessId);
            string imagePath = Directory.GetFiles(folder, "*.png").LastOrDefault();

            RectangleShape extent = UpdateMapExtent(pagePrinterLayer.GetBoundingBox(), printingInfo.Percentage);

            LayerOverlay layerOverlay = new LayerOverlay();
            NativeImageRasterLayer gdiPlusRasterLayer = new NativeImageRasterLayer(imagePath, extent);
            layerOverlay.Layers.Add(gdiPlusRasterLayer);

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Updates printing information for client operation, including pan, zoom, add/remove printer layer.
        /// </summary>
        [Route("UpdatePrintingInfo/{accessId}/{key}/{value}")]
        [HttpGet]
        public void UpdatePrintingInfo(string accessId, string key, string value)
        {
            PrintingInfo printingInfo = GetPrintingInfo(accessId);

            // Update xml file by key and value.
            string printingInfoFilePath = Path.Combine(baseDirectory, "Temp", accessId, "PrintingInfo.xml");
            if (!File.Exists(printingInfoFilePath)) printingInfo.Save(printingInfoFilePath);

            XDocument xDocument = XDocument.Load(printingInfoFilePath);
            XElement node = xDocument.XPathSelectElement("PrintingInfo/" + key);
            if (node == null)
            {
                node = xDocument.XPathSelectElement("PrintingInfo/MapExtent");
                value = UpdateMapExtent(printingInfo.MapExtent, key).GetWellKnownText();
            }
            if (key == "Percentage") value = string.Format("{0}%", value);
            node.Value = value;

            xDocument.Save(printingInfoFilePath);
        }

        /// <summary>
        /// Creates files for client export.
        /// </summary>
        [Route("Export/{accessId}/{exportType}")]
        [HttpGet]
        public HttpResponseMessage Export(string accessId, string exportType)
        {
            PrintingInfo printingInfo = GetPrintingInfo(accessId);
            LayerOverlay printerInteractiveOverlay = GetPrinterOverlay(printingInfo);

            HttpResponseMessage msg = new HttpResponseMessage(HttpStatusCode.OK);
            if (exportType == "To PDF")
            {
                PdfDocument pdfDocument = new PdfDocument();
                PdfPage pdfPage = pdfDocument.AddPage();
                pdfPage.Orientation = printingInfo.Orientation == PrinterOrientation.Portrait ? PageOrientation.Portrait : PageOrientation.Landscape;
                pdfPage.Size = GetPdfPageSize(printingInfo.PaperSize);

                PdfGeoCanvas pdfGeoCanvas = new PdfGeoCanvas();
                RectangleShape extent = printerInteractiveOverlay.Layers["pagePrinterLayer"].GetBoundingBox();

                pdfGeoCanvas.BeginDrawing(pdfPage, extent, GeographyUnit.Meter);
                printerInteractiveOverlay.Draw(pdfGeoCanvas);
                pdfGeoCanvas.EndDrawing();

                MemoryStream ms = new MemoryStream();
                pdfDocument.Save(ms);
                msg.Content = new ByteArrayContent(ms.ToArray());
                msg.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "PrintingResults.pdf" };
                msg.Content.Headers.ContentType = new MediaTypeHeaderValue("file/pdf");
            }
            else
            {
                // Draw printing layers.
                Bitmap bitmap = DrawOverlayToBitmap(printerInteractiveOverlay);
                MemoryStream ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Png);
                msg.Content = new ByteArrayContent(ms.ToArray());
                msg.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                msg.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") { FileName = "PrintingResults.png" };
            }
            return msg;
        }

        /// <summary>
        /// Draws overlay to bitmap
        /// </summary>
        private static Bitmap DrawOverlayToBitmap(LayerOverlay overlay)
        {
            RectangleShape extent = overlay.Layers["pagePrinterLayer"].GetBoundingBox();

            int width = (int)extent.Width;
            int height = (int)extent.Height;
            // Draw printing layers.
            Bitmap bitmap = new Bitmap(width, height);

            PlatformGeoCanvas geoCanvas = new PlatformGeoCanvas();
            geoCanvas.BeginDrawing(bitmap, extent, GeographyUnit.Meter);
            overlay.Draw(geoCanvas);
            geoCanvas.EndDrawing();
            return bitmap;
        }

        /// <summary>
        /// Gets printer overlay with printing information.
        /// </summary>
        private static LayerOverlay GetPrinterOverlay(PrintingInfo printingInfo)
        {
            LayerOverlay layerOverlay = new LayerOverlay();

            // Create page printer layer.
            PagePrinterLayer pagePrinterLayer = new PagePrinterLayer(printingInfo.PaperSize, printingInfo.Orientation);
            layerOverlay.Layers.Add("pagePrinterLayer", pagePrinterLayer);
            pagePrinterLayer.Open();
            RectangleShape pageBoundingBox = pagePrinterLayer.GetBoundingBox();

            // Create map printer layer.
            MapPrinterLayer mapPrinterLayer = GetMapPrinterLayer(pageBoundingBox);
            mapPrinterLayer.MapExtent = printingInfo.MapExtent;
            layerOverlay.Layers.Add(mapPrinterLayer);

            LabelPrinterLayer titleLabelPrinterLayer = GetTitleLabelPrinterLayer(pageBoundingBox);
            layerOverlay.Layers.Add(titleLabelPrinterLayer);

            // Add Printing layers by selected item.
            if (printingInfo.ScaleBarPrinterLayer)
            {
                ScaleBarPrinterLayer scaleBarPrinterLayer = GetScaleBarPrinterLayer(mapPrinterLayer, pageBoundingBox);
                layerOverlay.Layers.Add(scaleBarPrinterLayer);
            }
            if (printingInfo.ImagePrinterLayer)
            {
                ImagePrinterLayer imagePrinterLayer = GetImagePrinterLayer(pageBoundingBox);
                layerOverlay.Layers.Add(imagePrinterLayer);
            }
            if (printingInfo.ScaleLinePrinterLayer)
            {
                ScaleLinePrinterLayer scaleLinePrinterLayer = GetScaleLinePrinterLayer(mapPrinterLayer, pageBoundingBox);
                layerOverlay.Layers.Add(scaleLinePrinterLayer);
            }
            if (printingInfo.DataGridPrinterLayer)
            {
                DataGridPrinterLayer dataGridPrinterLayer = GetDataGridPrinterLayer(pageBoundingBox);
                layerOverlay.Layers.Add(dataGridPrinterLayer);
            }
            if (printingInfo.LabelPrinterLayer)
            {
                LabelPrinterLayer labelPrinterLayer = GetLabelPrinterLayer(pageBoundingBox);
                layerOverlay.Layers.Add(labelPrinterLayer);
            }

            return layerOverlay;
        }

        /// <summary>
        /// Gets countries layer for map printer layer content.
        /// </summary>
        private static ShapeFileFeatureLayer GetSourceLayer()
        {
            ShapeFileFeatureLayer countriesLayer =
                new ShapeFileFeatureLayer(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data") + "/Countries02.shp");
            countriesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = WorldStreetsAreaStyles.BaseLand();
            countriesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColors.Gray);
            countriesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            countriesLayer.FeatureSource.Projection = new Proj4Projection(Proj4Projection.GetDecimalDegreesParametersString(), Proj4Projection.GetGoogleMapParametersString());
            countriesLayer.Open();
            return countriesLayer;
        }

        /// <summary>
        /// Gets map printer layer.
        /// </summary>
        private static MapPrinterLayer GetMapPrinterLayer(RectangleShape pageBoundingbox)
        {
            MapPrinterLayer mapPrinterLayer = new MapPrinterLayer();
            mapPrinterLayer.BackgroundMask = WorldStreetsAreaStyles.Water();
            mapPrinterLayer.MapUnit = GeographyUnit.Meter;
            mapPrinterLayer.Layers.Add(GetSourceLayer());
            mapPrinterLayer.SetPosition(8, 7, pageBoundingbox.GetCenterPoint().X, pageBoundingbox.GetCenterPoint().Y,
                PrintingUnit.Inch);
            return mapPrinterLayer;
        }

        /// <summary>
        /// Gets title label printer layer.
        /// </summary>
        private static LabelPrinterLayer GetTitleLabelPrinterLayer(RectangleShape pageBoundingbox)
        {
            LabelPrinterLayer labelPrinterLayer = new LabelPrinterLayer();
            labelPrinterLayer.Text = "Population > 150 Million";
            labelPrinterLayer.Font = new GeoFont("Arial", 10, DrawingFontStyles.Bold);
            labelPrinterLayer.TextBrush = new GeoSolidBrush(GeoColor.StandardColors.Black);
            labelPrinterLayer.PrinterWrapMode = PrinterWrapMode.AutoSizeText;

            PointShape labelCenter = new PointShape();
            labelCenter.X = pageBoundingbox.GetCenterPoint().X;
            labelCenter.Y = pageBoundingbox.GetCenterPoint().Y + 3.8;
            labelPrinterLayer.SetPosition(5, 1, labelCenter, PrintingUnit.Inch);

            return labelPrinterLayer;
        }

        /// <summary>
        /// Gets label printer layer.
        /// </summary>
        private static LabelPrinterLayer GetLabelPrinterLayer(RectangleShape pageBoundingbox)
        {
            StringBuilder noteSpace = new StringBuilder();
            noteSpace.Append("Notes: ");
            for (int i = 0; i < 130; i++)
            {
                noteSpace.Append("_ ");
            }

            LabelPrinterLayer labelPrinterLayer = new LabelPrinterLayer();
            labelPrinterLayer.Text = noteSpace.ToString();
            labelPrinterLayer.Font = new GeoFont("Arial", 14, DrawingFontStyles.Regular);
            labelPrinterLayer.TextBrush = new GeoSolidBrush(GeoColor.StandardColors.Black);
            labelPrinterLayer.PrinterWrapMode = PrinterWrapMode.WrapText;

            double noteSpaceWidth = 3.6;

            labelPrinterLayer.SetPosition(noteSpaceWidth, 2, pageBoundingbox.GetCenterPoint().X + 2, pageBoundingbox.GetCenterPoint().Y - 4.5, PrintingUnit.Inch);
            return labelPrinterLayer;
        }

        /// <summary>
        /// Gets image printer layer.
        /// </summary>
        private static ImagePrinterLayer GetImagePrinterLayer(RectangleShape pageBoundingbox)
        {
            GeoImage compassGeoImage =
                new GeoImage(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images") + "/Compass.png");
            ImagePrinterLayer imagePrinterLayer = new ImagePrinterLayer(compassGeoImage);

            imagePrinterLayer.SetPosition(.75, .75, pageBoundingbox.GetCenterPoint().X + 3.5,
                pageBoundingbox.GetCenterPoint().Y - 3, PrintingUnit.Inch);
            return imagePrinterLayer;
        }

        /// <summary>
        /// Gets scale line printer layer.
        /// </summary>
        private static ScaleLinePrinterLayer GetScaleLinePrinterLayer(MapPrinterLayer mapPrinterLayer, RectangleShape pageBoundingbox)
        {
            ScaleLinePrinterLayer scaleLinePrinterLayer = new ScaleLinePrinterLayer(mapPrinterLayer);
            scaleLinePrinterLayer.MapUnit = GeographyUnit.Meter;
            scaleLinePrinterLayer.SetPosition(1.25, .25, pageBoundingbox.GetCenterPoint().X - 3.25,
                pageBoundingbox.GetCenterPoint().Y - 3.25, PrintingUnit.Inch);
            return scaleLinePrinterLayer;
        }

        /// <summary>
        /// Gets scale bar printer layer.
        /// </summary>
        private static ScaleBarPrinterLayer GetScaleBarPrinterLayer(MapPrinterLayer mapPrinterLayer, RectangleShape pageBoundingbox)
        {
            ScaleBarPrinterLayer scaleBarPrinterLayer = new ScaleBarPrinterLayer(mapPrinterLayer);
            scaleBarPrinterLayer.MapUnit = GeographyUnit.Meter;
            scaleBarPrinterLayer.SetPosition(1.25, .25, pageBoundingbox.GetCenterPoint().X - 3.332, pageBoundingbox.GetCenterPoint().Y - 2.75, PrintingUnit.Inch);
            return scaleBarPrinterLayer;
        }

        /// <summary>
        /// Gets data grid printer layer.
        /// </summary>
        private static DataGridPrinterLayer GetDataGridPrinterLayer(RectangleShape pageBoundingbox)
        {
            DataGridPrinterLayer dataGridPrinterLayer = new DataGridPrinterLayer();
            dataGridPrinterLayer.TextFont = new GeoFont("Arial", 8);
            dataGridPrinterLayer.TextHorizontalAlignment = PrinterTextHorizontalAlignment.Left;

            dataGridPrinterLayer.DataTable = new DataTable();
            dataGridPrinterLayer.DataTable.Columns.Add("FIPS_CNTRY");
            dataGridPrinterLayer.DataTable.Columns.Add("CNTRY_NAME");
            dataGridPrinterLayer.DataTable.Columns.Add("LONG_NAME");
            dataGridPrinterLayer.DataTable.Columns.Add("POP_CNTRY");

            var countriesLayer = GetSourceLayer();
            Collection<Feature> features = countriesLayer.QueryTools.GetAllFeatures(ReturningColumnsType.AllColumns);

            int pageSize = 0;
            foreach (Feature feature in features)
            {
                if (pageSize < 11 && int.Parse(feature.ColumnValues["POP_CNTRY"]) > 150000000)
                {
                    pageSize++;

                    dataGridPrinterLayer.DataTable.Rows.Add(feature.ColumnValues["FIPS_CNTRY"], feature.ColumnValues["CNTRY_NAME"], feature.ColumnValues["LONG_NAME"], feature.ColumnValues["POP_CNTRY"]);
                }
            }

            dataGridPrinterLayer.SetPosition(8, 1.75, pageBoundingbox.GetCenterPoint().X, pageBoundingbox.GetCenterPoint().Y - 4.5,
                PrintingUnit.Inch);
            return dataGridPrinterLayer;
        }

        /// <summary>
        /// Gets pdf page size.
        /// </summary>
        private static PageSize GetPdfPageSize(PrinterPageSize pageSize)
        {
            PageSize pdfPageSize = PageSize.Letter;
            switch (pageSize)
            {
                case PrinterPageSize.AnsiA:
                    pdfPageSize = PageSize.Letter;
                    break;
                case PrinterPageSize.AnsiB:
                    pdfPageSize = PageSize.Ledger;
                    break;
                case PrinterPageSize.AnsiC:
                    pdfPageSize = PageSize.A2;
                    break;
                case PrinterPageSize.AnsiD:
                    pdfPageSize = PageSize.A1;
                    break;
                case PrinterPageSize.AnsiE:
                    pdfPageSize = PageSize.A0;
                    break;
            }
            return pdfPageSize;
        }

        /// <summary>
        /// Gets printing information.
        /// </summary>
        private static PrintingInfo GetPrintingInfo(string accessId)
        {
            PrintingInfo printingInfo = new PrintingInfo();
            string printingInfoFilePath = Path.Combine(baseDirectory, "Temp", accessId, "PrintingInfo.xml");
            if (File.Exists(printingInfoFilePath)) printingInfo = PrintingInfo.Load(printingInfoFilePath);

            return printingInfo;
        }

        /// <summary>
        /// Updates map extent, including zoom in/out, pan.
        /// </summary>
        private static RectangleShape UpdateMapExtent(RectangleShape oldExtent, string updateID)
        {
            RectangleShape rectangleShape = oldExtent;
            if (updateID == "ZoomIn")
            {
                rectangleShape = ExtentHelper.ZoomIn(rectangleShape, 30);
            }
            else if (updateID == "ZoomOut")
            {
                rectangleShape = ExtentHelper.ZoomOut(rectangleShape, 30);
            }
            else if (updateID == "N")
            {
                rectangleShape = ExtentHelper.Pan(rectangleShape, PanDirection.Up, 30);
            }
            else if (updateID == "W")
            {
                rectangleShape = ExtentHelper.Pan(rectangleShape, PanDirection.Left, 30);
            }
            else if (updateID == "E")
            {
                rectangleShape = ExtentHelper.Pan(rectangleShape, PanDirection.Right, 30);
            }
            else if (updateID == "S")
            {
                rectangleShape = ExtentHelper.Pan(rectangleShape, PanDirection.Down, 30);
            }
            else if (updateID.Contains("%"))
            {
                int percentage = int.Parse(updateID.Replace("%", ""));

                if (percentage > 100)
                {
                    percentage = percentage - 100;
                    rectangleShape = ExtentHelper.ZoomOut(rectangleShape, percentage);
                }
                else if (percentage < 100)
                {
                    percentage = 100 - percentage;
                    rectangleShape = ExtentHelper.ZoomIn(rectangleShape, percentage);
                }

            }

            return rectangleShape;
        }

        /// <summary>
        ///  Draw the map and return the image back to client in an HttpResponseMessage.
        /// </summary>
        private static HttpResponseMessage DrawTileImage(LayerOverlay layerOverlay, int z, int x, int y)
        {
            using (Bitmap bitmap = new Bitmap(256, 256))
            {
                PlatformGeoCanvas geoCanvas = new PlatformGeoCanvas();
                RectangleShape boundingBox = WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, GeographyUnit.Meter);
                geoCanvas.BeginDrawing(bitmap, boundingBox, GeographyUnit.Meter);
                layerOverlay.Draw(geoCanvas);
                geoCanvas.EndDrawing();

                MemoryStream ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Png);

                HttpResponseMessage msg = new HttpResponseMessage(HttpStatusCode.OK);
                msg.Content = new ByteArrayContent(ms.ToArray());
                msg.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
                return msg;
            }
        }
    }
}
