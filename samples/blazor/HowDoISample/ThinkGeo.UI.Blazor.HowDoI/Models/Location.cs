using System;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Blazor.HowDoI
{
    /// <summary>
    /// This class specify the basic information for a location.
    /// </summary>
    public class Location
    {
        public Location()
            : this(0, 0, 0, DateTime.Now)
        { }

        public Location(double longitude, double latitude, double speed, DateTime dateTime)
        {
            Longitude = longitude;
            Latitude = latitude;
            Speed = speed;
            DateTime = dateTime;
        }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public double Speed { get; set; }

        public DateTime DateTime { get; set; }

        public PointShape GetLocationPointShape()
        {
            return new PointShape(Longitude, Latitude);
        }
    }
}
