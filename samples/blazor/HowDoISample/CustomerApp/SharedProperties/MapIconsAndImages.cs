using System.Collections;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Blazor.HowDoI.CustomerApp.SharedProperties
{
    /// <summary>
    /// Stores runtime image catalogs and lookup maps for map icons and directional arrows.
    /// </summary>
    /// <remarks>
    /// This state container keeps loaded <see cref="GeoImage"/> assets and their corresponding name-to-index maps.
    /// Utilities use these collections to resolve image styles efficiently without repeatedly scanning the file system.
    /// </remarks>
    public class MapIconsAndImages
    {
        #region Icon Image Catalog

        /// <summary>
        /// Loaded icon images used to build placed-feature styles.
        /// </summary>
        /// <remarks>
        /// The index position is the canonical style lookup key for icon markers.
        /// </remarks>
        public List<GeoImage> IconGeoImageList { get; } = new List<GeoImage>();

        /// <summary>
        /// Maps icon names to indexes in <see cref="IconGeoImageList"/>.
        /// </summary>
        /// <remarks>
        /// Keys are catalog names and values are integer indexes into <see cref="IconGeoImageList"/>.
        /// </remarks>
        public SortedList IconGeoImageIndexMap { get; } = new SortedList();

        #endregion

        #region Arrow Image Catalog

        /// <summary>
        /// Loaded arrow images used to build line-arrow styles.
        /// </summary>
        /// <remarks>
        /// Contains directional arrow assets (for example N/NE/E...) used for line direction rendering.
        /// </remarks>
        public List<GeoImage> ArrowGeoImageList { get; } = new List<GeoImage>();

        /// <summary>
        /// Maps arrow names to indexes in <see cref="ArrowGeoImageList"/>.
        /// </summary>
        /// <remarks>
        /// Keys are direction names and values are integer indexes into <see cref="ArrowGeoImageList"/>.
        /// </remarks>
        public SortedList ArrowGeoImageIndexMap { get; } = new SortedList();

        #endregion
    }
}
