using System.Web.Http;
using System.Web.Mvc;

namespace TopologyValidation
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
        }
    }
}
