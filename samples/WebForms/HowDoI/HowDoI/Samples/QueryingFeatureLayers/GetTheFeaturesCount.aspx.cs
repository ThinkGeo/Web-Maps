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

namespace HowDoI.Samples.Querying_Vector_Layers
{
    public partial class GetTheFeaturesCount : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-7601183.6904228, 5461367.090647, 8610804.1706833, -2943036.9966736);

                // Please input your ThinkGeo Cloud API Key to enable the background map.
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                Map1.CustomOverlays.Add(backgroundOverlay);

                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02_3857.shp"));
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.SimpleColors.Transparent, GeoColor.FromArgb(100, GeoColor.SimpleColors.Green));
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                LayerOverlay staticOverlay = new LayerOverlay("StaticOverlay");
                staticOverlay.IsBaseOverlay = false;
                staticOverlay.Layers.Add("WorldLayer", worldLayer);
                Map1.CustomOverlays.Add(staticOverlay);
            }
        }

        protected void btnGetCount_Click(object sender, EventArgs e)
        {
            LayerOverlay staticOverlay = (LayerOverlay)Map1.CustomOverlays["StaticOverlay"];
            FeatureLayer worldLayer = (FeatureLayer)staticOverlay.Layers["WorldLayer"];
            worldLayer.Open();
            FeatureCountLabel.Text = string.Format("There are <span style='color:red;font-size:15;font-weight:bolder;'>{0}</span> records in this vector layer.", worldLayer.QueryTools.GetCount());
            worldLayer.Close();
        }
    }
}
