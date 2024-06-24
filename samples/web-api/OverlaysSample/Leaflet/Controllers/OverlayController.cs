using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using ThinkGeo.Core;
using ThinkGeo.UI.WebApi;

namespace ThinkGeo.MapSuite.Overlays
{
    [ApiController]
    [Route("Overlays")]
    public class OverlayController : ControllerBase
    {
        private readonly ILogger<OverlayController> _logger;
        private static string baseDirectory = null;

        public OverlayController(ILogger<OverlayController> logger)
        {
            _logger = logger;
            baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

        /// <summary>
        /// Loads custom overlay.
        /// </summary>
        [Route("LoadCustomOverlay/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult LoadCustomOverlay(int z, int x, int y)
        {
            var customLayers = GetCustomLayers();
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
        public bool ValidateBingMapsKey([FromBody] Dictionary<string, string> postData)
        {
            bool validated = true;
            try
            { 
                // Validate key for BingMaps.
                string loginServiceTemplate = "http://dev.virtualearth.net/REST/v1/Imagery/Metadata/{0}?&incl=ImageryProviders&o=xml&key={1}";
                string loginServiceUri = string.Format(CultureInfo.InvariantCulture, loginServiceTemplate, BingMapsMapType.Road, postData["key"]);

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
        /// Draws the map and return the image back to client in an IActionResult. 
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

        /// <summary>
        /// Get custom layers.
        /// </summary>
        private Collection<Layer> GetCustomLayers()
        {
            var customLayers = new Collection<Layer>();
            ShapeFileFeatureLayer schoolsLayer = new ShapeFileFeatureLayer($@"{baseDirectory}\AppData\POIs\Schools.shp");
            schoolsLayer.Name = "schoolLayer";
            schoolsLayer.Transparency = 200f;
            schoolsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(new GeoImage($@"{baseDirectory}\AppData\Images\School.png"));
            schoolsLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            schoolsLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);
            customLayers.Add(schoolsLayer);

            return customLayers;
        }
    }
}