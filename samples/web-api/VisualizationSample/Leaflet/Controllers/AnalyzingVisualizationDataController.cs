using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using ThinkGeo.Core;
using ThinkGeo.UI.WebApi;

namespace Visualization.Controllers
{
    [Route("tile")]
    public class VisualizationController : ControllerBase
    {
        private static readonly string baseDirectory;
        private const string filterOverlayKey = "filterStyle";

        // Initialize the overlays for drawing, which is cached for whole website.
        private static Dictionary<string, LayerOverlay> cachedOverlays;

        // Initialize the filter expression.
        private static Dictionary<string, Tuple<string, string>> filterExpressions;

        static VisualizationController()
        {
            baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

            cachedOverlays = new Dictionary<string, LayerOverlay>() {
            {"HeatStyle",  OverlayBuilder.GetOverlayWithHeatStyle()},
            {"ClassBreakStyle", OverlayBuilder.GetOverlayWithClassBreakStyle()},
            {"DotDensityStyle", OverlayBuilder.GetOverlayWithDotDensityStyle()},
            {"IsolineStyle", OverlayBuilder.GetOverlayWithIsoLineStyle()},
            {"ClusterStyle", OverlayBuilder.GetOverlayWithClusterPointStyle()},
            //{"ZedGraphStyle", OverlayBuilder.GetOverlayWithZedGraphStyle()},
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
        public IActionResult GetTile(string layerId, string accessId, int z, int x, int y)
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
            string styleFilePath = Path.Combine(baseDirectory, "App_Data", "Temp" , string.Format("{0}.json", accessId));

            if (System.IO.File.Exists(styleFilePath))
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(System.IO.File.ReadAllText(styleFilePath));
            }

            return null;
        }

        private static void SaveStyle(string accessId, Dictionary<string, string> styleContent)
        {
            string styleFilePath = Path.Combine(baseDirectory, "App_Data", "Temp", string.Format("{0}.json", accessId));

            using (StreamWriter sw = new StreamWriter(styleFilePath, false))
            {
                sw.WriteLine(JsonConvert.SerializeObject(styleContent));
            }
        } 
    }
}
