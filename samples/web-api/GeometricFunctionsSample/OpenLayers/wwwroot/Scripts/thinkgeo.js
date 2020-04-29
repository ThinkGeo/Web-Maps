var accessId = guid();
var actionType = 'Union', xyzSource;
var thinkgeoApiKey = 'PIbGd76RyHKod99KptWTeb-Jg9JUPEPUBFD3SZJYLDE~';

// Initialize the map.
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
        center: [-10774179.366367, 3914858.6395301],
        zoom: 17
    })
});

// Added ThinkGeoCloudMaps as the background map.
var thinkgeoCloudMapsLayer = new ol.mapsuite.VectorTileLayer('https://cdn.thinkgeo.com/worldstreets-styles/3.0.0/light.json', {
    apiKey: thinkgeoApiKey,
    layerName: 'light'
});
map.addLayer(thinkgeoCloudMapsLayer);

// Add the layer to the map to render, it will be refreshed dynamically.
xyzSource = new ol.source.XYZ({
    url: getRootPath() + '/tile/input/{z}/{x}/{y}/1,2,3,4',
    maxZoom: 19
});
// Add timestamp to avoid broswer cache.
xyzSource.tileLoadFunction = function (imageTile, src) {
    imageTile.getImage().src = src + '?t=' + new Date().getTime();;
};
map.addLayer(new ol.layer.Tile({
    source: xyzSource
}));

// Add image buttons for layers, execute etc.
var imgControls = new app.ImagesControl({
    imgs: [
        {
            src: 'Images/layers.png', id: 'lyrOptions', title: 'Show layers', callback: function () {
                $('#styles-panel').animate({
                    'left': '0px'
                });
            }
        },
        {
            src: 'Images/go.png', id: 'btnGo', title: 'Run', callback: function () {
                var paramters = geometricData[actionType].param;
                var url = getRootPath() + '/tile/Execute/' + actionType + '/' + accessId;

                $.post(url, { '': paramters }, function (data) {
                    xyzSource.setUrl(getRootPath() + '/tile/output/{z}/{x}/{y}/' + accessId);
                });
            }
        },
        { src: 'Images/info.png', id: 'btnInfo', title: 'Show help', callback: function () { window.open('http://wiki.thinkgeo.com', '_blank'); } }
    ]
});
map.addControl(imgControls);

// Select a style from options
$('#style-options div').click(function () {
    var actionContainers = $("#style-options div");
    for (var i = 0; i < actionContainers.length; i++) {
        $(actionContainers[i]).attr("class", "unselected");
    }
    $(this).attr("class", "selected");

    actionType = $(this).attr('id');

    var featureIds = geometricData[actionType].init;
    xyzSource.setUrl(getRootPath() + '/tile/input/{z}/{x}/{y}/' + featureIds.toString());

    var centerInfo = geometricData[actionType].center;
    var projectedPoint = ol.proj.transform([centerInfo.x, centerInfo.y], 'EPSG:4326', 'EPSG:3857');
    map.setView(new ol.View({
        center: projectedPoint,
        zoom: centerInfo.z
    }));
});

// Do the layers panel animation
map.on("click", function () {
    $('#styles-panel').animate({
        'left': -$('#styles-panel').width() + 'px'
    });
});
$("html").click(function () {
    $('#styles-panel').animate({
        'left': -$('#styles-panel').width() + 'px'
    });
});

// Add a utility methods to get guid.
function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
    }
    return (s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4()).toLowerCase();
};

// Sample data.
var geometricData = {
    Union: { param: ['1', '2', '3', '4'], init: ['1', '2', '3', '4'], center: { x: -96.7861120308635, y: 33.1472685431747, z: 17 } },
    Difference: { param: [{ 'Key': 'source', 'Value': '5' }, { 'Key': 'target', 'Value': '6' }], init: ['5', '6'], center: { x: -96.7861120308635, y: 33.1462727687461, z: 17 } },
    Buffer: { param: [{ 'Key': 'id', 'Value': '7' }, { 'Key': 'distance', 'Value': '15' }, { 'Key': 'unit', 'Value': 'Meter' }], init: ['7'], center: { x: -96.7355329966487, y: 33.1833649004228, z: 17 } },
    Scale: { param: [{ 'Key': 'id', 'Value': '8' }, { 'Key': 'percentage', 'Value': '20' }], init: ['8'], center: { x: -96.7870806586458, y: 33.1460327413192, z: 17 } },
    Rotate: { param: [{ 'Key': 'id', 'Value': '9' }, { 'Key': 'angle', 'Value': '22.5' }], init: ['9'], center: { x: -96.7857794367446, y: 33.1472946523085, z: 17 } },
    CenterPoint: { param: '10', init: ['10'], center: { x: -96.7862554711922, y: 33.1496806178557, z: 17 } },
    CalculateArea: { param: '11', init: ['11'], center: { x: -96.7870806586458, y: 33.1460327413192, z: 17 } },
    Simplify: { param: [{ 'Key': 'id', 'Value': '12' }, { 'Key': 'distance', 'Value': '30' }, { 'Key': 'unit', 'Value': 'Meter' }], init: ['12'], center: { x: -96.8219987975971, y: 33.1320279418704, z: 17 } },
    Split: { param: [{ 'Key': 'polygonId', 'Value': '13' }, { 'Key': 'lineId', 'Value': '14' }], init: ['13', '14'], center: { x: -96.8113917603578, y: 33.1255858564896, z: 17 } },
    CalculateShortestLine: { param: [{ 'Key': 'id1', 'Value': '15' }, { 'Key': 'id2', 'Value': '16' }], init: ['15', '16'], center: { x: -96.796118336201, y: 33.1554078616389, z: 17 } },
    CalculateLength: { param: '17', init: ['17'], center: { x: -96.8134319520846, y: 33.1559860794668, z: 17 } },
    LineOnLine: { param: [{ 'Key': 'id', 'Value': '18' }, { 'Key': 'startDistance', 'Value': '80' }, { 'Key': 'distance', 'Value': '450' }, { 'Key': 'unit', 'Value': 'Meter' }], init: ['18', '29', '30', '31', '32', '33', '34'], center: { x: -96.8282498246283, y: 33.1397778134769, z: 17 } },
    Clip: { param: [{ 'Key': 'id', 'Value': '19' }, { 'Key': 'clippingSourceIds', 'Value': '37,38,39,40,41,42,43,44' }], init: ['19', '37', '38', '39', '40', '41', '42', '43', '44'], center: { x: -96.8132293915322, y: 33.1382982431531, z: 17 } },
    ConvexHull: { param: [{ 'Key': 'polygonId', 'Value': '20' }, { 'Key': 'pointIds', 'Value': '21,22,23,24,25' }], init: ['20', '21', '22', '23', '24', '25'], center: { x: -96.8149300680701, y: 33.1574603876092, z: 17 } },
    Snapping: { param: [{ 'Key': 'sourceId', 'Value': '26' }, { 'Key': 'targetId', 'Value': '27' }], init: ['26', '27', '35', '36'], center: { x: -96.8137451552825, y: 33.1383491522859, z: 17 } },
    EnvelopBoundingbox: { param: '28', init: ['28'], center: { x: -96.7862554711922, y: 33.1496806178557, z: 17 } },
};