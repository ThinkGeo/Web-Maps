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
    public partial class FindFeatureById : System.Web.UI.Page
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

                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02_3857.shp"));
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.SimpleColors.Transparent, GeoColor.FromArgb(100, GeoColor.SimpleColors.Green));
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                InMemoryFeatureLayer mapShapeLayer = new InMemoryFeatureLayer();
                mapShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.FromArgb(150, 60, 180, 60);
                mapShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.GeographicColors.DeepOcean;
                mapShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                LayerOverlay staticOverlay = new LayerOverlay();
                LayerOverlay dynamicOverlay = new LayerOverlay();

                staticOverlay.Layers.Add("WorldLayer", worldLayer);
                staticOverlay.IsBaseOverlay = false;
                dynamicOverlay.Layers.Add("InMemoryFeatureLayer", mapShapeLayer);
                dynamicOverlay.IsBaseOverlay = false;
                dynamicOverlay.TileType = TileType.SingleTile;

                Map1.CustomOverlays.Add(staticOverlay);
                Map1.CustomOverlays.Add(dynamicOverlay);   

                // The following two lines of code enable the client and server caching.
                // If you enable these features it will greatly increase the scalability of your
                // mapping application however there some side effects that may be counter intuitive.
                // Please read the white paper on web caching or the documentation regarding these methods.

                //Map1.StaticOverlay.ClientCache.CacheId = "WorldOverlay";
                //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path);
            }
        }

        protected void btnFind_Click(object sender, EventArgs e)
        {
            FeatureLayer shapeFileLayer = (FeatureLayer)((LayerOverlay)Map1.CustomOverlays[1]).Layers["WorldLayer"];
            InMemoryFeatureLayer mapShapeLayer = (InMemoryFeatureLayer)((LayerOverlay)Map1.CustomOverlays[2]).Layers["InMemoryFeatureLayer"];

            shapeFileLayer.Open();
            Feature feature = shapeFileLayer.FeatureSource.GetFeatureById(txtFeatureId.Text, new string[] { "CNTRY_NAME" });
            shapeFileLayer.Close();

            mapShapeLayer.InternalFeatures.Clear();
            mapShapeLayer.InternalFeatures.Add(feature.Id, feature);

            CloudPopup messageBox = new CloudPopup("selectedFeature", feature.GetBoundingBox().GetCenterPoint());
            messageBox.Height = 20;
            messageBox.ContentHtml = "<div style='color:#0065ce;font-size:10px;font-family:arial;'>The selected country is " + feature.ColumnValues["CNTRY_NAME"] + "</div>";
            if (!Map1.Popups.Contains("selectedFeature"))
            {
                Map1.Popups.Add(messageBox);
            }

            ((LayerOverlay)Map1.CustomOverlays[2]).Redraw();
        }
    }
}
