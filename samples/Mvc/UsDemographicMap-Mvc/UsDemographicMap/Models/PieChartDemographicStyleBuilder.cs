using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using ThinkGeo.MapSuite.Core;
using ZedGraph;

namespace ThinkGeo.MapSuite.USDemographicMap
{
    public class PieChartDemographicStyleBuilder : DemographicStyleBuilder
    {
        private Collection<string> selectedColumnAliases;
        private Collection<GeoColor> pieColors;

        public PieChartDemographicStyleBuilder()
            : this(new string[] { })
        { }

        public PieChartDemographicStyleBuilder(IEnumerable<string> selectedColumns)
            : base(selectedColumns)
        {
            this.Opacity = 200;
            this.Color = GeoColor.SimpleColors.LightBlue;
            this.selectedColumnAliases = new Collection<string>();
        }

        public Collection<string> SelectedColumnAliases
        {
            get { return selectedColumnAliases; }
        }

        protected override Collection<Style> GetStylesCore(FeatureSource featureSource)
        {
            PieZedGraphStyle zedGraphStyle = new PieZedGraphStyle();
            zedGraphStyle.ZedGraphDrawing += ZedGraphStyle_ZedGraphDrawing;
            pieColors = GeoColor.GetColorsInQualityFamily(GeoColor.FromArgb(Opacity, Color), SelectedColumns.Count);

            int i = 0;
            foreach (string column in SelectedColumns)
            {
                zedGraphStyle.RequiredColumnNames.Add(column);
                zedGraphStyle.PieItems.Add(SelectedColumnAliases[i], pieColors[i]);
                i++;
            }

             return new Collection<Style>(){zedGraphStyle};
        }

        private void ZedGraphStyle_ZedGraphDrawing(object sender, ZedGraphDrawingEventArgs e)
        {
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

            for (int i = 0; i < SelectedColumns.Count; i++)
            {
                double value = 0;
                if (!double.TryParse(e.Feature.ColumnValues[SelectedColumns[i]], out value))
                {
                    zedGraph.Dispose();
                    return;
                }
                Color color = System.Drawing.Color.FromArgb(pieColors[i].AlphaComponent, pieColors[i].RedComponent, pieColors[i].GreenComponent, pieColors[i].BlueComponent);
                PieItem pieItem = zedGraph.GraphPane.AddPieSlice(value, color, 0.08, "");
                pieItem.LabelDetail.IsVisible = false;
            }
            zedGraph.AxisChange();

            e.Bitmap = zedGraph.GraphPane.GetImage();
            zedGraph.Dispose();
        }
    }
}