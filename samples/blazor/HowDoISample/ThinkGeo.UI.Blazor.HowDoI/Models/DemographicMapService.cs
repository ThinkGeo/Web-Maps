using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace ThinkGeo.UI.Blazor.HowDoI
{
    public class DemographicMapService
    {
        public List<DemographicCategoryModel> GetDemographicCategories()
        {
            var menusFile = Path.Combine(Directory.GetCurrentDirectory(), "Data", "DemographicMap.json");
            return JsonConvert.DeserializeObject<List<DemographicCategoryModel>>(File.ReadAllText(menusFile));
        }
    }
}
