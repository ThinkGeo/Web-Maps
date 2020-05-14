/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.UI;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace ThinkGeo.MapSuite.VehicleTracking
{
    public partial class VehicleTracking : Page
    {
        // For the sample we set the current time to the time below.  This is so the time will
        // match the data in our sample database.
        private readonly DateTime adjustedStartTime = new DateTime(2009, 7, 10, 11, 31, 0);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                InitializeMap();

                // Read vehicles from database
                DateTime currentTime = GetAdjustedCurrentDateTime();
                Dictionary<int, Vehicle> vehicles = GetVehicles(currentTime);

                // Update vehicle information and spatial fence on the Map
                UpdateSpatialFencesAndVehicles();

                // Update vehicle information to Map
                UpdateVehiclesToOverlays(vehicles);

                // Display vehicle list
                rptVehicles.DataSource = vehicles.Values;
                rptVehicles.DataBind();
            }
        }

        protected void tmAutoRefreshTimer_Tick(object sender, EventArgs e)
        {
            // Update vehicle information and spatial fence on the Map
            UpdateSpatialFencesAndVehicles();
        }

        protected void ibtnDrawPolygon_Click(object sender, ImageClickEventArgs e)
        {
            Map1.MapTools.MeasureMapTool.MeasureType = MeasureType.None;
            Map1.EditOverlay.TrackMode = TrackMode.Polygon;
        }

        protected void ibtnSaveSpatialFences_Click(object sender, ImageClickEventArgs e)
        {
            Map1.MapTools.MeasureMapTool.MeasureType = MeasureType.None;
            Map1.EditOverlay.TrackMode = TrackMode.None;

            SaveSpatialFences();
            // Redraw spatial fences on the map
            UpdateSpatialFencesToOverlay();
        }

        protected void ibtnDeleteSpatialFences_Click(object sender, EventArgs e)
        {
            LayerOverlay spatialFenceOverlay = (LayerOverlay)Map1.CustomOverlays["SpatialFenceOverlay"];
            InMemoryFeatureLayer spatialFenceLayer = (InMemoryFeatureLayer)spatialFenceOverlay.Layers["SpatialFenceLayer"];

            spatialFenceLayer.InternalFeatures.Clear();
            spatialFenceLayer.FeatureIdsToExclude.Clear();
            SaveSpatialFences();

            // Redraw spatial fences on the map
            UpdateSpatialFencesToOverlay();
        }

        protected void ibtnCancelEditSpatialFences_Click(object sender, ImageClickEventArgs e)
        {
            // Refresh the spatial fence overlay
            LayerOverlay spatialFenceOverlay = (LayerOverlay)Map1.CustomOverlays["SpatialFenceOverlay"];
            InMemoryFeatureLayer spatialFenceLayer = (InMemoryFeatureLayer)spatialFenceOverlay.Layers["SpatialFenceLayer"];
            spatialFenceLayer.FeatureIdsToExclude.Clear();
            spatialFenceOverlay.Redraw();

            // Clear the spatial fences from spatial fence layer
            Map1.EditOverlay.Features.Clear();
            Map1.EditOverlay.TrackMode = TrackMode.Polygon;
            // Disable Measure tool
            Map1.MapTools.MeasureMapTool.MeasureType = MeasureType.None;

            // Update vehicle information on the Map
            DateTime currentTime = GetAdjustedCurrentDateTime();
            UpdateVehiclesToOverlays(GetVehicles(currentTime));
        }

        protected void ibtnEditSpatialFences_Click(object sender, ImageClickEventArgs e)
        {
            LayerOverlay spatialFenceOverlay = (LayerOverlay)Map1.CustomOverlays["SpatialFenceOverlay"];
            InMemoryFeatureLayer spatialFenceLayer = (InMemoryFeatureLayer)spatialFenceOverlay.Layers["SpatialFenceLayer"];

            // Move spatial fences from spatial fence layer to edit overlay for editing
            Map1.EditOverlay.Features.Clear();
            foreach (Feature feature in spatialFenceLayer.InternalFeatures)
            {
                Map1.EditOverlay.Features.Add(feature.Id, feature);
                spatialFenceLayer.FeatureIdsToExclude.Add(feature.Id);
            }
            spatialFenceOverlay.Redraw();

            // Enable the edit mode on map
            Map1.EditOverlay.TrackMode = TrackMode.Edit;
            // Disable Measure tool
            Map1.MapTools.MeasureMapTool.MeasureType = MeasureType.None;

            // Update vehicle information on the Map
            DateTime currentTime = GetAdjustedCurrentDateTime();
            UpdateVehiclesToOverlays(GetVehicles(currentTime));
        }

        protected void ibtnRefreshManually_Click(object sender, ImageClickEventArgs e)
        {
            // Update vehicle information and spatial fence on the Map
            UpdateSpatialFencesAndVehicles();
        }

        protected void ibtnAutoRefresh_Click(object sender, ImageClickEventArgs e)
        {
            // Toggle the timer
            if (tmAutoRefreshTimer.Enabled)
            {
                tmAutoRefreshTimer.Enabled = false;
                lblStatus.Text = "Off";
                lblStatus.ForeColor = Color.Red;
            }
            else
            {
                tmAutoRefreshTimer.Enabled = true;
                lblStatus.Text = "On";
                lblStatus.ForeColor = Color.Black;
            }

            // Update vehicle information and spatial fence on the Map
            UpdateSpatialFencesAndVehicles();
        }

        protected void ibtnMeasureLength_Click(object sender, ImageClickEventArgs e)
        {
            Map1.EditOverlay.TrackMode = TrackMode.None;
            Map1.MapTools.MeasureMapTool.MeasureType = MeasureType.Line;
        }

        protected void ibtnMeasureArea_Click(object sender, ImageClickEventArgs e)
        {
            Map1.EditOverlay.TrackMode = TrackMode.None;
            Map1.MapTools.MeasureMapTool.MeasureType = MeasureType.Area;
        }

        protected void ibtnClearMeasure_Click(object sender, ImageClickEventArgs e)
        {
            Map1.EditOverlay.TrackMode = TrackMode.None;
            Map1.MapTools.MeasureMapTool.MeasureType = MeasureType.None;
        }

        protected void ddlMeasureUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScaleBarAdornmentLayer scaleBarAdormentLayer = Map1.AdornmentOverlay.Layers[0] as ScaleBarAdornmentLayer;
            if (ddlMeasureUnit.SelectedValue == "Imperial")
            {
                scaleBarAdormentLayer.UnitFamily = UnitSystem.Imperial;
                Map1.MapTools.MeasureMapTool.MeasureUnitType = MeasureUnitType.English;
            }
            else
            {
                scaleBarAdormentLayer.UnitFamily = UnitSystem.Metric;
                Map1.MapTools.MeasureMapTool.MeasureUnitType = MeasureUnitType.Metric;
            }
        }

        private void InitializeMap()
        {
            // Setup the map.
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
            ThinkGeoCloudRasterMapsOverlay thinkgeoCloudLightMapOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            thinkgeoCloudLightMapOverlay.Name = "Light";
            thinkgeoCloudLightMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Light;
            Map1.CustomOverlays.Add(thinkgeoCloudLightMapOverlay);

            ThinkGeoCloudRasterMapsOverlay thinkgeCloudDardMapOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            thinkgeCloudDardMapOverlay.Name = "Dark";
            thinkgeCloudDardMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Dark;
            Map1.CustomOverlays.Add(thinkgeCloudDardMapOverlay);

            ThinkGeoCloudRasterMapsOverlay thinkgeoCloudAerialMapOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            thinkgeoCloudAerialMapOverlay.Name = "Aerial";
            thinkgeoCloudAerialMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Aerial;
            Map1.CustomOverlays.Add(thinkgeoCloudAerialMapOverlay);

            ThinkGeoCloudRasterMapsOverlay thinkgeoCloudHybridMapOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            thinkgeoCloudHybridMapOverlay.Name = "Hybrid";
            thinkgeoCloudHybridMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Hybrid;
            Map1.CustomOverlays.Add(thinkgeoCloudHybridMapOverlay);

            // Add spatial fences
            UpdateSpatialFencesToOverlay();

            // Add scale bar
            ScaleBarAdornmentLayer scaleBarAdormentLayer = new ScaleBarAdornmentLayer();
            scaleBarAdormentLayer.XOffsetInPixel = 10;
            scaleBarAdormentLayer.UnitFamily = UnitSystem.Metric;
            Map1.AdornmentOverlay.Layers.Add(scaleBarAdormentLayer);
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

        private void SaveSpatialFences()
        {
            try
            {
                LayerOverlay spatialFenceOverlay = (LayerOverlay)Map1.CustomOverlays["SpatialFenceOverlay"];
                InMemoryFeatureLayer spatialFenceLayer = (InMemoryFeatureLayer)spatialFenceOverlay.Layers["SpatialFenceLayer"];
                foreach (string featureId in spatialFenceLayer.FeatureIdsToExclude)
                {
                    spatialFenceLayer.InternalFeatures.Remove(featureId);
                }
                spatialFenceLayer.FeatureIdsToExclude.Clear();

                // Save the new spatial fences from edit overlay into the spatial fence layer
                foreach (Feature feature in Map1.EditOverlay.Features)
                {
                    if (!spatialFenceLayer.InternalFeatures.Contains(feature))
                    {
                        feature.ColumnValues["Restricted"] = "Restricted";
                        spatialFenceLayer.InternalFeatures.Add(feature.Id, feature);
                    }
                }
                spatialFenceOverlay.Redraw();

                // Synchronize the spatial fences from memory to database
                string dataRootPath = Path.GetFullPath(Server.MapPath(ConfigurationManager.AppSettings["DataRootPath"]));
                TrackingAccessProvider vehicleProvider = new TrackingAccessProvider(dataRootPath);
                // Delete Spatial fences which is not in current spatial fence layer
                vehicleProvider.DeleteSpatialFencesExcluding(spatialFenceLayer.InternalFeatures);

                // Add or update the spatial fences that already exist
                foreach (Feature feature in spatialFenceLayer.InternalFeatures)
                {
                    // Update Spatial fence by feature Id
                    // if the affected data row number is 0, we will add it as a new row into the database
                    vehicleProvider.UpdateSpatialFenceByFeature(feature);
                }

                ScriptManager.RegisterStartupScript(this, this.GetType(), "", "savedSuccessfully()", true);
            }
            catch
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "", "savedFailed()", true);
            }
            finally
            {
                Map1.EditOverlay.Features.Clear();
                Map1.EditOverlay.TrackMode = TrackMode.None;
            }
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

            bool isInSpatialFence = false;
            if (spatialFencesWithin.Count > 0)
            {
                isInSpatialFence = true;
            }

            return isInSpatialFence;
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

        private Collection<Feature> GetSpatialFences()
        {
            // Get the spatial fences from the database
            Collection<Feature> spatialFences = new Collection<Feature>();
            string dataRootPath = Path.GetFullPath(Server.MapPath(ConfigurationManager.AppSettings["DataRootPath"]));
            TrackingAccessProvider vehicleProvider = new TrackingAccessProvider(dataRootPath);
            spatialFences = vehicleProvider.GetSpatialFences();

            return spatialFences;
        }

        private void UpdateSpatialFencesAndVehicles()
        {
            // Update vehicle information on the Map
            // Read vehicles from database
            DateTime currentTime = GetAdjustedCurrentDateTime();
            Dictionary<int, Vehicle> vehicles = GetVehicles(currentTime);

            // Display vehicle list
            rptVehicles.DataSource = vehicles.Values;
            rptVehicles.DataBind();

            // Update vehicle information to Map
            UpdateVehiclesToOverlays(vehicles);

            // Redraw spatial fences on the map
            UpdateSpatialFencesToOverlay();
        }

        private void UpdateSpatialFencesToOverlay()
        {
            InMemoryFeatureLayer spatialFenceLayer = null;
            if (!Map1.CustomOverlays.Contains("SpatialFenceOverlay"))
            {
                // Initialize SpatialFenceOverlay if it's not existed
                LayerOverlay spatialFenceOverlay = new LayerOverlay("SpatialFenceOverlay");
                spatialFenceOverlay.TileType = TileType.SingleTile;
                spatialFenceOverlay.IsBaseOverlay = false;
                spatialFenceOverlay.IsVisibleInOverlaySwitcher = false;
                Map1.CustomOverlays.Add(spatialFenceOverlay);

                // Initialize SpatialFenceLayer.
                spatialFenceLayer = new InMemoryFeatureLayer();
                spatialFenceLayer.Open();
                spatialFenceLayer.Columns.Add(new FeatureSourceColumn("Restricted", "Charater", 10));
                spatialFenceLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoColor.FromArgb(255, 204, 204, 204), 2), new GeoSolidBrush(GeoColor.FromArgb(112, 255, 0, 0)));
                spatialFenceLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("Restricted", "Arial", 12, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.SimpleColors.White, 2);
                spatialFenceLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                spatialFenceOverlay.Layers.Add("SpatialFenceLayer", spatialFenceLayer);
            }
            else
            {
                LayerOverlay spatialFenceOverlay = Map1.CustomOverlays["SpatialFenceOverlay"] as LayerOverlay;
                spatialFenceLayer = (InMemoryFeatureLayer)spatialFenceOverlay.Layers["SpatialFenceLayer"];
            }

            spatialFenceLayer.InternalFeatures.Clear();

            // Get the spatial fences from the database and insert fences from database into fence layer
            Collection<Feature> spatialFences = GetSpatialFences();
            foreach (Feature spatialFence in spatialFences)
            {
                spatialFence.ColumnValues["Restricted"] = "Restricted";
                spatialFenceLayer.InternalFeatures.Add(spatialFence);
            }
        }

        private Dictionary<int, Vehicle> GetVehicles(DateTime currentTime)
        {
            string dataRootPath = Path.GetFullPath(Server.MapPath(ConfigurationManager.AppSettings["DataRootPath"]));
            TrackingAccessProvider vehicleProvider = new TrackingAccessProvider(dataRootPath);
            var vehicles = vehicleProvider.GetCurrentVehicles(currentTime);

            foreach (var vehicle in vehicles)
            {
                vehicle.Value.IsInFence = CheckIsInSpatialFence(vehicle.Value);
            }

            return vehicles;
        }

        private void UpdateVehiclesToOverlays(Dictionary<int, Vehicle> vehicles)
        {
            Proj4Projection proj4 = new Proj4Projection();
            proj4.InternalProjectionParametersString = Proj4Projection.GetGoogleMapParametersString();
            proj4.ExternalProjectionParametersString = Proj4Projection.GetEpsgParametersString(4326);
            proj4.Open();

            foreach (var currentVehicle in vehicles)
            {
                // Create an InMemoryMarkerOverlay for the vehicle to hold the points and current location
                InMemoryMarkerOverlay vehicleOverlay = null;
                if (Map1.CustomOverlays.Contains(currentVehicle.Value.Name))
                {
                    vehicleOverlay = Map1.CustomOverlays[currentVehicle.Value.Name] as InMemoryMarkerOverlay;
                }
                else
                {
                    vehicleOverlay = new InMemoryMarkerOverlay(currentVehicle.Value.Name);
                    vehicleOverlay.Name = currentVehicle.Value.Id.ToString(CultureInfo.InvariantCulture);
                    vehicleOverlay.ZoomLevelSet.ZoomLevel01.CustomMarkerStyle = GetCustomMarkerStyle(currentVehicle.Value.IconPath);
                    vehicleOverlay.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                    vehicleOverlay.IsVisibleInOverlaySwitcher = false;
                    Map1.CustomOverlays.Add(vehicleOverlay);

                    // Add all the required columns so we can populate later
                    vehicleOverlay.FeatureSource.Open();
                }

                // Clear old vehicle's old positions
                vehicleOverlay.FeatureSource.InternalFeatures.Clear();

                // Add the vheicle's location histories
                foreach (Location historyLocation in currentVehicle.Value.HistoryLocations)
                {
                    Feature breadcrumbFeature = new Feature(historyLocation.GetLocation().GetWellKnownBinary(), currentVehicle.Value.Name + historyLocation.DateTime);
                    breadcrumbFeature.ColumnValues["DateTime"] = historyLocation.DateTime.ToString();
                    breadcrumbFeature.ColumnValues["IsCurrentPosition"] = "IsNotCurrentPosition";
                    breadcrumbFeature.ColumnValues["Speed"] = historyLocation.Speed.ToString(CultureInfo.InvariantCulture);

                    Vertex projectedVertex = GetProjectedVertext(proj4, historyLocation.Longitude, historyLocation.Latitude);
                    breadcrumbFeature.ColumnValues["Longitude"] = projectedVertex.X.ToString(CultureInfo.InvariantCulture);
                    breadcrumbFeature.ColumnValues["Latitude"] = projectedVertex.Y.ToString(CultureInfo.InvariantCulture);

                    breadcrumbFeature.ColumnValues["VehicleName"] = currentVehicle.Value.Name;
                    breadcrumbFeature.ColumnValues["Duration"] = currentVehicle.Value.SpeedDuration.ToString(CultureInfo.InvariantCulture);
                    vehicleOverlay.FeatureSource.InternalFeatures.Add(breadcrumbFeature.Id, breadcrumbFeature);
                }

                // Add the vehicle's latest position
                Feature latestPositionFeature = new Feature(currentVehicle.Value.Location.GetLocation().GetWellKnownBinary(), currentVehicle.Value.Name);
                latestPositionFeature.ColumnValues["DateTime"] = currentVehicle.Value.Location.DateTime.ToString();
                latestPositionFeature.ColumnValues["IsCurrentPosition"] = "IsCurrentPosition";
                latestPositionFeature.ColumnValues["Speed"] = currentVehicle.Value.Location.Speed.ToString(CultureInfo.InvariantCulture);

                Vertex lastProjectedVertex = GetProjectedVertext(proj4, currentVehicle.Value.Location.Longitude,
    currentVehicle.Value.Location.Latitude);
                latestPositionFeature.ColumnValues["Longitude"] = lastProjectedVertex.X.ToString(CultureInfo.InvariantCulture);
                latestPositionFeature.ColumnValues["Latitude"] = lastProjectedVertex.Y.ToString(CultureInfo.InvariantCulture);
                latestPositionFeature.ColumnValues["VehicleName"] = currentVehicle.Value.Name;
                latestPositionFeature.ColumnValues["Duration"] = currentVehicle.Value.SpeedDuration.ToString(CultureInfo.InvariantCulture);
                vehicleOverlay.FeatureSource.InternalFeatures.Add(latestPositionFeature.Id, latestPositionFeature);

                vehicleOverlay.FeatureSource.Close();
            }
        }

        private Vertex GetProjectedVertext(Proj4Projection proj4, double x, double y)
        {
            Vertex result = new Vertex();

            Vertex projectedVertex = proj4.ConvertToExternalProjection(x, y);
            result.X = Math.Round(projectedVertex.X, 6);
            result.Y = Math.Round(projectedVertex.Y, 6);

            return result;
        }
    }
}