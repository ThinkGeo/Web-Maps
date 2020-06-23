using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using ThinkGeo.Core;
using ThinkGeo.UI.WebApi;

namespace DisplayNauticalChart
{
    [RoutePrefix("DisplayNauticalChart")]
    public class HelloWorldController : ApiController
    {
        private readonly string nauticalChartsFilePath = HttpContext.Current.Server.MapPath("~/App_Data/US4IL10M.000");
        private static NauticalChartsFeatureLayer nauticalChartsFeatureLayer;
        private static ProjectionConverter projectionConverter = new ProjectionConverter(4326, 3857);

        [Route("tile/{z}/{x}/{y}")]
        [HttpGet]
        public HttpResponseMessage GetTile(int z, int x, int y)
        {
            if (nauticalChartsFeatureLayer == null)
                nauticalChartsFeatureLayer = new NauticalChartsFeatureLayer(nauticalChartsFilePath);
            lock (nauticalChartsFeatureLayer)
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
            }

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
    }
}