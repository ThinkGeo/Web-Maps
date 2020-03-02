using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace Visualization
{
    public class WindPointStyle : Style
    {
        private string textColumn;
        private string angleColumn;
        private string windLevelColumn;
        private GeoFont font;
        private GeoBrush textBrush;
        private GeoSolidBrush fillBrush;
        private GeoSolidBrush blackBrush;
        private float directionLineLength1;
        private float directionLineLength2;
        private GeoPen outlinePen;
        private GeoPen innerlinePen;

        public WindPointStyle()
            : this(string.Empty, string.Empty, string.Empty, GeoColor.StandardColors.Orange)
        { }

        public WindPointStyle(string textColumn, string levelColumn, string angleColumn, GeoColor fillColor)
        {
            this.directionLineLength1 = 40;
            this.directionLineLength2 = 10;
            this.blackBrush = new GeoSolidBrush(GeoColor.SimpleColors.Black);
            this.font = new GeoFont("Verdana", 10);
            this.textBrush = new GeoSolidBrush(GeoColor.StandardColors.Black);
            this.fillBrush = new GeoSolidBrush(fillColor);
            this.outlinePen = new GeoPen(GeoColor.StandardColors.Black, 4);
            this.innerlinePen = new GeoPen(fillBrush, 2);
            this.textColumn = textColumn;
            this.windLevelColumn = levelColumn;
            this.angleColumn = angleColumn;
        }

        public string TextColumn
        {
            get { return textColumn; }
            set { textColumn = value; }
        }

        public string AngleColumn
        {
            get { return angleColumn; }
            set { angleColumn = value; }
        }

        public string WindLevelColumn
        {
            get { return windLevelColumn; }
            set { windLevelColumn = value; }
        }

        protected override Collection<string> GetRequiredColumnNamesCore()
        {
            return new Collection<string> { TextColumn, AngleColumn, WindLevelColumn };
        }

        protected override void DrawCore(IEnumerable<Feature> features, GeoCanvas canvas, Collection<SimpleCandidate> labelsInThisLayer, Collection<SimpleCandidate> labelsInAllLayers)
        {
            double resolution = Math.Max(canvas.CurrentWorldExtent.Width / canvas.Width, canvas.CurrentWorldExtent.Height / canvas.Height);
            foreach (var feature in features)
            {
                PointShape pointShape = feature.GetShape() as PointShape;
                if (pointShape != null)
                {
                    float screenOffsetX = (float)((pointShape.X - canvas.CurrentWorldExtent.UpperLeftPoint.X) / resolution);
                    float screenOffsetY = (float)((canvas.CurrentWorldExtent.UpperLeftPoint.Y - pointShape.Y) / resolution);
                    string angle = feature.ColumnValues[AngleColumn];
                    string level = feature.ColumnValues[WindLevelColumn];
                    int windLevel = int.Parse(level);

                    ScreenPointF[] directionLine = null;
                    ScreenPointF[] levelLine1 = null;
                    ScreenPointF[] levelLine2 = null;

                    if (!string.IsNullOrEmpty(angle))
                    {
                        double radian1 = double.Parse(angle) * Math.PI / 180;
                        float x1 = (float)(directionLineLength1 * Math.Cos(radian1));
                        float y1 = (float)(directionLineLength1 * Math.Sin(radian1));

                        double radian2 = (double.Parse(angle) - 90) * Math.PI / 180;
                        float x2 = (float)(directionLineLength2 * Math.Cos(radian2));
                        float y2 = (float)(directionLineLength2 * Math.Sin(radian2));

                        float x3 = (float)((directionLineLength1 - 8) * Math.Cos(radian1));
                        float y3 = (float)((directionLineLength1 - 8) * Math.Sin(radian1));

                        float x4 = (float)(directionLineLength2 * Math.Cos(radian2));
                        float y4 = (float)(directionLineLength2 * Math.Sin(radian2));

                        if (windLevel >= 1)
                        {
                            directionLine = new ScreenPointF[2];
                            directionLine[0] = new ScreenPointF(screenOffsetX, screenOffsetY);
                            directionLine[1] = new ScreenPointF(screenOffsetX + x1, screenOffsetY + y1);
                        }

                        if (windLevel >= 2)
                        {
                            levelLine1 = new ScreenPointF[2];
                            levelLine1[0] = new ScreenPointF(screenOffsetX + x1, screenOffsetY + y1);
                            levelLine1[1] = new ScreenPointF(screenOffsetX + x1 + x2, screenOffsetY + y1 + y2);
                        }

                        if (windLevel >= 3)
                        {
                            levelLine2 = new ScreenPointF[2];
                            levelLine2[0] = new ScreenPointF(screenOffsetX + x3, screenOffsetY + y3);
                            levelLine2[1] = new ScreenPointF(screenOffsetX + x3 + x4, screenOffsetY + y3 + y4);
                        }
                    }

                    // draw back
                    canvas.DrawEllipse(feature, 26, 26, blackBrush, ThinkGeo.MapSuite.Drawing.DrawingLevel.LevelOne);
                    if (directionLine != null)
                    {
                        canvas.DrawLine(directionLine, outlinePen, DrawingLevel.LevelOne, 0, 0);
                    }

                    if (levelLine1 != null)
                    {
                        canvas.DrawLine(levelLine1, outlinePen, DrawingLevel.LevelOne, 0, 0);
                    }

                    if (levelLine2 != null)
                    {
                        canvas.DrawLine(levelLine2, outlinePen, DrawingLevel.LevelOne, 0, 0);
                    }

                    //draw fore
                    canvas.DrawEllipse(feature, 24, 24, fillBrush, ThinkGeo.MapSuite.Drawing.DrawingLevel.LevelTwo);
                    if (directionLine != null)
                    {
                        canvas.DrawLine(directionLine, innerlinePen, DrawingLevel.LevelTwo, 0, 0);
                    }

                    if (levelLine1 != null)
                    {
                        canvas.DrawLine(levelLine1, innerlinePen, DrawingLevel.LevelTwo, 0, 0);
                    }

                    if (levelLine2 != null)
                    {
                        canvas.DrawLine(levelLine2, innerlinePen, DrawingLevel.LevelTwo, 0, 0);
                    }

                    string text = feature.ColumnValues[TextColumn];
                    if (!string.IsNullOrEmpty(text))
                    {
                        canvas.DrawTextWithScreenCoordinate(text, font, textBrush, screenOffsetX, screenOffsetY, DrawingLevel.LabelLevel);
                    }
                }
            }
        }
    }
}