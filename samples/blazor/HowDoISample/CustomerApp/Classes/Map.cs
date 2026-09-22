using System.Collections.ObjectModel;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Blazor.HowDoI.CustomerApp.Classes
{
    /// <summary>
    /// Represents persisted map state including view position and feature collections.
    /// </summary>
    /// <remarks>
    /// This model is serialized to XML and restored at load time to preserve map viewport context
    /// and user-authored placed/drawn features across sessions.
    /// </remarks>
    [Serializable]
    public class Map
    {
        /// <summary>
        /// Gets or sets the center point of the map view in decimal degrees.
        /// </summary>
        /// <remarks>
        /// Defaults to (176.00, 11.00), corresponding to the INDOPACOM-focused startup extent.
        /// </remarks>
        public PointShape MapCenterPoint { get; set; } = new PointShape(176.00, 11.00);

        /// <summary>
        /// Gets or sets the map zoom level.
        /// </summary>
        /// <remarks>
        /// Defaults to 3 for a broad overview. Runtime logic clamps zoom to configured min/max limits.
        /// </remarks>
        public int MapZoomLevel { get; set; } = 3;

        /// <summary>
        /// Gets or sets the placed icon features on the map.
        /// </summary>
        /// <remarks>
        /// Contains point-based entities (for example facilities and assets) rendered with icon styles.
        /// </remarks>
        public Collection<MapPlacedFeature> MapPlacedFeatures { get; set; } = new Collection<MapPlacedFeature>();

        /// <summary>
        /// Gets or sets the drawn geometry features on the map.
        /// </summary>  
        /// <remarks>
        /// Contains user-authored lines, polygons, and circles including associated measurement metadata.
        /// </remarks>
        public Collection<MapDrawnFeature> MapDrawnFeatures { get; set; } = new Collection<MapDrawnFeature>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Map"/> class.
        /// </summary>
        public Map() { }
    }
}
