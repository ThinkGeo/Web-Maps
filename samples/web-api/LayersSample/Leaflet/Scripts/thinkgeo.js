// Create the map.
var map = L.map('map').setView([40.14711, -95.81131], 5);

// Add image buttons for layers and help.
L.imageButtons({
    imgs: [
        {
            src: 'Images/layers.png',
            id: 'lyrOptions',
            title: 'Show layers',
            callback: function () {
                $('#layers-panel').animate({
                    'left': '0px'
                });
            }
        },
        {
            src: 'Images/info.png',
            id: 'btnInfo',
            title: 'Show help',
            callback: function () { window.open('http://wiki.thinkgeo.com/wiki/map_suite_layers', '_blank'); }
        }
    ]
}).addTo(map);

// Add ThinkGeoCloudMaps as the map's background. 
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
thinkgeoCloudMapsLayer.addTo(map);

// Add dynamic tile layer to the map, it can be refreshed dynamically
var shapeLayer = L.dynamicLayer(L.Util.getRootPath() + 'Layers/VectorLayers/shapeFile/{z}/{x}/{y}', { unloadInvisibleTiles: true, reuseTiles: false }).addTo(map);
// Show or hide loading image.
shapeLayer.on('loading', function () {
    $('#loadingImage').show();
});
shapeLayer.on('load', function () {
    $('#loadingImage').hide();
});

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
    // Redraw the map, if no osm background layer, then add it.
    if (!map.hasLayer(thinkgeoCloudMapsLayer)) {
        map.addLayer(thinkgeoCloudMapsLayer);
        thinkgeoCloudMapsLayer.bringToBack();
    }

    //update message for selected layer
    var layerName = $(this).find("h6").text();
    var layerDescription = $(this).find("p").text();
    $("#layer-description").text(layerName + "  (" + layerDescription + ")");

    shapeLayer.setUrl(L.Util.getRootPath() + "Layers/" + categoryName + "/" + selectedLayer + '/{z}/{x}/{y}');
    map.setView(L.latLng(center[0], center[1]), zoom);
    shapeLayer.redraw();
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