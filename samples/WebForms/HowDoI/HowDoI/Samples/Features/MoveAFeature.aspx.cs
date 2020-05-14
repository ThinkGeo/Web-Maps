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
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples.Features
{
    public partial class MoveAFeature : System.Web.UI.Page
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

                BaseShape feature = new EllipseShape(new PointShape(0, 3503549.84350437), 2000000, 1000000);
                feature.Id = "MutlipointShape";
                mapShapeLayer.InternalFeatures.Add(feature.Id, new Feature(feature));

                LayerOverlay dynamicOverlay = new LayerOverlay();
                dynamicOverlay.TileType = TileType.SingleTile;
                dynamicOverlay.Layers.Add("mapShapeLayer", mapShapeLayer);
                dynamicOverlay.IsBaseOverlay = false;
                Map1.CustomOverlays.Add(dynamicOverlay);
            }
        }

        protected void btnMoveRight_Click(object sender, ImageClickEventArgs e)
        {
            TranslateByOffset(1000000, 0);
        }

        protected void btnMoveLeft_Click(object sender, ImageClickEventArgs e)
        {
            TranslateByOffset(-1000000, 0);
        }

        protected void btnMoveUp_Click(object sender, ImageClickEventArgs e)
        {
            TranslateByOffset(0, 1000000);
        }

        protected void btnMoveDown_Click(object sender, ImageClickEventArgs e)
        {
            TranslateByOffset(0, -1000000);
        }

        private void TranslateByOffset(double xOffset, double yOffset)
        {
            InMemoryFeatureLayer mapShapeLayer = (InMemoryFeatureLayer)((LayerOverlay)Map1.CustomOverlays[1]).Layers["mapShapeLayer"];
            mapShapeLayer.Open();
            mapShapeLayer.EditTools.BeginTransaction();
            mapShapeLayer.EditTools.TranslateByOffset("MutlipointShape", xOffset, yOffset, GeographyUnit.Meter, DistanceUnit.Meter);
            mapShapeLayer.EditTools.CommitTransaction();
            mapShapeLayer.Close();
            ((LayerOverlay)Map1.CustomOverlays[1]).Redraw();
        }
    }
}
