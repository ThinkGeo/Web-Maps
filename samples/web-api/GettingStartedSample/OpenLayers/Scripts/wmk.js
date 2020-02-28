L.WorldMapKitLayer = L.TileLayer.WMS.extend({
    initialize: function (layer) {
        var defaultOptions = {
            subdomains: ['worldmapkit1', 'worldmapkit2', 'worldmapkit3', 'worldmapkit4', 'worldmapkit5', 'worldmapkit6'],
            layers: 'OSMWorldMapKitLayer',
            format: 'image/png',
            styles: 'WorldMapKitDefaultStyle',
            version: '1.1.1',
            attribution: '<a href="http://thinkgeo.com/map-suite-developer-gis/world-map-kit-sdk/">ThinkGeo World Map Kit</a> | &copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors '
        };

        defaultOptions.layers = layer || "OSMWorldMapKitLayer";
        L.TileLayer.WMS.prototype.initialize.apply(this, ['http://{s}.thinkgeo.com/CachedWMSServer/WmsServer.axd', defaultOptions]);
    },

    // the layer has the value: 
    // OSMWorldMapKitLayer <L.CRS.EPSG3857,L.CRS.EPSG3857>default
    // WorldMapKitLayer <L.CRS.EPSG3857,L.CRS.EPSG3857>
    setLayer: function (layer) {
        switch (layer) {
            case 'WorldMapKitLayer':
                this.wmsParams.layers = "WorldMapKitLayer";
                break;
            case 'OSMWorldMapKitLayer':
            default:
                this.wmsParams.layers = 'OSMWorldMapKitLayer';
                break;
        }
        this.redraw();
    },

    // override this method as the attribution doesn't support html tag like <a>.
    getTileUrl: function (coords) {
        var tileBounds = this._tileCoordsToBounds(coords),
            nw = this._crs.project(tileBounds.getNorthWest()),
            se = this._crs.project(tileBounds.getSouthEast()),

            bbox = (this._wmsVersion >= 1.3 && this._crs === L.CRS.EPSG4326 ?
                [se.y, nw.x, nw.y, se.x] :
                [nw.x, se.y, se.x, nw.y]).join(','),

            url = L.TileLayer.prototype.getTileUrl.call(this, coords);
        if (this._crs === L.CRS.EPSG3857) { // this is a workaround as our "WorldMapKitLayer" only recognizes EPSG:900913 not EPSG:3857 in server side.
            this.wmsParams.srs = "EPSG:900913";
        }
        return url + this._getParamString(this.wmsParams, url, true) + '&BBOX=' + bbox;
    },

    _getParamString: function (obj, existingUrl, uppercase) {
        var params = [];
        for (var i in obj) {
            if (i != 'attribution')
                params.push(encodeURIComponent(uppercase ? i.toUpperCase() : i) + '=' + encodeURIComponent(obj[i]));
        }
        return ((!existingUrl || existingUrl.indexOf('?') === -1) ? '?' : '&') + params.join('&');
    }
});

L.worldMapKitLayer = function (layer) {
    return new L.WorldMapKitLayer(layer);
};