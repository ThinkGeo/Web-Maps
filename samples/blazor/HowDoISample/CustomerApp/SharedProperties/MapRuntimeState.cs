using System.Collections;
using ThinkGeo.Core;
using ThinkGeo.UI.Blazor;

namespace ThinkGeo.UI.Blazor.HowDoI.CustomerApp.SharedProperties
{
    /// <summary>
    /// Represents the complete runtime state of the map, composing map settings, layers/overlays, asset caches, and selection state.
    /// </summary>
    /// <remarks>
    /// This class serves as the central state container for all map operations, extending MapSettings with three additional state components:
    /// 
    /// - **LayersAndOverlays**: Manages all ThinkGeo layers (drawn features, placed features, arrows, selection highlight) and overlays
    ///   (raster background, edit overlay, features overlay, dynamic overlay for interaction chrome)
    /// 
    /// - **IconsAndImages**: Caches pre-loaded GeoImage assets for icons and arrow directions, mapped by index for efficient style lookup.
    ///   These assets are loaded once and reused across layer styling to avoid repeated I/O.
    /// 
    /// - **SelectionState**: Tracks currently selected features by ID, name, and metadata. Maintains a separate HiddenSelectedFeatureIds set
    ///   for features selected but not currently visible (e.g., due to filtering or zoom constraints).
    /// 
    /// Properties are exposed via pass-through accessors that delegate to the composed objects, providing a unified interface for accessing
    /// all map state without requiring callers to know about the internal composition structure.
    /// 
    /// Initialization: All composed objects are initialized to new instances by default, ensuring null-safe access throughout the application.
    /// </remarks>
    public class MapRuntimeState : MapSettings
    {
        /// <summary>
        /// Manages all map layers (drawn features, placed features, selection highlight, arrows) and overlays (raster, edit, features, dynamic).
        /// </summary>
        public MapLayersAndOverlays LayersAndOverlays { get; set; } = new MapLayersAndOverlays();

        /// <summary>
        /// Caches pre-loaded GeoImage assets for icons and arrow directions with index maps for style lookup.
        /// </summary>
        public MapIconsAndImages IconsAndImages { get; set; } = new MapIconsAndImages();

        /// <summary>
        /// Tracks currently selected features by ID, name, and metadata, including hidden selections.
        /// </summary>
        public MapSelectedFeatures SelectionState { get; set; } = new MapSelectedFeatures();

        #region Overlay Properties

        /// <summary>
        /// Gets or sets the ThinkGeo cloud raster maps overlay used for the base map background.
        /// </summary>
        /// <remarks>Delegates to LayersAndOverlays.RasterOverlay for centralized state management.</remarks>
        public ThinkGeoCloudRasterMapsOverlay RasterOverlay
        {
            get => LayersAndOverlays.RasterOverlay;
            set => LayersAndOverlays.RasterOverlay = value;
        }

        /// <summary>
        /// Gets or sets the overlay for user-driven map editing (draw, pan, selection operations).
        /// </summary>
        /// <remarks>Delegates to LayersAndOverlays.MapEditOverlay. Typically created by the Blazor MapView component.</remarks>
        public EditOverlay MapEditOverlay
        {
            get => LayersAndOverlays.MapEditOverlay;
            set => LayersAndOverlays.MapEditOverlay = value;
        }

        /// <summary>
        /// Gets or sets the overlay containing all persisted feature layers (drawn, placed, arrows).
        /// </summary>
        /// <remarks>Delegates to LayersAndOverlays.FeaturesOverlay. Layers in this overlay are refreshed when styles change.</remarks>
        public LayerOverlay FeaturesOverlay
        {
            get => LayersAndOverlays.FeaturesOverlay;
            set => LayersAndOverlays.FeaturesOverlay = value;
        }

        /// <summary>
        /// Gets or sets the overlay for dynamic/transient features (selection highlights, interaction chrome).
        /// </summary>
        /// <remarks>Delegates to LayersAndOverlays.DynamicOverlay. Kept separate from FeaturesOverlay to isolate interactive state.</remarks>
        public LayerOverlay DynamicOverlay
        {
            get => LayersAndOverlays.DynamicOverlay;
            set => LayersAndOverlays.DynamicOverlay = value;
        }

        #endregion

        #region Feature Layer Properties

        /// <summary>
        /// Gets or sets the in-memory feature layer for user-drawn shapes (polygons, circles, lines).
        /// </summary>
        /// <remarks>Delegates to LayersAndOverlays.DrawnFeaturesLayer. Contains unified schema for all drawn shape types.</remarks>
        public InMemoryFeatureLayer DrawnFeaturesLayer
        {
            get => LayersAndOverlays.DrawnFeaturesLayer;
            set => LayersAndOverlays.DrawnFeaturesLayer = value;
        }

        /// <summary>
        /// Gets or sets the in-memory feature layer for placed icon markers.
        /// </summary>
        /// <remarks>Delegates to LayersAndOverlays.PlacedFeaturesLayer. Contains icon metadata (type, name) for point features.</remarks>
        public InMemoryFeatureLayer PlacedFeaturesLayer
        {
            get => LayersAndOverlays.PlacedFeaturesLayer;
            set => LayersAndOverlays.PlacedFeaturesLayer = value;
        }

        /// <summary>
        /// Gets or sets the in-memory feature layer for selection highlight visualization.
        /// </summary>
        /// <remarks>Delegates to LayersAndOverlays.SelectionHighlightLayer. Rendered in DynamicOverlay, spans all zoom levels.</remarks>
        public InMemoryFeatureLayer SelectionHighlightLayer
        {
            get => LayersAndOverlays.SelectionHighlightLayer;
            set => LayersAndOverlays.SelectionHighlightLayer = value;
        }

        /// <summary>
        /// Gets or sets the in-memory feature layer for generated arrow direction indicators on lines.
        /// </summary>
        /// <remarks>Delegates to LayersAndOverlays.ArrowFeaturesLayer. Reuses drawn feature schema; rendered at half scale.</remarks>
        public InMemoryFeatureLayer ArrowFeaturesLayer
        {
            get => LayersAndOverlays.ArrowFeaturesLayer;
            set => LayersAndOverlays.ArrowFeaturesLayer = value;
        }

        #endregion

        #region Asset Properties (Icon and Arrow Images)

        /// <summary>
        /// Gets the list of pre-loaded GeoImage assets for icon markers.
        /// </summary>
        /// <remarks>Delegates to IconsAndImages.IconGeoImageList. Loaded once and reused across all placed feature styling.</remarks>
        public List<GeoImage> IconGeoImageList => IconsAndImages.IconGeoImageList;

        /// <summary>
        /// Gets the sorted map of icon names to their indices in IconGeoImageList.
        /// </summary>
        /// <remarks>Delegates to IconsAndImages.IconGeoImageIndexMap. Enables O(1) asset lookup during style creation.</remarks>
        public SortedList IconGeoImageIndexMap => IconsAndImages.IconGeoImageIndexMap;

        /// <summary>
        /// Gets the list of pre-loaded GeoImage assets for 8-direction arrow indicators.
        /// </summary>
        /// <remarks>Delegates to IconsAndImages.ArrowGeoImageList. Loaded once and reused across arrow feature styling.</remarks>
        public List<GeoImage> ArrowGeoImageList => IconsAndImages.ArrowGeoImageList;

        /// <summary>
        /// Gets the sorted map of compass directions (N, NE, E, SE, S, SW, W, NW) to their indices in ArrowGeoImageList.
        /// </summary>
        /// <remarks>Delegates to IconsAndImages.ArrowGeoImageIndexMap. Enables O(1) arrow asset lookup during style creation.</remarks>
        public SortedList ArrowGeoImageIndexMap => IconsAndImages.ArrowGeoImageIndexMap;

        #endregion

        #region Selection State Properties

        /// <summary>
        /// Gets the set of currently selected feature IDs.
        /// </summary>
        /// <remarks>Delegates to SelectionState.SelectedFeatureIds. Features in this set are highlighted on the map.</remarks>
        public HashSet<string> SelectedFeatureIds => SelectionState.SelectedFeatureIds;

        /// <summary>
        /// Gets the set of selected feature IDs that are currently hidden (not visible on map).
        /// </summary>
        /// <remarks>Delegates to SelectionState.HiddenSelectedFeatureIds. Updated when features are filtered or zoom-constraints hide them.</remarks>
        public HashSet<string> HiddenSelectedFeatureIds => SelectionState.HiddenSelectedFeatureIds;

        /// <summary>
        /// Gets the set of display names for currently selected features.
        /// </summary>
        /// <remarks>Delegates to SelectionState.SelectedFeatureNames. Populated from feature NameColumn values.</remarks>
        public HashSet<string> SelectedFeatureNames => SelectionState.SelectedFeatureNames;

        /// <summary>
        /// Gets the set of detailed information objects for currently selected features.
        /// </summary>
        /// <remarks>Delegates to SelectionState.SelectedFeatures. Contains ID, name, type (drawn/placed), and other metadata for each selection.</remarks>
        public HashSet<MapSelectedFeatures.SelectedFeatureInfo> SelectedFeatures => SelectionState.SelectedFeatures;

        #endregion
    }
}
