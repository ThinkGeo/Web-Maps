using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using ThinkGeo.MapSuite.Shapes;

namespace ThinkGeo.MapSuite.EarthquakeStatistics
{
    public static class InternalHelper
    {
        public static DataTable GetQueriedResultTableDefination()
        {
            DataTable tableDefination = new DataTable();
            tableDefination.Columns.Add("id");
            tableDefination.Columns.Add("year");
            tableDefination.Columns.Add("longitude");
            tableDefination.Columns.Add("latitude");
            tableDefination.Columns.Add("depth_km");
            tableDefination.Columns.Add("magnitude");
            tableDefination.Columns.Add("location");

            return tableDefination;
        }

        public static string ConvertFeaturesToJson(IEnumerable<Feature> features)
        {
            Collection<JsonFeature> jsonFeatures = new Collection<JsonFeature>();
            foreach (Feature feature in features)
            {
                // re-order the columns for display it in query table
                Dictionary<string, string> orderedColumns = new Dictionary<string, string>();
                orderedColumns.Add("year", feature.ColumnValues["YEAR"]);
                orderedColumns.Add("longitude", feature.ColumnValues["LONGITUDE"]);
                orderedColumns.Add("latitude", feature.ColumnValues["LATITIUDE"]);
                orderedColumns.Add("depth_km", feature.ColumnValues["DEPTH_KM"]);
                orderedColumns.Add("magnitude", feature.ColumnValues["magnitude"]);
                orderedColumns.Add("location", feature.ColumnValues["LOCATION"]);

                JsonFeature jsonFeature = new JsonFeature(feature.Id, feature.GetWellKnownText(), orderedColumns);
                jsonFeatures.Add(new JsonFeature(feature));
            }

            return JsonSerializer.Serialize<Collection<JsonFeature>>(jsonFeatures);
        }
    }
}