using ThinkGeo.UI.Blazor.HowDoI.CustomerApp.SharedProperties;
using Microsoft.AspNetCore.Components.Web;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Blazor.HowDoI.CustomerApp.Utilities
{
    /// <summary>
    /// Utility class for handling map interaction logic including feature selection, drag-and-drop, bounds calculations, and UI scripting.
    /// </summary>
    internal class MapInteractionUtilities
    {
        /// <summary>Prefix used to identify dragged icon types in drag-and-drop events.</summary>
        private const string DragIconTypePrefix = "application/x-basemap-icon-";

        /// <summary>JavaScript template for updating the selection box position and size.</summary>
        private const string SelectionBoxUpdateScriptTemplate = "(function(){{var el=document.getElementById('ctrl-selection-box');" 
                                                              + "el.style.left='{0}px';el.style.top='{1}px';el.style.width='{2}px';" 
                                                              + "el.style.height='{3}px';}})();";

        #region UI Scripting
        /// <summary>Gets the appropriate drop effect string for drag-and-drop based on read-only state.</summary>
        /// <param name="isReadOnly">Whether the map is in read-only mode.</param>
        /// <returns>"none" if read-only, "copy" otherwise.</returns>
        public string GetDropEffect(bool isReadOnly) => isReadOnly ? "none" : "copy";

        /// <summary>Generates a JavaScript snippet to show the control selection box.</summary>
        public string GetShowCtrlSelectionBoxScript() => "document.getElementById('ctrl-selection-box').style.display='block';";

        /// <summary>Generates a JavaScript snippet to hide the control selection box.</summary>
        public string GetHideCtrlSelectionBoxScript() => "document.getElementById('ctrl-selection-box').style.display='none';";

        /// <summary>Generates a JavaScript snippet to update the mouse position label.</summary>
        /// <param name="labelTextJson">The label text as a JSON-encoded string.</param>
        public string BuildMousePositionLabelUpdateScript(string labelTextJson) =>
            $"document.getElementById('mouse-position-label').innerText = {labelTextJson};";
        #endregion

        #region Drag and Drop
        /// <summary>Extracts the dragged icon name from a drag event's data transfer object.</summary>
        /// <param name="e">The drag event arguments.</param>
        /// <returns>The decoded icon name if found, otherwise null.</returns>
        public string? GetDraggedIconName(DragEventArgs e)
        {
            // Look for a data type that starts with the DragIconTypePrefix
            var type = e.DataTransfer?.Types?
                .FirstOrDefault(t => t.StartsWith(DragIconTypePrefix, StringComparison.OrdinalIgnoreCase));

            // If no matching type is found, return null
            if (string.IsNullOrWhiteSpace(type))
            {
                return null;
            }

            // Extract the encoded name by removing the prefix and decode it
            var encodedName = type[DragIconTypePrefix.Length..];

            // Decode the name to handle any URL-encoded characters
            return Uri.UnescapeDataString(encodedName);
        }
        #endregion

        #region Client Bounds and Selection Box
        /// <summary>Calculates the bounding box formed by two client-coordinate points.</summary>
        /// <param name="startClient">The starting point in client coordinates.</param>
        /// <param name="endClient">The ending point in client coordinates.</param>
        /// <returns>A tuple containing Left, Right, Top, and Bottom bounds.</returns>
        public (double Left, double Right, double Top, double Bottom) GetClientBounds((double X, double Y) startClient, (double X, double Y) endClient)
        {
            // Calculate the minimum and maximum X and Y values to form the bounding box
            return (
                Left: Math.Min(startClient.X, endClient.X),
                Right: Math.Max(startClient.X, endClient.X),
                Top: Math.Min(startClient.Y, endClient.Y),
                Bottom: Math.Max(startClient.Y, endClient.Y));
        }

        /// <summary>Builds a JavaScript snippet that updates the selection box element's position and size.</summary>
        /// <param name="startOffset">The starting offset position.</param>
        /// <param name="currentOffset">The current offset position.</param>
        /// <returns>JavaScript code to execute for updating the selection box.</returns>
        public string BuildCtrlSelectionBoxUpdateScript((double X, double Y) startOffset, (double X, double Y) currentOffset)
        {
            // Calculate the left, top, width, and height of the selection box based on the start and current offsets
            var left = Math.Min(startOffset.X, currentOffset.X);
            var top = Math.Min(startOffset.Y, currentOffset.Y);
            var width = Math.Abs(currentOffset.X - startOffset.X);
            var height = Math.Abs(currentOffset.Y - startOffset.Y);

            // Return the formatted JavaScript snippet using the template
            return string.Format(SelectionBoxUpdateScriptTemplate, left, top, width, height);
        }
        #endregion

        #region Distance Calculations
        /// <summary>Determines whether the distance between two points meets or exceeds a minimum threshold.</summary>
        /// <param name="start">The starting point.</param>
        /// <param name="end">The ending point.</param>
        /// <param name="minimumDistance">The minimum distance required.</param>
        /// <returns>True if the distance is at least the minimum; otherwise false.</returns>
        public bool IsDistanceAtLeast((double X, double Y) start, (double X, double Y) end, double minimumDistance) =>
            GetDistance(start, end) >= minimumDistance;

        /// <summary>Determines whether the distance between two points does not exceed a maximum threshold.</summary>
        /// <param name="start">The starting point.</param>
        /// <param name="end">The ending point.</param>
        /// <param name="maxDistance">The maximum distance allowed.</param>
        /// <returns>True if the distance is within the maximum; otherwise false.</returns>
        public bool IsDistanceWithin((double X, double Y) start, (double X, double Y) end, double maxDistance) =>
            GetDistance(start, end) <= maxDistance;

        /// <summary>Calculates the Euclidean distance between two points.</summary>
        /// <param name="start">The starting point.</param>
        /// <param name="end">The ending point.</param>
        /// <returns>The Euclidean distance between the points.</returns>
        private static double GetDistance((double X, double Y) start, (double X, double Y) end)
        {
            // Calculate the differences in X and Y coordinates
            var dx = end.X - start.X;
            var dy = end.Y - start.Y;

            // Return the Euclidean distance using the Pythagorean theorem
            return Math.Sqrt((dx * dx) + (dy * dy));
        }
        #endregion

        #region Feature Selection
        /// <summary>Applies single-feature selection logic, handling both normal click and Ctrl+Click toggles.</summary>
        /// <param name="selectedFeatureIds">The current set of selected feature IDs.</param>
        /// <param name="featureId">The ID of the feature being selected, or null to clear selection.</param>
        /// <param name="ctrlKey">Whether the Ctrl key was pressed; if true, toggles the feature; if false, replaces selection.</param>
        public void ApplySingleFeatureSelection(HashSet<string> selectedFeatureIds, string? featureId, bool ctrlKey)
        {
            // If no feature ID is provided, clear selection unless Ctrl is pressed
            if (string.IsNullOrEmpty(featureId))
            {
                // If Ctrl is not pressed, clear the selection set
                if (!ctrlKey)
                {
                    selectedFeatureIds.Clear();
                }

                return;
            }

            // If Ctrl is pressed, toggle the selection state of the feature
            if (ctrlKey)
            {
                // Toggle: add if not present, remove if already selected.
                if (!selectedFeatureIds.Add(featureId))
                {
                    selectedFeatureIds.Remove(featureId);
                }

                return;
            }

            // If Ctrl is not pressed, clear the selection and select only the new feature
            selectedFeatureIds.Clear();
            selectedFeatureIds.Add(featureId);
        }
        #endregion

        #region Wrapped Selection Bounds
        /// <summary>Creates a selection bounds object that correctly handles dateline wrapping.</summary>
        /// <param name="topLeftCoords">The top-left corner coordinate.</param>
        /// <param name="bottomRightCoords">The bottom-right corner coordinate.</param>
        /// <param name="mapUnit">The map's geographic unit (Meter or DecimalDegrees).</param>
        /// <param name="anchorX">The x-coordinate anchor for normalization.</param>
        /// <returns>A WrappedSelectionBounds object.</returns>
        public WrappedSelectionBounds CreateWrappedSelectionBounds(PointShape topLeftCoords, PointShape bottomRightCoords, GeographyUnit mapUnit, double anchorX)
        {
            // Determine the world width based on the map's geographic unit.
            var worldWidth = mapUnit == GeographyUnit.Meter
                ? MapConstants.WorldWidthInMeters
                : MapConstants.WorldWidthInDecimalDegrees;

            // Determine half the world width based on the map's geographic unit for normalization purposes.
            var halfWorldWidth = mapUnit == GeographyUnit.Meter
                ? MapConstants.HalfWorldWidthInMeters
                : MapConstants.HalfWorldWidthInDecimalDegrees;

            // Calculate the minimum and maximum Y coordinates for the selection bounds.
            var minY = Math.Min(topLeftCoords.Y, bottomRightCoords.Y);
            var maxY = Math.Max(topLeftCoords.Y, bottomRightCoords.Y);

            // Normalize the X coordinates to the anchor point to handle dateline wrapping.
            var x1 = NormalizeXToAnchor(topLeftCoords.X, anchorX, worldWidth, halfWorldWidth);
            var x2 = NormalizeXToAnchor(bottomRightCoords.X, anchorX, worldWidth, halfWorldWidth);

            // Determine the minimum and maximum X coordinates for the selection bounds.
            var minX = Math.Min(x1, x2);
            var maxX = Math.Max(x1, x2);

            // Check if the selection box crosses the dateline by comparing the absolute difference of normalized X coordinates to half the world
            // width.
            var crossesDateline = Math.Abs(x1 - x2) > halfWorldWidth;

            // Return a new WrappedSelectionBounds object encapsulating the calculated bounds and properties.
            return new WrappedSelectionBounds(minX, maxX, minY, maxY, anchorX, worldWidth, halfWorldWidth, crossesDateline);
        }

        /// <summary>Determines whether a point is inside a dateline-aware selection box.</summary>
        /// <param name="point">The point to test.</param>
        /// <param name="bounds">The wrapped selection bounds.</param>
        /// <returns>True if the point is inside the bounds; otherwise false.</returns>
        public bool IsInsideWrappedSelectionBox(PointShape point, WrappedSelectionBounds bounds)
        {
            // First, check if the point's Y coordinate is outside the vertical bounds of the selection box.
            if (point.Y < bounds.MinY || point.Y > bounds.MaxY)
            {
                return false;
            }

            // Normalize the point's X coordinate to the anchor point to handle dateline wrapping.
            var px = NormalizeXToAnchor(point.X, bounds.AnchorX, bounds.WorldWidth, bounds.HalfWorldWidth);

            // If the selection box crosses the dateline, check if the point's X coordinate is either greater than the max X or less than the min X.
            return bounds.CrossesDateline
                ? (px >= bounds.MaxX || px <= bounds.MinX)
                : (px >= bounds.MinX && px <= bounds.MaxX);
        }
        #endregion

        #region Internal Helpers
        /// <summary>Normalizes an x-coordinate to ensure it remains within half a world width of an anchor point, handling dateline wrapping.</summary>
        /// <param name="x">The x-coordinate to normalize.</param>
        /// <param name="anchor">The anchor x-coordinate.</param>
        /// <param name="worldWidth">The total world width in map units.</param>
        /// <param name="halfWorldWidth">Half of the world width.</param>
        /// <returns>The normalized x-coordinate.</returns>
        private static double NormalizeXToAnchor(double x, double anchor, double worldWidth, double halfWorldWidth)
        {
            // Adjust the x-coordinate to ensure it is within half a world width of the anchor point, wrapping around if necessary.
            while (x - anchor > halfWorldWidth)
            {
                x -= worldWidth;
            }

            while (x - anchor < -halfWorldWidth)
            {
                x += worldWidth;
            }

            // Return the normalized x-coordinate.
            return x;
        }
        #endregion

        /// <summary>
        /// Represents the bounds of a selection box that may cross the dateline, including normalized coordinates and relevant properties for
        /// selection logic.
        /// </summary>
        /// <param name="MinX">The minimum X coordinate of the selection box.</param>
        /// <param name="MaxX">The maximum X coordinate of the selection box.</param>
        /// <param name="MinY">The minimum Y coordinate of the selection box.</param>
        /// <param name="MaxY">The maximum Y coordinate of the selection box.</param>
        /// <param name="AnchorX">The anchor X coordinate used for dateline wrapping.</param>
        /// <param name="WorldWidth">The total world width in map units.</param>
        /// <param name="HalfWorldWidth">Half of the total world width in map units.</param>
        /// <param name="CrossesDateline">Indicates whether the selection box crosses the dateline.</param>
        internal readonly record struct WrappedSelectionBounds(
            double MinX,
            double MaxX,
            double MinY,
            double MaxY,
            double AnchorX,
            double WorldWidth,
            double HalfWorldWidth,
            bool CrossesDateline);
    }
}
