using ThinkGeo.UI.Blazor.HowDoI.CustomerApp.Classes;
using ThinkGeo.UI.Blazor.HowDoI.CustomerApp.SharedProperties;
using System.Collections;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Blazor.HowDoI.CustomerApp.Utilities
{
    /// <summary>
    /// Provides icon and arrow image catalog loading, lookup, and canonicalization for map feature styling.
    /// </summary>
    /// <remarks>
    /// Images are loaded from disk into ordered <see cref="GeoImage"/> lists with parallel name-to-index maps,
    /// allowing style code to resolve image assets quickly at runtime. The utility also normalizes user-provided
    /// image names through case-insensitive canonical lookup and supports resilient directory resolution across
    /// local development, test hosts, and published deployments.
    /// </remarks>
    internal class MapGeoImageUtilities
    {
        #region Dependencies and Runtime Projections

        /// <summary>
        /// Gets or sets the runtime map state used to access shared image collections and index maps.
        /// </summary>
        public MapRuntimeState RuntimeState { get; set; } = new MapRuntimeState();

        /// <summary>
        /// Gets the loaded icon image catalog.
        /// </summary>
        /// <remarks>Delegates to <see cref="MapRuntimeState.IconGeoImageList"/>.</remarks>
        public List<GeoImage> IconGeoImageList => RuntimeState.IconGeoImageList;

        /// <summary>
        /// Gets the icon name-to-index lookup map.
        /// </summary>
        /// <remarks>Delegates to <see cref="MapRuntimeState.IconGeoImageIndexMap"/>.</remarks>
        public SortedList IconGeoImageIndexMap => RuntimeState.IconGeoImageIndexMap;

        /// <summary>
        /// Gets the loaded arrow image catalog.
        /// </summary>
        /// <remarks>Delegates to <see cref="MapRuntimeState.ArrowGeoImageList"/>.</remarks>
        public List<GeoImage> ArrowGeoImageList => RuntimeState.ArrowGeoImageList;

        /// <summary>
        /// Gets the arrow direction name-to-index lookup map.
        /// </summary>
        /// <remarks>Delegates to <see cref="MapRuntimeState.ArrowGeoImageIndexMap"/>.</remarks>
        public SortedList ArrowGeoImageIndexMap => RuntimeState.ArrowGeoImageIndexMap;

        #endregion

        #region Catalog Loading and Index Lookup

        /// <summary>
        /// Load geographic images from the "Icons" directory located at the base path specified in Settings. 
        /// This method clears the existing GeoImageList and GeoImageIndexMap, then iterates through all PNG files in the 
        /// "Icons" directory. For each PNG file, it creates a new GeoImage and adds it to the GeoImageList, while also 
        /// adding an entry to the GeoImageIndexMap with the filename (without extension) as the key and the index of the 
        /// image in the list as the value. If the "Icons" directory does not exist, the method simply returns without 
        /// loading any icons.
        /// </summary>
        /// <param name="directoryName">The name of the directory containing the geographic images.</param>
        /// <param name="geoImageIndexMap">The sorted list to populate with image identifiers and their corresponding index values.</param>
        /// <param name="geoImageList">The list to populate with GeoImage objects.</param>
        /// <remarks>
        /// Files are loaded in deterministic name order so index assignment remains stable between runs.
        /// </remarks>
        public void LoadGeoImages(string directoryName, SortedList geoImageIndexMap, List<GeoImage> geoImageList)
        {
            // Construct the path to the "Icons" directory based on the base path specified in Settings.
            var imagePath = ResolveDirectoryPath(directoryName);

            // Clear the existing GeoImageList and GeoImageIndexMap to prepare for loading new icons.
            geoImageList.Clear();
            geoImageIndexMap.Clear();

            // Check if the "Icons" directory exists. If it does not exist, return without loading any icons.
            if (!Directory.Exists(imagePath))
            {
                return;
            }

            // Iterate through all PNG files in the specified directory, ordered by name, create a new GeoImage for each file, 
            // and add it to the GeoImageList, while also adding an entry to the GeoImageIndexMap with the filename as the key
            // and the index of the image in the list as the value.
            var files = Directory.GetFiles(imagePath, "*" + MapConstants.MapImageFileExtension)
                .OrderBy(filename => filename)
                .ToArray();

            for (var index = 0; index < files.Length; index++)
            {
                var filename = files[index];
                string name = Path.GetFileNameWithoutExtension(filename);
                geoImageList.Add(new GeoImage(filename));
                geoImageIndexMap[name] = index;
            }
        }

        /// <summary>
        /// Get the index of an icon geographic image based on its name. This method checks the IconGeoImageIndexMap for
        /// the specified name and returns the corresponding index value.
        /// </summary>
        /// <param name="name">The name of the icon geographic image.</param>
        /// <returns>The index of the icon geographic image, or -1 if not found.</returns>
        /// <remarks>
        /// Delegates to <see cref="GetGeoImageIndex(string, string, SortedList, List{GeoImage})"/> using the icons directory.
        /// </remarks>
        public int GetIconGeoImageIndex(string name)
        {
            // Call the GetGeoImageIndex method with the appropriate parameters for icons.
            return GetGeoImageIndex(name, MapConstants.IconsDirectoryName, IconGeoImageIndexMap, IconGeoImageList);
        }

        /// <summary>
        /// Get the index of an arrow geographic image based on its name. This method checks the ArrowGeoImageIndexMap for
        /// the specified name and returns the corresponding index value.
        /// </summary>
        /// <param name="name">The name of the arrow geographic image.</param>
        /// <returns>The index of the arrow geographic image, or -1 if not found.</returns>
        /// <remarks>
        /// Delegates to <see cref="GetGeoImageIndex(string, string, SortedList, List{GeoImage})"/> using the arrows directory.
        /// </remarks>
        public int GetArrowGeoImageIndex(string name)
        {
            // Call the GetGeoImageIndex method with the appropriate parameters for arrows.
            return GetGeoImageIndex(name, MapConstants.ArrowsDirectoryName, ArrowGeoImageIndexMap, ArrowGeoImageList);
        }

        /// <summary>
        /// Get the index of a geographic image based on its name. This method checks the GeoImageIndexMap for the specified name and returns the corresponding index value. 
        /// If the name is not found, it attempts to load the icons if they haven't been loaded yet. If the name is still not found 
        /// after loading, it returns the index for a Settings.MissingIconName if available, or -1 if not found.
        /// </summary>
        /// <param name="name">The name of the geographic image.</param>
        /// <param name="directoryName">The name of the directory containing the geographic images.</param>
        /// <param name="geoImageIndexMap">The map of geographic image names to their corresponding indices.</param>
        /// <param name="geoImageList">The list of geographic images.</param>
        /// <returns>The index of the geographic image, or -1 if not found.</returns>
        /// <remarks>
        /// Performs exact lookup first, then case-insensitive lookup, and finally falls back to
        /// <see cref="MapConstants.MissingIconName"/> when available.
        /// </remarks>
        public int GetGeoImageIndex(string name, string directoryName, SortedList geoImageIndexMap, List<GeoImage> geoImageList)
        {
            // Ensure the image catalog is loaded before attempting to look up the index. If loading fails, return -1.
            if (!EnsureGeoImageCatalogLoaded(directoryName, geoImageIndexMap, geoImageList))
            {
                return -1;
            }

            // If the name is null, empty, or whitespace, attempt to return the index for the missing icon.
            if (string.IsNullOrWhiteSpace(name))
            {
                return TryGetIndex(geoImageIndexMap, MapConstants.MissingIconName, out var missingIconIndex)
                    ? missingIconIndex
                    : -1;
            }

            // Attempt to get the index for the exact name first. If found, return it.
            if (TryGetIndex(geoImageIndexMap, name, out var exactMatchIndex))
            {
                return exactMatchIndex;
            }

            // If the exact name is not found, attempt a case-insensitive lookup. If found, return it.
            if (TryGetCaseInsensitiveIndex(geoImageIndexMap, name, out var caseInsensitiveIndex))
            {
                return caseInsensitiveIndex;
            }

            // If the name is still not found, attempt to return the index for the missing icon.
            return TryGetIndex(geoImageIndexMap, MapConstants.MissingIconName, out var fallbackMissingIconIndex)
                ? fallbackMissingIconIndex
                : -1;
        }

        #endregion

        #region Canonical Name Helpers

        /// <summary>   
        /// Get the canonical name of an icon geographic image based on its name.
        /// </summary>
        /// <param name="name">The name of the icon geographic image.</param>
        /// <returns>The canonical name of the icon geographic image.</returns> 
        /// <remarks>
        /// Returns the catalog key casing used in the icon index map when a case-insensitive match exists.
        /// </remarks>
        public string GetCanonicalIconGeoImageName(string name)
        {
            return GetCanonicalGeoImageName(name, MapConstants.IconsDirectoryName, IconGeoImageIndexMap, IconGeoImageList);
        }

        /// <summary>
        /// Get the canonical name of an arrow geographic image based on its name.
        /// </summary>
        /// <param name="name">The name of the arrow geographic image.</param>
        /// <returns>The canonical name of the arrow geographic image.</returns>
        /// <remarks>
        /// Returns the catalog key casing used in the arrow index map when a case-insensitive match exists.
        /// </remarks>
        public string GetCanonicalArrowGeoImageName(string name)
        {
            return GetCanonicalGeoImageName(name, MapConstants.ArrowsDirectoryName, ArrowGeoImageIndexMap, ArrowGeoImageList);
        }

        /// <summary>
        /// Get the canonical name of a geographic image based on its name. This method checks the GeoImageIndexMap for the 
        /// specified name and returns the corresponding key if found.
        /// </summary>
        /// <param name="name">The name of the geographic image.</param>
        /// <param name="directoryName">The name of the directory containing the geographic images.</param>
        /// <param name="geoImageIndexMap">The map of geographic image names to their corresponding indices.</param>
        /// <param name="geoImageList">The list of geographic images.</param>
        /// <returns>The canonical name of the geographic image.</returns>
        /// <remarks>
        /// If the name is not present in the loaded catalog, returns the original input unchanged.
        /// </remarks>
        public string GetCanonicalGeoImageName(string? name, string directoryName, SortedList geoImageIndexMap, List<GeoImage> geoImageList)
        {
            // Ensure the image catalog is loaded before attempting to look up the canonical name. If loading fails, return the original name.
            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Empty;
            }

            // Ensure the image catalog is loaded before attempting to look up the canonical name. If loading fails, return the original name.
            if (!EnsureGeoImageCatalogLoaded(directoryName, geoImageIndexMap, geoImageList))
            {
                return name;
            }

            // Attempt to get the canonical key for the input name using case-insensitive matching. If found, return it; otherwise, return the
            // original name.
            return TryGetCaseInsensitiveKey(geoImageIndexMap, name, out var canonicalKey)
                ? canonicalKey
                : name;
        }

        #endregion

        #region Directory Resolution

        /// <summary>
        /// Resolve the directory path where the map icons are stored. This method checks multiple potential locations for the icons, 
        /// including the base directory of the application, the location of the BaseMap assembly, and the current working directory. 
        /// It also performs an upward search from the application's base directory to find a sibling BaseMap project that contains the 
        /// icons. The method ensures that the selected directory actually contains icon PNG files before returning it. If no valid 
        /// directory is found, it falls back to a default relative path.
        /// </summary>
        /// <param name="directoryName">The name of the directory containing the icons.</param>
        /// <returns>The resolved directory path.</returns>
        /// <remarks>
        /// The first candidate containing PNG files wins. If none are found, returns the default relative path
        /// to preserve existing "no icons available" behavior.
        /// </remarks>
        public string ResolveDirectoryPath(string directoryName)
        {
            // Construct the default relative path to the icons directory based on the web root and the specified directory name.
            var directoryPath = Path.Combine(MapConstants.WebRoot, directoryName);

            // A candidate only wins if it actually contains icon PNGs. This prevents an empty
            // wwwroot\Icons folder (for example one materialized by static web asset tooling in
            // the host project's source tree) from shadowing the real icons that the host copies
            // into its output/publish directory.
            static bool ContainsIcons(string path) =>
                Directory.Exists(path) && Directory.EnumerateFiles(path, "*" + MapConstants.MapImageFileExtension).Any();

            // Path next to the running app's base directory. The host project copies the
            // BaseMap icons into wwwroot\Icons of its output/publish directory, so this is the
            // primary location in both local dev and published deployments.
            var assemblyDirectory = Path.GetDirectoryName(typeof(MapGeoImageUtilities).Assembly.Location);
            var candidatePaths = new List<string>
            {
                Path.Combine(AppContext.BaseDirectory, MapConstants.WebRoot, directoryName),
                Path.Combine(Directory.GetCurrentDirectory(), MapConstants.WebRoot, directoryName)
            };

            // Resolve relative to the BaseMap assembly location so callers running from a
            // different current directory can still find copied static assets adjacent to the
            // compiled binaries.
            if (!string.IsNullOrWhiteSpace(assemblyDirectory))
            {
                candidatePaths.Insert(1, Path.Combine(assemblyDirectory, MapConstants.WebRoot, directoryName));
            }

            // Check each candidate path for the presence of icon PNG files. Return the first valid path found.
            foreach (var candidatePath in candidatePaths)
            {
                if (ContainsIcons(candidatePath))
                {
                    return candidatePath;
                }
            }

            // Walk up from the app base directory to locate a sibling BaseMap project folder.
            // This helps test hosts and tooling executions where static web assets are not copied
            // yet but project sources are present.
            var current = new DirectoryInfo(AppContext.BaseDirectory);
            while (current != null)
            {
                // Look for a sibling BaseMap project folder containing the icons. This is useful for local development scenarios where the
                // host project and BaseMap are in the same solution.
                var siblingBaseMapCandidate = Path.Combine(current.FullName, MapConstants.BaseMapFolder, MapConstants.WebRoot, directoryName);
                if (ContainsIcons(siblingBaseMapCandidate))
                {
                    return siblingBaseMapCandidate;
                }

                // Move up one directory level to continue the search.
                current = current.Parent;
            }

            // Fall back to the original relative path so the existing "no icons" behavior is
            // preserved rather than throwing.
            return directoryPath;
        }

        #endregion

        #region Private Lookup Helpers

        /// <summary>
        /// Ensures the image catalog map/list are initialized and loaded for lookup operations.
        /// </summary>
        /// <param name="directoryName">The catalog directory name to load when the map is empty.</param>
        /// <param name="geoImageIndexMap">The name-to-index lookup map.</param>
        /// <param name="geoImageList">The loaded image list.</param>
        /// <returns>
        /// <see langword="true"/> when lookup structures are ready for use; otherwise, <see langword="false"/>.
        /// </returns>
        /// <remarks>
        /// Returns false when required structures are null or when loading is unavailable on the current platform.
        /// </remarks>
        private bool EnsureGeoImageCatalogLoaded(string directoryName, SortedList geoImageIndexMap, List<GeoImage> geoImageList)
        {
            // If either the index map or image list is null, return false to indicate that the catalog cannot be loaded.
            if (geoImageIndexMap == null || geoImageList == null)
            {
                return false;
            }

            // If the index map already contains entries, return true to indicate that the catalog is already loaded.
            if (geoImageIndexMap.Count > 0)
            {
                return true;
            }

            try
            {
                // Attempt to load the geographic images from the specified directory. If loading is successful, return true.
                LoadGeoImages(directoryName, geoImageIndexMap, geoImageList);
                return true;
            }
            catch (PlatformNotSupportedException)
            {
                // Return false to indicate that the catalog cannot be loaded.
                return false;
            }
        }

        /// <summary>
        /// Attempts to resolve an exact-case image index by key.
        /// </summary>
        /// <param name="geoImageIndexMap">The image name-to-index lookup map.</param>
        /// <param name="key">The image key to resolve.</param>
        /// <param name="index">When this method returns, contains the resolved index if found; otherwise -1.</param>
        /// <returns><see langword="true"/> if the key exists and maps to an integer index; otherwise, <see langword="false"/>.</returns>
        private static bool TryGetIndex(SortedList geoImageIndexMap, string key, out int index)
        {
            // Attempt to retrieve the index for the specified key from the geoImageIndexMap. If the key exists and maps to an integer index,
            // return true; otherwise, return false.
            if (geoImageIndexMap != null &&
                geoImageIndexMap.ContainsKey(key) &&
                geoImageIndexMap[key] is int resolvedIndex)
            {
                index = resolvedIndex;
                return true;
            }

            index = -1;
            return false;
        }

        /// <summary>
        /// Attempts to resolve an image index using case-insensitive key comparison.
        /// </summary>
        /// <param name="geoImageIndexMap">The image name-to-index lookup map.</param>
        /// <param name="key">The input key to match ignoring case.</param>
        /// <param name="index">When this method returns, contains the resolved index if found; otherwise -1.</param>
        /// <returns><see langword="true"/> if a case-insensitive match is found; otherwise, <see langword="false"/>.</returns>
        private static bool TryGetCaseInsensitiveIndex(SortedList geoImageIndexMap, string key, out int index)
        {
            // Iterate through the entries in the geoImageIndexMap and perform a case-insensitive comparison of the keys. If a match is found
            // and the value is an integer index, return true; otherwise, return false.
            if (geoImageIndexMap != null)
            {
                foreach (DictionaryEntry item in geoImageIndexMap)
                {
                    if (item.Key is string entryKey &&
                        string.Equals(entryKey, key, StringComparison.OrdinalIgnoreCase) &&
                        item.Value is int resolvedIndex)
                    {
                        index = resolvedIndex;
                        return true;
                    }
                }
            }

            index = -1;
            return false;
        }

        /// <summary>
        /// Attempts to resolve the canonical catalog key for an input key using case-insensitive matching.
        /// </summary>
        /// <param name="geoImageIndexMap">The image name-to-index lookup map.</param>
        /// <param name="key">The input key to canonicalize.</param>
        /// <param name="canonicalKey">When this method returns, contains the canonical key casing if found; otherwise an empty string.</param>
        /// <returns><see langword="true"/> if a case-insensitive key match is found; otherwise, <see langword="false"/>.</returns>
        private static bool TryGetCaseInsensitiveKey(SortedList geoImageIndexMap, string key, out string canonicalKey)
        {
            // Iterate through the entries in the geoImageIndexMap and perform a case-insensitive comparison of the keys. If a match is found,
            // return the canonical key casing; otherwise, return false.
            if (geoImageIndexMap != null)
            {
                foreach (DictionaryEntry item in geoImageIndexMap)
                {
                    if (item.Key is string entryKey &&
                        string.Equals(entryKey, key, StringComparison.OrdinalIgnoreCase))
                    {
                        canonicalKey = entryKey;
                        return true;
                    }
                }
            }

            canonicalKey = string.Empty;
            return false;
        }

        #endregion
    }
}
