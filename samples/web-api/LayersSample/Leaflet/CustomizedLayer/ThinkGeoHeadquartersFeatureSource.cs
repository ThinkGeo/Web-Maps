using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ThinkGeo.Core;

namespace ThinkGeo.MapSuite.Layers
{
    [Serializable]
    public class ThinkGeoHeadquartersFeatureSource : FeatureSource
    {
        protected override Collection<Feature> GetAllFeaturesCore(IEnumerable<string> returningColumnNames)
        {
            return SampleHelper.GetFeatures("CustomizedLayer");
        }
    }
}
