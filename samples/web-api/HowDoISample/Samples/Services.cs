using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI.Samples
{
    /// <summary>
    /// Maps from a Service: feature services the server reads as each tile needs them - a WFS,
    /// an OGC API, NOAA's feeds - and OGC image services the server draws tile by tile. The
    /// services are never called from the browser.
    /// </summary>
    public class Services : ISampleGroup
    {
        private const string WmsService = "https://ows.mundialis.de/osm/service";
        private const string WmtsService = "https://wmts.geo.admin.ch/EPSG/3857/1.0.0/WMTSCapabilities.xml";

        // A few of what each service advertises; the capability routes show the whole list.
        private static readonly (string Key, string Name)[] WmsLayers =
        {
            ("osm", "OSM-WMS"),
            ("topo", "TOPO-WMS"),
            ("relief", "SRTM30-Colored-Hillshade"),
            ("overlay", "OSM-Overlay-WMS"),
        };
        private static readonly (string Key, string Name, string MatrixSet, string Format)[] WmtsLayers =
        {
            ("pk25", "ch.swisstopo.pixelkarte-farbe-pk25.noscale", "3857_18", "image/jpeg"),
            ("colour", "ch.swisstopo.pixelkarte-farbe", "3857_19", "image/jpeg"),
            ("grey", "ch.swisstopo.pixelkarte-grau", "3857_19", "image/jpeg"),
            ("aerial", "ch.swisstopo.swissimage", "3857_20", "image/jpeg"),
        };

        private static WmsAsyncLayer Wms() => new WmsAsyncLayer(new Uri(WmsService)) { Crs = "EPSG:3857" };

        public void Register(OverlayCatalog catalog)
        {
            // An asynchronous source is added to the overlay's collection like any other; the
            // collection wraps it for the cutter. Cadastral parcels of the Netherlands from
            // PDOK's WFS 2.0 service.
            catalog.Vector("parcels", () =>
            {
                var overlay = new VectorTileOverlay();
                overlay.FeatureSources.Add("parcels", new WfsV2AsyncFeatureSource("https://service.pdok.nl/kadaster/kadastralekaart/wfs/v5_0", "kadastralekaart:Perceel")
                {
                    TimeoutInSeconds = 120,
                    Crs = "urn:ogc:def:crs:EPSG::3857",
                });
                return overlay;
            });

            // Named places of Spain from the IGN's OGC API - Features service, read in latitude
            // and longitude and projected to the map before the tiles are cut.
            catalog.Vector("places", () =>
            {
                var overlay = new VectorTileOverlay();
                overlay.FeatureSources.Add("places", new OgcApiAsyncFeatureSource("https://api-features.ign.es", "namedplace")
                {
                    ProjectionConverter = new ProjectionConverter(4326, 3857),
                });
                return overlay;
            });

            // NOAA's weather stations with their current readings, and the warnings active right
            // now: two live feeds read by their own feature sources.
            catalog.Vector("stations", () =>
            {
                var overlay = new VectorTileOverlay();
                overlay.FeatureSources.Add("stations", new NoaaWeatherStationFeatureSource { ProjectionConverter = new ProjectionConverter(4326, 3857) });
                return overlay;
            });
            catalog.Vector("warnings", () =>
            {
                var overlay = new VectorTileOverlay();
                overlay.FeatureSources.Add("warnings", new NoaaWeatherWarningsFeatureSource { ProjectionConverter = new ProjectionConverter(4326, 3857) });
                return overlay;
            });

            // A WMS renders on demand: the server asks for the exact extent of every tile it is
            // cutting, so any of the service's named layers can be drawn at any scale.
            foreach (var pair in WmsLayers)
            {
                var layerName = pair.Name;
                catalog.Raster("wms-" + pair.Key, () =>
                {
                    var wms = Wms();
                    wms.ActiveLayerNames.Add(layerName);
                    wms.ActiveStyleNames.Add("default");
                    var overlay = new LayerOverlay();
                    overlay.Layers.Add(wms);
                    return overlay;
                });
            }

            // A WMTS serves tiles already cut, on a matrix set it names. Every layer is tied to
            // one of them, and the server has to ask on that layer's own matrix and format.
            foreach (var pair in WmtsLayers)
            {
                var layer = pair;
                catalog.Raster("wmts-" + pair.Key, () =>
                {
                    var wmts = new WmtsAsyncLayer(new Uri(WmtsService))
                    {
                        ActiveLayerName = layer.Name,
                        ActiveStyleName = layer.Name,
                        TileMatrixSetName = layer.MatrixSet,
                        OutputFormat = layer.Format,
                    };
                    var overlay = new LayerOverlay();
                    overlay.Layers.Add(wmts);
                    return overlay;
                });
            }
        }

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            // What the WMS says about itself. There is no WMS client in MapLibre, so the layer
            // names, styles, formats and projections below are ones only the server ever sees,
            // read from the service's GetCapabilities document.
            app.MapGet("/samples/services/wms-capabilities", async () =>
            {
                var wms = Wms();
                await wms.OpenAsync();
                var answer = new
                {
                    service = WmsService,
                    version = wms.GetServiceVersion(),
                    layers = wms.GetServerLayers().Where(layer => !string.IsNullOrEmpty(layer.Name))
                        .Select(layer => new { name = layer.Name, title = layer.Title }).ToArray(),
                    formats = wms.GetServerOutputFormats(),
                    projections = wms.GetServerCrsCollection().Count,
                    featureInfo = wms.GetServerFeatureInfoFormats(),
                    offered = WmsLayers.Select(pair => new { key = pair.Key, name = pair.Name }),
                };
                await wms.CloseAsync();
                return Results.Json(answer);
            });

            // The same question of the WMTS, whose answer is mostly about matrices: every layer
            // names the one it was cut on, and the server must ask on that one.
            app.MapGet("/samples/services/wmts-capabilities", async () =>
            {
                var wmts = new WmtsAsyncLayer(new Uri(WmtsService));
                await wmts.OpenAsync();
                var answer = new
                {
                    service = WmtsService,
                    layers = wmts.GetServerLayerNames().Count,
                    matrixSets = wmts.GetTileMatrixSets().Select(pair => new { name = pair.Key, levels = pair.Value.TileMatrices.Count }).OrderBy(set => set.name).ToArray(),
                    offered = WmtsLayers.Select(pair => new { key = pair.Key, name = pair.Name, matrixSet = pair.MatrixSet, format = pair.Format }),
                };
                await wmts.CloseAsync();
                return Results.Json(answer);
            });
        }
    }
}
