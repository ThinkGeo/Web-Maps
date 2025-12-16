var thinkgeoApiKey = 'PIbGd76RyHKod99KptWTeb-Jg9JUPEPUBFD3SZJYLDE~';

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
    url: getRootPath() + '/Layers/VectorLayers/shapeFile/{z}/{x}/{y}',
    maxZoom: 19
});
xyzSource.tileLoadFunction = function (imageTile, src) {
    imageTile.getImage().src = src + '?t=' + new Date().getTime();
};
map.addLayer(new ol.layer.Tile({
    source: xyzSource
}));

// Add image buttons for layers and help.
var imgControls = new app.ImagesControl({
    imgs: [
        {
            id: 'lyrOptions',
            src: 'Images/layers.png',
            title: 'Show layers',
            callback: function () {
                $('#layers-panel').animate({
                    'left': '0px'
                });
            }
        },
        {
            id: 'btnInfo',
            src: 'Images/info.png',
            title: 'Show help',
            callback: function () { window.open('http://wiki.thinkgeo.com/wiki/map_suite_layers', '_blank'); }
        }
    ]
});
map.addControl(imgControls);

// Refresh sample by selecting sample item.
$('.layers-item div').click(function () {
    var layers = $(".layers-item div");
    for (var i = 0; i < layers.length; i++) {
        $(layers[i]).attr("class", "unselected");
    }
    $(this).attr("class", "selected");
    // Disable edit toolbar button
    var selectedLayer = $(this).attr('id');
    var categoryName = $(this).parent().parent().attr("id");
    var center = $(this).attr('center').split(',');
    var zoom = $(this).attr('zoom');

    //update message for selected layer
    var layerName = $(this).find("h6").text();
    var layerDescription = $(this).find("p").text();
    $("#layer-description").text(layerName + "  (" + layerDescription + ")");
    // Redraw the map
    xyzSource.setUrl(getRootPath() + "Layers/" + categoryName + "/" + selectedLayer + '/{z}/{x}/{y}');
    map.setView(new ol.View({
        center: ol.proj.transform([parseFloat(center[1]), parseFloat(center[0])], 'EPSG:4326', 'EPSG:3857'),
        zoom: zoom
    }));
});

// Switch the layers-item.
$(".layers-category-name").click(function () {
    if ($(this).hasClass("collapse")) {
        $(this).parent().parent().find("ul").hide();
        $(this).removeClass("collapse");
    } else {
        $(this).parent().parent().find("ul").hide();
        $(this).parent().find("ul").show();
        $(this).parent().parent().find(".collapse").removeClass("collapse");
        $(this).addClass("collapse");
    }
});

// Stop the click bubble event to avoid panel collapse when click on the category item.
$(".layers-category-name").each(function () {
    this.onclick = function (e) {
        if (e && e.stopPropagation) {
            e.stopPropagation();
        } else {
            e = window.event;
            e.cancelBubble = true;
        }
    };
});
// Show or hide left navigation panel.
$("html").click(function () {
    $('#layers-panel').animate({
        'left': -$('#layers-panel').width() + 'px'
    });
});