using ThinkGeo.UI.Blazor.HowDoI.CustomerApp.Classes;
using ThinkGeo.UI.Blazor.HowDoI.CustomerApp.SharedProperties;
using System.Collections.ObjectModel;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Blazor.HowDoI.CustomerApp.Utilities
{
    /// <summary>
    /// Provides utility methods for converting between map features and drawn/placed features, handling coordinate normalization, 
    /// and managing feature properties.
    /// </summary>
    /// <remarks>
    /// This class manages two types of feature conversions:
    /// - Drawn Features: User-drawn shapes (lines, polygons, circles) with editing state and generated geometry (densified paths)
    /// - Placed Features: Icon-based point features with location and icon metadata
    /// 
    /// Conversions handle:
    /// - Coordinate system transformations (decimal degrees ↔ meter space based on RuntimeState.ActiveMapView.MapUnit)
    /// - Dateline wrapping normalization to prevent world-wrap rendering artifacts
    /// - Vertex parsing and serialization from/to persisted feature columns
    /// - Generated geometry (post-processing output like densified paths and split segments)
    /// - Polygon ring closure validation and line preservation
    /// - Icon resolution and type mapping for placed features
    /// </remarks>
    internal class MapFeatureConversionUtilities
    {
        #region Dependencies

        /// <summary>
        /// The runtimeState instance that holds the current map view, map class, and other related settings.
        /// </summary>
        public MapRuntimeState RuntimeState { get; set; } = new MapRuntimeState();

        /// <summary>
        /// Utilities for geographic calculations and conversions related to map coordinates.
        /// </summary>
        public MapGeographicUtilities MapGeoUtilities { get; set; } = new MapGeographicUtilities();

        /// <summary>
        /// Utilities for measuring distances, areas, and other spatial properties on the map.
        /// </summary>
        public MapMeasurementUtilities MapMeasurementUtilities { get; set; } = new MapMeasurementUtilities();

        /// <summary>
        /// Utilities for loading and resolving map icon and image resources.
        /// </summary>
        public MapGeoImageUtilities MapGeoImageUtilities { get; set; } = new MapGeoImageUtilities();

        /// <summary>
        /// Utilities for loading and resolving map icon and image resources, with the runtime state set for context.
        /// </summary>
        private MapGeoImageUtilities GeoImageUtilities
        {
            get
            {
                MapGeoImageUtilities.RuntimeState = RuntimeState;
                return MapGeoImageUtilities;
            }
        }

        #endregion

        #region Drawn Feature Conversion

        /// <summary>
        /// Converts a persisted Feature to a MapDrawnFeature for editing and display.
        /// </summary>
        /// <param name="feature">The Feature to convert. Can represent a line, polygon, or circle.</param>
        /// <returns>The converted MapDrawnFeature with parsed vertices, generated geometry, and circle properties if applicable.</returns>
        /// <remarks>
        /// This method reconstructs editing-time feature state from persisted columns:
        /// - Parses base vertices and generated (post-processed) vertices from serialized strings
        /// - Normalizes vertices across the dateline to prevent rendering artifacts
        /// - Reconstructs generated geometry from pipe-delimited segment strings
        /// - Validates polygon closure and ensures ring topology consistency
        /// - Extracts circle center and radius if the feature is a circle
        /// Returns an empty MapDrawnFeature if the feature is malformed or has no valid vertices.
        /// </remarks>
        public MapDrawnFeature ConvertMapFeatureToMapDrawnFeature(Feature feature)
        {
            // Create a new MapDrawnFeature and populate its properties based on the feature's column values.
            var drawnFeature = new MapDrawnFeature
            {
                Id = feature.Id,
                Name = feature.ColumnValues[MapConstants.NameColumn].ToString(),
                IsLine = feature.ColumnValues[MapConstants.IsLineColumn] == "1",
                IsCircle = feature.ColumnValues[MapConstants.IsCircleColumn] == "1"
            };

            // If the feature represents a circle, extract the circle's radius and center coordinates.
            if (feature.ColumnValues[MapConstants.IsCircleColumn] == "1")
            {
                // If the circle radius is not a valid positive number, return an empty MapDrawnFeature.
                drawnFeature.CircleRadius = double.TryParse(feature.ColumnValues[MapConstants.CircleRadiusColumn].ToString(), out var radius) ? radius : 0;
                if (drawnFeature.CircleRadius <= 0)
                {
                    return new MapDrawnFeature();
                }

                // Extract the circle center coordinates from the feature's column values, defaulting to (0,0) if parsing fails.s
                var circleCenter = new MapCoordinate
                {
                    Latitude = double.TryParse(feature.ColumnValues[MapConstants.CircleCenterYColumn].ToString(), out var centerY) ? centerY : 0,
                    Longitude = double.TryParse(feature.ColumnValues[MapConstants.CircleCenterXColumn].ToString(), out var centerX) ? centerX : 0
                };

                // Stored circles use decimal degrees; convert only for meter-based map units.
                drawnFeature.CircleCenter = MapGeoUtilities.ConvertToDecimalDegrees(circleCenter);

                return drawnFeature;
            }
            // Create the line or polygon. 
            // Persisted vertex columns use a semicolon-delimited "(lon,lat)" format.
            var verticesString = feature.ColumnValues[MapConstants.IsLineColumn] == "1"
                ? feature.ColumnValues[MapConstants.LineVerticesColumn].ToString()
                : feature.ColumnValues[MapConstants.PolygonVerticesColumn].ToString();

            // Split the vertices string into individual vertex strings, removing parentheses and whitespace.s
            var verticesList = verticesString.Split(';').Select(v => v.Replace("(", "").Replace(")", "")).ToArray();
            if ((feature.ColumnValues[MapConstants.IsLineColumn] == "1" && verticesList.Length < 2) ||
                (feature.ColumnValues[MapConstants.IsLineColumn] == "0" && verticesList.Length < 3))
            {
                return new MapDrawnFeature();
            }

            // Serialized vertices come from persisted feature columns and may include malformed points.
            // We parse defensively and only keep coordinates that successfully parse to doubles.
            var vertices = MapGeoUtilities.ParseVerticesFromString(verticesString);

            // Normalize longitudes to ensure consistent representation of dateline-crossing paths/polygons and prevent world-wrap
            // artifacts during rendering.
            vertices = MapGeoUtilities.NormalizeMapCoordinatesAcrossDateLine(vertices);
            if (vertices.Count == 0)
            {
                return new MapDrawnFeature();
            }

            // Generated vertices are stored in a separate column and may represent post-processing output (for example, split/densified paths).s
            var genVerticesStrings = feature.ColumnValues[MapConstants.IsLineColumn] == "1"
                ? feature.ColumnValues[MapConstants.GeneratedLineVerticesColumn].ToString()
                : feature.ColumnValues[MapConstants.GeneratedPolygonVerticesColumn].ToString();

            // Generated vertices are stored as path segments separated by '|'.
            // Generated vertices represent post-processing output (for example, split/densified paths) and
            // are persisted as multiple segments separated by '|'. Each segment still uses "(lon,lat);...".
            var genVerticesLists = new List<List<MapCoordinate>>();
            foreach (var genVerticesString in genVerticesStrings.Split('|'))
            {
                // Split the generated vertices string into individual vertex strings, removing parentheses and whitespace.
                var genVerticesList = MapGeoUtilities.ParseVerticesFromString(genVerticesString, convertToDecimalDegrees: true);

                // If the generated vertices list for this segment contains any valid vertices, add it to the list of generated
                // vertex segments.
                if (genVerticesList.Count > 0)
                {
                    genVerticesLists.Add(genVerticesList);
                }
            }

            // If the feature represents a line, assign the parsed vertices and generated vertices to the MapDrawnFeature
            // and return it.
            if (drawnFeature.IsLine)
            {
                // Lines can stay open; no explicit closure step is required.
                drawnFeature.LineVertices = vertices;
                drawnFeature.GeneratedLineVertices = genVerticesLists;
                return drawnFeature;
            }

            // Polygons must be explicitly closed for reliable ring reconstruction.
            var firstVertex = vertices.FirstOrDefault();
            var lastVertex = vertices.LastOrDefault();
            if (firstVertex != null && lastVertex != null && !MapGeoUtilities.CoordinatesEqual(firstVertex, lastVertex))
            {
                vertices.Add(firstVertex);
            }

            // Ensure that the generated vertices for polygons are also closed, so that reconstructed geometry remains
            // topologically closed whether callers use base vertices or generated vertices.
            if (genVerticesLists.Count > 0)
            {
                // Mirror polygon closure in the generated segments so reconstructed geometry stays topologically closed
                // whether callers use base vertices or generated vertices.
                var firstGenSegment = genVerticesLists[0];
                if (firstGenSegment.Count > 0)
                {
                    var lastGenSegment = genVerticesLists[^1];
                    var firstGenVertex = firstGenSegment[0];
                    var lastGenVertex = lastGenSegment.Count > 0 ? lastGenSegment[^1] : null;
                    if (firstGenVertex != null && lastGenVertex != null && !MapGeoUtilities.CoordinatesEqual(firstGenVertex, lastGenVertex))
                    {
                        lastGenSegment.Add(firstGenVertex);
                    }
                }
            }

            // Assign the parsed vertices and generated vertices to the MapDrawnFeature and return it.
            drawnFeature.PolygonVertices = vertices;
            drawnFeature.GeneratedPolygonVertices = genVerticesLists;
            return drawnFeature;
        }

        #endregion

        #region Reverse Conversion (Drawn Feature to Feature)

        /// <summary>
        /// Converts a MapDrawnFeature (edited user shape) back to a persisted Feature for storage.
        /// </summary>
        /// <param name="drawnFeature">The MapDrawnFeature to convert. Can be a line, polygon, or circle.</param>
        /// <returns>The converted Feature with persisted vertex strings, generated geometry, and shape-specific column values.</returns>
        /// <remarks>
        /// This method prepares features for persistence and cross-dateline rendering:
        /// - Normalizes vertices across the dateline to prevent world-wrap artifacts
        /// - Densifies geodesic lines to preserve curved paths edited by users
        /// - Explicitly closes polygons and mirrors closure in generated segments for topological consistency
        /// - Converts coordinates to/from meter space based on active map unit
        /// - Serializes vertices and generated geometry into persisted column format
        /// - Splits lines by dateline to ensure proper rendering on world maps
        /// - Stores generated geometry in pipe-delimited segment format for multi-segment output (e.g., split lines)
        /// </remarks>
        public Feature ConvertMapDrawnFeatureToMapFeature(MapDrawnFeature drawnFeature)
        {
            // Create a new Feature to hold the converted properties from the MapDrawnFeature.s
            var newFeature = new Feature();

            // If the drawnFeature is null, return an empty feature.
            if (drawnFeature == null)
            {
                return newFeature;
            }

            // If the drawnFeature represents a circle, extract the circle's radius and center coordinates.s
            var circleCenter = drawnFeature.CircleCenter;

            // Create the string representations of the vertices for storage in the feature's column values.
            var verticesString = string.Empty;
            var genVerticesString = string.Empty;

            // If the drawnFeature represents a circle, create an EllipseShape for the feature's shape based on the circle's center
            // and radius.
            if (drawnFeature.IsCircle)
            {
                // If the circle radius is not a valid positive number, return an empty feature.
                if (drawnFeature.CircleRadius <= 0)
                {
                    return newFeature;
                }

                // Convert the circle center to meters if the current map unit is in meters, so that the feature's shape is
                // stored consistently.
                if (RuntimeState.ActiveMapView.MapUnit == GeographyUnit.Meter)
                {
                    circleCenter = MapGeoUtilities.ConvertMapCoordinateToMeters(drawnFeature.CircleCenter);
                }

                // Create a new Feature with an EllipseShape based on the circle's center and radius.
                newFeature = new Feature(new EllipseShape(new PointShape(
                                                              circleCenter.Longitude,
                                                              circleCenter.Latitude),
                                                              drawnFeature.CircleRadius));
            }
            else
            {
                // Ignore incomplete in-progress edits that cannot form a valid line or polygon.
                if (drawnFeature.LineVertices.Count < 2 && drawnFeature.PolygonVertices.Count < 3)
                {
                    return newFeature;
                }

                // Normalize longitudes before rebuilding ThinkGeo shapes so dateline-crossing paths/polygons
                // are represented consistently and do not create world-wrap artifacts during rendering.
                var normalizedVertices = MapGeoUtilities.NormalizeMapCoordinatesAcrossDateLine(
                    drawnFeature.IsLine ? drawnFeature.LineVertices : drawnFeature.PolygonVertices);

                // Densify geodesic lines so persisted shapes preserve the same curved path users edited.
                if (drawnFeature.IsLine)
                {
                    normalizedVertices = MapMeasurementUtilities.DensifyGreatCircle(normalizedVertices);
                }

                // For polygons, ensure the first and last vertices are equal to close the ring explicitly for reliable ring 
                // reconstruction. Lines can stay open; no explicit closure step is required.
                var verticesToUse = normalizedVertices;
                if (!drawnFeature.IsLine && !MapGeoUtilities.CoordinatesEqual(verticesToUse[0], verticesToUse[^1]))
                {
                    verticesToUse.Add(new MapCoordinate(
                        verticesToUse[0].Latitude,
                        verticesToUse[0].Longitude));
                }

                // Convert the normalized vertices to ThinkGeo Vertex objects for constructing the feature's shape.
                var vertices = verticesToUse
                    .Select(v => new Vertex(v.Longitude, v.Latitude))
                    .ToList();

                // Keep authored vertices for storage while using normalized vertices for split/render operations.
                verticesString = MapGeoUtilities.SerializeVerticesToString(GetVerticesForSerialization(drawnFeature));

                // Lines are split across the dateline; polygons are preserved as a single multi polygon container.
                newFeature = drawnFeature.IsLine ? DecimalDegreesHelper.SplitByDateline(
                    new Feature(new LineShape(vertices)))
                    : new Feature(new MultipolygonShape(new List<PolygonShape> { new PolygonShape(new RingShape(vertices)) }));

                // Create a list to hold the generated vertices..
                var genVerticesList = new List<string>();

                // Create a new feature with the split shape, which may be a MultilineShape or MultipolygonShape depending on the
                // drawnFeature type.
                var splitShape = newFeature.GetShape();

                // Process the line.
                if (splitShape is MultilineShape splitMultiLine)
                {
                    // Create a collection to hold the lines in meters.
                    var meterLines = new Collection<LineShape>();

                    // Iterate through each line in the split multi line shape.
                    foreach (var line in splitMultiLine.Lines)
                    {
                        if (RuntimeState.ActiveMapView.MapUnit == GeographyUnit.Meter)
                        {
                            // Convert vertices to meter-space while preserving wrapped longitude behavior.
                            var meterLineVertices = MapGeoUtilities.ConvertVerticesToMeterSpace(line.Vertices);
                            meterLines.Add(new LineShape(meterLineVertices));
                            genVerticesList.Add(string.Join(";", meterLineVertices.Select(v => $"({v.X},{v.Y})")));
                        }
                        else
                        {
                            // Store the generated decimal-degree vertices in the genVerticesList.
                            genVerticesList.Add(string.Join(";", line.Vertices.Select(v => $"({v.X},{v.Y})")));
                        }
                    }

                    // If the current map unit is in meters, create a new Feature with a MultilineShape in meters.
                    if (RuntimeState.ActiveMapView.MapUnit == GeographyUnit.Meter)
                    {
                        newFeature = new Feature(new MultilineShape(meterLines));
                    }
                }
                // Process the polygon.
                else if (splitShape is MultipolygonShape splitMultiPolygon)
                {
                    // Create a collection to hold the polygons in meters.
                    var meterPolygons = new Collection<PolygonShape>();

                    // Iterate through each polygon in the split multi polygon shape.
                    foreach (var polygon in splitMultiPolygon.Polygons)
                    {
                        if (RuntimeState.ActiveMapView.MapUnit == GeographyUnit.Meter)
                        {
                            // Convert outer ring vertices to meter-space while preserving wrapped longitude behavior.
                            var meterOuterRingVertices = MapGeoUtilities.ConvertVerticesToMeterSpace(polygon.OuterRing.Vertices);
                            meterPolygons.Add(new PolygonShape(new RingShape(meterOuterRingVertices)));
                            genVerticesList.Add(string.Join(";", meterOuterRingVertices.Select(v => $"({v.X},{v.Y})")));
                        }
                        else
                        {
                            // Store the generated decimal-degree vertices in the genVerticesList.
                            genVerticesList.Add(string.Join(";", polygon.OuterRing.Vertices.Select(v => $"({v.X},{v.Y})")));
                        }
                    }

                    // If the current map unit is in meters, create a new Feature with a MultipolygonShape in meters.
                    if (RuntimeState.ActiveMapView.MapUnit == GeographyUnit.Meter)
                    {
                        newFeature = new Feature(new MultipolygonShape(meterPolygons));
                    }
                }

                // Store generated geometry with the same segmented serialization format expected by reload paths.
                genVerticesString = string.Join("|", genVerticesList);
            }

            // Assign the feature's ID and column values based on the drawnFeature's properties and the generated vertices.
            newFeature.Id = !string.IsNullOrWhiteSpace(drawnFeature.Id) ? drawnFeature.Id : new Guid().ToString();
            newFeature.ColumnValues[MapConstants.NameColumn] = drawnFeature.Name;
            newFeature.ColumnValues[MapConstants.LabelNameColumn] = drawnFeature.IsLine ? string.Empty : drawnFeature.Name;
            newFeature.ColumnValues[MapConstants.LineLabelNameColumn] = drawnFeature.IsLine ? drawnFeature.Name : string.Empty;
            newFeature.ColumnValues[MapConstants.ArrowNameColumn] = string.Empty;
            newFeature.ColumnValues[MapConstants.IsLineColumn] = drawnFeature.IsLine ? "1" : "0";
            newFeature.ColumnValues[MapConstants.LineVerticesColumn] = drawnFeature.IsLine ? verticesString : string.Empty;
            newFeature.ColumnValues[MapConstants.GeneratedLineVerticesColumn] = drawnFeature.IsLine ? genVerticesString : string.Empty;
            newFeature.ColumnValues[MapConstants.IsCircleColumn] = drawnFeature.IsCircle ? "1" : "0";
            newFeature.ColumnValues[MapConstants.CircleRadiusColumn] = drawnFeature.CircleRadius.ToString();
            newFeature.ColumnValues[MapConstants.CircleCenterXColumn] = circleCenter.Longitude.ToString();
            newFeature.ColumnValues[MapConstants.CircleCenterYColumn] = circleCenter.Latitude.ToString();
            newFeature.ColumnValues[MapConstants.PolygonVerticesColumn] = drawnFeature.IsLine ? string.Empty : verticesString;
            newFeature.ColumnValues[MapConstants.GeneratedPolygonVerticesColumn] = drawnFeature.IsLine ? genVerticesString : string.Empty;

            // Return the created Feature with the assigned properties.
            return newFeature;
        }

        /// <summary>
        /// Converts a persisted Feature to a MapPlacedFeature for display as an icon/marker.
        /// </summary>
        /// <param name="placedFeature">The Feature to convert. Contains icon metadata and center point geometry.</param>
        /// <returns>The converted MapPlacedFeature with center coordinates and icon information.</returns>
        /// <remarks>
        /// Extracts placed feature properties from persisted columns and validates coordinate validity:
        /// - Gets the center point of the feature's shape
        /// - Adjusts for world map wrapping to ensure consistent coordinate representation
        /// - Converts coordinates from meter space to decimal degrees if applicable
        /// - Normalizes center coordinates to valid geographic ranges
        /// - Preserves icon type and name for UI rendering
        /// Defaults invalid coordinates to (0, 0) to prevent null reference issues.
        /// </remarks>
        public MapPlacedFeature ConvertMapFeatureToMapPlacedFeature(Feature placedFeature)
        {
            // Create a new MapPlacedFeature and populate its properties based on the feature's column values.
            var newFeature = new MapPlacedFeature
            {
                Id = placedFeature.Id,
                Name = placedFeature.ColumnValues[MapConstants.NameColumn].ToString(),
                IconType = placedFeature.ColumnValues[MapConstants.IconTypeColumn].ToString(),
                IconName = placedFeature.ColumnValues[MapConstants.IconNameColumn].ToString()
            };

            // Get the center point of the feature's shape.
            var centerPoint = placedFeature.GetShape().GetCenterPoint();

            // Adjust the center point for world map wrapping if necessary.
            centerPoint = MapGeoUtilities.AdjustObjectPointFromWorldMap(centerPoint);

            // If the X or Y values are invalid, we default to 0.
            var centerCoordinates = new MapCoordinate
            {
                Latitude = double.TryParse(centerPoint.Y.ToString(), out var centerY) ? centerY : 0,
                Longitude = double.TryParse(centerPoint.X.ToString(), out var centerX) ? centerX : 0
            };

            // If the current map unit is in meters, convert the center coordinates from meters to decimal degrees.
            if (RuntimeState.ActiveMapView.MapUnit == GeographyUnit.Meter)
            {
                centerCoordinates = MapGeoUtilities.ConvertMapCoordinateToDecimalDegrees(centerCoordinates);
            }

            // Normalize the center coordinates to ensure they are within valid ranges for geographic coordinates.
            newFeature.FeatureCenter = MapGeoUtilities.NormalizePlacedFeatureCenter(centerCoordinates);

            // Return the created MapPlacedFeature.
            return newFeature;
        }

        #endregion

        #region Reverse Conversion (Placed Feature to Feature)

        /// <summary>
        /// Converts a MapPlacedFeature back to a persisted Feature for storage in a feature layer or database.
        /// </summary>
        /// <param name="placedFeature">The MapPlacedFeature to convert. Contains center location and icon metadata.</param>
        /// <returns>The converted Feature with persisted center point geometry and icon properties.</returns>
        /// <remarks>
        /// Prepares placed features for persistence with coordinate system handling:
        /// - Normalizes center coordinates to valid geographic ranges
        /// - Converts coordinates from decimal degrees to meter space if applicable
        /// - Creates a PointShape centered at the feature's location
        /// - Adjusts for world map wrapping to maintain consistent representation
        /// - Resolves missing icon names to MapConstants.MissingIconName
        /// - Looks up icon type index from GeoImageUtilities if not provided
        /// - Stores all properties in persisted feature columns for retrieval
        /// </remarks>
        public Feature ConvertMapPlacedFeatureToMapFeature(MapPlacedFeature placedFeature)
        {
            // Create a new Feature.
            var newFeature = new Feature();

            // If the placedFeature is null, return an empty feature.
            if (placedFeature == null)
            {
                return newFeature;
            }

            // Normalize first to prevent invalid coordinates clearing map render.
            var normalizedCenter = MapGeoUtilities.NormalizePlacedFeatureCenter(placedFeature.FeatureCenter);

            // Set the center to the normalized center.
            var center = normalizedCenter;

            // If the current map unit is in meters, convert the center coordinates from decimal degrees to meters.
            if (RuntimeState.ActiveMapView.MapUnit == GeographyUnit.Meter)
            {
                center = MapGeoUtilities.ConvertMapCoordinateToMeters(center);
            }

            // Create a PointShape for the center of the placed feature.
            var centerPoint = new PointShape(center.Longitude, center.Latitude);

            // Adjust the center point for world map wrapping if necessary.
            centerPoint = MapGeoUtilities.AdjustObjectPointFromWorldMap(centerPoint);

            // If the X or Y values of the center point are invalid, default them to 0.
            if (!double.IsFinite(centerPoint.X) || !double.IsFinite(centerPoint.Y))
            {
                centerPoint = new PointShape(0, 0);
            }

            // Create a new Feature with the center point as its shape.
            newFeature = new Feature(centerPoint);

            // If then placed feature has an Id, assign it to the new feature.
            if (!string.IsNullOrWhiteSpace(placedFeature.Id))
            {
                newFeature.Id = placedFeature.Id;
            }

            // Assign the Name, IconType, and IconName properties of the placed feature.
            var iconName = string.IsNullOrWhiteSpace(placedFeature.IconName)
                ? MapConstants.MissingIconName
                : placedFeature.IconName;
            var iconType = placedFeature.IconType;
            if (string.IsNullOrWhiteSpace(iconType) || iconType == "0")
            {
                var iconIndex = GeoImageUtilities.GetIconGeoImageIndex(iconName);
                iconType = iconIndex >= 0 ? iconIndex.ToString() : "0";
            }

            // Assign the column values for the new feature based on the placed feature's properties.
            newFeature.ColumnValues[MapConstants.NameColumn] = placedFeature.Name ?? string.Empty;
            newFeature.ColumnValues[MapConstants.IconTypeColumn] = iconType;
            newFeature.ColumnValues[MapConstants.IconNameColumn] = iconName;

            // Return the created Feature.
            return newFeature;
        }

        #endregion

        #region Internal Helpers

        /// <summary>
        /// Gets the appropriate vertices list for serialization based on whether the feature is a line or polygon.
        /// </summary>
        /// <param name="drawnFeature">The drawn feature to extract vertices from.</param>
        /// <returns>The line vertices if it's a line; otherwise polygon vertices.
        /// This ensures consistent vertex serialization regardless of feature type.</returns>
        /// <remarks>
        /// This helper simplifies the logic for accessing the correct vertex collection,
        /// as MapDrawnFeature stores line and polygon vertices in separate properties.
        /// The returned vertices are used for persistence and reconstruction of feature geometry.
        /// </remarks>
        private List<MapCoordinate> GetVerticesForSerialization(MapDrawnFeature drawnFeature)
        {
            return drawnFeature.IsLine ? drawnFeature.LineVertices : drawnFeature.PolygonVertices;
        }
        #endregion
    }
}

