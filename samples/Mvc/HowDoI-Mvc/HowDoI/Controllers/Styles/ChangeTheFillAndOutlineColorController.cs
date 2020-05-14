using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Mvc;

namespace CSharp_HowDoISamples_for_Debug
{
    public partial class StylesController : Controller
    {
        //
        // GET: /ChangeTheFillAndOutlineColor/

        public ActionResult ChangeTheFillAndOutlineColor()
        {
            return View();
        }

        [MapActionFilter]
        public void ChangeTheColor(Map map, GeoCollection<object> args)
        {
            if (null != map)
            {
                FeatureLayer worldLayer = (ShapeFileFeatureLayer)map.StaticOverlay.Layers["WorldLayer"];

                string color = args[0] as string;

                switch (color)
                {
                    case "yellow":
                        worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.StandardColors.LightGoldenrodYellow;
                        break;
                    case "gray":
                        worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.StandardColors.Gray;
                        break;
                    case "green":
                        worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.StandardColors.LightGreen;
                        break;
                    case "black":
                        worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.StandardColors.Black;
                        break;
                }
            }
        }
    }
}
