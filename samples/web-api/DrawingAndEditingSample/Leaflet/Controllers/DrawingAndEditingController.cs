using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;
using ThinkGeo.Core;
using ThinkGeo.UI.WebApi;

namespace DrawingAndEditing.Controllers
{
    [Route("edit")]
    public class DrawingAndEditingController : ControllerBase
    {
        private static readonly string baseDirectory = null;
        private static string jsonArrayTemplate = "[{0}]";

        static DrawingAndEditingController()
        {
            baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        }

        [Route("{z}/{x}/{y}/{accessId}")]
        public IActionResult GetTile(int z, int x, int y, string accessId)
        {
            // Create the layerOverlay for displaying the map.
            LayerOverlay layerOverlay = new LayerOverlay();
            // Get the featureLayer including all drawn shapes by the access id for display.
            InMemoryFeatureLayer drawnShapesFeatureLayer = GetDrawnShapesFeatureLayer(accessId);
            if (drawnShapesFeatureLayer != null && drawnShapesFeatureLayer.InternalFeatures.Count > 0)
            {
                layerOverlay.Layers.Add(drawnShapesFeatureLayer);
            }

            // Draw the map and return the image back to client in an IActionResult.
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

        [Route("getshapes/{accessId}")]
        public string GetShapes(string accessId)
        {
            // Get the featureLayer including all drawn shapes by the access id.
            InMemoryFeatureLayer shapesFeatureLayer = GetDrawnShapesFeatureLayer(accessId);
            // Return the shapes in JSON saved on server side to client side.
            return string.Format(CultureInfo.InvariantCulture, jsonArrayTemplate, string.Join(",", shapesFeatureLayer.InternalFeatures.Select(f => f.GetGeoJson())));
        }

        [Route("saveshapes/{accessId}")]
        [HttpPost]
        [Consumes("application/x-www-form-urlencoded")]
        public HttpResponseMessage SaveShapes(string accessId, [FromForm] string modifiedShapesInJson)
        {
            try
            {
                if (!string.IsNullOrEmpty(modifiedShapesInJson))
                {
                    // Parse the JSON.
                    // There are 2 groups of shapes, one is for shapes removed on client side,
                    // and the other is for shapes added on client side.
                    JObject jObject = JObject.Parse(modifiedShapesInJson);

                    InMemoryFeatureLayer shapesFeatureLayer = GetDrawnShapesFeatureLayer(accessId);
                    shapesFeatureLayer.Open();

                    // Deal with removed shapes.
                    string[] ids = jObject["removedIds"].ToObject<string[]>();
                    foreach (var id in ids)
                    {
                        if (shapesFeatureLayer.InternalFeatures.Count > 0)
                        {
                            shapesFeatureLayer.InternalFeatures.Remove(id);
                        }
                    }

                    // Deal with newly added shapes.
                    string featureGeoJsons = jObject["newShapes"].ToString();
                    foreach (Feature feature in CreateFeaturesFromGeoJsons(featureGeoJsons))
                    {
                        shapesFeatureLayer.InternalFeatures.Add(feature.Id, feature);
                    }
                    shapesFeatureLayer.Close();

                    // Update the local saved features for a specific access id.
                    SaveFeatures(accessId, shapesFeatureLayer.InternalFeatures);
                }

                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                return responseMessage;
            }
            catch (Exception exception)
            {
                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = new StringContent(exception.ToString(), Encoding.UTF8, "text/plain");
                return responseMessage;
            }
        }

        private static Collection<Feature> CreateFeaturesFromGeoJsons(string geoJsons)
        {
            Collection<Feature> features = new Collection<Feature>();
            foreach (JToken item in JArray.Parse(geoJsons))
            {
                Feature feature = null;
                string geoJson = item.ToString();
                if (geoJson.Contains("\"type\": \"Circle\""))
                {
                    // Circle is not supported in standard GeoJSON, it's defined in this sample to make it easier to pass to the server.
                    feature = CreateCircleFeatureFromGeoJson(geoJson);
                }
                else
                {
                    feature = Feature.CreateFeatureFromGeoJson(geoJson);
                }

                if (feature?.GetWellKnownBinary() != null)
                {
                    features.Add(feature);
                }
            }

            return features;
        }

        private static Feature CreateCircleFeatureFromGeoJson(string geoJson)
        {
            dynamic featureEntity = JsonConvert.DeserializeObject(geoJson);
            dynamic geometryEntity = JsonConvert.DeserializeObject(featureEntity.geometry.ToString());
            double x = Convert.ToDouble(geometryEntity.coordinates[0].ToString(), CultureInfo.InvariantCulture);
            double y = Convert.ToDouble(geometryEntity.coordinates[1].ToString(), CultureInfo.InvariantCulture);
            double radius = Convert.ToDouble(geometryEntity.radius.ToString(), CultureInfo.InvariantCulture);

            DistanceUnit distanceUnit = (DistanceUnit)Enum.Parse(typeof(DistanceUnit), geometryEntity.radiusUnit.ToString(), true);
            EllipseShape circleShape = new EllipseShape(new PointShape(x, y), radius, GeographyUnit.DecimalDegree, distanceUnit);
            Feature feature = new Feature(circleShape);
            foreach (var item in featureEntity.properties)
            {
                feature.ColumnValues[item.Name] = item.Value.ToString();
            }

            return feature;
        }

        private static InMemoryFeatureLayer GetDrawnShapesFeatureLayer(string accessId)
        {
            InMemoryFeatureLayer shapesFeatureLayer = new InMemoryFeatureLayer(new Collection<FeatureSourceColumn>() { new FeatureSourceColumn("Name") }, new Collection<BaseShape>());
            shapesFeatureLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);

            shapesFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(GeoColor.FromArgb(100, GeoColor.FromHtml("#676e51")), GeoColors.Black);
            shapesFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyle.CreateSimpleCircleStyle(GeoColor.FromHtml("#2b7a05"), 14, GeoColors.Black);
            shapesFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(GeoColor.FromHtml("#676e51"), 2, true);
            shapesFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("Name", "Arial", 12, DrawingFontStyles.Bold, GeoColors.Gray, GeoColors.White, 2);
            shapesFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;




            // Restore the features if have, or we will use the default features loaded from the xml file.
            Collection<Feature> savedFeatures = GetFeatures(accessId);
            if (!savedFeatures.Any())
            {
                string dataFilePath = Path.Combine(baseDirectory, "App_Data\\Data.xml");
                XElement xElement = XElement.Load(dataFilePath);
                foreach (var geometryXElement in xElement.Descendants("Geometry"))
                {
                    Feature feature = new Feature(geometryXElement.Value);
                    feature.Id = geometryXElement.Attribute("id").Value;
                    feature.ColumnValues.Add("name", geometryXElement.Attribute("name").Value);
                    shapesFeatureLayer.InternalFeatures.Add(feature.Id, feature);
                }
            }
            else
            {
                foreach (Feature feature in savedFeatures)
                {
                    shapesFeatureLayer.InternalFeatures.Add(feature.Id, feature);
                }
            }

            return shapesFeatureLayer;
        }

        private static void SaveFeatures(string accessId, Collection<Feature> features)
        {
            string jsonFilePath = Path.Combine(baseDirectory, "App_Data/Temp", string.Format("{0}.json", accessId));
            using (StreamWriter streamWriter = new StreamWriter(jsonFilePath, false))
            {
                foreach (Feature feature in features)
                {
                    streamWriter.WriteLine(feature.GetGeoJson());
                }
                streamWriter.Close();
            }
        }

        private static Collection<Feature> GetFeatures(string accessId)
        {
            Collection<Feature> features = new Collection<Feature>();
            string jsonFilePath = Path.Combine(baseDirectory, "App_Data/Temp", string.Format("{0}.json", accessId));
            if (System.IO.File.Exists(jsonFilePath))
            {
                string[] geojsons = System.IO.File.ReadAllLines(jsonFilePath);
                foreach (string geojson in geojsons)
                {
                    features.Add(Feature.CreateFeatureFromGeoJson(geojson));
                }
            }

            return features;
        }

    }
}