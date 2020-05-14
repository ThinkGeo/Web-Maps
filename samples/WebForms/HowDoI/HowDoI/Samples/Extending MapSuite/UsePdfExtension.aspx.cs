using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Web.UI;
using PdfSharp;
using PdfSharp.Pdf;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace HowDoI.Samples.Extending_MapSuite
{
    public partial class UsePdfExtension : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.DecimalDegree;
                Map1.CurrentExtent = new RectangleShape(-139.9125, 85.0546875, 151.35703125, -65.94140625);
                Map1.MapBackground = new GeoSolidBrush(GeoColor.GeographicColors.ShallowOcean);

                EcwRasterLayer worldImageLayer = new EcwRasterLayer(MapPath("~/SampleData/world/World.ecw"));

                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02.shp"));
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.StartCap = DrawingLineCap.Round;
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.EndCap = DrawingLineCap.Round;
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                ShapeFileFeatureLayer usStatesLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/USA/STATES.SHP"));
                usStatesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.StandardColors.Transparent, GeoColor.FromArgb(255, 156, 155, 154), 1);
                usStatesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.StartCap = DrawingLineCap.Round;
                usStatesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                ShapeFileFeatureLayer worldCapitalsLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/capital.shp"));
                worldCapitalsLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.StandardColors.White, 6F, GeoColor.StandardColors.Black, 1F);
                worldCapitalsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                ShapeFileFeatureLayer worldCapitalsLabelsLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/capital.shp"));
                worldCapitalsLabelsLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("city_name", "Arial", 13, DrawingFontStyles.Bold, GeoColor.StandardColors.Maroon, 0, -8);
                worldCapitalsLabelsLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle.HaloPen = new GeoPen(GeoColor.StandardColors.White, 2);
                worldCapitalsLabelsLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle.HaloPen.StartCap = DrawingLineCap.Round;
                worldCapitalsLabelsLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle.HaloPen.EndCap = DrawingLineCap.Round;
                worldCapitalsLabelsLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle.SuppressPartialLabels = true;
                worldCapitalsLabelsLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                Map1.StaticOverlay.Layers.Add("WorldImageLayer", worldImageLayer);
                Map1.StaticOverlay.Layers.Add("WorldLayer", worldLayer);
                Map1.StaticOverlay.Layers.Add("USStatesLayer", usStatesLayer);
                Map1.StaticOverlay.Layers.Add("WorldCapitals", worldCapitalsLayer);
                Map1.StaticOverlay.Layers.Add("WorldCapitalsLabels", worldCapitalsLabelsLayer);
            }
        }

        protected void btnToPdf_Click(object sender, EventArgs e)
        {
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            if (rdlOptions.SelectedItem.Text == "Landscape")
            {
                page.Orientation = PageOrientation.Landscape;
            }
            PdfGeoCanvas pdfGeoCanvas = new PdfGeoCanvas();

            // This allows you to control the area in which you want the
            // map to draw in.  Leaving this commented out uses the whole page
            //pdfGeoCanvas.DrawingArea = new Rectangle(200, 50, 400, 400);
            Collection<SimpleCandidate> labelsInLayers = new Collection<SimpleCandidate>();
            foreach (Layer layer in Map1.StaticOverlay.Layers)
            {
                RectangleShape printExtent = ExtentHelper.GetDrawingExtent(Map1.CurrentExtent, (float)Map1.WidthInPixels, (float)Map1.HeightInPixels);
                pdfGeoCanvas.BeginDrawing(page, printExtent, GeographyUnit.DecimalDegree);
                layer.Open();
                layer.Draw(pdfGeoCanvas, labelsInLayers);
                layer.Close();
                pdfGeoCanvas.EndDrawing();
            }

            string filename = Server.MapPath(".") + "\\MapSuite PDF Map.pdf";
            document.Save(filename);
            OpenPdfFile(filename);
        }

        private void OpenPdfFile(string filename)
        {
            string javascriptFormat = "<script language='javascript' type='text/javascript'>{0}</script>";
            try
            {
                ClientScript.RegisterStartupScript(this.GetType(), "showPdfFile", string.Format(javascriptFormat, "function A() {window.open('MapSuite PDF Map.pdf');};A();"));
            }
            catch (Win32Exception ex)
            {
                if (ex.Message == "No application is associated with the specified file for this operation")
                {
                    ClientScript.RegisterStartupScript(this.GetType(), "showPdfFile", string.Format(javascriptFormat, "function A() {alert('Failed to generate PDF file.');};A();"));
                }
            }
        }

    }
}
