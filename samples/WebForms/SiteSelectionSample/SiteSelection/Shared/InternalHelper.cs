using System.Collections.Generic;
using System.Data;

namespace ThinkGeo.MapSuite.SiteSelection
{
    public static class InternalHelper
    {
        private static Dictionary<string, string> poiColumns;

        public static DataTable GetQueryResultDefination()
        {
            DataTable queriedResult = new DataTable();
            queriedResult.Columns.Add("WKT");
            queriedResult.Columns.Add("Name");

            return queriedResult;
        }

        public static string GetDbfColumnByPoiType(string poiCategory)
        {
            if (poiColumns == null)
            {
                poiColumns = new Dictionary<string, string>();
                poiColumns.Add(Resource.Hotels, "ROOMS");
                poiColumns.Add(Resource.MedicalFacilites, "TYPE");
                poiColumns.Add(Resource.Restaurants, "FoodType");
                poiColumns.Add(Resource.Schools, "TYPE");
            }

            return poiColumns[poiCategory];
        }
    }
}