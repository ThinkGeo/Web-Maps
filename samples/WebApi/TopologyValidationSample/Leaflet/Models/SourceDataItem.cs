using System.Collections.ObjectModel;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace TopologyValidation
{
    public class SourceDataItem
    {
        private string id;
        private Collection<Feature> firstInputFeatures;
        private Collection<Feature> secondInputFeatures;
        private string comment;

        public SourceDataItem()
        {
            firstInputFeatures = new Collection<Feature>();
            secondInputFeatures = new Collection<Feature>();
        }

        public string Id
        {
            get { return id; }
            set { id = value; }
        }
        public Collection<Feature> FirstInputFeatures
        {
            get { return firstInputFeatures; }
        }

        public Collection<Feature> SecondInputFeatures
        {
            get { return secondInputFeatures; }
        }

        public string Comment
        {
            get { return comment; }
            set { comment = value; }
        }
    }
}
