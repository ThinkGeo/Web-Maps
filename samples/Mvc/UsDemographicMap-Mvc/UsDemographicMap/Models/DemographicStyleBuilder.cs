using System.Collections.Generic;
using System.Collections.ObjectModel;
using ThinkGeo.MapSuite.Core;

namespace ThinkGeo.MapSuite.USDemographicMap
{
    public abstract class DemographicStyleBuilder
    {
        private int opacity;
        private GeoColor color;
        private Collection<string> selectedColumns;

        protected DemographicStyleBuilder()
            : this(new Collection<string>())
        { }

        protected DemographicStyleBuilder(IEnumerable<string> selectedColumns)
        {
            this.Opacity = 100;
            this.color = GeoColor.FromHtml("#f1f369");
            this.selectedColumns = new Collection<string>(new List<string>(selectedColumns));
        }

        public Collection<string> SelectedColumns
        {
            get { return selectedColumns; }
        }

        public GeoColor Color
        {
            get { return color; }
            set { color = value; }
        }

        public int Opacity
        {
            get { return opacity; }
            protected set { opacity = value; }
        }

        public Collection<Style> GetStyles(FeatureSource featureSource)
        {
            return GetStylesCore(featureSource);
        }

        protected abstract Collection<Style> GetStylesCore(FeatureSource featureSource);
    }
}