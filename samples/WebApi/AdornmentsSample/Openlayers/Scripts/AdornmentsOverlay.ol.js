var div = document.createElement('div');
var img;
var adornmentType;
AdornmensType = {
    'LogoAdornment': 1,
    'ScaleBarAdornment': 2,
    'ScaleLineAdornment': 3,
    'ScaleTextAdornment': 4,
    'GraticuleAdornment': 5,
    'LegendAdornment': 6,
    'MagneticDeclinationAdornment': 7,
};

AdornmentsOverlay = function (opt_options) {
    var options = opt_options || {};
    adornmentType = options.adornmentType;
    img = document.createElement('img');
    img.id = options.id;
    img.title = '';
    img.setAttribute('class', 'adornments');
    div.setAttribute('class', 'adornments-pane');
    div.appendChild(img);
}

ol.inherits(AdornmentsOverlay, ol.Overlay);

addTo = function (map) {
    var element = document.getElementsByClassName('ol-overlaycontainer');
    element[0].appendChild(div);
    map.on('resizeend', resize);
    map.on('moveend', moveend);
    map.on('zoomend', zoomend);
};

zoomend = function () {
    var size = map.getSize();
    var extent = map.getView().calculateExtent(size);
    img.src = 'Adornments/' + adornmentType + '/' + size + '/' + extent + '/';
};

moveend = function () {
    var size = map.getSize();
    var extent = map.getView().calculateExtent(size);
    img.src = 'Adornments/' + adornmentType + '/' + size + '/' + extent + '/';
}

resize = function () {
    var size = map.getSize();
    var extent = map.getView().calculateExtent(size);
    img.src = 'Adornments/' + adornmentType + '/' + size + '/' + extent + '/';
};

setUrl = function (url) {
    img.src = url;
}











