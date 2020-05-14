/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Globalization;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples
{
    public partial class PlotAPointUsingLatAndLong : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-13064290.945669, 6003560.5647606, -9011293.9803927, 3902459.5429304);
                Map1.MapTools.MouseCoordinate.Enabled = true;

                // Please input your ThinkGeo Cloud API Key to enable the background map.
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                Map1.CustomOverlays.Add(backgroundOverlay);

                InMemoryFeatureLayer shapeLayer = new InMemoryFeatureLayer();
                shapeLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.PointType = PointType.Bitmap;
                shapeLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.Image = new GeoImage(MapPath("~/SampleData/USA/United States.png"));
                shapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                LayerOverlay dynamicOverlay = new LayerOverlay("DynamicOverlay");
                dynamicOverlay.Layers.Add("ShapeLayer", shapeLayer);
                dynamicOverlay.IsBaseOverlay = false;

                Map1.CustomOverlays.Add(dynamicOverlay);
            }
        }

        protected void btnAddPoint_Click(object sender, EventArgs e)
        {
            LayerOverlay dynamicOverlay = (LayerOverlay)Map1.CustomOverlays["DynamicOverlay"];
            InMemoryFeatureLayer shapeLayer = (InMemoryFeatureLayer)dynamicOverlay.Layers["ShapeLayer"];

            Feature pointFeature = new Feature(new PointShape(double.Parse(LongitudeTextBox.Text, CultureInfo.InvariantCulture), double.Parse(LatitudeTextBox.Text, CultureInfo.InvariantCulture)));
            Proj4Projection proj4 = new Proj4Projection(4326, 3857);
            proj4.Open();
            pointFeature = proj4.ConvertToExternalProjection(pointFeature);
            shapeLayer.InternalFeatures.Add(pointFeature.Id, pointFeature);
            dynamicOverlay.Redraw();
        }
    }
}
