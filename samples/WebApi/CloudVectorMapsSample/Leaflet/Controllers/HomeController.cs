using System;
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
using ThinkGeo.MapSuite.WebApi;

namespace ThinkGeoCloudVectorMapsSample.Controllers
{
    [RoutePrefix("ThinkGeoCloudVectorMaps")]
    public class HomeController : ApiController
    {
        private const string cloudServiceClientId = "Your-ThinkGeo-Cloud-Service-Cliend-ID";    // Get it from https://cloud.thinkgeo.com
        private const string cloudServiceClientSecret = "Your-ThinkGeo-Cloud-Service-Cliend-Secret";

        private static readonly string baseDirectory;

        static HomeController()
        {
            baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        }

        [Route("tile/{mapType}/{z}/{x}/{y}")]
        [HttpGet]
        public HttpResponseMessage GetTile(string mapType, int z, int x, int y)
        {

            LayerOverlay layerOverlay = new LayerOverlay();
            ThinkGeoCloudVectorMapsFeatureLayer thinkGeoCloudVectorMapsFeatureLayer = null;

            if (mapType == "light")
            {
                thinkGeoCloudVectorMapsFeatureLayer = new ThinkGeoCloudVectorMapsFeatureLayer(cloudServiceClientId, cloudServiceClientSecret);
            }
            else if (mapType == "dark")
            {
                thinkGeoCloudVectorMapsFeatureLayer = new ThinkGeoCloudVectorMapsFeatureLayer(cloudServiceClientId, cloudServiceClientSecret, ThinkGeoCloudVectorMapsMapType.Dark);
            }
            else if (mapType == "transparentBackground")
            {
                thinkGeoCloudVectorMapsFeatureLayer = new ThinkGeoCloudVectorMapsFeatureLayer(cloudServiceClientId, cloudServiceClientSecret, ThinkGeoCloudVectorMapsMapType.TransparentBackground);
            }
            else if (mapType == "custom")
            {
                string styleJsonFilePath = Path.Combine(baseDirectory, "App_Data", "mutedblue.json");
                thinkGeoCloudVectorMapsFeatureLayer = new ThinkGeoCloudVectorMapsFeatureLayer(cloudServiceClientId, cloudServiceClientSecret, new Uri(styleJsonFilePath, UriKind.Relative));
            }

            var bitmapTileCache = new XyzFileBitmapTileCache(Path.Combine(baseDirectory, "Cache", "ImageTileCache"), $"{thinkGeoCloudVectorMapsFeatureLayer.MapType}_{3857}", new ThinkGeoCloudMapsZoomLevelSet(), new MapSuiteTileMatrix(295829515.16123772, 512, 512, GeographyUnit.Meter, thinkGeoCloudVectorMapsFeatureLayer.GetTileMatrixBoundingBox()));
            thinkGeoCloudVectorMapsFeatureLayer.BitmapTileCache = bitmapTileCache;
            var vectorTileCache = new FileVectorTileCache(Path.Combine(baseDirectory, "Cache", "VectorTileCache"), "3857");
            thinkGeoCloudVectorMapsFeatureLayer.VectorTileCache = vectorTileCache;

            layerOverlay.MaxExtent = thinkGeoCloudVectorMapsFeatureLayer.GetTileMatrixBoundingBox();
            layerOverlay.Layers.Add(thinkGeoCloudVectorMapsFeatureLayer);

            return DrawTileImage(layerOverlay, x, y, z);
        }

        private static HttpResponseMessage DrawTileImage(LayerOverlay layerOverlay, int x, int y, int z)
        {
            using (Bitmap bitmap = new Bitmap(512, 512))
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

                HttpResponseMessage message = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new ByteArrayContent(memoryStream.ToArray())
                };
                message.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

                return message;
            }
        }
    }
}
