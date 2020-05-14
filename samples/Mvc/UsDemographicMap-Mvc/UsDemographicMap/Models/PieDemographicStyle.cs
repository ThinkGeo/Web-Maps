using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using ThinkGeo.MapSuite.Core;
using ZedGraph;

namespace ThinkGeo.MapSuite.USDemographicMap
{
    public class PieDemographicStyle : DemographicStyle
    {
        private Dictionary<string, string> nameAliases;
        private Collection<GeoColor> pieColors;

        public PieDemographicStyle()
            : base()
        { }

        public PieDemographicStyle(IEnumerable<string> selectedColumns)
            : base(selectedColumns)
        {
            this.Opacity = 200;
            this.BaseColor = GeoColor.SimpleColors.LightBlue;
        }

        public Dictionary<string, string> NameAliases
        {
            get
            {
                if (nameAliases == null)
                {
                    nameAliases = new Dictionary<string, string>();
                }
                return nameAliases;
            }
        }

        protected override Style GetStyleCore(FeatureSource featureSource)
        {
            PieZedGraphStyle zedGraphStyle = new PieZedGraphStyle();
            zedGraphStyle.ZedGraphDrawing += new EventHandler<ZedGraphDrawingEventArgs>(zedGraphStyle_ZedGraphDrawing);
            pieColors = GeoColor.GetColorsInQualityFamily(GeoColor.FromArgb(Opacity, BaseColor), SelectedColumns.Count);
            for (int i = 0; i < SelectedColumns.Count; i++)
            {
                zedGraphStyle.RequiredColumnNames.Add(SelectedColumns[i]);
                zedGraphStyle.PieItems.Add(NameAliases[SelectedColumns[i]], pieColors[i]);
            }

            return zedGraphStyle;
        }

        void zedGraphStyle_ZedGraphDrawing(object sender, ZedGraphDrawingEventArgs e)
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
                Color color = Color.FromArgb(pieColors[i].AlphaComponent, pieColors[i].RedComponent, pieColors[i].GreenComponent, pieColors[i].BlueComponent);
                PieItem pieItem = zedGraph.GraphPane.AddPieSlice(Convert.ToDouble(e.Feature.ColumnValues[SelectedColumns[i]]), color, 0.08, "");
                pieItem.LabelDetail.IsVisible = false;
            }
            zedGraph.AxisChange();

            e.Bitmap = zedGraph.GraphPane.GetImage();
        }
    }
}