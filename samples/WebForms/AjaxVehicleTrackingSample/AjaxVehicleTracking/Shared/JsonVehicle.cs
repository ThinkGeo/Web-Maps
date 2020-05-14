using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ThinkGeo.MapSuite.AjaxVehicleTracking
{
    /// <summary>
    /// Summary description for VehicleStatus
    /// </summary>
    [DataContract(Name = "Vehicle")]
    public class JsonVehicle
    {
        private int speedDuration;

        public JsonVehicle()
            : this(String.Empty, String.Empty, String.Empty, new JsonLocation())
        { }

        public JsonVehicle(string id, string name, string vehicleIconVirtualPath, string trackTime, double speed, double longitude, double latitude)
            : this(id, name, vehicleIconVirtualPath, new JsonLocation(longitude, latitude, speed, trackTime))
        { }

        public JsonVehicle(string id, string name, string vehicleIconVirtualPath, JsonLocation currentLocation)
        {
            Id = id;
            Name = name;
            VehicleIconVirtualPath = vehicleIconVirtualPath;
            CurrentLocation = currentLocation;
            HistoryLocations = new Collection<JsonLocation>();
        }

        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "path")]
        public string VehicleIconVirtualPath { get; set; }

        [DataMember(Name = "motion")]
        public int VehicleMotionState { get; set; }

        [DataMember(Name = "loc")]
        public JsonLocation CurrentLocation { get; set; }

        [DataMember(Name = "his")]
        public Collection<JsonLocation> HistoryLocations { get; set; }

        [DataMember(Name = "dur")]
        public int SpeedDuration
        {
            get
            {
                double lastSpeed = CurrentLocation.Speed;
                foreach (JsonLocation location in HistoryLocations)
                {
                    if (location.Speed == lastSpeed)
                    {
                        speedDuration++;
                    }
                    else
                    {
                        break;
                    }
                }

                return speedDuration;
            }
            set { speedDuration = value; }
        }

        [DataMember(Name = "isIn")]
        public bool IsInFence { get; set; }
    }
}
