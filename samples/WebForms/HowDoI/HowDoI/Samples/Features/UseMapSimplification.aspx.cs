using System;
using System.Globalization;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace HowDoI.Samples.Features
{
    public partial class UseMapSimplification : System.Web.UI.Page
    {
        private static AreaBaseShape areaBaseShape;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.DecimalDegree;
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-177.39584350585937, 83.113876342773437, -52.617362976074219, 14.550546646118164);

                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02.shp"));
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 243, 239, 228), GeoColor.FromArgb(255, 218, 193, 163), 1);
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                worldLayer.Open();
                Feature feature = worldLayer.QueryTools.GetFeatureById("135", new string[0]);
                areaBaseShape = (AreaBaseShape)feature.GetShape();
                worldLayer.Close();

                InMemoryFeatureLayer simplificationLayer = new InMemoryFeatureLayer();
                simplificationLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                simplificationLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.StandardColors.Transparent, GeoColor.FromArgb(255, 118, 138, 69));
                simplificationLayer.InternalFeatures.Add(feature);

                Map1.StaticOverlay.Layers.Add("SimplificationLayer", simplificationLayer);
            }
        }

        protected void btnSimplify_Click(object sender, EventArgs e)
        {
            InMemoryFeatureLayer simplificationLayer = (InMemoryFeatureLayer)Map1.StaticOverlay.Layers["SimplificationLayer"];

            double tolerance = Convert.ToDouble(ddlTolerance.SelectedItem.Text, CultureInfo.InvariantCulture);
            SimplificationType simplificationType = (SimplificationType)ddlsimplification.SelectedIndex;

            MultipolygonShape multipolygonShape = areaBaseShape.Simplify(tolerance, simplificationType);
            simplificationLayer.InternalFeatures.Clear();
            simplificationLayer.InternalFeatures.Add(new Feature(multipolygonShape));
        }
    }
}
