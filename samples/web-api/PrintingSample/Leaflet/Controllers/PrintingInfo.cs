using System;
using System.Xml.Linq;
using ThinkGeo.Core;

namespace Printing.Controllers
{
    public class PrintingInfo
    {
        private RectangleShape mapExtent;
        private bool labelPrinterLayer;
        private bool imagePrinterLayer;
        private bool scaleLinePrinterLayer;
        private bool scaleBarPrinterLayer;
        private bool dataGridPrinterLayer;
        private PrinterPageSize paperSize;
        private PrinterOrientation orientation;
        private string percentage;

        public RectangleShape MapExtent
        {
            get { return mapExtent; }
            set { mapExtent = value; }
        }


        public PrinterPageSize PaperSize
        {
            get { return paperSize; }
            set { paperSize = value; }
        }

        public PrinterOrientation Orientation
        {
            get { return orientation; }
            set { orientation = value; }
        }

        public string Percentage
        {
            get { return percentage; }
            set { percentage = value; }
        }

        public bool LabelPrinterLayer
        {
            get { return labelPrinterLayer; }
            set { labelPrinterLayer = value; }
        }

        public bool ImagePrinterLayer
        {
            get { return imagePrinterLayer; }
            set { imagePrinterLayer = value; }
        }

        public bool ScaleLinePrinterLayer
        {
            get { return scaleLinePrinterLayer; }
            set { scaleLinePrinterLayer = value; }
        }

        public bool ScaleBarPrinterLayer
        {
            get { return scaleBarPrinterLayer; }
            set { scaleBarPrinterLayer = value; }
        }

        public bool DataGridPrinterLayer
        {
            get { return dataGridPrinterLayer; }
            set { dataGridPrinterLayer = value; }
        }

        public PrintingInfo()
        {
            mapExtent = new RectangleShape("POLYGON((-20037508.2314698 18418382.1370691,-20037508.2314698 -20037508.2314698,20037508.2314698 -20037508.2314698,20037508.2314698 18418382.1370691,-20037508.2314698 18418382.1370691))");

            paperSize = PrinterPageSize.AnsiA;
            orientation = PrinterOrientation.Portrait;
            percentage = "100%";
        }

        /// <summary>
        /// Saves printng information to xml file.
        /// </summary>
        internal void Save(string printingInfoFilePath)
        {
            XDocument xDoc = new XDocument(new XElement("PrintingInfo",
               new XElement("MapExtent", mapExtent.GetWellKnownText()),
               new XElement("LabelPrinterLayer", labelPrinterLayer),
               new XElement("ImagePrinterLayer", imagePrinterLayer),
               new XElement("ScaleLinePrinterLayer", imagePrinterLayer),
               new XElement("ScaleBarPrinterLayer", imagePrinterLayer),
               new XElement("DataGridPrinterLayer", imagePrinterLayer),
               new XElement("PaperSize", paperSize),
               new XElement("Orientation", orientation),
               new XElement("Percentage", percentage)
               ));
            xDoc.Save(printingInfoFilePath);
        }

        /// <summary>
        /// Load printing information value from xml file.
        /// </summary>
        internal static PrintingInfo Load(string printingInfoFilePath)
        {
            PrintingInfo printingInfo = new PrintingInfo();

            XElement element = XElement.Load(printingInfoFilePath);

            printingInfo.MapExtent = new RectangleShape(element.Element("MapExtent").Value);
            printingInfo.LabelPrinterLayer = bool.Parse(element.Element("LabelPrinterLayer").Value);
            printingInfo.ImagePrinterLayer = bool.Parse(element.Element("ImagePrinterLayer").Value);
            printingInfo.ScaleLinePrinterLayer = bool.Parse(element.Element("ScaleLinePrinterLayer").Value);
            printingInfo.ScaleBarPrinterLayer = bool.Parse(element.Element("ScaleBarPrinterLayer").Value);
            printingInfo.DataGridPrinterLayer = bool.Parse(element.Element("DataGridPrinterLayer").Value);
            printingInfo.PaperSize = (PrinterPageSize)Enum.Parse(typeof(PrinterPageSize), element.Element("PaperSize").Value);
            printingInfo.Orientation = (PrinterOrientation)Enum.Parse(typeof(PrinterOrientation), element.Element("Orientation").Value);
            printingInfo.Percentage = element.Element("Percentage").Value;
            return printingInfo;
        }
    }
}