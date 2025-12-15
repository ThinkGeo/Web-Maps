using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ThinkGeo.UI.Blazor.HowDoI
{
    public class DemographicMapService
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public List<DemographicCategoryModel> GetDemographicCategories()
        {
            var menusFile = Path.Combine(Directory.GetCurrentDirectory(), "Data", "DemographicMap.json");
            return JsonSerializer.Deserialize<List<DemographicCategoryModel>>(File.ReadAllText(menusFile), JsonOptions);
        }
    }
}
