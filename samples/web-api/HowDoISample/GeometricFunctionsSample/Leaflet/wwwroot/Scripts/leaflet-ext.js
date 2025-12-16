L.Control.MsControl = L.Control.extend({
    options: {
        position: 'topleft',
        imgs: []
    },

    onAdd: function () {
        var container = L.DomUtil.create('div', 'leaflet-bar leaflet-control');
        for (var i = 0; i < this.options.imgs.length; i++) {
            this.link = L.DomUtil.create('a', 'leaflet-bar-part', container);
            var img = L.DomUtil.create('img', '', this.link);
            img.src = this.options.imgs[i].img;
            if (this.options.imgs[i].id) {
                img.id = this.options.imgs[i].id;
            }
            this.options.imgs[i].target = img;
            L.DomEvent.on(this.link, 'click', function (e) {
                L.DomEvent.stopPropagation(e);
                L.DomEvent.preventDefault(e);
                this.func(e);
            }, this.options.imgs[i]);
            this.link.title = this.options.imgs[i].title;
        }

        return container;
    },

    intendedFunction: function () { alert('no function selected'); },

    _click: function (e) {
        this.intendedFunction();
    },
});

L.msControl = function (options) {
    return new L.Control.MsControl(options);
};

//// override this method to remove the attribution info from url.
//L.TileLayer.WMS.prototype.getTileUrl = function (coords) {
//    var tileBounds = this._tileCoordsToBounds(coords),
//        nw = this._crs.project(tileBounds.getNorthWest()),
//        se = this._crs.project(tileBounds.getSouthEast()),

//        bbox = (this._wmsVersion >= 1.3 && this._crs === L.CRS.EPSG4326 ?
//            [se.y, nw.x, nw.y, se.x] :
//            [nw.x, se.y, se.x, nw.y]).join(','),

//        url = L.TileLayer.prototype.getTileUrl.call(this, coords);
//    return url + this._getParamString(this.wmsParams, url, true) + '&BBOX=' + bbox;
//}

L.TileLayer.WMS.prototype._getParamString = function (obj, existingUrl, uppercase) {
    var params = [];
    for (var i in obj) {
        if (i != 'attribution')
            params.push(encodeURIComponent(uppercase ? i.toUpperCase() : i) + '=' + encodeURIComponent(obj[i]));
    }
    return ((!existingUrl || existingUrl.indexOf('?') === -1) ? '?' : '&') + params.join('&');
}


// add get root path method.
L.Util.getRootPath = function () {
    var pathArray = location.pathname.split('/');
    var appPath = "/";
    for (var i = 1; i < pathArray.length - 1; i++) {
        appPath += pathArray[i] + "/";
    }

    return appPath === "/" ? "" : appPath;
};

// dynamic layer for loading server layer, which can be redraw dynamically
L.DynamicLayer = L.TileLayer.extend({
    getTileUrl: function (coords) {
        var tileUrl = L.Util.template(this._url, L.extend({
            r: this.options.detectRetina && L.Browser.retina && this.options.maxZoom > 0 ? '@2x' : '',
            s: this._getSubdomain(coords),
            x: coords.x,
            y: this.options.tms ? this._tileNumBounds.max.y - coords.y : coords.y,
            z: this._getZoomForUrl()
        }, this.options));
        tileUrl += '?t=' + new Date().getTime();

        return tileUrl;
    }
});

L.dynamicLayer = function (options) {
    return new L.DynamicLayer(options);
};