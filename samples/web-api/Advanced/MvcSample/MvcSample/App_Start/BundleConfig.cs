using System.Web;
using System.Web.Optimization;

namespace MvcSample
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-1.11.1.min.js"));
             
            bundles.Add(new ScriptBundle("~/bundles/openlayers").Include(
                        "~/Scripts/thinkgeo.openlayers.js"));

            bundles.Add(new ScriptBundle("~/bundles/index").Include(
                        "~/Scripts/index.js"));

 
            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/bootstrap.min.css",
                        "~/Content/ol.css",
                        "~/Content/site.css",
                        "~/Content/thinkgeo.openlayers.css"));
             

        }
    }
}