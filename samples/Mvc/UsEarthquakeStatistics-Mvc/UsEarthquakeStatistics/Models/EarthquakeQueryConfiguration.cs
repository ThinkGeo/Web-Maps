using System.Runtime.Serialization;

namespace ThinkGeo.MapSuite.EarthquakeStatistics
{
    [DataContract]
    public class EarthquakeQueryConfiguration
    {
        private string parameter;
        private int minimum;
        private int maximum;

        public EarthquakeQueryConfiguration()
        {
        }

        [DataMember(Name = "name")]
        public string Parameter
        {
            get { return parameter; }
            set { parameter = value; }
        }

        [DataMember(Name = "min")]
        public int Minimum
        {
            get { return minimum; }
            set { minimum = value; }
        }

        [DataMember(Name = "max")]
        public int Maximum
        {
            get { return maximum; }
            set { maximum = value; }
        }
    }
}