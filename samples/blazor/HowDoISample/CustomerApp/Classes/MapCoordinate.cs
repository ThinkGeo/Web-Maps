namespace ThinkGeo.UI.Blazor.HowDoI.CustomerApp.Classes
{
    /// <summary>
    /// Represents the geographic coordinates of a point in decimal degrees.
    /// </summary>
    /// <remarks>Use the MapPoint class to specify a location on a map using its latitude and longitude. This
    /// class is commonly used in mapping, geolocation, and spatial data scenarios to represent a single position on the
    /// Earth's surface.</remarks>
    public class MapCoordinate
    {
        /// <summary>
        /// Gets or sets the latitude of the coordinate in decimal degrees. Latitude values range from -90.0 to 90.0, where
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the coordinate in decimal degrees. Longitude values range from -180.0 to 180.0, where
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Initializes a new instance of the MapCoordinate class with default values (0, 0).
        /// </summary>
        public MapCoordinate() { }

        /// <summary>
        /// Initializes a new instance of the MapCoordinate class with the specified latitude and longitude values.
        /// </summary>
        /// <param name="latitude">The latitude of the coordinate in decimal degrees.</param>
        /// <param name="longitude">The longitude of the coordinate in decimal degrees.</param>
        public MapCoordinate(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
