namespace ThinkGeo.UI.Blazor.HowDoI.CustomerApp.SharedProperties
{
    /// <summary>
    /// Stores and organizes map feature selection state used by selection, highlighting, and visibility workflows.
    /// </summary>
    /// <remarks>
    /// This class keeps multiple synchronized selection views for different runtime concerns:
    /// selected IDs for quick membership checks, hidden selected IDs for filtered or non-visible features,
    /// selected names for UI display, and ID/name pairs for richer selection projections.
    /// </remarks>
    public class MapSelectedFeatures
    {
        #region Selection Metadata

        /// <summary>
        /// Represents minimal selected feature metadata exposed to UI and callers.
        /// </summary>
        /// <param name="Id">The unique feature identifier.</param>
        /// <param name="Name">The display name associated with the feature.</param>
        public sealed record SelectedFeatureInfo(string Id, string Name);

        #endregion

        #region Selection Sets

        /// <summary>
        /// Gets the set of feature IDs currently selected on the map.
        /// </summary>
        /// <remarks>
        /// Backed by ordinal string comparison for stable, case-sensitive identifier semantics.
        /// Used by selection and highlight logic for fast O(1) lookups.
        /// </remarks>
        public readonly HashSet<string> SelectedFeatureIds = new(StringComparer.Ordinal);

        /// <summary>
        /// Gets the set of selected feature IDs that are currently hidden from the visible map view.
        /// </summary>
        /// <remarks>
        /// Tracks features that remain selected but are not visible due to filtering, layer visibility changes,
        /// or zoom-based presentation rules.
        /// </remarks>
        public readonly HashSet<string> HiddenSelectedFeatureIds = new(StringComparer.Ordinal);

        /// <summary>
        /// Gets the set of display names for currently selected features.
        /// </summary>
        /// <remarks>
        /// Useful for quick UI projections (selection lists, counters, status text) without re-querying layers.
        /// </remarks>
        public readonly HashSet<string> SelectedFeatureNames = new(StringComparer.Ordinal);

        /// <summary>
        /// Gets the set of selected feature ID/name pairs.
        /// </summary>
        /// <remarks>
        /// Provides a compact projection for callers that need both identity and display data in a single set.
        /// </remarks>
        public readonly HashSet<SelectedFeatureInfo> SelectedFeatures = new();

        #endregion
    }
}
