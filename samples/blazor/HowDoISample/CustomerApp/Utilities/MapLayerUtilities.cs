using ThinkGeo.UI.Blazor.HowDoI.CustomerApp.SharedProperties;
using System.Collections;
using System.Collections.ObjectModel;
using ThinkGeo.Core;
using ThinkGeo.UI.Blazor;

namespace ThinkGeo.UI.Blazor.HowDoI.CustomerApp.Utilities
{
    /// <summary>
    /// Provides utility methods for managing map layers, overlays, and styling configuration.
    /// </summary>
    /// <remarks>
    /// This utility class handles:
    /// - Creating and configuring in-memory feature layers (drawn features, placed features, arrows, selection highlights)
    /// - Managing overlay registration and refresh when style definitions change
    /// - Querying feature IDs by name or validating candidate feature IDs
    /// - Creating and scaling text/image styles for different zoom levels
    /// - Maintaining consistent label colors based on dark/light mode theme
    /// 
    /// The layer configuration follows a five-tier zoom bucket strategy (scale factors 1-5) to group ThinkGeo zoom levels
    /// and apply consistent style scaling across all layers. Overlays are automatically refreshed when styles change to ensure
    /// theme updates (dark/light mode, icon/arrow assets) are properly applied.
    /// </remarks>
    internal class MapLayerUtilities
    {
        #region Dependencies
        /// <summary>
        /// The MapRuntimeState instance that holds the current map view, map class, and other related settings.
        /// </summary>
        public MapRuntimeState RuntimeState { get; set; } = new MapRuntimeState();
        #endregion

        #region Colors

        /// <summary>
        /// Gets the appropriate label color based on the current theme mode.
        /// </summary>
        /// <remarks>
        /// Returns white for dark mode to maintain visibility against dark map backgrounds,
        /// and black for light mode to maintain visibility against light map backgrounds.
        /// This property is used consistently across all text styles to ensure theme-aware label rendering.
        /// </remarks>
        private GeoColor SetLabelColor => RuntimeState.UseDarkMode ? GeoColors.White : GeoColors.Black;
        #endregion

        #region Layer and Overlay Setup

        /// <summary>
        /// Loads and configures all map overlays and feature layers, recreating them to apply current theme and style settings.
        /// </summary>
        /// <remarks>
        /// This method is typically called after theme changes (dark/light mode) or when icon/arrow assets are updated.
        /// Recreating layers ensures all visual definitions (colors, styles, icon resources) reflect the current state.
        /// Order matters: drawn features first, then placed features, arrows, and selection highlight, followed by overlay sync.
        /// Silently returns if the active map view is not initialized.
        /// </remarks>
        public void LoadOverlaysAndLayers()
        {
            if (RuntimeState.ActiveMapView == null)
            {
                return;
            }

            // Recreate layers from settings each time so theme changes and refreshed style assets are applied consistently.
            CreateDrawnFeaturesLayer();
            CreatePlacedFeaturesLayer();
            CreateSelectionHighlightLayer();
            CreateArrowFeaturesLayer();
            EnsureOverlaysConfigured();
        }

        /// <summary>
        /// Creates and configures the in-memory feature layer for drawn shapes (polygons, circles, and lines).
        /// </summary>
        /// <remarks>
        /// Drawn features share one unified schema containing columns for both line and polygon attributes, allowing
        /// the layer to hold any drawn shape type simultaneously. The layer uses value-based styling:
        /// - IsLine column differentiates line styles (width, color) from invisible placeholders
        /// - IsCircle column differentiates circle fills (radial gradient) from polygon fills (solid color)
        /// - Scale factors (1-5) determine label sizing and overlap suppression rules per zoom bucket
        /// - Line labels follow route paths with unlimited overlap/duplicate suppression for operational visibility
        /// </remarks>
        public void CreateDrawnFeaturesLayer()
        {
            // Clear any existing custom styles to ensure a clean slate for the new layer.
            ClearLayerCustomStyles(RuntimeState.DrawnFeaturesLayer);

            // Drawn features share one schema for polygons, circles, and lines so the layer can hold any drawn shape type.
            RuntimeState.DrawnFeaturesLayer = new InMemoryFeatureLayer(
               new Collection<FeatureSourceColumn>()
               {
                new FeatureSourceColumn(MapConstants.NameColumn),
                new FeatureSourceColumn(MapConstants.LabelNameColumn),
                new FeatureSourceColumn(MapConstants.LineLabelNameColumn),
                new FeatureSourceColumn(MapConstants.ArrowNameColumn),
                new FeatureSourceColumn(MapConstants.IsLineColumn),
                new FeatureSourceColumn(MapConstants.LineVerticesColumn),
                new FeatureSourceColumn(MapConstants.GeneratedLineVerticesColumn),
                new FeatureSourceColumn(MapConstants.IsCircleColumn),
                new FeatureSourceColumn(MapConstants.CircleCenterXColumn),
                new FeatureSourceColumn(MapConstants.CircleCenterYColumn),
                new FeatureSourceColumn(MapConstants.CircleRadiusColumn),
                new FeatureSourceColumn(MapConstants.PolygonVerticesColumn),
                new FeatureSourceColumn(MapConstants.GeneratedPolygonVerticesColumn)
               }, new Collection<BaseShape>());

            // Disable default rendering so all visible output comes from the explicit value/text styles defined below.
            RuntimeState.DrawnFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.IsActive = false;

            // Create the line style for drawn features.
            var lineStyle = new ValueStyle { ColumnName = MapConstants.IsLineColumn, Name = MapConstants.DrawnFeatureLineStyle };

            // Use a light gray line color for dark mode and black for light mode to ensure visibility against the map background.
            var lineColor = RuntimeState.UseDarkMode ? GeoColors.LightGray : GeoColors.Black;

            // Add a value item for lines (IsLine = 1) with the specified line color, width, and anti-aliasing enabled.s
            lineStyle.ValueItems.Add(new ValueItem("1", LineStyle.CreateSimpleLineStyle(lineColor, .5f, true)));

            // Add a value item for non-lines (IsLine = 0) with a transparent line style to effectively hide them.
            lineStyle.ValueItems.Add(new ValueItem("0", LineStyle.CreateSimpleLineStyle(GeoColors.Transparent, .5f, true)));

            // Create the fill style for drawn features, differentiating between circles and polygons using the IsCircle column.
            var shapeFillStyle = new ValueStyle { ColumnName = MapConstants.IsCircleColumn, Name = MapConstants.DrawnFeatureFillStyle };

            // Circle and polygon fills are separated by IsCircle so both geometry types can coexist in the same feature layer.
            shapeFillStyle.ValueItems.Add(new ValueItem("1",
                                          new AreaStyle(new GeoPen(MapConstants.ColorMediumRed),
                                                        new GeoRadialGradientBrush(MapConstants.ColorRed,
                                                                                   MapConstants.ColorDarkRed))));
            shapeFillStyle.ValueItems.Add(new ValueItem("0",
                                                        AreaStyle.CreateSimpleAreaStyle(MapConstants.ColorLightPurple,
                                                                                        MapConstants.ColorDarkPurple)));

            // Loop through the scale factors to create and configure label styles for each zoom bucket.
            for (int scaleFactor = 1; scaleFactor <= 5; scaleFactor++)
            {
                // Scale labels by zoom bucket instead of every zoom level to keep size changes readable and predictable.
                double scale = (double)(MapConstants.DefaultScaleOffset - scaleFactor + 1) / 5;

                var labelStyle = CreateTextStyleToScale(MapConstants.LabelTextStyle, MapConstants.LabelNameColumn, scale, SetLabelColor, 0);
                // Overlap and duplicate suppression are relaxed because operational map labels often need to stay visible
                // even when features are clustered or share similar names.
                labelStyle.OverlappingRule = LabelOverlappingRule.AllowOverlapping;
                labelStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
                labelStyle.SuppressPartialLabels = false;

                // Line labels need a separate style because they follow the route path instead of rendering at a single centroid.
                var lineLabelStyle = CreateTextStyleToScale($"{MapConstants.LabelTextStyle}Line", MapConstants.LineLabelNameColumn, scale, SetLabelColor, 10);
                lineLabelStyle.OverlappingRule = LabelOverlappingRule.AllowOverlapping;
                lineLabelStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
                lineLabelStyle.SuppressPartialLabels = false;
                lineLabelStyle.SplineType = SplineType.StandardSplining;
                lineLabelStyle.FittingLineInScreen = true;
                lineLabelStyle.LabelAllLineParts = true;
                // Let ThinkGeo treat the full route as eligible label space instead of clipping to small line fragments.
                lineLabelStyle.TextLineSegmentRatio = double.MaxValue;

                // Apply shared fill/line styles and then add the separate line-label style on the same zoom bucket.
                var zoomLevel = SetZoomLevelByScale(RuntimeState.DrawnFeaturesLayer, scaleFactor);
                SetCustomStylesByZoomLevel(zoomLevel, true, labelStyle, shapeFillStyle);
                SetCustomTextStyleByZoomLevel(zoomLevel, lineLabelStyle);
                zoomLevel.CustomStyles.Add(lineStyle);
            }
        }

        /// <summary>
        /// Creates and configures the in-memory feature layer for placed features (icon markers) on the map.
        /// </summary>
        /// <remarks>
        /// Placed features use a minimal schema (Name, IconType, IconName columns) since their geometry is always a point.
        /// Icon rendering is fully value-based: the IconTypeColumn maps to pre-loaded GeoImage assets via RuntimeState.IconGeoImageList.
        /// Scale factors determine icon size and label visibility (labels suppressed at highest zoom level to reduce clutter).
        /// Y-offset adjustments ensure labels stay above icon markers even as icons scale with zoom.
        /// </remarks>
        public void CreatePlacedFeaturesLayer()
        {
            ClearLayerCustomStyles(RuntimeState.PlacedFeaturesLayer);

            // Placed features only need icon/name columns because their geometry is always a point shape.
            RuntimeState.PlacedFeaturesLayer = new InMemoryFeatureLayer(
               new Collection<FeatureSourceColumn>()
               {
                new FeatureSourceColumn(MapConstants.NameColumn),
                new FeatureSourceColumn(MapConstants.IconTypeColumn),
                new FeatureSourceColumn(MapConstants.IconNameColumn)
               }, new Collection<BaseShape>());

            // Disable all defaults so icon features only render through the icon/name value styles configured per zoom bucket.
            RuntimeState.PlacedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.IsActive = false;
            RuntimeState.PlacedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.IsActive = false;
            RuntimeState.PlacedFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.IsActive = false;

            // Loop through the scale factors to create and configure icon and label styles for each zoom bucket.
            for (int scaleFactor = 1; scaleFactor <= 5; scaleFactor++)
            {
                // Reuse the same zoom bucket progression as drawn features so icon and shape labels scale together.
                double scale = (double)(MapConstants.DefaultScaleOffset - scaleFactor + 1) / 5;
                // Move text upward as icons scale so labels continue to sit above the marker instead of overlapping it.
                float fontYOffset = MapConstants.IconLabelYOffset * (float)scale;

                var labelStyle = CreateTextStyleToScale(MapConstants.LabelTextStyle, MapConstants.NameColumn, scale, SetLabelColor, fontYOffset);
                // Icon labels intentionally allow collisions because the icon symbol itself remains the primary locator.
                labelStyle.OverlappingRule = LabelOverlappingRule.AllowOverlapping;
                labelStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
                labelStyle.SuppressPartialLabels = false;

                var iconStyle = new ValueStyle { Name = MapConstants.LabelIconStyle, ColumnName = MapConstants.IconTypeColumn };
                foreach (DictionaryEntry item in RuntimeState.IconGeoImageIndexMap)
                {
                    var value = (int?)item.Value ?? -1;
                    if (value >= 0 && value < RuntimeState.IconGeoImageList.Count)
                    {
                        // Value styles let each icon type resolve to a pre-loaded image without rebuilding the layer per feature.
                        iconStyle.ValueItems.Add(new ValueItem(value.ToString(),
                            CreateImagePointStyleToScale(RuntimeState.IconGeoImageList[value], scale)));
                    }
                }

                // Highest zoom skips point labels to reduce clutter when icons are already visually distinct.
                var showLabel = scaleFactor != 5;
                SetCustomStylesByZoomLevel(SetZoomLevelByScale(RuntimeState.PlacedFeaturesLayer, scaleFactor), showLabel, labelStyle, iconStyle);
            }
        }

        /// <summary>
        /// Creates and configures the in-memory feature layer for generated line arrows.
        /// </summary>
        /// <remarks>
        /// Arrow features reuse the drawn-feature schema so generated arrow points can move through the same feature pipeline.
        /// Rendering is purely name-based: the ArrowNameColumn (8 compass directions: N, NE, E, SE, S, SW, W, NW) maps
        /// to pre-loaded directional images via RuntimeState.ArrowGeoImageIndexMap.
        /// Arrow images are rendered at half scale to emphasize line direction without competing with point markers.
        /// </remarks>
        public void CreateArrowFeaturesLayer()
        {
            // Clear any existing custom styles to ensure a clean slate for the new layer.
            ClearLayerCustomStyles(RuntimeState.ArrowFeaturesLayer);

            // Arrow features reuse the drawn-feature schema so generated arrow points can move through the same feature pipeline.
            RuntimeState.ArrowFeaturesLayer = new InMemoryFeatureLayer(
               new Collection<FeatureSourceColumn>()
               {
                new FeatureSourceColumn(MapConstants.NameColumn),
                new FeatureSourceColumn(MapConstants.LabelNameColumn),
                new FeatureSourceColumn(MapConstants.LineLabelNameColumn),
                new FeatureSourceColumn(MapConstants.ArrowNameColumn),
                new FeatureSourceColumn(MapConstants.IsLineColumn),
                new FeatureSourceColumn(MapConstants.LineVerticesColumn),
                new FeatureSourceColumn(MapConstants.GeneratedLineVerticesColumn),
                new FeatureSourceColumn(MapConstants.IsCircleColumn),
                new FeatureSourceColumn(MapConstants.CircleCenterXColumn),
                new FeatureSourceColumn(MapConstants.CircleCenterYColumn),
                new FeatureSourceColumn(MapConstants.CircleRadiusColumn),
                new FeatureSourceColumn(MapConstants.PolygonVerticesColumn),
                new FeatureSourceColumn(MapConstants.GeneratedPolygonVerticesColumn)
               }, new Collection<BaseShape>());

            // Arrows are rendered exclusively through direction-based value styles, so all default feature styles stay off.
            RuntimeState.ArrowFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.IsActive = false;
            RuntimeState.ArrowFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.IsActive = false;
            RuntimeState.ArrowFeaturesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.IsActive = false;

            // Loop through the scale factors to create and configure arrow styles for each zoom bucket.
            for (int scaleFactor = 1; scaleFactor <= 5; scaleFactor++)
            {
                // Arrow scaling follows the same zoom buckets as other map adornments so direction markers stay visually balanced.
                double scale = (double)(MapConstants.DefaultScaleOffset - scaleFactor + 1) / 5;

                // Arrow features are always rendered as points, so the value style is keyed to the arrow name column.
                var arrowStyle = new ValueStyle { ColumnName = MapConstants.ArrowNameColumn, Name = MapConstants.DrawnFeatureArrowStyle };

                // Map the arrow name to the pre-loaded image index so each arrow feature can resolve to the correct art asset.
                foreach (DictionaryEntry item in RuntimeState.ArrowGeoImageIndexMap)
                {
                    // Arrow names are expected to be non-empty strings, and the index must be valid for the pre-loaded image list.
                    var arrowName = item.Key?.ToString();

                    // The value is expected to be an integer index into the ArrowGeoImageList, so we safely cast it and provide a default of -1
                    // for invalid entries.
                    var value = (int?)item.Value ?? -1;

                    // Only add the value item if the arrow name is valid and the index is within the bounds of the pre-loaded image list.
                    if (!string.IsNullOrWhiteSpace(arrowName) && value >= 0 && value < RuntimeState.ArrowGeoImageList.Count)
                    {
                        // Arrow art is intentionally rendered at half scale so it reads as line direction, not as a competing point marker.
                        arrowStyle.ValueItems.Add(new ValueItem(arrowName,
                            CreateImagePointStyleToScale(RuntimeState.ArrowGeoImageList[value], scale / 2)));
                    }
                }

                // Apply the arrow value style to the appropriate zoom level based on the current scale factor.
                var zoomLevel = SetZoomLevelByScale(RuntimeState.ArrowFeaturesLayer, scaleFactor);
                zoomLevel.CustomStyles.Add(arrowStyle);
            }
        }

        /// <summary>
        /// Configures the in-memory feature layer for selection highlight visualization.
        /// </summary>
        /// <remarks>
        /// Selection highlighting uses direct default styles (not value-based) since rendering is interaction-driven, not data-driven.
        /// The layer spans the full zoom range (Level01 to Level20) because selection state is persistent across zoom levels.
        /// Highlights include circles for point selections, area fills for polygons, and lines for line selections,
        /// using light/dark blue colors for visual distinction from operational features.
        /// </remarks>
        public void CreateSelectionHighlightLayer()
        {
            // Clear any existing custom styles to ensure a clean slate for the new layer.
            RuntimeState.SelectionHighlightLayer = new InMemoryFeatureLayer();

            // Selection highlighting always spans the full zoom range because it reflects interaction state, not cartographic scale.
            var zoom = RuntimeState.SelectionHighlightLayer.ZoomLevelSet.ZoomLevel01;
            zoom.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Highlight defaults are direct styles instead of value styles because selection rendering is interaction-driven, not data-driven.
            zoom.DefaultPointStyle = PointStyle.CreateSimpleCircleStyle(
                MapConstants.ColorLightBlue47, 36,
                MapConstants.ColorDarkBlue78, 3);
            zoom.DefaultAreaStyle = AreaStyle.CreateSimpleAreaStyle(
                MapConstants.ColorLightBlue31,
                MapConstants.ColorDarkBlue, 2);
            zoom.DefaultLineStyle = LineStyle.CreateSimpleLineStyle(
                MapConstants.ColorDarkBlue, 3, true);
        }

        /// <summary>
        /// Ensures overlays and their layers are registered and synchronized with current layer definitions.
        /// </summary>
        /// <remarks>
        /// This method performs three synchronization tasks:
        /// 1. Updates MapEditOverlay reference if the live map provides an EditOverlay
        /// 2. Refreshes feature layers (placed, drawn, arrows) in FeaturesOverlay by removing and re-adding them
        ///    to ensure updated style definitions take effect
        /// 3. Refreshes selection highlight layer in DynamicOverlay to keep interaction chrome separate from persisted content
        /// Remove-and-re-add pattern is necessary because ThinkGeo layer styles are cached; re-registration forces re-evaluation.
        /// </remarks>
        public void EnsureOverlaysConfigured()
        {
            // Sync edit overlay reference if the map provides one.
            if (RuntimeState.ActiveMapView?.EditOverlay != null && RuntimeState.MapEditOverlay != RuntimeState.ActiveMapView.EditOverlay)
            {
                RuntimeState.MapEditOverlay = RuntimeState.ActiveMapView.EditOverlay;
            }

            // Update feature layers in the features overlay.
            if (RuntimeState.FeaturesOverlay != null)
            {
                RefreshOverlayLayer(RuntimeState.FeaturesOverlay, MapConstants.PlacedFeaturesName, RuntimeState.PlacedFeaturesLayer);
                RefreshOverlayLayer(RuntimeState.FeaturesOverlay, MapConstants.DrawnFeaturesName, RuntimeState.DrawnFeaturesLayer);
                RefreshOverlayLayer(RuntimeState.FeaturesOverlay, MapConstants.ArrowFeaturesName, RuntimeState.ArrowFeaturesLayer);
            }

            // Update selection highlight layer in the dynamic overlay.
            if (RuntimeState.DynamicOverlay != null)
            {
                RefreshOverlayLayer(RuntimeState.DynamicOverlay, MapConstants.SelectionHiglightLayer, RuntimeState.SelectionHighlightLayer);
            }
        }

        /// <summary>
        /// Refreshes a layer in an overlay by removing and re-adding it to apply updated style definitions.
        /// </summary>
        /// <remarks>
        /// ThinkGeo caches layer styles when they're first added to an overlay. To apply updated style definitions
        /// (e.g., after theme changes), the layer must be removed and re-added to force re-evaluation of its custom styles.
        /// This operation is safe even if the layer isn't currently in the overlay.
        /// </remarks>
        private void RefreshOverlayLayer(LayerOverlay overlay, string layerName, Layer layer)
        {
            if (overlay?.Layers != null)
            {
                if (overlay.Layers.Contains(layerName))
                {
                    overlay.Layers.Remove(layerName);
                }
                overlay.Layers.Add(layerName, layer);
            }
        }
        #endregion

        #region Feature Query Helpers
        /// <summary>
        /// Returns IDs from placed and drawn layers that match the provided feature names.
        /// </summary>
        /// <remarks>
        /// Normalizes input feature names by trimming whitespace and deduplicating before querying both layers.
        /// Queries layers independently to collect all matching IDs regardless of feature type.
        /// Uses shared QueryLayerForFeatureIdsByName helper to reduce code duplication.
        /// Returns empty set if input is null/empty or no features match the provided names.
        /// </remarks>
        /// <param name="featureNames">Feature names to resolve to IDs.</param>
        /// <returns>A hash set of matching feature IDs.</returns>
        public HashSet<string> GetFeatureIdsByNames(List<string> featureNames)
        {
            var names = new HashSet<string>(StringComparer.Ordinal);
            var ids = new HashSet<string>(StringComparer.Ordinal);

            if (featureNames == null || featureNames.Count == 0)
            {
                return ids;
            }

            // Normalize feature names by trimming whitespace and deduplicating.
            foreach (var featureName in featureNames.Where(name => !string.IsNullOrWhiteSpace(name)))
            {
                names.Add(featureName.Trim());
            }

            if (names.Count == 0)
            {
                return ids;
            }

            // Query both layers for features matching the normalized names.
            QueryLayerForFeatureIdsByName(RuntimeState.PlacedFeaturesLayer, names, ids, useNameColumn: true);
            QueryLayerForFeatureIdsByName(RuntimeState.DrawnFeaturesLayer, names, ids, useNameColumn: true);

            return ids;
        }

        /// <summary>
        /// Returns candidate IDs that exist in either placed or drawn feature layers.
        /// </summary>
        /// <remarks>
        /// Validates a set of candidate feature IDs by querying both layers for actual feature presence.
        /// Useful for confirming which features in a list are currently loaded in the map.
        /// Uses shared QueryLayerForExistingFeatureIds helper with LINQ filtering to reduce duplication.
        /// Returns empty set if input is null/empty or no candidates exist in either layer.
        /// </remarks>
        /// <param name="candidateIds">Candidate feature IDs to validate.</param>
        /// <returns>A hash set containing only IDs that exist in current layers.</returns>
        public HashSet<string> GetExistingFeatureIds(HashSet<string> candidateIds)
        {
            var existing = new HashSet<string>(StringComparer.Ordinal);

            if (candidateIds == null || candidateIds.Count == 0)
            {
                return existing;
            }

            // Query both layers for features with matching IDs.
            QueryLayerForExistingFeatureIds(RuntimeState.PlacedFeaturesLayer, candidateIds, existing);
            QueryLayerForExistingFeatureIds(RuntimeState.DrawnFeaturesLayer, candidateIds, existing);

            return existing;
        }

        /// <summary>
        /// Queries a feature layer for IDs matching either names or existing candidate IDs.
        /// </summary>
        /// <remarks>
        /// Opens the layer, iterates features, and collects matching IDs based on the NameColumn value.
        /// Properly closes the layer in a finally block to ensure resource cleanup.
        /// Skips features with null/empty IDs or missing NameColumn values.
        /// </remarks>
        private void QueryLayerForFeatureIdsByName(InMemoryFeatureLayer layer, HashSet<string> names, HashSet<string> ids, bool useNameColumn)
        {
            layer.Open();
            try
            {
                foreach (Feature feature in layer.InternalFeatures)
                {
                    if (string.IsNullOrEmpty(feature.Id))
                        continue;

                    if (useNameColumn && feature.ColumnValues.TryGetValue(MapConstants.NameColumn, out var nameValue))
                    {
                        var currentName = nameValue?.ToString();
                        if (!string.IsNullOrWhiteSpace(currentName) && names.Contains(currentName))
                        {
                            ids.Add(feature.Id);
                        }
                    }
                }
            }
            finally
            {
                layer.Close();
            }
        }

        /// <summary>
        /// Queries a feature layer for existing feature IDs from a candidate set.
        /// </summary>
        /// <remarks>
        /// Opens the layer, uses LINQ Where to filter features with valid IDs that exist in the candidate set,
        /// and collects matching IDs into the result set. Properly closes the layer in a finally block.
        /// Skips features with null/empty IDs automatically via LINQ filtering.
        /// </remarks>
        private void QueryLayerForExistingFeatureIds(InMemoryFeatureLayer layer, HashSet<string> candidateIds, HashSet<string> existing)
        {
            layer.Open();
            try
            {
                foreach (var feature in layer.InternalFeatures.Where(f => !string.IsNullOrEmpty(f.Id) && candidateIds.Contains(f.Id)))
                {
                    existing.Add(feature.Id);
                }
            }
            finally
            {
                layer.Close();
            }
        }
        #endregion

        #region Style Helpers
        /// <summary>
        /// Sets the zoom level on a layer by the current scale factor, configuring which ThinkGeo zoom levels the style applies to.
        /// </summary>
        /// <remarks>
        /// Maps five scale factors (1-5) to coarse ThinkGeo zoom level groupings rather than applying styles to every individual level.
        /// This approach keeps style size changes readable and predictable across zoom operations.
        /// - Scale 1 (zoomed in): ZoomLevel17, applies to Levels 17-20
        /// - Scale 2: ZoomLevel13, applies to Levels 13-16
        /// - Scale 3 (medium): ZoomLevel09, applies to Levels 9-12
        /// - Scale 4: ZoomLevel05, applies to Levels 5-8
        /// - Scale 5 (zoomed out): ZoomLevel01, applies to Levels 1-4
        /// </remarks>
        /// <param name="layer">The feature layer to set the zoom level for.</param>
        /// <param name="scaleFactor">The scale factor used to determine the zoom level.</param>
        /// <returns>The determined ZoomLevel for the given scale factor.</returns>
        public ZoomLevel SetZoomLevelByScale(FeatureLayer layer, int scaleFactor)
        {
            // Map scale factors to zoom levels with corresponding ApplyUntilZoomLevel settings.
            var scaleZoomMap = new Dictionary<int, (ZoomLevel, ApplyUntilZoomLevel)>
            {
                { 1, (layer.ZoomLevelSet.ZoomLevel17, ApplyUntilZoomLevel.Level20) },
                { 2, (layer.ZoomLevelSet.ZoomLevel13, ApplyUntilZoomLevel.Level16) },
                { 3, (layer.ZoomLevelSet.ZoomLevel09, ApplyUntilZoomLevel.Level12) },
                { 4, (layer.ZoomLevelSet.ZoomLevel05, ApplyUntilZoomLevel.Level08) },
            };

            // Default to Level01 with Level04 if scale factor not found.
            var (zoomLevel, applyUntil) = scaleZoomMap.TryGetValue(scaleFactor, out var mapping)
                ? mapping
                : (layer.ZoomLevelSet.ZoomLevel01, ApplyUntilZoomLevel.Level04);

            zoomLevel.ApplyUntilZoomLevel = applyUntil;
            return zoomLevel;
        }

        /// <summary>
        /// Applies custom styles to a zoom level, with optional label text styling.
        /// </summary>
        /// <remarks>
        /// Always applies the ValueStyle (e.g., fill, line, icon styles) to the zoom level.
        /// Optionally applies TextStyle only if labelsVisible is true, allowing callers to suppress labels
        /// on specific zoom buckets (e.g., crowded low-zoom levels).
        /// This separation allows stacking multiple label behaviors on the same zoom level via separate calls.
        /// </remarks>
        /// <param name="zoomLevel">The zoom level to set the custom styles for.</param>
        /// <param name="labelsVisible">A boolean indicating whether labels should be visible.</param>
        /// <param name="textStyle">The text style to apply if labels are visible.</param>
        /// <param name="valueStyle">The value style to always apply.</param>
        public void SetCustomStylesByZoomLevel(ZoomLevel zoomLevel, bool labelsVisible, TextStyle textStyle, ValueStyle valueStyle)
        {
            // Value style is always applied; label style is optional so callers can suppress text on selected zoom buckets.
            zoomLevel.CustomStyles.Add(valueStyle);
            if (labelsVisible)
            {
                SetCustomTextStyleByZoomLevel(zoomLevel, textStyle);
            }
        }

        /// <summary>
        /// Applies a text style to a zoom level for label rendering.
        /// </summary>
        /// <remarks>
        /// Text styles are added separately from value styles to allow stacking multiple label behaviors
        /// on the same zoom bucket (e.g., combining point labels with line-following labels).
        /// </remarks>
        /// <param name="zoomLevel">The zoom level to set the custom text style for.</param>
        /// <param name="textStyle">The text style to apply.</param>
        public void SetCustomTextStyleByZoomLevel(ZoomLevel zoomLevel, TextStyle textStyle)
        {
            // Text styles are added separately so callers can stack multiple label behaviors on the same zoom bucket.
            zoomLevel.CustomStyles.Add(textStyle);
        }

        /// <summary>
        /// Clears all custom styles from the five explicit zoom buckets used by the map styling convention.
        /// </summary>
        /// <remarks>
        /// Iterates through the standard five zoom levels (1, 5, 9, 13, 17) and clears their custom style collections.
        /// Used at the start of layer creation to ensure a clean slate before rebuilding styles.
        /// </remarks>
        /// <param name="layer">The feature layer to clear the custom styles for.</param>
        public void ClearLayerCustomStyles(InMemoryFeatureLayer layer)
        {
            // Clear custom styles for the five explicit zoom levels used in the map styling convention.
            layer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
            layer.ZoomLevelSet.ZoomLevel05.CustomStyles.Clear();
            layer.ZoomLevelSet.ZoomLevel09.CustomStyles.Clear();
            layer.ZoomLevelSet.ZoomLevel13.CustomStyles.Clear();
            layer.ZoomLevelSet.ZoomLevel17.CustomStyles.Clear();
        }

        /// <summary>
        /// Creates a scaled text style for labels using the user font settings.
        /// </summary>
        /// <remarks>
        /// Scales the standard font size (MapConstants.FontSize) by the provided scale factor and rounds to one decimal place
        /// for consistent rendering. Configures text placement at center with center alignment and a maximum text-to-line-segment ratio
        /// to prevent line labels from being artificially clipped to short path fragments.
        /// </remarks>
        /// <param name="style">The name of the style.</param>
        /// <param name="columnName">The name of the column to use for the text.</param>
        /// <param name="scale">The scale factor to apply to the font size.</param>
        /// <param name="color">The color of the text.</param>
        /// <param name="fontYOffset">The vertical offset for the font.</param>
        /// <returns>A TextStyle object configured with the specified parameters.</returns>
        public TextStyle CreateTextStyleToScale(string style, string columnName, double scale, GeoColor color, float fontYOffset)
        {
            // Round the font size to one decimal place for consistent rendering across different zoom levels.
            float fontSize = (float)Math.Round(MapConstants.FontSize * scale, 1);

            // Create a simple text style with the specified parameters, including font name, size, style, color, and vertical offset.
            TextStyle textStyle =
                   TextStyle.CreateSimpleTextStyle(columnName, MapConstants.FontName, fontSize, DrawingFontStyles.Regular, color, 0, fontYOffset);
            textStyle.TextPlacement = TextPlacement.Center;
            textStyle.Alignment = DrawingTextAlignment.Center;
            textStyle.OverlappingRule = LabelOverlappingRule.AllowOverlapping;
            textStyle.Name = style;
            
            // A large segment ratio keeps line labels from being artificially clipped to short path fragments.
            textStyle.TextLineSegmentRatio = MapConstants.LineLabelSegmentRatio;

            // Return the configured text style for use in the layer's zoom level.
            return textStyle;
        }

        /// <summary>
        /// Creates a scaled point style from a GeoImage asset.
        /// </summary>
        /// <remarks>
        /// Uses image scaling to maintain the original art asset's quality while allowing visual sizing based on zoom level.
        /// This eliminates the need for separate image files at different sizes.
        /// Typical usage: creating icon styles from pre-loaded GeoImages or arrow direction assets.
        /// </remarks>
        /// <param name="image">The image to use for the point style.</param>
        /// <param name="scale">The scale factor to apply to the image.</param>
        /// <returns>A PointStyle object configured with the specified image and scale.</returns>
        public PointStyle CreateImagePointStyleToScale(GeoImage image, double scale) =>
            new PointStyle(image)
            {
                PointType = PointType.Image,
                ImageScale = scale
            };
        #endregion
    }
}
