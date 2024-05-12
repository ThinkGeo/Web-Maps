using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ThinkGeo.Core;
using ThinkGeo.UI.WebApi;


namespace WebSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapServiceController : ControllerBase
    {
        [Route("{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult GetTile(int z, int x, int y)
        {
            // Create the LayerOverlay for displaying the map.
            LayerOverlay capitalOverlay = new LayerOverlay();

            // Create the point layer for capitals.
            ShapeFileFeatureLayer capitalFeatureLayer = new ShapeFileFeatureLayer(Path.Combine(AppContext.BaseDirectory, "../../../Data/capital.shp"));
            // Create the "Mercator projection" and apply it to the layer to match the background map.
            capitalFeatureLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(Projection.GetWgs84ProjString(), Projection.GetSphericalMercatorProjString());

            // Create the point style for positions.
            capitalFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle(PointSymbolType.Circle, 4, new GeoSolidBrush(GeoColor.FromHtml("#21FF00")), new GeoPen(GeoColor.FromHtml("#ffffff"), 1));
            // Create the labeling style for capitals.
            capitalFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = new TextStyle("CITY_NAME", new GeoFont("Arial", 14), new GeoSolidBrush(GeoColor.FromHtml("#21FF00")));
            capitalFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle.YOffsetInPixel = 5;
            capitalFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            // Set DrawingMarginPercentage to a proper value to avoid some labels are cut-off
            capitalFeatureLayer.DrawingMarginInPixel = 300;
            capitalOverlay.Layers.Add("street", capitalFeatureLayer);

            using (GeoImage image = new GeoImage(256, 256))
            {
                GeoCanvas geoCanvas = GeoCanvas.CreateDefaultGeoCanvas();
                RectangleShape boundingBox = WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, GeographyUnit.Meter);
                geoCanvas.BeginDrawing(image, boundingBox, GeographyUnit.Meter);

                if (capitalOverlay != null)
                {
                    capitalOverlay.Draw(geoCanvas);
                }
                geoCanvas.EndDrawing();
                byte[] imageBytes = image.GetImageBytes(GeoImageFormat.Png);

                return File(imageBytes, "image/png");
            }
        }

    }
}
