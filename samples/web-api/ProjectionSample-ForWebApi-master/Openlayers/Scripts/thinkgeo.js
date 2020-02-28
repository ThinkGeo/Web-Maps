// Initialize default value .
var projection3857 = ol.proj.get('EPSG:3857');
var projection4326 = ol.proj.get('EPSG:4326');

// Custom projection EPSG2163.
proj4.defs('EPSG:2163', '+proj=laea +lat_0=45 +lon_0=-100 +x_0=0 +y_0=0 +a=6370997 +b=6370997 +units=m +no_defs');
var projection2163 = ol.proj.get('EPSG:2163');
projection2163.setExtent([-4040784.5135, -4577524.9210, 4668901.4484, 4785105.1096]);

// Initialize the map.
var map = new ol.Map({
    target: 'map',
    controls: ol.control.defaults({ attribution: false, rotate: false }).extend(
        [new ol.control.Attribution({ collapsible: false })]),
    view: new ol.View({ center: [0, 0], zoom: 3, projection: projection3857 })
});

// Add image buttons for layers and help.
var imgControls = new app.ImagesControl({
    imgs: [
        {
            id: 'btnProjectionOptions',
            src: 'Images/layers.png',
            title: 'Show layers',
            callback: function () { $('#leftPanel').animate({ 'left': '0px' }); }
        },
        {
            id: 'btnInfo',
            src: 'Images/info.png',
            title: 'Show help',
            callback: function () { window.open('http://wiki.thinkgeo.com/wiki/map_suite_projection', '_blank'); }
        }
    ]
});
map.addControl(imgControls);

// Create ThinkGeoCloudMap layer.
var thinkGeoCloudMapsLayer = new ol.layer.Tile({
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

map.addLayer(thinkGeoCloudMapsLayer);

// Create countries layer, it has four source.
var layerDecimalDegreeSource = new ol.source.XYZ({
    projection: projection4326,
    url: getRootPath() + 'Projection/LoadCountriesLayer/DecimalDegree/{z}/{x}/{y}',
    maxZoom: 19,
    tileLoadFunction: function (imageTile, src) { imageTile.getImage().src = src + '?t=' + new Date().getTime(); }
});
var layerMercatorSource = new ol.source.XYZ({
    projection: projection3857,
    url: getRootPath() + 'Projection/LoadCountriesLayer/Mercator/{z}/{x}/{y}',
    maxZoom: 19,
    tileLoadFunction: function (imageTile, src) { imageTile.getImage().src = src + '?t=' + new Date().getTime(); }
});

// Custom projection source.
var layerCustomProjectionSource = new ol.source.XYZ({
    projection: projection2163,
    url: getRootPath() + 'Projection/LoadCustomProjectionLayer/{z}/{x}/{y}',
    maxZoom: 19,
    tileLoadFunction: function (imageTile, src) { imageTile.getImage().src = src + '?t=' + new Date().getTime(); }
});
// Create rotation Source.
var rotationSource = new ol.source.XYZ({
    projection: projection3857,
    url: getRootPath() + 'Projection/LoadRotationLayers/10/-10776836.6105256/3912344.07403825/{z}/{x}/{y}',
    maxZoom: 19,
    tileLoadFunction: function (imageTile, src) { imageTile.getImage().src = src + '?t=' + new Date().getTime(); }
});

var countriesLayer = new ol.layer.Tile({ source: layerMercatorSource });

map.addLayer(countriesLayer);

// Refresh sample by selecting sample item.
$('#leftPanelOptions div').bind('click', function () {
    // Get current sample type.
    var selected = $(this).attr('id');

    // Refresh UI.
    RefreshUI(selected);

    // Remove map's layers.
    map.removeLayer(countriesLayer);
    map.removeLayer(thinkGeoCloudMapsLayer);

    // According to the sample type, change map's crs and add layers.

    switch (selected) {
        case "webMercatorProjection":
            map.setView(new ol.View({ center: [0, 0], zoom: 3, projection: projection3857 }));

            // Add OSM WorldMapKit layer with Mercator Projection.
            map.addLayer(thinkGeoCloudMapsLayer);

            // Add countries layer with Mercator Projection.
            countriesLayer.setSource(layerMercatorSource);
            map.addLayer(countriesLayer);
            break;
        case "rotationProjection":
            map.setView(new ol.View({ center: ol.proj.transform([-95.81131, 40.14711], 'EPSG:4326', 'EPSG:3857'), zoom: 4, projection: projection3857 }));

            // Add countries layer with Rotation Projection.
            countriesLayer.setSource(rotationSource);
            map.addLayer(countriesLayer);
            break;
        case "customProjection":
            var newView = new ol.View({ center: [0, 0], projection: projection2163, zoom: 2 });
            map.setView(newView);

            // Add countries layer with Custom Projection.
            countriesLayer.setSource(layerCustomProjectionSource);
            map.addLayer(countriesLayer);
            break;
    }
});

// Update the center point for roation projection when dragged.
var coordinateX = -10776836.6105256;
var coordinateY = 3912344.07403825;
map.on("moveend", function () {
    var view = map.getView();
    var coordinate = view.getCenter();
    coordinateX = coordinate[0];
    coordinateY = coordinate[1];
    $("#pivotCenter").html("X: " + coordinateX + ",<br/>" + "Y: " + coordinateY);
});

// Expand or collapse Projection information div.
$("#descriptionExpandOrCollapseButton").click(function () {
    if ($(this).hasClass("expand")) {
        $(this).removeClass('expand');
        $(this).addClass("collapse");
        $("#descriptionPanel").hide();
    } else {
        $(this).removeClass('collapse');
        $(this).addClass("expand");
        $("#descriptionPanel").show();
    }
});

// Show or hide left navigation panel.
$("html").click(function () {
    $('#leftPanel').animate({ 'left': -$('#leftPanel').width() + 'px' });
});

// Refresh UI, including left panel and description.
function RefreshUI(selectedProjection) {
    $('.selected').attr('class', 'unselected');
    $('#' + selectedProjection).attr('class', 'selected');

    $('.selectedDescription').attr('class', 'unselectedDescription');
    if (selectedProjection === "rotationProjection") {
        $('#' + selectedProjection + "Description").attr('class', 'selectedDescription');
    }
    else {
        $('#projectionDescription').attr('class', 'selectedDescription');
    }

    // Update projection's information.
    var epsgParameters = $("#" + selectedProjection).attr("epsg");
    $("#epsgId").html(epsgParameters);
    var baseUrl = getRootPath() + "/Projection/InformationDataRequest/" + epsgParameters;
    $.get(baseUrl, function (res) {
        $("#proj4String").html(res.Proj4String);
        $("#unit").html(res.Unit);
    });
}


var rotationAngle = 10;
// Refresh rotation layer when increasing angle.
function LeftRotation() {
    rotationAngle += 10;
    $("#currentAngle").html("Current angle: " + rotationAngle);
    rotationSource.setUrl(getRootPath() + 'Projection/LoadRotationLayers/' + rotationAngle + '/' + coordinateX + '/' + coordinateY + '/{z}/{x}/{y}');
}
// Refresh rotation layer when reduceing angle.
function RightRotation() {
    rotationAngle -= 10;
    $("#currentAngle").html("Current angle: " + rotationAngle);
    rotationSource.setUrl(getRootPath() + 'Projection/LoadRotationLayers/' + rotationAngle + '/' + coordinateX + '/' + coordinateY + '/{z}/{x}/{y}');
}