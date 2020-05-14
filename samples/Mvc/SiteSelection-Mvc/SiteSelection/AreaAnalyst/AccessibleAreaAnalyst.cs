using ThinkGeo.MapSuite.Core;

namespace ThinkGeo.MapSuite.SiteSelection
{
    public abstract class AccessibleAreaAnalyst
    {
        private PointShape startLocation;
        private GeographyUnit geographyUnit;

        protected AccessibleAreaAnalyst()
        { }

        protected AccessibleAreaAnalyst(PointShape startLocation)
            : this(startLocation, GeographyUnit.DecimalDegree)
        { }

        protected AccessibleAreaAnalyst(PointShape startLocation, GeographyUnit geographyUnit)
        {
            this.startLocation = startLocation;
            this.geographyUnit = geographyUnit;
        }

        public PointShape StartLocation
        {
            get { return startLocation; }
            set { startLocation = value; }
        }

        public GeographyUnit GeographyUnit
        {
            get { return geographyUnit; }
            set { geographyUnit = value; }
        }

        public BaseShape CreateAccessibleArea()
        {
            if (StartLocation == null)
            {
                return null;
            }
            else
            {
                return CreateAccessibleAreaCore();
            }
        }

        protected abstract BaseShape CreateAccessibleAreaCore();
    }
}