using ThinkGeo.MapSuite.Core;

namespace ThinkGeo.MapSuite.SiteSelection
{
    public class BufferAccessibleAreaAnalyst : AccessibleAreaAnalyst
    {
        private double distance;
        private DistanceUnit distanceUnit;

        public BufferAccessibleAreaAnalyst()
            : this(2, DistanceUnit.Mile)
        { }

        public BufferAccessibleAreaAnalyst(int distance)
            : this(distance, DistanceUnit.Mile)
        { }

        public BufferAccessibleAreaAnalyst(PointShape startLocation, GeographyUnit geographyUnit)
            : this(startLocation, geographyUnit, 2, DistanceUnit.Mile)
        {
        }

        public BufferAccessibleAreaAnalyst(int distance, DistanceUnit distanceUnit)
            : this(null, GeographyUnit.Meter, distance, distanceUnit)
        {
        }

        public BufferAccessibleAreaAnalyst(PointShape startLocation, GeographyUnit geographyUnit, int distance, DistanceUnit distanceUnit)
            : base(startLocation, geographyUnit)
        {
            this.Distance = distance;
            this.DistanceUnit = distanceUnit;
        }

        public double Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        public DistanceUnit DistanceUnit
        {
            get { return distanceUnit; }
            set { distanceUnit = value; }
        }

        protected override BaseShape CreateAccessibleAreaCore()
        {
            if (StartLocation == null)
            {
                return null;
            }

            return StartLocation.Buffer(Distance, 40, GeographyUnit, DistanceUnit);
        }
    }
}