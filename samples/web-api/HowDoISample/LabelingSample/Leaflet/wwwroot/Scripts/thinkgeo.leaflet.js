// Extend leaflet by creating an imageButtons control. 
L.Control.ImageButtons = L.Control.extend({
    options: {
        position: 'topleft',
        imgs: []
    },

    onAdd: function () {
        var container = L.DomUtil.create('div', 'leaflet-bar leaflet-control');
        if (this.options.id) {
            container.id = this.options.id;
        }

        for (var i = 0; i < this.options.imgs.length; i++) {
            this.link = L.DomUtil.create('a', 'leaflet-bar-part', container);
            var img = L.DomUtil.create('img', '', this.link);
            img.src = this.options.imgs[i].src;
            this.options.imgs[i].target = img;

            var handleCallback = function (e) {
                L.DomEvent.stopPropagation(e);
                L.DomEvent.preventDefault(e);
                this.callback(e);
            };

            L.DomEvent.on(this.link, 'click', handleCallback, this.options.imgs[i]);
            L.DomEvent.on(this.link, 'touchstart', handleCallback, this.options.imgs[i]);
            this.link.title = this.options.imgs[i].title;
        }

        return container;
    },

    intendedFunction: function () {
        alert('no function selected');
    },

    _click: function (e) {
        this.intendedFunction();
    }
});

L.imageButtons = function (options) {
    return new L.Control.ImageButtons(options);
};

// Add the utility methods to L.Util
L.Util.getRootPath = function () {
    var pathArray = location.pathname.split('/');
    var appPath = "/";
    for (var i = 1; i < pathArray.length - 1; i++) {
        appPath += pathArray[i] + "/";
    }

    return appPath === "/" ? "" : appPath;
};

// Create DynamicLayer, which loads server layer dynamically by avoiding using the browser client cache.
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