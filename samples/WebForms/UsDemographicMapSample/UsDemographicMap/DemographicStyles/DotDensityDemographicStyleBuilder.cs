using System.Collections.Generic;
using System.Globalization;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace ThinkGeo.MapSuite.USDemographicMap
{
    public class DotDensityDemographicStyleBuilder : DemographicStyleBuilder
    {
        private double dotDensityValue;

        public DotDensityDemographicStyleBuilder()
            : this(new string[] { })
        { }

        public DotDensityDemographicStyleBuilder(IEnumerable<string> selectedColumns)
            : base(selectedColumns)
        {
            this.Opacity = 255;
            this.DotDensityValue = 50;
            this.Color = GeoColor.SimpleColors.DarkRed;
        }

        public double DotDensityValue
        {
            get { return dotDensityValue; }
            set { dotDensityValue = value; }
        }

        protected override Style GetStyleCore(FeatureSource featureSource)
        {
            double totalValue = 0;
            featureSource.Open();
            int featureCount = featureSource.GetCount();
            for (int i = 0; i < featureCount; i++)
            {
                Feature feature = featureSource.GetFeatureById((i + 1).ToString(CultureInfo.InvariantCulture), SelectedColumns);
                double columnValue;
                double.TryParse(feature.ColumnValues[SelectedColumns[0]], out columnValue);
                totalValue += columnValue;
            }
            featureSource.Close();
            double pointToValueRatio = DotDensityValue / (totalValue / featureCount);

            CustomDotDensityStyle dotDensityStyle = new CustomDotDensityStyle();
            dotDensityStyle.ColumnName = SelectedColumns[0];
            dotDensityStyle.PointToValueRatio = pointToValueRatio;
            dotDensityStyle.CustomPointStyle = new PointStyle(PointSymbolType.Circle, new GeoSolidBrush(GeoColor.FromArgb(Opacity, Color)), 4);

            return dotDensityStyle;
        }
    }
}