using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using ThinkGeo.Core;
using ThinkGeo.UI.WebApi;

namespace BasicStyling.Controllers
{
    [Route("BasicStyling")]
    public class BasicStylingController : ControllerBase
    {
        static BasicStylingController()
        { }

        [Route("{styleId}/{z}/{x}/{y}/{accessId}")]
        [HttpGet]
        public IActionResult GetDynamicLayerTile(string styleId, int z, int x, int y, string accessId)
        {
            // Create the LayerOverlay for displaying the map with different styles.
            LayerOverlay layerOverlay = GetStyleOverlay(styleId, accessId);

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Gets LayerOverlay with different style query by style id and access id.
        /// </summary>
        /// <param name="styleId">style id</param>
        /// <param name="accessId">access id</param>
        /// <returns></returns>
        private LayerOverlay GetStyleOverlay(string styleId, string accessId)
        {
            LayerOverlay layerOverlay = new LayerOverlay();
            switch (styleId)
            {
                case "PredefinedStyles":
                    foreach (var layer in LayerBuilder.GetPredefinedStyleLayers())
                    {
                        layerOverlay.Layers.Add(layer);
                    }
                    break;

                case "AreaStyle":
                    layerOverlay.Layers.Add(LayerBuilder.GetAreaStyleLayer(styleId, accessId));
                    break;

                case "LineStyle":
                    layerOverlay.Layers.Add(LayerBuilder.GetLineStyleLayer(styleId, accessId));
                    break;

                case "ImagePointStyle":
                    layerOverlay.Layers.Add(LayerBuilder.GeImagePointStyleLayer());
                    break;

                case "SymbolPoint":
                    layerOverlay.Layers.Add(LayerBuilder.GetSymbolPointLayer(styleId, accessId));
                    break;

                case "CharacterPoint":
                    layerOverlay.Layers.Add(LayerBuilder.GetCharacterPointLayer());
                    break;

                case "StyleByZoomLevel":
                    foreach (var layer in LayerBuilder.GetMultipleStyleLayers())
                    {
                        layerOverlay.Layers.Add(layer);
                    }
                    break;

                case "CompoundStyle":
                    layerOverlay.Layers.Add(LayerBuilder.GetCompoundStyleLayer());
                    break;

                default:
                    break;
            }
            return layerOverlay;
        }

        /// <summary>
        /// Gets style options by style id and access id.
        /// </summary>
        [Route("GetStyle/{styleId}/{accessId}")]
        public Dictionary<string, object> GetStyle(string styleId, string accessId)
        {
            Dictionary<string, object> styles = new Dictionary<string, object>();
            switch (styleId)
            {
                case "AreaStyle":
                    styles = LayerBuilder.GetAreaLayerStyle(styleId, accessId);
                    break;

                case "LineStyle":
                    styles = LayerBuilder.GetLineLayerStyle(styleId, accessId);
                    break;

                case "SymbolPoint":
                    styles = LayerBuilder.GetSymbolPointLayerStyle(styleId, accessId);
                    break;
            }
            return styles;
        }

        /// <summary>
        /// Update style to tempoary folder.
        /// </summary>
        [Route("UpdateStyle/{styleId}/{accessId}")]
        public bool UpdateStyle(string styleId, string accessId, [FromBody] string postData)
        {
            // Deserialize the style in JSON format passed from client side.
            Dictionary<string, string> styles = JsonConvert.DeserializeObject<Dictionary<string, string>>(postData);

            // Save the updated style to tempoary folder for a specific acess id and style id.
            LayerBuilder.UpdateLayerStyle(styleId, accessId, styles);

            return true;
        }

        /// <summary>
        /// Draw the map and return the image back to client in an HttpResponseMessage.
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