using ThinkGeo.UI.Blazor.HowDoI.CustomerApp.Classes;
using ThinkGeo.UI.Blazor.HowDoI.CustomerApp.SharedProperties;
using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.UI.Blazor;

namespace ThinkGeo.UI.Blazor.HowDoI.CustomerApp.Utilities
{
    /// <summary>
    /// Provides the high-level map orchestration logic for loading assets, managing overlays and layers,
    /// synchronizing persisted feature data, and tracking map view state.
    /// </summary>
    /// <remarks>
    /// This class coordinates the constructor-wired map utility collaborators responsible for feature conversion,
    /// measurement, geographic normalization, layer styling, persistence, and overlay redraw sequencing.
    /// </remarks>
    internal class MapUtilities
    {
        #region Runtime Dependencies
        /// <summary>
        /// Initializes a new <see cref="MapUtilities"/> instance and wires its collaborating utilities to a shared runtime context.
        /// </summary>
        /// <param name="runtimeState">The shared runtime state containing map settings and overlays.</param>
        /// <param name="webRootPath">The root path of the web application, used for resolving web resources.</param>
        /// <param name="mapGeoUtilities">Utilities for geographic calculations.</param>
        /// <param name="mapMeasurementUtilities">Utilities for measuring distances, areas, and other spatial properties.</param>
        /// <param name="mapGeoImageUtilities">Utilities for loading and resolving map icon and image resources.</param>
        /// <param name="mapFeatureConversionUtilities">Utilities for converting between map features and runtime feature models.</param>
        /// <param name="mapLayerUtilities">Utilities for managing map layers.</param>
        /// <remarks>
        /// This constructor wires the provided utility instances to the shared runtime state and establishes their inter-dependencies.
        /// </remarks>
        public MapUtilities(
            MapRuntimeState runtimeState,
            string webRootPath,
            MapGeographicUtilities mapGeoUtilities,
            MapMeasurementUtilities mapMeasurementUtilities,
            MapGeoImageUtilities mapGeoImageUtilities,
            MapFeatureConversionUtilities mapFeatureConversionUtilities,
            MapLayerUtilities mapLayerUtilities)
        {
            // Validate that none of the constructor parameters are null, throwing an ArgumentNullException if any are.
            ArgumentNullException.ThrowIfNull(runtimeState);
            ArgumentNullException.ThrowIfNull(webRootPath);
            ArgumentNullException.ThrowIfNull(mapGeoUtilities);
            ArgumentNullException.ThrowIfNull(mapMeasurementUtilities);
            ArgumentNullException.ThrowIfNull(mapGeoImageUtilities);
            ArgumentNullException.ThrowIfNull(mapFeatureConversionUtilities);
            ArgumentNullException.ThrowIfNull(mapLayerUtilities);

            // Assign the provided parameters to the corresponding properties of the MapUtilities instance.
            RuntimeState = runtimeState;
            WebRootPath = webRootPath;
            MapGeoUtilities = mapGeoUtilities;
            MapMeasurementUtilities = mapMeasurementUtilities;
            MapGeoImageUtilities = mapGeoImageUtilities;
            MapFeatureConversionUtilities = mapFeatureConversionUtilities;
            MapLayerUtilities = mapLayerUtilities;

            // Wire the shared runtime state to each of the utility instances, allowing them to access and modify the map's runtime context.
            MapGeoUtilities.RuntimeState = runtimeState;
            MapMeasurementUtilities.RuntimeState = runtimeState;
            MapGeoImageUtilities.RuntimeState = runtimeState;
            MapFeatureConversionUtilities.RuntimeState = runtimeState;
            MapLayerUtilities.RuntimeState = runtimeState;

            // Establish inter-dependencies between the utility instances.
            MapMeasurementUtilities.MapGeoUtilities = MapGeoUtilities;
            MapFeatureConversionUtilities.MapGeoUtilities = MapGeoUtilities;
            MapFeatureConversionUtilities.MapMeasurementUtilities = MapMeasurementUtilities;
            MapFeatureConversionUtilities.MapGeoImageUtilities = MapGeoImageUtilities;
        }

        /// <summary>
        /// The Settings instance that holds the current map view, map class, and other related settings.
        /// </summary>
        public MapRuntimeState RuntimeState { get; }

        /// <summary>
        /// Utilities for geographic calculations and conversions related to map coordinates.
        /// </summary>
        public MapGeographicUtilities MapGeoUtilities { get; }

        /// <summary>
        /// Utilities for measuring distances, areas, and other spatial properties on the map.
        /// </summary>
        public MapMeasurementUtilities MapMeasurementUtilities { get; }

        /// <summary>
        /// Utilities for managing map layers, including adding, removing, and manipulating layers on the map.
        /// </summary>
        public MapLayerUtilities MapLayerUtilities { get; }

        /// <summary>
        /// Utilities for loading and resolving map icon and image resources.
        /// </summary>
        public MapGeoImageUtilities MapGeoImageUtilities { get; }

        /// <summary>
        /// Utilities for converting between map features and runtime feature models.
        /// </summary>
        public MapFeatureConversionUtilities MapFeatureConversionUtilities { get; }

        /// <summary>
        /// Utilities for line-arrow path/index calculations and arrow direction mapping.
        /// </summary>
        public MapLineArrowUtilities MapLineArrowUtilities { get; set; } = new MapLineArrowUtilities();

        /// <summary>
        /// Gets the configured image utility for use in image loading and resolution.
        /// </summary>
        private MapGeoImageUtilities GeoImageUtilities => MapGeoImageUtilities;

        /// <summary>
        /// Gets the configured feature conversion utility for use in feature conversions.
        /// </summary>
        private MapFeatureConversionUtilities ConversionUtilities => MapFeatureConversionUtilities;

        /// <summary>
        /// Indicates whether the map is in read-only mode, which prevents add, delete, and edit operations while still allowing selection.
        /// </summary>
        private bool IsReadOnly => RuntimeState.IsReadOnly;
        #endregion

        #region Map Initialization and View State
        /// <summary>
        /// Load the map asynchronously, including icons, overlays, and layers. If <paramref name="loadExternalMap"/> is false, it attempts to load the map
        /// from an XML file. It also adds any placed or drawn features to the map and ensures that the map center point and zoom level are set correctly 
        /// based on the current map unit.
        /// </summary>
        /// <param name="loadExternalMap">A boolean indicating whether to load an external map.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task LoadMapAsync(bool loadExternalMap)
        {
            if (RuntimeState.ActiveMapView != null)
            {
                // Set the Map zoom level.
                // The Math.Clamp method is used to restrict the zoom level to the specified range.
                var clampedZoom = Math.Clamp(RuntimeState.ActiveMapClass.MapZoomLevel, MapSettings.MinZoomLevel, MapSettings.MaxZoomLevel);
                RuntimeState.ActiveMapClass.MapZoomLevel = clampedZoom;

                // Load the icons for placeable features.
                // The PNGs are copied into wwwroot\Icons of the host's
                // output/publish directory by the host project (see the BaseMap Icons <None>
                // item in Web.csproj), where IconDirectoryPath's default resolver
                // finds them in both dev and published deployments.
                GeoImageUtilities.LoadGeoImages(MapConstants.IconsDirectoryName, RuntimeState.IconGeoImageIndexMap, RuntimeState.IconGeoImageList);

                // Load the arrow images for directional lines.
                // The PNGs are copied into wwwroot\Arrows of the host's
                // output/publish directory by the host project (see the BaseMap Arrows <None>
                // item in Web.csproj), where ArrowDirectoryPath's default resolver
                // finds them in both dev and published deployments.
                GeoImageUtilities.LoadGeoImages(MapConstants.ArrowsDirectoryName, RuntimeState.ArrowGeoImageIndexMap, RuntimeState.ArrowGeoImageList);

                // Load the overlays (OSM, Placed Features, Drawn Features) and ensure they are configured properly.
                LoadOverlaysAndLayers();

                // If the map has not been loaded, load the map from an XML file if it exists.
                if (!loadExternalMap)
                {
                    RuntimeState.ActiveMapClass = XmlUtilities.LoadFromXml<Map>
                                                  (GetMapFilePath()) ?? new Map();
                }

                // If there are placed features in the loaded map, add them to the map.
                if (RuntimeState.ActiveMapClass.MapPlacedFeatures != null && RuntimeState.ActiveMapClass.MapPlacedFeatures.Count > 0)
                {
                    AddPlacedFeatures();
                }

                // If there are drawn features in the loaded map, add them to the map.
                if (RuntimeState.ActiveMapClass.MapDrawnFeatures != null && RuntimeState.ActiveMapClass.MapDrawnFeatures.Count > 0)
                {
                    await AddDrawnFeaturesAsync();
                }

                // If the map unit is in meters, convert the map center point to map units.
                if (RuntimeState.ActiveMapView.MapUnit == GeographyUnit.Meter)
                {
                    RuntimeState.ActiveMapClass.MapCenterPoint = MapGeoUtilities.ConvertMapPointToMapUnits(RuntimeState.ActiveMapClass.MapCenterPoint);
                }
            }
        }

        /// <summary>
        /// Load the overlays and layers for the map, including the placed features layer, drawn features layer, and selection highlight layer.
        /// </summary>
        private void LoadOverlaysAndLayers()
        {
            MapLayerUtilities.LoadOverlaysAndLayers();
        }

        /// <summary>
        /// Gets or sets the web root path for the application. 
        /// </summary>
        public string WebRootPath { get; set; } = string.Empty;

        /// <summary>
        /// Gets the file path for the map XML file.
        /// </summary>
        /// <returns>The file path to save the map to.</returns>
        public string GetMapFilePath() =>
            Path.Combine(WebRootPath, MapConstants.AppDataDirectoryName, MapConstants.MapFileName);

        /// <summary>
        /// Updates runtime zoom/center state from the current extent and refreshes arrows when zoom changes.
        /// </summary>
        /// <param name="currentExtent">The current map extent.</param>
        /// <param name="mapWidthPixels">Current rendered map width in pixels.</param>
        /// <param name="lastArrowRefreshZoomLevel">The last zoom level used for arrow refresh.</param>
        /// <returns>The zoom level to persist as the new arrow-refresh checkpoint.</returns>
        public async Task<int> UpdateMapViewStateFromExtentAsync(RectangleShape currentExtent, double mapWidthPixels, int lastArrowRefreshZoomLevel)
        {
            if (RuntimeState.ActiveMapView == null || currentExtent == null || mapWidthPixels <= 0d)
            {
                return lastArrowRefreshZoomLevel;
            }

            RuntimeState.MapViewportWidthPixels = mapWidthPixels;

            var currentZoomLevel = RuntimeState.ActiveMapView.ZoomLevelSet.GetScales().IndexOf(
                RuntimeState.ActiveMapView.ZoomLevelSet.GetZoomLevel(currentExtent, mapWidthPixels, GeographyUnit.Meter).Scale) + 1;
            RuntimeState.ActiveMapClass.MapZoomLevel = currentZoomLevel;

            RuntimeState.ActiveMapClass.MapCenterPoint = RuntimeState.ActiveMapView.MapUnit == GeographyUnit.Meter
                ? MapGeoUtilities.ConvertMapPointFromMapUnits(currentExtent.GetCenterPoint())
                : currentExtent.GetCenterPoint();

            if (currentZoomLevel != lastArrowRefreshZoomLevel && RuntimeState.ActiveMapClass.MapDrawnFeatures?.Count > 0)
            {
                await RefreshLineArrowsAsync();
            }

            return currentZoomLevel;
        }
        #endregion

        #region Feature Refresh
        /// <summary>
        /// Replaces the map's drawn features (e.g. threat center circles and airspace polygons)
        /// with <paramref name="drawnFeatures"/> and redraws the features overlay. Call this from a
        /// host component after mutating the underlying data (for example, when the DMO object
        /// manager gains or loses threat centers).
        /// </summary>
        /// <param name="drawnFeatures">The full set of drawn shapes to display on the map, or an
        /// empty collection to clear all drawn shapes.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RefreshDrawnFeaturesAsync(List<MapDrawnFeature>? drawnFeatures)
        {
            // If the map view is not yet initialized, defer to OnAfterRenderAsync which will pick
            // up MapDrawnFeatures on first render.
            if (RuntimeState.ActiveMapView == null)
            {
                // Update the map class with the new drawn features if available.
                RuntimeState.ActiveMapClass.MapDrawnFeatures = new Collection<MapDrawnFeature>(drawnFeatures?.ToList() ?? new List<MapDrawnFeature>());
                return;
            }

            // Update state immediately so whichever debounced call ultimately runs the redraw
            // renders the latest snapshot.
            RuntimeState.ActiveMapClass.MapDrawnFeatures = new Collection<MapDrawnFeature>(drawnFeatures?.ToList() ?? new List<MapDrawnFeature>());

            // Clear and repopulate the drawn features layer in a single open/transaction/close cycle.
            RuntimeState.DrawnFeaturesLayer.Open();

            try
            {
                // Begin a transaction to ensure that all changes to the drawn features layer are applied.
                RuntimeState.DrawnFeaturesLayer.FeatureSource.BeginTransaction();

                // Clear the existing features in the drawn features layer.
                RuntimeState.DrawnFeaturesLayer.InternalFeatures.Clear();

                // Add each drawn feature from the map class to the drawn features layer.
                foreach (var drawnFeature in RuntimeState.ActiveMapClass.MapDrawnFeatures)
                {
                    // Convert the MapDrawnFeature to a MapFeature and add it to the drawn features layer.
                    var feature = ConversionUtilities.ConvertMapDrawnFeatureToMapFeature(drawnFeature);
                    RuntimeState.DrawnFeaturesLayer.FeatureSource.AddFeature(feature);
                }

                // Commit the transaction to apply all changes to the drawn features layer.
                RuntimeState.DrawnFeaturesLayer.EditTools.CommitTransaction();
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of an error.
                RuntimeState.DrawnFeaturesLayer.FeatureSource.RollbackTransaction();

                // Log the error message for debugging purposes.
                System.Diagnostics.Debug.WriteLine($"[Map] RefreshDrawnFeaturesAsync FAILED: {ex.Message}");
            }
            finally
            {
                // Close the drawn features layer.
                RuntimeState.DrawnFeaturesLayer.Close();
            }

            await RefreshLineArrowsAsync(false);

            // If there are any selected features, refresh the selection highlight layer to reflect the current selection.
            if (RuntimeState.SelectedFeatureIds.Count > 0)
            {
                await RefreshSelectionHighlightAsync();
            }

            // Redraw the features overlay to reflect the updated drawn features.
            await RuntimeState.FeaturesOverlay.RedrawAsync();
        }

        /// <summary>
        /// Regenerates only line-arrow features from current drawn lines, without rebuilding the base drawn features.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RefreshLineArrowsAsync(bool refreshOverlay = true)
        {
            // If the map view or the drawn features layer or the arrow features layer is not initialized, return without doing anything.
            if (RuntimeState.ActiveMapView == null || RuntimeState.DrawnFeaturesLayer == null || RuntimeState.ArrowFeaturesLayer == null)
            {
                return;
            }

            // Collect all drawn line features from the drawn features layer, along with their IDs, into a list.
            var lineSources = new List<(MapDrawnFeature Line, string Id)>();

            // Open the drawn features layer to access its internal features.
            RuntimeState.DrawnFeaturesLayer.Open();
            try
            {
                // Iterate through each feature in the drawn features layer's internal features.
                foreach (var sourceFeature in RuntimeState.DrawnFeaturesLayer.InternalFeatures)
                {
                    // Check if the feature has a column value indicating that it is a line feature (IsLineColumn = "1").
                    if (sourceFeature.ColumnValues.TryGetValue(MapConstants.IsLineColumn, out var isLineValue) &&
                        isLineValue == "1")
                    {
                        // Convert the source feature to a MapDrawnFeature and check if it has at least two vertices (indicating a valid line).
                        var lineFeature = ConversionUtilities.ConvertMapFeatureToMapDrawnFeature(sourceFeature);
                        if (lineFeature.LineVertices.Count >= 2)
                        {
                            lineSources.Add((lineFeature, sourceFeature.Id));
                        }
                    }
                }
            }
            finally
            {
                // Close the drawn features layer.
                RuntimeState.DrawnFeaturesLayer.Close();
            }

            var transactionStarted = false;

            //Open the arrow features layer to update its internal features with the newly generated line-arrow features.
            RuntimeState.ArrowFeaturesLayer.Open();

            try
            {
                // Begin a transaction to ensure that all changes to the arrow features layer are applied.
                RuntimeState.ArrowFeaturesLayer.FeatureSource.BeginTransaction();
                transactionStarted = true;

                // Clear the existing features in the arrow features layer.
                RuntimeState.ArrowFeaturesLayer.InternalFeatures.Clear();
                
                // Iterate through each line source and create corresponding arrow features.
                foreach (var lineSource in lineSources)
                {
                    foreach (var arrowFeature in await CreateLineArrowFeaturesAsync(lineSource.Line, lineSource.Id))
                    {
                        RuntimeState.ArrowFeaturesLayer.FeatureSource.AddFeature(arrowFeature);
                    }
                }

                // Commit the transaction to apply all changes to the arrow features layer.
                RuntimeState.ArrowFeaturesLayer.EditTools.CommitTransaction();
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of an error.
                if (transactionStarted)
                {
                    RuntimeState.ArrowFeaturesLayer.FeatureSource.RollbackTransaction();
                }

                // Log the error message for debugging purposes.
                System.Diagnostics.Debug.WriteLine($"[Map] RefreshLineArrowsAsync FAILED: {ex.Message}");
            }
            finally
            {
                // Close the arrow features layer.
                RuntimeState.ArrowFeaturesLayer.Close();
            }

            // If the refreshOverlay parameter is true, redraw the features overlay to reflect the updated arrow features.
            if (refreshOverlay)
            {
                await RuntimeState.FeaturesOverlay.RedrawAsync();
            }
        }

        /// <summary>
        /// Replaces the map's placed features with <paramref name="placedFeatures"/> and redraws
        /// the features overlay. Call this from a host component after mutating the underlying data
        /// (for example, when the DMO object manager gains or loses entries with coordinates).
        /// </summary>
        /// <param name="placedFeatures">The full set of markers to display on the map, or an empty
        /// collection to clear all markers. </param>       
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task RefreshPlacedFeaturesAsync(List<MapPlacedFeature>? placedFeatures)
        {
            // If the map view is not yet initialized, defer to OnAfterRenderAsync which will pick
            // up MapPlacedFeatures on first render.
            if (RuntimeState.ActiveMapView == null)
            {
                // Update the map class with the new placed features if available.
                RuntimeState.ActiveMapClass.MapPlacedFeatures = new Collection<MapPlacedFeature>(placedFeatures?.ToList() ?? new List<MapPlacedFeature>());
                return;
            }

            // Update state immediately so whichever debounced call ultimately runs the redraw
            // renders the latest snapshot.
            RuntimeState.ActiveMapClass.MapPlacedFeatures = new Collection<MapPlacedFeature>(placedFeatures?.ToList() ?? new List<MapPlacedFeature>());

            // Resolve each feature's IconType from its IconName so ValueStyle can pick the right
            // PointStyle in SetPlacedFeaturesLayer. Callers only need to provide IconName.
            foreach (var feature in RuntimeState.ActiveMapClass.MapPlacedFeatures)
            {
                if (string.IsNullOrWhiteSpace(feature.IconType) || feature.IconType == "0")
                {
                    var iconIndex = GeoImageUtilities.GetIconGeoImageIndex(feature.IconName);
                    feature.IconType = iconIndex >= 0 ? iconIndex.ToString() : "0";
                }
            }

            // Clear and repopulate the layer in a single open/transaction/close cycle.
            RuntimeState.PlacedFeaturesLayer.Open();

            try
            {
                // Begin a transaction to ensure that all changes to the placed features layer are applied.
                RuntimeState.PlacedFeaturesLayer.FeatureSource.BeginTransaction();

                // Clear the existing features in the placed features layer.
                RuntimeState.PlacedFeaturesLayer.InternalFeatures.Clear();

                // Add each placed feature from the map class to the placed features layer.
                foreach (var placedFeature in RuntimeState.ActiveMapClass.MapPlacedFeatures)
                {
                    // Convert the MapPlacedFeature to a MapFeature and add it to the placed features layer.
                    var feature = ConversionUtilities.ConvertMapPlacedFeatureToMapFeature(placedFeature);
                    RuntimeState.PlacedFeaturesLayer.FeatureSource.AddFeature(feature);
                }

                // Commit the transaction to apply all changes to the placed features layer.
                RuntimeState.PlacedFeaturesLayer.EditTools.CommitTransaction();
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of an error.
                RuntimeState.PlacedFeaturesLayer.FeatureSource.RollbackTransaction();

                // Log the error message for debugging purposes.
                System.Diagnostics.Debug.WriteLine($"[Map] RefreshPlacedFeaturesAsync FAILED: {ex.Message}");
            }
            finally
            {
                // Close the placed features layer.
                RuntimeState.PlacedFeaturesLayer.Close();
            }

            // If there are any selected features, refresh the selection highlight layer to reflect the current selection.
            if (RuntimeState.SelectedFeatureIds.Count > 0)
            {
                await RefreshSelectionHighlightAsync();
            }

            // Redraw the features overlay to reflect the updated placed features.
            await RuntimeState.FeaturesOverlay.RedrawAsync();
        }

        /// <summary>
        /// Refreshes the selection highlight layer to reflect the current selection.
        /// </summary>
        /// <returns> A task that represents the asynchronous operation.</returns>
        public async Task RefreshSelectionHighlightAsync()
        {
            // Create a hash set of the currently selected feature IDs for efficient lookup.
            var selectedIds = new HashSet<string>(RuntimeState.SelectedFeatureIds, StringComparer.Ordinal);
            var visibleFeatures = new List<Feature>();

            // Open the selection highlight layer.
            RuntimeState.SelectionHighlightLayer.Open();

            try
            {
                // Begin a transaction to update the selection highlight layer.
                RuntimeState.SelectionHighlightLayer.FeatureSource.BeginTransaction();

                // Clear all existing features from the selection highlight layer to prepare for the new selection.
                RuntimeState.SelectionHighlightLayer.InternalFeatures.Clear();

                // If there are any selected feature IDs, add the corresponding features from the placed and drawn
                // feature layers to the selection highlight layer.
                if (selectedIds.Count > 0)
                {
                    // Open the placed features layer to retrieve the selected features.
                    RuntimeState.PlacedFeaturesLayer.Open();

                    try
                    {
                        // Loop through each feature in the placed features layer and check if its ID is in the set of
                        // selected IDs.
                        foreach (var f in RuntimeState.PlacedFeaturesLayer.InternalFeatures)
                        {
                            // If the feature's ID is in the selected IDs and not in the exclusion list, add it to the
                            // selection highlight layer.
                            if (selectedIds.Contains(f.Id) &&
                                !RuntimeState.PlacedFeaturesLayer.FeatureIdsToExclude.Contains(f.Id))
                            {
                                RuntimeState.SelectionHighlightLayer.FeatureSource.AddFeature(new Feature(f.GetShape()) { Id = f.Id });
                                visibleFeatures.Add(f);
                            }
                        }
                    }
                    finally
                    {
                        // Close the placed features layer after processing all features.
                        RuntimeState.PlacedFeaturesLayer.Close();
                    }

                    // Open the drawn features layer to retrieve the selected features.
                    RuntimeState.DrawnFeaturesLayer.Open();

                    try
                    {
                        // Loop through each feature in the drawn features layer and check if its ID is in the set of
                        // selected IDs.
                        foreach (var f in RuntimeState.DrawnFeaturesLayer.InternalFeatures)
                        {

                            // If the feature's ID is in the selected IDs and not in the exclusion list, add it to the
                            // selection highlight layer.
                            if (selectedIds.Contains(f.Id) &&
                                !RuntimeState.DrawnFeaturesLayer.FeatureIdsToExclude.Contains(f.Id))
                            {
                                RuntimeState.SelectionHighlightLayer.FeatureSource.AddFeature(new Feature(f.GetShape()) { Id = f.Id });
                                visibleFeatures.Add(f);
                            }
                        }
                    }
                    finally
                    {
                        // Close the drawn features layer after processing all features.
                        RuntimeState.DrawnFeaturesLayer.Close();
                    }
                }

                // Clear the existing selected feature IDs, names, and features in the settings to prepare for the new
                // selection.
                RuntimeState.SelectedFeatureIds.Clear();
                RuntimeState.SelectedFeatureNames.Clear();
                RuntimeState.SelectedFeatures.Clear();

                // Update the selected feature IDs, names, and features in the settings based on the visible selected
                // features.
                foreach (var f in visibleFeatures)
                {
                    RuntimeState.SelectedFeatureIds.Add(f.Id);
                    var name = f.ColumnValues.TryGetValue(MapConstants.NameColumn, out var value)
                        ? value?.ToString() ?? string.Empty
                        : string.Empty;
                    RuntimeState.SelectedFeatureNames.Add(name);
                    RuntimeState.SelectedFeatures.Add(new MapSelectedFeatures.SelectedFeatureInfo(f.Id, name));
                }


                // Commit the transaction to save the changes to the selection highlight layer.
                RuntimeState.SelectionHighlightLayer.EditTools.CommitTransaction();
            }
            catch (Exception ex)
            {
                // If an error occurs while updating the selection highlight layer, roll back the transaction to revert any changes made.
                RuntimeState.SelectionHighlightLayer.FeatureSource.RollbackTransaction();

                // Log the error message for debugging purposes .
                System.Diagnostics.Debug.WriteLine($"[Map] RefreshSelectionHighlightAsync FAILED: {ex.Message}");
                throw;
            }
            finally
            {
                // Close the selection highlight layer after updating it.
                RuntimeState.SelectionHighlightLayer.Close();
            }

            // Redraw the dynamic overlay to reflect the updated selection highlight layer.
            await RuntimeState.DynamicOverlay.RedrawAsync();
        }
        #endregion

        #region Feature Creation
        /// <summary>
        /// Adds the drawn features from the map to the drawn features layer.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task AddDrawnFeaturesAsync()
        {

            // Open the drawn features layer.
            RuntimeState.DrawnFeaturesLayer.Open();

            try
            {
                // Begin a transaction to add all drawn features to the drawn features layer in a single operation.
                RuntimeState.DrawnFeaturesLayer.FeatureSource.BeginTransaction();

                foreach (var drawnFeature in RuntimeState.ActiveMapClass.MapDrawnFeatures)
                {
                    // Convert the drawn feature to a map feature and add it to the drawn features layer.
                    var feature = ConversionUtilities.ConvertMapDrawnFeatureToMapFeature(drawnFeature);
                    RuntimeState.DrawnFeaturesLayer.FeatureSource.AddFeature(feature);
                }

                // Commit the transaction to save the drawn features to the layer.
                RuntimeState.DrawnFeaturesLayer.EditTools.CommitTransaction();
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of an error.
                RuntimeState.DrawnFeaturesLayer.FeatureSource.RollbackTransaction();

                // Log the error message for debugging purposes.
                System.Diagnostics.Debug.WriteLine($"[Map] AddDrawnFeatures FAILED: {ex.Message}");
            }
            finally
            {
                // Close the drawn features layer.
                RuntimeState.DrawnFeaturesLayer.Close();
            }

            // Refresh the line arrows to ensure that any lines in the drawn features layer have their corresponding arrow 
            // features generated and displayed correctly.
            await RefreshLineArrowsAsync(false);
        }

        /// <summary>
        /// Adds the placed features from the map to the placed features layer.
        /// </summary>
        private void AddPlacedFeatures()
        {
            // Open the placed features layer.
            RuntimeState.PlacedFeaturesLayer.Open();

            try
            {
                // Begin a transaction to add all placed features to the placed features layer in a single operation.
                RuntimeState.PlacedFeaturesLayer.FeatureSource.BeginTransaction();

                foreach (var placedFeature in RuntimeState.ActiveMapClass.MapPlacedFeatures)
                {
                    // Convert the placed feature to a map feature and add it to the placed features layer.
                    var feature = ConversionUtilities.ConvertMapPlacedFeatureToMapFeature(placedFeature);
                    RuntimeState.PlacedFeaturesLayer.FeatureSource.AddFeature(feature);
                }

                // Commit the transaction to save the placed features to the layer.
                RuntimeState.PlacedFeaturesLayer.EditTools.CommitTransaction();
            }
            catch (Exception ex)
            {
                // Rollback the transaction in case of an error.
                RuntimeState.PlacedFeaturesLayer.FeatureSource.RollbackTransaction();

                // Log the error message for debugging purposes.
                System.Diagnostics.Debug.WriteLine($"[Map] AddPlacedFeatures FAILED: {ex.Message}");
            }
            finally
            {
                // Close the placed features layer.
                RuntimeState.PlacedFeaturesLayer.Close();
            }
        }

        /// <summary>
        /// Adds a new feature on the Map.
        /// </summary>
        /// <param name="pointShape">The shape of the point to add as a feature.</param>
        /// <param name="icon">The icon representing the feature.</param>
        /// <param name="iconName">The name of the icon representing the feature.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddPlacedFeature(PointShape pointShape, string icon, string iconName)
        {
            // If the map is in read-only mode, do not allow adding new features.
            if (IsReadOnly)
            {
                return;
            }

            // Create a new feature with the provided point shape. Assign a stable Guid Id so
            // both the rectangle-lookup layer (which pairs rectangles to features by Id) and the
            // HTML label overlay (which uses Id as a @key) can identify it uniquely.
            Feature newFeature = new Feature(pointShape);
            newFeature.Id = Guid.NewGuid().ToString();

            // Generate a default name for the feature based on the icon name.
            var name = GetDefaultPlacedFeatureName(iconName);

            // Open the placed features layer.
            RuntimeState.PlacedFeaturesLayer.Open();

            // Add the necessary column values for the new feature, including the icon type, icon name, and feature name.
            try
            {
                // Begin a transaction to add the new feature to the placed features layer.
                RuntimeState.PlacedFeaturesLayer.FeatureSource.BeginTransaction();

                // Add the new feature to the placed features layer.
                newFeature.ColumnValues.Add(MapConstants.IconTypeColumn, icon);
                newFeature.ColumnValues.Add(MapConstants.IconNameColumn, iconName);
                newFeature.ColumnValues.Add(MapConstants.NameColumn, name);
                RuntimeState.PlacedFeaturesLayer.FeatureSource.AddFeature(newFeature);

                // Commit the transaction to save the new feature to the layer.
                RuntimeState.PlacedFeaturesLayer.EditTools.CommitTransaction();
            }
            catch (Exception ex)
            {
                // If an error occurs while adding the feature, roll back the transaction.
                RuntimeState.PlacedFeaturesLayer.FeatureSource.RollbackTransaction();

                // Log the error message for debugging purposes.
                System.Diagnostics.Debug.WriteLine($"[Map] AddPlacedFeature FAILED: {ex.Message}");
            }
            finally
            {
                // Close the feature source
                RuntimeState.PlacedFeaturesLayer.Close();
            }

            await UpdateLineEndpointsForPlacedFeatureAsync(newFeature);

            // Keep ActiveMapClass in sync with the layer.
            SyncActiveMapClassFromLayers();
        }

        /// <summary>
        /// Updates existing line endpoints when a placed feature is added close to either endpoint.
        /// </summary>
        /// <param name="placedFeature">The feature that was placed on the map.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task UpdateLineEndpointsForPlacedFeatureAsync(Feature placedFeature)
        {
            // If the placed feature is not a point shape, or if the drawn features layer is null or empty, return without doing anything.
            if (placedFeature?.GetShape() is not PointShape placedPoint ||
                RuntimeState.DrawnFeaturesLayer == null ||
                RuntimeState.DrawnFeaturesLayer.InternalFeatures.Count == 0)
            {
                return;
            }

            // Get the endpoint selection radius in world coordinates and calculate its square for distance comparison.
            var endpointSelectionRadiusWorld = await GetEndpointSelectionRadiusWorldAsync();
            var endpointSelectionRadiusSquared = endpointSelectionRadiusWorld * endpointSelectionRadiusWorld;

            // Adjust the placed point from world map coordinates to map coordinates for accurate distance calculations.
            var adjustedPlacedPoint = MapGeoUtilities.AdjustObjectPointFromWorldMap(placedPoint);

            // Compute the snapped line-vertex coordinate once and reuse it for both line endpoints.
            var placedCenterPoint = RuntimeState.ActiveMapView.MapUnit == GeographyUnit.Meter
                ? MapGeoUtilities.ConvertMapPointFromMapUnits(adjustedPlacedPoint)
                : adjustedPlacedPoint;
            var placedCenterCoordinate = new MapCoordinate(placedCenterPoint.Y, placedCenterPoint.X);

            // Create a list to hold the replacement features after updating line endpoints.
            var replacementFeatures = new List<Feature>(RuntimeState.DrawnFeaturesLayer.InternalFeatures.Count);

            // Flag to track if any updates were made to the line endpoints.
            var hasUpdates = false;

            // Flag to track if a transaction has been started for the drawn features layer.
            var transactionStarted = false;

            // Open the drawn features layer to access its internal features for updating line endpoints.
            RuntimeState.DrawnFeaturesLayer.Open();

            try
            {
                // Iterate through each feature in the drawn features layer's internal features.
                foreach (var sourceFeature in RuntimeState.DrawnFeaturesLayer.InternalFeatures)
                {
                    // Check if the feature has a column value indicating that it is a line feature (IsLineColumn = "1").
                    if (sourceFeature.ColumnValues.TryGetValue(MapConstants.IsLineColumn, out var isLineValue) &&
                        isLineValue == "1")
                    {
                        // Convert the source feature to a line feature for updating its endpoints.
                        var lineFeature = ConversionUtilities.ConvertMapFeatureToMapDrawnFeature(sourceFeature);

                        // Check if the line feature has at least two vertices (indicating a valid line).
                        if (lineFeature.LineVertices.Count >= 2)
                        {
                            // Calculate the start and end points of the line feature in map coordinates.
                            var startPoint = MapMeasurementUtilities.ConvertMapCoordinateToMapPoint(lineFeature.LineVertices[0]);
                            var endPoint = MapMeasurementUtilities.ConvertMapCoordinateToMapPoint(lineFeature.LineVertices[^1]);

                            // Check if either the start or end point of the line feature is near the placed feature within the endpoint selection radius.
                            var startIsNearPlacedFeature = MapMeasurementUtilities.GetWrappedDistanceSquared(startPoint, adjustedPlacedPoint) <= endpointSelectionRadiusSquared;
                            var endIsNearPlacedFeature = MapMeasurementUtilities.GetWrappedDistanceSquared(endPoint, adjustedPlacedPoint) <= endpointSelectionRadiusSquared;

                            // If either endpoint is near the placed feature, update the corresponding endpoint to snap to the placed feature's center.
                            if (startIsNearPlacedFeature || endIsNearPlacedFeature)
                            {
                                // If the start point is near the placed feature, update the start vertex to the placed feature's center coordinate.
                                if (startIsNearPlacedFeature)
                                {
                                    lineFeature.LineVertices[0] = placedCenterCoordinate;
                                }

                                // If the end point is near the placed feature, update the end vertex to the placed feature's center coordinate.
                                if (endIsNearPlacedFeature)
                                {
                                    lineFeature.LineVertices[^1] = placedCenterCoordinate;
                                }

                                // Create a new LineShape using the updated line vertices to generate a name for the line feature.
                                var lineShapeForNaming = new LineShape(lineFeature.LineVertices
                                    .Select(v =>
                                    {
                                        // Convert the map coordinate to a map point for accurate placement in the line shape.
                                        var mapPoint = MapMeasurementUtilities.ConvertMapCoordinateToMapPoint(v);
                                        return new Vertex(mapPoint.X, mapPoint.Y);
                                    })
                                    .ToList());

                                // Update the name of the line feature based on the updated line shape and the endpoint selection radius.
                                lineFeature.Name = GetLineFeatureName(lineShapeForNaming, endpointSelectionRadiusSquared);
                                hasUpdates = true;
                            }
                        }

                        // Convert the updated line feature back to a map feature and add it to the replacement features list.
                        var lineFeatureToKeep = ConversionUtilities.ConvertMapDrawnFeatureToMapFeature(lineFeature);
                        lineFeatureToKeep.Id = sourceFeature.Id;

                        // Add the updated line feature to the replacement features list to be saved back to the drawn features layer.
                        replacementFeatures.Add(lineFeatureToKeep);
                        continue;
                    }

                    // If the feature is not a line feature, add it to the replacement features list without any changes.
                    replacementFeatures.Add(sourceFeature);
                }

                // If no updates were made to any line endpoints, return without making any changes to the drawn features layer.
                if (!hasUpdates)
                {
                    return;
                }

                // Begin a transaction to update the drawn features layer with the replacement features.
                RuntimeState.DrawnFeaturesLayer.FeatureSource.BeginTransaction();
                transactionStarted = true;

                // Clear the existing features in the drawn features layer to prepare for the updated features.
                RuntimeState.DrawnFeaturesLayer.InternalFeatures.Clear();

                //  Add each replacement feature to the drawn features layer.
                foreach (var feature in replacementFeatures)
                {
                    RuntimeState.DrawnFeaturesLayer.FeatureSource.AddFeature(feature);
                }

                // Commit the transaction to save the updated features to the drawn features layer.
                RuntimeState.DrawnFeaturesLayer.EditTools.CommitTransaction();
            }
            catch
            {
                // If an error occurs while updating the drawn features layer, roll back the transaction to revert any changes made.
                if (transactionStarted)
                {
                    RuntimeState.DrawnFeaturesLayer.FeatureSource.RollbackTransaction();
                }
                throw;
            }
            finally
            {
                // Close the drawn features layer after updating it.
                RuntimeState.DrawnFeaturesLayer.Close();
            }

            // If any updates were made to the line endpoints, refresh the line arrows to ensure that the corresponding arrow features are generated and displayed correctly.
            if (hasUpdates)
            {
                await RefreshLineArrowsAsync(false);
            }
        }

        /// <summary>
        /// Create a feature using the drawn shape so we own the ColumnValues dictionary
        /// and can safely Add all schema columns. args.DrawnFeature comes from the EditOverlay
        /// and does not have the drawn-features-layer schema columns pre-initialized.
        /// </summary>
        /// <param name="drawnFeature">The feature that was drawn.</param>
        /// <param name="geometryInfo">Information about the geometry of the drawn feature.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DrawFeature(Feature drawnFeature, GeometryInfo geometryInfo)
        {
            // If the map is in read-only mode, do not allow adding new features.
            if (IsReadOnly)
            {
                return;
            }

            // Generate a default name for the drawn feature.
            var featureName = GetDefaultDrawnFeatureName(drawnFeature.GetShape() is LineShape);
            var isLine = false;

            // Check if the drawn feature is a circle.
            if (geometryInfo != null && geometryInfo is CircleGeometryInfo circleInfo)
            {
                // Add the necessary column values for the drawn circle feature, including its center coordinates and radius.
                drawnFeature.ColumnValues.Add(MapConstants.NameColumn, featureName);
                drawnFeature.ColumnValues.Add(MapConstants.LabelNameColumn, featureName);
                drawnFeature.ColumnValues.Add(MapConstants.LineLabelNameColumn, string.Empty);
                drawnFeature.ColumnValues.Add(MapConstants.ArrowNameColumn, string.Empty);
                drawnFeature.ColumnValues.Add(MapConstants.IsLineColumn, "0");
                drawnFeature.ColumnValues.Add(MapConstants.LineVerticesColumn, string.Empty);
                drawnFeature.ColumnValues.Add(MapConstants.GeneratedLineVerticesColumn, string.Empty);
                drawnFeature.ColumnValues.Add(MapConstants.IsCircleColumn, "1");
                drawnFeature.ColumnValues.Add(MapConstants.CircleCenterXColumn, circleInfo.Center.X.ToString());
                drawnFeature.ColumnValues.Add(MapConstants.CircleCenterYColumn, circleInfo.Center.Y.ToString());
                drawnFeature.ColumnValues.Add(MapConstants.CircleRadiusColumn, circleInfo.RadiusInMapUnits.ToString());
                drawnFeature.ColumnValues.Add(MapConstants.PolygonVerticesColumn, string.Empty);
                drawnFeature.ColumnValues.Add(MapConstants.GeneratedPolygonVerticesColumn, string.Empty);
            }

            // If the drawn feature is not a circle, process it as a polygon or line.
            else
            {
                // Store the original ID of the drawn feature to preserve it when creating a new feature.
                var originalId = drawnFeature.Id;

                // Strings to hold the vertices. 
                var verticesString = string.Empty;
                var genVerticesString = string.Empty;

                // Process the polygon.
                if (drawnFeature.GetShape() is PolygonShape polygon)
                {
                    // Get the polygon coordinates from the outer ring vertices of the polygon shape. 
                    var polygonCoordinates = polygon.OuterRing.Vertices
                        .Select(v => new MapCoordinate(v.Y, v.X))
                        .ToList();

                    // If the map unit is meters, convert the coordinates to decimal degrees for consistent processing.
                    if (RuntimeState.ActiveMapView.MapUnit == GeographyUnit.Meter)
                    {
                        // Convert the polygon coordinates to decimal degrees for consistent processing.
                        polygonCoordinates = polygonCoordinates
                            .Select(MapGeoUtilities.ConvertMapCoordinateToDecimalDegrees)
                            .ToList();
                    }

                    // Normalize the polygon coordinates across the date line to ensure proper rendering and calculations.
                    polygonCoordinates = MapGeoUtilities.NormalizeMapCoordinatesAcrossDateLine(polygonCoordinates);

                    // Ensure the polygon is closed by adding the first coordinate to the end of the list if it's not already there.
                    if (polygonCoordinates.Count > 0 && !MapGeoUtilities.CoordinatesEqual(polygonCoordinates[0], polygonCoordinates[^1]))
                    {
                        polygonCoordinates.Add(new MapCoordinate(polygonCoordinates[0].Latitude, polygonCoordinates[0].Longitude));
                    }

                    // Preserve the original polygon vertices as decimal degrees in a string representation for storage in the
                    // drawn features layer.
                    var vertexList = polygonCoordinates.Select(v => $"({v.Longitude},{v.Latitude})");
                    verticesString = string.Join(";", vertexList);

                    // If the map unit is meters and there are at least 3 coordinates, create a new polygon shape using decimal degree vertices.
                    if (RuntimeState.ActiveMapView.MapUnit == GeographyUnit.Meter && polygonCoordinates.Count >= 3)
                    {
                        // Convert the polygon coordinates to decimal degree vertices for creating a new polygon shape.
                        var decimalDegreeVertices = polygonCoordinates
                            .Select(v => new Vertex(v.Longitude, v.Latitude))
                            .ToList();

                        // Create a new polygon shape using the decimal degree vertices.
                        polygon = new PolygonShape(new RingShape(decimalDegreeVertices));
                    }

                    // Split the polygon by the date line to handle cases where the polygon crosses the international date line.
                    var multiPolygon = new MultipolygonShape(new Collection<PolygonShape> { polygon });

                    // Create a list to hold the string representations of the multi-polygon vertices.
                    var multiVertices = new List<string>();

                    // Create a collection to hold the individual polygon shapes that make up the multi-polygon shape.
                    var polygons = new Collection<PolygonShape>();

                    // Create a string representation of the multi-polygon vertices, where each polygon's vertices are separated by
                    // semicolons.
                    foreach (var poly in multiPolygon.Polygons)
                    {
                        if (RuntimeState.ActiveMapView.MapUnit == GeographyUnit.Meter)
                        {
                            // Convert the line vertices to meters for storage in the drawn features layer.
                            var meterOuterRingVertices = polygon.OuterRing.Vertices
                                .Select(v =>
                                {
                                    var m = MapGeoUtilities.ConvertMapCoordinateToMetersPreserveLongitude(
                                        new MapCoordinate(v.Y, v.X));
                                    return new Vertex(m.Longitude, m.Latitude);
                                })
                                .ToList();

                            // Add the line shape with meter vertices to the collection of lines for the multi-line shape.
                            polygons.Add(new PolygonShape(new RingShape(meterOuterRingVertices)));

                            // Add the string representation of the line vertices in meters to the multiVertices list.s
                            multiVertices.Add(string.Join(";", meterOuterRingVertices.Select(v => $"({v.X},{v.Y})")));
                        }

                        // If the map unit is not meters, use the original line vertices.
                        else
                        {
                            multiVertices.Add(string.Join(";", poly.OuterRing.Vertices.Select(v => $"({v.X},{v.Y})")));
                        }
                    }

                    // Join the multi-polygon vertices with a pipe character to create a single string representation.
                    genVerticesString = string.Join("|", multiVertices);

                    // Create a new feature with the multi-polygon shape, preserving the original ID of the drawn feature.
                    drawnFeature = new Feature(RuntimeState.ActiveMapView.MapUnit == GeographyUnit.Meter ? new MultipolygonShape(polygons) : multiPolygon)
                    {
                        Id = originalId
                    };
                }

                // Process the line.
                else if (drawnFeature.GetShape() is LineShape line)
                {
                    // Set isLine to true since the drawn feature is a line.
                    isLine = true;

                    // Use selection-style distance tolerance (pixel-based in current view) for endpoint snap/name behavior.
                    var endpointSelectionRadiusWorld = await GetEndpointSelectionRadiusWorldAsync();
                    var endpointSelectionRadiusSquared = endpointSelectionRadiusWorld * endpointSelectionRadiusWorld;

                    // Snap each line endpoint to the nearest placed-feature center only when within selection-style distance.
                    var snappedLine = await SnapLineEndpointsToNearbyPlacedFeaturesAsync(line, endpointSelectionRadiusSquared);

                    // Name the line only from endpoint objects that are within selection-style distance.
                    featureName = GetLineFeatureName(snappedLine, endpointSelectionRadiusSquared);

                    // Convert the captured line vertices to decimal degrees so we can densify along
                    // a great-circle path that follows earth curvature.
                    var lineCoordinates = snappedLine.Vertices
                        .Select(v => new MapCoordinate(v.Y, v.X))
                        .ToList();

                    // If the map unit is meters, convert the line coordinates to decimal degrees for consistent processing.
                    if (RuntimeState.ActiveMapView.MapUnit == GeographyUnit.Meter)
                    {
                        lineCoordinates = lineCoordinates
                            .Select(MapGeoUtilities.ConvertMapCoordinateToDecimalDegrees)
                            .ToList();
                    }

                    // Preserve the original line vertices as decimal degrees in a string representation for storage in the
                    // drawn features layer.
                    var vertexList = lineCoordinates.Select(v => $"({v.Longitude},{v.Latitude})");
                    verticesString = string.Join(";", vertexList);

                    // Densify the line coordinates along a great-circle path to create a curved line that follows the earth's
                    // curvature.
                    var curvedCoordinates = MapMeasurementUtilities.DensifyGreatCircle(lineCoordinates);

                    // Convert the curved points back to the current map unit for rendering/storage.
                    var curvedVertices = new List<Vertex>(curvedCoordinates.Count);
                    foreach (var curvedCoordinate in curvedCoordinates)
                    {
                        curvedVertices.Add(new Vertex(curvedCoordinate.Longitude, curvedCoordinate.Latitude));
                    }

                    // Split the curved line by the date line to handle cases where the line crosses the international date line.
                    var multiLine = DecimalDegreesHelper.SplitByDateline(new LineShape(curvedVertices));

                    // Create a list to hold the string representations of the multi-line vertices.
                    var multiVertices = new List<string>();

                    // Create a collection to hold the individual line shapes that make up the multi-line shape.
                    var lines = new Collection<LineShape>();

                    // Create a string representation of the multi-line vertices, where each line's vertices are separated by semicolons.
                    foreach (var lin in multiLine.Lines)
                    {
                        if (RuntimeState.ActiveMapView.MapUnit == GeographyUnit.Meter)
                        {
                            // Convert the line vertices to meters for storage in the drawn features layer.
                            var meterLineVertices = lin.Vertices
                                .Select(v =>
                                {
                                    var m = MapGeoUtilities.ConvertMapCoordinateToMetersPreserveLongitude(
                                        new MapCoordinate(v.Y, v.X));
                                    return new Vertex(m.Longitude, m.Latitude);
                                })
                                .ToList();

                            // Add the line shape with meter vertices to the collection of lines for the multi-line shape.
                            lines.Add(new LineShape(meterLineVertices));

                            // Add the string representation of the line vertices in meters to the multiVertices list.s
                            multiVertices.Add(string.Join(";", meterLineVertices.Select(v => $"({v.X},{v.Y})")));
                        }

                        // If the map unit is not meters, use the original line vertices.
                        else
                        {
                            // Add the line shape with original vertices to the collection of lines for the multi-line shape.
                            multiVertices.Add(string.Join(";", lin.Vertices.Select(v => $"({v.X},{v.Y})")));
                        }
                    }

                    // Join the multi-line vertices with a pipe character to create a single string representation.s
                    genVerticesString = string.Join("|", multiVertices);

                    // Create a new feature with the multi-line shape, preserving the original ID of the drawn feature.
                    drawnFeature = new Feature(RuntimeState.ActiveMapView.MapUnit == GeographyUnit.Meter ? new MultilineShape(lines) : multiLine)
                    {
                        Id = originalId
                    };
                }

                // Add the necessary column values for the drawn feature.
                drawnFeature.ColumnValues.Add(MapConstants.NameColumn, featureName);
                drawnFeature.ColumnValues.Add(MapConstants.LabelNameColumn, isLine ? string.Empty : featureName);
                drawnFeature.ColumnValues.Add(MapConstants.LineLabelNameColumn, isLine ? featureName : string.Empty);
                drawnFeature.ColumnValues.Add(MapConstants.ArrowNameColumn, string.Empty);
                drawnFeature.ColumnValues.Add(MapConstants.IsLineColumn, isLine ? "1" : "0");
                drawnFeature.ColumnValues.Add(MapConstants.LineVerticesColumn, isLine ? verticesString : string.Empty);
                drawnFeature.ColumnValues.Add(MapConstants.GeneratedLineVerticesColumn, isLine ? genVerticesString : string.Empty);
                drawnFeature.ColumnValues.Add(MapConstants.IsCircleColumn, "0");
                drawnFeature.ColumnValues.Add(MapConstants.CircleCenterXColumn, string.Empty);
                drawnFeature.ColumnValues.Add(MapConstants.CircleCenterYColumn, string.Empty);
                drawnFeature.ColumnValues.Add(MapConstants.CircleRadiusColumn, string.Empty);
                drawnFeature.ColumnValues.Add(MapConstants.PolygonVerticesColumn, isLine ? string.Empty : verticesString);
                drawnFeature.ColumnValues.Add(MapConstants.GeneratedPolygonVerticesColumn, isLine ? string.Empty : genVerticesString);
            }

            // Commit through FeatureSource so QueryTools can locate the feature immediately.
            RuntimeState.DrawnFeaturesLayer.Open();

            try
            {
                // Begin a transaction to add the drawn feature to the drawn features layer.
                RuntimeState.DrawnFeaturesLayer.FeatureSource.BeginTransaction();

                // Add the drawn feature to the drawn features layer.
                RuntimeState.DrawnFeaturesLayer.FeatureSource.AddFeature(drawnFeature);

                // Commit the transaction to save the drawn feature to the layer.
                RuntimeState.DrawnFeaturesLayer.EditTools.CommitTransaction();
            }
            catch (Exception ex)
            {
                // If an error occurs while adding the feature, roll back the transaction.
                RuntimeState.DrawnFeaturesLayer.FeatureSource.RollbackTransaction();

                // Log the error message for debugging purposes.
                System.Diagnostics.Debug.WriteLine($"[Map] OnFeatureDrawn FAILED: {ex.Message}");
            }
            finally
            {
                // Close the drawn features layer.
                RuntimeState.DrawnFeaturesLayer.Close();
            }

            // If the drawn feature is a line, refresh the line arrows to ensure that the corresponding arrow features
            // are generated and displayed correctly.
            if (isLine)
            {
                await RefreshLineArrowsAsync(false);
            }

            // Render the newly committed feature before touching the EditOverlay.
            await RuntimeState.FeaturesOverlay.RedrawAsync();

            // Clear the EditOverlay ghost shape and redraw it.
            RuntimeState.MapEditOverlay.Features.Clear();

            // Redraw the EditOverlay to reflect the cleared ghost shape.
            await RuntimeState.MapEditOverlay.RedrawAsync();

            // Keep ActiveMapClass in sync with the layer.
            SyncActiveMapClassFromLayers();
        }

        /// <summary>
        /// Gets the default name for a new feature based on the existing features count.
        /// </summary>
        /// <param name="isLine">Indicates whether the feature is a line.</param>
        /// <returns>Returns a name for the drawn feature.</returns>
        public string GetDefaultDrawnFeatureName(bool isLine)
        {
            // Count the existing features in the Drawn Features layer.
            int existingLayerFeatureCount = 0;
            if (RuntimeState.FeaturesOverlay.Layers.Count > 0 && RuntimeState.FeaturesOverlay.Layers[MapConstants.DrawnFeaturesName] is InMemoryFeatureLayer drawnLayer)
            {
                // If there are features in the drawn features layer, set the existing layer feature count to that count.
                existingLayerFeatureCount = drawnLayer.InternalFeatures.Count;
            }

            // Count the features currently in the Edit Overlay.
            int editOverlayFeatureCount = RuntimeState.ActiveMapView?.EditOverlay?.Features?.Count ?? 0;

            // Generate a default name using the total count of features from both the Drawn Features layer and the
            // Edit Overlay.
            return $"{(isLine ? "Line" : "Shape")} {existingLayerFeatureCount + editOverlayFeatureCount}";
        }

        /// <summary>
        /// Gets a route line name using nearest placed features at endpoints that fall within the provided distance.
        /// </summary>
        /// <param name="line">The line shape used to build the route name.</param>
        /// <param name="maxDistanceSquared">Maximum endpoint-to-feature distance squared in world units.</param>
        /// <returns>The route name, or the default drawn feature name if endpoint matches are unavailable.</returns>
        public string GetLineFeatureName(LineShape line, double maxDistanceSquared)
        {
            // If the line is null or has fewer than 2 vertices, return the default drawn feature name.
            if (line == null || line.Vertices.Count < 2)
            {
                return GetDefaultDrawnFeatureName(true);
            }

            // Create PointShape objects for the start and end points of the line using the first and last vertices.
            var startPoint = new PointShape(line.Vertices[0]);
            var endPoint = new PointShape(line.Vertices[^1]);

            // Try to get the closest placed feature names for the start and end points within the specified maximum distance squared.
            var hasStart = TryGetClosestPlacedFeatureNameWithinDistance(startPoint, maxDistanceSquared, out var startName);
            var hasEnd = TryGetClosestPlacedFeatureNameWithinDistance(endPoint, maxDistanceSquared, out var endName);

            // If both start and end names are found and are not null or whitespace, return the combined name; otherwise, return the default
            // drawn feature name.
            return hasStart && hasEnd &&
                   !string.IsNullOrWhiteSpace(startName) &&
                   !string.IsNullOrWhiteSpace(endName)
                ? $"{startName} to {endName}"
                : GetDefaultDrawnFeatureName(true);
        }

        /// <summary>
        /// Computes endpoint snap radius in world units using the placed-feature hitbox radius in pixels.
        /// </summary>
        /// <returns>The endpoint snap radius in world units.</returns>
        private async Task<double> GetEndpointSelectionRadiusWorldAsync()
        {
            // If there is no active map view, return 0 as the endpoint selection radius.
            if (RuntimeState.ActiveMapView == null)
            {
                return 0d;
            }

            // Get the current map viewport width in pixels from the settings.
            var mapWidthPx = RuntimeState.MapViewportWidthPixels;
            if (mapWidthPx <= 0d)
            {
                return 0d;
            }

            // Get the current map extent from the active map view.
            var currentExtent = await RuntimeState.ActiveMapView.GetCurrentExtentAsync();
            if (currentExtent == null || currentExtent.Width <= 0d)
            {
                return 0d;
            }

            // Calculate the world units per pixel based on the current extent width and the map viewport width in pixels.
            var worldUnitsPerPixel = currentExtent.Width / mapWidthPx;

            // Calculate the endpoint snap radius in pixels based on half of the placed-feature hitbox size in pixels.
            var endpointSnapPixels = MapConstants.PlacedFeatureBoundingBoxPixels / 2d;

            // Return the endpoint snap radius in world units by multiplying the endpoint snap pixels by the world units per pixel.
            return endpointSnapPixels * worldUnitsPerPixel;
        }

        /// <summary>
        /// Snaps a line's start and end vertices to the nearest placed-feature hitbox edge only when within max distance.
        /// </summary>
        /// <param name="line">The line shape to snap.</param>  
        /// <param name="maxDistanceSquared">The maximum distance squared for snapping in world units.</param>
        /// <returns>The snapped line shape.</returns>
        private async Task<LineShape> SnapLineEndpointsToNearbyPlacedFeaturesAsync(LineShape line, double maxDistanceSquared)
        {
            //  If the line is null, return a new empty LineShape.
            if (line == null)
            {
                return new LineShape();
            }

            // If the line has fewer than 2 vertices, return the original line without any snapping.
            if (line.Vertices.Count < 2)
            {
                return line;
            }

            // Add the line vertices to a list of Vertex objects for processing.
            var vertices = line.Vertices.Select(v => new Vertex(v.X, v.Y)).ToList();

            // Get the nearest placed-feature center for the start vertex within the specified max distance squared.
            if (TryGetNearestPlacedFeatureCenterWithinDistance(
                new PointShape(vertices[0]),
                maxDistanceSquared,
                out var startCenterPoint))
            {
                // If a nearest placed-feature center is found, update the start vertex to snap to that center point.
                vertices[0] = new Vertex(startCenterPoint.X, startCenterPoint.Y);
            }

            // Get the nearest placed-feature center for the end vertex within the specified max distance squared.
            var lastIndex = vertices.Count - 1;
            if (TryGetNearestPlacedFeatureCenterWithinDistance(
                new PointShape(vertices[lastIndex]),
                maxDistanceSquared,
                out var endCenterPoint))
            {
                vertices[lastIndex] = new Vertex(endCenterPoint.X, endCenterPoint.Y);
            }

            // Return a new LineShape with the potentially snapped vertices.
            return new LineShape(vertices);
        }

        /// <summary>
        /// Computes half of the placed-feature hitbox size in world units.
        /// </summary>
        /// <returns>The half size of the placed-feature hitbox in world units.</returns>
        private async Task<double> GetPlacedFeatureHalfSizeWorldAsync()
        {
            // If there is no active map view, return half of the placed-feature hitbox size in pixels as the default value.
            if (RuntimeState.ActiveMapView == null)
            {
                return MapConstants.PlacedFeatureBoundingBoxPixels / 2d;
            }

            // Get the current map viewport width in pixels from the settings.
            var mapWidthPx = RuntimeState.MapViewportWidthPixels;
            if (mapWidthPx <= 0d)
            {
                return MapConstants.PlacedFeatureBoundingBoxPixels / 2d;
            }

            // Get the current map extent from the active map view.
            var currentExtent = await RuntimeState.ActiveMapView.GetCurrentExtentAsync();
            if (currentExtent == null || currentExtent.Width <= 0d)
            {
                return MapConstants.PlacedFeatureBoundingBoxPixels / 2d;
            }

            // Calculate the world units per pixel based on the current extent width and the map viewport width in pixels.
            var worldUnitsPerPixel = currentExtent.Width / mapWidthPx;

            // Return half of the placed-feature hitbox size in world units by multiplying the half size in pixels by the world units per pixel.
            return (MapConstants.PlacedFeatureBoundingBoxPixels * worldUnitsPerPixel) / 2d;
        }

        /// <summary>
        /// Tries to resolve the nearest placed-feature center within the specified max distance.
        /// </summary>
        /// <param name="maxDistanceSquared">The maximum distance squared for snapping in world units.</param>  
        /// <param name="point" >The point to compare against placed feature locations.</param>
        /// <param name="snappedPoint">The nearest placed-feature center point if found; otherwise, the original point.</param>
        /// <returns>True if a nearest placed-feature center is found within the max distance; otherwise, false.</returns>
        private bool TryGetNearestPlacedFeatureCenterWithinDistance(PointShape point, double maxDistanceSquared, out PointShape snappedPoint)
        {
            // Initialize the snappedPoint to the original point in case no nearest placed-feature center is found.
            snappedPoint = point;

            // If the point is null, or if there are no layers in the FeaturesOverlay, or if the PlacedFeatures layer is not found or has no
            // features, return false.
            if (!TryGetClosestPlacedFeatureWithinDistance(point, maxDistanceSquared, out var _, out var nearestCenter))
            {
                return false;
            }

            // If a nearest placed-feature center is found within the max distance, update the snappedPoint to that center point and return true.
            snappedPoint = nearestCenter;
            return true;
        }

        /// <summary>
        /// Tries to resolve the nearest placed-feature name within the specified max distance.
        /// </summary>
        /// <param name="point">The point to compare against placed feature locations.</param>
        /// <param name="maxDistanceSquared">The maximum distance squared for snapping in world units.</param>  
        /// <param name="name"  >The nearest placed-feature name if found; otherwise, an empty string.</param>
        /// <returns>True if a nearest placed-feature name is found within the max distance; otherwise, false.</returns>    
        private bool TryGetClosestPlacedFeatureNameWithinDistance(PointShape point, double maxDistanceSquared, out string name)
        {
            // Initialize the name to an empty string in case no nearest placed-feature name is found.
            name = string.Empty;

            // If the point is null, or if there are no layers in the FeaturesOverlay, or if the PlacedFeatures layer is not found or has no
            // features, return false.
            if (!TryGetClosestPlacedFeatureWithinDistance(point, maxDistanceSquared, out var feature, out var _))
            {
                return false;
            }

            // If a nearest placed-feature is found, attempt to retrieve its name from the feature's column values.
            if (feature != null &&
                feature.ColumnValues.TryGetValue(MapConstants.NameColumn, out var nameValue) &&
                !string.IsNullOrWhiteSpace(nameValue?.ToString()))
            {
                name = nameValue.ToString()!;
                return true;
            }

            // If no valid name is found, return false.
            return false;
        }

        /// <summary>
        /// Finds the nearest placed feature to a point, but only returns it when within max distance.
        /// </summary>
        /// <param name="point">The point to compare against placed feature locations.</param>
        /// <param name="maxDistanceSquared">The maximum distance squared for snapping in world units.</param>
        /// <param name="feature">The nearest placed feature if found; otherwise, null.</param>
        /// <param name="center">The center point of the nearest placed feature if found; otherwise, the provided point.</param>
        /// <returns>True if a nearest placed feature is found within the max distance; otherwise, false.</returns>
        private bool TryGetClosestPlacedFeatureWithinDistance(PointShape point, double maxDistanceSquared, out Feature? feature, out PointShape center)
        {
            // Initialize the output parameters to default values.
            feature = null;
            center = point ?? new PointShape();

            // If the point is null, or if there are no layers in the FeaturesOverlay, or if the PlacedFeatures layer is not found or has no
            // features, return false.
            if (point == null ||
                RuntimeState.FeaturesOverlay.Layers.Count == 0 ||
                RuntimeState.FeaturesOverlay.Layers[MapConstants.PlacedFeaturesName] is not InMemoryFeatureLayer placedLayer ||
                placedLayer.InternalFeatures.Count == 0)
            {
                return false;
            }

            // Adjust the point from world map coordinates to account for wrapping and other transformations.
            var adjustedPoint = MapGeoUtilities.AdjustObjectPointFromWorldMap(point);

            // Initialize variables to track the nearest distance squared, the nearest center point, and the nearest feature found.
            var nearestDistanceSquared = double.MaxValue;
            PointShape? nearestCenter = null;
            Feature? nearestFeature = null;

            // Iterate through each placed feature in the PlacedFeatures layer to find the nearest feature to the provided point.
            foreach (var placedFeature in placedLayer.InternalFeatures)
            {
                // Adjust the center point of the placed feature from world map coordinates to account for wrapping and other transformations.
                var candidateCenter = MapGeoUtilities.AdjustObjectPointFromWorldMap(placedFeature.GetShape().GetCenterPoint());

                // Calculate the squared distance between the adjusted point and the candidate center point, accounting for wrapping.
                var distanceSquared = MapMeasurementUtilities.GetWrappedDistanceSquared(adjustedPoint, candidateCenter);

                // Update the nearest feature if the current candidate is closer.
                if (distanceSquared < nearestDistanceSquared)
                {
                    // Update the nearest distance squared, nearest center point, and nearest feature to the current candidate.
                    nearestDistanceSquared = distanceSquared;
                    nearestCenter = candidateCenter;
                    nearestFeature = placedFeature;
                }
            }

            // If no nearest feature is found, or if the nearest distance squared exceeds the maximum distance squared, return false.
            if (nearestFeature == null || nearestCenter == null || nearestDistanceSquared > maxDistanceSquared)
            {
                return false;
            }

            // If a nearest feature is found within the max distance, update the output parameters and return true.
            feature = nearestFeature;
            center = nearestCenter;
            return true;
        }

        /// <summary>
        /// Finds the feature at the given world point, checking placed features first and then drawn features.
        /// </summary>
        /// <param name="worldPoint">The world point to hit-test against map features.</param>
        /// <param name="selectionTolerancePixels">Selection tolerance in pixels.</param>
        /// <returns>The matched feature, or null when no feature is within tolerance.</returns>
        public async Task<Feature?> FindFeatureAtPointAsync(PointShape worldPoint, double selectionTolerancePixels)
        {
            // If there is no active map view or the world point is null, return null as no feature can be found.
            if (RuntimeState.ActiveMapView == null || worldPoint == null)
            {
                return null;
            }

            // Get the current map extent from the active map view.
            var currentExtent = await RuntimeState.ActiveMapView.GetCurrentExtentAsync();
            if (currentExtent == null || currentExtent.Width <= 0d)
            {
                return null;
            }

            // Get the current map viewport width in pixels from the settings.
            var mapWidthPixels = RuntimeState.MapViewportWidthPixels;

            // Calculate the world units per pixel based on the current extent width and the map viewport width in pixels.
            var worldUnitsPerPixel = mapWidthPixels > 0d
                ? currentExtent.Width / mapWidthPixels
                : 1.0;

            // Calculate the selection radius in world units based on the selection tolerance in pixels and the world
            // units per pixel.
            var selectionRadiusWorld = selectionTolerancePixels * worldUnitsPerPixel;

            // Calculate the selection radius squared for distance comparisons.
            var selectionRadiusSquared = selectionRadiusWorld * selectionRadiusWorld;

            // Check if there are any placed features in the PlacedFeatures layer.
            if (RuntimeState.PlacedFeaturesLayer.InternalFeatures.Count > 0)
            {
                // Open the PlacedFeatures layer.
                RuntimeState.PlacedFeaturesLayer.Open();
                try
                {
                    // First, check for features that contain the world point.
                    var nearest = RuntimeState.PlacedFeaturesLayer.QueryTools.GetFeaturesNearestTo(
                        worldPoint,
                        GeographyUnit.Meter,
                        1,
                        ReturningColumnsType.AllColumns);

                    // If a nearest feature is found and its shape is a PointShape, check if the distance to the world
                    // point is within the selection radius.
                    if (nearest.Count > 0 && nearest[0].GetShape() is PointShape featurePoint)
                    {
                        var dx = worldPoint.X - featurePoint.X;
                        var dy = worldPoint.Y - featurePoint.Y;
                        if ((dx * dx) + (dy * dy) <= selectionRadiusSquared)
                        {
                            return nearest[0];
                        }
                    }
                }
                finally
                {
                    // Close the PlacedFeatures layer.
                    RuntimeState.PlacedFeaturesLayer.Close();
                }
            }

            // Check if there are any drawn features in the DrawnFeatures layer.
            if (RuntimeState.DrawnFeaturesLayer.InternalFeatures.Count > 0)
            {
                // Open the DrawnFeatures layer.
                RuntimeState.DrawnFeaturesLayer.Open();
                try
                {
                    // First, check for features that contain the world point.
                    var featuresFound = RuntimeState.DrawnFeaturesLayer.QueryTools.GetFeaturesContaining(
                        worldPoint,
                        ReturningColumnsType.AllColumns);

                    // If any features are found that contain the world point, return the first one.
                    if (featuresFound.Count > 0)
                    {
                        return featuresFound[0];
                    }

                    // If no features contain the world point, check for the nearest feature to the world point.
                    var nearest = RuntimeState.DrawnFeaturesLayer.QueryTools.GetFeaturesNearestTo(
                        worldPoint,
                        GeographyUnit.Meter,
                        1,
                        ReturningColumnsType.AllColumns);

                    // If a nearest feature is found, create an ellipse shape around the world point with the selection
                    // radius and check if it intersects with the nearest feature's shape.
                    if (nearest.Count > 0)
                    {
                        var hitShape = new EllipseShape(worldPoint, selectionRadiusWorld);
                        if (hitShape.Intersects(nearest[0].GetShape()))
                        {
                            return nearest[0];
                        }
                    }
                }
                finally
                {
                    // Close the DrawnFeatures layer.
                    RuntimeState.DrawnFeaturesLayer.Close();
                }
            }

            // If no feature is found at the world point, return null.
            return null;
        }

        /// <summary>
        /// Gets the default name for a new feature based on the existing features count.
        /// </summary>
        /// <returns>A name for the placed feature.</returns>
        public string GetDefaultPlacedFeatureName(string iconName)
        {
            // If the icon name is invalid, use a generic name for the feature.
            if (string.IsNullOrWhiteSpace(iconName))
            {
                iconName = "Generic Object";
            }

            // Count the existing features in the Placed Features layer.
            int existingLayerFeatureCount = 0;

            // If there are features in the placed features layer, set the existing layer feature count to the count of features with the same icon name.
            if (RuntimeState.FeaturesOverlay.Layers.Count > 0 && RuntimeState.FeaturesOverlay.Layers[MapConstants.PlacedFeaturesName] is InMemoryFeatureLayer placedLayer)
            {
                // Count the number of features in the placed features layer that have the same icon name as the new feature.
                existingLayerFeatureCount = placedLayer.InternalFeatures.Count(f => f.ColumnValues[MapConstants.IconNameColumn].ToString() == iconName);
            }

            // Generate a default name for the feature based on the icon name and the existing feature count.
            return $"{iconName}-{existingLayerFeatureCount + 1}";
        }
        #endregion

        #region Feature Deletion
        /// <summary>
        /// Deletes features with the specified IDs from both drawn and placed feature layers and redraws the features overlay.
        /// </summary>
        /// <param name="featureIds">The list of feature IDs to delete.</param>
        /// <param name="refreshOverlay">Indicates whether to refresh the overlay after deleting the features.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeleteFeaturesByIdAsync(List<string> featureIds, bool refreshOverlay)
        {
            // This is the orchestration entry point for deletion:
            // it applies deletions across all in-memory layers, then re-syncs derived state
            // (selection highlight + ActiveMapClass collections) so UI and persisted model remain aligned.
            if (IsReadOnly)
            {
                return;
            }

            // If the list of IDs is null or empty, return early as there are no features to delete.
            if (featureIds == null || featureIds.Count == 0)
            {
                return;
            }

            // Delete the features with the specified IDs in all feature layers.
            foreach (var layer in RuntimeState.FeaturesOverlay.Layers)
            {
                // Check if the layer is an InMemoryFeatureLayer before attempting to delete features.
                if (layer is InMemoryFeatureLayer featureLayer)
                {
                    await DeleteFeaturesByIdAsync(featureIds, featureLayer);
                }
            }

            // Keep selection highlight in sync.
            await RefreshSelectionHighlightAsync();

            // Redraw the features overlay to reflect the deletions if requested.
            if (refreshOverlay)
            {
                await RuntimeState.FeaturesOverlay.RedrawAsync();
            }

            // Keep ActiveMapClass in sync with the layer.
            SyncActiveMapClassFromLayers();
        }

        /// <summary>
        /// Deletes features with the specified IDs from a specific feature layer.
        /// </summary>
        /// <param name="featureIds">The list of feature IDs to delete.</param>
        /// <param name="featureLayer">The feature layer to delete from.</param>
        /// <param name="refreshOverlay">Indicates whether to refresh the overlay after deleting the features.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task DeleteFeaturesByIdAsync(List<string> featureIds, InMemoryFeatureLayer featureLayer)
        {
            // If the list of IDs is null or empty, return early as there are no features to delete.
            if (featureIds == null || featureIds.Count == 0)
            {
                return;
            }

            // Open the feature layer for deletion.
            featureLayer.Open();

            try
            {
                // Begin a transaction to ensure that all changes to the placed features layer are applied.
                featureLayer.FeatureSource.BeginTransaction();

                // Perform deletions inside a single transaction so partial layer updates are avoided
                // if any delete operation fails mid-batch.
                // Loop through each ID in the provided list of IDs.
                foreach (var id in featureIds)
                {
                    // If the ID is not null or empty, attempt to delete the feature.
                    if (!string.IsNullOrEmpty(id))
                    {
                        // Try to remove the feature from the internal features collection.
                        var featureToDelete = featureLayer.InternalFeatures.FirstOrDefault(f => f.Id == id);
                        if (featureToDelete != null)
                        {
                            // If the feature is currently selected, remove it from the selected feature IDs collection.
                            if (RuntimeState.SelectedFeatureIds.Contains(id))
                            {
                                RuntimeState.SelectedFeatureIds.Remove(id);
                            }

                            // Delete the feature from the feature source using its ID.
                            featureLayer.FeatureSource.DeleteFeature(featureToDelete.Id);
                        }
                    }
                }

                // Commit the transaction to save the deletions.
                featureLayer.FeatureSource.CommitTransaction();
            }
            catch (Exception ex)
            {
                // If an error occurs while deleting the features, roll back the transaction.
                featureLayer.FeatureSource.RollbackTransaction();

                // Log the error message for debugging purposes.
                System.Diagnostics.Debug.WriteLine($"[Map] DeleteFeaturesByIdAsync FAILED: {ex.Message}");
            }
            finally
            {
                // Close the feature layer.
                featureLayer.Close();
            }
        }

        /// <summary>
        /// Deletes features with the specified names from both drawn and placed feature layers and redraws the features overlay.
        /// </summary>
        /// <param name="featureNames">The list of feature names to delete.</param>
        /// <param name="refreshOverlay">Indicates whether to refresh the overlay after deleting the features.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DeleteFeaturesByNameAsync(List<string> featureNames, bool refreshOverlay = true)
        {
            // This is the orchestration entry point for deletion:
            if (IsReadOnly)
            {
                return;
            }

            // If the list of names is null or empty, return early as there are no features to delete.
            if (featureNames == null || featureNames.Count == 0)
            {
                return;
            }

            // Delete the features with the specified names in all feature layers.
            foreach (var layer in RuntimeState.FeaturesOverlay.Layers)
            {
                // Check if the layer is an InMemoryFeatureLayer before attempting to delete features.
                if (layer is InMemoryFeatureLayer featureLayer)
                {
                    // Retrieve the feature IDs corresponding to the provided names.
                    List<string> featureIds = GetFeatureIdsFromNames(featureNames, featureLayer);

                    // Delete the features with the retrieved IDs.
                    await DeleteFeaturesByIdAsync(featureIds, featureLayer);
                }
            }

            // Keep selection highlight in sync.
            await RefreshSelectionHighlightAsync();

            // Redraw the features overlay to reflect the deletions if requested.
            if (refreshOverlay)
            {
                await RuntimeState.FeaturesOverlay.RedrawAsync();
            }

            // Keep ActiveMapClass in sync with the layer.
            SyncActiveMapClassFromLayers();
        }
        #endregion

        #region Feature Visibility Management
        /// <summary>
        /// Changes the visibility of all drawn and placed features on the map based on the provided visibility flag.
        /// </summary>
        /// <param name="visible">A boolean value indicating whether to show (true) or hide (false) all features.</param>
        /// <param name="refreshOverlay">Indicates whether to refresh the overlay after changing the visibility.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ChangeAllFeaturesVisibility(bool visible, bool refreshOverlay = true)
        {
            //  If the visibility flag is true, show all features; otherwise, hide all features.
            await ApplyVisibilityChangeAsync(
                () => visible ? ShowAllFeaturesAsync() : HideAllFeaturesAsync(),
                refreshOverlay);
        }

        /// <summary>
        /// Changes the visibility of features with the specified IDs based on the provided visibility flag and redraws the features overlay.
        /// </summary>
        /// <param name="visible">A boolean value indicating whether to show (true) or hide (false) the features.</param>
        /// <param name="featureIds">The list of feature IDs to update.</param>
        /// <param name="refreshOverlay">Indicates whether to refresh the overlay after changing the visibility.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ChangeFeaturesVisibilityByIdAsync(bool visible, List<string> featureIds, bool refreshOverlay)
        {
            //  If the list of IDs is null or empty, return early as there are no features to update.
            if (featureIds == null || featureIds.Count == 0)
            {
                return;
            }

            //  Apply the visibility change for the specified feature IDs based on the visibility flag.
            await ApplyVisibilityChangeAsync(
                () => visible ? ShowFeaturesByIdAsync(featureIds) : HideFeaturesByIdAsync(featureIds),
                refreshOverlay);
        }

        /// <summary>
        /// Changes the visibility of features with the specified names based on the provided visibility flag and redraws the features overlay. 
        /// </summary>
        /// <param name="visible">A boolean value indicating whether to show (true) or hide (false) the features.</param>
        /// <param name="featureNames">The list of feature names to update.</param>
        /// <param name="refreshOverlay">Indicates whether to refresh the overlay after changing the visibility.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ChangeFeaturesVisibilityByNameAsync(bool visible, List<string> featureNames, bool refreshOverlay = true)
        {
            //  If the list of names is null or empty, return early as there are no features to update.
            if (featureNames == null || featureNames.Count == 0)
            {
                return;
            }

            //  Apply the visibility change for the specified feature names based on the visibility flag.
            await ApplyVisibilityChangeAsync(
                () => visible ? ShowFeaturesByNameAsync(featureNames) : HideFeaturesByNameAsync(featureNames),
                refreshOverlay);
        }

        /// <summary>
        /// Executes a visibility change action and applies the shared post-update behavior.
        /// </summary>
        /// <param name="changeVisibilityAsync">The visibility operation to apply.</param>
        /// <param name="refreshOverlay">Indicates whether to redraw the features overlay after the change.</param>
        private async Task ApplyVisibilityChangeAsync(Func<Task> changeVisibilityAsync, bool refreshOverlay)
        {
            // Execute the provided visibility change operation (show or hide features).
            await changeVisibilityAsync();

            // After changing visibility, refresh the selection highlight to ensure it reflects the current state
            // of visible features.
            await RefreshSelectionHighlightAsync();

            // Redraw the features overlay to reflect the visibility changes if requested.
            if (refreshOverlay)
            {
                await RuntimeState.FeaturesOverlay.RedrawAsync();
            }
        }

        #region Feature Hiding
        /// <summary>
        /// Hides all drawn and placed features on the map and redraws the features overlay.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private Task HideAllFeaturesAsync()
        {
            // Hide all features in all feature layers by adding their IDs to the FeatureIdsToExclude collection.
            foreach (var layer in RuntimeState.FeaturesOverlay.Layers)
            {
                // Check if the layer is an InMemoryFeatureLayer before attempting to hide features.
                if (layer is InMemoryFeatureLayer featureLayer)
                {
                    // Add all feature IDs from the internal features collection to the FeatureIdsToExclude collection to hide them.
                    foreach (var feature in featureLayer.InternalFeatures)
                    {
                        // Only add the feature ID to the FeatureIdsToExclude collection if it is not already present.
                        if (!featureLayer.FeatureIdsToExclude.Contains(feature.Id))
                        {
                            featureLayer.FeatureIdsToExclude.Add(feature.Id);
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Hides all drawn and placed features whose IDs appear in <paramref name="featureIds"/> and
        /// redraws the features overlay.
        /// </summary>
        /// <param name="featureIds">The list of feature IDs to hide.</param>
        /// <param name="refreshOverlay">Indicates whether to refresh the overlay after deleting the features.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private Task HideFeaturesByIdAsync(List<string> featureIds, InMemoryFeatureLayer featureLayer)
        {
            // If the list of IDs is null or empty, return early as there are no features to hide.
            if (featureIds == null || featureIds.Count == 0)
            {
                return Task.CompletedTask;
            }

            // Add the specified feature IDs to the FeatureIdsToExclude collection to hide them.
            foreach (var id in featureIds)
            {
                // Only add the feature ID to the FeatureIdsToExclude collection if it is not null or empty, and if it exists in the internal
                // features collection.
                if (!string.IsNullOrEmpty(id))
                {
                    // Only add the feature ID to the FeatureIdsToExclude collection if it is not already present.
                    if (featureLayer.InternalFeatures.Contains(id) && !featureLayer.FeatureIdsToExclude.Contains(id))
                    {
                        featureLayer.FeatureIdsToExclude.Add(id);
                    }
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Hides the features with the specified IDs in both the drawn and placed feature layers and redraws the features overlay.
        /// </summary>
        /// <param name="featureIds">The list of feature IDs to hide.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task HideFeaturesByIdAsync(List<string> featureIds)
        {
            // If the list of IDs is null or empty, return early as there are no features to hide.
            if (featureIds == null || featureIds.Count == 0)
            {
                return;
            }

            // Hide the features with the specified IDs in all feature layers.
            foreach (var layer in RuntimeState.FeaturesOverlay.Layers)
            {
                // Check if the layer is an InMemoryFeatureLayer before attempting to hide features.
                if (layer is InMemoryFeatureLayer featureLayer)
                {
                    // Hide the features with the specified IDs in the current feature layer and optionally refresh the overlay.
                    await HideFeaturesByIdAsync(featureIds, featureLayer);
                }
            }
        }

        /// <summary>
        /// Hides the features with the specified names in both drawn and placed feature layers and redraws the features overlay.
        /// </summary>
        /// <param name="featureNames">The list of feature names to hide.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task HideFeaturesByNameAsync(List<string> featureNames)
        {
            // If the list of names is null or empty, return early as there are no features to hide.
            if (featureNames == null || featureNames.Count == 0)
            {
                return;
            }

            // Hide the features with the specified names in all feature layers.
            foreach (var layer in RuntimeState.FeaturesOverlay.Layers)
            {
                // Check if the layer is an InMemoryFeatureLayer before attempting to hide features.
                if (layer is InMemoryFeatureLayer featureLayer)
                {
                    // Retrieve the feature IDs corresponding to the provided names.
                    List<string> featureIds = GetFeatureIdsFromNames(featureNames, featureLayer);

                    // Hide the features with the retrieved IDs in the current feature layer and optionally refresh the overlay.
                    await HideFeaturesByIdAsync(featureIds, featureLayer);
                }
            }
        }
        #endregion

        #region Feature Showing
        /// <summary>
        /// Shows all drawn and placed features on the map and redraws the features overlay.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private Task ShowAllFeaturesAsync()
        {
            // Show all features in all feature layers by clearing the FeatureIdsToExclude collection.
            foreach (var layer in RuntimeState.FeaturesOverlay.Layers)
            {
                // Check if the layer is an InMemoryFeatureLayer before attempting to show features.
                if (layer is InMemoryFeatureLayer featureLayer)
                {
                    // Clear the FeatureIdsToExclude collection to show all features in the current feature layer.
                    featureLayer.FeatureIdsToExclude.Clear();
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Restores visibility for features whose IDs appear in <paramref name="featureIds"/> and
        /// redraws the features overlay.
        /// </summary>
        /// <param name="featureIds">The list of feature IDs to show.</param>
        /// <param name="featureLayer">The feature layer containing the features.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static Task ShowFeaturesByIdAsync(List<string> featureIds, InMemoryFeatureLayer featureLayer)
        {
            // If the list of IDs is null or empty, return early as there are no features to show.
            if (featureIds == null || featureIds.Count == 0)
            {
                return Task.CompletedTask;
            }

            // Remove the specified feature IDs from the FeatureIdsToExclude collection to restore their visibility.
            foreach (var id in featureIds)
            {
                // Only remove the feature ID from the FeatureIdsToExclude collection if it is not null or empty, and if it exists in the internal
                // features collection.
                if (!string.IsNullOrEmpty(id))
                {
                    // Only remove the feature ID from the FeatureIdsToExclude collection if it is currently present.
                    if (featureLayer.InternalFeatures.Contains(id) && featureLayer.FeatureIdsToExclude.Contains(id))
                    {
                        featureLayer.FeatureIdsToExclude.Remove(id);
                    }
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Restores visibility for the features with the specified IDs in both the drawn and placed feature layers and redraws the features overlay.
        /// </summary>
        /// <param name="featureIds">The list of feature IDs to show.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ShowFeaturesByIdAsync(List<string> featureIds)
        {
            // If the list of IDs is null or empty, return early as there are no features to show.
            if (featureIds == null || featureIds.Count == 0)
            {
                return;
            }

            // Show the features with the specified IDs in all feature layers.
            foreach (var layer in RuntimeState.FeaturesOverlay.Layers)
            {
                // Check if the layer is an InMemoryFeatureLayer before attempting to show features.
                if (layer is InMemoryFeatureLayer featureLayer)
                {
                    // Restore visibility for the features with the specified IDs in the current feature layer and optionally refresh the overlay.
                    await ShowFeaturesByIdAsync(featureIds, featureLayer);
                }
            }
        }

        /// <summary>
        /// Restores visibility for the features with the specified names in both the drawn and placed feature layers and redraws the features overlay.
        /// </summary>
        /// <param name="featureNames">The list of feature names to show.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ShowFeaturesByNameAsync(List<string> featureNames)
        {
            // If the list of names is null or empty, return early as there are no features to show.
            if (featureNames == null || featureNames.Count == 0)
            {
                return;
            }

            // Show the features with the specified names in all feature layers.
            foreach (var layer in RuntimeState.FeaturesOverlay.Layers)
            {
                // Check if the layer is an InMemoryFeatureLayer before attempting to show features.
                if (layer is InMemoryFeatureLayer featureLayer)
                {
                    // Retrieve the feature IDs corresponding to the provided names.
                    List<string> featureIds = GetFeatureIdsFromNames(featureNames, featureLayer);

                    // Restore visibility for the features with the retrieved IDs in the current feature layer.
                    await ShowFeaturesByIdAsync(featureIds, featureLayer);
                }
            }
        }
        #endregion

        #endregion

        #region Persistence and Runtime Cleanup
        /// <summary>
        /// Saves map features and related map state.
        /// </summary>
        /// <returns>A task representing the save operation.</returns>
        public Task SaveFeaturesAsync()
        {
            // Sync the ActiveMapClass collections from the live feature layers before saving to ensure that the saved
            // map reflects the current state of the map.
            SyncActiveMapClassFromLayers();

            // Convert the map center point from map units to a format suitable for saving.
            RuntimeState.ActiveMapClass.MapCenterPoint = MapGeoUtilities.ConvertMapPointFromMapUnits(RuntimeState.ActiveMapClass.MapCenterPoint);

            // Save the ActiveMapClass to an XML file at the specified map file path.
            XmlUtilities.SaveToXml(RuntimeState.ActiveMapClass, GetMapFilePath());

            return Task.CompletedTask;
        }

        /// <summary>
        /// Syncs <see cref="MapSettings.ActiveMapClass"/> collections from the live feature layers.
        /// Call this after any operation that mutates the drawn or placed feature layers directly.
        /// </summary>
        private void SyncActiveMapClassFromLayers()
        {
            // Clear the existing collections of drawn and placed features in the ActiveMapClass.
            RuntimeState.ActiveMapClass.MapDrawnFeatures = new Collection<MapDrawnFeature>();

            // Iterate through each feature in the DrawnFeatures layer to process and add them to the ActiveMapClass's collection of drawn features.
            foreach (Feature feature in ((InMemoryFeatureLayer)RuntimeState.FeaturesOverlay.Layers[MapConstants.DrawnFeaturesName]).InternalFeatures)
            {
                // If the feature name is null or empty, assign a default name based on whether it is a line or not.
                if (string.IsNullOrEmpty(feature.ColumnValues[MapConstants.NameColumn].ToString()))
                {
                    feature.ColumnValues[MapConstants.NameColumn] = GetDefaultDrawnFeatureName(feature.ColumnValues[MapConstants.IsLineColumn].ToString() == "1");
                }

                // Convert the map feature to a MapDrawnFeature. 
                var mapDrawnShape = ConversionUtilities.ConvertMapFeatureToMapDrawnFeature(feature);

                // Normalize the longitudes of the drawn shape to ensure they are within valid ranges.
                MapGeoUtilities.NormalizeSavedLongitudes(mapDrawnShape);

                // Calculate the line distance of the drawn shape in kilometers.
                mapDrawnShape.LineDistance = MapMeasurementUtilities.CalculateLineDistanceKilometers(mapDrawnShape);

                // Populate the FromFeature and ToFeature properties of the drawn shape based on its endpoints.
                PopulateLineEndpointFeatures(mapDrawnShape);

                // Calculate the area of the drawn shape in square kilometers.
                mapDrawnShape.ShapeArea = MapMeasurementUtilities.CalculateShapeAreaSquareKilometers(mapDrawnShape);

                // Add the processed drawn shape to the ActiveMapClass's collection of drawn features.
                RuntimeState.ActiveMapClass.MapDrawnFeatures.Add(mapDrawnShape);
            }

            // Include any features still in the EditOverlay (e.g. unsaved in-progress shapes).
            if (RuntimeState.ActiveMapView?.EditOverlay?.Features != null)
            {
                // Iterate through each feature in the EditOverlay's Features collection.
                foreach (Feature feature in RuntimeState.ActiveMapView.EditOverlay.Features)
                {
                    // If the feature name is null or empty, assign a default name based on its icon name.
                    if (string.IsNullOrEmpty(feature.ColumnValues[MapConstants.NameColumn].ToString()))
                    {
                        // Assign a default name to the feature based on whether it is a line or not.
                        feature.ColumnValues[MapConstants.NameColumn] = GetDefaultDrawnFeatureName(feature.ColumnValues[MapConstants.IsLineColumn].ToString() == "1");
                    }

                    // Convert the map feature to a MapDrawnFeature.
                    var mapFeature = ConversionUtilities.ConvertMapFeatureToMapDrawnFeature(feature);

                    // Normalize the longitudes of the drawn shape to ensure they are within valid ranges.
                    MapGeoUtilities.NormalizeSavedLongitudes(mapFeature);

                    // Calculate the line distance of the drawn shape in kilometers.
                    mapFeature.LineDistance = MapMeasurementUtilities.CalculateLineDistanceKilometers(mapFeature);

                    // Populate the FromFeature and ToFeature properties of the drawn shape based on its endpoints.
                    PopulateLineEndpointFeatures(mapFeature);

                    // Calculate the area of the drawn shape in square kilometers.
                    mapFeature.ShapeArea = MapMeasurementUtilities.CalculateShapeAreaSquareKilometers(mapFeature);

                    // Add the processed drawn shape to the ActiveMapClass's collection of drawn features.
                    RuntimeState.ActiveMapClass.MapDrawnFeatures.Add(mapFeature);
                }
            }

            // Rebuild placed features collection from the placed features layer.
            RuntimeState.ActiveMapClass.MapPlacedFeatures = new Collection<MapPlacedFeature>();

            // Iterate through each feature in the PlacedFeatures layer to process and add them to the ActiveMapClass's collection of placed features.
            foreach (Feature feature in ((InMemoryFeatureLayer)RuntimeState.FeaturesOverlay.Layers[MapConstants.PlacedFeaturesName]).InternalFeatures)
            {
                // If the feature name is null or empty, assign a default name based on its icon name.
                if (string.IsNullOrEmpty(feature.ColumnValues[MapConstants.NameColumn].ToString()))
                {
                    // Assign a default name to the feature based on its icon name.
                    feature.ColumnValues[MapConstants.NameColumn] = GetDefaultPlacedFeatureName(feature.ColumnValues[MapConstants.IconNameColumn].ToString());
                }

                // Convert the map feature to a MapPlacedFeature.
                var mapPlacedShape = ConversionUtilities.ConvertMapFeatureToMapPlacedFeature(feature);

                // Normalize the longitude of the placed shape's feature center to ensure it is within valid ranges.
                mapPlacedShape.FeatureCenter = MapGeoUtilities.NormalizeLongitudeForSave(mapPlacedShape.FeatureCenter);

                // Calculate the area of the placed shape in square kilometers (if applicable).
                RuntimeState.ActiveMapClass.MapPlacedFeatures.Add(mapPlacedShape);
            }

        }

        /// <summary>
        /// Populates the FromFeature and ToFeature properties of a drawn feature based on its endpoints and nearby placed features.
        /// </summary>
        /// <param name="drawnFeature">The drawn feature whose endpoint features are to be populated.</param>
        private void PopulateLineEndpointFeatures(MapDrawnFeature drawnFeature)
        {
            // If the drawn feature is null, return early as there are no endpoints to process.
            if (drawnFeature == null)
            {
                return;
            }

            // If the drawn feature is not a line or does not have enough vertices, return early as there are no endpoints to process.
            if (!drawnFeature.IsLine || drawnFeature.LineVertices == null || drawnFeature.LineVertices.Count < 2)
            {
                return;
            }

            // Reset the FromFeature and ToFeature properties to empty strings before attempting to populate them.
            drawnFeature.FromFeature = string.Empty;
            drawnFeature.ToFeature = string.Empty;

            // Determine the threshold distance for snapping to nearby placed features based on the map's unit of measurement
            // (meters or decimal degrees).
            var endpointThreshold = RuntimeState.ActiveMapView?.MapUnit == GeographyUnit.Meter
                ? MapConstants.LineEndpointSnapThresholdMeters
                : MapConstants.LineEndpointSnapThresholdDecimalDegrees;

            // Calculate the squared threshold distance for efficient distance comparisons.
            var maxDistanceSquared = endpointThreshold * endpointThreshold;

            // Convert the start and end vertices of the drawn feature to map points for distance calculations.
            var startPoint = MapMeasurementUtilities.ConvertMapCoordinateToMapPoint(drawnFeature.LineVertices[0]);
            var endPoint = MapMeasurementUtilities.ConvertMapCoordinateToMapPoint(drawnFeature.LineVertices[^1]);

            // Attempt to find the closest placed feature within the threshold distance for both the start and end points of the drawn feature.
            if (TryGetClosestPlacedFeatureWithinDistance(startPoint, maxDistanceSquared, out var startFeature, out _) && startFeature != null)
            {
                // If a nearby placed feature is found for the start point, set the FromFeature property of the drawn feature to the name of that
                // placed feature.
                drawnFeature.FromFeature = startFeature.ColumnValues.TryGetValue(MapConstants.NameColumn, out var startName)
                    ? startName?.ToString() ?? string.Empty
                    : string.Empty;
            }

            // Attempt to find the closest placed feature within the threshold distance for the end point of the drawn feature.
            if (TryGetClosestPlacedFeatureWithinDistance(endPoint, maxDistanceSquared, out var endFeature, out _) && endFeature != null)
            {
                // If a nearby placed feature is found for the end point, set the ToFeature property of the drawn feature to the name of that
                // placed feature.
                drawnFeature.ToFeature = endFeature.ColumnValues.TryGetValue(MapConstants.NameColumn, out var endName)
                    ? endName?.ToString() ?? string.Empty
                    : string.Empty;
            }
        }

        /// <summary>
        /// Clears the saved map by deleting the map XML file and resetting the map layers.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ClearSavedMapAsync()
        {
            // If the map is read-only, do not allow clearing the saved map.
            if (IsReadOnly)
            {
                return;
            }

            // Delete the map XML file if it exists.
            var mapPath = GetMapFilePath();
            if (File.Exists(mapPath))
            {
                File.Delete(mapPath);
            }

            // Reset the ActiveMapClass to a new instance and clear all features from the drawn, placed, and arrow feature layers.
            RuntimeState.ActiveMapClass = new Map();
            RuntimeState.DrawnFeaturesLayer.InternalFeatures.Clear();
            RuntimeState.PlacedFeaturesLayer.InternalFeatures.Clear();
            RuntimeState.ArrowFeaturesLayer.InternalFeatures.Clear();
            RuntimeState.MapEditOverlay.Features.Clear();

            // Keep selection highlight in sync and redraw the features overlay to reflect the cleared state.s
            await RefreshSelectionHighlightAsync();
            await RuntimeState.FeaturesOverlay.RedrawAsync();
        }

        /// <summary>
        /// Closes map layers and disposes the active map view.
        /// </summary>
        public async ValueTask DisposeRuntimeStateAsync()
        {
            // Safely close the specified feature layer, ignoring any exceptions that may occur during the close
            // operation.
            static void SafeClose(FeatureLayer? layer)
            {
                try { layer?.Close(); } catch { }
            }

            // Safely close the drawn, placed, and selection highlight feature layers.
            SafeClose(RuntimeState.DrawnFeaturesLayer);
            SafeClose(RuntimeState.PlacedFeaturesLayer);
            SafeClose(RuntimeState.SelectionHighlightLayer);

            // Safely dispose the active map view, handling both IAsyncDisposable and IDisposable cases.
            if (RuntimeState.ActiveMapView is IAsyncDisposable asyncDisposableMapView)
            {
                try
                {
                    await asyncDisposableMapView.DisposeAsync();
                }
                catch
                {
                }
            }
            else if (RuntimeState.ActiveMapView is IDisposable disposableMapView)
            {
                try
                {
                    disposableMapView.Dispose();
                }
                catch
                {
                }
            }
        }
        #endregion

        #region Internal Utility Helpers
        /// <summary>
        /// Retrieves the feature IDs for features with the specified names from the provided feature layer.
        /// </summary>
        /// <param name="featureNames">The list of feature names to search for.</param>
        /// <param name="featureLayer">The feature layer to search in.</param>
        /// <returns>A list of feature IDs corresponding to the specified names.</returns>
        public List<string> GetFeatureIdsFromNames(List<string> featureNames, InMemoryFeatureLayer featureLayer)
        {
            // If the list of names is null or empty, or if the feature layer is null, return an empty list as there are no features to search for.
            if (featureNames == null || featureNames.Count == 0 || featureLayer == null)
            {
                return new List<string>();
            }

            // Build a case-sensitive name-to-id lookup once, preserving first match order from the layer.
            var featureNameToIdLookup = new Dictionary<string, string>(StringComparer.Ordinal);

            // Open the feature layer to access its internal features for processing.
            featureLayer.Open();

            try
            {
                // Iterate through each feature in the internal features collection of the feature layer.
                foreach (Feature feature in featureLayer.InternalFeatures)
                {
                    // Attempt to retrieve the name value from the feature's column values using the NameColumn key.
                    if (!feature.ColumnValues.TryGetValue(MapConstants.NameColumn, out var nameValue))
                    {
                        continue;
                    }

                    // Convert the name value to a string and check if it is null, empty, or already present in the lookup dictionary.
                    var currentFeatureName = nameValue?.ToString();
                    if (string.IsNullOrWhiteSpace(currentFeatureName) || featureNameToIdLookup.ContainsKey(currentFeatureName))
                    {
                        continue;
                    }

                    // Add the feature name and its corresponding ID to the lookup dictionary.
                    featureNameToIdLookup[currentFeatureName] = feature.Id;
                }
            }
            finally
            {
                // Close the feature layer.
                featureLayer.Close();
            }

            // Build the list of feature IDs corresponding to the provided feature names.
            var featureIds = new List<string>();
            foreach (var name in featureNames)
            {
                // If the name is not null or whitespace and exists in the lookup dictionary, add the corresponding
                // feature ID to the list.
                if (!string.IsNullOrWhiteSpace(name) && featureNameToIdLookup.TryGetValue(name, out var featureId))
                {
                    // Add the feature ID to the list of feature IDs.
                    featureIds.Add(featureId);
                }
            }

            //  Return the list of feature IDs corresponding to the specified names.
            return featureIds;
        }

        /// <summary>
        /// Creates line arrow features for the specified drawn feature and associates them with the given line feature ID.
        /// </summary>
        /// <param name="drawnFeature">The drawn feature for which to create line arrow features.</param>
        /// <param name="lineFeatureId">The ID of the line feature to associate with the created arrow features.</param>
        /// <returns>A list of created line arrow features.</returns>
        private async Task<List<Feature>> CreateLineArrowFeaturesAsync(MapDrawnFeature drawnFeature, string lineFeatureId)
        {
            // If the drawn feature is null or the line feature ID is null or whitespace, return an empty list as there are no arrow features
            // to create.
            if (drawnFeature == null || string.IsNullOrWhiteSpace(lineFeatureId))
            {
                return new List<Feature>();
            }

            // Initialize the list of arrow features to be created.
            var arrowFeatures = new List<Feature>();
            if (!MapLineArrowUtilities.ShouldAddLineArrows(drawnFeature))
            {
                return arrowFeatures;
            }

            // Determine the endpoint selection radius in world units, which is used to check for nearby placed features at the line endpoints.
            var endpointSelectionRadius = await GetEndpointSelectionRadiusWorldAsync();

            // If the endpoint selection radius is not positive, use the default snap threshold based on the map unit.
            if (endpointSelectionRadius <= 0d)
            {
                // If the endpoint selection radius is not positive, use the default snap threshold based on the map unit.
                endpointSelectionRadius = RuntimeState.ActiveMapView?.MapUnit == GeographyUnit.Meter
                    ? MapConstants.LineEndpointSnapThresholdMeters
                    : MapConstants.LineEndpointSnapThresholdDecimalDegrees;
            }

            // Calculate the squared endpoint selection radius for efficient distance comparisons.
            var endpointSelectionRadiusSquared = endpointSelectionRadius * endpointSelectionRadius;

            // Check if there are placed features near both endpoints of the drawn feature. If not, return an empty list of arrow features.
            if (!HasPlacedFeaturesAtLineEndpoints(drawnFeature, endpointSelectionRadiusSquared))
            {
                return arrowFeatures;
            }

            // Retrieve the vertices of the arrow path from the drawn feature, preferring generated vertices if available.
            var arrowPathVertices = MapLineArrowUtilities.GetArrowPathVertices(drawnFeature);

            // If there are fewer than two vertices in the arrow path, return an empty list of arrow features.
            if (arrowPathVertices.Count < 2)
            {
                return arrowFeatures;
            }

            // Calculate the line distance in kilometers, using the drawn feature's LineDistance if available, or calculating it from the vertices.
            var lineDistanceKilometers = drawnFeature.LineDistance > 0d
                ? drawnFeature.LineDistance
                : MapMeasurementUtilities.CalculateLineDistanceKilometers(drawnFeature);

            // Arrow density formula: one arrow per LineArrowDistanceKilometers, floored to an integer count.
            var requestedArrowCount = (int)Math.Floor(lineDistanceKilometers / MapConstants.LineArrowDistanceKilometers);

            // Build the initial density-based arrow indices.
            var arrowIndices = MapLineArrowUtilities.GetArrowVertexIndices(arrowPathVertices.Count, requestedArrowCount);

            // Ensure each original line segment contributes at least one arrow index.
            var minimumSegmentArrowIndices = MapLineArrowUtilities.GetMinimumArrowIndicesPerLineSegment(drawnFeature, arrowPathVertices);
            if (minimumSegmentArrowIndices.Count > 0)
            {
                arrowIndices = arrowIndices
                    .Concat(minimumSegmentArrowIndices)
                    .Distinct()
                    .OrderBy(i => i)
                    .ToList();
            }

            // If there are no valid arrow indices, return an empty list of arrow features.
            if (arrowIndices.Count == 0)
            {
                return arrowFeatures;
            }

            // Determine the arrow prefix based on the current dark mode setting, and generate a unique arrow ID prefix for the arrow features.
            var arrowPrefix = RuntimeState.UseDarkMode
                ? MapConstants.ArrowDefaultDarkPrefix
                : MapConstants.ArrowDefaultLightPrefix;

            // Use the provided line feature ID as the prefix for arrow feature IDs, or generate a new GUID if the line feature ID is null
            // or whitespace.
            var arrowIdPrefix = string.IsNullOrWhiteSpace(lineFeatureId)
                ? Guid.NewGuid().ToString()
                : lineFeatureId;

            // Retrieve the half-size of the placed feature in world coordinates, which is used to nudge the terminal arrow so it visually touches the
            // endpoint placed feature symbol.
            var placedFeatureHalfSizeWorld = await GetPlacedFeatureHalfSizeWorldAsync();

            // Determine the last vertex index in the arrow path vertices, which is used to identify the terminal arrow for nudging.
            var lastVertexIndex = arrowPathVertices.Count - 1;

            // Iterate through each arrow vertex index to create arrow features at the specified vertices along the drawn feature's path.
            foreach (var vertexIndex in arrowIndices)
            {
                // Skip invalid vertex indices that are out of bounds for the arrow path vertices.
                if (vertexIndex <= 0 || vertexIndex > lastVertexIndex)
                {
                    continue;
                }

                // Always use the previous vertex for direction; endpoint arrow uses last and next-to-last vertices.
                var directionStartPoint = MapMeasurementUtilities.ConvertMapCoordinateToMapPoint(arrowPathVertices[vertexIndex - 1]);
                var directionEndPoint = MapMeasurementUtilities.ConvertMapCoordinateToMapPoint(arrowPathVertices[vertexIndex]);

                // Calculate the wrapped delta-X and delta-Y between the start and end points to determine the direction of the arrow.
                var dx = MapMeasurementUtilities.GetWrappedDeltaX(directionStartPoint.X, directionEndPoint.X);
                var dy = directionEndPoint.Y - directionStartPoint.Y;

                // Skip near zero-length segments to avoid creating arrows with undefined direction.
                if (Math.Abs(dx) < MapConstants.CoordinateComparisonEpsilon &&
                    Math.Abs(dy) < MapConstants.CoordinateComparisonEpsilon)
                {
                    continue;
                }

                // Use wrapped delta-X so headings stay correct around the dateline.
                var angle = Math.Atan2(dy, dx) * (180d / Math.PI);

                // Determine the arrow direction based on the calculated angle and retrieve the canonical arrow image name for that direction.
                var arrowDirection = MapLineArrowUtilities.GetArrowDirection(angle);

                //Get the canonical arrow image name for the determined direction.
                var arrowName = GeoImageUtilities.GetCanonicalArrowGeoImageName($"{arrowPrefix}{arrowDirection}");

                // Skip invalid arrow names that are null, empty, or do not correspond to a valid arrow image index.
                if (string.IsNullOrWhiteSpace(arrowName) || GeoImageUtilities.GetArrowGeoImageIndex(arrowName) < 0)
                {
                    continue;
                }

                // Normalize into visible world copy for stable rendering, then nudge only the terminal arrow
                // so it visually touches (rather than overlays) the endpoint placed feature symbol.
                var arrowPoint = MapGeoUtilities.AdjustObjectPointFromWorldMap(directionEndPoint);

                // If this is the last vertex index, nudge the arrow point to visually touch the endpoint placed feature symbol.
                if (vertexIndex == lastVertexIndex)
                {
                    // Nudge the arrow point to visually touch the endpoint placed feature symbol, using the half-size of the placed feature in world coordinates.
                    arrowPoint = MapMeasurementUtilities.GetPlacedFeatureTouchPoint(arrowPoint, directionStartPoint, placedFeatureHalfSizeWorld);
                }

                // Create a new arrow feature at the calculated arrow point, with a unique ID based on the arrow ID prefix and vertex index.
                var arrowFeature = new Feature(arrowPoint)
                {
                    Id = $"{arrowIdPrefix}_arrow_{vertexIndex}"
                };

                // Set the column values for the arrow feature, including the arrow name, line feature ID, and other relevant properties.
                arrowFeature.ColumnValues[MapConstants.NameColumn] = string.Empty;
                arrowFeature.ColumnValues[MapConstants.LabelNameColumn] = string.Empty;
                arrowFeature.ColumnValues[MapConstants.LineLabelNameColumn] = string.Empty;
                arrowFeature.ColumnValues[MapConstants.ArrowNameColumn] = arrowName;
                arrowFeature.ColumnValues[MapConstants.IsLineColumn] = "0";
                arrowFeature.ColumnValues[MapConstants.LineVerticesColumn] = string.Empty;
                arrowFeature.ColumnValues[MapConstants.GeneratedLineVerticesColumn] = string.Empty;
                arrowFeature.ColumnValues[MapConstants.IsCircleColumn] = "0";
                arrowFeature.ColumnValues[MapConstants.CircleCenterXColumn] = string.Empty;
                arrowFeature.ColumnValues[MapConstants.CircleCenterYColumn] = string.Empty;
                arrowFeature.ColumnValues[MapConstants.CircleRadiusColumn] = string.Empty;
                arrowFeature.ColumnValues[MapConstants.PolygonVerticesColumn] = string.Empty;
                arrowFeature.ColumnValues[MapConstants.GeneratedPolygonVerticesColumn] = string.Empty;

                // Add the created arrow feature to the list of arrow features to be returned.
                arrowFeatures.Add(arrowFeature);
            }

            // Return the list of created arrow features for the drawn feature.
            return arrowFeatures;
        }

        /// <summary>
        /// Determines whether there are placed features near both endpoints of the specified drawn feature within the given maximum distance squared.
        /// </summary>
        /// <param name="drawnFeature">The drawn feature to check for nearby placed features at its endpoints.</param>
        /// <param name="maxDistanceSquared">The maximum distance squared within which to search for placed features.</param>
        /// <returns>True if there are placed features near both endpoints; otherwise, false.</returns>
        private bool HasPlacedFeaturesAtLineEndpoints(MapDrawnFeature drawnFeature, double maxDistanceSquared)
        {
            // Arrow generation requires a source and destination point feature near both line endpoints.
            if (drawnFeature?.LineVertices == null || drawnFeature.LineVertices.Count < 2 || maxDistanceSquared <= 0d)
            {
                return false;
            }

            // Convert the start and end vertices of the drawn feature to map points for distance calculations.
            var startPoint = MapMeasurementUtilities.ConvertMapCoordinateToMapPoint(drawnFeature.LineVertices[0]);
            var endPoint = MapMeasurementUtilities.ConvertMapCoordinateToMapPoint(drawnFeature.LineVertices[^1]);

            // Attempt to find the closest placed feature within the threshold distance for both the start and end points of the drawn feature.
            var hasStartFeature = TryGetClosestPlacedFeatureWithinDistance(startPoint, maxDistanceSquared, out _, out _);
            var hasEndFeature = TryGetClosestPlacedFeatureWithinDistance(endPoint, maxDistanceSquared, out _, out _);

            // Return true only if both endpoints have nearby placed features; otherwise, return false.
            return hasStartFeature && hasEndFeature;
        }

        #endregion
    }
}
