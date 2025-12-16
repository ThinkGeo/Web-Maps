var div = L.DomUtil.create('div', 'adornments-pane');
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

AdornmentsOverlay = L.ImageOverlay.extend({
    initialize: function (opt_options) {
        var options = opt_options || {};
        img = L.DomUtil.create('img', 'adornments', div);
        adornmentType = 'ScaleBarAdornment';
        img.id = options.id;
        img.title = '';
    },
    addTo: function (map) {
        var element = map.getContainer();
        element.appendChild(div);
        map.on('zoomend', zoomend);
        map.on('resizeend', resize);
        map.on('moveend', moveend);
    },
    setUrl: function (url) {
        img.src = url;
    },
});

function zoomend() {
    img.src = 'Adornments/' + adornmentType + '/' + GetMapSize(map) + '/' + GetMapBounds(map) + '/';
};

function resize() {
    img.src = 'Adornments/' + adornmentType + '/' + GetMapSize(map) + '/' + GetMapBounds(map) + '/';
}

function moveend() {
    img.src = 'Adornments/' + adornmentType + '/' + GetMapSize(map) + '/' + GetMapBounds(map) + '/';
}

function GetMapBounds(map) {
    var bounds = map.getBounds();
    var soutWestPoint = L.CRS.EPSG3857.project(bounds._southWest);
    var northEastPoint = L.CRS.EPSG3857.project(bounds._northEast);
    return [soutWestPoint.x, soutWestPoint.y, northEastPoint.x, northEastPoint.y];
}

function GetMapSize(map) {
    var point = map.getSize();
    return [point.x, point.y]
}
