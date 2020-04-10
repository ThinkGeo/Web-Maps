/**
 * Define a namespace for customizing the application.
 */
window.app = {};
var app = window.app;

/**
 * Create the customized toolbar inherited from OL3 control
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
        img.context = this;

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

// Extend OpenLayers map to calculate the scale 
app.getScale = function (zoom) {
    var pixelScales = [295829355.45, 147914677.73, 73957338.86, 36978669.43, 18489334.72, 9244667.36, 4622333.68,
        2311166.84, 1155583.42, 577791.71, 288895.85, 144447.93, 72223.96, 36111.98, 18055.99, 9028.00, 4514.00,
        2257.00, 1128.50, 564.25, 282.12, 141.06, 70.53];
    return pixelScales[zoom];
};

/**
 * create a customized context-menu
 * @constructor
 * @extends {ol.control.Control}
 * @param {Object=} opt_options Control options.
 */
var contentMenu;
app.ContentMenu = function (opt_options) {
    var options = opt_options || {};
    var element = document.createElement('div');
    element.className = 'contentMenu';
    element.style.position = 'absolute';
    element.style.width = '100px';
    element.style.zIndex = 100000;
    element.style.display = 'none';
    element.oncontextmenu = function () { return false; };
    element.style.padding = '1px';
    element.style.backgroundColor = 'white';
    element.style.border = '#cccccc 1px solid';
    element.style.borderRight = '#333333 1px solid';
    element.style.borderBottom = '#333333 1px solid';
    element.style.lineHeight = '18px';
    contentMenu = element;
    createItems(element, options.menuItems);
    ol.control.Control.call(this, {
        element: element,
        target: options.target
    });
};
ol.inherits(app.ContentMenu, ol.control.Control);