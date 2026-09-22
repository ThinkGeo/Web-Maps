using ThinkGeo.UI.Blazor.HowDoI.CustomerApp.Classes;
using ThinkGeo.UI.Blazor.HowDoI.CustomerApp.SharedProperties;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Blazor.HowDoI.CustomerApp.Utilities
{
    /// <summary>
    /// Provides utility methods for geographic coordinate conversions, normalizations, and calculations for map operations.
    /// </summary>
    /// <remarks>
    /// This utility class handles all geographic-related transformations including:
    /// - Coordinate system conversions (decimal degrees ↔ Web Mercator meters)
    /// - Dateline wrapping and normalization for world-wrap handling
    /// - Coordinate parsing and serialization from/to persisted formats
    /// - Geographic coordinate clamping and validation
    /// - DMS (degrees, minutes, seconds) formatting for display
    /// - World anchor point adjustments for pan-outside-extent scenarios
    /// 
    /// Key design patterns:
    /// - Uses Web Mercator (EPSG:3857) projection for meter conversions
    /// - Applies epsilon-based comparison for floating-point coordinate equality
    /// - Normalizes longitudes to -180..180 range for consistency
    /// - Clamps latitudes to Web Mercator limits (-85.05..85.05 degrees)
    /// - Handles dateline-crossing paths with wrapped longitude preservation
    /// </remarks>
    internal class MapGeographicUtilities
    {
        /// <summary>
        /// The Settings instance that holds the current map view, map class, and other related settings.
        /// </summary>
        public MapSettings RuntimeState { get; set; } = new MapSettings();

        #region Map Coordinate Conversion Methods and Properties

        /// <summary>
        /// Convert a PointShape representing a location on the map to the appropriate coordinate system based on the current
        /// map unit (meters or decimal degrees).
        /// </summary>
        /// <remarks>
        /// This method intelligently detects the current coordinate system by checking if coordinates are within valid
        /// decimal degree ranges (-180..180 for longitude, -90..90 for latitude). If detected as decimal degrees,
        /// converts to meters if needed; if detected as meters, keeps them as-is unless conversion to decimal degrees is needed.
        /// Handles null RuntimeState gracefully by returning the original point.
        /// </remarks>
        /// <param name="mapPoint">The PointShape representing a location on the map.</param>
        /// <returns>The converted PointShape in the appropriate map units.</returns>
        public PointShape ConvertMapPointToMapUnits(PointShape mapPoint)
        {
            // If the CurrentMapView is null, we cannot determine the map unit, so we return the original point
            if (RuntimeState.ActiveMapView == null)
            {
                return mapPoint;
            }

            // If the map point is within the valid range for decimal degrees,
            // we assume it is in decimal degrees.
            if (Math.Abs(mapPoint.X) < 180d && Math.Abs(mapPoint.Y) < 90d)
            {
                // Check the current map unit and convert accordingly.
                return RuntimeState.ActiveMapView.MapUnit switch
                {
                    GeographyUnit.Meter => ConvertPointToMeters(mapPoint),
                    GeographyUnit.DecimalDegree => mapPoint,
                    _ => mapPoint, // For unsupported units, return the original point
                };
            }
            // If the map point is outside the valid range for decimal degrees,
            // we assume it is in meters.
            else
            {
                // Check the current map unit and convert accordingly.
                return RuntimeState.ActiveMapView.MapUnit switch
                {
                    GeographyUnit.Meter => mapPoint,
                    GeographyUnit.DecimalDegree => ConvertPointToDecimalDegrees(mapPoint),
                    _ => mapPoint, // For unsupported units, return the original point
                };
            }
        }

        /// <summary>
        /// Convert a PointShape representing a location on the map from the current map unit (meters or decimal degrees) to 
        /// a standard coordinate system (decimal degrees).
        /// </summary>
        /// <remarks>
        /// Inverse of ConvertMapPointToMapUnits. Detects coordinate system by range checking:
        /// - If within decimal degree range (-180..180, -90..90), assumes already in decimal degrees
        /// - If outside that range, assumes meters and converts to decimal degrees
        /// Uses Web Mercator inverse projection formulas for meter-to-degree conversion.
        /// Returns empty PointShape if inputs are null.
        /// </remarks>
        /// <param name="mapPoint">The PointShape representing a location on the map.</param>
        /// <returns>The converted PointShape in decimal degrees.</returns>
        public PointShape ConvertMapPointFromMapUnits(PointShape mapPoint)
        {
            // If CurrentMapView is null, we cannot determine the map unit, so we return the original point
            if (RuntimeState.ActiveMapView == null || mapPoint == null)
            {
                return new PointShape();
            }

            // If the map point is within the valid range for decimal degrees,
            // we assume it is in decimal degrees.
            if (Math.Abs(mapPoint.X) < 180d && Math.Abs(mapPoint.Y) < 90d)
            {
                return mapPoint;
            }
            // If the map point is outside the valid range for decimal degrees,
            // we assume it is in meters.
            else
            {
                return ConvertPointToDecimalDegrees(mapPoint);
            }
        }

        /// <summary>
        /// Convert a PointShape from Web Mercator (EPSG:3857) meters to decimal degrees. 
        /// This involves applying the inverse of the Web Mercator projection formulas to convert the X and Y coordinates
        /// in meters back to longitude and latitude in decimal degrees. The resulting longitude and latitude values are 
        /// then normalized and clamped to ensure they fall within valid ranges for geographic coordinates.
        /// </summary>
        /// <param name="mapPoint">The PointShape representing a location on the map.</param>
        /// <returns>The converted PointShape in decimal degrees.</returns>
        public PointShape ConvertPointToDecimalDegrees(PointShape mapPoint)
        {
            // If the input mapPoint is null, return a new PointShape with default values (0,0).
            if (mapPoint == null)
            {
                return new PointShape();
            }

            // Convert latitude using the inverse Web Mercator formula.
            var latitude = RadiansToDegrees(2d * Math.Atan(Math.Exp(mapPoint.Y / MapConstants.EarthRadiusMeters)) - Math.PI / 2d);

            // Convert longitude using the inverse Web Mercator formula.
            var longitude = RadiansToDegrees(mapPoint.X / MapConstants.EarthRadiusMeters);

            // Clamp the resulting latitude value to ensure it is within valid ranges.
            latitude = ClampLatitude(latitude);

            // Normalize the resulting longitude value to ensure it is within valid ranges.
            longitude = NormalizeLongitude(longitude);

            // Return a new PointShape with the converted longitude and latitude values.
            return new PointShape(longitude, latitude);
        }

        /// <summary>
        /// Convert a PointShape from decimal degrees to Web Mercator (EPSG:3857) meters.
        /// </summary>
        /// <param name="mapPoint">The PointShape representing a location on the map.</param>
        /// <returns>The converted PointShape in meters.</returns>
        public PointShape ConvertPointToMeters(PointShape mapPoint)
        {
            // Clamp the latitude value to ensure it is within valid ranges.
            var latitude = ClampLatitude(mapPoint.Y);

            // Normalize the longitude value to ensure it is within valid ranges.
            var longitude = NormalizeLongitude(mapPoint.X);

            // Convert the latitude values to radians for the Web Mercator projection formulas.
            var y = MapConstants.EarthRadiusMeters * Math.Log(Math.Tan(Math.PI / 4d + DegreesToRadians(latitude) / 2d));

            // Convert the longitude value to radians and then to meters using the Web Mercator projection formula.
            var x = MapConstants.EarthRadiusMeters * DegreesToRadians(longitude);

            // Return a new PointShape with the converted X and Y values in meters.
            return new PointShape(x, y);
        }

        /// <summary>
        /// Convert longitude and latitude values from meters to decimal degrees.
        /// </summary>
        /// <param name="mapCoordinate">The MapCoordinate representing a location on the map.</param>
        /// <returns>The converted MapCoordinate in decimal degrees.</returns>
        public MapCoordinate ConvertMapCoordinateToDecimalDegrees(MapCoordinate mapCoordinate)
        {
            // Convert the MapCoordinate to a PointShape for conversion.
            var point = ConvertPointToDecimalDegrees(new PointShape(mapCoordinate.Longitude, mapCoordinate.Latitude));

            // Return a new MapCoordinate with the converted latitude and longitude values.
            return new MapCoordinate(point.Y, point.X);
        }

        /// <summary>
        /// Convert longitude and latitude values from decimal degrees to meters.
        /// </summary>
        /// <param name="mapCoordinate">The MapCoordinate representing a location on the map.</param>
        /// <returns>The converted MapCoordinate in meters.</returns>
        public MapCoordinate ConvertMapCoordinateToMeters(MapCoordinate mapCoordinate)
        {
            // Convert the MapCoordinate to a PointShape for conversion.
            var point = ConvertPointToMeters(new PointShape(mapCoordinate.Longitude, mapCoordinate.Latitude));

            // Return a new MapCoordinate with the converted latitude and longitude values.
            return new MapCoordinate(point.Y, point.X);
        }

        /// <summary>
        /// Adjust a point on the map when using the world, if the map has been panned outside 
        /// the maps normal extent.
        /// </summary>
        /// <param name="mapPoint">The PointShape representing a location on the map.</param>
        /// <returns>The adjusted PointShape within the valid map extent.</returns>
        public PointShape AdjustObjectPointFromWorldMap(PointShape mapPoint)
        {
            // If the input mapPoint is null, return a new PointShape with default values (0,0).
            if (mapPoint == null)
            {
                return new PointShape();
            }

            // Clone the input PointShape to create an adjusted point that can be modified without affecting the original.
            var adjusted = (PointShape)mapPoint.CloneDeep();

            // Get the anchor X coordinate from the current map view.
            var anchorX = GetWorldAnchorX();

            // If the current map unit is in meters, adjust the X coordinate of the point to account for wrapping around the
            // world in Web Mercator projection.
            if (RuntimeState.ActiveMapView?.MapUnit == GeographyUnit.Meter)
            {

                // Adjust the X coordinate of the point by subtracting the world width until it is within half a
                // world of the anchor X coordinate.
                while (adjusted.X - anchorX > MapConstants.HalfWorldWidthInMeters)
                {
                    adjusted.X -= MapConstants.WorldWidthInMeters;
                }

                // Adjust the X coordinate of the point by adding the world width until it is within half a
                // world of the anchor X coordinate.
                while (adjusted.X - anchorX < -MapConstants.HalfWorldWidthInMeters)
                {
                    adjusted.X += MapConstants.WorldWidthInMeters;
                }
            }
            // If the current map unit is in decimal degrees, adjust the X coordinate of the point to account for wrapping
            // around the world in geographic coordinates.
            else
            {
                // Adjust the X coordinate of the point by subtracting the world width until it is within half a
                // world of the anchor X coordinate.
                while (adjusted.X - anchorX > MapConstants.HalfWorldWidthInDecimalDegrees)
                {
                    adjusted.X -= MapConstants.WorldWidthInDecimalDegrees;
                }

                // Adjust the X coordinate of the point by adding the world width until it is within half a
                // world of the anchor X coordinate.
                while (adjusted.X - anchorX < -MapConstants.HalfWorldWidthInDecimalDegrees)
                {
                    adjusted.X += MapConstants.WorldWidthInDecimalDegrees;
                }

                // Clamp the Y coordinate (latitude) of the point to ensure it is within valid ranges for geographic
                // coordinates.
                adjusted.Y = ClampLatitude(adjusted.Y);
            }

            // Return the adjusted PointShape with the modified X and Y coordinates.
            return adjusted;
        }

        /// <summary>
        /// Get the anchor X coordinate from the current map view, which is used as a reference point for adjusting object 
        /// points when the map is panned outside its normal extent. The method attempts to retrieve the center point of the 
        /// map view and extract the X coordinate, or if that fails, it tries to get the center point from the map's extent. 
        /// If both attempts fail, it returns 0 as a default anchor X coordinate.
        /// </summary>
        /// <returns>The anchor X coordinate from the current map view or 0 if it cannot be determined.</returns>
        private double GetWorldAnchorX()
        {
            try
            {
                // If the CurrentMapView is null, we cannot retrieve the anchor X coordinate, so we return 0 as a default value.
                if (RuntimeState.ActiveMapView == null)
                {
                    return 0d;
                }

                // Try CenterPoint.X
                // Get the CenterPoint property of the CurrentMapView.
                var centerPoint = RuntimeState.ActiveMapView.GetType().GetProperty("CenterPoint");

                // Get the value of the CenterPoint property.
                var center = centerPoint?.GetValue(RuntimeState.ActiveMapView);

                // Get the X property of the CenterPoint and return it if it is a finite double value.
                var xValue = center?.GetType().GetProperty("X")?.GetValue(center);

                // If the X value is a double and is finite, return it as the anchor X coordinate.
                if (xValue is double x && double.IsFinite(x))
                {
                    return x;
                }

                // Try Extent/CurrentExtent.GetCenterPoint().X
                // If retrieving the CenterPoint.X value fails, attempt to retrieve the anchor X coordinate from the map's extent.
                var extentProp = RuntimeState.ActiveMapView.GetType().GetProperty("CurrentExtent")
                              ?? RuntimeState.ActiveMapView.GetType().GetProperty("Extent");

                // Get the current map view extent.
                var extent = extentProp?.GetValue(RuntimeState.ActiveMapView);

                // Get the GetCenterPoint method of the extent.
                var getCenter = extent?.GetType().GetMethod("GetCenterPoint", Type.EmptyTypes);

                // Get the value of the CenterPoint of the extent from the GetCenterPoint method.
                var extentCenterPoint = getCenter?.Invoke(extent, null);

                // Get the X property of the extent's CenterPoint and return it if it is a finite double value.
                var cxValue = extentCenterPoint?.GetType().GetProperty("X")?.GetValue(extentCenterPoint);

                // If the X value from the extent's CenterPoint is a double and is finite, return it as the anchor X
                // coordinate.
                if (cxValue is double cx && double.IsFinite(cx))
                {
                    return cx;
                }
            }
            catch
            {
                // ignore and use default
            }

            // If both attempts to retrieve the anchor X coordinate fail, return 0 as a default value.
            return 0d;
        }


        /// <summary>
        /// Convert a MapCoordinate from decimal degrees to meters using the Web Mercator projection. This method takes a 
        /// MapCoordinate and returns a new MapCoordinate with the same longitude but with the latitude converted to meters.
        /// </summary>
        /// <param name="mapCoordinate">The MapCoordinate to convert.</param>
        /// <returns>The converted MapCoordinate in meters.</returns>
        public MapCoordinate ConvertMapCoordinateToMetersPreserveLongitude(MapCoordinate mapCoordinate)
        {
            // Clamp the latitude to ensure it is within valid ranges for geographic coordinates.
            var latitude = ClampLatitude(mapCoordinate.Latitude);

            // Convert the latitude to meters using the Web Mercator projection, which involves taking the natural logarithm
            // of the tangent of the latitude in radians plus 90 degrees, multiplied by the Earth's radius in meters.
            var y = MapConstants.EarthRadiusMeters * Math.Log(Math.Tan(Math.PI / 4d + DegreesToRadians(latitude) / 2d));

            // Convert the longitude to meters using the Web Mercator projection, while preserving the unwrapped longitude
            // value to handle date line crossing correctly.
            var x = MapConstants.EarthRadiusMeters * DegreesToRadians(mapCoordinate.Longitude);

            // Return a new MapCoordinate with the converted latitude and longitude values in meters.
            // The constructor is called with (latitude, longitude) order, so we pass y as latitude and x as longitude.
            return new MapCoordinate(y, x); // ctor is (latitude, longitude)
        }

        /// <summary>
        /// Convert decimal degrees to degrees, minutes, and seconds (DMS) format. This method takes a decimal degree value and a boolean 
        /// indicating whether the value represents latitude or longitude. It calculates the degrees, minutes, and seconds components of 
        /// the DMS representation and appends the appropriate hemisphere indicator (N/S for latitude, E/W for longitude) based on the sign 
        /// of the decimal degree value. 
        /// The resulting DMS string is formatted with degrees, minutes, seconds, and hemisphere.
        /// </summary>
        /// <param name="decimalDegrees">The decimal degree value to convert.</param>
        /// <param name="isLatitude">Indicates whether the value represents latitude (true) or longitude (false).</param>
        /// <returns>A string representing the value in DMS format with the appropriate hemisphere.</returns>
        public string ConvertToDmsString(double decimalDegrees, bool isLatitude)
        {
            // Determine the hemisphere based on the sign of the decimal degree value and whether it represents latitude or longitude.
            var hemisphere = isLatitude
                ? (decimalDegrees >= 0 ? "N" : "S")
                : (decimalDegrees >= 0 ? "E" : "W");

            // Calculate the absolute value of the decimal degree to simplify the conversion process.
            var abs = Math.Abs(decimalDegrees);

            // Calculate the degrees, minutes, and seconds components of the DMS representation.
            var degrees = (int)Math.Floor(abs);

            // Calculate the full minutes by subtracting the degrees from the absolute value and multiplying by the DMS unit conversion factor.
            var minutesFull = (abs - degrees) * MapConstants.DmsUnitConversion;

            // Calculate the minutes by taking the integer part of the full minutes.
            var minutes = (int)Math.Floor(minutesFull);

            // Calculate the seconds by subtracting the minutes from the full minutes and multiplying by the DMS unit conversion factor.
            var seconds = (minutesFull - minutes) * MapConstants.DmsUnitConversion;

            // Format the DMS string with degrees, minutes, seconds, and hemisphere.
            return $"{degrees}° {minutes:00}' {seconds:00.##}\" {hemisphere}";
        }

        /// <summary>
        /// Formats the mouse-position label text using decimal-degree coordinates and map zoom level.
        /// </summary>
        /// <remarks>
        /// Converts decimal degree coordinates to DMS (degrees, minutes, seconds) format for improved readability.
        /// Adjusts zoom level display by subtracting MapSettings.MinZoomLevel to show user-facing zoom numbering.
        /// Returns MapConstants.DefaultMousePositionLabel if input point is null.
        /// Format: "Latitude: {N/S} {deg}° {min}' {sec}\", Longitude: {E/W} {deg}° {min}' {sec}\", Zoom: {zoom}"
        /// </remarks>
        /// <param name="decimalDegreePoint">The mouse location in decimal degrees.</param>
        /// <param name="mapZoomLevel">The current map zoom level.</param>
        /// <returns>A formatted mouse-position label string.</returns>
        public string FormatMousePositionLabel(PointShape decimalDegreePoint, int mapZoomLevel)
        {
            if (decimalDegreePoint == null)
            {
                return MapConstants.DefaultMousePositionLabel;
            }

            var latText = ConvertToDmsString(decimalDegreePoint.Y, isLatitude: true);
            var lonText = ConvertToDmsString(decimalDegreePoint.X, isLatitude: false);
            var zoomText = (mapZoomLevel - MapSettings.MinZoomLevel).ToString();

            return $"Latitude: {latText}, Longitude: {lonText}, Zoom: {zoomText}";
        }

        #endregion

        #region Normalization Methods for Geographic Coordinates

        /// <summary>
        /// Normalize the longitude to be within the range of -180 to 180 degrees. This is important for ensuring that
        /// longitude values are consistent and can be correctly interpreted by mapping libraries and APIs, especially 
        /// when dealing with global maps that may wrap around the International Date Line.
        /// Add 180 to the longitude get it to a value comparable to a 360 degree coordinate system. 
        /// Use modulus 360 to the get the remainder.
        /// Add the remainder to 360, this is necessary for negative values to be calculated correctly.
        /// Use modulus 360 again to get the remainder created from the above addition. 
        /// Subtract the originally added 180 to get it to correctly adjusted value. 
        /// </summary>
        /// <param name="longitude">The longitude value to normalize.</param>
        /// <returns>The normalized longitude value within the range of -180 to 180 degrees.</returns>
        private double NormalizeLongitude(double longitude) =>
            Math.Round(((longitude + 180d) % 360d + 360d) % 360d - 180d, 5);

        /// <summary>
        /// Normalize the center coordinates of a placed feature to ensure that they are within valid ranges for geographic 
        /// coordinates.
        /// </summary>
        /// <param name="center">The MapCoordinate representing the center of the placed feature.</param>
        /// <returns>The normalized MapCoordinate with latitude and longitude values within valid ranges.</returns>
        public MapCoordinate NormalizePlacedFeatureCenter(MapCoordinate? center)
        {
            // If the center latitude is null default to 0.
            var latitude = center?.Latitude ?? 0d;
            // If the center longitude is null default to 0.
            var longitude = center?.Longitude ?? 0d;

            // If the latitude value is not finite, default to 0.
            if (!double.IsFinite(latitude))
            {
                latitude = 0d;
            }

            // If the longitude value is not finite, default to 0.
            if (!double.IsFinite(longitude))
            {
                longitude = 0d;
            }

            // Clamp the latitude to ensure it is within valid ranges for geographic coordinates.
            latitude = ClampLatitude(latitude);

            // Adjust the longitude to ensure it is within valid ranges for geographic coordinates.
            longitude = NormalizeLongitude(longitude);

            // Return a new MapCoordinate with the normalized latitude and longitude values.
            return new MapCoordinate(latitude, longitude);
        }

        /// <summary>
        /// Normalize a list of MapCoordinate objects ensuring that the longitude values are adjusted to handle cases 
        /// where the coordinates cross the International Date Line.
        /// </summary>
        /// <param name="coordinates">The list of MapCoordinate objects.</param>
        /// <returns>A list of MapCoordinate objects with normalized longitude values.</returns>
        public List<MapCoordinate> NormalizeMapCoordinatesAcrossDateLine(IEnumerable<MapCoordinate> coordinates)
        {
            // Convert the input coordinates to a list, or create an empty list if the input is null.
            var source = coordinates?.ToList() ?? new List<MapCoordinate>();

            // If there are no coordinates, return the empty list.
            if (source.Count == 0)
            {
                return source;
            }

            // Create a new list to hold the normalized coordinates.
            var normalized = new List<MapCoordinate>(source.Count);

            // Start with the first coordinate, adjusting its longitude to be within the standard range.
            var prevLon = NormalizeLongitude(source[0].Longitude);

            // Add the first coordinate to the normalized list with the adjusted longitude.
            normalized.Add(new MapCoordinate(source[0].Latitude, prevLon));

            // Iterate through the remaining coordinates, adjusting their longitude values to ensure that they are consistent .
            // with the previous longitude and handle date line crossing.
            for (int i = 1; i < source.Count; i++)
            {
                // Adjust the longitude of the current coordinate to be within the standard range.
                var lon = NormalizeLongitude(source[i].Longitude);

                // Adjust the longitude of the current coordinate to ensure that it is within 180 degrees of the previous
                // longitude, which helps to handle cases where the polygon crosses the International Date Line.
                while (lon - prevLon > 180d)
                {
                    lon -= 360d;
                }

                while (lon - prevLon < -180d)
                {
                    lon += 360d;
                }

                // Add the current coordinate to the normalized list with the adjusted longitude. 
                normalized.Add(new MapCoordinate(source[i].Latitude, lon));

                // Update the previous longitude.
                prevLon = lon;
            }

            // Return the list of normalized coordinates.
            return normalized;
        }

        #endregion

        #region Persistence Normalization Methods
        /// <summary>
        /// Normalizes longitudes of saved drawn features when a line or polygon crosses the date line.
        /// </summary>
        /// <remarks>
        /// Prepares features for persistence by normalizing dateline-crossing coordinates:
        /// - For circles: normalizes center coordinate longitude to -180..180 range
        /// - For lines/polygons: normalizes all vertices if they cross the dateline
        /// Uses ShouldNormalizeForDateLine to detect when normalization is necessary.
        /// Modifies the input feature in-place; no return value.
        /// </remarks>
        /// <param name="drawnFeature">The MapDrawnFeature to normalize.</param>
        public void NormalizeSavedLongitudes(MapDrawnFeature drawnFeature)
        {
            // If the drawn feature is null, there is nothing to normalize, so we return early.
            if (drawnFeature == null)
            {
                return;
            }

            // If the drawn feature is a circle, we normalize its center coordinate's longitude for saving.
            if (drawnFeature.IsCircle)
            {
                drawnFeature.CircleCenter = NormalizeLongitudeForSave(drawnFeature.CircleCenter);
                return;
            }

            // If the drawn feature is a line and its vertices should be normalized for date line crossing, we normalize each vertex's longitude.
            if (drawnFeature.IsLine && ShouldNormalizeForDateLine(drawnFeature.LineVertices))
            {
                drawnFeature.LineVertices = drawnFeature.LineVertices
                    .Select(NormalizeLongitudeForSave)
                    .ToList();
            }

            // If the drawn feature is a polygon and its vertices should be normalized for date line crossing, we normalize each vertex's longitude.
            if (!drawnFeature.IsLine && ShouldNormalizeForDateLine(drawnFeature.PolygonVertices))
            {
                drawnFeature.PolygonVertices = drawnFeature.PolygonVertices
                    .Select(NormalizeLongitudeForSave)
                    .ToList();
            }
        }

        /// <summary>
        /// Normalizes a coordinate longitude to the standard -180..180 range for saved map data.
        /// </summary>
        /// <param name="coordinate">The MapCoordinate to normalize.</param>
        public MapCoordinate NormalizeLongitudeForSave(MapCoordinate coordinate)
        {
            // If the input coordinate is null, return a new MapCoordinate with default values (0,0).
            if (coordinate == null)
            {
                return new MapCoordinate(0, 0);
            }

            // Normalize the longitude to be within the standard -180..180 range.
            var normalizedLongitude = coordinate.Longitude;

            // Adjust the longitude to ensure it is within valid ranges for geographic coordinates.
            while (normalizedLongitude > 180d)
            {
                normalizedLongitude -= 360d;
            }

            while (normalizedLongitude < -180d)
            {
                normalizedLongitude += 360d;
            }

            // Return a new MapCoordinate with the original latitude and the normalized longitude.
            return new MapCoordinate(coordinate.Latitude, normalizedLongitude);
        }

        /// <summary>
        /// Determines whether longitudes should be normalized for date-line-safe persistence.
        /// </summary>
        /// <param name="coordinates">The list of MapCoordinate objects to check.</param>
        /// <returns>True if the longitudes should be normalized for date-line crossing; otherwise, false.</returns>
        private static bool ShouldNormalizeForDateLine(IReadOnlyList<MapCoordinate> coordinates)
        {
            // If the list of coordinates is null or empty, there is no need to normalize for date line crossing.
            if (coordinates == null || coordinates.Count == 0)
            {
                return false;
            }

            // Check if any coordinate has a longitude outside the valid range of -180 to 180 degrees.
            if (coordinates.Any(c => c != null && Math.Abs(c.Longitude) > 180d))
            {
                return true;
            }

            // Check if the list of coordinates has fewer than 2 points, in which case normalization is not needed.
            if (coordinates.Count < 2)
            {
                return false;
            }

            // Iterate through the list of coordinates and check if any consecutive points have a longitude difference greater than 180 degrees,s
            for (int i = 1; i < coordinates.Count; i++)
            {
                // If either of the consecutive coordinates is null, skip to the next iteration.
                if (coordinates[i] == null || coordinates[i - 1] == null)
                {
                    continue;
                }

                // Check if the absolute difference in longitude between consecutive coordinates exceeds 180 degrees, indicating a date line crossing.
                if (Math.Abs(coordinates[i].Longitude - coordinates[i - 1].Longitude) > 180d)
                {
                    return true;
                }
            }

            // If none of the conditions for normalization are met, return false.
            return false;
        }
        #endregion

        #region Math Constants, Functions and Methods

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degrees">The angle in degrees to convert to radians.</param>
        /// <returns>The angle in radians.</returns>
        private double DegreesToRadians(double degrees) => degrees * Math.PI / MapConstants.HalfWorldWidthInDecimalDegrees;

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="radians">The angle in radians to convert to degrees.</param>
        /// <returns>The angle in degrees.</returns>
        private double RadiansToDegrees(double radians) => radians * MapConstants.HalfWorldWidthInDecimalDegrees / Math.PI;

        /// <summary>
        /// Clamp the latitude to be within the range of -85.05112878 to 85.05112878 degrees, which are the limits for the 
        /// Web Mercator projection (EPSG:3857).
        /// </summary>
        /// <param name="latitude">The latitude value to clamp.</param>
        /// <returns>The clamped latitude value.</returns>
        private double ClampLatitude(double latitude) =>
            Math.Max(-MapConstants.MaxWebMercatorLatitude, Math.Min(MapConstants.MaxWebMercatorLatitude, latitude));

        /// <summary>
        /// Normalizes an angle in radians to the -PI..PI range.
        /// </summary>
        /// <param name="radians">The angle in radians to normalize.</param>
        /// <returns>The normalized angle in radians.</returns>
        private static double NormalizeRadians(double radians)
        {
            // Intentionally left in the code although it is not currently used. This method can be useful for normalizing angles in radians to the
            // standard range of -PI to PI, which is often required in geographic calculations and transformations.

            // Normalize the angle to be within the range of -PI to PI radians.
            if (radians > Math.PI)
            {
                radians -= 2d * Math.PI;
            }
            else if (radians < -Math.PI)
            {
                radians += 2d * Math.PI;
            }

            // Return the normalized angle in radians.
            return radians;
        }

        /// <summary>
        /// Determine if two MapCoordinate objects are equal within a specified epsilon tolerance. This method compares the 
        /// longitude and latitude values of the two MapCoordinate objects and returns true if both the longitude and 
        /// latitude differences are less than the specified epsilon value, indicating that the coordinates are effectively 
        /// equal for practical purposes. The default epsilon value uses the shared map coordinate comparison tolerance
        /// for comparing geographic coordinates.
        /// </summary>
        /// <remarks>
        /// Uses epsilon-based comparison to handle floating-point precision errors that arise from coordinate transformations.
        /// Critical for operations like vertex de-duplication and coordinate matching where exact equality is unreliable.
        /// Default epsilon is MapConstants.CoordinateComparisonEpsilon; can be overridden for custom tolerance requirements.
        /// </remarks>
        /// <param name="coord1">The first MapCoordinate to compare.</param>
        /// <param name="coord2">The second MapCoordinate to compare.</param>
        /// <param name="epsilon">The tolerance value for comparing the coordinates.</param>
        /// <returns>True if the coordinates are equal within the specified epsilon tolerance; otherwise, false.</returns>
        public bool CoordinatesEqual(MapCoordinate coord1, MapCoordinate coord2, double epsilon = MapConstants.CoordinateComparisonEpsilon) =>
            Math.Abs(coord1.Longitude - coord2.Longitude) < epsilon &&
            Math.Abs(coord1.Latitude - coord2.Latitude) < epsilon;

        #endregion

        #region Coordinate Parsing and Serialization

        /// <summary>
        /// Parses a comma-separated coordinate string (e.g., "lon,lat") into latitude and longitude.
        /// </summary>
        /// <remarks>
        /// Handles coordinate strings in "lon,lat" format, trimming whitespace and validating that both components parse as doubles.
        /// Defensive parsing: returns false if either component fails to parse; latitude and longitude are set to 0 on failure.
        /// Note: coordinate string format is (longitude, latitude) for storage compatibility, but returns (latitude, longitude) in out parameters.
        /// </remarks>
        /// <param name="coordinateString">The coordinate string to parse (format: "longitude,latitude").</param>
        /// <param name="latitude">The parsed latitude, or 0 if parsing fails.</param>
        /// <param name="longitude">The parsed longitude, or 0 if parsing fails.</param>
        /// <returns>True if both coordinates parsed successfully as doubles; otherwise false.</returns>
        public bool TryParseCoordinate(string coordinateString, out double latitude, out double longitude)
        {
            // Initialize output parameters to 0 in case parsing fails.
            latitude = 0;
            longitude = 0;

            // Split the input coordinate string by the comma delimiter to separate longitude and latitude components.
            var coordinates = coordinateString.Split(',');

            // Check if the split resulted in exactly two components (longitude and latitude) and attempt to parse both as doubles.
            return coordinates.Length == 2 &&
                   double.TryParse(coordinates[0], out longitude) &&
                   double.TryParse(coordinates[1], out latitude);
        }

        /// <summary>
        /// Parses a vertices string in "(lon,lat);(lon,lat);..." format into a list of MapCoordinates, applying optional unit conversion.
        /// </summary>
        /// <remarks>
        /// Deserializes persisted vertex geometry from semicolon-delimited parenthesized pairs.
        /// Strips parentheses and whitespace, then parses each "lon,lat" pair using TryParseCoordinate.
        /// Defensively skips malformed coordinates that fail to parse, accumulating only valid vertices.
        /// Optionally converts from meter-space to decimal degrees if convertToDecimalDegrees is true and active map unit is Meter.
        /// Returns empty list if input is null or empty.
        /// </remarks>
        /// <param name="verticesString">The vertices string to parse (format: "(lon,lat);(lon,lat);...").</param>
        /// <param name="convertToDecimalDegrees">If true and map unit is Meter, converts coordinates from meters to decimal degrees.</param>
        /// <returns>A list of parsed MapCoordinates. Empty if no valid coordinates found.</returns>
        public List<MapCoordinate> ParseVerticesFromString(string verticesString, bool convertToDecimalDegrees = false)
        {
            // Initialize an empty list to hold the parsed MapCoordinate objects.
            var vertices = new List<MapCoordinate>();

            // If the input vertices string is null, empty, or consists only of whitespace, return the empty list.
            if (string.IsNullOrWhiteSpace(verticesString))
            {
                return vertices;
            }

            // Split the input vertices string by the semicolon delimiter to separate individual vertex coordinate pairs.
            var verticesList = verticesString.Split(';').Select(v => v.Replace("(", "").Replace(")", "")).ToArray();

            // Iterate through each vertex coordinate string in the list.
            foreach (var vertex in verticesList)
            {
                // Attempt to parse the vertex coordinate string into latitude and longitude using the TryParseCoordinate method.
                if (TryParseCoordinate(vertex, out var latitude, out var longitude))
                {
                    // Create a new MapCoordinate object with the parsed latitude and longitude values.
                    var mapCoordinate = new MapCoordinate { Latitude = latitude, Longitude = longitude };

                    // If the convertToDecimalDegrees flag is true and the active map unit is Meter, convert the MapCoordinate from meters to
                    // decimal degrees.
                    if (convertToDecimalDegrees && RuntimeState.ActiveMapView.MapUnit == GeographyUnit.Meter)
                    {
                        mapCoordinate = ConvertMapCoordinateToDecimalDegrees(mapCoordinate);
                    }

                    // Add the parsed (and possibly converted) MapCoordinate to the list of vertices.
                    vertices.Add(mapCoordinate);
                }
            }

            // Return the list of parsed MapCoordinate objects. If no valid coordinates were found, the list will be empty.
            return vertices;
        }

        /// <summary>
        /// Serializes a list of MapCoordinates to a semicolon-delimited string in "(lon,lat);(lon,lat);..." format.
        /// </summary>
        /// <remarks>
        /// Converts vertex list to persisted format for storage in feature columns.
        /// Format: "(longitude,latitude);(longitude,latitude);..." to match database schema expectations.
        /// Handles null/empty lists gracefully by returning empty string.
        /// Does not perform coordinate transformations; assumes input coordinates are in the desired storage unit.
        /// </remarks>
        /// <param name="vertices">The vertices to serialize.</param>
        /// <returns>The serialized vertices string, or empty string if input is null/empty.</returns>
        public string SerializeVerticesToString(List<MapCoordinate> vertices)
        {
            // If the input list of vertices is null or empty, return an empty string to indicate no vertices to serialize.
            if (vertices == null || vertices.Count == 0)
            {
                return string.Empty;
            }

            // Use LINQ to project each MapCoordinate in the list to a string in the format "(longitude,latitude)" and join them with semicolons.
            return string.Join(";", vertices.Select(v => $"({v.Longitude},{v.Latitude})"));
        }

        /// <summary>
        /// Converts a collection of Vertex objects to meter-space coordinates, preserving wrapped longitude behavior.
        /// </summary>
        /// <remarks>\n        /// Converts geographic coordinates (decimal degrees) to Web Mercator projection (meters).
        /// Uses ConvertMapCoordinateToMetersPreserveLongitude to maintain unwrapped longitude values,\n        /// allowing dateline-crossing paths to be correctly represented in meter space.\n        /// Input vertices are expected to be in (X=longitude, Y=latitude) format; output maintains same format.\n        /// Returns new list; does not modify input collection.\n        /// </remarks>
        /// <param name="vertices">The vertices to convert (in decimal degrees).</param>
        /// <returns>A list of Vertex objects converted to meter space.</returns>
        public List<Vertex> ConvertVerticesToMeterSpace(IEnumerable<Vertex> vertices)
        {
            // Return a new list of Vertex objects by projecting each input vertex from decimal degrees to meter space using the
            // Web Mercator projection.
            return vertices
                .Select(v =>
                {
                    var m = ConvertMapCoordinateToMetersPreserveLongitude(
                        new MapCoordinate(v.Y, v.X));
                    return new Vertex(m.Longitude, m.Latitude);
                })
                .ToList();
        }

        /// <summary>
        /// Converts a coordinate from the current map unit to decimal degrees. If map unit is Meter, converts; otherwise returns unchanged.
        /// </summary>
        /// <remarks>
        /// Convenience method for conditional coordinate conversion based on active map unit.
        /// - If map unit is Meter: applies Web Mercator inverse projection to convert to decimal degrees
        /// - If map unit is DecimalDegree or other: returns coordinate unchanged\n        /// Assumes RuntimeState.ActiveMapView is initialized; null map view may cause unexpected behavior.
        /// </remarks>
        /// <param name="coordinate">The coordinate to convert.</param>
        /// <returns>The converted coordinate in decimal degrees, or unchanged if already in decimal degrees.</returns>
        public MapCoordinate ConvertToDecimalDegrees(MapCoordinate coordinate)
        {
            // If the active map unit is Meter, convert the coordinate from meters to decimal degrees; otherwise, return it unchanged.
            return RuntimeState.ActiveMapView.MapUnit == GeographyUnit.Meter
                ? ConvertMapCoordinateToDecimalDegrees(coordinate)
                : coordinate;
        }

        #endregion

    }
}
