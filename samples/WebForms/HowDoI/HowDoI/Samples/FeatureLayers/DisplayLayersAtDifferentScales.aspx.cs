using System;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace HowDoI
{
    public partial class DisplayLayersAtDifferentScales : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.MapTools.ScaleLine.Enabled = true;
                Map1.CurrentExtent = new RectangleShape(-195.7852, 91.2020, 0.2821, -30.6353);
                Map1.MapUnit = GeographyUnit.DecimalDegree;

                Map1.MapTools.MouseMapTool.Enabled = false;

                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02.shp"));
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 243, 239, 228), GeoColor.FromArgb(255, 218, 193, 163), 1);
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                ShapeFileFeatureLayer citiesLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/USA/cities_a.shp"));
                citiesLayer.ZoomLevelSet.ZoomLevel05.DefaultPointStyle = PointStyles.CreateCompoundCircleStyle(GeoColor.StandardColors.White, 6F, GeoColor.StandardColors.Black, 1F, GeoColor.StandardColors.Black, 3F);
                citiesLayer.ZoomLevelSet.ZoomLevel05.DefaultTextStyle = new TextStyle("AREANAME", new GeoFont("Verdana", 9), new GeoSolidBrush(GeoColor.StandardColors.Black));
                citiesLayer.ZoomLevelSet.ZoomLevel05.DefaultTextStyle.HaloPen = new GeoPen(GeoColor.StandardColors.White, 2);
                citiesLayer.ZoomLevelSet.ZoomLevel05.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                ShapeFileFeatureLayer streetsLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/USA/Austin/austinstreets.shp"));
                streetsLayer.ZoomLevelSet.ZoomLevel10.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.White, 3F, GeoColor.StandardColors.DarkGray, 5F, true);
                streetsLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                Map1.StaticOverlay.Layers.Add(worldLayer);
                Map1.StaticOverlay.Layers.Add(citiesLayer);
                Map1.StaticOverlay.Layers.Add(streetsLayer);

                // The following two lines of code enable the client and server caching.
                // If you enable these features it will greatly increase the scalability of your
                // mapping application however there some side effects that may be counter intuitive.
                // Please read the white paper on web caching or the documentation regarding these methods.

                //Map1.StaticOverlay.ClientCache.CacheId = "WorldOverlay";
                //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path);
                
                ZoomLevelSet zoomLevelSet = new ZoomLevelSet();
                Map1.ZoomToScale(10000);
            }
        }

        protected void btnLow_Click(object sender, EventArgs e)
        {
            ZoomLevelSet zoomLevelSet = new ZoomLevelSet();
            Map1.ZoomToScale(10000);
        }

        protected void btnNoraml_Click(object sender, EventArgs e)
        {
            ZoomLevelSet zoomLevelSet = new ZoomLevelSet();
            Map1.ZoomToScale(10000000);
        }

        protected void btnHigh_Click(object sender, EventArgs e)
        {
            ZoomLevelSet zoomLevelSet = new ZoomLevelSet();
            Map1.ZoomToScale(100000000);
        }
    }
}
