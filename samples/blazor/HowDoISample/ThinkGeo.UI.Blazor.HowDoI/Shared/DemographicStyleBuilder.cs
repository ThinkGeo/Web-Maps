using System.Collections.Generic;
using System.Collections.ObjectModel;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Blazor.HowDoI
{
    public abstract class DemographicStyleBuilder
    {
        private GeoColor color;
        private byte opacity;
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

        public byte Opacity
        {
            get { return opacity; }
            set { opacity = value; }
        }

        public Style GetStyle(FeatureSource featureSource)
        {
            return GetStyleCore(featureSource);
        }

        protected abstract Style GetStyleCore(FeatureSource featureSource);
    }
}