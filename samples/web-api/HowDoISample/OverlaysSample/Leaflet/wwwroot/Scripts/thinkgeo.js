// Initialize default value for some parameters.
var bingMapsKey = null;
var selectedOverlay = "thinkgeoCloudMap";

// Create the map.
var map = L.map('map').setView([33.150205, -96.8277325], 13);

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
            callback: function () { window.open('http://wiki.thinkgeo.com/wiki/map_suite_overlays', '_blank'); }
        }
    ]
}).addTo(map);

// Add ThinkGeoCloudMaps  
var thinkgeoCloudMapsLayer = L.tileLayer('https://{s}.thinkgeo.com/api/v1/maps/raster/light/x1/3857/256/{z}/{x}/{y}.png?apikey=PIbGd76RyHKod99KptWTeb-Jg9JUPEPUBFD3SZJYLDE~', {
    subdomains: ['cloud1', 'cloud2', 'cloud3', 'cloud4', 'cloud5', 'cloud6'],
    layers: 'ThinkGeoCloudMaps',
    format: 'image/png',
    styles: 'Light',
    version: '1.1.1'
});

// --------------------------------------------------------------------------------------
// Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
// an API Key. The following function is just for reminding you to input the key. 
// Feel free to remove this function after the key was input. 
// --------------------------------------------------------------------------------------
thinkgeoCloudMapsLayer.on('tileloadstart', function (e) {
    //e.tile.src = drawException();
    fetch(e.tile.src).then((response) => {
        if (response.status === 401) {
            var canvas = document.createElement("canvas");
            canvas.width = 256;
            canvas.height = 256;
            var context = canvas.getContext("2d");
            context.font = "14px Arial";
            context.strokeText("Backgrounds for this sample are", 10, 20);
            context.strokeText("powered by ThinkGeo Cloud Maps and", 10, 40);
            context.strokeText("require an API Key. This was sent", 10, 60);
            context.strokeText("to you via email when you signed up", 10, 80);
            context.strokeText("with ThinkGeo, or you can register", 10, 100);
            context.strokeText("now at https://cloud.thinkgeo.com", 10, 120);
            e.tile.src = canvas.toDataURL("image/png", 1);
        }
        else {
            response.blob().then((blob) => {
                if (blob) {
                    e.tile.src = URL.createObjectURL(blob);
                }
                else {
                    e.tile.src = "";
                }
            });
        }
    });
});

// Create open street map
var openStreetMap = L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="http://osm.org/copyright" title="OpenStreetMap" target="_blank">OpenStreetMap</a> contributors | Tiles Courtesy of <a href="http://www.mapquest.com/" title="MapQuest" target="_blank">MapQuest</a> <img src="http://developer.mapquest.com/content/osm/mq_logo.png" width="16" height="16">',
    subdomains: ['a', 'b', 'c']
});

// Create bing map without source, need to create bing map with bing maps key.
var bingMap;

// Create google map
var googleMap = L.tileLayer('http://{s}.google.com/vt/lyrs=m&x={x}&y={y}&z={z}', {
    maxZoom: 20,
    subdomains: ['mt0', 'mt1', 'mt2', 'mt3']
});

// Create layer group.
var baseLayerGroup = L.featureGroup([thinkgeoCloudMapsLayer]).addTo(map);

// Add dynamic tile layer to the map, it can be refreshed dynamically
var shapeLayer = L.dynamicLayer(L.Util.getRootPath() + 'Overlays/LoadCustomOverlay/{z}/{x}/{y}', {
    unloadInvisibleTiles: true, reuseTiles: false
}).addTo(map);

shapeLayer.on('loading', function () {
    $('#loadingImage').show();
});
shapeLayer.on('load', function () {
    $('#loadingImage').hide();
});

//Select dynamic checkbox. 
$("#CustomOverlay").click(function () {
    if ($("#CustomOverlay").is(':checked')) {
        if (!map.hasLayer(shapeLayer)) {
            map.addLayer(shapeLayer);
        }
    } else {
        if (map.hasLayer(shapeLayer)) {
            map.removeLayer(shapeLayer);
        }
    }
});

// Refresh sample by selecting sample item.
$('#leftPanelOptions div').bind('click', function () {
    selectedOverlay = $(this).attr('id');

    if (selectedOverlay === "bingMap" && bingMapsKey === null) {
        $('#bgMask').show();
        $('#editPanel').slideToggle("fast");
    } else {
        RefreshSelectedOverlay(selectedOverlay);
    }
});

// Hook up click event on "OK" button in edit dialog.
$('#btnSave').click(function () {
    bingMapsKey = document.getElementById("bingMapsKey").value;
    var paras = {
        keyStr: bingMapsKey
    };
    // Validate Bing Maps key. If successful, replace the basemap to Bing Maps.
    var url = L.Util.getRootPath() + '/Overlays/ValidateBingMapsKey/';
    $.post(url, { '': JSON.stringify(paras) }, function (data) {
        if (data) {
            hideDlg();
            bingMap = L.bingLayer(bingMapsKey, { type: "Aerial" });
            RefreshSelectedOverlay(selectedOverlay);
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

// Show or hide overlay options.
$("#overlayExpandOrCollapseButton").click(function () {
    if ($(this).attr("class") === "expand") {
        $(this).attr("class", "collapse");
        $("#overlaysList").hide();
    } else {
        $(this).attr("class", "expand");
        $("#overlaysList").show();
    }
});

// Show or hide left navigation panel.
$('html').click(function () {
    $('#leftPanel').animate({
        'left': -$('#leftPanel').width() + 'px'
    });
});

// Refresh current overlay by selection.
function RefreshSelectedOverlay(selectedOverlay) {
    map.setView([33.150205, -96.8277325], 13);
    baseLayerGroup.clearLayers();
    $('.selected').attr('class', 'unselected');
    $('#' + selectedOverlay).attr('class', 'selected');

    // Replace basemap.
    switch (selectedOverlay) {
        case "bingMap":
            baseLayerGroup.addLayer(bingMap);
            break;
        case "thinkgeoCloudMap":
            baseLayerGroup.addLayer(thinkgeoCloudMapsLayer);
            break;
        case "openStreetMap":
            baseLayerGroup.addLayer(openStreetMap);
            break;
        case "googleMap":
            baseLayerGroup.addLayer(googleMap);
            break;
        default:
    }
    baseLayerGroup.bringToBack();
}

// Hide edit dialog.
function hideDlg() {
    $('#bgMask').hide();
    $('#editPanel').slideToggle("fast");
}
