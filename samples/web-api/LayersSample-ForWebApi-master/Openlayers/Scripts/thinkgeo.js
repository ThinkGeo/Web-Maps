// Create the map.
var map = new ol.Map({
    target: 'map',
    controls: ol.control.defaults({ attribution: false }).extend(
        [new ol.control.Attribution({
            collapsible: false
        })]),
    view: new ol.View({
        center: ol.proj.transform([-95.81131, 40.14711], 'EPSG:4326', 'EPSG:3857'),
        zoom: 5
    })
});

// Add ThinkGeoCloudMaps as the map's background. 
var thinkgeoCloudMapsLayer = new ol.layer.Tile({
    source: new ol.source.XYZ({
        url: 'https://cloud{1-6}.thinkgeo.com/api/v1/maps/raster/light/x1/3857/512/{z}/{x}/{y}.png?apikey=ThinkGeo Cloud API Key',
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
map.addLayer(thinkgeoCloudMapsLayer);

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