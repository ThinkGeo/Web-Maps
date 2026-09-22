using ThinkGeo.Core;

namespace ThinkGeo.UI.Blazor.HowDoI.CustomerApp.SharedProperties
{
    /// <summary>
    /// Defines shared constants used by map rendering, interaction, persistence, and asset resolution logic.
    /// </summary>
    /// <remarks>
    /// Centralizing constant values here keeps map behavior consistent across utilities and reduces magic values
    /// in styling, geometry, and file-system workflows.
    /// </remarks>
    internal static class MapConstants
    {

        #region File and Directory Names
        /// <summary>
        /// The name of the directory where icons are stored.
        /// </summary>
        public const string IconsDirectoryName = "Icons";

        /// <summary>
        /// The name of the directory where the line directional arrows are stored.
        /// </summary>
        public const string ArrowsDirectoryName = "Arrows";

        /// <summary>
        /// The name of the directory where application data is stored.
        /// </summary>
        public const string AppDataDirectoryName = "App_Data";

        /// <summary>
        /// The name of the map file.
        /// </summary>
        public const string MapFileName = "Map.xml";

        /// <summary>
        /// The name of the directory where base map data is stored.
        /// </summary>
        public const string BaseMapFolder = "BaseMap";

        /// <summary>
        /// The name of the web root directory.
        /// </summary>
        public const string WebRoot = "wwwroot";

        /// <summary>
        /// The file extension for icon images.
        /// </summary>
        public const string MapImageFileExtension = ".png";
        #endregion

        #region Layer Names
        /// <summary>
        /// The name of the layer for drawn features.
        /// </summary>
        public const string DrawnFeaturesName = "DrawnFeatures";

        /// <summary>
        /// The name of the layer for placed features.
        /// </summary>
        public const string PlacedFeaturesName = "PlacedFeatures";

        /// <summary>
        /// The name of the layer for highlighted features.
        /// </summary>
        public const string HighlightedFeaturesName = "HighlightedFeatures";

        /// <summary>
        /// The name of the layer for selected features.
        /// </summary>
        public const string SelectionHiglightLayer = "SelectionHighlightLayer";

        /// <summary>
        /// The name of the layer for generated line arrows.
        /// </summary>
        public const string ArrowFeaturesName = "ArrowFeatures";
        #endregion

        #region Misc String
        /// <summary>
        /// The API key for the map service.
        /// </summary>
        public const string MapApiKey = "PIbGd76RyHKod99KptWTeb-Jg9JUPEPUBFD3SZJYLDE~";

        /// <summary>
        /// The name used for the missing icon image.
        /// </summary>
        public const string MissingIconName = "__Missing Icon";

        /// <summary>
        /// The filename used for the missing icon image.
        /// </summary>
        public const string MissingIconFileName = "MissingIcon.png";

        /// <summary>
        /// The default label format for the map, showing latitude, longitude, and zoom level.
        /// </summary>
        public const string DefaultMousePositionLabel = "Latitude:--° --' --.--\", - Longitude:--° --' --.--\", - Zoom:--";
        #endregion

        #region Map Colors
        /// <summary>
        /// The color used for dark blue elements on the map 100% Opacity.
        /// </summary>
        public static readonly GeoColor ColorDarkBlue = GeoColor.FromArgb(255, 0, 0, 139);

        /// <summary>
        /// The color used for dark blue elements on the map 78% Opacity.
        /// </summary>
        public static readonly GeoColor ColorDarkBlue78 = GeoColor.FromArgb(200, 0, 0, 139);

        /// <summary>
        /// The color used for light blue elements on the map 47% Opacity.
        /// </summary>
        public static readonly GeoColor ColorLightBlue47 = GeoColor.FromArgb(120, 173, 216, 230);
        /// <summary>
        /// The color used for light blue elements on the map 31% Opacity.
        /// </summary>
        public static readonly GeoColor ColorLightBlue31 = GeoColor.FromArgb(80, 173, 216, 230);

        /// <summary>
        /// The color used for light purple elements on the map.
        /// </summary>
        public static readonly GeoColor ColorLightPurple = GeoColor.FromArgb(51, 139, 83, 237);

        /// <summary>
        /// The color used for dark purple elements on the map.
        /// </summary>
        public static readonly GeoColor ColorDarkPurple = GeoColor.FromArgb(255, 125, 50, 221);

        /// <summary>
        /// The color used for red elements on the map.
        /// </summary>
        public static readonly GeoColor ColorRed = GeoColor.FromArgb(153, 226, 20, 54);

        /// <summary>
        /// The color used for dark red elements on the map.
        /// </summary>  
        public static readonly GeoColor ColorDarkRed = GeoColor.FromArgb(51, 145, 40, 58);

        /// <summary>
        /// The color used for medium red elements on the map.
        /// </summary>
        public static readonly GeoColor ColorMediumRed = GeoColor.FromArgb(255, 200, 26, 55);
        #endregion

        #region Geographic Constants
        /// <summary>
        /// Earth radius in meters used for Web Mercator and great-circle distance calculations.
        /// </summary>
        public const double EarthRadiusMeters = 6378137d;

        /// <summary>
        /// Total world width in Web Mercator meters.
        /// </summary>
        public const double WorldWidthInMeters = 2d * Math.PI * EarthRadiusMeters;

        /// <summary>
        /// Half of <see cref="WorldWidthInMeters"/> used for shortest-path wrap calculations.
        /// </summary>
        public const double HalfWorldWidthInMeters = WorldWidthInMeters / 2d;

        /// <summary>
        /// Maximum valid latitude for Web Mercator projection.
        /// </summary>
        public const double MaxWebMercatorLatitude = 85.0511d;

        /// <summary>
        /// Total world width in decimal degrees.
        /// </summary>
        public const double WorldWidthInDecimalDegrees = 360d;

        /// <summary>
        /// Half of <see cref="WorldWidthInDecimalDegrees"/> used for dateline wrap calculations.
        /// </summary>
        public const double HalfWorldWidthInDecimalDegrees = WorldWidthInDecimalDegrees / 2d;

        /// <summary>
        /// Unit conversion constant used for degrees-minutes-seconds formatting.
        /// </summary>
        public const double DmsUnitConversion = 60.0;

        /// <summary>
        /// Selection tolerance in pixels for hit-testing and proximity checks.
        /// </summary>
        public const double SelectionTolerancePixels = 25.0;

        /// <summary>
        /// Maximum snap distance in meters between line endpoints and placed feature centers.
        /// </summary>
        public const double LineEndpointSnapThresholdMeters = 20000d;

        /// <summary>
        /// Maximum snap distance in decimal degrees between line endpoints and placed feature centers.
        /// </summary>
        public const double LineEndpointSnapThresholdDecimalDegrees = 0.2d;

        /// <summary>
        /// Maximum segment length in kilometers used when densifying great-circle lines.
        /// </summary>
        public const double MaxLineSegmentLengthInKM = 50d;

        /// <summary>
        /// Epsilon tolerance used for coordinate equality comparisons.
        /// </summary>
        public const double CoordinateComparisonEpsilon = 1e-9;

        /// <summary>
        /// Minimum non-zero geometry component used to avoid unstable scaling and division by zero.
        /// </summary>
        public const double MinimumGeometryComponent = 1e-12;

        /// <summary>
        /// Conversion factor from square meters to square kilometers.
        /// </summary>
        public const double SquareMetersPerSquareKilometer = 1_000_000d;

        /// <summary>
        /// Distance interval in kilometers between generated line-direction arrows.
        /// </summary>
        public const double LineArrowDistanceKilometers = 500d;

        /// <summary>
        /// Multiplier used to extend world-width bounds for wrapped map extents.
        /// </summary>
        public const int MaxWorldWidthMultiplier = 10;

        #endregion

        #region Font and Style Constants
        /// <summary>
        /// Default scale offset used for icon and label sizing.
        /// </summary>
        public const double DefaultScaleOffset = 7.5;

        /// <summary>
        /// Y-axis offset for icon labels (negative places labels above icons).
        /// </summary>
        public const int IconLabelYOffset = -20;

        /// <summary>
        /// Placed-feature hitbox size in pixels for selection and endpoint snapping.
        /// </summary>
        public const double PlacedFeatureBoundingBoxPixels = 32d;

        /// <summary>
        /// Font family used for map labels.
        /// </summary>
        public const string FontName = "Inter";

        /// <summary>
        /// Font size used for map labels.
        /// </summary>
        public const float FontSize = 14;

        /// <summary>
        /// Style cache key for icon label styles.
        /// </summary>
        public const string LabelIconStyle = "LabelIconStyle";

        /// <summary>
        /// Style cache key for text label styles.
        /// </summary>
        public const string LabelTextStyle = "LabelTextStyle";

        /// <summary>
        /// Style cache key for line label text styles.
        /// </summary>
        public const string LineLabelTextStyle = "LineLabelTextStyle";

        /// <summary>
        /// Style cache key for drawn feature fill styles.
        /// </summary>
        public const string DrawnFeatureFillStyle = "DrawnFeatureFillStyle";

        /// <summary>
        /// Style cache key for drawn feature line styles.
        /// </summary>
        public const string DrawnFeatureLineStyle = "DrawnFeatureLineStyle";

        /// <summary>
        /// Style cache key for drawn feature arrow styles.
        /// </summary>
        public const string DrawnFeatureArrowStyle = "DrawnFeatureArrowStyle";

        /// <summary>
        /// Segment ratio used to allow long labels to span full line geometries.
        /// </summary>
        public const int LineLabelSegmentRatio = 1000;
        #endregion

        #region Arrows
        /// <summary>
        /// Prefix for default light-theme arrow image names.
        /// </summary>
        public const string ArrowDefaultLightPrefix = "Black_";

        /// <summary>
        /// Prefix for default dark-theme arrow image names.
        /// </summary>
        public const string ArrowDefaultDarkPrefix = "LightGray_";

        /// <summary>
        /// Arrow direction token: north.
        /// </summary>
        public const string ArrowDirectionNorth = "N";

        /// <summary>
        /// Arrow direction token: northeast.
        /// </summary>
        public const string ArrowDirectionNorthEast = "NE";

        /// <summary>
        /// Arrow direction token: east.
        /// </summary>
        public const string ArrowDirectionEast = "E";

        /// <summary>
        /// Arrow direction token: southeast.
        /// </summary>
        public const string ArrowDirectionSouthEast = "SE";

        /// <summary>
        /// Arrow direction token: south.
        /// </summary>
        public const string ArrowDirectionSouth = "S";

        /// <summary>
        /// Arrow direction token: southwest.
        /// </summary>
        public const string ArrowDirectionSouthWest = "SW";

        /// <summary>
        /// Arrow direction token: west.
        /// </summary>
        public const string ArrowDirectionWest = "W";

        /// <summary>
        /// Arrow direction token: northwest.
        /// </summary>
        public const string ArrowDirectionNorthWest = "NW";
        #endregion

        #region Layer Data Columns
        /// <summary>
        /// Column name for feature display name.
        /// </summary>
        public const string NameColumn = "Name";

        /// <summary>
        /// Column name for icon label text.
        /// </summary>
        public const string LabelNameColumn = "LabelName";

        /// <summary>
        /// Column name for line label text.
        /// </summary>
        public const string LineLabelNameColumn = "LineLabelName";

        /// <summary>
        /// Column name for placed feature icon type identifier.
        /// </summary>
        public const string IconTypeColumn = "IconType";

        /// <summary>
        /// Column name for icon asset key.
        /// </summary>
        public const string IconNameColumn = "IconName";

        /// <summary>
        /// Column name for arrow asset key.
        /// </summary>
        public const string ArrowNameColumn = "ArrowName";

        /// <summary>
        /// Column name indicating line geometry state.
        /// </summary>
        public const string IsLineColumn = "IsLine";

        /// <summary>
        /// Column name for serialized source line vertices.
        /// </summary>
        public const string LineVerticesColumn = "LineVertices";

        /// <summary>
        /// Column name for serialized generated line vertices.
        /// </summary>
        public const string GeneratedLineVerticesColumn = "GeneratedLineVertices";

        /// <summary>
        /// Column name indicating circle geometry state.
        /// </summary>
        public const string IsCircleColumn = "IsCircle";

        /// <summary>
        /// Column name for serialized circle center X coordinate.
        /// </summary>
        public const string CircleCenterXColumn = "CircleCenterX";

        /// <summary>
        /// Column name for serialized circle center Y coordinate.
        /// </summary>
        public const string CircleCenterYColumn = "CircleCenterY";

        /// <summary>
        /// Column name for circle radius.
        /// </summary>
        public const string CircleRadiusColumn = "CircleRadius";

        /// <summary>
        /// Column name for serialized source polygon vertices.
        /// </summary>
        public const string PolygonVerticesColumn = "PolygonVertices";

        /// <summary>
        /// Column name for serialized generated polygon vertices.
        /// </summary>
        public const string GeneratedPolygonVerticesColumn = "GeneratedPolygonVertices";
        #endregion
    }
}
