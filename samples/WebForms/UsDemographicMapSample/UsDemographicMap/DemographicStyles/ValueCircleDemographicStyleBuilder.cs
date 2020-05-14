using System.Collections.Generic;
using System.Globalization;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace ThinkGeo.MapSuite.USDemographicMap
{
    public class ValueCircleDemographicStyleBuilder : DemographicStyleBuilder
    {
        private double radiusRatio;

        public ValueCircleDemographicStyleBuilder()
            : this(new string[] { })
        { }

        public ValueCircleDemographicStyleBuilder(IEnumerable<string> selectedColumns)
            : base(selectedColumns)
        {
            this.radiusRatio = 1;
            this.Color = GeoColor.SimpleColors.BrightOrange;
            this.Opacity = 160;
        }

        public double RadiusRatio
        {
            get { return radiusRatio; }
            set { radiusRatio = value; }
        }

        protected override Style GetStyleCore(FeatureSource featureSource)
        {
            double minValue = double.MaxValue;
            double maxValue = double.MinValue;

            featureSource.Open();
            for (int i = 0; i < featureSource.GetCount(); i++)
            {
                Feature feature = featureSource.GetFeatureById((i + 1).ToString(CultureInfo.InvariantCulture), SelectedColumns);
                double columnValue;
                if (double.TryParse(feature.ColumnValues[SelectedColumns[0]], out columnValue))
                {
                    if (columnValue < minValue)
                    {
                        minValue = columnValue;
                    }
                    else if (columnValue > maxValue)
                    {
                        maxValue = columnValue;
                    }
                }
            }
            featureSource.Close();

            ValueCircleStyle valueCircleStyle = new ValueCircleStyle();
            valueCircleStyle.ColumnName = SelectedColumns[0];
            valueCircleStyle.DrawingRadiusRatio = radiusRatio;
            valueCircleStyle.MinValidValue = minValue;
            valueCircleStyle.MaxValidValue = maxValue;
            valueCircleStyle.MinCircleAreaInDefaultZoomLevel = 80;
            valueCircleStyle.MaxCircleAreaInDefaultZoomLevel = 10000;
            valueCircleStyle.InnerColor = GeoColor.FromArgb(this.Opacity, Color);
            valueCircleStyle.OuterColor = GeoColor.SimpleColors.White;

            return valueCircleStyle;
        }
    }
}