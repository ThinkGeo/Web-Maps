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
using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.VehicleTracking;

namespace MapSuiteVehicleTracking.Controllers
{
    public class DefaultController : Controller
    {
        private readonly DateTime adjustedStartTime = new DateTime(2009, 7, 10, 11, 31, 0);

        public ActionResult Index()
        {
            Map map = InitializeMap();
            return View(map);
        }

        [MapActionFilter]
        public ActionResult DisplayVehicles(Map map, GeoCollection<object> args)
        {
            // Read vehicles from database
            DateTime currentTime = GetAdjustedCurrentDateTime();
            Dictionary<int, Vehicle> vehicles = GetVehicles(map, currentTime);

            // Update vehicle information to Map
            UpdateVehiclesToOverlays(map, vehicles);

            return View(vehicles.Values.ToList());
        }

        [MapActionFilter]
        public JsonResult EditFences(Map map, GeoCollection<object> args)
        {
            Collection<JsonFeature> jsonFeatures = new Collection<JsonFeature>();

            LayerOverlay spatialFenceOverlay = (LayerOverlay)map.CustomOverlays["SpatialFenceOverlay"];
            InMemoryFeatureLayer spatialFenceLayer = (InMemoryFeatureLayer)spatialFenceOverlay.Layers["SpatialFenceLayer"];
            foreach (Feature feature in spatialFenceLayer.InternalFeatures)
            {
                // Add feature into editOverlay on server side to make sure it's synchronous on client and server side
                map.EditOverlay.Features.Add(feature);
                // Create feature on client side
                JsonFeature jsonFeature = new JsonFeature(feature.Id, feature.GetWellKnownText());
                jsonFeatures.Add(jsonFeature);
            }
            return Json(jsonFeatures);
        }

        [MapActionFilter]
        public JsonResult SaveFences(Map map, GeoCollection<object> args)
        {
            string result = string.Empty;
            Collection<JsonFeature> jsonFeatures = Newtonsoft.Json.JsonConvert.DeserializeObject<Collection<JsonFeature>>(args[0].ToString());
            result = string.Empty + SaveSpatialFences(map, jsonFeatures);
            return Json(new { status = result });
        }

        private bool SaveSpatialFences(Map Map1, Collection<JsonFeature> features)
        {
            LayerOverlay spatialFenceOverlay = (LayerOverlay)Map1.CustomOverlays["SpatialFenceOverlay"];
            InMemoryFeatureLayer spatialFenceLayer = (InMemoryFeatureLayer)spatialFenceOverlay.Layers["SpatialFenceLayer"];

            try
            {
                spatialFenceLayer.Open();

                Collection<Feature> newAddedFeatures = new Collection<Feature>();
                foreach (JsonFeature jsonFeature in features)
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
                    TrackingAccessProvider vehicleProvider = new TrackingAccessProvider(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["AccessDataBase"]);
                    // Insert new added and modified features to database
                    foreach (Feature feature in newAddedFeatures)
                    {
                        vehicleProvider.InsertSpatialFence(feature);
                    }
                }
                else // Deal with modify mode is activated scenario
                {
                    Collection<Feature> modifiedFeatures = new Collection<Feature>();
                    Dictionary<string, JsonFeature> keyedJsonFeatures = new Dictionary<string, JsonFeature>();
                    foreach (JsonFeature jsonFeature in features)
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
                    TrackingAccessProvider vehicleProvider = new TrackingAccessProvider(AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["AccessDataBase"]);
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

        private bool ChangeScaleBarUnit(Map Map1, UnitSystem unitSystem)
        {
            ScaleBarAdornmentLayer scaleBarAdormentLayer = Map1.AdornmentOverlay.Layers[0] as ScaleBarAdornmentLayer;
            scaleBarAdormentLayer.UnitFamily = unitSystem;

            return true;
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
            if (Session["ApplicationStartTime"] == null)
            {
                Session["ApplicationStartTime"] = DateTime.Now;
                currentSampleTime = adjustedStartTime;
            }
            else
            {
                double sampleSecondPerActualSecond = 12;
                double realSpentTime = TimeSpan.FromTicks(DateTime.Now.Ticks - ((DateTime)Session["ApplicationStartTime"]).Ticks).TotalSeconds;
                int sampleSpentTime = (int)(realSpentTime * sampleSecondPerActualSecond);
                currentSampleTime = adjustedStartTime.AddSeconds(sampleSpentTime);
            }

            return currentSampleTime;
        }

        private bool CheckIsInSpatialFence(Map Map1, Vehicle vehicle)
        {
            LayerOverlay spatialFenceOverlay = (LayerOverlay)Map1.CustomOverlays["SpatialFenceOverlay"];
            InMemoryFeatureLayer spatialFenceLayer = (InMemoryFeatureLayer)spatialFenceOverlay.Layers["SpatialFenceLayer"];

            // Get the point shape and then check if it is within any of the sptail fences using the QueryTools
            PointShape pointShape = new PointShape(vehicle.Location.Longitude, vehicle.Location.Latitude);
            spatialFenceLayer.Open();
            Collection<Feature> spatialFencesWithin = spatialFenceLayer.QueryTools.GetFeaturesContaining(pointShape, ReturningColumnsType.NoColumns);
            spatialFenceLayer.Close();

            return spatialFencesWithin.Count > 0;
        }

        private Dictionary<int, Vehicle> GetVehicles(Map Map1, DateTime currentTime)
        {
            Dictionary<int, Vehicle> vehicles = new Dictionary<int, Vehicle>();

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
                vehicle.Value.IsInFence = CheckIsInSpatialFence(Map1, vehicle.Value);
            }

            return vehicles;
        }

        private Collection<Feature> GetSpatialFences()
        {
            // Get the spatial fences from the database            
            Collection<Feature> spatialFences = new Collection<Feature>();
            TrackingAccessProvider vehicleProvider = new TrackingAccessProvider(Server.MapPath(ConfigurationManager.AppSettings["AccessDataBase"]));
            spatialFences = vehicleProvider.GetSpatialFences();

            return spatialFences;
        }

        private Map InitializeMap()
        {
            Map Map1 = new Map("Map1");
            Map1.MapUnit = GeographyUnit.Meter;
            Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            Map1.Width = new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage);
            Map1.Height = new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage);
            Map1.CurrentExtent = new RectangleShape(-10785241.6495495, 3916508.33762434, -10778744.5183967, 3912187.74540771);
            Map1.MapTools.MiniMap.Enabled = true;
            Map1.MapTools.OverlaySwitcher.Enabled = true;
            Map1.EditOverlay.EditSettings.IsResizable = false;

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay ThinkGeoCloudMapsLight = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key")
            {
                Name = "ThinkGeo Cloud Maps Light",
                MapType = ThinkGeoCloudRasterMapsMapType.Light
            };
            Map1.CustomOverlays.Add(ThinkGeoCloudMapsLight);

            ThinkGeoCloudRasterMapsOverlay ThinkGeoCloudMapsAerial = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key")
            {
                Name = "ThinkGeo Cloud Maps Aerial",
                MapType = ThinkGeoCloudRasterMapsMapType.Aerial
            };
            Map1.CustomOverlays.Add(ThinkGeoCloudMapsAerial);

            ThinkGeoCloudRasterMapsOverlay ThinkGeoCloudMapsHybrid = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key")
            {
                Name = "ThinkGeo Cloud Maps Hybrid",
                MapType = ThinkGeoCloudRasterMapsMapType.Hybrid
            };
            Map1.CustomOverlays.Add(ThinkGeoCloudMapsHybrid);

            OpenStreetMapOverlay openStreetMapOverlay = new OpenStreetMapOverlay("Open Street Map");
            Map1.CustomOverlays.Add(openStreetMapOverlay);

            // Add spatial fences
            AddSpatialFenceOverlay(Map1);

            // Add vehicles.
            AddVehicleMarkerOverlay(Map1);

            // Add vehicles position history.
            AddVehicleHistoryMarkerOverlay(Map1);

            // Add scale bar
            ScaleBarAdornmentLayer scaleBarAdormentLayer = new ScaleBarAdornmentLayer();
            scaleBarAdormentLayer.XOffsetInPixel = 10;
            scaleBarAdormentLayer.UnitFamily = UnitSystem.Metric;
            //Map1.AdornmentOverlay.Layers.Add(scaleBarAdormentLayer);

            return Map1;
        }

        private void AddSpatialFenceOverlay(Map Map1)
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

        private void AddVehicleMarkerOverlay(Map Map1)
        {
            DateTime currentTime = GetAdjustedCurrentDateTime();
            Dictionary<int, Vehicle> vehicles = GetVehicles(Map1, currentTime);

            ValueMarkerStyle valueStyle = new ValueMarkerStyle("VehicleId");
            foreach (var item in vehicles)
            {
                Vehicle vehicle = item.Value;
                WebImage currentPositionImage = new WebImage(vehicle.VehicleIconVirtualPath);
                PointMarkerStyle currentPositionStyle = new PointMarkerStyle(currentPositionImage);
                MarkerValueItem currentPositionItem = new MarkerValueItem(vehicle.Id.ToString(), (MarkerStyle)currentPositionStyle);
                valueStyle.ValueItems.Add(currentPositionItem);
            }

            InMemoryMarkerOverlay vehiclesMarkerOverlay = new InMemoryMarkerOverlay("VehicleOverlay", new FeatureSourceColumn[] { new FeatureSourceColumn("VehicleId") });
            vehiclesMarkerOverlay.IsVisibleInOverlaySwitcher = false;
            vehiclesMarkerOverlay.IsBaseOverlay = false;
            vehiclesMarkerOverlay.ZoomLevelSet.ZoomLevel01.CustomMarkerStyle = valueStyle;
            vehiclesMarkerOverlay.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            Map1.CustomOverlays.Add(vehiclesMarkerOverlay);
        }

        private void AddVehicleHistoryMarkerOverlay(Map Map1)
        {
            InMemoryMarkerOverlay vehicleHistoryOverlay = new InMemoryMarkerOverlay("VehicleHistoryOverlay", new FeatureSourceColumn[]{
                new FeatureSourceColumn("IsCurrentPosition"),
                new FeatureSourceColumn("Speed"),
                new FeatureSourceColumn("DateTime"),
                new FeatureSourceColumn("Longitude"),
                new FeatureSourceColumn("Latitude"),
                new FeatureSourceColumn("VehicleName"),
                new FeatureSourceColumn("Duration")
            });
            vehicleHistoryOverlay.IsBaseOverlay = false;
            vehicleHistoryOverlay.IsVisibleInOverlaySwitcher = false;
            Map1.CustomOverlays.Add(vehicleHistoryOverlay);

            StringBuilder popupHtml = new StringBuilder("<table>");
            popupHtml.Append("<tr><td colspan='2' class='vehicleName'>[#VehicleName#]</td></tr>");
            popupHtml.Append("<tr><td colspan='2'><div class='hrLine'></div></td></tr>");
            popupHtml.Append("<tr class='vehicleTxt'><td>Longitude:</td><td>[#Longitude#]</td></tr>");
            popupHtml.Append("<tr class='vehicleTxt'><td>Latitude:</td><td>[#Latitude#]</td></tr>");
            popupHtml.Append("<tr class='vehicleTxt'><td>Speed:</td><td>[#Speed#]</td></tr>");
            popupHtml.Append("<tr class='vehicleTxt'><td>Time</td><td>[#DateTime#]</td></tr>");
            popupHtml.Append("</table>");

            vehicleHistoryOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage = new WebImage("images/trail point.png", 6, 6, -3, -3);
            vehicleHistoryOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.BorderWidth = 1;
            vehicleHistoryOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.BorderColor = GeoColor.StandardColors.Gray;
            vehicleHistoryOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.ContentHtml = popupHtml.ToString();
            vehicleHistoryOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.AutoSize = true;
            vehicleHistoryOverlay.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        }

        private void UpdateVehiclesToOverlays(Map Map1, Dictionary<int, Vehicle> vehicles)
        {
            InMemoryMarkerOverlay vehiclesMarkerOverlay = Map1.CustomOverlays["VehicleOverlay"] as InMemoryMarkerOverlay;
            InMemoryMarkerOverlay vehiclesHistoryOverlay = Map1.CustomOverlays["VehicleHistoryOverlay"] as InMemoryMarkerOverlay;

            // Clear old vehicle's old positions
            vehiclesMarkerOverlay.FeatureSource.InternalFeatures.Clear();
            vehiclesHistoryOverlay.FeatureSource.InternalFeatures.Clear();

            foreach (var currentVehicle in vehicles)
            {
                Vehicle vehicle = currentVehicle.Value;
                Feature vehicleFeature = new Feature(vehicle.Location.GetLocationPointShape().GetWellKnownBinary());
                vehicleFeature.ColumnValues.Add("VehicleId", vehicle.Id.ToString());

                vehiclesMarkerOverlay.FeatureSource.Open();
                vehiclesMarkerOverlay.FeatureSource.InternalFeatures.Add(vehicleFeature);

                // Add the vheicle's location histories                    
                foreach (Location historyLocation in currentVehicle.Value.HistoryLocations.Take(5))
                {
                    Feature breadcrumbFeature = new Feature(historyLocation.GetLocationPointShape().GetWellKnownBinary());
                    breadcrumbFeature.ColumnValues.Add("DateTime", historyLocation.DateTime.ToString());
                    breadcrumbFeature.ColumnValues.Add("IsCurrentPosition", "IsNotCurrentPosition");
                    breadcrumbFeature.ColumnValues.Add("Speed", historyLocation.Speed.ToString(CultureInfo.InvariantCulture));

                    Location projectedLocation = ProjectLocation(historyLocation);
                    breadcrumbFeature.ColumnValues.Add("Longitude", projectedLocation.Longitude.ToString("N6", CultureInfo.InvariantCulture));
                    breadcrumbFeature.ColumnValues.Add("Latitude", projectedLocation.Latitude.ToString("N6", CultureInfo.InvariantCulture));
                    breadcrumbFeature.ColumnValues.Add("VehicleName", currentVehicle.Value.VehicleName);
                    breadcrumbFeature.ColumnValues.Add("Duration", currentVehicle.Value.SpeedDuration.ToString(CultureInfo.InvariantCulture));
                    vehiclesHistoryOverlay.FeatureSource.InternalFeatures.Add(breadcrumbFeature);
                }

                // Add the vehicle's latest position
                Feature latestPositionFeature = new Feature(currentVehicle.Value.Location.GetLocationPointShape().GetWellKnownBinary(), currentVehicle.Value.VehicleName);
                latestPositionFeature.ColumnValues["DateTime"] = currentVehicle.Value.Location.DateTime.ToString();
                latestPositionFeature.ColumnValues["IsCurrentPosition"] = "IsCurrentPosition";
                latestPositionFeature.ColumnValues["Speed"] = currentVehicle.Value.Location.Speed.ToString(CultureInfo.InvariantCulture);

                Location projectedCurrentLocation = ProjectLocation(currentVehicle.Value.Location);
                latestPositionFeature.ColumnValues["Longitude"] = projectedCurrentLocation.Longitude.ToString("N6", CultureInfo.InvariantCulture);
                latestPositionFeature.ColumnValues["Latitude"] = projectedCurrentLocation.Latitude.ToString("N6", CultureInfo.InvariantCulture);
                latestPositionFeature.ColumnValues["VehicleName"] = currentVehicle.Value.VehicleName;
                latestPositionFeature.ColumnValues["Duration"] = currentVehicle.Value.SpeedDuration.ToString(CultureInfo.InvariantCulture);
                vehiclesHistoryOverlay.FeatureSource.InternalFeatures.Add(latestPositionFeature);

                vehiclesHistoryOverlay.FeatureSource.Close();
            }
        }

        private Location ProjectLocation(Location location)
        {
            Proj4Projection prj4 = new Proj4Projection();
            prj4.InternalProjectionParametersString = Proj4Projection.GetGoogleMapParametersString();
            prj4.ExternalProjectionParametersString = Proj4Projection.GetEpsgParametersString(4326);
            prj4.Open();
            PointShape projectedPoint = prj4.ConvertToExternalProjection(new PointShape(location.Longitude, location.Latitude)) as PointShape;

            Location projectedLocation = new Location();
            projectedLocation.Longitude = Math.Round(projectedPoint.X, 6);
            projectedLocation.Latitude = Math.Round(projectedPoint.Y, 6);
            return projectedLocation;
        }
    }
}
