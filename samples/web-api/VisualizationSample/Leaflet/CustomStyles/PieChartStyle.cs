using System;
using System.Drawing;
using System.Globalization;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Styles;
using ZedGraph;

namespace Visualization
{
    public class PieChartStyle : ZedGraphStyle
    {
        public PieChartStyle()
        {
            this.ZedGraphDrawing += new EventHandler<ZedGraphDrawingEventArgs>(zedGraphStyle_ZedGraphDrawing);

            this.RequiredColumnNames.Add("WHITE");
            this.RequiredColumnNames.Add("ASIAN");
            this.RequiredColumnNames.Add("Black");
            this.RequiredColumnNames.Add("Other");
            this.RequiredColumnNames.Add("AREANAME");
        }

        private void zedGraphStyle_ZedGraphDrawing(object sender, ZedGraphDrawingEventArgs e)
        {
            ZedGraphControl zedGraph = new ZedGraphControl()
            {
                Size = new Size(100, 100)
            };
            zedGraph.GraphPane.Fill.Type = FillType.None;
            zedGraph.GraphPane.Chart.Fill.Type = FillType.None;
            zedGraph.GraphPane.Border.IsVisible = false;
            zedGraph.GraphPane.Chart.Border.IsVisible = false;
            zedGraph.GraphPane.XAxis.IsVisible = false;
            zedGraph.GraphPane.YAxis.IsVisible = false;
            zedGraph.GraphPane.Legend.IsVisible = false;
            zedGraph.GraphPane.Title.IsVisible = false;

            PieItem pieItem1 = zedGraph.GraphPane.AddPieSlice(Convert.ToDouble(e.Feature.ColumnValues["WHITE"], CultureInfo.InvariantCulture), GetColorFromGeoColor(GeoColor.FromHtml("#93fc8f")), 0.08f, "White");
            pieItem1.LabelDetail.IsVisible = false;

            PieItem pieItem2 = zedGraph.GraphPane.AddPieSlice(Convert.ToDouble(e.Feature.ColumnValues["ASIAN"], CultureInfo.InvariantCulture), GetColorFromGeoColor(GeoColor.FromHtml("#8ffbe8")), 0.08f, "Asian");
            pieItem2.LabelDetail.IsVisible = false;

            PieItem pieItem3 = zedGraph.GraphPane.AddPieSlice(Convert.ToDouble(e.Feature.ColumnValues["Black"], CultureInfo.InvariantCulture), GetColorFromGeoColor(GeoColor.FromHtml("#cc8efa")), 0.08f, "Black");
            pieItem3.LabelDetail.IsVisible = false;

            PieItem pieItem4 = zedGraph.GraphPane.AddPieSlice(Convert.ToDouble(e.Feature.ColumnValues["Other"], CultureInfo.InvariantCulture), GetColorFromGeoColor(GeoColor.FromHtml("#fcab8d")), 0.08f, "Other");
            pieItem4.LabelDetail.IsVisible = false;

            zedGraph.AxisChange();
            e.GeoImage = new GeoImage(zedGraph.GraphPane.GetImage());
        }

        private static Color GetColorFromGeoColor(GeoColor geoColor)
        {
            return Color.FromArgb(geoColor.AlphaComponent, geoColor.RedComponent, geoColor.GreenComponent, geoColor.BlueComponent);
        }
    }
}