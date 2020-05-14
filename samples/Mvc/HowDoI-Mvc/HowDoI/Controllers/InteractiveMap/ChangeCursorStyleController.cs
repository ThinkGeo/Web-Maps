using System.Web;
using System.Web.Mvc;

namespace CSharp_HowDoISamples
{
    public partial class InteractiveMapController : Controller
    {
        //
        // GET: /ChangeTheCursorStyle/
        public ActionResult ChangeCursorStyle()
        {
            return View();
        }

        //
        // GET: /ChangeTheCursorStyle/
        public string GetCustomCursor()
        {
            string cursorAddr = VirtualPathUtility.ToAbsolute("~/content/images/cursor.cur");
            return cursorAddr;
        }
    }
}
