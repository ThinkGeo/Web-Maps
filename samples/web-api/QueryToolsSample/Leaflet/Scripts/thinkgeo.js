// Initialize default value for some parameters.
var initZoom = 3;
var initLatLng = L.latLng(0, 0);
var selectedQuery = 'SpatialQuery';
var currentCoordinate = { x: 1115369.1167372921, y: 1937220.0448595073 };

// Create the map.
var map = L.map('map', { zoomAnimation: false, center: initLatLng, zoom: initZoom });

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
                if ($('#QueryAllFeaturesContent').css("display") === "block") {
                    $('#QueryAllFeaturesContent').animate({
                        'left': '220px',
                        'width': $('#QueryAllFeaturesContent').width() - 220 + 'px'
                    });
                }
            }
        },
        {
            src: 'Images/info.png',
            id: 'btnInfo',
            title: 'Show help',
            callback: function () { window.open('http://wiki.thinkgeo.com/wiki/map_suite_querying', '_blank'); }
        }
    ]
}).addTo(map);

// Add ThinkGeoCloudMaps as the map's background. 
var thinkgeoCloudMapsLayer = L.tileLayer('https://{s}.thinkgeo.com/api/v1/maps/raster/light/x1/3857/256/{z}/{x}/{y}.png?apikey=ThinkGeo Cloud API Key', {
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

// Add query tool's source layer to map. 
L.dynamicLayer(L.Util.getRootPath() + 'QueryTools/InitializeSourceLayer/{z}/{x}/{y}', { unloadInvisibleTiles: true, reuseTiles: true }).addTo(map);

// Add result layer to map for display the query result features.
var resultLayer = L.dynamicLayer(GetBaseUrl(), { unloadInvisibleTiles: true, reuseTiles: true }).addTo(map);

// Trigger the events when changing spatial query type. 
$('#SpatialQueryDescription input[name="SpatialQueryGroup"]').click(function () {
    var selectedQueryType = $(this).attr("id");
    map.setView(initLatLng, initZoom);
    resultLayer.setUrl(GetBaseUrl() + '?queryType=' + selectedQueryType);
});

// Trigger the events when changing distance in query features.
$("#WithinDistanceData").change(function () {
    var distance = $('#WithinDistanceData option:selected').attr("value");
    resultLayer.setUrl(GetBaseUrl() + '?distance=' + distance + '&lng=' + currentCoordinate.x + '&lat=' + currentCoordinate.y);
});

// Refresh sample by selecting sample item.
$('#leftPanelOptions div').bind('click', function () {
    map.setView(initLatLng, initZoom);
    currentCoordinate = { x: 1115369.1167372921, y: 1937220.0448595073 };

    selectedQuery = $(this).attr('id');
    RefreshUIBySelectedItem(selectedQuery);
    QueryAndFillData(selectedQuery, currentCoordinate);

    map.removeLayer(resultLayer);
    var hasTargetLayer = $(this).attr('hasTargetLayer');
    if (hasTargetLayer) {
        resultLayer.setUrl(GetBaseUrl());
        resultLayer.addTo(map);
    }
});

//Show or hide description.
$("#descriptionExpandOrCollapseButton").click(function () {
    if ($(this).hasClass("expand")) {
        $(this).removeClass('expand');
        $(this).addClass("collapse");
        $("#descriptionDetail").hide();
    } else {
        $(this).removeClass('collapse');
        $(this).addClass("expand");
        $("#descriptionDetail").show();
    }
});

// Show or hide left navigation panel.
$('html').click(function () {
    $('#leftPanel').animate({
        'left': -$('#leftPanel').width() + 'px'
    });
    if ($('#QueryAllFeaturesContent').css("display") === "block") {
        $('#QueryAllFeaturesContent').animate({
            'left': '0px',
            'width': '100%'
        });
    }
});

// Handler Map's click event. refresh map and data when clicking map.
map.on('click', function onMapClick(e) {
    currentCoordinate = L.CRS.EPSG3857.project(e.latlng);
    var baseUrl = GetBaseUrl() + '?lng=' + currentCoordinate.x + '&lat=' + currentCoordinate.y;
    if (selectedQuery === "QueryFeaturesWithinDistance") {
        var distance = $('#WithinDistanceData option:selected').attr("value");
        baseUrl = baseUrl + '&distance=' + distance;
    }
    else if (selectedQuery === "QueryColumnData") {
        QueryAndFillData(selectedQuery, currentCoordinate);
    }
    else {
        return;
    }

    resultLayer.setUrl(baseUrl);
});

// Zoom map to new extent by given world point.
function zoomToExtent(worldX, worldY, featureId) {
    resultLayer.setUrl(GetBaseUrl() + '?featureId=' + featureId);
    if (!map.hasLayer(resultLayer)) { resultLayer.addTo(map); }
    var point = L.point(worldX, worldY);
    map.setView(L.CRS.EPSG3857.unproject(point), 5);
}

// Refresh sample's UI by selected query type.
function RefreshUIBySelectedItem(selectedQuery) {
    $("#descriptionDetail").show();
    $('.collapse').attr('class', 'expand');

    $('.selected').attr('class', 'unselected');
    $('#' + selectedQuery).attr('class', 'selected');

    $(".selectedDescription").attr('class', 'unselectedDescription');
    $('#' + selectedQuery + 'Description').attr('class', 'selectedDescription');

    if (selectedQuery === "SpatialQuery") {
        $('#Within').prop("checked", true);
    }

    if (selectedQuery === 'QueryAllFeatures') {
        $("#descriptionExpandOrCollapseButton").css('bottom', '204px');
        $('#QueryAllFeaturesContent').show();

        var descriptionText = $('#' + selectedQuery).attr('description');
        $('#description').text(descriptionText);
        $('#description').show();

    } else {
        $("#descriptionExpandOrCollapseButton").css('bottom', '');
        $('#QueryAllFeaturesContent').hide();
        $('#description').hide();
    }
    if (selectedQuery === 'QueryColumnData') {
        $("#map").css('cursor', 'default');
    } else {
        $("#map").css('cursor', '');
    }
}

// Query data and fill the data to table.
function QueryAndFillData(selectedQuery, coordinate) {
    var url = L.Util.getRootPath() + '/QueryTools/' + selectedQuery + 'DataRequests';

    if (selectedQuery === 'QueryAllFeatures' || selectedQuery === 'QueryColumnData') {
        url = url + '?lng=' + coordinate.x + '&lat=' + coordinate.y;
        $.get(url, function (res) { $('#' + selectedQuery + 'Tbody').html(res); });
    } else if (selectedQuery === 'SQLQuery') {
        var sqlString = $("#SQLString").val();
        url = url + '?sqlString=' + sqlString;
        $.get(url, function (res) {
            $('#' + selectedQuery + 'Tbody').html(res);
            resultLayer.setUrl(GetBaseUrl());
            if (!map.hasLayer(resultLayer)) { resultLayer.addTo(map); }
        });
    }
}

// Get base url.
function GetBaseUrl() {
    return L.Util.getRootPath() + 'QueryTools/' + selectedQuery + '/{z}/{x}/{y}';
}