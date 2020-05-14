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
    public partial class ScaleFeatureUpAndDown : System.Web.UI.Page
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
                mapShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.FromArgb(122, 145, 255, 144);
                mapShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.GeographicColors.DeepOcean;
                mapShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                BaseShape feature = new RectangleShape(-2226389.81586547, 5621521.48619207, 2226389.81586547, 1689200.13960789);
                feature.Id = "Rectangle";
                mapShapeLayer.InternalFeatures.Add("Rectangle", new Feature(feature));

                LayerOverlay dynamicOverlay = new LayerOverlay();
                dynamicOverlay.Layers.Add("InMemoryFeatureLayer", mapShapeLayer);
                dynamicOverlay.IsBaseOverlay = false;
                dynamicOverlay.TileType = TileType.SingleTile;
                Map1.CustomOverlays.Add(dynamicOverlay);
            }
        }

        protected void btnScaleUp_Click(object sender, EventArgs e)
        {
            UpdateFeatureByScale(20, true);
        }

        protected void btnScaleDown_Click(object sender, EventArgs e)
        {
            UpdateFeatureByScale(20, false);
        }

        private void UpdateFeatureByScale(double percentage, bool isScaleUp)
        {
            InMemoryFeatureLayer mapShapeLayer = (InMemoryFeatureLayer)((LayerOverlay)Map1.CustomOverlays[1]).Layers["InMemoryFeatureLayer"];
            mapShapeLayer.Open();
            mapShapeLayer.EditTools.BeginTransaction();
            if (isScaleUp)
            {
                mapShapeLayer.EditTools.ScaleUp("Rectangle", percentage);
            }
            else
            {
                mapShapeLayer.EditTools.ScaleDown("Rectangle", percentage);
            }
            mapShapeLayer.EditTools.CommitTransaction();
            mapShapeLayer.Close();
            ((LayerOverlay)Map1.CustomOverlays[1]).Redraw();
        }
    }
}
