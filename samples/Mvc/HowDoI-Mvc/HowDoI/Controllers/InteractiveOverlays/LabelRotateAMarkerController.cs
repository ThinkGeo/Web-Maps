using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Mvc;

namespace CSharp_HowDoISamples
{
    public partial class InteractiveOverlaysController : Controller
    {
        //
        // GET: /LabelRotateAMarker/

        public ActionResult LabelRotateAMarker()
        {
            return View();
        }

        [MapActionFilter]
        public void ShowTextCheckBox(Map map, GeoCollection<object> args)
        {
            if (null != map)
            {
                string checkedString = args[0] as string;

                InMemoryMarkerOverlay markerOverlay = (InMemoryMarkerOverlay)map.CustomOverlays["MarkerOverlay"];
                if ("checked" == checkedString)
                {
                    markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.Text = "Vehicle";
                    markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.FontColor = GeoColor.StandardColors.OrangeRed;
                    markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.FontStyle = new GeoFont("Arail Black", 12, DrawingFontStyles.Bold);
                    markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.TextOffsetX = 7F;
                    markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.TextOffsetY = 3F;
                }
                else
                {
                    markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.Text = string.Empty;
                }
            }
        }

        [MapActionFilter]
        public void ShowRotation(Map map, GeoCollection<object> args)
        {
            if (null != map)
            {
                InMemoryMarkerOverlay markerOverlay = (InMemoryMarkerOverlay)map.CustomOverlays["MarkerOverlay"];
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage.RotationAngle += 30;
            }
        }
    }
}
