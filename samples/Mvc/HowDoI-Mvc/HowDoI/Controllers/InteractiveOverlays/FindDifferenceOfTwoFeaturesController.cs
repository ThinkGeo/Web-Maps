using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;

namespace CSharp_HowDoISamples
{
    public partial class InteractiveOverlaysController : Controller
    {
        //
        // GET: /FindDifferenceOfTwoFeatures/

        public ActionResult FindDifferenceOfTwoFeatures()
        {
            return View();
        }

        [MapActionFilter]
        public void DifferenceFeature(Map map, GeoCollection<object> args)
        {
            if (null != map)
            {
                InMemoryFeatureLayer mapShapeLayer = (InMemoryFeatureLayer)((LayerOverlay)map.CustomOverlays["OverLayer"]).Layers["InMemoryFeatureLayer"];

                if (mapShapeLayer.InternalFeatures.Count > 1)
                {
                    AreaBaseShape targetShape = (AreaBaseShape)mapShapeLayer.InternalFeatures["AreaShape1"].GetShape();

                    mapShapeLayer.Open();
                    mapShapeLayer.EditTools.BeginTransaction();
                    mapShapeLayer.EditTools.GetDifference("AreaShape2", targetShape);
                    mapShapeLayer.EditTools.Delete("AreaShape1");
                    mapShapeLayer.EditTools.CommitTransaction();
                    mapShapeLayer.Close();
                    mapShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.FromArgb(100, GeoColor.StandardColors.Blue);
                }
                ((LayerOverlay)map.CustomOverlays["OverLayer"]).Redraw();
            }
        }
    }
}
