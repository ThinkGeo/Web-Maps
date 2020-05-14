
namespace CSharp_HowDoISamples
{
    public class HotelInfomation
    {
        public HotelInfomation()
        { }

        public HotelInfomation(string featureId, string name, string address, int rooms)
        {
            this.FeatureId = featureId;
            this.Name = name;
            this.Address = address;
            this.Rooms = rooms;
        }

        public string FeatureId
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Address
        {
            get;
            set;
        }

        public int Rooms
        {
            get;
            set;
        }
    }
}