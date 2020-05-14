using System;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace HowDoI
{
    public partial class DrawThematicFeatures : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
            Map1.CurrentExtent = new RectangleShape(-140, 60, 140, -60);
            Map1.MapUnit = GeographyUnit.DecimalDegree;

            // Draw thematic features
            ClassBreakStyle classBreakStyle = new ClassBreakStyle("POP_CNTRY");
            classBreakStyle.ClassBreaks.Add(new ClassBreak(double.MinValue, AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(150, 216, 221, 188))));
            classBreakStyle.ClassBreaks.Add(new ClassBreak(1000000, AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(150, 144, 238, 144))));
            classBreakStyle.ClassBreaks.Add(new ClassBreak(10000000, AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(150, 154, 205, 50))));
            classBreakStyle.ClassBreaks.Add(new ClassBreak(50000000, AreaStyles.CreateSimpleAreaStyle(GeoColor.StandardColors.LightGreen)));
            classBreakStyle.ClassBreaks.Add(new ClassBreak(100000000, AreaStyles.CreateSimpleAreaStyle(GeoColor.StandardColors.DarkGreen)));

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02.shp"));
            worldLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69)));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            worldLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(classBreakStyle);
            Map1.StaticOverlay.Layers.Add("WorldLayer", worldLayer);

            // The following two lines of code enable the client and server caching.
            // If you enable these features it will greatly increase the scalability of your
            // mapping application however there some side effects that may be counter intuitive.
            // Please read the white paper on web caching or the documentation regarding these methods.

            //Map1.StaticOverlay.ClientCache.CacheId = "WorldOverlay";
            //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path);
        }
    }
}
