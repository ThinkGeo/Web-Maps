using ThinkGeo.UI.Blazor.HowDoI.CustomerApp.Classes;
using ThinkGeo.UI.Blazor.HowDoI.CustomerApp.SharedProperties;

namespace ThinkGeo.UI.Blazor.HowDoI.CustomerApp.Utilities
{
    /// <summary>
    /// Provides utility methods for managing line arrows on drawn features within a map context.
    /// </summary>
    /// <remarks>
    /// This utility class handles arrow placement along line features, including:
    /// - Determining when arrows should be added to a line
    /// - Calculating arrow positions along the path (preferring generated/densified vertices when available)
    /// - Computing arrow directions based on line angles using 8 compass directions (N, NE, E, SE, S, SW, W, NW)
    /// - Ensuring at least one arrow per original line segment
    /// - Spacing arrows evenly along the entire line based on requested count
    /// 
    /// Arrow placement uses coordinate epsilon comparison to handle floating-point precision, and falls back 
    /// to original vertices when generated vertices are unavailable.
    /// </remarks>
    internal class MapLineArrowUtilities
    {
        #region Constants

        /// <summary>
        /// Array of 8 compass directions for arrow orientation, indexed from North (0) through NorthWest (7).
        /// </summary>
        private static readonly string[] ArrowDirections =
        {
            MapConstants.ArrowDirectionNorth,
            MapConstants.ArrowDirectionNorthEast,
            MapConstants.ArrowDirectionEast,
            MapConstants.ArrowDirectionSouthEast,
            MapConstants.ArrowDirectionSouth,
            MapConstants.ArrowDirectionSouthWest,
            MapConstants.ArrowDirectionWest,
            MapConstants.ArrowDirectionNorthWest
        };

        /// <summary>
        /// Baseline angle (90.0 degrees) used to normalize angles for compass direction calculation.
        /// </summary>
        private const double AngleBaselineOffset = 90.0;

        /// <summary>
        /// Degrees per direction step (45.0 degrees) dividing the 360-degree circle into 8 directions.
        /// </summary>
        private const double AngleStepPerDirection = 45.0;

        /// <summary>
        /// Total number of compass directions used for arrow orientation.
        /// </summary>
        private const int DirectionCount = 8;

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether line arrows should be added for the specified drawn feature based on its properties.
        /// </summary>
        /// <param name="drawnFeature">The drawn feature to evaluate.</param>
        /// <returns>True if line arrows should be added; otherwise, false.</returns>
        public bool ShouldAddLineArrows(MapDrawnFeature drawnFeature) =>
            drawnFeature != null &&
            drawnFeature.IsLine &&
            drawnFeature.LineVertices != null &&
            drawnFeature.LineVertices.Count >= 2;

        /// <summary>
        /// Retrieves the vertices of the arrow path for the specified drawn feature, preferring generated vertices if available.
        /// </summary>
        /// <param name="drawnFeature">The drawn feature for which to retrieve arrow path vertices.</param>
        /// <returns>A list of map coordinates representing the arrow path vertices. Falls back to original line vertices if generated vertices are unavailable.</returns>
        /// <remarks>
        /// Generated vertices represent post-processing output such as densified paths that preserve geodesic curvature.
        /// This method prefers generated vertices for arrow placement to maintain visual continuity with rendered geometry.
        /// If generated vertices are unavailable or empty, falls back to the original line vertices.
        /// Uses epsilon-based coordinate comparison to remove consecutive duplicates, ensuring clean arrow placement.
        /// </remarks>
        public List<MapCoordinate> GetArrowPathVertices(MapDrawnFeature drawnFeature)
        {
            // If generated vertices are not available, return the original line vertices or an empty list.
            if (drawnFeature?.GeneratedLineVertices == null)
            {
                return drawnFeature?.LineVertices ?? new List<MapCoordinate>();
            }

            // Create a flattened list of generated vertices.
            var flattenedGeneratedVertices = new List<MapCoordinate>();

            // Flatten all segments and filter out nulls in a single LINQ chain.
            var allCoordinates = drawnFeature.GeneratedLineVertices
                .Where(segment => segment != null && segment.Count > 0)
                .SelectMany(segment => segment)
                .Where(coordinate => coordinate != null)
                .ToList();

            // De-duplicate consecutive coordinates using epsilon-based comparison to avoid adjacent duplicates.
            foreach (var coordinate in allCoordinates)
            {
                // Use AreCoordinatesEqual to compare coordinates with a tolerance for floating-point precision.
                if (flattenedGeneratedVertices.Count == 0 ||
                    !AreCoordinatesEqual(flattenedGeneratedVertices[^1], coordinate))
                {
                    // Add the coordinate if it's not a duplicate of the last added coordinate.
                    flattenedGeneratedVertices.Add(coordinate);
                }
            }

            // Return the flattened generated vertices if they contain at least two points; otherwise, fall back to the original line vertices
            // or an empty list.
            return flattenedGeneratedVertices.Count >= 2
                ? flattenedGeneratedVertices
                : drawnFeature.LineVertices ?? new List<MapCoordinate>();
        }

        /// <summary>
        /// Calculates at least one arrow index per original line segment by mapping each original vertex pair onto the selected arrow path.
        /// </summary>
        /// <param name="drawnFeature">The drawn feature containing the original line vertices.</param>
        /// <param name="arrowPathVertices">The arrow path vertices (generated vertices when available, otherwise original vertices).</param>
        /// <returns>A list of vertex indices where each original segment contributes at least one arrow. Arrows are placed at segment midpoints, avoiding endpoints.</returns>
        /// <remarks>
        /// This method ensures arrows represent all user-drawn segments, even if the arrow path is denser than the original vertices.
        /// Algorithm:
        /// 1. Maps each original line vertex to its position in the arrow path using epsilon-based coordinate equality
        /// 2. For each original segment (pair of consecutive vertices), calculates a range in the arrow path
        /// 3. Places an arrow at the midpoint of that range, ensuring it's not at segment boundaries
        /// 4. Returns indices sorted in ascending order, deduplicated via HashSet
        /// Returns empty list if mapping fails or if vertex count doesn't match.
        /// </remarks>
        public List<int> GetMinimumArrowIndicesPerLineSegment(MapDrawnFeature drawnFeature, List<MapCoordinate> arrowPathVertices)
        {
            // Use a HashSet to ensure unique indices and avoid duplicates.
            var indices = new HashSet<int>();

            // Validate that both the drawn feature and arrow path vertices are valid and contain enough points to form segments.
            if (drawnFeature?.LineVertices == null || drawnFeature.LineVertices.Count < 2 ||
                arrowPathVertices == null || arrowPathVertices.Count < 2)
            {
                // If the input is invalid, return an empty list of indices.
                return indices.ToList();
            }

            // Map each original line vertex to its corresponding index in the arrow path vertices.
            var lineVertexIndices = MapLineVerticesToArrowPathIndices(drawnFeature.LineVertices, arrowPathVertices);

            // If the mapping fails (e.g., due to coordinate mismatches), return the current indices (which may be empty).
            if (lineVertexIndices.Count != drawnFeature.LineVertices.Count)
            {
                return indices.ToList();
            }

            // Iterate through each segment defined by consecutive original line vertices.
            for (var segmentNumber = 0; segmentNumber < lineVertexIndices.Count - 1; segmentNumber++)
            {
                // Get the start and end indices of the current segment in the arrow path.
                var segmentStartIndex = lineVertexIndices[segmentNumber];
                var segmentEndIndex = lineVertexIndices[segmentNumber + 1];

                // Ensure the segment has a valid range; if not, skip to the next segment.
                if (segmentEndIndex <= segmentStartIndex)
                {
                    continue;
                }

                // Place arrow at segment midpoint avoid endpoints.
                var segmentArrowIndex = segmentStartIndex + (int)Math.Ceiling((segmentEndIndex - segmentStartIndex) / 2d);
                segmentArrowIndex = Math.Min(segmentEndIndex, Math.Max(segmentStartIndex + 1, segmentArrowIndex));
                indices.Add(segmentArrowIndex);
            }

            // Return the sorted list of unique indices where arrows should be placed along the line.
            return indices.OrderBy(i => i).ToList();
        }

        /// <summary>
        /// Calculates the indices of vertices along a line where arrows should be placed based on the total number of generated vertices and the requested number of arrows.
        /// </summary>
        /// <param name="generatedVertexCount">The total number of generated vertices along the line.</param>
        /// <param name="requestedArrowCount">The number of arrows to be placed along the line.</param>
        /// <returns>A list of vertex indices where arrows should be placed, sorted in ascending order.</returns>
        /// <remarks>
        /// Distributes arrows evenly along the line by calculating a step size (generatedVertexCount / requestedArrowCount).
        /// Ensures arrows are placed:
        /// - At least one vertex away from line start (index >= 1)
        /// - At least one vertex away from line end (index <= lastVertexIndex - 1)
        /// - At positions: step*1, step*2, ..., step*(requestedArrowCount-1)
        /// If only one arrow is requested, places it at the last valid vertex index.
        /// Returns deduplicated, sorted indices via HashSet and OrderBy.
        /// </remarks>
        public List<int> GetArrowVertexIndices(int generatedVertexCount, int requestedArrowCount)
        {
            // Set the last vertex index to the last valid index in the generated vertices.
            var lastVertexIndex = generatedVertexCount - 1;

            // Use a HashSet to ensure unique indices and avoid duplicates.
            var indices = new HashSet<int>();

            // If there are no valid vertices, return an empty list.
            if (lastVertexIndex < 1)
            {
                return indices.ToList();
            }

            // If only one arrow is requested, place it at the last vertex index.
            if (requestedArrowCount > 1)
            {
                // Calculate the step size between arrows based on the total number of generated vertices and the requested number of arrows.
                var vertexStep = generatedVertexCount / (double)requestedArrowCount;

                // Place arrows at calculated indices along the line, ensuring they are within valid bounds and avoiding endpoints.
                for (var arrowNumber = 1; arrowNumber < requestedArrowCount; arrowNumber++)
                {
                    // Calculate the index for the current arrow based on the step size and ensure it is within valid bounds.
                    var index = (int)Math.Floor(arrowNumber * vertexStep);

                    // Ensure the index is at least 1 and at most lastVertexIndex - 1 to avoid placing arrows at the endpoints.
                    index = Math.Min(lastVertexIndex - 1, Math.Max(1, index));

                    // Add the calculated index to the set of indices where arrows should be placed.
                    indices.Add(index);
                }
            }

            // If no indices were added (e.g., if requestedArrowCount was 1), ensure at least one arrow is placed at the last vertex index.
            if (indices.Count == 0)
            {
                indices.Add(lastVertexIndex);
            }

            // Return the sorted list of unique indices where arrows should be placed along the line.
            return indices.OrderBy(i => i).ToList();
        }

        /// <summary>
        /// Determines the compass direction name for an arrow based on the line's angle at that point.
        /// </summary>
        /// <param name="angle">The angle in degrees (-180 to 180) used to determine direction. Typically computed from line segment tangent.</param>
        /// <returns>The compass direction name (North, NorthEast, East, SouthEast, South, SouthWest, West, NorthWest).</returns>
        /// <remarks>
        /// Converts a continuous angle to one of 8 discrete compass directions by:
        /// 1. Normalizing angle relative to AngleBaselineOffset (90 degrees) to align with compass convention
        /// 2. Dividing into 45-degree steps (AngleStepPerDirection)
        /// 3. Using modulo arithmetic with pattern ((x % 8) + 8) % 8 to handle negative angles correctly
        /// Compass directions are indexed 0-7: N(0), NE(1), E(2), SE(3), S(4), SW(5), W(6), NW(7).
        /// </remarks>
        public string GetArrowDirection(double angle)
        {
            // Normalize angle to 0-7 range (8 directions). The ((x % 8) + 8) % 8 pattern handles negative modulo correctly.
            var index = ((int)Math.Round((AngleBaselineOffset - angle) / AngleStepPerDirection) % DirectionCount + DirectionCount) % DirectionCount;

            // Return the arrow direction corresponding to the calculated index.
            return ArrowDirections[index];
        }

        #endregion

        #region Internal Helpers

        /// <summary>
        /// Maps each original line vertex to its corresponding index in the arrow path vertices, ensuring they align closely based on coordinate equality.
        /// </summary>
        /// <param name="lineVertices">The original line vertices to map (from user-drawn geometry).</param>
        /// <param name="arrowPathVertices">The arrow path vertices to search within (may be generated/densified).</param>
        /// <returns>A list of indices corresponding to the arrow path positions of each original line vertex. Empty list if any mapping fails.</returns>
        /// <remarks>
        /// Uses FindIndex with monotonic forward search from the last matched position, assuming arrow path is in order.
        /// Coordinate matching uses epsilon-based AreCoordinatesEqual to handle floating-point precision.
        /// If any original vertex cannot be found in the arrow path, returns an empty list (indicating mapping failure).
        /// This ensures all original line segments are represented when calculating minimum arrows per segment.
        /// </remarks>
        private List<int> MapLineVerticesToArrowPathIndices(List<MapCoordinate> lineVertices, List<MapCoordinate> arrowPathVertices)
        {
            // Initialize a list to hold the mapped indices of line vertices in the arrow path.
            var mappedIndices = new List<int>();

            // Validate input lists to ensure they are not null and contain vertices to map.
            if (lineVertices == null || arrowPathVertices == null || lineVertices.Count == 0 || arrowPathVertices.Count == 0)
            {
                return mappedIndices;
            }

            // Initialize the starting index.
            var searchStartIndex = 0;

            // Iterate through each line vertex to find its corresponding index in the arrow path vertices.
            foreach (var lineVertex in lineVertices)
            {
                // Search monotonically forward from the last matched index (assumes arrow path is in order).
                var matchedIndex = arrowPathVertices.FindIndex(searchStartIndex,
                    vertex => AreCoordinatesEqual(lineVertex, vertex));

                // If no match is found, return an empty list indicating mapping failure.
                if (matchedIndex < 0)
                {
                    return new List<int>();
                }

                // Add the matched index to the list of mapped indices and update the search start index for the next iteration.
                mappedIndices.Add(matchedIndex);
                searchStartIndex = matchedIndex;
            }

            // Return the list of mapped indices corresponding to the original line vertices in the arrow path.
            return mappedIndices;
        }

        /// <summary>
        /// Compares two map coordinates for equality using a tolerance-based epsilon comparison for floating-point coordinates.
        /// </summary>
        /// <param name="first">The first coordinate to compare.</param>
        /// <param name="second">The second coordinate to compare.</param>
        /// <returns>True if both coordinates are non-null and their latitude and longitude differ by less than MapConstants.CoordinateComparisonEpsilon; otherwise, false.</returns>
        /// <remarks>
        /// Uses epsilon-based comparison (MapConstants.CoordinateComparisonEpsilon) rather than exact equality to account for
        /// floating-point rounding errors that arise during coordinate transformations (e.g., densification, dateline wrapping).
        /// Checks both null references and coordinate component differences independently before returning true.
        /// </remarks>
        private static bool AreCoordinatesEqual(MapCoordinate first, MapCoordinate second)
        {
            // Check for null coordinates and compare their latitude and longitude using a small epsilon value to account for floating-point
            // precision issues.
            return first != null &&
                   second != null &&
                   System.Math.Abs(first.Longitude - second.Longitude) < MapConstants.CoordinateComparisonEpsilon &&
                   System.Math.Abs(first.Latitude - second.Latitude) < MapConstants.CoordinateComparisonEpsilon;
        }
        #endregion
    }
}

