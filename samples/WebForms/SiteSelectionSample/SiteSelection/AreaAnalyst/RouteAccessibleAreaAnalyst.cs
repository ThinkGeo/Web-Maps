using System;
using System.IO;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Routing;
using ThinkGeo.MapSuite.Shapes;

namespace ThinkGeo.MapSuite.SiteSelection
{
    public class RouteAccessibleAreaAnalyst : AccessibleAreaAnalyst
    {
        private int drivingTimeInMinutes;
        private string streetShapeFilePathName;

        public RouteAccessibleAreaAnalyst()
            : this(null, GeographyUnit.DecimalDegree)
        { }

        public RouteAccessibleAreaAnalyst(PointShape startLocation, GeographyUnit geographyUnit)
            : this(startLocation, geographyUnit, 6)
        {
        }

        public RouteAccessibleAreaAnalyst(PointShape startLocation, GeographyUnit geographyUnit, int drivingTimeInMinutes)
            : base(startLocation, geographyUnit)
        {
            this.drivingTimeInMinutes = drivingTimeInMinutes;
        }

        public string StreetShapeFilePathName
        {
            get { return streetShapeFilePathName; }
            set { streetShapeFilePathName = value; }
        }

        public int DrivingTimeInMinutes
        {
            get { return drivingTimeInMinutes; }
            set { drivingTimeInMinutes = value; }
        }

        protected override BaseShape CreateAccessibleAreaCore()
        {
            string rtgFilePathName = Path.ChangeExtension(StreetShapeFilePathName, ".rtg");

            RtgRoutingSource routingSource = new RtgRoutingSource(rtgFilePathName);
            FeatureSource featureSource = new ShapeFileFeatureSource(StreetShapeFilePathName);
            RoutingEngine routingEngine = new RoutingEngine(routingSource, featureSource);

            if (!featureSource.IsOpen)
            {
                featureSource.Open();
            }
            Proj4Projection proj = new Proj4Projection();
            proj.InternalProjectionParametersString = Proj4Projection.GetBingMapParametersString();
            proj.ExternalProjectionParametersString = Proj4Projection.GetEpsgParametersString(4326);

            proj.Open();
            StartLocation = proj.ConvertToExternalProjection(StartLocation) as PointShape;

            Feature feature = featureSource.GetFeaturesNearestTo(StartLocation, GeographyUnit, 1, ReturningColumnsType.NoColumns)[0];
            PolygonShape polygonShape = routingEngine.GenerateServiceArea(feature.Id, new TimeSpan(0, DrivingTimeInMinutes, 0), 100, GeographyUnit.Feet);

            polygonShape = proj.ConvertToInternalProjection(polygonShape) as PolygonShape;

            proj.Close();

            return polygonShape;
        }
    }
}