using System.Web.Mvc;
using System.Web.UI.WebControls;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Styles;

namespace CSharp_HowDoISamples
{
    public partial class InteractiveOverlaysController : Controller
    {
        //
        // GET: /SetImageFormatAndQuality/

        [MapActionFilter]
        public ActionResult SetImageFormatAndQuality(Map map, GeoCollection<object> args)
        {
            if (null == map)
            {
                map = new Map("Map1", new System.Web.UI.WebControls.Unit(100, UnitType.Percentage), 510);

                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(Server.MapPath("~/App_Data/cntry02.shp"));
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 243, 239, 228), GeoColor.FromArgb(255, 218, 193, 163), 1);
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                ShapeFileFeatureLayer majorCitiesShapeLayer = new ShapeFileFeatureLayer(Server.MapPath(@"~\App_Data\cities_a.shp"));
                majorCitiesShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateCompoundCircleStyle(GeoColor.StandardColors.White, 6F, GeoColor.StandardColors.Black, 1F, GeoColor.StandardColors.Black, 3F);
                majorCitiesShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                ShapeFileFeatureLayer majorCitiesLabelLayer = new ShapeFileFeatureLayer(Server.MapPath(@"~\App_Data\cities_a.shp"));
                majorCitiesLabelLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("AREANAME", "Verdana", 8, DrawingFontStyles.Regular, GeoColor.StandardColors.Black);
                majorCitiesLabelLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle.PointPlacement = PointPlacement.UpperCenter;
                majorCitiesLabelLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                LayerOverlay overlay = new LayerOverlay("mapOverlay");
                overlay.TileType = TileType.SingleTile;
                overlay.Layers.Add("WorldLayer", worldLayer);
                overlay.Layers.Add("MajorCitiesShapes", majorCitiesShapeLayer);
                overlay.Layers.Add("MajorCitiesLabels", majorCitiesLabelLayer);

                map.CustomOverlays.Add(overlay);
            }
            else
            {
                if (null != args && args.Count > 0)
                {
                    int quality = System.Convert.ToInt32(args[0]);

                    ((LayerOverlay)map.CustomOverlays["mapOverlay"]).JpegQuality = quality;

                }
            }
            return View(map);
        }

        [MapActionFilter]
        public ActionResult SetImageFormat(Map map, GeoCollection<object> args)
        {
            int quality = System.Convert.ToInt32(args[0]);

            if (WebImageFormat.Jpeg != ((LayerOverlay)map.CustomOverlays["mapOverlay"]).WebImageFormat)
            {
                ((LayerOverlay)map.CustomOverlays["mapOverlay"]).WebImageFormat = WebImageFormat.Jpeg;
                ((LayerOverlay)map.CustomOverlays["mapOverlay"]).JpegQuality = quality;
            }
            else
            {
                ((LayerOverlay)map.CustomOverlays["mapOverlay"]).WebImageFormat = WebImageFormat.Png;
            }

            return View(map);
        }
    }
}
