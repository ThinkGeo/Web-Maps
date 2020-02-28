using System.Web.Http;

namespace Visualization
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            //Enable RouteAttribute
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //Enable RouteAttribute, if delete this line, will throw other exceptions
            config.EnsureInitialized();

        }
    }
}
