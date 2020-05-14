using System.Web;
using ThinkGeo.MapSuite.WmsServer;

namespace ThinkGeo.MapSuite.CustomWmsServer
{
    public class CustomWmsHandler : WmsHandler
    {
        public CustomWmsHandler()
        {
            CapabilitiesFilePathName = HttpContext.Current.Server.MapPath(@"~\CapabilitiesXml\WmsHandlerCapabilities.xml");
        }
    }
}