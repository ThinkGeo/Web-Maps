using System;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace HowDoI.Samples
{
    public partial class UsingMasterPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Map1.CurrentExtent = new RectangleShape(-112.8, 41.7, -111.6, 40.7);
            Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
            Map1.MapUnit = GeographyUnit.DecimalDegree;

            ShapeFileFeatureLayer utahWaterShapeLayer = new ShapeFileFeatureLayer(Server.MapPath(@"~\SampleData\USA\UtahWater.shp"));
            utahWaterShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = WorldStreetsAreaStyles.Water();
            utahWaterShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = WorldStreetsTextStyles.Water("Landname", 6);
            utahWaterShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            Map1.StaticOverlay.Layers.Add(utahWaterShapeLayer);

            // The following two lines of code enable the client and server caching.
            // If you enable these features it will greatly increase the scalability of your
            // mapping application however there some side effects that may be counter intuitive.
            // Please read the white paper on web caching or the documentation regarding these methods.

            //Map1.StaticOverlay.ClientCache.CacheId = "WorldOverlay";
            //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path);
        }
    }
}
