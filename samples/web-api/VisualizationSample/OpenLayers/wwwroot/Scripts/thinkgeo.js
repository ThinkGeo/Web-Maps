var accessId = guid();
var selectedLayer = 'FilterStyle';
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
        center: [-10777396.499651, 4821690.0604384],
        zoom: 4
    })
});

// Added ThinkGeoCloudMap as the background map.
var thinkGeoCloudMapsLayer = new ol.mapsuite.VectorTileLayer('https://cdn.thinkgeo.com/worldstreets-styles/3.0.0/light.json', {
    apiKey: thinkgeoApiKey,
    layerName: 'light'
});
map.addLayer(thinkGeoCloudMapsLayer);

// Add a dynamic layer to the map, it can be refreshed dynamically.
var xyzSource = new ol.source.XYZ({
    url: getRootPath() + '/tile/FilterStyle/{z}/{x}/{y}/' + accessId,
    maxZoom: 19
});
xyzSource.tileLoadFunction = function (imageTile, src) {
    imageTile.getImage().src = src + '?t=' + new Date().getTime();;
};
map.addLayer(new ol.layer.Tile({
    source: xyzSource
}));

var imgControls = new app.ImagesControl({
    imgs: [
        {
            id: 'lyrOptions',
            src: 'Images/layers.png',
            title: 'Show layers',
            callback: function () {
                $('#styles-panel').animate({
                    'left': '0px'
                });
            }
        },
        {
            id: 'btnConfig',
            src: 'Images/gear.png',
            title: 'Show style settings',
            css: 'active',
            callback: function () { showDlg(); }
        },
        {
            id: 'btnInfo',
            src: 'Images/info.png',
            title: 'Show help',
            callback: function () { window.open('http://wiki.thinkgeo.com', '_blank'); }
        }
    ]
});
map.addControl(imgControls);

map.on("click", function () {
    $('#styles-panel').animate({
        'left': -$('#styles-panel').width() + 'px'
    });
});

// Apply 'select' style after clicking on a style item of left-panel
$('#style-options div').click(function () {
    var layers = $("#style-options div");
    for (var i = 0; i < layers.length; i++) {
        $(layers[i]).attr("class", "unselected");
    }
    $(this).attr("class", "selected");

    // Disable edit toolbar button
    selectedLayer = $(this).attr('id');
    $('#btnConfig').removeClass('active');
    $('#btnConfig').removeClass('deactive');
    if (selectedLayer == 'FilterStyle') {
        $('#btnConfig').addClass('active');
    } else {
        $('#btnConfig').addClass('deactive');
    }

    if (selectedLayer == 'ZedGraphStyle') {
        $('#zedgraph-legend').show();
    }
    else {
        $('#zedgraph-legend').hide();
    }

    // Redraw the map
    redrawLayer();
});

// Hook up click event on "save" button in edit dialog
$('#btnSave').click(function () {
    var paras = {
        filterExpression: $('#filterCondition option:selected').text(),
        filterValue: $('#filterMatchValue').val()
    }
    var url = getRootPath() + '/tile/UpdateFilterStyle/' + accessId;
    $.post(url, { '': JSON.stringify(paras) }, function (data) {
        if (data) {
            hideDlg();
            redrawLayer();
        }
        else {
            alert('Update failed.');
        }
    });
});

$('#btnCancel').click(function (e) {
    hideDlg();
});

function redrawLayer() {
    switch (selectedLayer) {
        case 'IconStyle':
            map.setView(new ol.View({
                center: [-10770913.637479, 3917015.5139286],
                zoom: 13
            }));
            break;
        case 'ZedGraphStyle':
        default:
            map.setView(new ol.View({
                center: [-10777396.499651, 4821690.0604384],
                zoom: 4
            }));
            break;
    }
    xyzSource.setUrl(getRootPath() + '/tile/' + selectedLayer + '/{z}/{x}/{y}/' + accessId);
    xyzSource.refresh();
}

// Show/hide "edit" dialog in model mode
function showDlg() {
    if ($('#btnConfig').hasClass('active')) {
        $('#bg-mask').show();

        $('#edit-panel').slideToggle("fast");
    }
}

function hideDlg() {
    $('#bg-mask').hide();
    $('#edit-panel').slideToggle("fast");
}

// Do the layers panel animation
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