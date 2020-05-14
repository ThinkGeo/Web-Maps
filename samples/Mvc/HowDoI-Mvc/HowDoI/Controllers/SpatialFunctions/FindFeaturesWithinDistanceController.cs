/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Styles;

namespace CSharp_HowDoISamples
{
    public partial class SpatialFunctionsController : Controller
    {
        //
        // GET: /FindFeaturesWithinDistance/

        public ActionResult FindFeaturesWithinDistance()
        {
            Map map = new Map("Map1", new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage),
                new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage));
            map.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#B3C6D4"));
            map.MapUnit = GeographyUnit.Meter;
            map.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            map.CurrentExtent = new RectangleShape(-13914936.3491592, 11753184.6153385, 5565974.53966368, -5780349.22025635);

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            map.CustomOverlays.Add(backgroundOverlay);

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(Server.MapPath("~/App_Data/cntry02_3857.shp"));
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.SimpleColors.Transparent, GeoColor.FromArgb(100, GeoColor.SimpleColors.Green));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay staticOverlay = new LayerOverlay("StaticOverlay");
            staticOverlay.IsBaseOverlay = false;
            staticOverlay.Layers.Add("WorldLayer", worldLayer);
            map.CustomOverlays.Add(staticOverlay);

            InMemoryFeatureLayer highlightLayer = new InMemoryFeatureLayer();
            highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateCompoundPointStyle(PointSymbolType.Square, GeoColor.StandardColors.White, GeoColor.StandardColors.Black, 1F, 8F, PointSymbolType.Square, GeoColor.StandardColors.Navy, GeoColor.StandardColors.Transparent, 0F, 4F);
            highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(150, 154, 205, 50));
            highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay dynamicOverlay = new LayerOverlay("DynamicOverlay");
            dynamicOverlay.IsBaseOverlay = false;
            dynamicOverlay.TileType = TileType.SingleTile;
            dynamicOverlay.Layers.Add("HighlightLayer", highlightLayer);
            map.CustomOverlays.Add(dynamicOverlay);

            return View(map);
        }

        [MapActionFilter]
        public void FindQueriedFeatures(Map map, GeoCollection<object> args)
        {
            double distance = Convert.ToDouble(args[0].ToString(), CultureInfo.InvariantCulture);
            double clickX = Convert.ToDouble(args[1].ToString(), CultureInfo.InvariantCulture);
            double clickY = Convert.ToDouble(args[2].ToString(), CultureInfo.InvariantCulture);
            PointShape pointShape = new PointShape(clickX, clickY);

            LayerOverlay staticOverlay = (LayerOverlay)map.CustomOverlays["StaticOverlay"];
            LayerOverlay dynamicOverlay = (LayerOverlay)map.CustomOverlays["DynamicOverlay"];
            FeatureLayer worldLayer = (FeatureLayer)staticOverlay.Layers["WorldLayer"];
            InMemoryFeatureLayer highlightLayer = (InMemoryFeatureLayer)dynamicOverlay.Layers["HighlightLayer"];

            worldLayer.Open();
            Collection<Feature> selectedFeatures = worldLayer.QueryTools.GetFeaturesWithinDistanceOf(pointShape, GeographyUnit.Meter, DistanceUnit.Kilometer, distance, new string[0]);
            worldLayer.Close();

            if (highlightLayer.InternalFeatures.Count > 0)
            {
                highlightLayer.InternalFeatures.Clear();
            }

            highlightLayer.InternalFeatures.Add("Point", new Feature(pointShape));
            foreach (Feature feature in selectedFeatures)
            {
                highlightLayer.InternalFeatures.Add(feature.Id, feature);
            }

            dynamicOverlay.Redraw();
        }
    }
}
