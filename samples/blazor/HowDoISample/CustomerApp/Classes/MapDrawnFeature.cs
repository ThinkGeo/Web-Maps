using System.Xml.Serialization;

namespace ThinkGeo.UI.Blazor.HowDoI.CustomerApp.Classes
{
    /// <summary>
    /// Represents a user-drawn map feature, including lines, polygons, and circles with derived measurement metadata.
    /// </summary>
    /// <remarks>
    /// This model is persisted and restored as part of map state. Generated geometry collections are runtime-only
    /// projections used for rendering/interaction and are excluded from XML serialization.
    /// </remarks>
    public class MapDrawnFeature
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
        /// Used in map labels, selection readouts, and feature lookup by name.
        /// </remarks>
        public string Name { get; set; } = "";

        /// <summary>
        /// Gets or sets whether this drawn feature is a line.
        /// </summary>
        public bool IsLine { get; set; } = false;

        /// <summary>
        /// The total great-circle distance of this line feature in kilometers.
        /// For non-line features, this value remains 0.
        /// </summary>
        public double LineDistance { get; set; } = 0d;

        /// <summary>
        /// The placed-feature name at the line start vertex.
        /// For non-line features or unmatched endpoints, this value is empty.
        /// </summary>
        public string FromFeature { get; set; } = "";

        /// <summary>
        /// The placed-feature name at the line end vertex.
        /// For non-line features or unmatched endpoints, this value is empty.
        /// </summary>
        public string ToFeature { get; set; } = "";

        /// <summary>
        /// Gets or sets the source vertices defining the line geometry.
        /// </summary>
        public List<MapCoordinate> LineVertices { get; set; } = new List<MapCoordinate>();

        /// <summary>
        /// Gets or sets runtime-generated line segments derived from source vertices.
        /// </summary>
        /// <remarks>
        /// Used for densification and dateline-safe rendering. Not persisted to XML.
        /// </remarks>
        [XmlIgnore]
        public List<List<MapCoordinate>> GeneratedLineVertices { get; set; } = new List<List<MapCoordinate>>(); 

        /// <summary>
        /// Gets or sets whether this drawn feature is a circle.
        /// </summary>
        public bool IsCircle { get; set; } = false;

        /// <summary>
        /// The radius of the circle, if the drawn shape is a circle in meters.
        /// The default value is set to 0.0, indicating that the circle has no radius defined yet.
        /// </summary>
        public double CircleRadius { get; set; } = 0.0;

        /// <summary>
        /// The area of the drawn shape in square kilometers. For circles, this is calculated based on the radius, 
        /// and for polygons, it is calculated based on the vertices.
        /// </summary>
        public double ShapeArea { get; set; } = 0.0;
        
        /// <summary>
        /// Gets or sets the circle center coordinate in decimal degrees.
        /// </summary>
        public MapCoordinate CircleCenter { get; set; } = new MapCoordinate(0, 0);

        /// <summary>
        /// Gets or sets the source vertices defining polygon geometry.
        /// </summary>
        /// <remarks>
        /// Polygon ring closure may be normalized during conversion/serialization workflows.
        /// </remarks>
        public List<MapCoordinate> PolygonVertices { get; set; } = new List<MapCoordinate>();

        /// <summary>
        /// Gets or sets runtime-generated polygon segments used for wrapped rendering.
        /// </summary>
        /// <remarks>
        /// Represents dateline-split or otherwise post-processed polygon geometry. Not persisted to XML.
        /// </remarks>
        [XmlIgnore]
        public List<List<MapCoordinate>> GeneratedPolygonVertices { get; set; } = new List<List<MapCoordinate>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MapDrawnFeature"/> class.
        /// </summary>
        public MapDrawnFeature() { }
    }
}
