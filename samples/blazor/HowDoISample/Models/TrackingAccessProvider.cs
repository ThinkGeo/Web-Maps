using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Blazor.HowDoI
{
    public class TrackingAccessProvider
    {
        private readonly string dataRootPath;

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
            var records = ParseSpatialFenceRecords(path);
            foreach (var record in records)
            {
                spatialFences.Add(new Feature(record.Wkt, record.FeatureId));
            }
            return spatialFences;
        }

        public void DeleteSpatialFences(IEnumerable<Feature> features)
        {
            var path = Path.Combine(dataRootPath, "SpatialFence.txt");
            var targetIds = new HashSet<string>((features ?? Enumerable.Empty<Feature>()).Where(f => f != null).Select(f => f.Id));
            var records = ParseSpatialFenceRecords(path);
            var result = records.Where(record => !targetIds.Contains(record.FeatureId)).ToList();
            WriteSpatialFenceRecords(path, result);
        }

        public void UpdateSpatialFenceByFeature(Feature feature)
        {
            if (feature == null || string.IsNullOrEmpty(feature.Id))
            {
                return;
            }

            var path = Path.Combine(dataRootPath, "SpatialFence.txt");
            var records = ParseSpatialFenceRecords(path);
            var recordToUpdate = records.FirstOrDefault(record => record.FeatureId == feature.Id);
            if (recordToUpdate != null)
            {
                recordToUpdate.Wkt = feature.GetWellKnownText();
                WriteSpatialFenceRecords(path, records);
            }
        }

        public void InsertSpatialFence(Feature feature)
        {
            if (feature == null || string.IsNullOrEmpty(feature.Id))
            {
                return;
            }

            var path = Path.Combine(dataRootPath, "SpatialFence.txt");
            var records = ParseSpatialFenceRecords(path);
            var existing = records.FirstOrDefault(record => record.FeatureId == feature.Id);
            if (existing != null)
            {
                existing.Wkt = feature.GetWellKnownText();
                WriteSpatialFenceRecords(path, records);
                return;
            }

            int latestId = records.Count > 0 ? records.Max(record => record.Sequence) : 0;
            records.Add(new SpatialFenceRecord
            {
                Sequence = latestId + 1,
                Wkt = feature.GetWellKnownText(),
                FeatureId = feature.Id
            });

            WriteSpatialFenceRecords(path, records);
        }

        private static List<SpatialFenceRecord> ParseSpatialFenceRecords(string filePath)
        {
            List<SpatialFenceRecord> result = new List<SpatialFenceRecord>();
            if (!File.Exists(filePath))
            {
                return result;
            }

            foreach (var line in File.ReadLines(filePath))
            {
                if (TryParseSpatialFenceRecord(line, out var record))
                {
                    result.Add(record);
                }
            }

            return result;
        }

        private static bool TryParseSpatialFenceRecord(string line, out SpatialFenceRecord record)
        {
            record = null;
            if (string.IsNullOrWhiteSpace(line))
            {
                return false;
            }

            int firstComma = line.IndexOf(',');
            int lastComma = line.LastIndexOf(',');
            if (firstComma <= 0 || lastComma <= firstComma + 1 || lastComma >= line.Length - 1)
            {
                return false;
            }

            string sequenceText = line.Substring(0, firstComma).Trim();
            string wktText = line.Substring(firstComma + 1, lastComma - firstComma - 1).Trim();
            string featureId = line.Substring(lastComma + 1).Trim();

            if (!int.TryParse(sequenceText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int sequence))
            {
                return false;
            }

            if (wktText.Length >= 2 && wktText.StartsWith("\"") && wktText.EndsWith("\""))
            {
                wktText = wktText.Substring(1, wktText.Length - 2).Replace("\"\"", "\"");
            }

            if (string.IsNullOrWhiteSpace(wktText) || string.IsNullOrWhiteSpace(featureId))
            {
                return false;
            }

            record = new SpatialFenceRecord
            {
                Sequence = sequence,
                Wkt = wktText,
                FeatureId = featureId
            };
            return true;
        }

        private static void WriteSpatialFenceRecords(string filePath, IEnumerable<SpatialFenceRecord> records)
        {
            var lines = (records ?? Enumerable.Empty<SpatialFenceRecord>())
                .OrderBy(record => record.Sequence)
                .Select(record => $"{record.Sequence},\"{EscapeCsvValue(record.Wkt)}\",{record.FeatureId}")
                .ToList();
            File.WriteAllLines(filePath, lines);
        }

        private static string EscapeCsvValue(string value)
        {
            return (value ?? string.Empty).Replace("\"", "\"\"");
        }

        private sealed class SpatialFenceRecord
        {
            public int Sequence { get; set; }

            public string Wkt { get; set; }

            public string FeatureId { get; set; }
        }

        private static List<List<string>> ParseCsv(string filePath)
        {
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
                        var bufffer = new char[1024];
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
            catch (Exception)
            {
                return null;
            }

        }
    }
}
