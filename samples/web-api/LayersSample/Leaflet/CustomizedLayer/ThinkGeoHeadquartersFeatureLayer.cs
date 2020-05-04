using System;
using ThinkGeo.Core;

namespace ThinkGeo.MapSuite.Layers
{
    [Serializable]
    public class ThinkGeoHeadquartersFeatureLayer : FeatureLayer
    {
        public ThinkGeoHeadquartersFeatureLayer()
        {
            FeatureSource = new ThinkGeoHeadquartersFeatureSource();
        }
    }
}
