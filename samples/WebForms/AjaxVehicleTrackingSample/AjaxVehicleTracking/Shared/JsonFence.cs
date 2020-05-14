using System.Runtime.Serialization;

namespace ThinkGeo.MapSuite.AjaxVehicleTracking
{
    [DataContract]
    public class JsonFence
    {
        private string id;
        private string wkt;

        public JsonFence(string id, string wkt)
        {
            this.id = id;
            this.wkt = wkt;
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
    }
}