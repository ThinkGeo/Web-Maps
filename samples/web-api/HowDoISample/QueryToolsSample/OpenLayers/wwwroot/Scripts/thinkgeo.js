// Initialize default value for some parameters.
var selectedQuery = 'SpatialQuery';
var currentCoordinate = [1115369.1167372921, 1937220.0448595073];
var thinkgeoApiKey = 'PIbGd76RyHKod99KptWTeb-Jg9JUPEPUBFD3SZJYLDE~';

// Create the map.
var map = new ol.Map({
    target: 'map',
	renderer: 'webgl',
    loadTilesWhileAnimating: true,
    loadTilesWhileInteracting: true,
    controls: ol.control.defaults({ attribution: false, rotate: false }).extend(
        [new ol.control.Attribution({
            collapsible: false
        })]),
    view: new ol.View({ center: ol.proj.transform([0, 0], 'EPSG:4326', 'EPSG:3857'), zoom: 3 })
});

// Add image buttons for layers and help.
var imgControls = new app.ImagesControl({
    imgs: [
        {
            id: 'btnLayerOptions',
            src: 'Images/layers.png',
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
            id: 'btnInfo',
            src: 'Images/info.png',
            title: 'Show help',
            callback: function () { window.open('http://wiki.thinkgeo.com/wiki/map_suite_querying', '_blank'); }
        }
    ]
});
map.addControl(imgControls);

// Add ThinkgeoVectorTileLayer as the map's background layer.
map.addLayer(new ol.mapsuite.VectorTileLayer('https://cdn.thinkgeo.com/worldstreets-styles/3.0.0/light.json', {
    apiKey: thinkgeoApiKey,
    layerName: 'light'
}));

// Add query tool's source layer to map. 
var sourceLayer = new ol.layer.Tile({
    source: new ol.source.XYZ({
        url: getRootPath() + 'QueryTools/InitializeSourceLayer/{z}/{x}/{y}',
        maxZoom: 19,
        tileLoadFunction: function (imageTile, src) { imageTile.getImage().src = src + '?t=' + new Date().getTime(); }
    })
});
map.addLayer(sourceLayer);

// Add result layer to display query result features.
var layerSource = new ol.source.XYZ({
    url: GetBaseUrl(),
    maxZoom: 19,
    tileLoadFunction: function (imageTile, src) {
        var format = src.indexOf('?') > 0 ? '&t=' : '?t=';
        imageTile.getImage().src = src + format + new Date().getTime();
    }
});
var resultLayer = new ol.layer.Tile({
    source: layerSource,
    visible: true
});
map.addLayer(resultLayer);

// Trigger the events when changing spatial query type. 
$('#SpatialQueryDescription input[name="SpatialQueryGroup"]').click(function () {
    var queryType = $(this).attr("id");
    map.setView(new ol.View({ center: [0, 0], zoom: 3 }));
    layerSource.setUrl(GetBaseUrl() + '?queryType=' + queryType);    
    layerSource.refresh();    
});

// Trigger the events when changing distance in query features. 
$("#WithinDistanceData").change(function () {
    var distance = $('#WithinDistanceData option:selected').attr("value");
    layerSource.setUrl(GetBaseUrl() + '?distance=' + distance + '&lng=' + currentCoordinate[0] + '&lat=' + currentCoordinate[1]);
});

// Refresh sample by selecting sample item.
$('#leftPanelOptions div').bind('click', function () {
    map.setView(new ol.View({ center: [0, 0], zoom: 3 }));

    selectedQuery = $(this).attr('id');
    RefreshUIBySelectedItem(selectedQuery);
    QueryAndFillData(selectedQuery, currentCoordinate);

    resultLayer.setVisible(false);
    var hasTargetLayer = $(this).attr('hasTargetLayer');
    if (hasTargetLayer) {
        resultLayer.setVisible(true);
        layerSource.setUrl(GetBaseUrl());
        layerSource.refresh(); 
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
    currentCoordinate = e.coordinate;
    var url = GetBaseUrl() + '?lng=' + e.coordinate[0] + '&lat=' + e.coordinate[1];
    if (selectedQuery === "QueryFeaturesWithinDistance") {
        var distance = $('#WithinDistanceData option:selected').attr("value");
        url = url + '&distance=' + distance;
    }
    else if (selectedQuery === "QueryColumnData") {
        QueryAndFillData(selectedQuery, e.coordinate);
    }
    else {
        return;
    }

    layerSource.setUrl(url);
    layerSource.refresh();
});

// Zoom map to new extent by given world point.
function zoomToExtent(worldX, worldY, featureId) {
    map.setView(new ol.View({ center: [worldX, worldY], zoom: 5 }));
    layerSource.setUrl(GetBaseUrl() + '?featureId=' + featureId);
    resultLayer.setVisible(true);
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
    var url = getRootPath() + '/QueryTools/' + selectedQuery + 'DataRequests';

    if (selectedQuery === 'QueryAllFeatures' || selectedQuery === 'QueryColumnData') {
        url = url + '?lng=' + coordinate[0] + '&lat=' + coordinate[1];
        $.get(url, function (res) { $('#' + selectedQuery + 'Tbody').html(res); });
    } else if (selectedQuery === 'SQLQuery') {
        var sqlString = $("#SQLString").val();
        url = url + '?sqlString=' + sqlString;
        $.get(url, function (res) {
            $('#' + selectedQuery + 'Tbody').html(res);
            resultLayer.setVisible(true);
            layerSource.setUrl(GetBaseUrl());
        });
    }
}

// Get base url.
function GetBaseUrl() {
    return getRootPath() + 'QueryTools/' + selectedQuery + '/{z}/{x}/{y}';
}