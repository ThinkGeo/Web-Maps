using System.Web.Mvc;

namespace ThinkGeo.MapSuite.EarthquakeStatistics
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}