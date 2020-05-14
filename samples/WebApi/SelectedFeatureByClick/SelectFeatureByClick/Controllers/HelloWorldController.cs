using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using ThinkGeo.Core;
using ThinkGeo.UI.WebApi;

namespace SelectFeatureByClick.Controllers
{
    [RoutePrefix("SelectFeatureByClick")]
    public class SelectFeatureByClickController : ApiController
    {
        [HttpGet, Route("Tile/{z}/{x}/{y}")]
        public HttpResponseMessage GetTile(int z, int x, int y)
        {
            // generate overlay
            string shapePath = HttpContext.Current.Server.MapPath("~/App_Data/Austinstreets.shp");
            var layer = new ShapeFileFeatureLayer(shapePath);
            layer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = new LineStyle(
                 new GeoPen(
                    new GeoSolidBrush(
                        GeoColor.FromHtml("#3b5998")
                    ),
                    5f
                )
            );
            layer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);
            layer.Name = "Austinstreets";

            // draw tile image
            var bitmap = new GeoImage(256, 256);

            var geoCanvas = GeoCanvas.CreateDefaultGeoCanvas();
            var boundingBox = WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, GeographyUnit.Meter);
            geoCanvas.BeginDrawing(bitmap, boundingBox, GeographyUnit.Meter);
            layer.Open();
            layer.Draw(geoCanvas, new System.Collections.ObjectModel.Collection<SimpleCandidate>());
            layer.Close();
            geoCanvas.EndDrawing();

            var stream = new MemoryStream();
            bitmap.Save(stream, GeoImageFormat.Png);
            bitmap.Dispose();

            // return response
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new ByteArrayContent(stream.ToArray());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

            return response;
        }

        [HttpPost, Route("SelectRoad")]
        public HttpResponseMessage GetRoad([FromBody] JObject jObject)
        {
            // get parameters
            double x = jObject.Property("x").Value.ToObject<double>();
            double y = jObject.Property("y").Value.ToObject<double>();
            double radius = jObject.Property("radius").Value.ToObject<double>();

            // get nearest feature
            string shapePath = HttpContext.Current.Server.MapPath("~/App_Data/Austinstreets.shp");
            var source = new ShapeFileFeatureSource(shapePath);
            source.Open();
            var features = source.GetFeaturesNearestTo(
                new PointShape(x, y),
                GeographyUnit.DecimalDegree,
                1,
                new string[1] { "NAME" },
                radius,
                DistanceUnit.Meter
            );

            // return response
            var result = new JObject();
            if (features.Count > 0)
            {
                string roadName = features[0].ColumnValues["NAME"];
                result.Add("name", JToken.FromObject(roadName));
                result.Add("success", JToken.FromObject(true));
            }
            else
            {
                result.Add("success", JToken.FromObject(false));
            }
            
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StringContent(result.ToString(), Encoding.UTF8, "application/json");

            return response;
        }
    }
}
