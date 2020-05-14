using System.Collections.Generic;
using System.Runtime.Serialization;
using ThinkGeo.MapSuite.Shapes;

namespace ThinkGeo.MapSuite.EarthquakeStatistics
{
    [DataContract]
    public class JsonFeature
    {
        private string id;
        private string wkt;
        private Dictionary<string, string> values;

        public JsonFeature()
        { }

        public JsonFeature(Feature feature)
            : this(feature.Id, feature.GetWellKnownText(), feature.ColumnValues)
        {
        }

        public JsonFeature(string id, string wkt, Dictionary<string, string> values)
        {
            this.id = id;
            this.wkt = wkt;
            this.values = values;
        }

        [DataMember(Name = "id")]
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        [DataMember(Name = "wkt")]
        public string Wkt
        {
            get { return wkt; }
            set { wkt = value; }
        }

        [DataMember(Name = "values")]
        public Dictionary<string, string> Values
        {
            get { return values; }
            set { values = value; }
        }
    }
}