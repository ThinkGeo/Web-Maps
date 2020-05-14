using System;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace HowDoI.Samples.Labeling
{
    public partial class DrawCurvedLabels : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.DecimalDegree;
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromArgb(255, 233, 232, 214));
                Map1.CurrentExtent = new RectangleShape(-97.6881803712033, 30.3177912428115, -97.6723016938352, 30.3064615919325);

                ShapeFileFeatureLayer austinStreetsShapeLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/USA/Austin/austinstreets.shp"));
                austinStreetsShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                austinStreetsShapeLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.White, 9.2F, GeoColor.StandardColors.DarkGray, 12.2F, true));

                ShapeFileFeatureLayer austinStreetsLabelLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/USA/Austin/austinstreets.shp"));
                austinStreetsLabelLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                TextStyle textStyle = WorldStreetsTextStyles.GeneralPurpose("FENAME",9);
                textStyle.TextLineSegmentRatio = double.MaxValue;
                textStyle.SplineType = SplineType.StandardSplining;
                austinStreetsLabelLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(textStyle);

                Map1.StaticOverlay.Layers.Add("AustinStreetsShapeLayer", austinStreetsShapeLayer);
                Map1.StaticOverlay.Layers.Add("AustinStreetsLabelLayer", austinStreetsLabelLayer);
            }
        }
    }
}
