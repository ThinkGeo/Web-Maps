using System;
using System.Runtime.Serialization;

namespace ThinkGeo.MapSuite.AjaxVehicleTracking
{
    [DataContract]
    public class JsonLocation
    {
        [DataMember(Name = "t")]
        public string TrackTime { get; set; }

        [DataMember(Name = "s")]
        public double Speed { get; set; }

        [DataMember(Name = "x")]
        public double Longitude { get; set; }

        [DataMember(Name = "y")]
        public double Latitude { get; set; }

        public JsonLocation()
            : this(.0, .0, .0, DateTime.MinValue.ToString())
        { }

        public JsonLocation(double longitude, double latitude, double speed, string trackTime) : base()
        {
            Longitude = longitude;
            Latitude = latitude;
            Speed = speed;
            TrackTime = trackTime;
        }
    }
}
