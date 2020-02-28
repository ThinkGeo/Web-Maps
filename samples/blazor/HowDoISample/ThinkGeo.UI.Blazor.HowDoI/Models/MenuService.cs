using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace ThinkGeo.UI.Blazor.HowDoI
{
    public class MenuService
    {
        public List<MenuModel> GetMenus()
        {
            var menusFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "menus.json");
            return JsonConvert.DeserializeObject<List<MenuModel>>(File.ReadAllText(menusFile));
        }
    }
}
