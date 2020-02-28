using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Blazor.HowDoI
{
    public class TrackingAccessProvider
    {
        private string dataRootPath;

        public TrackingAccessProvider(string dataRootPath)
        {
            this.dataRootPath = dataRootPath;
        }

        public List<Vehicle> GetCurrentVehicles(DateTime currentTime)
        {
            List<Vehicle> vehiclesList = new List<Vehicle>();

            var path = Path.Combine(dataRootPath, "Vehicle.txt");
            var records = ParseCsv(path);
            TimeSpan trackHistoryVehicleTimeSpan = TimeSpan.FromHours(8);
            foreach (var record in records)
            {
                int vehicleId = Convert.ToInt32(record[1], CultureInfo.InvariantCulture);
                Vehicle vehicle = GetCurrentVehicle(vehicleId, currentTime, trackHistoryVehicleTimeSpan);
                vehiclesList.Add(vehicle);
            }

            return vehiclesList;
        }

        private Vehicle GetCurrentVehicle(int vehicleId, DateTime currentTime, TimeSpan trackHistoryVehicleTimeSpan)
        {
            DateTime trackStartTime = currentTime.AddTicks(-trackHistoryVehicleTimeSpan.Ticks);
            Vehicle currentVechicle = new Vehicle(vehicleId);

            var vehicleFilePath = Path.Combine(dataRootPath, "Vehicle.txt");
            var vehicleRecords = ParseCsv(vehicleFilePath);
            foreach (var vehicleRecord in vehicleRecords)
            {
                int id = Convert.ToInt32(vehicleRecord[1], CultureInfo.InvariantCulture);
                if (id == vehicleId)
                {
                    currentVechicle.Id = vehicleId;
                    currentVechicle.VehicleName = vehicleRecord[0];
                    currentVechicle.VehicleIconVirtualPath = "images/" + vehicleRecord[2];
                    break;
                }
            }

            // Get the locations from current time back to the passed time span
            Collection<double> historySpeeds = new Collection<double>();
            var locationFilePath = Path.Combine(dataRootPath, "Location.txt");
            var records = ParseCsv(locationFilePath).Where(r =>
            {
                DateTime dateTime = Convert.ToDateTime(r[4], CultureInfo.InvariantCulture);
                return r[1] == vehicleId.ToString() && dateTime <= currentTime && dateTime >= trackStartTime;
            }).OrderByDescending(r => r[4]).ToList();
            for (int rowIndex = 0; rowIndex < records.Count; rowIndex++)
            {
                var columns = records[rowIndex];
                double latitude = Convert.ToDouble(columns[3], CultureInfo.InvariantCulture);
                double longitude = Convert.ToDouble(columns[2], CultureInfo.InvariantCulture);
                double speed = Convert.ToDouble(columns[5], CultureInfo.InvariantCulture);
                DateTime dateTime = Convert.ToDateTime(columns[4], CultureInfo.InvariantCulture);
                Location currentLocation = new Location(longitude, latitude, speed, dateTime);
                historySpeeds.Add(speed);

                if (rowIndex == 0)
                {
                    currentVechicle.Location = currentLocation;
                }
                else
                {
                    currentVechicle.HistoryLocations.Add(currentLocation);
                }
            }

            return currentVechicle;
        }

        public List<Feature> GetSpatialFences()
        {
            List<Feature> spatialFences = new List<Feature>();
            var path = Path.Combine(dataRootPath, "SpatialFence.txt");
            var records = ParseCsv(path);
            foreach (var record in records)
            {
                string wkt = record[1];
                string id = record[2];
                spatialFences.Add(new Feature(wkt, id));
            }
            return spatialFences;
        }

        public void DeleteSpatialFences(IEnumerable<Feature> features)
        {
            List<string> resultRecords = new List<string>();
            var path = Path.Combine(dataRootPath, "SpatialFence.txt");
            var records = ParseCsv(path);
            foreach (var record in records)
            {
                bool needDelete = false;
                foreach (Feature feature in features)
                {
                    if (feature.Id == record[2])
                    {
                        needDelete = true;
                        break;
                    }
                }

                if (!needDelete)
                {
                    resultRecords.Add(string.Join(",", record));
                }
            }

            File.WriteAllLines(path, resultRecords);
        }

        public void UpdateSpatialFenceByFeature(Feature feature)
        {
            var path = Path.Combine(dataRootPath, "SpatialFence.txt");
            var records = ParseCsv(path);
            List<string> result = new List<string>();
            foreach (var record in records)
            {
                if (record[2] == feature.Id)
                {
                    record[1] = $"\"{feature.GetWellKnownText()}\"";
                    result.Add(string.Join(",", record));
                }
                else
                    result.Add(string.Join(",", record));
            }

            File.WriteAllLines(path, result);
        }

        public void InsertSpatialFence(Feature feature)
        {
            var path = Path.Combine(dataRootPath, "SpatialFence.txt");
            var records = ParseCsv(path);
            Dictionary<int, string> result = new Dictionary<int, string>();
            foreach (var record in records)
            {
                result.Add(int.Parse(record[0]), $"{record[0]},\"{record[1]}\",{record[2]}");
            }

            result = result.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);
            var latestId = result.ElementAt(result.Count - 1).Key;
            latestId += 1;
            result.Add(latestId, $"{latestId},\"{feature.GetWellKnownText()}\",{feature.Id}");

            File.WriteAllLines(path, result.Values);
        }

        private List<List<string>> ParseCsv(string filePath)
        {
            char[] bufffer = null;
            List<string> dataLine = new List<string>();
            List<char> dataSubStr = new List<char>();
            List<List<string>> LinesInfo = new List<List<string>>();

            bool IsInQuote = false;
            bool IsNewLine = false;
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    StreamReader CsvReader = new StreamReader(fs);
                    while (CsvReader.Peek() >= 0)
                    {
                        bufffer = new char[1024];
                        CsvReader.Read(bufffer, 0, bufffer.Length);
                        for (int i = 0; i < bufffer.Length; i++)
                        {
                            if (bufffer[i] == '"')
                            {
                                dataSubStr.Add(bufffer[i]);
                                IsInQuote = !IsInQuote;
                            }
                            else if (bufffer[i] == ',' && !IsInQuote)
                            {
                                string strColumn = new string(dataSubStr.ToArray<char>());
                                dataLine.Add(strColumn.Trim().Trim('\"'));
                                dataSubStr = new List<char>();
                            }
                            else if (bufffer[i] == '\n' && !IsNewLine)
                            {
                                dataSubStr.Add(bufffer[i]);
                            }
                            else if (bufffer[i] == '\r')
                            {
                                IsNewLine = true;
                                continue;
                            }
                            else if (bufffer[i] == '\n' && IsNewLine)
                            {
                                dataLine.Add(new string(dataSubStr.ToArray<char>()).Trim('\"'));
                                IsNewLine = false;
                                LinesInfo.Add(dataLine);
                                dataSubStr = new List<char>();
                                dataLine = new List<string>();
                            }
                            else
                            {
                                dataSubStr.Add(bufffer[i]);
                            }
                        }
                    }
                }
                return LinesInfo;
            }
            catch (Exception e)
            {
                return null;
            }

        }
    }
}
