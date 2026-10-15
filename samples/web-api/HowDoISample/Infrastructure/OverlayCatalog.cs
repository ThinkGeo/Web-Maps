using System.Collections.Concurrent;
using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI
{
    /// <summary>
    /// The overlays the site serves, by the name their route goes by. Each is made the first time
    /// it is asked for and kept: a vector overlay keeps its feature sources open and its cutter
    /// warm across requests, which is what makes the tiles fast. A raster or adornment overlay
    /// draws through classic layers that are not safe to draw from two requests at once, so
    /// each has a lock the gallery's endpoints take while drawing. A styled overlay is a
    /// style.json the server draws into raster tiles over a vector overlay's sources.
    /// </summary>
    public sealed class OverlayCatalog
    {
        private readonly ConcurrentDictionary<string, Lazy<object>> overlays = new ConcurrentDictionary<string, Lazy<object>>(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, Func<VectorTileOverlay>> vectorFactories = new ConcurrentDictionary<string, Func<VectorTileOverlay>>(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, SemaphoreSlim> locks = new ConcurrentDictionary<string, SemaphoreSlim>(StringComparer.OrdinalIgnoreCase);

        public void Vector(string id, Func<VectorTileOverlay> make)
        {
            vectorFactories[id] = make;
            Add(id, () => make());
        }

        public void Raster(string id, Func<LayerOverlay> make) => Add(id, () => make());

        public void Adornment(string id, Func<AdornmentOverlay> make) => Add(id, () => make());

        /// <summary>A styled overlay is kept by name once made; the same name asked for again is the same tiles.</summary>
        public void Styled(string id, Func<StyledTiles> make) => overlays.TryAdd(id, new Lazy<object>(make, LazyThreadSafetyMode.ExecutionAndPublication));

        public VectorTileOverlay GetVector(string id) => Get(id) as VectorTileOverlay;

        public LayerOverlay GetRaster(string id) => Get(id) as LayerOverlay;

        public AdornmentOverlay GetAdornment(string id) => Get(id) as AdornmentOverlay;

        public StyledTiles GetStyled(string id) => Get(id) as StyledTiles;

        public bool HasVector(string id) => id != null && vectorFactories.ContainsKey(id);

        /// <summary>
        /// A vector overlay's feature sources made anew, never opened, for a renderer of its own:
        /// a file is not read by two renderers at once.
        /// </summary>
        public IEnumerable<KeyValuePair<string, FeatureSource>> FreshSources(string id)
        {
            var overlay = id != null && vectorFactories.TryGetValue(id, out var make) ? make() : null;
            var sources = (overlay?.TileSource as FeatureSourceVectorTileSource)?.FeatureSources;
            if (sources == null) yield break;
            var keys = sources.GetKeys();
            for (var i = 0; i < keys.Count; i++)
            {
                yield return new KeyValuePair<string, FeatureSource>(keys[i], sources[i]);
            }
        }

        /// <summary>The lock a classic overlay is drawn under.</summary>
        public SemaphoreSlim LockFor(string id) => locks.GetOrAdd(id, _ => new SemaphoreSlim(1, 1));

        private void Add(string id, Func<object> make)
        {
            overlays[id] = new Lazy<object>(make, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        private object Get(string id)
        {
            return id != null && overlays.TryGetValue(id, out var overlay) ? overlay.Value : null;
        }
    }
}
