using System.Collections.Generic;

namespace ThinkGeo.UI.Blazor.HowDoI
{
    public class DemographicCategoryModel
    {
        public string Name { get; set; }

        public string Icon { get; set; }

        public IEnumerable<Legend> Legends { get; set; }
    }

    public class Legend
    {
        public string ColumnName { get; set; }

        public string Alias { get; set; }
    }
}
