/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Collections.ObjectModel;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms; 

namespace HowDoI.Samples
{
    public partial class PrintAMap : System.Web.UI.Page
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
                shapeLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimplePointStyle(PointSymbolType.Star, GeoColor.StandardColors.Red, 5);
                shapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
                shapeLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 255, 255, 128), 8F, GeoColor.StandardColors.LightGray, 10F, true);
                shapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                LayerOverlay staticOverlay = new LayerOverlay();
                staticOverlay.IsBaseOverlay = false;
                staticOverlay.Layers.Add(shapeLayer);

                Map1.CustomOverlays.Add(staticOverlay);
                    
                Collection<Vertex> points = new Collection<Vertex>();
                points.Add(new Vertex(-10517734.8833162, 4687318.87110816));
                points.Add(new Vertex(-12298577.3428409, 4758264.23546119));
                LineShape line = new LineShape(points);
                shapeLayer.InternalFeatures.Add("Line", new Feature(line));

                Map1.MapTools.MouseCoordinate.Enabled = true;
                Map1.MapTools.PanZoomBar.Enabled = true;
                Map1.MapTools.ScaleLine.Enabled = true;
                Map1.MapTools.KeyboardMapTool.Enabled = true;
            }
        }
    }
}
