// Initialize default value for some parameters.
var bingMapsKey = null;
var selectedOverlay = "thinkgeoCloudMap";

// Create ThinkGeoCloudMap layer.
var thinkGeoCloudMapsLayer = new ol.layer.Tile({
    source: new ol.source.XYZ({
        url: 'https://cloud{1-6}.thinkgeo.com/api/v1/maps/raster/light/x1/3857/512/{z}/{x}/{y}.png?apikey=PIbGd76RyHKod99KptWTeb-Jg9JUPEPUBFD3SZJYLDE~',
        maxZoom: 19,
        tileSize: 512,
        params:
        {
            'LAYERS': 'ThinkGeoCloudMaps',
            'VERSION': '10.4.0',
            'STYLE': 'Light'
        },

        // --------------------------------------------------------------------------------------
        // Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
        // an API Key. The following function is just for reminding you to input the key. 
        // Feel free to remove this function after the key was input. 
        // --------------------------------------------------------------------------------------
        tileLoadFunction: function (imageTile, src) {
            fetch(src).then((response) => {
                if (response.status === 401) {
                    var canvas = document.createElement("canvas");
                    canvas.width = 512;
                    canvas.height = 512;
                    var context = canvas.getContext("2d");
                    context.font = "14px Arial";
                    context.strokeText("Backgrounds for this sample are", 256, 100);
                    context.strokeText("powered by ThinkGeo Cloud Maps and", 256, 120);
                    context.strokeText("require an API Key. This was sent", 256, 140);
                    context.strokeText("to you via email when you signed up", 256, 160);
                    context.strokeText("with ThinkGeo, or you can register", 256, 180);
                    context.strokeText("now at https://cloud.thinkgeo.com", 256, 200);
                    var url = canvas.toDataURL("image/png", 1);
                    imageTile.getImage().src = url;
                }
                else {
                    response.blob().then((blob) => {
                        if (blob) {
                            imageTile.getImage().src = URL.createObjectURL(blob);
                        }
                        else {
                            imageTile.getImage().src = "";
                        }
                    });
                }
            });
        }
    })
});

// Create bing map layer without source,need to create bing map with bing maps key.
var bingMapLayer = new ol.layer.Tile({});

// Create open street map layer.
var openStreetMapLayer = new ol.layer.Tile({
    source: new ol.source.OSM({
        attributions: [
            new ol.Attribution({
                html: 'All maps &copy; ' +
                    '<a href="http://www.openstreetmap.org/">OpenStreetMap</a>'
            }),
            ol.source.OSM.ATTRIBUTION
        ]
    }),
    visible: false
});

// Create google map layer.
var googleMapLayer = new ol.layer.Tile({
    source: new ol.source.XYZ(({
        urls: ['http://mt0.google.com/vt/lyrs=m&x={x}&y={y}&z={z}',
            'http://mt1.google.com/vt/lyrs=m&x={x}&y={y}&z={z}',
            'http://mt2.google.com/vt/lyrs=m&x={x}&y={y}&z={z}',
            'http://mt3.google.com/vt/lyrs=m&x={x}&y={y}&z={z}',]
    })),
    visible: false
});

// Create Openlayers layer group. 
var layers = [thinkGeoCloudMapsLayer, bingMapLayer, openStreetMapLayer, googleMapLayer];
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
        center: ol.proj.transform([-96.8277325, 33.150205], 'EPSG:4326', 'EPSG:3857'),
        zoom: 13
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
            callback: function () { window.open('http://wiki.thinkgeo.com/wiki/map_suite_overlays', '_blank'); }
        }
    ]
});
map.addControl(imgControls);

// Add dynamic tile layer to the map, it can be refreshed dynamically
var xyzSource = new ol.source.XYZ({
    url: getRootPath() + 'Overlays/LoadCustomOverlay/{z}/{x}/{y}',
    maxZoom: 19
});
xyzSource.tileLoadFunction = function (imageTile, src) {
    imageTile.getImage().src = src + '?t=' + new Date().getTime();;
};
var shapLayer = new ol.layer.Tile({
    source: xyzSource
});
map.addLayer(shapLayer);

//Shows or hides dynamic customer overlay.
$("#CustomOverlay").click(function () {
    if ($("#CustomOverlay").is(':checked')) {
        shapLayer.setVisible(true);
    } else {
        shapLayer.setVisible(false);
    }
});

// Hook up click event on "OK" button in edit dialog.
$('#btnSave').click(function () {
    bingMapsKey = document.getElementById("bingMapsKey").value;
    var paras = {
        keyStr: bingMapsKey
    }
    // Validate Bing Maps key. If successful, replace the basemap to Bing Maps.
    $.post(getRootPath() + '/Overlays/ValidateBingMapsKey/', { '': JSON.stringify(paras) }, function (data) {
        if (data) {
            hideDlg();
            map.setView(new ol.View({
                center: ol.proj.transform([-96.8277325, 33.150205], 'EPSG:4326', 'EPSG:3857'),
                zoom: 13
            }));
            baseLayerGroup.getLayers().forEach(function (sublayer) {
                sublayer.setVisible(false);
            });

            // Create BingMap layer.
            bingMapLayer = new ol.layer.Tile({
                source: new ol.source.BingMaps({
                    key: bingMapsKey,
                    imagerySet: 'aerial'
                })
            });
            baseLayerGroup.getLayers().setAt(1, bingMapLayer);
            $('.selected').attr('class', 'unselected');
            $('#' + selectedOverlay).attr('class', 'selected');
        }
        else {
            alert('The remote server returned an error:(401) Unauthorized.');
        }
    });
});

// Cancel select bing map.
$('#btnCancel').click(function () {
    hideDlg();
});

// Shows edit dialog in model mode.
function showDlg() {
    $('#bgMask').show();
    $('#editPanel').slideToggle("fast");
}

// Hides edit dialog.
function hideDlg() {
    $('#bgMask').hide();
    $('#editPanel').slideToggle("fast");
}

// Shows or hides overlay options.
$("#overlayExpandOrCollapseButton").click(function () {
    if ($(this).attr("class") == "expand") {
        $(this).attr("class", "collapse");
        $("#overlaysList").hide();
    } else {
        $(this).attr("class", "expand");
        $("#overlaysList").show();
    }
});

// Refresh sample by selecting sample item.
$('#leftPanelOptions div').bind('click', function () {
    selectedOverlay = $(this).attr('id');

    if (selectedOverlay == "bingMap" && bingMapsKey == null) {
        if (bingMapsKey == null) {
            showDlg();
        }
    } else {
        map.setView(new ol.View({
            center: ol.proj.transform([-96.8277325, 33.150205], 'EPSG:4326', 'EPSG:3857'),
            zoom: 13
        }));
        baseLayerGroup.getLayers().forEach(function (sublayer) {
            sublayer.setVisible(false);
        });
        $('.selected').attr('class', 'unselected');
        $('#' + selectedOverlay).attr('class', 'selected');
        // Replace basemap.
        switch (selectedOverlay) {
            case "bingMap":
                bingMapLayer.setVisible(true);
                break;
            case "thinkgeoCloudMap":
                thinkGeoCloudMapsLayer.setVisible(true);
                break;
            case "openStreetMap":
                openStreetMapLayer.setVisible(true);
                break;
            case "googleMap":
                googleMapLayer.setVisible(true);
                break;
            default:
        }
    }
});

// Shows or hides left navigation panel.
$('html').click(function () {
    $('#leftPanel').animate({
        'left': -$('#leftPanel').width() + 'px'
    });
});