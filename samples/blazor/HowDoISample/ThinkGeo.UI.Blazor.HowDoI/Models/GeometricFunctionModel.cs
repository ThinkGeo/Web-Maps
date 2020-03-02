using System.Collections.Generic;

namespace ThinkGeo.UI.Blazor.HowDoI
{
    public class GeometricFunctionModel
    {
        public string Name { get; set; }

        public string Text { get; set; }

        public string Description { get; set; }

        public double[] Center { get; set; }

        public IEnumerable<string> FeatureIds { get; set; }
    }
}
