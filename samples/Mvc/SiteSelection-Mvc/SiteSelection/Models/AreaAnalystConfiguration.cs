using System;
using System.Globalization;
using ThinkGeo.MapSuite.Core;

namespace ThinkGeo.MapSuite.SiteSelection
{
    public class AreaAnalystConfiguration
    {
        private PointShape searchPoint;
        private string category;
        private string subCategory;
        private ServiceSearchMode serviceSearchMode;
        private int drivingTime;
        private double bufferDistance;
        private DistanceUnit distanceUnit;

        private string routeFilePathName;
        private GeographyUnit mapUnit;

        public AreaAnalystConfiguration(string routeFilePathName)
            : this(routeFilePathName, new GeoCollection<object>())
        {
        }

        public AreaAnalystConfiguration(string routeFilePathName, GeoCollection<object> parameters)
        {
            this.MapUnit = GeographyUnit.Meter;
            this.routeFilePathName = routeFilePathName;
            this.DistanceUnit = DistanceUnit.Mile;

            if (parameters.Contains("category"))
            {
                this.category = parameters["category"].ToString();
            }
            if (parameters.Contains("subCategory"))
            {
                this.subCategory = parameters["subCategory"].ToString().Replace(">~", ">=");
            }
            if (parameters.Contains("searchPoint"))
            {
                this.searchPoint = PointShape.CreateShapeFromWellKnownData(parameters["searchPoint"].ToString()) as PointShape;
            }

            if (parameters.Contains("searchMode") && parameters["searchMode"].ToString().Equals("ServiceArea", StringComparison.OrdinalIgnoreCase))
            {
                this.serviceSearchMode = ServiceSearchMode.ServiceArea;
                this.drivingTime = Convert.ToInt32(parameters["driveTime"].ToString(), CultureInfo.InvariantCulture);
            }
            else
            {
                this.serviceSearchMode = ServiceSearchMode.BufferArea;
                this.bufferDistance = Convert.ToDouble(parameters["bufferDistance"].ToString(), CultureInfo.InvariantCulture);
                this.distanceUnit = GetDistanceFrom(parameters["distanceUnit"].ToString());
            }
        }

        public string RouteFilePathName
        {
            get { return routeFilePathName; }
            set { routeFilePathName = value; }
        }

        public GeographyUnit MapUnit
        {
            get { return mapUnit; }
            set { mapUnit = value; }
        }

        public PointShape SearchPoint
        {
            get { return searchPoint; }
            set { searchPoint = value; }
        }

        public string Category
        {
            get { return category; }
            set { category = value; }
        }

        public string SubCategory
        {
            get { return subCategory; }
            set { subCategory = value; }
        }

        public ServiceSearchMode ServiceSearchMode
        {
            get { return serviceSearchMode; }
            set { serviceSearchMode = value; }
        }

        public DistanceUnit DistanceUnit
        {
            get { return distanceUnit; }
            set { distanceUnit = value; }
        }

        public AccessibleAreaAnalyst GetAccessibleAreaAnalyst()
        {
            AccessibleAreaAnalyst analyst;
            if (ServiceSearchMode == SiteSelection.ServiceSearchMode.ServiceArea)
            {
                analyst = new RouteAccessibleAreaAnalyst(SearchPoint, MapUnit)
                {
                    StreetShapeFilePathName = RouteFilePathName,
                    DrivingTimeInMinutes = drivingTime
                };
            }
            else
            {
                analyst = new BufferAccessibleAreaAnalyst(SearchPoint, MapUnit)
                {
                    Distance = bufferDistance,
                    DistanceUnit = distanceUnit
                };
            }

            return analyst;
        }

        private DistanceUnit GetDistanceFrom(string inputUnit)
        {
            DistanceUnit distanceUnit = DistanceUnit.Kilometer;
            switch (inputUnit.ToLowerInvariant())
            {
                case "mile":
                    distanceUnit = DistanceUnit.Mile;
                    break;
                default:
                    break;
            }

            return distanceUnit;
        }
    }
}