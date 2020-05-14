using ThinkGeo.MapSuite.Core;

namespace ThinkGeo.MapSuite.UsDemographicMap
{
    public class PieSlice
    {
        private double value;
        private GeoColor color;
        private double displacement;
        private string label;

        public PieSlice()
        { }

        public PieSlice(double value, GeoColor color, double displacement, string label)
        {
            this.Value = value;
            this.Color = color;
            this.Displacement = displacement;
            this.Label = label;
        }

        public double Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public GeoColor Color
        {
            get { return this.color; }
            set { this.color = value; }
        }

        public double Displacement
        {
            get { return this.displacement; }
            set { this.displacement = value; }
        }

        public string Label
        {
            get { return this.label; }
            set { this.label = value; }
        }
    }
}