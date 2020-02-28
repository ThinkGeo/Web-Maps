using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Newtonsoft.Json;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebApi;

namespace ThinkGeo.MapSuite.Overlays
{
    [RoutePrefix("Overlays")]
    public class OverlayController : ApiController
    {
        private static readonly string baseDirectory;
        private static Collection<Layer> customLayers;

        static OverlayController()
        {
            baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
            InitializeCustomLayers();
        }

        /// <summary>
        /// Loads custom overlay.
        /// </summary>
        [Route("LoadCustomOverlay/{z}/{x}/{y}")]
        [HttpGet]
        public HttpResponseMessage LoadCustomOverlay(int z, int x, int y)
        {
            LayerOverlay layerOverlay = new LayerOverlay();
            // Get custom overlay.
            layerOverlay.Layers.Add(customLayers.Single(l => l.Name == "schoolLayer"));

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Validates key for BingMaps.
        /// </summary>
        [Route("ValidateBingMapsKey")]
        [HttpPost]
        public bool ValidateBingMapsKey([FromBody] string postData)
        {
            bool validated = true;
            try
            {
                // Get bing maps key from post data
                Dictionary<string, string> parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(postData);
                // Validate key for BingMaps.
                string loginServiceTemplate = "http://dev.virtualearth.net/REST/v1/Imagery/Metadata/{0}?&incl=ImageryProviders&o=xml&key={1}";
                string loginServiceUri = string.Format(CultureInfo.InvariantCulture, loginServiceTemplate, BingMapsMapType.Road, parameters["keyStr"]);

                WebRequest request = WebRequest.Create(loginServiceUri);
                request.GetResponse();
            }
            catch (Exception ex)
            {
                validated = false;
            }
            return validated;

        }

        /// <summary>
        /// Draws the map and return the image back to client in an HttpResponseMessage. 
        /// </summary>
        private HttpResponseMessage DrawTileImage(LayerOverlay layerOverlay, int z, int x, int y)
        {
            using (Bitmap bitmap = new Bitmap(256, 256))
            {
                PlatformGeoCanvas geoCanvas = new PlatformGeoCanvas();
                RectangleShape boundingBox = WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, GeographyUnit.Meter);
                geoCanvas.BeginDrawing(bitmap, boundingBox, GeographyUnit.Meter);
                layerOverlay.Draw(geoCanvas);
                geoCanvas.EndDrawing();

                MemoryStream ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Png);

                HttpResponseMessage msg = new HttpResponseMessage(HttpStatusCode.OK);
                msg.Content = new ByteArrayContent(ms.ToArray());
                msg.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

                return msg;
            }
        }

        /// <summary>
        /// Initializes custom layers.
        /// </summary>
        private static void InitializeCustomLayers()
        {
            Proj4Projection wgs84ToGoogleProjection = new Proj4Projection();
            wgs84ToGoogleProjection.InternalProjectionParametersString = Proj4Projection.GetWgs84ParametersString(); //4326
            wgs84ToGoogleProjection.ExternalProjectionParametersString = Proj4Projection.GetGoogleMapParametersString(); //900913
            wgs84ToGoogleProjection.Open();

            customLayers = new Collection<Layer>();
            string shpFilePathName = string.Format(@"{0}/POIs/Schools.shp", baseDirectory);
            string schoolImage = string.Format(@"{0}/Images/school.png", baseDirectory);
            ShapeFileFeatureLayer schoolsLayer = new ShapeFileFeatureLayer(shpFilePathName);
            schoolsLayer.Name = "schoolLayer";
            schoolsLayer.Transparency = 200f;
            schoolsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(new GeoImage(schoolImage));
            schoolsLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            schoolsLayer.FeatureSource.Projection = wgs84ToGoogleProjection;
            customLayers.Add(schoolsLayer);
        }
    }
}