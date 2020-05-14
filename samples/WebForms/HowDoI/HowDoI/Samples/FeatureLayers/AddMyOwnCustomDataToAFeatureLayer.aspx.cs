/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI
{
    public partial class AddMyOwnCustomDataToAFeatureLayer : System.Web.UI.Page
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

                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath(@"~\SampleData\World\cntry02_3857.shp"));
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.SimpleColors.Transparent, GeoColor.FromArgb(100, GeoColor.SimpleColors.Green));
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Test", "Arial", 10, DrawingFontStyles.Regular, GeoColor.StandardColors.Red, 0, -12);
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                worldLayer.FeatureSource.CustomColumnFetch += new EventHandler<CustomColumnFetchEventArgs>(FeatureSource_CustomColumnFetch);
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.RequiredColumnNames.Add("Test");

                LayerOverlay staticOverlay = new LayerOverlay();
                staticOverlay.IsBaseOverlay = false;
                staticOverlay.Layers.Add(worldLayer);
                Map1.CustomOverlays.Add(staticOverlay);
            }
        }

        void FeatureSource_CustomColumnFetch(object sender, CustomColumnFetchEventArgs e)
        {
            if (e.Id == "135" || e.Id == "47")
            {
                string columnName = e.ColumnName;
                e.ColumnValue = "CountryId:" + e.Id;
            }
        }   
    }
}
