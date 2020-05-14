using Newtonsoft.Json;
using System;
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

namespace Visualization.Controllers
{
    [RoutePrefix("tile")]
    public class VisualizationController : ApiController
    {
        private const string filterOverlayKey = "filterStyle";

        // Initialize the overlays for drawing, which is cached for whole website.
        private static Dictionary<string, LayerOverlay> cachedOverlays;

        // Initialize the filter expression.
        private static Dictionary<string, Tuple<string, string>> filterExpressions;

        static VisualizationController()
        {
            cachedOverlays = new Dictionary<string, LayerOverlay>() { 
            {"HeatStyle",  OverlayBuilder.GetOverlayWithHeatStyle()},
            {"ClassBreakStyle", OverlayBuilder.GetOverlayWithClassBreakStyle()},
            {"DotDensityStyle", OverlayBuilder.GetOverlayWithDotDensityStyle()},
            {"IsolineStyle", OverlayBuilder.GetOverlayWithIsoLineStyle()},
            {"ClusterStyle", OverlayBuilder.GetOverlayWithClusterPointStyle()},
            {"ZedGraphStyle", OverlayBuilder.GetOverlayWithZedGraphStyle()},
            {"IconStyle", OverlayBuilder.GetOverlayWithIconStyle()},
            {"CustomStyle", OverlayBuilder.GetOverlayWithCustomeStyle()} };

            filterExpressions = new Dictionary<string, Tuple<string, string>>(){
            {"GreaterThanOrEqualTo", new Tuple<string, string>(">=", string.Empty)},
            {"GreaterThan", new Tuple<string, string>(">", string.Empty)},
            {"LessThanOrEqualTo", new Tuple<string, string>("<=", string.Empty)},
            {"LessThan", new Tuple<string, string>("<", string.Empty)},
            {"Equal", new Tuple<string, string>("^", "$")},
            {"DoesNotEqual", new Tuple<string, string>("^(?!", ").*?$")}};
        }

        [Route("{layerId}/{z}/{x}/{y}/{accessId}")]
        public HttpResponseMessage GetTile(string layerId, string accessId, int z, int x, int y)
        {
            // Create the LayerOverlay for displaying the map.
            LayerOverlay layerOverlay;
            // The FilterStyle overlay is not stored in CachedOverlay.
            if (layerId == "FilterStyle")
            {
                layerOverlay = GetFilterStyleOverlay(accessId);
            }
            else
            {
                layerOverlay = cachedOverlays[layerId];
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

        [Route("UpdateFilterStyle/{accessId}")]
        [HttpPost]
        public bool UpdateFilterStyle(string accessId, [FromBody] string postData)
        {
            // Parse the post data from client side in JSON format.
            // There are 2 parameters included in the "postData", one is the filterExpression and another one is the value for filter.

            bool updateSuc = true;
            try
            {
                Dictionary<string, string> parameters = JsonConvert.DeserializeObject<Dictionary<string, string>>(postData);

                SaveStyle(accessId, parameters);
            }
            catch (Exception ex)
            {
                updateSuc = false;
            }

            return updateSuc;
        }

        private static LayerOverlay GetFilterStyleOverlay(string accessId)
        {
            LayerOverlay layerOverlay = OverlayBuilder.GetOverlayWithFilterStyle();

            Dictionary<string, string> savedFilterStyles = GetSavedStyle(accessId);
            if (savedFilterStyles == null)
            {
                return layerOverlay;
            }

            string filterExpression = savedFilterStyles["filterExpression"];
            string filterValue = savedFilterStyles["filterValue"];

            if (filterExpressions.ContainsKey(filterExpression) && layerOverlay.Layers.Count > 0)
            {
                // Get the filter style applied to the drawing Overlay.
                FilterStyle filterStyle = ((FeatureLayer)layerOverlay.Layers[0]).ZoomLevelSet.ZoomLevel01.CustomStyles[0] as FilterStyle;
                if (filterStyle != null)
                {
                    filterStyle.Conditions.Clear();

                    // Create the filter expression based on the values from client side.
                    string expression = string.Format("{0}{1}{2}", filterExpressions[filterExpression].Item1, filterValue, filterExpressions[filterExpression].Item2);
                    FilterCondition filterCondition = new FilterCondition("Population", expression);
                    filterStyle.Conditions.Add(filterCondition);
                }
            }

            return layerOverlay;
        }

        private static Dictionary<string, string> GetSavedStyle(string accessId)
        {
            string styleFile = string.Format("{0}.json", accessId);
            string styleFilePath = Path.Combine(GetTempDirectory(), styleFile);

            if (File.Exists(styleFilePath))
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(styleFilePath));
            }

            return null;
        }

        private static void SaveStyle(string accessId, Dictionary<string, string> styleContent)
        {
            string styleFile = string.Format("{0}.json", accessId);
            string styleFilePath = Path.Combine(GetTempDirectory(), styleFile);

            using (StreamWriter sw = new StreamWriter(styleFilePath, false))
            {
                sw.WriteLine(JsonConvert.SerializeObject(styleContent));
            }
        }

        private static string GetTempDirectory()
        {
            Uri uri = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            string rootDirectory = Path.GetDirectoryName(Path.GetDirectoryName(uri.LocalPath));

            return Path.Combine(rootDirectory, "App_Data", "Temp");
        }
    }
}
