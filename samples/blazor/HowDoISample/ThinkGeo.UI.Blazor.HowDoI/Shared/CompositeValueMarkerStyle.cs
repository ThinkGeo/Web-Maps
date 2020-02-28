using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Blazor.HowDoI
{
    internal class CompositeValueMarkerStyle : ValueMarkerStyle
    {
        private readonly Dictionary<string, ValueMarkerStyle> valueMarkers = new Dictionary<string, ValueMarkerStyle>();

        public CompositeValueMarkerStyle(string columnName)
            : base(columnName)
        {
        }

        public void Add(string name, ValueMarkerStyle style)
        {
            valueMarkers.Add(name, style);
        }

        public override Collection<Marker> GetMarkers(IEnumerable<Feature> features)
        {
            var markers = features.GroupBy(f => f.ColumnValues[ColumnName])
                 .SelectMany(g => valueMarkers[g.Key].GetMarkers(g)).ToList();
            return new Collection<Marker>(markers);
        }
    }
}
