// Initialize default value .
var projection3857 = ol.proj.get('EPSG:3857');
var projection4326 = ol.proj.get('EPSG:4326');

// Custom projection EPSG2163.
proj4.defs('EPSG:2163', '+proj=laea +lat_0=45 +lon_0=-100 +x_0=0 +y_0=0 +a=6370997 +b=6370997 +units=m +no_defs');
var projection2163 = ol.proj.get('EPSG:2163');
projection2163.setExtent([-4040784.5135, -4577524.9210, 4668901.4484, 4785105.1096]);
var thinkgeoApiKey = 'PIbGd76RyHKod99KptWTeb-Jg9JUPEPUBFD3SZJYLDE~';

// Initialize the map.
var map = new ol.Map({
    target: 'map',
	renderer: 'webgl',
    loadTilesWhileAnimating: true,
    loadTilesWhileInteracting: true,
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

// Add thinkGeoCloudMapsLayer as the map's background layer.
var thinkGeoCloudMapsLayer = new ol.mapsuite.VectorTileLayer('https://cdn.thinkgeo.com/worldstreets-styles/3.0.0/light.json', {
    apiKey: thinkgeoApiKey,
    layerName: 'light'
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