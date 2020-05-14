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
    public partial class ConvertToFromWKB : System.Web.UI.Page
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

                InMemoryFeatureLayer mapShapeLayer = new InMemoryFeatureLayer();
                mapShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.FromArgb(122, 145, 255, 144);
                mapShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.GeographicColors.DeepOcean;
                mapShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                LayerOverlay dynamicOverlay = new LayerOverlay();
                dynamicOverlay.IsBaseOverlay = false;
                dynamicOverlay.TileType = TileType.SingleTile;
                dynamicOverlay.Layers.Add("InMemoryFeatureLayer", mapShapeLayer);
                Map1.CustomOverlays.Add(dynamicOverlay);
            }
        }

        protected void btnConvert_Click(object sender, EventArgs e)
        {
            InMemoryFeatureLayer mapShapeLayer = (InMemoryFeatureLayer)(((LayerOverlay)Map1.CustomOverlays[1]).Layers["InMemoryFeatureLayer"]);
            Proj4Projection proj4 = new Proj4Projection(4326, 3857);
            proj4.Open();

            if (btnConvert.Text == "WKB  to  Feature")
            {
                byte[] wellKnownBinary = Convert.FromBase64String(txtWKB.Text);
                Feature feature = new Feature(wellKnownBinary);
                feature = proj4.ConvertToExternalProjection(feature);
                mapShapeLayer.InternalFeatures.Add("feature", feature);

                txtWKB.Text = string.Empty;
                btnConvert.Text = "Feature  to  WKB";
            }
            else
            {
                Feature feature = mapShapeLayer.InternalFeatures["feature"];
                feature = proj4.ConvertToInternalProjection(feature);
                byte[] wellKnownBinary = feature.GetWellKnownBinary();

                mapShapeLayer.InternalFeatures.Clear();

                txtWKB.Text = Convert.ToBase64String(wellKnownBinary);
                btnConvert.Text = "WKB  to  Feature";
            }

            ((LayerOverlay)Map1.CustomOverlays[1]).Redraw();
        }
    }
}
