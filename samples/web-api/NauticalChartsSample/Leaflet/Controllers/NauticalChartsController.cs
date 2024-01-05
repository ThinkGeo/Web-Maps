using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using ThinkGeo.Core;
using ThinkGeo.MapSuite.Layers;
using System.Web;
using ThinkGeo.UI.WebApi;

namespace Layers.Controllers
{
    [Route("DisplayNauticalChart")]
    public class NauticalChartsController : ControllerBase
    {
        private static readonly string nauticalChartsFilePath = null;
        private static NauticalChartsFeatureLayer nauticalChartsFeatureLayer;
        private static ProjectionConverter projectionConverter = new ProjectionConverter(4326, 3857);
        private static ProjectionConverter wgs84ToGoogleProjectionConverter;
        private static Object lockObject = new Object();



        static NauticalChartsController()
        {
            nauticalChartsFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "App_Data", "US4IL10M.000");
            wgs84ToGoogleProjectionConverter = new ProjectionConverter(4326, 3857);
        }

        /// <summary>
        /// Load ShapeFileFeatureLayer.
        /// </summary>
        [Route("tile/{z}/{x}/{y}")]
        [HttpGet]
        public HttpResponseMessage GetTile(int z, int x, int y)
        {
            if (nauticalChartsFeatureLayer == null)
                nauticalChartsFeatureLayer = new NauticalChartsFeatureLayer(nauticalChartsFilePath);
            lock (lockObject)
            {
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
                //nauticalChartsFeatureLayer.Open();
            }
            //nauticalChartsFeatureLayer.Open();

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(nauticalChartsFeatureLayer);

            return DrawLayerOverlay(layerOverlay, z, x, y);
        }

        private HttpResponseMessage DrawLayerOverlay(LayerOverlay layerOverlay, int z, int x, int y)
        {
            using (GeoImage bitmap = new GeoImage(256, 256))
            {
                GeoCanvas geoCanvas = GeoCanvas.CreateDefaultGeoCanvas();
                RectangleShape boundingBox = WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, GeographyUnit.Meter);
                geoCanvas.BeginDrawing(bitmap, boundingBox, GeographyUnit.Meter);
                layerOverlay.Layers[0]?.Open();
                layerOverlay.Draw(geoCanvas);
                geoCanvas.EndDrawing();

                MemoryStream ms = new MemoryStream();
                bitmap.Save(ms, GeoImageFormat.Png);

                HttpResponseMessage msg = new HttpResponseMessage(HttpStatusCode.OK);
                msg.Content = new ByteArrayContent(ms.ToArray());
                msg.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

                return msg;
            }
        }


        /// <summary>
        /// Draw the map and return the image back to client in an IActionResult.
        /// </summary>
        private IActionResult DrawTileImage(LayerOverlay layerOverlay, int z, int x, int y)
        {
            using (GeoImage image = new GeoImage(256, 256))
            {
                GeoCanvas geoCanvas = GeoCanvas.CreateDefaultGeoCanvas();
                RectangleShape boundingBox = WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, GeographyUnit.Meter);
                geoCanvas.BeginDrawing(image, boundingBox, GeographyUnit.Meter);
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                layerOverlay.Draw(geoCanvas);
                geoCanvas.EndDrawing();

                byte[] imageBytes = image.GetImageBytes(GeoImageFormat.Png);

                return File(imageBytes, "image/png");
            }
        }
    }
}
