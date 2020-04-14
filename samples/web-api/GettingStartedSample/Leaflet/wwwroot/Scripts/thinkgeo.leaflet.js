// Extend leaflet map, get scale from current zoomlevel.  
L.Map.prototype.getScale = function () {
    var pixelScales = [295829355.45, 147914677.73, 73957338.86, 36978669.43, 18489334.72, 9244667.36, 4622333.68, 2311166.84,
        1155583.42, 577791.71, 288895.85, 144447.93, 72223.96, 36111.98, 18055.99, 9028.00, 4514.00, 2257.00, 1128.50, 564.25,
        282.12, 141.06, 70.53];
    return pixelScales[this.getZoom()];
};

// Avoid raisng single-click event when double clicking on the map.
L.Map.prototype.preclick;
L.Map.prototype._handleMouseEvent = function (e) {
    if (!this._loaded) { return; }

    if (e.type == 'click') {
        if (this.preclick) {
            clearTimeout(this.preclick);
            this.preclick = 0;
            return;
        }

        this.preclick = setTimeout(function (map) {
            map._fireMouseEvent(map, e, e.type);

            clearTimeout(map.preclick);
            map.preclick = 0;
        }, 500, this); // If the interval between 2 clicks is below 500 ms, we treat it as 1 double click instead of 2 single clicks. 
    } else {
        this._fireMouseEvent(this, e, e.type === 'mouseenter' ? 'mouseover' : e.type === 'mouseleave' ? 'mouseout' : e.type);
    }
};

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
            L.DomEvent.on(this.link, 'click', function (e) {
                L.DomEvent.stopPropagation(e);
                L.DomEvent.preventDefault(e);
                this.callback(e);
            }, this.options.imgs[i]);
            this.link.title = this.options.imgs[i].title;
        }

        return container;
    },

    intendedFunction: function () {
        alert('no function applied.');
    },

    _click: function (e) {
        this.intendedFunction();
    },
});

L.imageButtons = function (options) {
    return new L.Control.ImageButtons(options);
};