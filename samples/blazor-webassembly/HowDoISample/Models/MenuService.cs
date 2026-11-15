using System.Text.Json;

namespace ThinkGeo.UI.Blazor.HowDoI;

public sealed class MenuService
{
    private readonly List<MenuModel> menus;

    public MenuService()
    {
        using var stream = typeof(MenuService).Assembly.GetManifestResourceStream(
            "ThinkGeo.UI.Blazor.HowDoI.wwwroot.menus.json");
        menus = JsonSerializer.Deserialize<List<MenuModel>>(stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public List<MenuModel> GetMenus() => menus;

    public string GetSource(MenuModel menu)
    {
        if (menu == null || !menus.Contains(menu)) return "Source unavailable.";
        using var stream = typeof(MenuService).Assembly.GetManifestResourceStream(
            $"ThinkGeo.UI.Blazor.HowDoI.Pages.{menu.Source}.razor");
        if (stream == null) return "Source unavailable.";
        using var reader = new System.IO.StreamReader(stream);
        return reader.ReadToEnd();
    }
}
