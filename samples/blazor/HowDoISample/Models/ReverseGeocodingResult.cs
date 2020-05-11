using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ThinkGeo.UI.Blazor.HowDoI
{
    public class ReverseGeocodingResult
    {
        public ReverseGeocodingResult(ReverseGeocodingLocation bestMatchLocation, IEnumerable<ReverseGeocodingLocation> nearby)
        {
            BestMatchLocation = bestMatchLocation;
            NearbyLocations = nearby;
        }

        /// <summary>
        /// The best matching location.
        /// </summary>
        public ReverseGeocodingLocation BestMatchLocation { get; }

        /// <summary>
        /// A collection of the matching location order by the distance ASC.
        /// </summary>
        public IEnumerable<ReverseGeocodingLocation> NearbyLocations { get; }
    }
}
