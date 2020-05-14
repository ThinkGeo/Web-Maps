/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Drawing;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;
using ZedGraph;

namespace HowDoI.Samples.Styles
{
    public partial class DrawUsingZedGraphStyle : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-13939426.6371, 6701997.4056, -7812401.86, 2626987.386962);

                // Please input your ThinkGeo Cloud API Key to enable the background map. 
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                Map1.CustomOverlays.Add(backgroundOverlay);

                //Create our Zedgraph Sytle and wire up the event.
                ZedGraphStyle zedGraphStyle = new ZedGraphStyle();
                zedGraphStyle.ZedGraphDrawing += new EventHandler<ZedGraphDrawingEventArgs>(zedGraphStyle_ZedGraphDrawing);

                zedGraphStyle.RequiredColumnNames.Add("WHITE");
                zedGraphStyle.RequiredColumnNames.Add("ASIAN");
                zedGraphStyle.RequiredColumnNames.Add("AREANAME");

                ShapeFileFeatureLayer citiesLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/USA/cities_a.shp"));
                citiesLayer.FeatureSource.Projection = new Proj4Projection(4326, 3857);
                citiesLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(zedGraphStyle);
                citiesLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(WorldStreetsTextStyles.GeneralPurpose("AREANAME",8));
                citiesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                LayerOverlay staticOverlay = new LayerOverlay();
                staticOverlay.Layers.Add("Cities", citiesLayer);
                staticOverlay.IsBaseOverlay = false;
                Map1.CustomOverlays.Add(staticOverlay);
            }
        }

        private void zedGraphStyle_ZedGraphDrawing(object sender, ZedGraphDrawingEventArgs e)
        {
            ChangeLabelPosition(((ShapeFileFeatureLayer)((LayerOverlay)Map1.CustomOverlays[1]).Layers["Cities"]), 100);

            ZedGraphControl zedGraph = new ZedGraphControl();
            zedGraph.Size = new Size(100, 100);

            zedGraph.GraphPane.Fill.Type = FillType.None;
            zedGraph.GraphPane.Chart.Fill.Type = FillType.None;

            zedGraph.GraphPane.Border.IsVisible = false;
            zedGraph.GraphPane.Chart.Border.IsVisible = false;
            zedGraph.GraphPane.XAxis.IsVisible = false;
            zedGraph.GraphPane.YAxis.IsVisible = false;
            zedGraph.GraphPane.Legend.IsVisible = false;
            zedGraph.GraphPane.Title.IsVisible = false;

            PieItem pieItem1 = zedGraph.GraphPane.AddPieSlice(Convert.ToDouble(e.Feature.ColumnValues["WHITE"]), GetColorFromGeoColor(GeoColor.StandardColors.LightBlue), 0, "White");
            pieItem1.LabelDetail.IsVisible = false;

            PieItem pieItem2 = zedGraph.GraphPane.AddPieSlice(Convert.ToDouble(e.Feature.ColumnValues["ASIAN"]), GetColorFromGeoColor(GeoColor.StandardColors.LightGreen), 0, "Asian");
            pieItem2.LabelDetail.IsVisible = false;
            pieItem2.Displacement = 0.2;

            zedGraph.AxisChange();

            e.GeoImage = new GeoImage(zedGraph.GraphPane.GetImage());
        }

        private void ChangeLabelPosition(ShapeFileFeatureLayer shapeFileLayer, int graphHeight)
        {
            ((TextStyle)shapeFileLayer.ZoomLevelSet.ZoomLevel01.CustomStyles[1]).XOffsetInPixel = -20;
            ((TextStyle)shapeFileLayer.ZoomLevelSet.ZoomLevel01.CustomStyles[1]).YOffsetInPixel = Convert.ToSingle(graphHeight * 0.4);
        }

        private Color GetColorFromGeoColor(GeoColor geoColor)
        {
            return Color.FromArgb(geoColor.AlphaComponent, geoColor.RedComponent, geoColor.GreenComponent, geoColor.BlueComponent);
        }
    }
}
