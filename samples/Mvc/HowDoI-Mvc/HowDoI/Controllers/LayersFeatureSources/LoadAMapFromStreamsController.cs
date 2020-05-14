/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.IO;
using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Styles;

namespace CSharp_HowDoISamples
{
    public partial class LayersFeatureSourcesController : Controller
    {
        //
        // GET: /LoadAMapFromStreams/

        public ActionResult LoadAMapFromStreams()
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

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer("cntry02_3857.shp");
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.SimpleColors.Transparent, GeoColor.FromArgb(100, GeoColor.SimpleColors.Green));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            ((ShapeFileFeatureSource)worldLayer.FeatureSource).StreamLoading += new EventHandler<StreamLoadingEventArgs>(LoadAMapFromStreams_StreamLoading);

            LayerOverlay staticOverlay = new LayerOverlay();
            staticOverlay.IsBaseOverlay = false;
            staticOverlay.Layers.Add(worldLayer);
            map.CustomOverlays.Add(staticOverlay);

            return View(map);
        }

        private void LoadAMapFromStreams_StreamLoading(object sender, StreamLoadingEventArgs e)
        {
            string fileName = Path.GetFileName(e.AlternateStreamName);
            e.AlternateStream = new FileStream(Server.MapPath("~/App_Data/" + fileName), (FileMode)e.FileMode, (FileAccess)e.ReadWriteMode);
        }
    }
}
