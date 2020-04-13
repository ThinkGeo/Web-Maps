// Refine leaflet L.CRS.EPSG4326.
L.CRS.EPSG4326.transformation = new L.Transformation(1 / 360, 0.5, -1 / 360, 0.25);

// Initialize the map.
var map = L.map('map', { crs: L.CRS.EPSG3857 }).setView([0, 0], 3);

// Add image buttons for layers and help.
L.imageButtons({
    imgs: [
        {
            id: 'btnProjectionOptions',
            src: 'Images/Layers.png',
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
thinkgeoCloudMapsLayer.addTo(map);

// Create countries layer for Web Mercator projection.
var countriesLayer = L.dynamicLayer(L.Util.getRootPath() + 'Projection/LoadCountriesLayer/Mercator/{z}/{x}/{y}', { unloadInvisibleTiles: true, reuseTiles: true }).addTo(map);

// Refresh sample by selecting item.
var selected;
$('#leftPanelOptions div').bind('click', function () {
    // Get current sample type.
    selected = $(this).attr('id');

    // Refresh UI.
    RefreshUI(selected);

    // Remove map's layers.
    map.removeLayer(countriesLayer);
    map.removeLayer(thinkgeoCloudMapsLayer);

    // According to the sample type, change map's crs and set current layers.
    switch (selected) {
        case "webMercatorProjection":
            map.options.crs = L.CRS.EPSG3857;
            thinkgeoCloudMapsLayer.addTo(map);

            // Add countries layer with Mercator Projection.
            countriesLayer.setUrl(L.Util.getRootPath() + 'Projection/LoadCountriesLayer/Mercator/{z}/{x}/{y}');
            countriesLayer.addTo(map);
            break;
        case "rotationProjection":
            map.options.crs = L.CRS.EPSG3857;

            // Add countries layer with Rotation Projection.
            countriesLayer.setUrl(L.Util.getRootPath() + 'Projection/LoadRotationLayers/10/-10776836.6105256/3912344.07403825/{z}/{x}/{y}');
            countriesLayer.addTo(map);
            break;
        case "customProjection":
            map.options.crs = L.CRS.EPSG3857;

            // Add countries layer with Custom Projection.
            countriesLayer.setUrl(L.Util.getRootPath() + 'Projection/LoadCustomProjectionLayer/{z}/{x}/{y}');
            countriesLayer.addTo(map);
            break;
    }

    var center = $(this).attr('center').split(',');
    var zoom = $(this).attr('zoom');
    map.setView(L.latLng(center[0], center[1]), zoom);
});

// Update the center point for roation projection when dragged.
var coordinateX = -10776836.6105256;
var coordinateY = 3912344.07403825;
map.on("dragend", function () {
    var latLng = map.getCenter();
    var coordinate = L.CRS.EPSG3857.project(latLng);
    coordinateX = coordinate.x;
    coordinateY = coordinate.y;
    $("#pivotCenter").html("X: " + coordinateX + ",<br/>" + "Y: " + coordinateY);
});

// Expand or collapse Projection information div.
$("#descriptionExpandOrCollapseButton").click(function () {
    if ($(this).attr("class") == "expand") {
        $(this).attr("class", "collapse");
        $("#descriptionPanel").hide();
    } else {
        $(this).attr("class", "expand");
        $("#descriptionPanel").show();
    }
});

// Show or hide left navigation panel.
$('html').click(function () {
    $('#leftPanel').animate({
        'left': -$('#leftPanel').width() + 'px'
    });
});

// Refresh UI, including left panel and description.
function RefreshUI(selectedProjection) {
    $('.selected').attr('class', 'unselected');
    $('#' + selectedProjection).attr('class', 'selected');

    $('.selectedDescription').attr('class', 'unselectedDescription');
    if (selectedProjection == "rotationProjection") {
        $('#' + selectedProjection + "Description").attr('class', 'selectedDescription');
    }
    else {
        $('#projectionDescription').attr('class', 'selectedDescription');
    }

    // Update projection's information.
    var epsgParameters = $("#" + selectedProjection).attr("epsg");
    $("#epsgId").html(epsgParameters);
    var baseUrl = L.Util.getRootPath() + "/Projection/InformationDataRequest/" + epsgParameters;
    $.get(baseUrl, function (res) {
        $("#proj4String").html(res.Proj4String);
        $("#unit").html(res.Unit);
    });
}

var rotationAngle = 10;
// Refresh rotation layers when increasing angle.
function LeftRotation() {
    rotationAngle += 10;
    $("#currentAngle").html("Current angle: " + rotationAngle);
    var baseUrl = L.Util.getRootPath() + 'Projection/LoadRotationLayers/' + rotationAngle + '/' + coordinateX + '/' + coordinateY + '/{z}/{x}/{y}';
    countriesLayer.setUrl(baseUrl);
}
// Refresh rotation layers when reduceing angle.
function RightRotation() {
    rotationAngle -= 10;
    $("#currentAngle").html("Current angle: " + rotationAngle);
    var baseUrl = L.Util.getRootPath() + 'Projection/LoadRotationLayers/' + rotationAngle + '/' + coordinateX + '/' + coordinateY + '/{z}/{x}/{y}';
    countriesLayer.setUrl(baseUrl);
}