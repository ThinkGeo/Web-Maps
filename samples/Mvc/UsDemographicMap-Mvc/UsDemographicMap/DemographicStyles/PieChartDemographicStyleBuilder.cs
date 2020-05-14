using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Styles;
using ZedGraph;

namespace ThinkGeo.MapSuite.USDemographicMap
{
    public class PieChartDemographicStyleBuilder : DemographicStyleBuilder
    {
        private Collection<string> selectedColumnAliases;
        private Collection<GeoColor> pieColors;

        public PieChartDemographicStyleBuilder()
            : base()
        { }

        public PieChartDemographicStyleBuilder(IEnumerable<string> selectedColumns)
            : base(selectedColumns)
        {
            this.Opacity = 200;
            this.Color = GeoColor.SimpleColors.LightBlue;
        }

        public Collection<string> SelectedColumnAliases
        {
            get
            {
                if (selectedColumnAliases == null)
                {
                    selectedColumnAliases = new Collection<string>();
                }
                return selectedColumnAliases;
            }
        }

        protected override Style GetStyleCore(FeatureSource featureSource)
        {
            PieZedGraphStyle zedGraphStyle = new PieZedGraphStyle();
            zedGraphStyle.ZedGraphDrawing += ZedGraphStyle_ZedGraphDrawing;
            pieColors = GeoColor.GetColorsInQualityFamily(GeoColor.FromArgb(Opacity, Color), SelectedColumns.Count);
            for (int i = 0; i < SelectedColumns.Count; i++)
            {
                zedGraphStyle.RequiredColumnNames.Add(SelectedColumns[i]);
                zedGraphStyle.PieSlices.Add(SelectedColumnAliases[i], pieColors[i]);
            }

            return zedGraphStyle;
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
                Color color = System.Drawing.Color.FromArgb(pieColors[i].AlphaComponent, pieColors[i].RedComponent, pieColors[i].GreenComponent, pieColors[i].BlueComponent);
                PieItem pieItem = zedGraph.GraphPane.AddPieSlice(Convert.ToDouble(e.Feature.ColumnValues[SelectedColumns[i]]), color, 0.08, "");
                pieItem.LabelDetail.IsVisible = false;
            }
            zedGraph.AxisChange();

            e.GeoImage =new GeoImage( zedGraph.GraphPane.GetImage());
        }
    }
}