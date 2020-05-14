using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Shapes;

namespace CSharp_HowDoISamples
{
    public class VehicleTrackingController : Controller
    {
        //Start position for the six vehicles
        private static Dictionary<string, List<PointShape>> vehicles;
        private static int[] readIndex = new int[] { 0, 0, 0, 0, 0, 0 };

        public ActionResult VehicleTracking()
        {
            vehicles = ReadVehicles();

            PointShape[] startPositions = new PointShape[6];
            for (int i = 0; i < 6; i++)
            {
                startPositions[i] = vehicles[(i + 1).ToString(CultureInfo.InvariantCulture)][0];
            }
            TempData["InitPositions"] = startPositions;

            return View();
        }

        public string GetCurrentPosition()
        {
            if (vehicles == null) ReadVehicles();

            GeoCollection<JsonVehicle> jsonVehicles = new GeoCollection<JsonVehicle>();

            for (int i = 0; i < 6; i++)
            {
                string vehicleId = (i + 1).ToString(CultureInfo.InvariantCulture);
                List<PointShape> locations = vehicles[vehicleId];

                readIndex[i]++;
                if (readIndex[i] >= locations.Count)
                {
                    readIndex[i] = 0;
                }

                PointShape location = locations[readIndex[i]];
                JsonVehicle vehicle = new JsonVehicle(vehicleId, "vehicle_van_" + (i + 1) + ".png", location.X, location.Y);

                jsonVehicles.Add(vehicle);
            }

            return Serialize<Collection<JsonVehicle>>(jsonVehicles);
        }

        public string Serialize<T>(T targetValue)
        {
            string result = String.Empty;

            DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(targetValue.GetType());
            using (MemoryStream jsonStream = new MemoryStream())
            {
                jsonSerializer.WriteObject(jsonStream, targetValue);
                result = Encoding.Default.GetString(jsonStream.GetBuffer());
                jsonStream.Close();
            }
            return result.Trim('\0');
        }

        private Dictionary<string, List<PointShape>> ReadVehicles()
        {
            Dictionary<string, List<PointShape>> vehicles = new Dictionary<string, List<PointShape>>();
            Proj4Projection proj4 = new Proj4Projection(4326, 3857);
            proj4.Open();

            for (int i = 1; i <= 6; i++)
            {
                string vehicleId = i.ToString(CultureInfo.InvariantCulture);

                string sql = "SELECT A.VehicleID, B.Longitude, B.Latitude FROM (Vehicle A LEFT OUTER JOIN Location B ON A.VehicleID = B.VehicleID) WHERE (A.VehicleID = {0}) ORDER BY A.VehicleID, B.[Date] DESC";
                sql = String.Format(CultureInfo.InvariantCulture, sql, vehicleId);

                DataTable dt = ExecuteQuery(sql);

                List<PointShape> locations = new List<PointShape>();
                foreach (DataRow row in dt.Rows)
                {
                    double longitude = double.Parse(row["Longitude"].ToString());
                    double latitude = double.Parse(row["Latitude"].ToString());
                    PointShape point = new PointShape(longitude, latitude);
                    point = (PointShape)proj4.ConvertToExternalProjection(point);

                    locations.Add(point);
                }

                vehicles.Add(vehicleId, locations);
            }

            return vehicles;
        }

        private DataTable ExecuteQuery(string selectCommandText)
        {
            string connectionString = string.Format(CultureInfo.InvariantCulture, "Provider=Microsoft.Jet.OLEDB.4.0; Data Source='{0}'", HttpContext.Server.MapPath(ConfigurationManager.AppSettings["AccessDataBase"]));
            OleDbConnection dataConnection = new OleDbConnection(connectionString);

            OleDbDataAdapter dataAdapter = null;
            try
            {
                dataAdapter = new OleDbDataAdapter(selectCommandText, dataConnection);
                dataConnection.Open();
                DataSet dataSet = new DataSet();
                dataSet.Locale = CultureInfo.InvariantCulture;
                dataAdapter.Fill(dataSet);

                if (dataSet.Tables.Count > 0)
                {
                    return dataSet.Tables[0];
                }
                return null;
            }
            finally
            {
                if (dataAdapter != null) { dataAdapter.Dispose(); }
                if (dataConnection != null) { dataConnection.Close(); }
            }
        }
    }
}
