// Initialize default value for some parameters.
var selectedOverlay = "light";

// Create the map.
var map = L.map('map').setView([37.78353660152195, -122.44897842407228], 12);

// Add image buttons for layers and help.
L.imageButtons({
    imgs: [
        {
            src: 'Images/Layers.png',
            id: 'btnLayerOptions',
            title: 'Show layers',
            callback: function () {
                $('#leftPanel').animate({
                    'left': '0px'
                });
            }
        },
        {
            src: 'Images/info.png',
            id: 'btnInfo',
            title: 'Show help',
            callback: function () { window.open('https://github.com/ThinkGeo/ThinkGeoCloudVectorMapsSample-ForWebApi', '_blank'); }
        }
    ]
}).addTo(map);

// Add ThinkGeoCloudMaps  
var thinkGeoCloudMapsRasterLayer = L.tileLayer('https://{s}.thinkgeo.com/api/v1/maps/raster/aerial/x1/3857/512/{z}/{x}/{y}.jpeg', {
    subdomains: ['gisserver1', 'gisserver2', 'gisserver3', 'gisserver4', 'gisserver5', 'gisserver6'],
    layers: 'ThinkGeoCloudMaps',
    format: 'image/jpeg',
    styles: 'aerial',
    version: '1.1.1',
    tileSize: 512,
    zoomOffset: -1
});


var lightLayer = L.tileLayer(L.Util.getRootPath() + "/ThinkGeoCloudVectorMaps/tile/light/{z}/{x}/{y}", {
    format: 'image/png',
    tileSize: 512,
    zoomOffset: -1
});


var darkLayer = L.tileLayer(L.Util.getRootPath() + "/ThinkGeoCloudVectorMaps/tile/dark/{z}/{x}/{y}", {
    format: 'image/png',
    tileSize: 512,
    zoomOffset: -1
});


var transparentBackgroundLayer = L.tileLayer(L.Util.getRootPath() + "/ThinkGeoCloudVectorMaps/tile/transparentBackground/{z}/{x}/{y}", {
    format: 'image/png',
    tileSize: 512,
    zoomOffset: -1
});

var customLayer = L.tileLayer(L.Util.getRootPath() + "/ThinkGeoCloudVectorMaps/tile/custom/{z}/{x}/{y}", {
    format: 'image/png',
    tileSize: 512,
    zoomOffset: -1
});

var baseLayerGroup = L.featureGroup([lightLayer]).addTo(map);


// Refresh sample by selecting sample item.
$('#leftPanelOptions div').bind('click', function () {
    selectedOverlay = $(this).attr('id');
    RefreshSelectedOverlay(selectedOverlay);
});

// Show or hide left navigation panel.
$('html').click(function () {
    $('#leftPanel').animate({
        'left': -$('#leftPanel').width() + 'px'
    });
});

// Refresh current overlay by selection.
function RefreshSelectedOverlay(selectedOverlay) {
    baseLayerGroup.clearLayers();
    $('.selected').attr('class', 'unselected');
    $('#' + selectedOverlay).attr('class', 'selected');

    // Replace basemap.
    switch (selectedOverlay) {
        case "light":
            baseLayerGroup.addLayer(lightLayer);
            break;
        case "dark":
            baseLayerGroup.addLayer(darkLayer);
            break;
        case "hybrid":
            baseLayerGroup.addLayer(transparentBackgroundLayer);
            baseLayerGroup.addLayer(thinkGeoCloudMapsRasterLayer);
            break;
        case "custom":
            baseLayerGroup.addLayer(customLayer);
            break;
        default:
    }
    baseLayerGroup.bringToBack();
}