using System;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace HowDoI.Samples.DataProviders
{
    public partial class LoadWfsFeatureLayer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.CurrentExtent = new RectangleShape(455417.2, 4992437.4, 485795.9, 4960950.6);
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));

                WfsFeatureLayer wfsFeatureLayer = new WfsFeatureLayer(@"http://www.datafinder.org/wfsconnector/com.esri.wfs.Esrimap/MN_MetroGIS_DataFinder_WFS_Water_Resources", "Watersheds-watersheds_2_a");
                wfsFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
                wfsFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                Map1.StaticOverlay.Layers.Add("WfsFeatureLayer", wfsFeatureLayer);
            }
        }
    }
}
