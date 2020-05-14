using System.Web;
using System.Web.Mvc;

namespace ThinkGeo.MapSuite.SiteSelection
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}