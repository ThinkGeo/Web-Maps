/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples.Features
{
    public partial class ShortestLineBetweenFeatures : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-8956259.3203343, 7303601.534613, 7255728.5407719, -1100802.5527076);

                // Please input your ThinkGeo Cloud API Key to enable the background map.
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                Map1.CustomOverlays.Add(backgroundOverlay);

                InMemoryFeatureLayer mapShapeLayer = new InMemoryFeatureLayer();
                mapShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.FromArgb(125, 141, 255, 141);
                mapShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.GeographicColors.DeepOcean;
                mapShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                mapShapeLayer.InternalFeatures.Add("AreaShape1", new Feature(BaseShape.CreateShapeFromWellKnownData("POLYGON((-10 40,40 70,50 0,-10 40))")));
                mapShapeLayer.InternalFeatures.Add("AreaShape2", new Feature(new EllipseShape(new PointShape(-7792364.35552915, -2273030.92698769), 1000000, 2000000)));

                InMemoryFeatureLayer shortestLineLayer = new InMemoryFeatureLayer();
                shortestLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.Red, 2, false);
                shortestLineLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
               
                LayerOverlay dynamicOverlay = new LayerOverlay();
                dynamicOverlay.Layers.Add("InMemoryFeatureLayer", mapShapeLayer);
                dynamicOverlay.Layers.Add("ShortestLineLayer", shortestLineLayer);
                dynamicOverlay.IsBaseOverlay = false;
                dynamicOverlay.TileType = TileType.SingleTile;
                Map1.CustomOverlays.Add(dynamicOverlay);
            }
        }

        protected void btnGetShortestLine_Click(object sender, EventArgs e)
        {
            InMemoryFeatureLayer mapShapeLayer = (InMemoryFeatureLayer)((LayerOverlay)Map1.CustomOverlays[1]).Layers["InMemoryFeatureLayer"];
            InMemoryFeatureLayer shortestLineLayer = (InMemoryFeatureLayer)((LayerOverlay)Map1.CustomOverlays[1]).Layers["ShortestLineLayer"];

            BaseShape areaShape1 = mapShapeLayer.InternalFeatures["AreaShape1"].GetShape();
            BaseShape areaShape2 = mapShapeLayer.InternalFeatures["AreaShape2"].GetShape();
            MultilineShape multiLineShape = areaShape1.GetShortestLineTo(areaShape2, GeographyUnit.Meter);

            shortestLineLayer.InternalFeatures.Clear();
            shortestLineLayer.InternalFeatures.Add("ShortestLine", new Feature(multiLineShape));
            ((LayerOverlay)Map1.CustomOverlays[1]).Redraw();
        }
    }
}
