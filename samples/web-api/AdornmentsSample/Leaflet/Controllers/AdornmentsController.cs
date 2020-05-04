using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;
using System.IO;
using ThinkGeo.Core;
using ThinkGeo.UI.WebApi;

namespace Adornments
{
    [Route("Adornments")]
    public class AdornmentsController : ControllerBase
    {
        private static readonly string baseDirectory;

        static AdornmentsController()
        {
            baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

        [Route("{adornmentType}/{size}/{extent}")]
        [HttpGet]
        public IActionResult RefreshSourceLayer(string adornmentType, Size size, string extent)
        {
            AdornmentsType currentAdornmentType = (AdornmentsType)Enum.Parse(typeof(AdornmentsType), adornmentType);
            string[] extentStrings = extent.Split(',');

            RectangleShape currentExtent = new RectangleShape(Convert.ToDouble(extentStrings[0]), Convert.ToDouble(extentStrings[3]), Convert.ToDouble(extentStrings[2]), Convert.ToDouble(extentStrings[1]));
            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(GetAdornmentLayer(currentAdornmentType));

            return DrawAdornmentImage(layerOverlay, size.Width, size.Height, currentExtent);
        }

        [Route("SchoolShapeFileLayer/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult SchoolShapeFileLayer(int z, int x, int y)
        {
            LayerOverlay layerOverlay = new LayerOverlay();


            string shpFilePathName = $@"{baseDirectory}/App_Data/ShapeFile/Schools.shp";
            string schoolImage = $@"{baseDirectory}/Images/school.png";
            ShapeFileFeatureLayer schoolsLayer = new ShapeFileFeatureLayer(shpFilePathName);
            schoolsLayer.Name = "schoolLayer";
            schoolsLayer.Transparency = 200f;
            schoolsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(new GeoImage(schoolImage));
            schoolsLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            schoolsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);
            layerOverlay.Layers.Add(schoolsLayer);

            return DrawTileImage(layerOverlay, z, x, y);
        }

        private IActionResult DrawAdornmentImage(LayerOverlay layerOverlay, int width, int height, RectangleShape currentExtent)
        {
            using (GeoImage image = new GeoImage(width, height))
            {
                GeoCanvas geoCanvas = GeoCanvas.CreateDefaultGeoCanvas();
                geoCanvas.BeginDrawing(image, currentExtent, GeographyUnit.Meter);
                layerOverlay.Draw(geoCanvas);
                geoCanvas.EndDrawing();

                byte[] imageBytes = image.GetImageBytes(GeoImageFormat.Png);

                return File(imageBytes, "image/png");
            }
        }

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

        private Layer GetAdornmentLayer(AdornmentsType adornmentType)
        {
            Layer adornmentLayer;
            switch (adornmentType)
            {
                case AdornmentsType.ScaleBarAdornment:
                    adornmentLayer = new ScaleBarAdornmentLayer();
                    break;

                case AdornmentsType.ScaleLineAdornment:
                    adornmentLayer = new ScaleLineAdornmentLayer();
                    break;

                case AdornmentsType.ScaleTextAdornment:
                    adornmentLayer = new ScaleTextAdornmentLayer();
                    break;

                case AdornmentsType.LogoAdornment:
                    adornmentLayer = BuildLogoAdornmentLayer();
                    break;

                case AdornmentsType.GraticuleAdornment:
                    adornmentLayer = BuildGraticuleAdornmentLayer();
                    break;

                case AdornmentsType.LegendAdornment:
                    adornmentLayer = BuildLegendAdornmentLayer();
                    break;

                case AdornmentsType.MagneticDeclinationAdornment:
                    adornmentLayer = BuildMagneticDeclinationAdornmentLayer();
                    break;

                default:
                    adornmentLayer = null;
                    break;
            }

            return adornmentLayer;
        }

        private LegendAdornmentLayer BuildLegendAdornmentLayer()
        {
            LegendItem title = new LegendItem();
            title.TextStyle = new TextStyle("Map Legend", new GeoFont("Arial", 10, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColors.Black));

            LegendItem legendItem1 = new LegendItem();
            GeoPen centerPen = new GeoPen(GeoColors.DarkGray, 6);
            centerPen.DashPattern.Add(0.25f);
            centerPen.DashPattern.Add(1);
            legendItem1.ImageStyle = new LineStyle() { InnerPen = new GeoPen(GeoColors.White, 2), OuterPen = new GeoPen(GeoColors.DarkGray, 4), CenterPen = centerPen };
            legendItem1.TextStyle = new TextStyle("Railroad ", new GeoFont("Arial", 8), new GeoSolidBrush(GeoColors.Black));

            LegendItem legendItem2 = new LegendItem();
            legendItem2.ImageStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 167, 204, 149));
            legendItem2.TextStyle = new TextStyle("Parks", new GeoFont("Arial", 8), new GeoSolidBrush(GeoColors.Black));

            LegendItem legendItem3 = new LegendItem();
            string path = string.Format(@"{0}/Images/school.png", baseDirectory);
            legendItem3.ImageStyle = new PointStyle(new GeoImage(path));
            legendItem3.TextStyle = new TextStyle("School", new GeoFont("Arial", 8), new GeoSolidBrush(GeoColors.Black));

            LegendAdornmentLayer legendLayer = new LegendAdornmentLayer();
            legendLayer.BackgroundMask = AreaStyle.CreateLinearGradientStyle(new GeoColor(255, 255, 255, 255), new GeoColor(255, 230, 230, 230), 90, GeoColors.Black);
            legendLayer.LegendItems.Add(legendItem1);
            legendLayer.LegendItems.Add(legendItem2);
            legendLayer.LegendItems.Add(legendItem3);
            legendLayer.Height = 125;
            legendLayer.Title = title;
            legendLayer.Location = AdornmentLocation.LowerLeft;

            return legendLayer;
        }

        private GraticuleFeatureLayer BuildGraticuleAdornmentLayer()
        {
            GraticuleFeatureLayer graticuleAdornmentLayer = new GraticuleFeatureLayer();
            graticuleAdornmentLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);
            graticuleAdornmentLayer.GraticuleLineStyle = new LineStyle(new GeoPen(GeoColor.FromArgb(150, GeoColors.Navy), 1));
            graticuleAdornmentLayer.GraticuleTextFont = new GeoFont("Times", 12, DrawingFontStyles.Bold);
            return graticuleAdornmentLayer;
        }

        private LogoAdornmentLayer BuildLogoAdornmentLayer()
        {
            LogoAdornmentLayer logoAdornmentLayer = new LogoAdornmentLayer();
            string path = $@"{baseDirectory}/Images/ThinkGeoLogo.png";
            logoAdornmentLayer.Location = AdornmentLocation.UpperRight;
            logoAdornmentLayer.Image = new GeoImage(path);

            return logoAdornmentLayer;
        }

        private MagneticDeclinationAdornmentLayer BuildMagneticDeclinationAdornmentLayer()
        {
            MagneticDeclinationAdornmentLayer magneticDeclinationAdornmentLayer = new MagneticDeclinationAdornmentLayer();
            magneticDeclinationAdornmentLayer.Projection = new Projection(Projection.GetGoogleMapProjString());
            magneticDeclinationAdornmentLayer.Location = AdornmentLocation.UpperRight;
            magneticDeclinationAdornmentLayer.TrueNorthPointStyle.SymbolSize = 25;
            magneticDeclinationAdornmentLayer.TrueNorthPointStyle.SymbolType = PointSymbolType.Triangle;
            magneticDeclinationAdornmentLayer.TrueNorthLineStyle.InnerPen.Width = 2f;
            magneticDeclinationAdornmentLayer.TrueNorthLineStyle.OuterPen.Width = 5f;
            magneticDeclinationAdornmentLayer.MagneticNorthLineStyle.InnerPen.Width = 2f;
            magneticDeclinationAdornmentLayer.MagneticNorthLineStyle.OuterPen.Width = 5f;

            return magneticDeclinationAdornmentLayer;
        }
    }
}