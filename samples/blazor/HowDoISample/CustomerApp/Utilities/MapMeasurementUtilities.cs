using ThinkGeo.UI.Blazor.HowDoI.CustomerApp.Classes;
using ThinkGeo.UI.Blazor.HowDoI.CustomerApp.SharedProperties;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Blazor.HowDoI.CustomerApp.Utilities
{
    /// <summary>
    /// Provides geometry measurement helpers for line distance, polygon/circle area, and wrapped map-space proximity calculations.
    /// </summary>
    /// <remarks>
    /// This utility focuses on measurement behavior and delegates coordinate-system normalization/conversion details
    /// to <see cref="MapGeographicUtilities"/>. It supports both great-circle (geographic) and map-space (projected)
    /// workflows so map interaction logic can consistently evaluate distance and area across zoom levels and dateline crossings.
    /// </remarks>
    internal class MapMeasurementUtilities
    {
        #region Dependencies
        /// <summary>
        /// The Settings instance that holds the current map view, map class, and other related settings.
        /// </summary>
        public MapSettings RuntimeState { get; set; } = new MapSettings();

        /// <summary>
        /// Geographic utility dependency used for coordinate normalization and map-space conversion helpers.
        /// </summary>
        // This class deliberately delegates coordinate-system concerns to MapGeographicUtilities
        // so measurement code can stay focused on distance, area, and wrapped geometry math.
        public MapGeographicUtilities MapGeoUtilities { get; set; } = new MapGeographicUtilities();
        #endregion

        #region Great-Circle and Line Measurement Methods
        /// <summary>
        /// Densifies a line using great-circle interpolation so rendered segments follow earth curvature.
        /// </summary>
        /// <param name="coordinates">Line coordinates in decimal degrees (latitude/longitude).</param>
        /// <returns>A densified list of coordinates in decimal degrees.</returns>
        /// <remarks>
        /// Input coordinates are normalized for dateline continuity before interpolation.
        /// This method is intended for visual and measurement accuracy, not for mutating authored source vertices.
        /// </remarks>
        public List<MapCoordinate> DensifyGreatCircle(IReadOnlyList<MapCoordinate> coordinates)
        {
            // Densification is only needed for lines with two or more vertices; single points are already fully defined.
            if (coordinates == null || coordinates.Count < 2)
            {
                return coordinates?.ToList() ?? new List<MapCoordinate>();
            }

            // Normalize first so great-circle interpolation follows the intended path even when the line crosses the dateline.
            // Without this, a segment that crosses +/-180 longitude can be interpreted as a nearly world-spanning segment
            // in the wrong direction, which would distort both the interpolated path and the arrow-placement math that
            // later depends on these display vertices.
            var normalized = MapGeoUtilities.NormalizeMapCoordinatesAcrossDateLine(coordinates);

            // Densify each segment of the normalized line so the rendered path approximates a great-circle arc.
            var densifiedCoords = new List<MapCoordinate>();

            // Loop through each segment of the normalized line and interpolate additional points along the great-circle arc.
            for (int i = 0; i < normalized.Count - 1; i++)
            {
                // Initialize the segment endpoints and compute the great-circle distance in kilometers.
                var start = normalized[i];
                var end = normalized[i + 1];
                var angularDistance = GetGreatCircleAngularDistance(start, end);
                var distanceKm = MapConstants.EarthRadiusMeters * angularDistance / 1000d;

                // Break long arcs into smaller rendered segments so curved lines stay visually smooth.
                // The source line still represents the authored control points; this densified list is strictly a
                // display/measurement helper that approximates the great-circle curve with short segments.
                var segmentCount = Math.Max(1, (int)Math.Ceiling(distanceKm / MapConstants.MaxLineSegmentLengthInKM));

                // Iterate through each sub-segment and interpolate the corresponding point along the great-circle arc.
                for (int s = 0; s < segmentCount; s++)
                {
                    // Compute the interpolation factor in the range [0, 1] for the current sub-segment.
                    var factor = (double)s / segmentCount;

                    // Interpolate the point along the great-circle arc and add it to the densified list if it's not a duplicate of the last point.
                    var point = InterpolateGreatCircle(start, end, factor);

                    // Avoid adding duplicate points to the densified list, which can occur when the interpolation factor is 0 or 1.
                    if (densifiedCoords.Count == 0 || !MapGeoUtilities.CoordinatesEqual(densifiedCoords[^1], point))
                    {
                        // Add the interpolated point to the densified list.
                        densifiedCoords.Add(point);
                    }
                }
            }

            // Ensure the last point of the original normalized line is included in the densified list.
            var last = normalized[^1];

            // Avoid adding a duplicate of the last point if it was already added during the densification loop.
            if (densifiedCoords.Count == 0 || !MapGeoUtilities.CoordinatesEqual(densifiedCoords[^1], last))
            {
                // Add the last point to the densified list.
                densifiedCoords.Add(last);
            }

            // Return the densified list of coordinates that approximates the great-circle path.
            return densifiedCoords;
        }

        /// <summary>
        /// Gets the great-circle distance in kilometers between two coordinates.
        /// </summary>
        /// <param name="start">The starting coordinate.</param>
        /// <param name="end">The ending coordinate.</param>
        /// <returns>The great-circle distance in kilometers between the two coordinates.</returns>
        /// <remarks>
        /// Uses spherical angular distance and earth radius constants rather than planar projection distance.
        /// </remarks>
        public double GetGreatCircleDistanceKilometers(MapCoordinate start, MapCoordinate end)
        {
            // Return 0 for null coordinates to avoid exceptions and indicate no distance can be computed.
            if (start == null || end == null)
            {
                return 0d;
            }

            // Great-circle distance measures the shortest path over the earth's surface, which is the correct
            // interpretation for strategic/global routes rather than flat planar distance in the current projection.
            var angularDistance = GetGreatCircleAngularDistance(start, end);
            return MapConstants.EarthRadiusMeters * angularDistance / 1000d;
        }

        /// <summary>
        /// Gets the total great-circle distance of a line feature in kilometers.
        /// </summary>
        /// <param name="drawnFeature">The drawn feature representing a line.</param>
        /// <returns>The total great-circle distance of the line in kilometers.</returns>
        /// <remarks>
        /// Iterates each consecutive vertex pair and accumulates segment distances.
        /// Non-line features or insufficient vertices return 0.
        /// </remarks>
        public double CalculateLineDistanceKilometers(MapDrawnFeature drawnFeature)
        {
            // Validate that the drawn feature is a line with at least two vertices; otherwise, return 0.
            if (drawnFeature == null || !drawnFeature.IsLine || drawnFeature.LineVertices == null || drawnFeature.LineVertices.Count < 2)
            {
                return 0d;
            }

            // Initialize the total line distance accumulator to 0 kilometers.
            var lineDistanceKilometers = 0d;

            // Loop through each consecutive pair of vertices in the line and accumulate the great-circle distance.
            for (var i = 1; i < drawnFeature.LineVertices.Count; i++)
            {
                // Accumulate the great-circle distance between consecutive vertices to compute the total line distance.
                lineDistanceKilometers += GetGreatCircleDistanceKilometers(
                    drawnFeature.LineVertices[i - 1],
                    drawnFeature.LineVertices[i]);
            }

            // Return the total great-circle distance of the line in kilometers.
            return lineDistanceKilometers;
        }

        /// <summary>
        /// Interpolates a coordinate between two coordinates along a great-circle arc.
        /// </summary>
        /// <param name="start">The starting coordinate.</param>
        /// <param name="end">The ending coordinate.</param>
        /// <param name="factor">The interpolation factor in the range [0, 1].</param>
        /// <returns>The interpolated coordinate along the great-circle arc.</returns>
        /// <remarks>
        /// This is a null-safe wrapper around the internal interpolation routine and preserves stable defaults for callers.
        /// </remarks>
        public MapCoordinate InterpolateGreatCircleCoordinate(MapCoordinate start, MapCoordinate end, double factor)
        {
            // Return a default coordinate if the start coordinate is null to avoid exceptions and indicate no interpolation can be performed.
            if (start == null)
            {
                return new MapCoordinate(0, 0);
            }

            // Return the start coordinate if the end coordinate is null to avoid exceptions and indicate no interpolation can be performed.
            if (end == null)
            {
                return new MapCoordinate(start.Latitude, start.Longitude);
            }

            // Interpolate the coordinate along the great-circle arc using the specified factor and return the result.
            return InterpolateGreatCircle(start, end, factor);
        }
        #endregion

        #region Area Measurement Methods
        /// <summary>
        /// Calculates the area of a drawn circle or polygon in square kilometers.
        /// </summary>
        /// <param name="drawnFeature">The drawn feature representing a circle or polygon.</param>
        /// <returns>The area of the shape in square kilometers.</returns>
        /// <remarks>
        /// Circle area uses Euclidean radius math (radius stored in meters); polygon area uses spherical accumulation.
        /// </remarks>
        public double CalculateShapeAreaSquareKilometers(MapDrawnFeature drawnFeature)
        {
            // Validate that the drawn feature is not null; otherwise, return 0.
            if (drawnFeature == null)
            {
                return 0d;
            }

            // Handle circle features by calculating the area using the radius.
            if (drawnFeature.IsCircle)
            {
                // Return 0 for non-positive radius to avoid invalid area calculations.
                if (drawnFeature.CircleRadius <= 0d)
                {
                    return 0d;
                }

                // CircleRadius is stored in meters, so compute the area in square meters first and then convert to km^2.
                // Circle features are already defined in projected map units, so this is a simple Euclidean area calculation.
                var circleAreaSquareMeters = Math.PI * drawnFeature.CircleRadius * drawnFeature.CircleRadius;
                return circleAreaSquareMeters / MapConstants.SquareMetersPerSquareKilometer;
            }

            // Polygons are stored as geographic coordinates, so use the spherical-area helper rather than a planar shoelace formula.
            if (drawnFeature.IsLine || drawnFeature.PolygonVertices == null || drawnFeature.PolygonVertices.Count < 3)
            {
                return 0d;
            }

            // Use the spherical-area helper to compute the area of the polygon in square kilometers.
            return CalculatePolygonAreaSquareKilometers(drawnFeature.PolygonVertices);
        }
        #endregion

        #region Map Space and Wrapped Geometry Helpers
        /// <summary>
        /// Converts a decimal-degree coordinate to a map-space point in the current map unit.
        /// </summary>
        /// <param name="coordinate">The coordinate in decimal degrees (latitude/longitude).</param>    
        /// <returns>A PointShape representing the coordinate in map-space units.</returns>
        /// <remarks>
        /// When map units are meters, longitude wrapping is preserved to avoid discontinuities for dateline-crossing geometry.
        /// </remarks>
        public PointShape ConvertMapCoordinateToMapPoint(MapCoordinate coordinate)
        {
            // Return a default point if the coordinate is null to avoid exceptions and indicate no conversion can be performed.
            if (coordinate == null)
            {
                return new PointShape();
            }

            // For meter map units, convert the coordinate to meters while preserving unwrapped longitude to avoid dateline-crossing artifacts.
            if (RuntimeState.ActiveMapView.MapUnit == GeographyUnit.Meter)
            {
                // Preserve unwrapped longitude so dateline-crossing line work remains continuous in projected map space.
                // Standard Web Mercator conversion normally normalizes longitude into -180..180; preserving the unwrapped
                // longitude avoids introducing a fake jump in X when a route intentionally continues past the dateline.
                var meters = MapGeoUtilities.ConvertMapCoordinateToMetersPreserveLongitude(coordinate);
                return new PointShape(meters.Longitude, meters.Latitude);
            }

            // For decimal-degree map units, simply return the coordinate as a PointShape without conversion.
            return new PointShape(coordinate.Longitude, coordinate.Latitude);
        }

        /// <summary>
        /// Computes squared planar distance between two points while honoring world wrap on the X-axis.
        /// </summary>
        /// <param name="from">The starting point in map-space units.</param>   
        /// <param name="to">The ending point in map-space units.</param>
        /// <returns>The squared distance between the two points in map-space units.</returns>
        /// <remarks>
        /// Returns squared distance intentionally to avoid repeated square root overhead in threshold comparisons.
        /// </remarks>
        public double GetWrappedDistanceSquared(PointShape from, PointShape to)
        {
            // Use squared distance because callers only compare relative distances and thresholds.
            // Avoiding the square root keeps repeated endpoint and proximity checks cheaper while preserving ordering.
            var dx = Math.Abs(GetWrappedDeltaX(from.X, to.X));
            var dy = to.Y - from.Y;
            return (dx * dx) + (dy * dy);
        }

        /// <summary>
        /// Computes the shortest signed X-axis delta between two points while honoring world wrap.
        /// </summary>
        /// <param name="fromX">The starting X coordinate in map-space units.</param>
        /// <param name="toX">The ending X coordinate in map-space units.</param>
        /// <returns>The shortest signed delta between the two X coordinates in map-space units.</returns>
        /// <remarks>
        /// Chooses the shortest horizontal path across the wrapped world extent for the active map unit.
        /// </remarks>
        public double GetWrappedDeltaX(double fromX, double toX)
        {
            // Determine the world width and half-width in the current map unit to handle wrapping correctly.
            var mapUnit = RuntimeState.ActiveMapView.MapUnit;

            // The world width is the total horizontal extent of the map in the current unit (meters or decimal degrees).
            var worldWidth = mapUnit == GeographyUnit.Meter
                ? MapConstants.WorldWidthInMeters
                : MapConstants.WorldWidthInDecimalDegrees;

            // The half-world width is used to determine if the delta should wrap around the world or not.
            var halfWorldWidth = mapUnit == GeographyUnit.Meter
                ? MapConstants.HalfWorldWidthInMeters
                : MapConstants.HalfWorldWidthInDecimalDegrees;

            // Use the shortest horizontal path across the wrapped world instead of the raw X delta.
            var dx = toX - fromX;

            // If the delta exceeds half the world width, wrap it around to the other side of the world.
            if (dx > halfWorldWidth)
            {
                dx -= worldWidth;
            }
            // If the delta is less than negative half the world width, wrap it around to the other side of the world.
            else if (dx < -halfWorldWidth)
            {
                dx += worldWidth;
            }

            // Return the wrapped delta, which represents the shortest signed distance between the two X coordinates in map-space units.
            return dx;
        }

        /// <summary>
        /// Projects a point from the placed-feature center to the edge of its square hitbox in the direction of the line.
        /// </summary>
        /// <param name="adjacentPoint">The adjacent point in map-space units that defines the direction of the line.</param>
        /// <param name="centerPoint">The center point of the placed feature in map-space units.</param>
        /// <param name="halfSizeWorld">The half-size of the placed feature's square hitbox in map-space units.</param>
        /// <returns>The point on the edge of the placed feature's square hitbox in the direction of the line.</returns>
        /// <remarks>
        /// Used to anchor line endpoints at marker edges instead of marker centers, improving visual alignment.
        /// </remarks>
        public PointShape GetPlacedFeatureTouchPoint(PointShape centerPoint, PointShape adjacentPoint, double halfSizeWorld)
        {
            // Return a default point if the center point is null to avoid exceptions and indicate no touch point can be computed.
            if (centerPoint == null)
            {
                return new PointShape();
            }

            // If the adjacent point is null or the half-size is non-positive, return the center point as the touch point.
            if (adjacentPoint == null || halfSizeWorld <= 0d)
            {
                return centerPoint;
            }

            // Work from the icon center toward the neighboring line vertex so the line visually terminates at the
            // feature boundary instead of the feature center.
            var dx = GetWrappedDeltaX(centerPoint.X, adjacentPoint.X);
            var dy = adjacentPoint.Y - centerPoint.Y;

            //Set the maximum component to the larger of the absolute values of dx and dy.
            //This is used to scale the touch point to the edge of the square hitbox.
            var maxComponent = Math.Max(Math.Abs(dx), Math.Abs(dy));

            // If the maximum component is too small, return the center point to avoid division by zero or unstable scaling.
            if (maxComponent < MapConstants.MinimumGeometryComponent)
            {
                return centerPoint;
            }

            // Scale against the dominant axis so the result lands on the square icon hitbox boundary.
            var scale = halfSizeWorld / maxComponent;
            var touchPoint = new PointShape(centerPoint.X + (dx * scale), centerPoint.Y + (dy * scale));
            return MapGeoUtilities.AdjustObjectPointFromWorldMap(touchPoint);
        }
        #endregion

        #region Great-Circle Private Helpers
        /// <summary>
        /// Interpolates between two coordinates along a great-circle arc.
        /// </summary>
        /// <param name="start">The starting coordinate of the great-circle arc.</param>
        /// <param name="end">The ending coordinate of the great-circle arc.</param>
        /// <param name="factor">The interpolation factor, where 0 returns the start coordinate and 1 returns the end coordinate.</param>
        /// <returns>The interpolated coordinate along the great-circle arc.</returns>
        private MapCoordinate InterpolateGreatCircle(MapCoordinate start, MapCoordinate end, double factor)
        {
            // Handle edge cases where the interpolation factor is outside the [0, 1] range.
            if (factor <= 0d)
            {
                return new MapCoordinate(ClampLatitude(start.Latitude), start.Longitude);
            }

            // If the factor is greater than or equal to 1, return the end coordinate to avoid extrapolation.
            if (factor >= 1d)
            {
                return new MapCoordinate(ClampLatitude(end.Latitude), end.Longitude);
            }

            // Convert the start and end coordinates from degrees to radians for spherical calculations.
            var startLatRad = DegreesToRadians(ClampLatitude(start.Latitude));
            var startLonRad = DegreesToRadians(start.Longitude);
            var endLatRad = DegreesToRadians(ClampLatitude(end.Latitude));
            var endLonRad = DegreesToRadians(end.Longitude);

            // Calculate the angular distance between the start and end coordinates using the haversine formula.
            var angularDistance = GetGreatCircleAngularDistance(start, end);

            // Handle extremely short segments by treating them as degenerate to avoid unstable division by sin(omega).
            if (angularDistance < MapConstants.MinimumGeometryComponent)
            {
                // Extremely short segments are treated as degenerate to avoid unstable division by sin(omega).
                return new MapCoordinate(ClampLatitude(start.Latitude), start.Longitude);
            }

            // Interpolate on the sphere in Cartesian space, then convert back to latitude/longitude.
            // This is spherical linear interpolation, which preserves the great-circle route between the endpoints.
            // Compute the sine of the angular distance to use in the interpolation coefficients.
            var sinOmega = Math.Sin(angularDistance);

            // Compute the interpolation coefficients for the start and end coordinates based on the factor and angular distance.
            var a = Math.Sin((1d - factor) * angularDistance) / sinOmega;
            var b = Math.Sin(factor * angularDistance) / sinOmega;

            // Convert the spherical coordinates to Cartesian coordinates for interpolation.
            var x = a * Math.Cos(startLatRad) * Math.Cos(startLonRad) + b * Math.Cos(endLatRad) * Math.Cos(endLonRad);
            var y = a * Math.Cos(startLatRad) * Math.Sin(startLonRad) + b * Math.Cos(endLatRad) * Math.Sin(endLonRad);
            var z = a * Math.Sin(startLatRad) + b * Math.Sin(endLatRad);

            // Convert the interpolated Cartesian coordinates back to spherical coordinates (latitude and longitude).
            var lat = Math.Atan2(z, Math.Sqrt(x * x + y * y));
            var lon = Math.Atan2(y, x);

            // Keep the interpolated longitude continuous with the source segment so dateline-crossing arcs do not flip.
            var interpolatedLon = RadiansToDegrees(lon);

            // Compute a reference longitude based on the start and end coordinates to ensure continuity across the dateline.
            var referenceLon = start.Longitude + (end.Longitude - start.Longitude) * factor;

            // Keep the interpolated longitude continuous with the source segment so dateline-crossing arcs do not flip.
            // This ensures that the interpolated longitude remains within the same hemisphere as the reference longitude.
            // The while loops adjust the interpolated longitude to be within half the world width of the reference longitude.
            // This prevents sudden jumps in longitude when interpolating across the dateline.

            // If the interpolated longitude is more than half the world width east of the reference, wrap it west.
            while (interpolatedLon - referenceLon > MapConstants.HalfWorldWidthInDecimalDegrees)
            {
                // If the interpolated longitude is more than half the world width east of the reference, wrap it west.
                interpolatedLon -= MapConstants.WorldWidthInDecimalDegrees;
            }

            // If the interpolated longitude is more than half the world width west of the reference, wrap it east.
            while (interpolatedLon - referenceLon < -MapConstants.HalfWorldWidthInDecimalDegrees)
            {
                // If the interpolated longitude is more than half the world width west of the reference, wrap it east.
                interpolatedLon += MapConstants.WorldWidthInDecimalDegrees;
            }

            // Return the interpolated coordinate along the great-circle arc, clamping the latitude to the valid range.
            return new MapCoordinate(
                ClampLatitude(RadiansToDegrees(lat)),
                interpolatedLon);
        }

        /// <summary>
        /// Gets the angular great-circle distance in radians between two coordinates.
        /// </summary>
        /// <param name="start">The starting coordinate.</param>
        /// <param name="end">The ending coordinate.</param>
        /// <returns>The angular great-circle distance in radians.</returns>
        private double GetGreatCircleAngularDistance(MapCoordinate start, MapCoordinate end)
        {
            // This uses the haversine form to keep the angular-distance calculation stable for long global segments.
            // The result is an angle in radians on the earth sphere; callers convert that into kilometers using earth radius.

            // Start and end latitudes are clamped to the valid Web Mercator range to avoid invalid calculations.
            var startLatRad = DegreesToRadians(ClampLatitude(start.Latitude));
            var endLatRad = DegreesToRadians(ClampLatitude(end.Latitude));

            // Compute the differences in latitude and longitude in radians for the haversine formula.
            var deltaLatRad = endLatRad - startLatRad;
            var deltaLonRad = DegreesToRadians(end.Longitude - start.Longitude);

            // Compute the haversine of the latitude and longitude differences to calculate the great-circle distance.
            var sinHalfDeltaLat = Math.Sin(deltaLatRad / 2d);
            var sinHalfDeltaLon = Math.Sin(deltaLonRad / 2d);

            // Compute the haversine formula components to calculate the great-circle distance.
            var a = sinHalfDeltaLat * sinHalfDeltaLat +
                    Math.Cos(startLatRad) * Math.Cos(endLatRad) * sinHalfDeltaLon * sinHalfDeltaLon;
            
            // Clamp the value of 'a' to the range [0, 1] to avoid invalid input to Math.Sqrt and Math.Atan2.
            var clampedA = Math.Min(1d, Math.Max(0d, a));

            // Return the angular distance in radians using the haversine formula, which is suitable for spherical calculations.
            return 2d * Math.Atan2(Math.Sqrt(clampedA), Math.Sqrt(1d - clampedA));
        }
        #endregion

        #region Area Private Helpers
        /// <summary>
        /// Calculates the area of a polygon in square kilometers.
        /// </summary>
        /// <param name="polygonVertices">The vertices of the polygon in decimal degrees (latitude/longitude).</param>
        /// <returns>The area of the polygon in square kilometers.</returns>
        private double CalculatePolygonAreaSquareKilometers(IReadOnlyList<MapCoordinate> polygonVertices)
        {
            // Validate that the polygon has at least three vertices; otherwise, return 0 as the area cannot be computed.
            if (polygonVertices == null || polygonVertices.Count < 3)
            {
                return 0d;
            }

            // Create a list of polygon vertices.
            var vertices = new List<MapCoordinate>(polygonVertices);

            // If not the start vertex and end vertex do not equal, add the start vertex. 
            if (!MapGeoUtilities.CoordinatesEqual(vertices[0], vertices[^1]))
            {
                vertices.Add(vertices[0]);
            }

            // Set the initial sum to 0.
            var sum = 0d;

            // Accumulate spherical excess using latitude bands and wrapped longitude deltas.
            // This produces a globe-aware area measurement that remains valid for large polygons and dateline-adjacent shapes.
            for (var i = 0; i < vertices.Count - 1; i++)
            {
                // Get the current vertex.
                var current = vertices[i];

                // Get the next vertex. 
                var next = vertices[i + 1];

                // Convert the latitude values from degrees to radians. 
                var lat1 = DegreesToRadians(current.Latitude);
                var lat2 = DegreesToRadians(next.Latitude);

                // Convert the longitude values from degrees to radians.
                var lon1 = DegreesToRadians(current.Longitude);
                var lon2 = DegreesToRadians(next.Longitude);

                // Compute the wrapped longitude delta in radians, normalizing it to the range [-PI, PI] to handle dateline crossings correctly.
                var deltaLon = NormalizeRadians(lon2 - lon1);

                // Accumulate the area using the spherical excess formula.
                sum += deltaLon * (2d + Math.Sin(lat1) + Math.Sin(lat2));
            }

            // Convert the accumulated sum to square meters using the earth's radius and then convert to square kilometers.
            var areaSquareMeters = Math.Abs(sum) * MapConstants.EarthRadiusMeters * MapConstants.EarthRadiusMeters / 2d;
            return areaSquareMeters / MapConstants.SquareMetersPerSquareKilometer;
        }
        #endregion

        #region Math Constants, Functions and Methods
        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">The angle in degrees.</param>
        /// <returns>The angle in radians.</returns>
        private double DegreesToRadians(double degrees) => degrees * Math.PI / MapConstants.HalfWorldWidthInDecimalDegrees;

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="radians">The angle in radians.</param>
        /// <returns>The angle in degrees.</returns>
        private double RadiansToDegrees(double radians) => radians * MapConstants.HalfWorldWidthInDecimalDegrees / Math.PI;

        /// <summary>
        /// Clamp the latitude to be within the valid Web Mercator range.
        /// </summary>
        /// <param name="latitude">The latitude in degrees.</param>
        /// <returns>The clamped latitude in degrees.</returns>
        private double ClampLatitude(double latitude) =>
            Math.Max(-MapConstants.MaxWebMercatorLatitude, Math.Min(MapConstants.MaxWebMercatorLatitude, latitude));

        /// <summary>
        /// Normalizes an angle in radians to the -PI..PI range.
        /// </summary>
        /// <param name="radians">The angle in radians.</param>
        /// <returns>The normalized angle in radians.</returns>
        private static double NormalizeRadians(double radians)
        {
            // If the angle is greater than PI, wrap it around to the negative side of the circle.
            if (radians > Math.PI)
            {
                radians -= 2d * Math.PI;
            }
            // If the angle is less than -PI, wrap it around to the positive side of the circle.
            else if (radians < -Math.PI)
            {
                radians += 2d * Math.PI;
            }

            // Return the normalized angle in radians, which is now guaranteed to be within the range [-PI, PI].
            return radians;
        }
        #endregion
    }
}
