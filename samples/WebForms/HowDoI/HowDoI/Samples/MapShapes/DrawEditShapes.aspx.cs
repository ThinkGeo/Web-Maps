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

namespace HowDoI
{
    public partial class DrawEditShapes : System.Web.UI.Page
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

       protected void buttonSubmit_Click(object sender, EventArgs e)
        {
            LayerOverlay dynamicOverlay = (LayerOverlay)Map1.CustomOverlays["DynamicOverlay"];
            InMemoryFeatureLayer shapeLayer = (InMemoryFeatureLayer)dynamicOverlay.Layers["shapeLayer"];

            foreach (Feature feature in Map1.EditOverlay.Features)
            {
                if (!shapeLayer.InternalFeatures.Contains(feature.Id))
                {
                    shapeLayer.InternalFeatures.Add(feature.Id, feature);
                }
            }

            Map1.EditOverlay.Features.Clear();
            Map1.EditOverlay.TrackMode = TrackMode.None;
            dynamicOverlay.Redraw();
        }

        protected void buttonEditShape_Click(object sender, EventArgs e)
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
    }
}
