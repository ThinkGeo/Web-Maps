using System.Text;
using System.Text.Json;
using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI
{
    /// <summary>
    /// Features and shapes as the GeoJSON the browser's map takes: in latitude and longitude,
    /// with the columns as properties. The samples keep their shapes in Web Mercator meters, so
    /// each is converted on the way out.
    /// </summary>
    public static class GeoJson
    {
        private static readonly ProjectionConverter ToDegrees = Open(new ProjectionConverter(3857, 4326));
        private static readonly ProjectionConverter ToMeters = Open(new ProjectionConverter(4326, 3857));

        private static ProjectionConverter Open(ProjectionConverter converter)
        {
            converter.Open();
            return converter;
        }

        /// <summary>A feature collection, as text.</summary>
        public static string Collection(IEnumerable<Feature> features)
        {
            var text = new StringBuilder("{\"type\":\"FeatureCollection\",\"features\":[");
            var first = true;
            foreach (var feature in features)
            {
                if (!first) text.Append(',');
                first = false;
                text.Append(One(feature));
            }
            return text.Append("]}").ToString();
        }

        /// <summary>A feature collection of bare shapes.</summary>
        public static string Collection(IEnumerable<BaseShape> shapes) => Collection(shapes.Select(shape => new Feature(shape)));

        /// <summary>One feature, as text: its shape in degrees and its column values as properties.</summary>
        public static string One(Feature feature)
        {
            var inDegrees = ToDegrees.ConvertToExternalProjection(feature.GetShape());
            var geometry = inDegrees.GetGeoJson();
            var properties = new Dictionary<string, string>(feature.ColumnValues);
            var text = new StringBuilder("{\"type\":\"Feature\"");
            if (!string.IsNullOrEmpty(feature.Id))
            {
                text.Append(",\"id\":").Append(JsonSerializer.Serialize(feature.Id));
            }
            text.Append(",\"geometry\":").Append(geometry);
            text.Append(",\"properties\":").Append(JsonSerializer.Serialize(properties));
            return text.Append('}').ToString();
        }

        /// <summary>A shape the browser sent as GeoJSON geometry, in the map's meters.</summary>
        public static BaseShape Shape(JsonElement geometry)
        {
            var inDegrees = BaseShape.CreateShapeFromGeoJson(geometry.GetRawText());
            return ToMeters.ConvertToExternalProjection(inDegrees);
        }

        /// <summary>A point the browser sent as [lng, lat], in the map's meters.</summary>
        public static PointShape Point(double lng, double lat) => (PointShape)ToMeters.ConvertToExternalProjection(new PointShape(lng, lat));

        /// <summary>A point in the map's meters as [lng, lat].</summary>
        public static double[] LngLat(PointShape point)
        {
            var degrees = (PointShape)ToDegrees.ConvertToExternalProjection(point);
            return new[] { degrees.X, degrees.Y };
        }

        /// <summary>A shape in the map's meters as GeoJSON geometry text, in degrees.</summary>
        public static string Geometry(BaseShape shape) => ToDegrees.ConvertToExternalProjection(shape).GetGeoJson();

        /// <summary>An extent in meters as [minx, miny, maxx, maxy] in degrees, the way the map's fitBounds takes it.</summary>
        public static double[] Bounds(RectangleShape extent)
        {
            var box = (RectangleShape)ToDegrees.ConvertToExternalProjection(extent);
            return new[] { box.LowerLeftPoint.X, box.LowerLeftPoint.Y, box.UpperRightPoint.X, box.UpperRightPoint.Y };
        }

        /// <summary>An extent the browser sent as minx,miny,maxx,maxy in degrees, in the map's meters.</summary>
        public static RectangleShape Extent(string bbox)
        {
            var parts = bbox.Split(',').Select(part => double.Parse(part, System.Globalization.CultureInfo.InvariantCulture)).ToArray();
            return (RectangleShape)ToMeters.ConvertToExternalProjection(new RectangleShape(parts[0], parts[3], parts[2], parts[1]));
        }
    }
}
