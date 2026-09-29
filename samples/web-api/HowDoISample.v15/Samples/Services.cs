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

            // A WMS and a WMTS drawn by the server: each tile the browser asks for is drawn from
            // what the service answers for that extent.
            catalog.Raster("wms", () =>
            {
                var wms = new WmsAsyncLayer(new Uri("https://ows.mundialis.de/osm/service")) { Crs = "EPSG:3857" };
                wms.ActiveLayerNames.Add("OSM-WMS");
                wms.ActiveStyleNames.Add("default");
                var overlay = new LayerOverlay();
                overlay.Layers.Add(wms);
                return overlay;
            });
            catalog.Raster("wmts", () =>
            {
                var wmts = new WmtsAsyncLayer(new Uri("https://wmts.geo.admin.ch/EPSG/3857/1.0.0/WMTSCapabilities.xml"))
                {
                    ActiveLayerName = "ch.swisstopo.pixelkarte-farbe-pk25.noscale",
                    ActiveStyleName = "ch.swisstopo.pixelkarte-farbe-pk25.noscale",
                    TileMatrixSetName = "3857_18",
                    OutputFormat = "image/jpeg",
                };
                var overlay = new LayerOverlay();
                overlay.Layers.Add(wmts);
                return overlay;
            });
        }

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
        }
    }
}
