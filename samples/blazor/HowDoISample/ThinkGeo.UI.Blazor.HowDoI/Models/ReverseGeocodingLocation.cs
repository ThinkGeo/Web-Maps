using System.Collections.Generic;

namespace ThinkGeo.UI.Blazor.HowDoI
{
    /// <summary>
    /// A class presents the location for ReverseGeocoding.
    /// </summary>
    /// <remarks>A class presents the location for ReverseGeocoding.</remarks>
    public class ReverseGeocodingLocation
    {
        /// <summary>
        /// The WKT of this location.  May be a point (e.g. a restaurant),
        /// a line (e.g. a road), or a polygon (e.g. a public park or a state boundary).
        /// </summary>
        public string LocationFeatureWellKnownText { get; set; }

        /// <summary>
        /// The name of this location, e.g. McDonald's.
        /// </summary>
        public string LocationName { get; set; }

        /// <summary>
        /// The cardinal direction of this location from the query shape you reverse geocoded.
        /// Note: won't apply when you use the reverse geocode area API.
        /// </summary>
        public string DirectionFromQueryFeature { get; set; }

        /// <summary>
        /// The distance of this location from the query shape you reverse geocoded.
        /// Note: will always be 0 when you use the reverse geocode area API.
        /// </summary>
        public double DistanceFromQueryFeature { get; set; }

        /// <summary>
        /// The category of this location, e.g. "Road", "Sustenance", "Building", etc.
        /// Many of these are based on known OpenStreetMap place categories.
        /// </summary>
        public string LocationCategory { get; set; }

        /// <summary>
        /// The type of location as defined by its OpenStreetMap type tag.
        /// Example 1: A location in the "Road" category may have a PlaceType of "primary".
        /// Example 2: A location in the "Building" category may have a PlaceType of "house".
        /// </summary>
        public string LocationType { get; set; }

        /// <summary>
        /// The human-readable address of this location.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// The house number of this location's address.
        /// </summary>
        public string HouseNumber { get; set; }

        /// <summary>
        /// Extended properties of the location. e.g., for a road, may contain properties 
        /// like the number of lanes, the surface type, whether it is one-way, etc.
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }

        /// <summary>
        /// The postal code of the location, if applicable.
        /// NOTE: Only populated if your request specified ReverseGeocodingResultDetail.Verbose.
        /// </summary>
        public string Postcode { get; set; }

        /// <summary>
        /// A dictionary of individual address components, e.g. street, city, country, etc.
        /// NOTE: Only populated if your request specified ReverseGeocodingResultDetail.Verbose.
        /// </summary>
        public Dictionary<string, string> AddressComponents { get; set; }
    }
}
