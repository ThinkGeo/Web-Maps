/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Layers;

namespace CSharp_HowDoISamples
{
    public partial class LayersFeatureSourcesController : Controller
    {
        //
        // GET: /LoadAHeatLayer/

        public ActionResult LoadAHeatLayer()
        {
            Map map = new Map("Map1",
                new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage),
              new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage));
            map.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
            map.MapUnit = GeographyUnit.Meter;
            map.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            map.CurrentExtent = new RectangleShape(-10228034.814086, 5337883.48150295, -7779006.01663396, 3485566.81262866);

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            map.CustomOverlays.Add(backgroundOverlay);
            //Shapefile Containing historical earthquake data
            ShapeFileFeatureSource featureSource = new ShapeFileFeatureSource(Server.MapPath("~/App_Data/quksigx020.shp"));
            featureSource.Projection = new Proj4Projection(4326, 3857);
            featureSource.CustomColumnFetch += new EventHandler<CustomColumnFetchEventArgs>(featureSource_CustomColumnFetch);

            HeatLayer heatLayer = new HeatLayer(featureSource);
            heatLayer.HeatStyle = new HeatStyle(180, "EarthQuakeMagnitude", 0, 12);

            LayerOverlay staticOverlay = new LayerOverlay();
            staticOverlay.IsBaseOverlay = false;
            staticOverlay.Layers.Add(heatLayer);
            map.CustomOverlays.Add(staticOverlay);

            return View(map);
        }

        private void featureSource_CustomColumnFetch(object sender, CustomColumnFetchEventArgs e)
        {
            Feature earthQuake = ((ShapeFileFeatureSource)sender).GetFeatureById(e.Id, ReturningColumnsType.AllColumns);

            e.ColumnValue = earthQuake.ColumnValues["OTHER_MAG1"];
        }
    }
}
