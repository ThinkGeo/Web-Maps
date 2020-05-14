using System;
using System.Runtime.Serialization;

namespace CSharp_HowDoISamples
{
    /// <summary>
    /// Summary description for VehicleStatus
    /// </summary>
    [DataContract(Name = "Vehicle")]
    public class JsonVehicle {
        public JsonVehicle()
            : this(String.Empty, String.Empty, double.NaN, double.NaN) { }

        public JsonVehicle(string id, string imageName, double longitude, double latitude)
            : base() {
            Id = id;
            ImageName = imageName;
            Longitude = longitude;
            Latitude = latitude;
        }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "n")]
        public string ImageName { get; set; }

        [DataMember(Name = "x")]
        public double Longitude { get; set; }

        [DataMember(Name = "y")]
        public double Latitude { get; set; }
    }
}
