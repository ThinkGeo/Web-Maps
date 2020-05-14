using System;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace HowDoI.Samples.Labeling
{
    public partial class ChangePointLabelPlacement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-131.22, 55.05, -54.03, 16.91);
                Map1.MapUnit = GeographyUnit.DecimalDegree;

                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02.shp"));
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 243, 239, 228), GeoColor.FromArgb(255, 218, 193, 163), 1);
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                ShapeFileFeatureLayer majorCitiesShapeLayer = new ShapeFileFeatureLayer(Server.MapPath(@"~\SampleData\USA\cities_a.shp"));
                majorCitiesShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateCompoundCircleStyle(GeoColor.StandardColors.White, 6F, GeoColor.StandardColors.Black, 1F, GeoColor.StandardColors.Black, 3F);
                majorCitiesShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                ShapeFileFeatureLayer majorCitiesLabelLayer = new ShapeFileFeatureLayer(Server.MapPath(@"~\SampleData\USA\cities_a.shp"));
                majorCitiesLabelLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("AREANAME", "Verdana", 8, DrawingFontStyles.Regular, GeoColor.StandardColors.Black);
                majorCitiesLabelLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                Map1.StaticOverlay.Layers.Add(worldLayer);
                Map1.StaticOverlay.Layers.Add(majorCitiesShapeLayer);
                Map1.DynamicOverlay.Layers.Add("MajorCitiesLabels", majorCitiesLabelLayer);

                // The following two lines of code enable the client and server caching.
                // If you enable these features it will greatly increase the scalability of your
                // mapping application however there some side effects that may be counter intuitive.
                // Please read the white paper on web caching or the documentation regarding these methods.

                //Map1.StaticOverlay.ClientCache.CacheId = "WorldOverlay";
                //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path);
            }
        }

        protected void PointPlacementDropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            PointPlacement placement;
            switch (PointPlacementDropDownList.SelectedValue)
            {
                case "Center":
                    placement = PointPlacement.Center;
                    break;
                case "CenterLeft":
                    placement = PointPlacement.CenterLeft;
                    break;
                case "CenterRight":
                    placement = PointPlacement.CenterRight;
                    break;
                case "LowerCenter":
                    placement = PointPlacement.LowerCenter;
                    break;
                case "LowerLeft":
                    placement = PointPlacement.LowerLeft;
                    break;
                case "LowerRight":
                    placement = PointPlacement.LowerRight;
                    break;
                case "UpperCenter":
                    placement = PointPlacement.UpperCenter;
                    break;
                case "UpperLeft":
                    placement = PointPlacement.UpperLeft;
                    break;
                case "UpperRight":
                    placement = PointPlacement.UpperRight;
                    break;
                default:
                    placement = PointPlacement.CenterRight;
                    break;
            }
            FeatureLayer labelPlacementLayer = (FeatureLayer)Map1.DynamicOverlay.Layers["MajorCitiesLabels"];
            labelPlacementLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle.PointPlacement = placement;
            Map1.DynamicOverlay.Redraw();
        }
    }
}
