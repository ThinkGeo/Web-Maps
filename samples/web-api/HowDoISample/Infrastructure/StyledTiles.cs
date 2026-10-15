using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using ThinkGeo.Core;
using ThinkGeo.Core.Styling;

namespace ThinkGeo.UI.WebApi.HowDoI
{
    /// <summary>
    /// A style.json drawn on the server into raster tiles. The document is the layers a page
    /// holds - the same ones the browser draws over vector tiles - over the feature sources of a
    /// vector overlay, and CpuTileRenderer draws it, with the images the layers name. The other
    /// way a style is drawn on the server, onto any canvas, is <see cref="StyledLayer"/>.
    /// </summary>
    public sealed class StyledTiles : IDisposable
    {
        private readonly FeatureSourceVectorTileSource source;
        private readonly Lazy<Task<CpuTileRenderer>> renderer;

        public StyledTiles(string sourceId, IEnumerable<KeyValuePair<string, FeatureSource>> featureSources, string layers, IReadOnlyDictionary<string, GeoImage> images = null)
        {
            // The tiles cut from the sources are kept on disk under the source's name: a style
            // draws them many times over - every render reads a ring of neighbours for its labels -
            // and every style over the same sources reads the same tiles.
            source = new FeatureSourceVectorTileSource { SourceId = sourceId, VectorTileCache = new FileTileCache(Path.Combine(Path.GetTempPath(), "howdoi-tiles", sourceId)) };
            foreach (var pair in featureSources)
            {
                source.FeatureSources.Add(pair.Key, pair.Value);
            }
            // An overlay is see-through where it draws nothing; a style with no background of its
            // own is given a clear one, since the renderer would otherwise start from an opaque
            // surface.
            var style = new MapStyle(Document(sourceId, layers, null, true), source);
            // The icons a style names ride the style itself; the renderer draws tiles on the grid's
            // 512-pixel size.
            foreach (var pair in images ?? Enumerable.Empty<KeyValuePair<string, GeoImage>>())
            {
                style.Images.Add(pair.Key, pair.Value);
            }
            renderer = new Lazy<Task<CpuTileRenderer>>(() => CpuTileRenderer.CreateAsync(new IVectorTileSource[] { source }, style), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public async Task<byte[]> RenderTileAsync(int z, int x, int y, CancellationToken cancellation)
        {
            return await (await renderer.Value).RenderTileAsync(z, x, y, cancellation);
        }

        /// <summary>
        /// A style document of one vector source and the layers given, each layer told its source
        /// and given a name if it has none; with a clear background first when asked and the
        /// layers have none.
        /// </summary>
        public static string Document(string sourceId, string layers, string tiles, bool clearBackground = false)
        {
            var array = JsonNode.Parse(layers) as JsonArray ?? throw new ArgumentException("The layers must be a JSON array.");
            if (clearBackground && !array.Any(node => node?["type"]?.GetValue<string>() == "background"))
            {
                array.Insert(0, new JsonObject { ["id"] = sourceId + "-clear", ["type"] = "background", ["paint"] = new JsonObject { ["background-color"] = "rgba(0,0,0,0)" } });
            }
            var index = 0;
            foreach (var node in array)
            {
                var layer = node as JsonObject ?? throw new ArgumentException("Every layer must be an object.");
                layer["id"] ??= JsonValue.Create(sourceId + "-" + index);
                if (layer["type"]?.GetValue<string>() != "background")
                {
                    layer["source"] = JsonValue.Create(sourceId);
                }
                index++;
            }
            var vector = new JsonObject { ["type"] = "vector" };
            if (tiles != null)
            {
                vector["tiles"] = new JsonArray(JsonValue.Create(tiles));
            }
            return new JsonObject { ["version"] = 8, ["sources"] = new JsonObject { [sourceId] = vector }, ["layers"] = array }.ToJsonString();
        }

        /// <summary>A name for a style: its source, and a hash of its layers and the images they name.</summary>
        public static string IdOf(string sourceId, string layers, IEnumerable<string> imageNames)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(layers + "|" + string.Join(",", imageNames ?? Enumerable.Empty<string>())));
            return "style-" + sourceId + "-" + Convert.ToHexString(hash, 0, 6).ToLowerInvariant();
        }

        public void Dispose()
        {
            if (renderer.IsValueCreated && renderer.Value.IsCompletedSuccessfully)
            {
                renderer.Value.Result.Dispose();
            }
            source.Dispose();
        }
    }

    /// <summary>
    /// A classic Layer that draws a style.json over feature sources on whatever GeoCanvas it is
    /// drawn to. The features inside the canvas's extent are read from each source, keyed by the
    /// source-layer name the style knows them by, and MvtTilesAsyncLayer's painter draws them
    /// with the style through the classic styles: vector paths on a PdfGeoCanvas, pixels on a
    /// GeoImage, a map inside a printer layout.
    /// </summary>
    public sealed class StyledLayer : Layer
    {
        private readonly MvtTilesAsyncLayer painter;
        private readonly Dictionary<string, FeatureSource> sources;

        public StyledLayer(string layers, IEnumerable<KeyValuePair<string, FeatureSource>> featureSources)
        {
            sources = featureSources.ToDictionary(pair => pair.Key, pair => pair.Value);
            painter = new MvtTilesAsyncLayer(DocumentFile(layers));
        }

        public override bool HasBoundingBox => true;

        // The painter reads its style from a file: one per distinct document, in the temp folder.
        // The source's tile address is never asked for; the features come from the sources here.
        private static string DocumentFile(string layers)
        {
            var path = Path.Combine(Path.GetTempPath(), "howdoi-" + StyledTiles.IdOf("features", layers, null) + ".json");
            if (!File.Exists(path))
            {
                File.WriteAllText(path, StyledTiles.Document("features", layers, "http://localhost/none/{z}/{x}/{y}.mvt"));
            }
            return path;
        }

        protected override void OpenCore()
        {
            painter.OpenAsync().GetAwaiter().GetResult();
            foreach (var source in sources.Values)
            {
                if (!source.IsOpen) source.Open();
            }
        }

        protected override void CloseCore()
        {
            foreach (var source in sources.Values)
            {
                if (source.IsOpen) source.Close();
            }
            painter.CloseAsync().GetAwaiter().GetResult();
        }

        protected override RectangleShape GetBoundingBoxCore()
        {
            return MapUtil.GetBoundingBoxOfItems(sources.Values.Select(source => source.GetBoundingBox()));
        }

        protected override void DrawCore(GeoCanvas canvas, Collection<SimpleCandidate> labelsInAllLayers)
        {
            // The painter finds a layer's features under "source|source-layer".
            var features = new Dictionary<string, Collection<Feature>>();
            foreach (var pair in sources)
            {
                features["features|" + pair.Key] = pair.Value.GetFeaturesInsideBoundingBox(canvas.CurrentWorldExtent, ReturningColumnsType.AllColumns);
            }
            painter.Draw(canvas, features, labelsInAllLayers);
        }
    }
}
