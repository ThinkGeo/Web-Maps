using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebApi;

namespace Labeling.Controllers
{
    [RoutePrefix("label")]
    public class LabelingController : ApiController
    {
        [Route("{z}/{x}/{y}")]
        public HttpResponseMessage GetBaseMapTile(int z, int x, int y)
        {
            // Create the LayerOverlay for displaying the map.
            LayerOverlay layerOverlay = OverlayBuilder.GetOverlayAsBaseMap();

            return DrawTileImage(layerOverlay, x, y, z);
        }

        [Route("{overlayId}/{z}/{x}/{y}/{accessId}")]
        public HttpResponseMessage GetDynamicLayerTile(string overlayId, int z, int x, int y, string accessId)
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
        public void UpdateTextStyle(string overlayId, string accessId, [FromBody] string postData)
        {
            // Deserialize the style in JSON format passed from client side.
            Dictionary<string, string> styles = JsonConvert.DeserializeObject<Dictionary<string, string>>(postData);

            // Save the updated labeling style to tempoary folder for a specific acess id, 
            // as the WebAPI service is a REST service, which is stateless. More infomation, 
            // please visit http://en.wikipedia.org/wiki/Representational_state_transfer.
            OverlayBuilder.SaveLabelStyle(overlayId, styles, accessId);
        }

        private static HttpResponseMessage DrawTileImage(LayerOverlay layerOverlay, int x, int y, int z)
        {
            using (Bitmap bitmap = new Bitmap(256, 256))
            {
                PlatformGeoCanvas geoCanvas = new PlatformGeoCanvas();
                RectangleShape boundingBox = WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, GeographyUnit.Meter);
                geoCanvas.BeginDrawing(bitmap, boundingBox, GeographyUnit.Meter);

                if (layerOverlay != null)
                {
                    layerOverlay.Draw(geoCanvas);
                }
                geoCanvas.EndDrawing();

                MemoryStream memoryStream = new MemoryStream();
                bitmap.Save(memoryStream, ImageFormat.Png);

                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK);
                message.Content = new ByteArrayContent(memoryStream.ToArray());
                message.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

                return message;
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