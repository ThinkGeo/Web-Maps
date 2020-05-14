using System;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples.Labeling
{
    public partial class LabelNiceLookingRoads : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.CurrentExtent = new RectangleShape(-97.766, 30.291, -97.755, 30.286);
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromArgb(255, 233, 232, 214));
                Map1.MapUnit = GeographyUnit.DecimalDegree;

                ShapeFileFeatureLayer austinStreetsShapeLayer = new ShapeFileFeatureLayer(Server.MapPath(@"~\SampleData\USA\Austin\austinstreets.shp"));
                austinStreetsShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.White, 9.2F, GeoColor.StandardColors.DarkGray, 12.2F, true);
                austinStreetsShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                ShapeFileFeatureLayer austinStreetsLabelLayer = new ShapeFileFeatureLayer(Server.MapPath(@"~\SampleData\USA\Austin\austinstreets.shp"));
                austinStreetsLabelLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = WorldStreetsTextStyles.GeneralPurpose("FENAME",9);
                austinStreetsLabelLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                Map1.StaticOverlay.Layers.Add(austinStreetsShapeLayer);
                Map1.StaticOverlay.TileType = TileType.MultipleTile;
                Map1.DynamicOverlay.Layers.Add(austinStreetsLabelLayer);

                // The following two lines of code enable the client and server caching.
                // If you enable these features it will greatly increase the scalability of your
                // mapping application however there some side effects that may be counter intuitive.
                // Please read the white paper on web caching or the documentation regarding these methods.

                //Map1.StaticOverlay.ClientCache.CacheId = "WorldOverlay";
                //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path);
            }
        }
    }
}
