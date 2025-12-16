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
        img.src = options.imgs[i].src;
        img.title = options.imgs[i].title;
        img.callback = options.imgs[i].callback;
        if (options.imgs[i].id) {
            img.id = options.imgs[i].id;
        }

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

// extend leaflet map 
app.getScale = function (zoom) {
    var pixelScales = [295829355.45, 147914677.73, 73957338.86, 36978669.43, 18489334.72, 9244667.36,
        4622333.68, 2311166.84, 1155583.42, 577791.71, 288895.85, 144447.93, 72223.96, 36111.98, 18055.99,
        9028.00, 4514.00, 2257.00, 1128.50, 564.25, 282.12, 141.06, 70.53];
    return pixelScales[zoom];
};

// add get root path method.
getRootPath = function () {
    var pathArray = location.pathname.split('/');
    var appPath = "/";
    for (var i = 1; i < pathArray.length - 1; i++) {
        appPath += pathArray[i] + "/";
    }

    return appPath === "/" ? "" : appPath;
};

guid = function () {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
    }

    return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
};