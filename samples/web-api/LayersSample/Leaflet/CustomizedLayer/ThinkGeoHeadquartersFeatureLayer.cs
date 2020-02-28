using ThinkGeo.MapSuite;

namespace ThinkGeo.MapSuite.Layers
{
    public class ThinkGeoHeadquartersFeatureLayer : FeatureLayer
    {
        public ThinkGeoHeadquartersFeatureLayer()
        {
            FeatureSource = new ThinkGeoHeadquartersFeatureSource();
        }
    }
}
