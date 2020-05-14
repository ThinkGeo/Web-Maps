using System;
using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Mvc;

namespace CSharp_HowDoISamples
{
    public partial class InteractiveOverlaysController : Controller
    {
        //
        // GET: /ClickEventOnMarker/
        private readonly string[] markerIcons = new string[] { "marker_blue.gif", "marker.gif", "marker_gold.gif", "marker_green.gif" };

        public ActionResult ClickEventOnMarker()
        {
            return View();
        }

        [MapActionFilter]
        public string ClickOnMarker(Map map, GeoCollection<object> args)
        {
            int times = 0;
            if (Session["ClickTimes"] == null)
            {
                times = 1;
            }
            else
            {
                times = (int)Session["ClickTimes"];
                times++;
            }
            Session["ClickTimes"] = times;

            int iconIndex = times % 4 + 1;
            if (iconIndex >= 4)
            {
                iconIndex -= 4;
            }

            string iconPath = Url.Content("~/Content/images/") + markerIcons[iconIndex];
            InMemoryMarkerOverlay markerOverlay = (InMemoryMarkerOverlay)map.CustomOverlays["MarkerOverlay"];
            markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.ImageVirtualPath = iconPath;
            markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.ImageOffsetX = -10.5f;
            markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.ImageOffsetY = 25f;

            return String.Format("You have clicked the marker <span style='color:red;font-weight:bolder;font-size:15;'>{0}</span> time(s)", times);
        }
    }
}
