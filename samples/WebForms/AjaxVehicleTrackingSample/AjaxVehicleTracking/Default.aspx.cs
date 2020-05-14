/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.UI;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace ThinkGeo.MapSuite.AjaxVehicleTracking
{
    public partial class Default : System.Web.UI.Page, ICallbackEventHandler
    {
        // For the sample we set the current time to the time below.  This is so the time will
        // match the data in our sample database.
        private readonly DateTime adjustedStartTime = new DateTime(2009, 7, 10, 11, 31, 0);

        private string callbackResult;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Initialize Map
                InitializeMap();

                // Read vehicles from database
                DateTime currentTime = GetAdjustedCurrentDateTime();
                Collection<Vehicle> vehicles = GetVehicles(currentTime);

                // Update vehicle information to Map
                UpdateVehiclesToOverlays(vehicles);

                // Register Callback references
                Page.ClientScript.GetCallbackEventReference(this, "", "", "");
            }
        }

        public string GetCallbackResult()
        {
            return callbackResult;
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
            CallbackRequest callbackRequest = JSONSerializer.Deserialize<CallbackRequest>(eventArgument);

            switch (callbackRequest.Request)
            {
                case "editFences":
                    callbackResult = JSONSerializer.Serialize(GetSpatialFencesInJsonFeature());
                    break;

                case "saveFences":
                    callbackResult = SaveSpatialFences(callbackRequest.Features).ToString();
                    break;

                case "refreshVehicles":
                    Collection<Vehicle> vehicels = GetVehicles(GetAdjustedCurrentDateTime());
                    UpdateVehiclesToOverlays(vehicels);

                    List<JsonVehicle> jsonVehicels = vehicels.Select((t) =>
                    {
                        return ConvertToJsonVehicle(t);
                    }).ToList();
                    callbackResult = JSONSerializer.Serialize(jsonVehicels);
                    break;

                default:
                    break;
            }
        }

        private JsonVehicle ConvertToJsonVehicle(Vehicle vehicle)
        {
            Collection<JsonLocation> historyLocations = new Collection<JsonLocation>();
            foreach (Location item in vehicle.HistoryLocations)
            {
                historyLocations.Add(new JsonLocation()
                {
                    Latitude = item.Latitude,
                    Longitude = item.Latitude,
                    Speed = item.Speed,
                    TrackTime = item.DateTime.ToString()
                });
            }
            return new JsonVehicle()
            {
                Id = vehicle.Id + string.Empty,
                Name = vehicle.VehicleName,
                CurrentLocation = new JsonLocation(vehicle.Location.Longitude, vehicle.Location.Latitude, vehicle.Location.Speed, vehicle.Location.DateTime.ToString()),
                VehicleIconVirtualPath = vehicle.VehicleIconVirtualPath,
                VehicleMotionState = vehicle.MotionState == VehicleMotionState.Motion ? 1 : 0,
                HistoryLocations = historyLocations,
                IsInFence = CheckIsInSpatialFence(vehicle),
            };
        }

        private Collection<JsonFence> GetSpatialFencesInJsonFeature()
        {
            Collection<JsonFence> jsonFeatures = new Collection<JsonFence>();

            LayerOverlay spatialFenceOverlay = (LayerOverlay)Map1.CustomOverlays["SpatialFenceOverlay"];
            InMemoryFeatureLayer spatialFenceLayer = (InMemoryFeatureLayer)spatialFenceOverlay.Layers["SpatialFenceLayer"];
            foreach (Feature feature in spatialFenceLayer.InternalFeatures)
            {
                // Add feature into editOverlay on server side to make sure it's synchronous on client and server side
                Map1.EditOverlay.Features.Add(feature);
                // Create feature on client side
                JsonFence jsonFeature = new JsonFence(feature.Id, feature.GetWellKnownText());
                jsonFeatures.Add(jsonFeature);
            }

            return jsonFeatures;
        }

        private bool SaveSpatialFences(Collection<JsonFence> features)
        {
            LayerOverlay spatialFenceOverlay = (LayerOverlay)Map1.CustomOverlays["SpatialFenceOverlay"];
            InMemoryFeatureLayer spatialFenceLayer = (InMemoryFeatureLayer)spatialFenceOverlay.Layers["SpatialFenceLayer"];

            try
            {
                spatialFenceLayer.Open();

                Collection<Feature> newAddedFeatures = new Collection<Feature>();
                foreach (JsonFence jsonFeature in features)
                {
                    if (string.IsNullOrEmpty(jsonFeature.Id))
                    {
                        Feature feature = new Feature(jsonFeature.Wkt, Guid.NewGuid().ToString());
                        feature.ColumnValues["Restricted"] = "Restricted";
                        newAddedFeatures.Add(feature);
                    }
                }

                if (features.Count != 0 && newAddedFeatures.Count == features.Count) // if all the features are requested features, means map is under draw polygon
                {
                    // add new features to spatial fence layer
                    foreach (Feature feature in newAddedFeatures)
                    {
                        spatialFenceLayer.InternalFeatures.Add(feature);
                    }

                    // Insert new feature into the database
                    TrackingAccessProvider vehicleProvider = new TrackingAccessProvider(MapPath(ConfigurationManager.AppSettings["AccessDataBase"]));
                    // Insert new added and modified features to database
                    foreach (Feature feature in newAddedFeatures)
                    {
                        vehicleProvider.InsertSpatialFence(feature);
                    }
                }
                else // Deal with modify mode is activated scenario
                {
                    Collection<Feature> modifiedFeatures = new Collection<Feature>();
                    Dictionary<string, JsonFence> keyedJsonFeatures = new Dictionary<string, JsonFence>();
                    foreach (JsonFence jsonFeature in features)
                    {
                        if (!string.IsNullOrEmpty(jsonFeature.Id))  // If id is not null, it's the feature after editing
                        {
                            keyedJsonFeatures.Add(jsonFeature.Id, jsonFeature);
                            Feature feature = new Feature(jsonFeature.Wkt, jsonFeature.Id);
                            feature.ColumnValues["Restricted"] = "Restricted";
                            modifiedFeatures.Add(feature);
                        }
                    }

                    // Get removed features
                    Collection<Feature> removedFeatures = new Collection<Feature>();
                    foreach (Feature feature in Map1.EditOverlay.Features)
                    {
                        if (!keyedJsonFeatures.ContainsKey(feature.Id))
                        {
                            removedFeatures.Add(feature);
                        }
                    }
                    // Refine the spatial fence layer
                    spatialFenceLayer.InternalFeatures.Clear();
                    foreach (Feature feature in newAddedFeatures)
                    {
                        spatialFenceLayer.InternalFeatures.Add(feature);
                    }
                    foreach (Feature feature in modifiedFeatures)
                    {
                        spatialFenceLayer.InternalFeatures.Add(feature);
                    }
                    Map1.EditOverlay.Features.Clear();
                    spatialFenceOverlay.Redraw();

                    // Update the database
                    TrackingAccessProvider vehicleProvider = new TrackingAccessProvider(MapPath(ConfigurationManager.AppSettings["AccessDataBase"]));
                    // Delete all spatial fences from database and then update the existing ones
                    if (removedFeatures.Count > 0)
                    {
                        vehicleProvider.DeleteSpatialFences(removedFeatures);
                    }
                    // Insert new added and modified features to database
                    foreach (Feature feature in newAddedFeatures)
                    {
                        vehicleProvider.InsertSpatialFence(feature);
                    }
                    foreach (Feature feature in modifiedFeatures)
                    {
                        vehicleProvider.UpdateSpatialFenceByFeature(feature);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private DateTime GetAdjustedCurrentDateTime()
        {
            // This vehicle tracking sample contains some simulated data
            // This method stores the real time when the application started and reflects to the start sample time
            // When the actual time increments 1 second, the sample time increments 6 seconds
            //
            // To make the application run in real time just have this method return to current date time
            //
            DateTime currentSampleTime;
            if (ViewState["ApplicationStartTime"] == null)
            {
                ViewState["ApplicationStartTime"] = DateTime.Now;
                currentSampleTime = adjustedStartTime;
            }
            else
            {
                double sampleSecondPerActualSecond = 12;
                double realSpentTime = TimeSpan.FromTicks(DateTime.Now.Ticks - ((DateTime)ViewState["ApplicationStartTime"]).Ticks).TotalSeconds;
                int sampleSpentTime = (int)(realSpentTime * sampleSecondPerActualSecond);
                currentSampleTime = adjustedStartTime.AddSeconds(sampleSpentTime);
            }

            return currentSampleTime;
        }

        private bool CheckIsInSpatialFence(Vehicle vehicle)
        {
            LayerOverlay spatialFenceOverlay = (LayerOverlay)Map1.CustomOverlays["SpatialFenceOverlay"];
            InMemoryFeatureLayer spatialFenceLayer = (InMemoryFeatureLayer)spatialFenceOverlay.Layers["SpatialFenceLayer"];

            // Get the point shape and then check if it is within any of the sptail fences using the QueryTools
            PointShape pointShape = new PointShape(vehicle.Location.Longitude, vehicle.Location.Latitude);
            spatialFenceLayer.Open();
            Collection<Feature> spatialFencesWithin = spatialFenceLayer.QueryTools.GetFeaturesContaining(pointShape, ReturningColumnsType.NoColumns);
            spatialFenceLayer.Close();

            bool isInFence = false;
            if (spatialFencesWithin.Count > 0)
            {
                isInFence = true;
            }

            return isInFence;
        }

        private Collection<Vehicle> GetVehicles(DateTime currentTime)
        {
            Collection<Vehicle> vehicles = new Collection<Vehicle>();

            // If you want to use SQL server as database, please attach /App_Data/VehicleTrackingDb.mdf to
            // SQL Server and change the connection string in the web.config first;
            // Then use the the commended line of code below.
            //
            // using (TrackingSqlProvider vehicleProvider = new TrackingSqlProvider(ConfigurationManager.ConnectionStrings["VehicleTrackingDbConnectionString"].ConnectionString))
            //
            // Get all the vehicles from the database
            TrackingAccessProvider vehicleProvider = new TrackingAccessProvider(Server.MapPath(ConfigurationManager.AppSettings["AccessDataBase"]));
            vehicles = vehicleProvider.GetCurrentVehicles(currentTime);

            foreach (var vehicle in vehicles)
            {
                vehicle.IsInFence = CheckIsInSpatialFence(vehicle);
            }

            return vehicles;
        }

        private Collection<Feature> GetSpatialFences()
        {
            // Get the spatial fences from the database
            Collection<Feature> spatialFences = new Collection<Feature>();
            TrackingAccessProvider vehicleProvider = new TrackingAccessProvider(MapPath(ConfigurationManager.AppSettings["AccessDataBase"]));
            spatialFences = vehicleProvider.GetSpatialFences();


            return spatialFences;
        }

        private void InitializeMap()
        {
            Map1.MapUnit = GeographyUnit.Meter;
            Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            Map1.MapTools.MiniMap.Enabled = true;

            Map1.MapTools.OverlaySwitcher.Enabled = true;
            Map1.MapTools.OverlaySwitcher.BaseOverlayTitle = "ThinkGeo Cloud Maps: ";

            Map1.MapTools.MeasureMapTool.Enabled = true;
            Map1.MapTools.MeasureMapTool.Geodesic = true;
            Map1.MapTools.MeasureMapTool.MeasureUnitType = MeasureUnitType.Metric;
            Map1.MapTools.MeasureMapTool.MeasureType = MeasureType.None;

            Map1.EditOverlay.EditSettings.IsResizable = false;
            Map1.CurrentExtent = new RectangleShape(-10785241.6495495, 3916508.33762434, -10778744.5183967, 3912187.74540771);

            // Please input your ThinkGeo Cloud API Key to enable the background map. 
            ThinkGeoCloudRasterMapsOverlay lightMapOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            lightMapOverlay.Name = "Light";
            lightMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Light;
            Map1.CustomOverlays.Add(lightMapOverlay);

            // Please input your ThinkGeo Cloud API Key to enable the background map. 
            ThinkGeoCloudRasterMapsOverlay darkMapOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            darkMapOverlay.Name = "Dark";
            darkMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Dark;
            Map1.CustomOverlays.Add(darkMapOverlay);

            // Please input your ThinkGeo Cloud API Key to enable the background map. 
            ThinkGeoCloudRasterMapsOverlay aerialMapOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            aerialMapOverlay.Name = "Aerial";
            aerialMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Aerial;
            Map1.CustomOverlays.Add(aerialMapOverlay);

            // Please input your ThinkGeo Cloud API Key to enable the background map. 
            ThinkGeoCloudRasterMapsOverlay hybridMapOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            hybridMapOverlay.Name = "Hybrid";
            hybridMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Hybrid;
            Map1.CustomOverlays.Add(hybridMapOverlay);

            // Add spatial fences
            AddSpatialFenceOverlay();

            // Add scale bar
            ScaleBarAdornmentLayer scaleBarAdormentLayer = new ScaleBarAdornmentLayer();
            scaleBarAdormentLayer.XOffsetInPixel = 10;
            scaleBarAdormentLayer.UnitFamily = UnitSystem.Metric;
            Map1.AdornmentOverlay.Layers.Add(scaleBarAdormentLayer);
        }

        private void AddSpatialFenceOverlay()
        {
            LayerOverlay spatialFenceOverlay = new LayerOverlay("SpatialFenceOverlay");
            spatialFenceOverlay.TileType = TileType.SingleTile;
            spatialFenceOverlay.IsBaseOverlay = false;
            spatialFenceOverlay.IsVisibleInOverlaySwitcher = false;
            Map1.CustomOverlays.Add(spatialFenceOverlay);

            // Initialize SpatialFenceLayers.
            InMemoryFeatureLayer spatialFenceLayer = new InMemoryFeatureLayer();
            spatialFenceLayer.Open();
            spatialFenceLayer.Columns.Add(new FeatureSourceColumn("Restricted", "Charater", 10));
            spatialFenceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoColor.FromArgb(255, 204, 204, 204), 2), new GeoSolidBrush(GeoColor.FromArgb(112, 255, 0, 0)));
            spatialFenceLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Restricted", "Arial", 12, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.SimpleColors.White, 2);
            spatialFenceLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle.OverlappingRule = LabelOverlappingRule.AllowOverlapping;
            spatialFenceLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            spatialFenceOverlay.Layers.Add("SpatialFenceLayer", spatialFenceLayer);

            // Get the spatial fences from the database and insert fences from database into fence layer
            Collection<Feature> spatialFences = GetSpatialFences();
            foreach (Feature spatialFence in spatialFences)
            {
                spatialFence.ColumnValues["Restricted"] = "Restricted";
                spatialFenceLayer.InternalFeatures.Add(spatialFence);
            }
        }

        private void UpdateVehiclesToOverlays(Collection<Vehicle> vehicles)
        {
            foreach (var currentVehicle in vehicles)
            {
                // Create an InMemoryMarkerOverlay for the vehicle to hold the points and current location
                InMemoryMarkerOverlay vehicleOverlay = null;
                if (Map1.CustomOverlays.Contains(currentVehicle.VehicleName))
                {
                    vehicleOverlay = Map1.CustomOverlays[currentVehicle.VehicleName] as InMemoryMarkerOverlay;
                }
                else
                {
                    vehicleOverlay = new InMemoryMarkerOverlay(currentVehicle.VehicleName);
                    vehicleOverlay.Name = currentVehicle.Id.ToString(CultureInfo.InvariantCulture);
                    vehicleOverlay.ZoomLevelSet.ZoomLevel01.CustomMarkerStyle = GetCustomMarkerStyle(currentVehicle.VehicleIconVirtualPath);
                    vehicleOverlay.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                    vehicleOverlay.IsVisibleInOverlaySwitcher = false;
                    Map1.CustomOverlays.Add(vehicleOverlay);

                    // Add all the required columns so we can populate later
                    vehicleOverlay.FeatureSource.Open();
                    vehicleOverlay.Columns.Add(new FeatureSourceColumn("IsCurrentPosition"));
                    vehicleOverlay.Columns.Add(new FeatureSourceColumn("Speed"));
                    vehicleOverlay.Columns.Add(new FeatureSourceColumn("DateTime"));
                    vehicleOverlay.Columns.Add(new FeatureSourceColumn("Longitude"));
                    vehicleOverlay.Columns.Add(new FeatureSourceColumn("Latitude"));
                    vehicleOverlay.Columns.Add(new FeatureSourceColumn("VehicleName"));
                    vehicleOverlay.Columns.Add(new FeatureSourceColumn("Duration"));
                }

                // Clear old vehicle's old positions
                vehicleOverlay.FeatureSource.InternalFeatures.Clear();

                // Add the vheicle's location histories
                foreach (Location historyLocation in currentVehicle.HistoryLocations.Take(5))
                {
                    Feature breadcrumbFeature = new Feature(historyLocation.GetLocationPointShape().GetWellKnownBinary(), currentVehicle.VehicleName + historyLocation.DateTime.ToString());
                    breadcrumbFeature.ColumnValues["DateTime"] = historyLocation.DateTime.ToString();
                    breadcrumbFeature.ColumnValues["IsCurrentPosition"] = "IsNotCurrentPosition";
                    breadcrumbFeature.ColumnValues["Speed"] = historyLocation.Speed.ToString(CultureInfo.InvariantCulture);

                    breadcrumbFeature.ColumnValues["Longitude"] = historyLocation.Longitude.ToString("N6", CultureInfo.InvariantCulture);
                    breadcrumbFeature.ColumnValues["Latitude"] = historyLocation.Latitude.ToString("N6", CultureInfo.InvariantCulture);
                    breadcrumbFeature.ColumnValues["VehicleName"] = currentVehicle.VehicleName;
                    breadcrumbFeature.ColumnValues["Duration"] = currentVehicle.SpeedDuration.ToString(CultureInfo.InvariantCulture);
                    vehicleOverlay.FeatureSource.InternalFeatures.Add(breadcrumbFeature.Id, breadcrumbFeature);
                }

                // Add the vehicle's latest position
                Feature latestPositionFeature = new Feature(currentVehicle.Location.GetLocationPointShape().GetWellKnownBinary(), currentVehicle.VehicleName);
                latestPositionFeature.ColumnValues["DateTime"] = currentVehicle.Location.DateTime.ToString();
                latestPositionFeature.ColumnValues["IsCurrentPosition"] = "IsCurrentPosition";
                latestPositionFeature.ColumnValues["Speed"] = currentVehicle.Location.Speed.ToString(CultureInfo.InvariantCulture);

                latestPositionFeature.ColumnValues["Longitude"] = currentVehicle.Location.Longitude.ToString("N6", CultureInfo.InvariantCulture);
                latestPositionFeature.ColumnValues["Latitude"] = currentVehicle.Location.Latitude.ToString("N6", CultureInfo.InvariantCulture);
                latestPositionFeature.ColumnValues["VehicleName"] = currentVehicle.VehicleName;
                latestPositionFeature.ColumnValues["Duration"] = currentVehicle.SpeedDuration.ToString(CultureInfo.InvariantCulture);
                vehicleOverlay.FeatureSource.InternalFeatures.Add(latestPositionFeature.Id, latestPositionFeature);

                vehicleOverlay.FeatureSource.Close();
            }
        }

        private static ValueMarkerStyle GetCustomMarkerStyle(string vehicleIconVirtualPath)
        {
            StringBuilder popupHtml = new StringBuilder("<table>");
            popupHtml.Append("<tr><td colspan='2' class='vehicleName'>[#VehicleName#]</td></tr>");
            popupHtml.Append("<tr><td colspan='2'><div class='hrLine'></div></td></tr>");
            popupHtml.Append("<tr class='vehicleTxt'><td>Longitude:</td><td>[#Longitude#]</td></tr>");
            popupHtml.Append("<tr class='vehicleTxt'><td>Latitude:</td><td>[#Latitude#]</td></tr>");
            popupHtml.Append("<tr class='vehicleTxt'><td>Speed:</td><td>[#Speed#]</td></tr>");
            popupHtml.Append("<tr class='vehicleTxt'><td>Time</td><td>[#DateTime#]</td></tr>");
            popupHtml.Append("</table>");

            ValueMarkerStyle valueStyle = new ValueMarkerStyle("IsCurrentPosition");

            WebImage currentPositionImage = new WebImage(vehicleIconVirtualPath);
            PointMarkerStyle currentPositionStyle = new PointMarkerStyle(currentPositionImage);
            currentPositionStyle.Popup.BorderWidth = 1;
            currentPositionStyle.Popup.BorderColor = GeoColor.StandardColors.Gray;
            currentPositionStyle.Popup.ContentHtml = popupHtml.ToString();
            currentPositionStyle.Popup.AutoSize = true;
            MarkerValueItem currentPositionItem = new MarkerValueItem("IsCurrentPosition", (MarkerStyle)currentPositionStyle);
            valueStyle.ValueItems.Add(currentPositionItem);

            WebImage historyPositionImage = new WebImage("images/trail point.png", 6, 6, -3, -3);
            PointMarkerStyle historyPositionStyle = new PointMarkerStyle(historyPositionImage);
            historyPositionStyle.Popup.BorderWidth = 1;
            historyPositionStyle.Popup.BorderColor = GeoColor.StandardColors.Gray;
            historyPositionStyle.Popup.ContentHtml = popupHtml.ToString();
            historyPositionStyle.Popup.AutoSize = true;
            MarkerValueItem historyPositionItem = new MarkerValueItem("IsNotCurrentPosition", (MarkerStyle)historyPositionStyle);
            valueStyle.ValueItems.Add(historyPositionItem);

            return valueStyle;
        }
    }
}