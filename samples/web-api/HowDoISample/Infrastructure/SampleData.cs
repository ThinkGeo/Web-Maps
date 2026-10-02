using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI
{
    /// <summary>Where the sample data is: next to the binaries, copied there from the Blazor gallery's Data folder on build.</summary>
    public static class SampleData
    {
        public static string Path(string relative) => System.IO.Path.Combine(AppContext.BaseDirectory, "Data", relative.Replace('/', System.IO.Path.DirectorySeparatorChar));

        /// <summary>A picture under wwwroot/images: next to the binaries once published, under the project while developing.</summary>
        public static string Image(string relative)
        {
            var published = System.IO.Path.Combine(AppContext.BaseDirectory, "wwwroot", "images", relative.Replace('/', System.IO.Path.DirectorySeparatorChar));
            return File.Exists(published) ? published : System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", relative.Replace('/', System.IO.Path.DirectorySeparatorChar));
        }

        /// <summary>The Frisco shapefiles are in Texas state plane feet; the map is in Web Mercator meters.</summary>
        public static ShapeFileFeatureSource Frisco(string shapefile) =>
            new ShapeFileFeatureSource(Path("Shapefile/" + shapefile)) { ProjectionConverter = new ProjectionConverter(2276, 3857) };
    }

    /// <summary>The keys the samples use with ThinkGeo Cloud: for these samples only. Create your own at https://cloud.thinkgeo.com.</summary>
    public static class GlobalSettings
    {
        public const string ThinkGeoApiKey = "PIbGd76RyHKod99KptWTeb-Jg9JUPEPUBFD3SZJYLDE~";
        public const string ThinkGeoCloudClientId = "FSDgWMuqGhZCmZnbnxh-Yl1HOaDQcQ6mMaZZ1VkQNYw~";
        public const string ThinkGeoCloudClientSecret = "IoOZkBJie0K9pz10jTRmrUclX6UYssZBeed401oAfbxb9ufF1WVUvg~~";
    }
}
