using ThinkGeo.UI.Blazor.HowDoI.CustomerApp.SharedProperties;

namespace ThinkGeo.UI.Blazor.HowDoI.CustomerApp.Classes
{
    /// <summary>
    /// Represents a placed icon feature on the map with identity, display metadata, and center coordinate.
    /// </summary>
    /// <remarks>
    /// Placed features are point-based markers rendered with icon styles and commonly used for facilities,
    /// assets, and other named map entities.
    /// </remarks>
    public class MapPlacedFeature
    {
        /// <summary>
        /// Gets or sets the unique feature identifier.
        /// </summary>
        /// <remarks>
        /// Typically assigned from a GUID when created, but stored as string to support external ID schemes.
        /// </remarks>
        public string Id { get; set; } = "";

        /// <summary>
        /// Gets or sets the user-facing feature name.
        /// </summary>
        /// <remarks>
        /// Used in labels, selection results, and lookup by feature name.
        /// </remarks>
        public string Name { get; set; } = "";

        /// <summary>
        /// Gets or sets the icon type identifier.
        /// </summary>
        /// <remarks>
        /// Stored as a string for compatibility with persisted schema and icon-catalog indexing.
        /// </remarks>
        public string IconType { get; set; } = "0";

        /// <summary>
        /// Gets or sets the icon asset name used for rendering this marker.
        /// </summary>
        /// <remarks>
        /// Defaults to <see cref="MapConstants.MissingIconName"/> when a specific icon is unavailable.
        /// </remarks>
        public string IconName { get; set; } = MapConstants.MissingIconName;

        /// <summary>
        /// Gets or sets the marker center coordinate in decimal degrees.
        /// </summary>
        public MapCoordinate FeatureCenter { get; set; } = new MapCoordinate(0, 0);

        /// <summary>
        /// Initializes a new instance of the <see cref="MapPlacedFeature"/> class.
        /// </summary>
        public MapPlacedFeature() { }
    }
}
