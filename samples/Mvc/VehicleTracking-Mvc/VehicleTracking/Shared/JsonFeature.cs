using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace ThinkGeo.MapSuite.VehicleTracking
{
    public class JsonFeature
    {
        private string id;
        private string wkt;

        public JsonFeature(string id, string wkt)
        {
            this.id = id;
            this.wkt = wkt;
        }

        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Wkt
        {
            get { return wkt; }
            set { wkt = value; }
        }
    }
}