using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ThinkGeo.MapSuite.Layers;

namespace ThinkGeo.MapSuite.AjaxVehicleTracking
{
    [DataContract]
    public class CallbackRequest
    {
        private string request;
        private Collection<JsonFence> features;

        [DataMember(Name = "displaySystem")]
        internal string displaySystem;

        public CallbackRequest()
        {
        }

        [DataMember(Name = "request")]
        public string Request
        {
            get { return request; }
            set { request = value; }
        }

        [DataMember(Name = "features")]
        public Collection<JsonFence> Features
        {
            get { return features; }
            internal set { features = value; }
        }

        public UnitSystem UnitSystem
        {
            get
            {
                if (displaySystem == "Metric")
                {
                    return UnitSystem.Metric;
                }
                else
                {
                    return UnitSystem.Imperial;
                }
            }
            set
            {
                if (value == UnitSystem.Metric)
                {
                    displaySystem = "Metric";
                }
                else
                {
                    displaySystem = "Imperial";
                }
            }
        }
    }
}