using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace ThinkGeo.MapSuite.USDemographicMap
{
    public class ValueCircleStyle : Style
    {
        private string columnName;
        private ZoomLevel defaultZoomLevel;
        private double drawingRadiusRatio;
        private GeoColor innerColor;
        private double maxCircleRadiusInDefaultZoomLevel;
        private double maxValidValue;
        private double minCircleRadiusInDefaultZoomLevel;
        private double minValidValue;
        private GeoColor outerColor;

        private double basedScale;

        public ValueCircleStyle()
            : base()
        {
            defaultZoomLevel = (new ZoomLevelSet()).ZoomLevel04;
            drawingRadiusRatio = 1;
            outerColor = GeoColor.FromArgb(255, 10, 20, 255);
            innerColor = GeoColor.FromArgb(100, 10, 20, 255);
            minCircleRadiusInDefaultZoomLevel = 10;
            maxCircleRadiusInDefaultZoomLevel = 100;
            basedScale = (new ZoomLevelSet()).ZoomLevel05.Scale;
        }

        public string ColumnName
        {
            get { return columnName; }
            set { columnName = value; }
        }

        public ZoomLevel DefaultZoomLevel
        {
            get { return defaultZoomLevel; }
            set { defaultZoomLevel = value; }
        }

        public double DrawingRadiusRatio
        {
            get { return drawingRadiusRatio; }
            set { drawingRadiusRatio = value; }
        }

        public GeoColor InnerColor
        {
            get { return innerColor; }
            set { innerColor = value; }
        }

        public double MaxCircleAreaInDefaultZoomLevel
        {
            get { return maxCircleRadiusInDefaultZoomLevel; }
            set { maxCircleRadiusInDefaultZoomLevel = value; }
        }

        public double MaxValidValue
        {
            get { return maxValidValue; }
            set { maxValidValue = value; }
        }

        public double MinCircleAreaInDefaultZoomLevel
        {
            get { return minCircleRadiusInDefaultZoomLevel; }
            set { minCircleRadiusInDefaultZoomLevel = value; }
        }

        public double MinValidValue
        {
            get { return minValidValue; }
            set { minValidValue = value; }
        }

        public GeoColor OuterColor
        {
            get { return outerColor; }
            set { outerColor = value; }
        }

        public double BasedScale
        {
            get { return basedScale; }
        }

        protected override void DrawCore(IEnumerable<Feature> features, GeoCanvas canvas, Collection<SimpleCandidate> labelsInThisLayer, Collection<SimpleCandidate> labelsInAllLayers)
        {
            double dCircleArea = maxCircleRadiusInDefaultZoomLevel - MinCircleAreaInDefaultZoomLevel;
            double dValue = maxValidValue - minValidValue;
            double ratio = dValue / dCircleArea;

            foreach (Feature f in features)
            {
                PointShape center = f.GetShape().GetCenterPoint();
                double value = 0;
                if (!double.TryParse(f.ColumnValues[columnName], out value))
                {
                    continue;
                }

                if (value > maxValidValue || value < minValidValue)
                {
                    continue;
                }

                double drawingDefaultCircleArea = (value - minValidValue) / ratio + minCircleRadiusInDefaultZoomLevel;

                double defaultScale = defaultZoomLevel.Scale;
                double scale = canvas.CurrentScale;
                double graphArea = 0, graphHeght = 0;
                graphArea = drawingDefaultCircleArea * defaultScale / basedScale * drawingRadiusRatio;

                graphHeght = Math.Sqrt(graphArea / Math.PI);

                canvas.DrawEllipse(center, (float)(graphHeght * 2), (float)(graphHeght * 2), new GeoPen(outerColor, 1), new GeoSolidBrush(innerColor), DrawingLevel.LevelOne);
            }
        }

        protected override Collection<string> GetRequiredColumnNamesCore()
        {
            Collection<string> requiredFieldNames = new Collection<string>();
            requiredFieldNames.Add(columnName);

            return requiredFieldNames;
        }
    }
}