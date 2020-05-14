using PdfSharp;
using PdfSharp.Pdf;
using System.Collections.ObjectModel;
using System.Web;
using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace CSharp_HowDoISamples
{
    public partial class MiscellaneousController : Controller
    {
        //
        // GET: /UsePdfExtension/

        public ActionResult UsePdfExtension()
        {
            return View();
        }

        [MapActionFilter]
        public string ToPdf(Map map, GeoCollection<object> args)
        {
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            string selectedItemText = args[0].ToString();
            if (selectedItemText == "Landscape")
            {
                page.Orientation = PageOrientation.Landscape;
            }
            PdfGeoCanvas pdfGeoCanvas = new PdfGeoCanvas();

            // This allows you to control the area in which you want the
            // map to draw in.  Leaving this commented out uses the whole page
            //pdfGeoCanvas.DrawingArea = new Rectangle(200, 50, 400, 400);
            Collection<SimpleCandidate> labelsInLayers = new Collection<SimpleCandidate>();
            foreach (Layer layer in map.StaticOverlay.Layers)
            {
                RectangleShape printExtent = ExtentHelper.GetDrawingExtent(map.CurrentExtent, (float)map.WidthInPixels, (float)map.HeightInPixels);
                pdfGeoCanvas.BeginDrawing(page, printExtent, GeographyUnit.DecimalDegree);
                layer.Open();
                layer.Draw(pdfGeoCanvas, labelsInLayers);
                layer.Close();
                pdfGeoCanvas.EndDrawing();
            }

            string relativePath = string.Format("~/Controllers/{0}/{1}", ControllerContext.RouteData.Values["Controller"], "MapSuite PDF Map.pdf");
            string filename = Server.MapPath(relativePath);
            document.Save(filename);

            return VirtualPathUtility.ToAbsolute(relativePath);
        }
    }
}
