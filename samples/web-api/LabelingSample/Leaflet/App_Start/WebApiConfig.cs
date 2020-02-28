using System.Web.Http;

namespace Labeling
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();
            config.EnsureInitialized();
        }
    }
}
