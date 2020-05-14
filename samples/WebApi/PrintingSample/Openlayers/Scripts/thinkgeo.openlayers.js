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

// Add get root path method.
getRootPath = function () {
    var pathArray = location.pathname.split('/');
    var appPath = "/";
    for (var i = 1; i < pathArray.length - 1; i++) {
        appPath += pathArray[i] + "/";
    }

    return appPath === "/" ? "" : appPath;
};