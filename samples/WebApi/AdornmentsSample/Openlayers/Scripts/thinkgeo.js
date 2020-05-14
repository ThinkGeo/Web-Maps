var zoom = 7;
var center = [33.150205, -96.8277325];
var selectedAdornment = 'ScaleBarAdornment';

var map = new ol.Map({
    target: 'map',
    controls: ol.control.defaults({ attribution: false }).extend(
        [new ol.control.Attribution({
            collapsible: false
        })]),
    view: new ol.View({
        center: ol.proj.transform([-95.81131, 40.14711], 'EPSG:4326', 'EPSG:3857'),
        zoom: 5,
        maxZoom: 19,
        minZoom: 0
    })
});

var imgControls = new app.ImagesControl({
    imgs: [
        {
            id: 'adornmentOptions',
            src: 'Images/LeftControlBar.png',
            title: 'Show Adornments Control Bar',
            callback: function () { $('#leftControlBar').animate({ 'left': '0px' }); }
        },
        {
            id: 'btnInfo',
            src: 'Images/info.png',
            title: 'Show help',
            callback: function () { window.open('http://wiki.thinkgeo.com/wiki/map_suite_adornments', '_blank'); }
        }
    ]
});
map.addControl(imgControls);

var url = 'https://cloud{1-6}.thinkgeo.com/api/v1/maps/raster/light/x1/3857/512/{z}/{x}/{y}.png?apikey=ThinkGeo Cloud API Key';

var thinkgeoCloudMapsLayer = new ol.layer.Tile({
    source: new ol.source.XYZ({
        url: url,
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
        // An API Key. The following function is just for reminding you to input the key. 
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

var xyzSource = new ol.source.XYZ({
    url: getRootPath() + 'Adornments/SchoolShapeFileLayer/{z}/{x}/{y}',
    maxZoom: 19
});
xyzSource.tileLoadFunction = function (imageTile, src) {
    imageTile.getImage().src = src + '?t=' + new Date().getTime();;
};
var shapLayer = new ol.layer.Tile({
    source: xyzSource
});
map.addLayer(shapLayer);

var adornment = AdornmentsOverlay({
    adornmentType: AdornmensType.ScaleBarAdornment,
    id: 'adornmentImg'
});
addTo(map);
var size = map.getSize();
var extent = map.getView().calculateExtent(size);
setUrl('Adornments/ScaleBarAdornment/' + size + '/' + extent + '/');

$('.leftControlBarBody div').click(function () {
    var leftControlBarItems = $(".leftControlBarBody div");
    for (var i = 0; i < leftControlBarItems.length; i++) {
        $(leftControlBarItems[i]).attr("class", "unselected");
    }
    $(this).attr("class", "selected");

    selectedAdornment = $(this).attr('id');

    if (selectedAdornment === "LegendAdornment") {
        map.setView(new ol.View({
            center: ol.proj.transform([-96.8216, 33.1447], 'EPSG:4326', 'EPSG:3857'),
            zoom: 15
        }));
        shapLayer.setVisible(true);
    } else {
        shapLayer.setVisible(false);
    }

    size = map.getSize();
    extent = map.getView().calculateExtent(size);
    adornmentType = selectedAdornment;
    setUrl('Adornments/' + selectedAdornment + '/' + size + '/' + extent + '/');
});

$("html").click(function () {
    $('#leftControlBar').animate({
        'left': -$('#leftControlBar').width() + 'px'
    });
});