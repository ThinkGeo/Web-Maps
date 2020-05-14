/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Collections.ObjectModel;
using System.Text;
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
        // GET: /FindTheFeatureTheUserClicked/

        public ActionResult FindFeatureClicked()
        {
            Map map = new Map("Map1",
                    new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage),
                    new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage));
            map.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
            map.MapUnit = GeographyUnit.Meter;
            map.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            map.CurrentExtent = new RectangleShape(-13914936.3491592, 11753184.6153385, 5565974.53966368, -5780349.22025635);

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            map.CustomOverlays.Add(backgroundOverlay);

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(ControllerContext.HttpContext.Server.MapPath("~/App_Data/Countries02.shp"));
            worldLayer.FeatureSource.Projection = new Proj4Projection(4326, 3857);
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.SimpleColors.Transparent, GeoColor.FromArgb(100, GeoColor.SimpleColors.Green));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay staticOverlay = new LayerOverlay("WorldStaticOverlay");
            staticOverlay.IsBaseOverlay = false;
            staticOverlay.Layers.Add("WorldLayer", worldLayer);

            InMemoryFeatureLayer highlightLayer = new InMemoryFeatureLayer();
            highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(100, 60, 180, 60), GeoColor.GeographicColors.DeepOcean);
            highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay dynamicOverlay = new LayerOverlay("HightLightDynamicOverlay");
            dynamicOverlay.Layers.Add("HighLightLayer", highlightLayer);
            dynamicOverlay.IsBaseOverlay = false;

            map.CustomOverlays.Add(staticOverlay);
            map.CustomOverlays.Add(dynamicOverlay);
            map.Popups.Add(new CloudPopup("information") { AutoSize = true, IsVisible = false });

            return View(map);
        }

        [MapActionFilter]
        public string FindFeatureClick(Map map, GeoCollection<object> args)
        {
            PointShape position = new PointShape(double.Parse(args[0].ToString()), double.Parse(args[1].ToString()));

            LayerOverlay staticOverlay = (LayerOverlay)map.CustomOverlays["WorldStaticOverlay"];
            LayerOverlay dynamicOverlay = (LayerOverlay)map.CustomOverlays["HightLightDynamicOverlay"];

            ShapeFileFeatureLayer shapeFileLayer = (ShapeFileFeatureLayer)(staticOverlay.Layers["WorldLayer"]);
            InMemoryFeatureLayer highLightLayer = (InMemoryFeatureLayer)(dynamicOverlay.Layers["HighLightLayer"]);
            highLightLayer.InternalFeatures.Clear();

            shapeFileLayer.Open();
            Collection<Feature> selectedFeatures = shapeFileLayer.QueryTools.GetFeaturesContaining(position, new string[] { "CNTRY_NAME", "POP_CNTRY", "COLOR_MAP" });
            shapeFileLayer.Close();

            foreach (Feature feature in selectedFeatures)
            {
                highLightLayer.InternalFeatures.Add(feature.Id, feature);
            }

            return GetPopupContent(selectedFeatures);
        }

        private static string GetPopupContent(Collection<Feature> features)
        {
            string content;
            if (features.Count > 0)
            {
                StringBuilder message = new StringBuilder();
                message.AppendFormat("<li>Country Name : {0}</li>", features[0].ColumnValues["CNTRY_NAME"].Trim());
                message.AppendFormat("<li>Country Population : {0}</li>", features[0].ColumnValues["POP_CNTRY"].Trim());
                message.AppendFormat("<li>Map Color : {0}</li>", features[0].ColumnValues["COLOR_MAP"].Trim());
                string messageInPopup = String.Format("<div class='normalBlueTx'>{0}</div>", message.ToString());

                content = messageInPopup;
            }
            else
            {
                content = @"<div class='normalBlueTx'>Please click on a country to show its information.</div>";
            }
            return content;
        }
    }
}
