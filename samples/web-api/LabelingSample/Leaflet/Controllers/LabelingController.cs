using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using ThinkGeo.Core;
using ThinkGeo.UI.WebApi;

namespace Labeling.Controllers
{
    [Route("label")]
    public class LabelingController : ControllerBase
    {
        [Route("{z}/{x}/{y}")]
        public IActionResult GetBaseMapTile(int z, int x, int y)
        {
            // Create the LayerOverlay for displaying the map.
            LayerOverlay layerOverlay = OverlayBuilder.GetOverlayAsBaseMap();

            return DrawTileImage(layerOverlay, x, y, z);
        }

        [Route("{overlayId}/{z}/{x}/{y}/{accessId}")]
        public IActionResult GetDynamicLayerTile(string overlayId, int z, int x, int y, string accessId)
        {
            // Get layerOverlay specified by the access id passed from client side.
            LayerOverlay layerOverlay = GetLabelingOverlay(overlayId, accessId);

            return DrawTileImage(layerOverlay, x, y, z);
        }

        [Route("GetStyle/{overlayId}/{accessId}")]
        public Dictionary<string, object> GetStyle(string overlayId, string accessId)
        {
            // Get layerOverlay specified by the access id passed from client side.
            LayerOverlay layerOverlay = GetLabelingOverlay(overlayId, accessId);

            Dictionary<string, object> styles = new Dictionary<string, object>();
            switch (overlayId)
            {
                case "LabelStyling":
                    styles = OverlayBuilder.GetLabelingStyle(layerOverlay);
                    break;
                case "LabelingPoints":
                    styles = OverlayBuilder.GetLabelingPointStyle(layerOverlay);
                    break;
                case "LabelingLines":
                    styles = OverlayBuilder.GetLabelingLineStyle(layerOverlay);
                    break;
                case "LabelingPolygons":
                    styles = OverlayBuilder.GetLabelingPolygonStyle(layerOverlay);
                    break;
                case "CustomLabeling":
                    styles = OverlayBuilder.GetCustomLabelingStyle(layerOverlay);
                    break;
                default:
                    break;
            }

            // Return the style defination in JSON saved on server side to client side.
            return styles;
        }

        [Route("Update/{overlayId}/{accessId}")]
        [HttpPost]
        public void UpdateTextStyle(string overlayId, string accessId, [FromForm] string postData)
        {
            // Deserialize the style in JSON format passed from client side.
            Dictionary<string, string> styles = JsonConvert.DeserializeObject<Dictionary<string, string>>(postData);

            // Save the updated labeling style to tempoary folder for a specific acess id, 
            // as the WebAPI service is a REST service, which is stateless. More infomation, 
            // please visit http://en.wikipedia.org/wiki/Representational_state_transfer.
            OverlayBuilder.SaveLabelStyle(overlayId, styles, accessId);
        }

        private IActionResult DrawTileImage(LayerOverlay layerOverlay, int x, int y, int z)
        {
            using (GeoImage image = new GeoImage(256, 256))
            {
                GeoCanvas geoCanvas = GeoCanvas.CreateDefaultGeoCanvas();
                RectangleShape boundingBox = WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, GeographyUnit.Meter);
                geoCanvas.BeginDrawing(image, boundingBox, GeographyUnit.Meter);

                if (layerOverlay != null)
                {
                    layerOverlay.Draw(geoCanvas);
                }
                geoCanvas.EndDrawing();

                byte[] imageBytes = image.GetImageBytes(GeoImageFormat.Png);

                return File(imageBytes, "image/png");
            }
        }

        private static LayerOverlay GetLabelingOverlay(string overlayId, string accessId)
        {
            LayerOverlay selectedOverlay = null;
            switch (overlayId)
            {
                case "LabelStyling":
                    selectedOverlay = OverlayBuilder.GetOverlayWithLabelingStyle("LabelStyling", accessId);
                    break;
                case "LabelingPoints":
                    selectedOverlay = OverlayBuilder.GetOverlayWithLabelingPoint("LabelingPoints", accessId);
                    break;
                case "LabelingLines":
                    selectedOverlay = OverlayBuilder.GetOverlayWithLabelingLine("LabelingLines", accessId);
                    break;
                case "LabelingPolygons":
                    selectedOverlay = OverlayBuilder.GetOverlayWithLabelingPolygon("LabelingPolygons", accessId);
                    break;
                case "CustomLabeling":
                    selectedOverlay = OverlayBuilder.GetOverlayWithCustomLabeling("CustomLabeling", accessId);
                    break;
                default:
                    break;
            }

            return selectedOverlay;
        }
    }
}