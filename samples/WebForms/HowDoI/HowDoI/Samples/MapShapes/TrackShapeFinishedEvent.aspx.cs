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

namespace HowDoI.Samples.MapShapes
{
    public partial class TrackShapeFinishedEvent : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-13064290.945669, 6003560.5647606, -9011293.9803927, 3902459.5429304);

                // Please input your ThinkGeo Cloud API Key to enable the background map. 
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                Map1.CustomOverlays.Add(backgroundOverlay);

                InMemoryFeatureLayer shapeLayer = new InMemoryFeatureLayer();
                shapeLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.FromArgb(180, 102, 255, 102), 10, GeoColor.StandardColors.DarkGreen, 1);
                shapeLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.Green, 4, true);
                shapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(180, 102, 255, 102), GeoColor.StandardColors.DarkGreen, 1);
                shapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                shapeLayer.DrawingQuality = DrawingQuality.HighQuality;

                LayerOverlay dynamicOverlay = new LayerOverlay("DynamicOverlay");
                dynamicOverlay.IsBaseOverlay = false;
                dynamicOverlay.TileType = TileType.SingleTile;
                dynamicOverlay.Layers.Add("shapeLayer", shapeLayer);
                Map1.CustomOverlays.Add(dynamicOverlay);
            }
        }

        protected void buttonDrawPoint_Click(object sender, ImageClickEventArgs e)
        {
            Map1.EditOverlay.TrackMode = TrackMode.Point;
        }

        protected void buttonDrawLine_Click(object sender, ImageClickEventArgs e)
        {
            Map1.EditOverlay.TrackMode = TrackMode.Line;
        }

        protected void buttonDrawRectangle_Click(object sender, ImageClickEventArgs e)
        {
            Map1.EditOverlay.TrackMode = TrackMode.Rectangle;
        }

        protected void buttonDrawSquare_Click(object sender, ImageClickEventArgs e)
        {
            Map1.EditOverlay.TrackMode = TrackMode.Square;
        }

        protected void buttonDrawPolygon_Click(object sender, ImageClickEventArgs e)
        {
            Map1.EditOverlay.TrackMode = TrackMode.Polygon;
        }

        protected void buttonDrawCircle_Click(object sender, ImageClickEventArgs e)
        {
            Map1.EditOverlay.TrackMode = TrackMode.Circle;
        }

        protected void buttonDrawEllipse_Click(object sender, ImageClickEventArgs e)
        {
            Map1.EditOverlay.TrackMode = TrackMode.Ellipse;
        }

        protected void buttonEditShape_Click(object sender, ImageClickEventArgs e)
        {
            LayerOverlay dynamicOverlay = (LayerOverlay)Map1.CustomOverlays["DynamicOverlay"];
            InMemoryFeatureLayer shapeLayer = (InMemoryFeatureLayer)dynamicOverlay.Layers["shapeLayer"];

            foreach (Feature feature in shapeLayer.InternalFeatures)
            {
                Map1.EditOverlay.Features.Add(feature.Id, feature);
            }

            shapeLayer.InternalFeatures.Clear();
            Map1.EditOverlay.TrackMode = TrackMode.Edit;
            dynamicOverlay.Redraw();
        }

        protected void Map1_TrackShapeFinished(object sender, EventArgs e)
        {
            LayerOverlay dynamicOverlay = (LayerOverlay)Map1.CustomOverlays["DynamicOverlay"];
            InMemoryFeatureLayer shapeLayer = (InMemoryFeatureLayer)dynamicOverlay.Layers["shapeLayer"];

            foreach (Feature feature in Map1.EditOverlay.Features)
            {
                shapeLayer.InternalFeatures.Add(feature.Id, feature);
            }

            Map1.EditOverlay.Features.Clear();
            dynamicOverlay.Redraw();
        }
    }
}
