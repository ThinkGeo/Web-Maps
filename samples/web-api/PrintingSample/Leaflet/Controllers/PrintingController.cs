using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using ThinkGeo.Core;
using ThinkGeo.UI.WebApi;

namespace Printing.Controllers
{
    [Route("Printing")]
    public class PrintingController : ControllerBase
    {
        private static readonly string baseDirectory;

        static PrintingController()
        {
            baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
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

            GeoImage image = DrawOverlayToImage(printerInteractiveOverlay);

            // Save printing layers bitmap.
            string accessIdFolder = Path.Combine(baseDirectory, "App_Data", "Temp", accessId);
            if (!Directory.Exists(accessIdFolder)) Directory.CreateDirectory(accessIdFolder);

            string imagePath = Path.Combine(accessIdFolder, DateTime.Now.ToString("yyyy-MM-dd_hh_mm_ss.fff") + ".png");
            image.Save(imagePath, GeoImageFormat.Png);
        }

        /// <summary>
        /// Loads printer overlay for printing.
        /// </summary>
        [Route("LoadPrinterOverlay/{z}/{x}/{y}/{accessId}")]
        [HttpGet]
        public IActionResult LoadPrinterOverlay(int z, int x, int y, string accessId)
        {
            PrintingInfo printingInfo = GetPrintingInfo(accessId);


            PagePrinterLayer pagePrinterLayer = new PagePrinterLayer(printingInfo.PaperSize, printingInfo.Orientation);
            pagePrinterLayer.Open();

            string folder = Path.Combine(baseDirectory, "App_Data", "Temp", accessId);
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
            string printingInfoFilePath = Path.Combine(baseDirectory, "App_Data", "Temp", accessId, "PrintingInfo.xml");
            if (!System.IO.File.Exists(printingInfoFilePath))
            {
                printingInfo.Save(printingInfoFilePath);
            }

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
        public IActionResult Export(string accessId, string exportType)
        {
            PrintingInfo printingInfo = GetPrintingInfo(accessId);
            LayerOverlay printerInteractiveOverlay = GetPrinterOverlay(printingInfo);

            // Draw printing layers.
            GeoImage image = DrawOverlayToImage(printerInteractiveOverlay);

            byte[] imageBytes = image.GetImageBytes(GeoImageFormat.Png);
            return File(imageBytes, "application/octet-stream", "Printing Sample.png");

        }

        /// <summary>
        /// Draws overlay to bitmap
        /// </summary>
        private static GeoImage DrawOverlayToImage(LayerOverlay overlay)
        {
            RectangleShape extent = overlay.Layers["pagePrinterLayer"].GetBoundingBox();

            int width = (int)extent.Width;
            int height = (int)extent.Height;
            // Draw printing layers.
            GeoImage image = new GeoImage(width, height);

            GeoCanvas geoCanvas = GeoCanvas.CreateDefaultGeoCanvas();
            geoCanvas.BeginDrawing(image, extent, GeographyUnit.Meter);
            overlay.Draw(geoCanvas);
            geoCanvas.EndDrawing();
            return image;
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
            ShapeFileFeatureLayer countriesLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "App_Data", "Countries02.shp"));
            countriesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoColors.Transparent), new GeoSolidBrush(new GeoColor(255, 250, 247, 243)));
            countriesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColors.Gray);
            countriesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            countriesLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);
            countriesLayer.Open();
            return countriesLayer;
        }

        /// <summary>
        /// Gets map printer layer.
        /// </summary>
        private static MapPrinterLayer GetMapPrinterLayer(RectangleShape pageBoundingbox)
        {
            MapPrinterLayer mapPrinterLayer = new MapPrinterLayer();
            mapPrinterLayer.BackgroundMask = new AreaStyle(new GeoPen(GeoColors.Transparent, 0), new GeoSolidBrush(new GeoColor(255, 160, 207, 235)));
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
            labelPrinterLayer.TextBrush = new GeoSolidBrush(GeoColors.Black);
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
            labelPrinterLayer.TextBrush = new GeoSolidBrush(GeoColors.Black);
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
            GeoImage compassGeoImage = new GeoImage(Path.Combine(baseDirectory, "Images", "Compass.png"));
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
        /// Gets printing information.
        /// </summary>
        private static PrintingInfo GetPrintingInfo(string accessId)
        {
            PrintingInfo printingInfo = new PrintingInfo();
            string printingInfoFilePath = Path.Combine(baseDirectory, "App_Data", "Temp", accessId, "PrintingInfo.xml");
            if (System.IO.File.Exists(printingInfoFilePath))
            {
                printingInfo = PrintingInfo.Load(printingInfoFilePath);
            }
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
                rectangleShape = MapUtil.ZoomIn(rectangleShape, 30);
            }
            else if (updateID == "ZoomOut")
            {
                rectangleShape = MapUtil.ZoomOut(rectangleShape, 30);
            }
            else if (updateID == "N")
            {
                rectangleShape = MapUtil.Pan(rectangleShape, PanDirection.Up, 30);
            }
            else if (updateID == "W")
            {
                rectangleShape = MapUtil.Pan(rectangleShape, PanDirection.Left, 30);
            }
            else if (updateID == "E")
            {
                rectangleShape = MapUtil.Pan(rectangleShape, PanDirection.Right, 30);
            }
            else if (updateID == "S")
            {
                rectangleShape = MapUtil.Pan(rectangleShape, PanDirection.Down, 30);
            }
            else if (updateID.Contains("%"))
            {
                int percentage = int.Parse(updateID.Replace("%", ""));

                if (percentage > 100)
                {
                    percentage = percentage - 100;
                    rectangleShape = MapUtil.ZoomOut(rectangleShape, percentage);
                }
                else if (percentage < 100)
                {
                    percentage = 100 - percentage;
                    rectangleShape = MapUtil.ZoomIn(rectangleShape, percentage);
                }

            }

            return rectangleShape;
        }

        /// <summary>
        ///  Draw the map and return the image back to client in an IActionResult.
        /// </summary>
        private IActionResult DrawTileImage(LayerOverlay layerOverlay, int z, int x, int y)
        {
            using (GeoImage image = new GeoImage(256, 256))
            {
                GeoCanvas geoCanvas = GeoCanvas.CreateDefaultGeoCanvas();
                RectangleShape boundingBox = WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, GeographyUnit.Meter);
                geoCanvas.BeginDrawing(image, boundingBox, GeographyUnit.Meter);
                layerOverlay.Draw(geoCanvas);
                geoCanvas.EndDrawing();

                byte[] imageBytes = image.GetImageBytes(GeoImageFormat.Png);

                return File(imageBytes, "image/png");
            }
        }
    }
}
