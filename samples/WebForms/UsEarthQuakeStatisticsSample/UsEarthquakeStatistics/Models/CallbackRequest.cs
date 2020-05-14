using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ThinkGeo.MapSuite.EarthquakeStatistics
{
    [DataContract]
    public class CallbackRequest
    {
        private string mapType;
        private string command;
        private string featureId;
        private Collection<EarthquakeQueryConfiguration> queryConfigrations;

        public CallbackRequest()
        { }

        [DataMember(Name = "command")]
        public string Command
        {
            get { return command; }
            set { command = value; }
        }

        [DataMember(Name = "mapType")]
        public string MapDisplayType
        {
            get { return mapType; }
            set { mapType = value; }
        }

        [DataMember(Name = "featureId")]
        public string FeatureId
        {
            get { return featureId; }
            set { featureId = value; }
        }

        [DataMember(Name = "configs")]
        public Collection<EarthquakeQueryConfiguration> QueryConfigurations
        {
            get
            {
                if (queryConfigrations == null)
                {
                    queryConfigrations = new Collection<EarthquakeQueryConfiguration>();
                }
                return queryConfigrations;
            }
            internal set { queryConfigrations = value; }
        }
    }
}