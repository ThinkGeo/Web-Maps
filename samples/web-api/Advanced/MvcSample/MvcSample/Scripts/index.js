$(document).ready(function () {
    var thinkgeoApiKey = 'PIbGd76RyHKod99KptWTeb-Jg9JUPEPUBFD3SZJYLDE~';

    function init() {

        // Create the map.
        var map = new ol.Map({
            target: 'map',
            renderer: 'webgl',
            loadTilesWhileAnimating: true,
            loadTilesWhileInteracting: true,
            controls: ol.control.defaults({ attribution: false }).extend(
                [new ol.control.Attribution({
                    collapsible: false
                })]),
            view: new ol.View({
                center: ol.proj.transform([-95.81131, 40.14711], 'EPSG:4326', 'EPSG:3857'),
                zoom: 5
            })
        });

        // Add ThinkgeoVectorTileLayer as the map's background layer.
        map.addLayer(new ol.mapsuite.VectorTileLayer('https://cdn.thinkgeo.com/worldstreets-styles/3.0.0/light.json', {
            apiKey: thinkgeoApiKey,
            layerName: 'light'
        }));

        // Add dynamic tile layer to the map, it can be refreshed dynamically
        var xyzSource = new ol.source.XYZ({
            url: getRootPath() + '/Home/GetShapeFileTile/{z}/{x}/{y}',
            maxZoom: 19
        });
        xyzSource.tileLoadFunction = function (imageTile, src) {
            imageTile.getImage().src = src + '?t=' + new Date().getTime();
        };
        map.addLayer(new ol.layer.Tile({
            source: xyzSource
        }));
    };

    // Get root path method.
    function getRootPath() {
        var pathArray = location.pathname.split('/');
        var appPath = "/";
        for (var i = 1; i < pathArray.length - 1; i++) {
            appPath += pathArray[i] + "/";
        }

        return appPath === "/" ? "" : appPath;
    };

    init();
});