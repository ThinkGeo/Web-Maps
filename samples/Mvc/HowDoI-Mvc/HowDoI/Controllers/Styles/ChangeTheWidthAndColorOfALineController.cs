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
        // GET: /ChangeTheWidthAndColorOfALine/

        public ActionResult ChangeTheWidthAndColorOfALine()
        {
            return View();
        }

        [MapActionFilter]
        public void ChangeLineType(Map map, GeoCollection<object> args)
        {
            if (null != map)
            {
                FeatureLayer streetLayer = (ShapeFileFeatureLayer)map.StaticOverlay.Layers["Austin"];

                string lineType = args[0] as string;

                switch (lineType)
                {
                    case "wider":
                        streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Width += 2;
                        streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.OuterPen.Width += 2;
                        map.StaticOverlay.Redraw();
                        break;
                    case "narrow":
                        if (streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.OuterPen.Width > 2)
                        {
                            streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Width -= 2;
                            streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.OuterPen.Width -= 2;
                        }
                        map.StaticOverlay.Redraw();
                        break;
                    case "lineColorYellow":
                        streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Color = GeoColor.StandardColors.LightYellow;
                        map.StaticOverlay.Redraw();
                        break;
                    case "lineColorPink":
                        streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Color = GeoColor.StandardColors.LightPink;
                        map.StaticOverlay.Redraw();
                        break;
                }
            }
        }

    }
}
