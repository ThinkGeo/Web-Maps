using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ThinkGeo.UI.Blazor.HowDoI
{
    public class MenuService
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public List<MenuModel> GetMenus()
        {
            var menusFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "menus.json");
            return JsonSerializer.Deserialize<List<MenuModel>>(File.ReadAllText(menusFile), JsonOptions);
        }
    }
}
