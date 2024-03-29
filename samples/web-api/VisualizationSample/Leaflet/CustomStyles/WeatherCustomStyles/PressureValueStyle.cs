using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ThinkGeo.Core;

namespace Visualization
{
    [Serializable]
    public class PressureValueStyle : ValueStyle
    {
        private ValueItem lowPressureValueItem;
        private ValueItem highPressureValueItem;

        public PressureValueStyle()
        {
            SquareTextPointStyle highPressurePointStyle = new SquareTextPointStyle();
            highPressurePointStyle.Text = "L";
            highPressurePointStyle.FillBrush = new GeoSolidBrush(GeoColor.FromArgb(255, 39, 39, 245));
            highPressureValueItem = new ValueItem("H", highPressurePointStyle);
            ValueItems.Add(highPressureValueItem);

            SquareTextPointStyle lowPressurePointStyle = new SquareTextPointStyle();
            lowPressurePointStyle.Text = "H";
            lowPressurePointStyle.FillBrush = new GeoSolidBrush(GeoColors.Red);
            lowPressureValueItem = new ValueItem("L", lowPressurePointStyle);
            ValueItems.Add(lowPressureValueItem);
        }

        protected override void DrawCore(IEnumerable<Feature> features, GeoCanvas canvas, Collection<SimpleCandidate> labelsInThisLayer, Collection<SimpleCandidate> labelsInAllLayers)
        {
            base.DrawCore(features, canvas, labelsInThisLayer, labelsInAllLayers);
        }

        [Serializable]
        public class SquareTextPointStyle : PointStyle
        {
            private string text;
            private GeoFont font;
            private GeoBrush textBrush;

            public SquareTextPointStyle()
            {
                SymbolType = PointSymbolType.Square;
                SymbolSize = 30;
                PointType = PointType.Symbol;
                font = new GeoFont("Verdana", 14);
                textBrush = new GeoSolidBrush(GeoColors.White);
                OutlinePen = new GeoPen(GeoColors.White, 1);
            }

            public string Text
            {
                get { return text; }
                set { text = value; }
            }

            protected override void DrawCore(IEnumerable<Feature> features, GeoCanvas canvas, Collection<SimpleCandidate> labelsInThisLayer, Collection<SimpleCandidate> labelsInAllLayers)
            {
                base.DrawCore(features, canvas, labelsInThisLayer, labelsInAllLayers);

                double resolution = Math.Max(canvas.CurrentWorldExtent.Width / canvas.Width, canvas.CurrentWorldExtent.Height / canvas.Height);
                foreach (Feature feature in features)
                {
                    PointShape pointShape = feature.GetShape() as PointShape;
                    if (pointShape != null)
                    {
                        float screenOffsetX = (float)((pointShape.X - canvas.CurrentWorldExtent.UpperLeftPoint.X) / resolution);
                        float screenOffsetY = (float)((canvas.CurrentWorldExtent.UpperLeftPoint.Y - pointShape.Y) / resolution);

                        canvas.DrawTextWithScreenCoordinate(Text, font, textBrush, screenOffsetX, screenOffsetY, DrawingLevel.LabelLevel);
                    }
                }
            }
        }
    }
}