// Extend leaflet by creating an imageButtons control
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

// Extend Leaflet by creating a dynamic layer which would refreshed dynamically by avoiding using the client cache.
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

// Extend the L.EditToolbar to make it capable of being disabled.
L.EditToolbar.prototype._checkDisabled = function () {
    var featureGroup = this.options.featureGroup,
        hasLayers = featureGroup.getLayers().length !== 0,
        button;

    if (this.options.remove) {
        button = this._modes[L.EditToolbar.Delete.TYPE].button;

        if (hasLayers) {
            L.DomUtil.removeClass(button, 'leaflet-disabled');
        } else {
            L.DomUtil.addClass(button, 'leaflet-disabled');
        }

        button.setAttribute(
            'title',
            hasLayers ?
            L.drawLocal.edit.toolbar.buttons.remove
            : L.drawLocal.edit.toolbar.buttons.removeDisabled
        );
    }
}