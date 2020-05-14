using System.Collections.Generic;
using System.Collections.ObjectModel;
using ThinkGeo.MapSuite.Shapes;

namespace ThinkGeo.MapSuite.Layers
{
    public class ThinkGeoHeadquartersFeatureSource : FeatureSource
    {
        protected override Collection<Feature> GetAllFeaturesCore(IEnumerable<string> returningColumnNames)
        {
            return SampleHelper.GetFeatures("CustomizedLayer");
        }
    }
}
