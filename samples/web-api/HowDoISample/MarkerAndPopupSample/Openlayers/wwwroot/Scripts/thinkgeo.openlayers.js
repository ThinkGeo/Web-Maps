/**
 * Define a namespace for the application.
 */
window.app = {};
var app = window.app;

/**
 * @constructor
 * @extends {ol.control.Control}
 * @param {Object=} opt_options Control options.
 */
app.ImagesControl = function (opt_options) {
    var options = opt_options || {};

    var element = document.createElement('div');
    element.className = 'img-control ol-unselectable';

    for (var i = 0; i < options.imgs.length; i++) {
        var img = document.createElement('img');
        img.id = options.imgs[i].id;
        img.src = options.imgs[i].src;
        if (options.imgs[i].css) {
            img.className = options.imgs[i].css;
        }
        img.title = options.imgs[i].title;
        img.callback = options.imgs[i].callback;

        var handleCallback = function (e) {
            // prevent #rotate-north anchor from getting appended to the url
            e.preventDefault();
            if (this.callback) {
                this.callback(e);
            }
        };

        img.addEventListener('click', handleCallback, false);
        img.addEventListener('touchstart', handleCallback, false);

        element.appendChild(img);
    }

    ol.control.Control.call(this, {
        element: element,
        target: options.target
    });
};
ol.inherits(app.ImagesControl, ol.control.Control);

/**
 * @constructor
 * @extends {ol.interaction.Pointer}
 */
app.Drag = function () {

    ol.interaction.Pointer.call(this, {
        handleDownEvent: app.Drag.prototype.handleDownEvent,
        handleDragEvent: app.Drag.prototype.handleDragEvent,
        handleMoveEvent: app.Drag.prototype.handleMoveEvent,
        handleUpEvent: app.Drag.prototype.handleUpEvent
    });

    /**
     * @type {ol.Pixel}
     * @private
     */
    this.coordinate_ = null;

    /**
     * @type {string|undefined}
     * @private
     */
    this.cursor_ = 'pointer';

    /**
     * @type {ol.Feature}
     * @private
     */
    this.feature_ = null;

    /**
     * @type {string|undefined}
     * @private
     */
    this.previousCursor_ = undefined;

};
ol.inherits(app.Drag, ol.interaction.Pointer);

/**
 * @param {ol.MapBrowserEvent} evt Map browser event.
 * @return {boolean} `true` to start the drag sequence.
 */
app.Drag.prototype.handleDownEvent = function (evt) {
    var map = evt.map;
    var feature = map.forEachFeatureAtPixel(evt.pixel,
        function (feature) {
            return feature;
        });

    if (feature) {
        this.coordinate_ = evt.coordinate;
        this.feature_ = feature;
    }

    return !!feature;
};

/**
 * @param {ol.MapBrowserEvent} evt Map browser event.
 */
app.Drag.prototype.handleDragEvent = function (evt) {
    var deltaX = evt.coordinate[0] - this.coordinate_[0];
    var deltaY = evt.coordinate[1] - this.coordinate_[1];

    var geometry = /** @type {ol.geom.SimpleGeometry} */
        (this.feature_.getGeometry());
    geometry.translate(deltaX, deltaY);

    $('#popup').css('display', 'none');
    var coordinate = geometry.getCoordinates();
    popupOverlay.setPosition(coordinate);

    this.coordinate_[0] = evt.coordinate[0];
    this.coordinate_[1] = evt.coordinate[1];
};


/**
 * @param {ol.MapBrowserEvent} evt Event.
 */
app.Drag.prototype.handleMoveEvent = function (evt) {
    if (this.cursor_) {
        var map = evt.map;
        var feature = map.forEachFeatureAtPixel(evt.pixel,
            function (feature) {
                return feature;
            });
        var element = evt.map.getTargetElement();
        if (feature) {
            if (element.style.cursor != this.cursor_) {
                this.previousCursor_ = element.style.cursor;
                element.style.cursor = this.cursor_;
            }
        } else if (this.previousCursor_ !== undefined) {
            element.style.cursor = this.previousCursor_;
            this.previousCursor_ = undefined;
        }
    }
};
