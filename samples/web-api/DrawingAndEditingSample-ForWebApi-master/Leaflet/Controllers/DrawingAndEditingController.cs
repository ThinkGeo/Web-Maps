using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Xml.Linq;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebApi;

namespace DrawingAndEditing.Controllers
{
    [RoutePrefix("edit")]
    public class DrawingAndEditingController : ApiController
    {
        private static string jsonArrayTemplate = "[{0}]";

        [Route("{z}/{x}/{y}/{accessId}")]
        public HttpResponseMessage GetTile(int z, int x, int y, string accessId)
        {
            // Create the layerOverlay for displaying the map.
            LayerOverlay layerOverlay = new LayerOverlay();
            // Get the featureLayer including all drawn shapes by the access id for display.
            InMemoryFeatureLayer drawnShapesFeatureLayer = GetDrawnShapesFeatureLayer(accessId);
            if (drawnShapesFeatureLayer != null && drawnShapesFeatureLayer.InternalFeatures.Count > 0)
            {
                layerOverlay.Layers.Add(drawnShapesFeatureLayer);
            }

            // Draw the map and return the image back to client in an HttpResponseMessage.
            using (Bitmap bitmap = new Bitmap(256, 256))
            {
                PlatformGeoCanvas geoCanvas = new PlatformGeoCanvas();
                RectangleShape boundingBox = WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, GeographyUnit.Meter);
                geoCanvas.BeginDrawing(bitmap, boundingBox, GeographyUnit.Meter);
                layerOverlay.Draw(geoCanvas);
                geoCanvas.EndDrawing();

                MemoryStream memoryStream = new MemoryStream();
                bitmap.Save(memoryStream, ImageFormat.Png);

                HttpResponseMessage responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                responseMessage.Content = new ByteArrayContent(memoryStream.ToArray());
                responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

                return responseMessage;
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
        public HttpResponseMessage SaveShapes(string accessId, [FromBody] string modifiedShapesInJson)
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
            shapesFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(100, GeoColor.FromHtml("#676e51")), GeoColor.SimpleColors.Black);
            shapesFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.FromHtml("#2b7a05"), 14, GeoColor.SimpleColors.Black);
            shapesFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromHtml("#676e51"), 2, true);
            shapesFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Name", "Arial", 12, DrawingFontStyles.Bold, GeoColor.StandardColors.Gray, GeoColor.StandardColors.White, 2);
            shapesFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            Proj4Projection proj4 = new Proj4Projection();
            proj4.InternalProjectionParametersString = Proj4Projection.GetWgs84ParametersString();
            proj4.ExternalProjectionParametersString = Proj4Projection.GetSphericalMercatorParametersString();
            shapesFeatureLayer.FeatureSource.Projection = proj4;

            // Restore the features if have, or we will use the default features loaded from the xml file.
            Collection<Feature> savedFeatures = GetFeatures(accessId);
            if (!savedFeatures.Any())
            {
                string dataFilePath = GetFullPath("App_Data\\Data.xml");
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
            string jsonFilePath = GetFullPath(Path.Combine("App_Data/Temp", string.Format("{0}.json", accessId)));
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
            string jsonFilePath = GetFullPath(Path.Combine("App_Data/Temp", string.Format("{0}.json", accessId)));
            if (File.Exists(jsonFilePath))
            {
                string[] geojsons = File.ReadAllLines(jsonFilePath);
                foreach (string geojson in geojsons)
                {
                    features.Add(Feature.CreateFeatureFromGeoJson(geojson));
                }
            }

            return features;
        }

        private static string GetFullPath(string fileName)
        {
            Uri uri = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            string rootDirectory = Path.GetDirectoryName(Path.GetDirectoryName(uri.LocalPath));
            return Path.Combine(rootDirectory, fileName);
        }
    }
}