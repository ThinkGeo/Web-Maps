using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Layers;

namespace CSharp_HowDoISamples
{
    public partial class LayersFeatureSourcesController : Controller
    {
        //
        // GET: /CreateASpatialIndexForAShapeFileFeatureLayer/

        public ActionResult CreateASpatialIndexForAShapeFileFeatureLayer()
        {
            return View();
        }

        [MapActionFilter]
        public string Spatial(Map map, GeoCollection<object> args)
        {
            ShapeFileFeatureLayer.BuildIndexFile(Server.MapPath("~/App_Data/cntry02_3857.shp"), BuildIndexMode.DoNotRebuild);
            return "Finish building spatial index. <br/>The index file is <b>" + Server.MapPath("~/App_Data/cntry02_3857.idx") + "</b>";
        }
    }
}
