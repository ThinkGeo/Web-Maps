/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Collections.ObjectModel;
using System.Text;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples
{
    public partial class FindTheFeatureTheUserClicked : System.Web.UI.Page
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

                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02_3857.shp"));
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.SimpleColors.Transparent, GeoColor.FromArgb(100, GeoColor.SimpleColors.Green));
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                LayerOverlay staticOverlay = new LayerOverlay("WorldStaticOverlay");
                staticOverlay.IsBaseOverlay = false;
                staticOverlay.Layers.Add("WorldLayer", worldLayer);

                InMemoryFeatureLayer highlightLayer = new InMemoryFeatureLayer();
                highlightLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(100, 60, 180, 60), GeoColor.GeographicColors.DeepOcean);
                highlightLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                LayerOverlay dynamicOverlay = new LayerOverlay("HightLightDynamicOverlay");
                dynamicOverlay.Layers.Add("HighLightLayer", highlightLayer);
                dynamicOverlay.IsBaseOverlay = false;

                Map1.CustomOverlays.Add(staticOverlay);
                Map1.CustomOverlays.Add(dynamicOverlay);
            }
        }

        protected void Map1_Click(object sender, MapClickedEventArgs e)
        {
            LayerOverlay staticOverlay = (LayerOverlay)Map1.CustomOverlays["WorldStaticOverlay"];
            LayerOverlay dynamicOverlay = (LayerOverlay)Map1.CustomOverlays["HightLightDynamicOverlay"];

            ShapeFileFeatureLayer shapeFileLayer = (ShapeFileFeatureLayer)(staticOverlay.Layers["WorldLayer"]);
            InMemoryFeatureLayer highLightLayer = (InMemoryFeatureLayer)(dynamicOverlay.Layers["HighLightLayer"]);
            highLightLayer.InternalFeatures.Clear();

            shapeFileLayer.Open();
            Collection<Feature> selectedFeatures = shapeFileLayer.QueryTools.GetFeaturesContaining(e.Position, new string[] { "CNTRY_NAME", "POP_CNTRY", "COLOR_MAP" });
            shapeFileLayer.Close();

            foreach (Feature feature in selectedFeatures)
            {
                highLightLayer.InternalFeatures.Add(feature.Id, feature);
            }
            dynamicOverlay.Redraw();

            CloudPopup popup;
            if (Map1.Popups.Count == 0)
            {
                popup = new CloudPopup("Popup", e.Position, string.Empty, 260, 60);
                popup.IsVisible = true;
                Map1.Popups.Add(popup);
            }
            else
            {
                popup = (CloudPopup)Map1.Popups["Popup"];
                popup.Position = e.Position;
            }

            popup.ContentHtml = GetPopupContent(selectedFeatures);
        }

        private static string GetPopupContent(Collection<Feature> features)
        {
            string content;
            if (features.Count > 0)
            {
                StringBuilder message = new StringBuilder();
                message.AppendFormat("<li>Country Name : {0}</li>", features[0].ColumnValues["CNTRY_NAME"].Trim());
                message.AppendFormat("<li>Country Population : {0}</li>", features[0].ColumnValues["POP_CNTRY"].Trim());
                message.AppendFormat("<li>Map Color : {0}</li>", features[0].ColumnValues["COLOR_MAP"].Trim());
                string messageInPopup = String.Format("<div class='normalBlueTx'>{0}</div>", message.ToString());

                content = messageInPopup;
            }
            else
            {
                content = @"<div class='normalBlueTx'>Please click on a country to show its information.</div>";
            }
            return content;
        }
    }
}
