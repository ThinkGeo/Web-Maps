// Extend leaflet map, get scale from current zoomlevel.  
L.Map.prototype.getScale = function () {
    var pixelScales = [295829355.45, 147914677.73, 73957338.86, 36978669.43, 18489334.72, 9244667.36, 4622333.68, 2311166.84,
        1155583.42, 577791.71, 288895.85, 144447.93, 72223.96, 36111.98, 18055.99, 9028.00, 4514.00, 2257.00, 1128.50, 564.25,
        282.12, 141.06, 70.53];
    return pixelScales[this.getZoom()];
};

// Extend leaflet map, create imageButtons control. 
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
            img.id = this.options.imgs[i].id;
            img.src = this.options.imgs[i].src;
            if (this.options.imgs[i].css) {
                img.className = this.options.imgs[i].css;
            }
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

// Add get root path method.
L.Util.getRootPath = function () {
    var pathArray = location.pathname.split('/');
    var appPath = "/";
    for (var i = 1; i < pathArray.length - 1; i++) {
        appPath += pathArray[i] + "/";
    }

    return appPath === "/" ? "" : appPath;
};

// Dynamic layer for loading server layer, which can avoid the browser cache and make the tiles get refreshed
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