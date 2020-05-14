// Initialize default value for some parameters.
var selectedOverlay = "light";

// Create ThinkGeoCloudMap Raster layer.
var thinkGeoCloudMapsRasterLayer = new ol.layer.Tile({
    source: new ol.source.XYZ({
        url: 'https://gisserver{1-6}.thinkgeo.com/api/v1/maps/raster/aerial/x1/3857/512/{z}/{x}/{y}.jpeg',
        maxZoom: 19,
        tileSize: 512,
        params:
        {
            'LAYERS': 'ThinkGeoCloudMaps',
            'VERSION': '10.4.0',
            'STYLE': 'aerial'
        }
    }),
    visible: false
});

var lightLayer = new ol.layer.Tile({
    source: new ol.source.XYZ({
        url: getRootPath() + "/ThinkGeoCloudVectorMaps/tile/light/{z}/{x}/{y}",
        maxZoom: 19,
        tileSize: 512
    })
});

var darkLayer = new ol.layer.Tile({
    source: new ol.source.XYZ({
        url: getRootPath() + "/ThinkGeoCloudVectorMaps/tile/dark/{z}/{x}/{y}",
        maxZoom: 19,
        tileSize: 512
    }),
    visible: false
});

var transparentBackgroundLayer = new ol.layer.Tile({
    source: new ol.source.XYZ({
        url: getRootPath() + "/ThinkGeoCloudVectorMaps/tile/transparentBackground/{z}/{x}/{y}",
        maxZoom: 19,
        tileSize: 512
    }),
    visible: false
});

var customLayer = new ol.layer.Tile({
    source: new ol.source.XYZ({
        url: getRootPath() + "/ThinkGeoCloudVectorMaps/tile/custom/{z}/{x}/{y}",
        maxZoom: 19,
        tileSize: 512
    }),
    visible: false
});

// Create Openlayers layer group. 
var layers = [lightLayer, darkLayer, thinkGeoCloudMapsRasterLayer, transparentBackgroundLayer, customLayer];
var baseLayerGroup = new ol.layer.Group({
    layers: layers
});

// Create the map.
var map = new ol.Map({
    layers: [baseLayerGroup],
    target: 'map',
    controls: ol.control.defaults({ attribution: false, rotate: false }).extend(
        [new ol.control.Attribution({
            collapsible: false
        })]),
    view: new ol.View({
        center: ol.proj.transform([-122.44897842407228, 37.78353660152195], 'EPSG:4326', 'EPSG:3857'),
        zoom: 12
    })
});

// Add image buttons for layers and help.
var imgControls = new app.ImagesControl({
    imgs: [
        {
            id: 'btnLayerOptions',
            src: 'Images/layers.png',
            title: 'Show layers',
            callback: function () {
                $('#leftPanel').animate({
                    'left': '0px'
                });
            }
        },
        {
            id: 'btnInfo',
            src: 'Images/info.png',
            title: 'Show help',
            callback: function () { window.open('https://github.com/ThinkGeo/ThinkGeoCloudVectorMapsSample-ForWebApi', '_blank'); }
        }
    ]
});
map.addControl(imgControls);

// Refresh sample by selecting sample item.
$('#leftPanelOptions div').bind('click', function () {
    selectedOverlay = $(this).attr('id');
    baseLayerGroup.getLayers().forEach(function (sublayer) {
        sublayer.setVisible(false);
    });
    $('.selected').attr('class', 'unselected');
    $('#' + selectedOverlay).attr('class', 'selected');
    // Replace basemap.
    switch (selectedOverlay) {
        case "light":
            lightLayer.setVisible(true);
            break;
        case "dark":
            darkLayer.setVisible(true);
            break;
        case "hybrid":
            thinkGeoCloudMapsRasterLayer.setVisible(true);
            transparentBackgroundLayer.setVisible(true);
            break;
        case "custom":
            customLayer.setVisible(true);
            break;
        default:
    }
});

// Shows or hides left navigation panel.
$('html').click(function () {
    $('#leftPanel').animate({
        'left': -$('#leftPanel').width() + 'px'
    });
});