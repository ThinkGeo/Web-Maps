using System.Collections.Generic;
using System.Collections.ObjectModel;
using ThinkGeo.MapSuite.Core;

namespace ThinkGeo.MapSuite.USDemographicMap
{
    public abstract class DemographicStyle
    {
        private GeoColor baseColor;
        private int opacity;
        private Collection<string> selectedColumns;

        protected DemographicStyle()
            : this(new Collection<string>())
        { }

        protected DemographicStyle(IEnumerable<string> selectedColumns)
        {
            this.selectedColumns = new Collection<string>(new List<string>(selectedColumns));
            this.baseColor = GeoColor.FromHtml("#f1f369");
            this.Opacity = 180;
        }

        public GeoColor BaseColor
        {
            get { return baseColor; }
            set { baseColor = value; }
        }

        public int Opacity
        {
            get { return opacity; }
            set { opacity = value; }
        }

        public Collection<string> SelectedColumns
        {
            get { return selectedColumns; }
        }

        public Style GetStyle(FeatureSource featureSource)
        {
            return GetStyleCore(featureSource);
        }

        protected abstract Style GetStyleCore(FeatureSource featureSource);
    }
}