using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using ThinkGeo.Core;
using ThinkGeo.UI.WebApi;

namespace DisplayNauticalChart
{
    [Route("DisplayNauticalChart")]
    [ApiController]
    public class NauticalChartsController : ControllerBase
    {
        private readonly string nauticalChartsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "App_Data", "US4IL10M.000");
        private static NauticalChartsFeatureLayer? nauticalChartsFeatureLayer;
        private static ProjectionConverter projectionConverter = new ProjectionConverter(4326, 3857);
        private static readonly object layerLock = new object();

        [HttpGet("tile/{z}/{x}/{y}")]        
        public IActionResult GetTile(int z, int x, int y)
        {
            lock (layerLock)
            {
                if (nauticalChartsFeatureLayer == null)
                nauticalChartsFeatureLayer = new NauticalChartsFeatureLayer(nauticalChartsFilePath);            
                { 
                    nauticalChartsFeatureLayer = new NauticalChartsFeatureLayer(nauticalChartsFilePath);
                }
                nauticalChartsFeatureLayer.IsDepthContourTextVisible = true;
                nauticalChartsFeatureLayer.IsLightDescriptionVisible = true;
                nauticalChartsFeatureLayer.StylingType = NauticalChartsStylingType.EmbeddedStyling;
                nauticalChartsFeatureLayer.SymbolTextDisplayMode = NauticalChartsSymbolTextDisplayMode.None;
                nauticalChartsFeatureLayer.DisplayCategory = NauticalChartsDisplayCategory.All;
                nauticalChartsFeatureLayer.DefaultColorSchema = NauticalChartsDefaultColorSchema.DayBright;
                nauticalChartsFeatureLayer.SymbolDisplayMode = NauticalChartsSymbolDisplayMode.Simplified;
                nauticalChartsFeatureLayer.BoundaryDisplayMode = NauticalChartsBoundaryDisplayMode.Plain;
                nauticalChartsFeatureLayer.SafetyDepthInMeter = NauticalChartsFeatureLayer.ConvertDistanceToMeters(28, NauticalChartsDepthUnit.Meter);
                nauticalChartsFeatureLayer.ShallowDepthInMeter = NauticalChartsFeatureLayer.ConvertDistanceToMeters(3, NauticalChartsDepthUnit.Meter);
                nauticalChartsFeatureLayer.DeepDepthInMeter = NauticalChartsFeatureLayer.ConvertDistanceToMeters(10, NauticalChartsDepthUnit.Meter);
                nauticalChartsFeatureLayer.SafetyContourDepthInMeter = NauticalChartsFeatureLayer.ConvertDistanceToMeters(10, NauticalChartsDepthUnit.Meter);
                nauticalChartsFeatureLayer.DrawingMode = NauticalChartsDrawingMode.Optimized;
                nauticalChartsFeatureLayer.IsFullLightLineVisible = true;
                nauticalChartsFeatureLayer.IsMetaObjectsVisible = false;
                nauticalChartsFeatureLayer.FeatureSource.ProjectionConverter = projectionConverter;            
            }

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(nauticalChartsFeatureLayer);

            return DrawLayerOverlay(layerOverlay, z, x, y);
        }

        private IActionResult DrawLayerOverlay(LayerOverlay layerOverlay, int z, int x, int y)
        {
            using (GeoImage bitmap = new GeoImage(256, 256))
            {
                GeoCanvas geoCanvas = GeoCanvas.CreateDefaultGeoCanvas();
                if (geoCanvas == null)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, "Failed to create GeoCanvas.");
                }

                RectangleShape boundingBox = WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, GeographyUnit.Meter);

                lock (layerLock) 
                {
                    geoCanvas.BeginDrawing(bitmap, boundingBox, GeographyUnit.Meter);
                    layerOverlay.Draw(geoCanvas);
                    geoCanvas.EndDrawing();
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, GeoImageFormat.Png);
                    return File(ms.ToArray(), "image/png");
                }
            }
        }
    }
}