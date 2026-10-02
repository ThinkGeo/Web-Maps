using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI
{
    /// <summary>Pictures drawn on the server with a GeoCanvas: a layer over an extent, or a style's sample.</summary>
    public static class Pictures
    {
        /// <summary>The layers drawn at an extent, fitted to the picture's size, as PNG bytes - a LayerOverlay drawn once, the way its tiles are.</summary>
        public static byte[] OfLayers(IEnumerable<LayerBase> layers, RectangleShape extent, int width, int height, GeoColor background, GeographyUnit unit = GeographyUnit.Meter)
        {
            var overlay = new LayerOverlay(layers);
            using (var image = new GeoImage(width, height))
            {
                var canvas = GeoCanvas.CreateDefaultGeoCanvas();
                canvas.BeginDrawing(image, MapUtil.GetDrawingExtent(extent, width, height), unit);
                canvas.Clear(new GeoSolidBrush(background));
                overlay.Draw(canvas);
                canvas.EndDrawing();
                return image.GetImageBytes(GeoImageFormat.Png);
            }
        }

        /// <summary>
        /// A style's sample - the picture a legend shows for it - as PNG bytes. A ground can be
        /// painted first, which is what a translucent fill needs to be read against; left out,
        /// the picture is see-through and takes the colour of whatever it is laid on.
        /// </summary>
        public static byte[] OfStyle(Style style, int width, int height, GeoColor ground = null)
        {
            using (var image = new GeoImage(width, height))
            {
                var canvas = GeoCanvas.CreateDefaultGeoCanvas();
                canvas.BeginDrawing(image, new RectangleShape(0, height, width, 0), GeographyUnit.Meter);
                if (ground != null) canvas.Clear(new GeoSolidBrush(ground));
                style.DrawSample(canvas, new DrawingRectangleF(width / 2f, height / 2f, width, height));
                canvas.EndDrawing();
                return image.GetImageBytes(GeoImageFormat.Png);
            }
        }
    }
}
