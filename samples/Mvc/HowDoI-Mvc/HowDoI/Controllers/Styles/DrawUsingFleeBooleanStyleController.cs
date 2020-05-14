using System.Web.Mvc;
using ThinkGeo.MapSuite.Mvc;

namespace CSharp_HowDoISamples_for_Debug
{
    public partial class StylesController : Controller
    {
        //
        // GET: /DrawUsingFleeBooleanStyle/
        [MapActionFilter]
        public ActionResult DrawUsingFleeBooleanStyle()
        {
            return View();
        }

    }
}
