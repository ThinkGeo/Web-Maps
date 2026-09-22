
using ThinkGeo.UI.Blazor.HowDoI.CustomerApp.Classes;
using ThinkGeo.Core;
using ThinkGeo.UI.Blazor;

namespace ThinkGeo.UI.Blazor.HowDoI.CustomerApp.SharedProperties
{
    /// <summary>
    /// Base class for map configuration and runtime state, containing core view settings, interaction modes, and zoom/extent constraints.
    /// </summary>
    /// <remarks>
    /// MapSettings serves as the foundation for the map's operational state across multiple layers of initialization and interaction:
    /// 
    /// - **View State**: Tracks the active ThinkGeo MapView (viewport, zoom level, center) and Map instance (feature management).
    ///   These are initialized to new instances on construction and populated/configured during map initialization (handled externally).
    /// 
    /// - **Interaction Modes**: IsReadOnly allows disabling add/delete/edit operations while preserving selection capabilities (query-only mode).
    ///   UseDarkMode selects appropriate theme colors for visibility in dark environments.
    ///   IsMapInitialized tracks whether the MapView has completed startup asynchronous work.
    /// 
    /// - **Rendering Context**: MapViewportWidthPixels caches the last rendered viewport width, enabling world-units-per-pixel calculations
    ///   for scale-dependent feature rendering and interaction detection.
    /// 
    /// - **Zoom and Extent Constraints**: Static zoom levels (2-20) and scale denominators (548-98,431,965) enforce hard limits across
    ///   all map operations. Restricted extent bounds (in both meter and decimal degree coordinate systems) define the viewable area,
    ///   supporting wrapping at the dateline via MaxWorldWidthMultiplier.
    /// 
    /// Extended by MapRuntimeState to add layers/overlays, asset caches, and selection state.
    /// </remarks>
    public class MapSettings
    {
        #region View State

        /// <summary>
        /// The current active MapView for the map display.
        /// </summary>
        /// <remarks>
        /// Initialized to a new MapView instance on construction. Populated and configured during map initialization
        /// (typically by the Blazor MapView component). Represents the viewport, zoom level, center coordinate, and other view state.
        /// </remarks>
        public MapView ActiveMapView { get; set; } = new MapView();

        /// <summary>
        /// The current active Map class instance managing features and layers.
        /// </summary>
        /// <remarks>
        /// Initialized to a new Map instance on construction. Manages all persisted features (drawn and placed),
        /// provides CRUD operations, and coordinates persistence with external storage. Initialized with default values;
        /// caller is responsible for configuring it during map setup.
        /// </remarks>
        public Map ActiveMapClass { get; set; } = new Map();

        #endregion

        #region Interaction Modes

        /// <summary>
        /// Indicates whether the map is displayed in dark mode.
        /// </summary>
        /// <remarks>
        /// When true, label colors, line colors, and icon tinting are adjusted to white or light colors for visibility on dark backgrounds.
        /// When false, colors are adjusted to black or dark colors for light backgrounds.
        /// This property is consulted by MapLayerUtilities and other styling utilities to select theme-appropriate colors.
        /// Defaults to true (dark mode enabled).
        /// </remarks>
        public bool UseDarkMode { get; set; } = true;

        /// <summary>
        /// Indicates whether the map has been initialized.
        /// </summary>
        /// <remarks>
        /// Set to false initially; typically set to true after the Blazor MapView component's initialization completes.
        /// Used to ensure that interactive operations (drawing, selection, etc.) are deferred until the map is ready.
        /// </remarks>
        public bool IsMapInitialized { get; set; } = false;

        /// <summary>
        /// Indicates whether the map should prevent add, delete, and edit operations.
        /// </summary>
        /// <remarks>
        /// When true, feature selection remains available for query purposes, but interactive drawing, editing, and deletion are disabled.
        /// This supports query-only or view-only scenarios without losing selection feedback.
        /// </remarks>
        public bool IsReadOnly { get; set; } = false;

        #endregion

        #region Rendering Context

        /// <summary>
        /// Latest rendered map viewport width in pixels, used for world-units-per-pixel calculations.
        /// </summary>
        /// <remarks>
        /// Updated whenever the map view is resized or redrawn. Used to calculate the scale (world units per pixel) for
        /// scale-dependent feature rendering and interaction hit detection (e.g., tolerance for feature selection by click).
        /// Defaults to 0; valid values should be greater than 0.
        /// </remarks>
        public double MapViewportWidthPixels { get; set; } = 0d;

        #endregion

        #region Zoom and Extent Constraints

        /// <summary>
        /// The minimum zoom level for the map, representing the furthest out (most zoomed-out) view.
        /// </summary>
        /// <remarks>
        /// Constant value: 2. Enforced globally across all map operations to prevent viewing beyond this constraint.
        /// </remarks>
        public static readonly int MinZoomLevel = 2;

        /// <summary>
        /// The maximum zoom level for the map, representing the closest in (most zoomed-in) view.
        /// </summary>
        /// <remarks>
        /// Constant value: 20. Enforced globally across all map operations to prevent excessive zoom magnification.
        /// </remarks>
        public static readonly int MaxZoomLevel = 20;

        /// <summary>
        /// The minimum scale for the map, expressed as a scale denominator (closest zoom).
        /// </summary>
        /// <remarks>
        /// Constant value: 548, corresponding to zoom level 20. Smaller denominators mean larger map area visible per screen pixel.
        /// Used by ThinkGeo to enforce maximum zoom constraints when converting zoom levels to scale denominators.
        /// </remarks>
        public static double MinimumScale = 548;

        /// <summary>
        /// The maximum scale for the map, expressed as a scale denominator (furthest out zoom).
        /// </summary>
        /// <remarks>
        /// Constant value: 98,431,965, corresponding to zoom level ~2.5. Larger denominators mean smaller map area visible per screen pixel.
        /// Used by ThinkGeo to enforce minimum zoom constraints when converting zoom levels to scale denominators.
        /// </remarks>
        public static double MaximumScale = 98431965;

        /// <summary>
        /// The restricted extent bounds for the map in Web Mercator (meter) coordinates.
        /// </summary>
        /// <remarks>
        /// Defines the geographic area that can be viewed and interacted with. Supports dateline wrapping via MaxWorldWidthMultiplier,
        /// allowing multiple world copies to be visible at high zoom levels. Used by the MapView to constrain panning and zooming.
        /// Calculated from MapConstants (HalfWorldWidthInMeters, MaxWebMercatorLatitude, MaxWorldWidthMultiplier).
        /// </remarks>
        public static readonly RectangleShape MapRestrictExtentinMeters =
            new RectangleShape(-MapConstants.MaxWorldWidthMultiplier * MapConstants.HalfWorldWidthInMeters,
                               MapConstants.HalfWorldWidthInMeters,
                               MapConstants.MaxWorldWidthMultiplier * MapConstants.HalfWorldWidthInMeters,
                               -MapConstants.HalfWorldWidthInMeters);

        /// <summary>
        /// The restricted extent bounds for the map in geographic (decimal degree) coordinates.
        /// </summary>
        /// <remarks>
        /// Parallel to MapRestrictExtentinMeters but expressed in longitude/latitude. Supports dateline wrapping via MaxWorldWidthMultiplier.
        /// Used when coordinate transformations or geographic queries require decimal degree bounds instead of meters.
        /// Calculated from MapConstants (HalfWorldWidthInDecimalDegrees, MaxWebMercatorLatitude, MaxWorldWidthMultiplier).
        /// </remarks>
        public static readonly RectangleShape MapRestrictExtentinDecimalDegrees =
            new RectangleShape(-MapConstants.MaxWorldWidthMultiplier * MapConstants.HalfWorldWidthInDecimalDegrees,
                               MapConstants.MaxWebMercatorLatitude,
                               MapConstants.MaxWorldWidthMultiplier * MapConstants.HalfWorldWidthInDecimalDegrees,
                               -MapConstants.MaxWebMercatorLatitude);

        #endregion
    }
}