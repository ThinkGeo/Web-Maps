var searchPoint, trackMode = 'plot';
var defaultSetting = {
    lng: -10776838.079653593,
    lat: 3912346.5047561876,
    category: 'Restaurants',
    subCategory: 'All',
    searchMode: 'serviceArea',
    driveTime: 5
};

$(document).ready(function () {
    initializePageElements();

    // Initialize the POI categories and its sub-type categories 
    Map1.ajaxCallAction('Default', 'GetCategories', {}, function (response) {
        var categoies = JSON.parse(response.get_responseData());
        $.each(categoies, function (key) {
            $('#sltCategory').append($("<option/>", {
                value: key,
                text: key
            }));
        });
        $("#sltCategory").change(function () {
            var selectValue = $("#sltCategory").val();
            var selectedCategories;
            if (categoies.hasOwnProperty(selectValue)) {
                selectedCategories = categoies[selectValue];
            }
            $('#sltCategorySubtype').empty();
            for (var i = 0; i < selectedCategories.length; i++) {
                $('#sltCategorySubtype').append($("<option/>", {
                    value: selectedCategories[i],
                    text: selectedCategories[i]
                }));
            }
        });
        var setting = defaultSetting;
        searchPoint = { lat: setting.lat, lng: setting.lng };

        $("#sltCategory").val(setting.category).trigger('change');
        $("#sltCategorySubtype").val(setting.subCategory);
        $("#sltSearchMode").val(setting.searchMode).trigger('change');
        if (setting.driveTime) {
            $("#tbxServiceArea").val(setting.driveTime);
        }
        if (setting.bufferDistance) {
            $("#tbxServiceArea").val(setting.bufferDistance);
        }
        if (setting.sltDistanceUnit) {
            $("#tbxServiceArea").val(setting.distanceUnit);
        }
        // searching the default point.
        Map1.ajaxCallAction("Default", "SearchSimilarSites", getApplyButtonParameters(), searchRequested);
    });
});

// A binding client method: Define the method to collect the parameters when plotting on the map.
function getPlotSearchParameters(e) {
    var args = getSearchConditionParameters();
    args.searchPoint = searchPoint.lng + "," + searchPoint.lat;
    return args;
}

// A binding client method: Define the method to collect the parameters when clicking the "Apply" button or the setting changes.
function getApplyButtonParameters() {
    var args = getSearchConditionParameters();
    if (searchPoint) {
        args.searchPoint = searchPoint.lng + "," + searchPoint.lat;
    }
    return args;
}

// Collect the setting info.
function getSearchConditionParameters() {
    var category = $("#sltCategory").val();;
    var subCategory = $('#sltCategorySubtype').val().replace(">=", ">~");
    var searchMode = $("input[name='serviceArea']:checked").val();

    var args = { "category": category, "subCategory": subCategory, "searchMode": searchMode };
    if (searchMode == "ServiceArea") {
        args["driveTime"] = $("#tbxServiceArea").val();
    } else {
        args["distanceBuffer"] = $("#tbxDistance").val();
        args["distanceUnit"] = $("#sltDistanceUnit").val();
    }
    return args;
}

// A binding client event method: check if the request will be triggered.
function searchRequesting(e) {
    if (trackMode == "plot") {
        searchPoint = { lng: e.worldXY.lon, lat: e.worldXY.lat };
    }
    else {
        e.cancel = true;
    }
}

// A binding client event method: handle the search response and redraw the layers.
function searchRequested(response) {
    $("#result-table tr:gt(0)").remove();

    var data = JSON.parse(response.get_responseData());
    if (data.status == 2) {
        $("#dlgErrorMessage").html("Please note that this sample map is only able to analyze service areas within the Frisco, TX city limits, <br\>which are indicated by a dashed red line. Click inside that boundary for best results. ");
        $(".ui-dialog-titlebar").hide();
        $("#dlgErrorMessage").dialog("open");
    }
    if (data.status == 1) {
        $("#dlgErrorMessage").text("No results found.");
        $(".ui-dialog-titlebar").hide();
        $("#dlgErrorMessage").dialog("open");
    }
    if (data.status == 0) {
        // Binding table data on the left panel.
        bindSearchTableResult(data.features);
        // redraw the server side layer.
        Map1.getLayer("QueriedOverlay").redraw(true);
        Map1.getLayer("DrawnPointOverlay").redraw(true);
        Map1.getMarkerOverlay().redraw(true);
    }
}

// A binding client event method: redraw the layers.
function clearRequested(response) {
    Map1.getLayer("QueriedOverlay").redraw(true);
    Map1.getLayer("DrawnPointOverlay").redraw(true);
    Map1.getMarkerOverlay().redraw(true);
}

function zoomToPoi(lat, lon) {
    var lonlat = new OpenLayers.LonLat(lon, lat);
    Map1.setCenter(lonlat, 16);
}

// Functions for displaying query result to grid table.
function bindSearchTableResult(queryItems) {
    // Remove all the lines except the header
    $("#resultTable tr:gt(0)").remove();
    // Display data in the Grid table
    for (var i = 0; i < queryItems.length; i++) {
        var name = queryItems[i].name;
        var position = queryItems[i].point;
        var newRow = $("<tr>");
        newRow.append('<td><a style="width:14px;display:block;"><img alt="Search" title="Zoom to sepcified POI" src="../Content/Images/find.png" onclick="zoomToPoi(' + position.split(',')[0] + ',' + position.split(',')[1] + ')" /></a></td>');
        newRow.append("<td>" + name + "</td>");
        $("#resultTable > tbody:last").append(newRow);
    }
}

var OnMapCreated = function (map) {
    map.events.register("mousemove", map, function (e) {
        var mouseCoordinate = this.getLonLatFromPixel(new OpenLayers.Pixel(e.clientX, e.clientY));
        if (mouseCoordinate) {
            $("#spanMouseCoordinate").text("X:" + mouseCoordinate.lon.toFixed(2) + "  Y: " + mouseCoordinate.lat.toFixed(2));
        }
    });
}

function initializePageElements() {
    var resizeElementHeight = function () {
        var documentheight = $(window).height();
        var contentDivH = documentheight - $("#footer").height() - $("#header").height() - 1;
        $("#content-container").height(contentDivH + "px");

        $("#leftContainer").height(contentDivH + "px");
        $("#map-content").height(contentDivH + "px");
        $("#toggle").css("line-height", contentDivH + "px");

        var mapDivH = contentDivH - $("#resultContainer").height();
        $("#mapContainer").height(mapDivH + "px");

        // refresh the map.
        Map1.updateSize();
    }
    window.onload = resizeElementHeight();
    $(window).resize(resizeElementHeight);

    // Function for plotting a point on the map and do a query based on the clicked point.
    $(Map1.getOpenLayersMap().getViewport()).css("cursor", "crosshair");

    // Bind toggle button events
    $("#toggle img").bind("click", function () {
        if ($("#leftContainer").is(':visible')) {
            $(this).attr("src", "Content/Images/expand.gif");
            $("#map-content").css("width", "100%");
            $("#toggle").css("left", "0%");
            $("#leftContainer").hide();
        }
        else {
            $("#leftContainer").show();
            $(this).attr("src", "Content/Images/collapse.gif")
            $("#map-content").css("width", "80%");
            $("#toggle").css("left", "20%");
        }
        resizeElementHeight();
    });

    // set the toggle style for group buttons
    $("#divTrackMode img").bind("click", function () {
        var btnImgs = $("#divTrackMode img");
        for (var i = 0; i < btnImgs.length; i++) {
            $(btnImgs[i]).attr("class", "imgButton");
        }
        $(this).attr("class", "active imgButton");
        if ($(this).attr("id") == "btnPan") {
            trackMode = "pan";
            $(Map1.getOpenLayersMap().getViewport()).css("cursor", "pointer");
        }
        if ($(this).attr("id") == "btnPlot") {
            trackMode = "plot";
            $(Map1.getOpenLayersMap().getViewport()).css("cursor", "crosshair");
        }
    });

    // set the toggle effect for 2 service mode radio button
    $("input[name='serviceArea']").change(function () {
        if ($("#divService").is(':visible')) {
            $("#divService").hide();
            $("#divBuffer").show();
        } else {
            $("#divService").show();
            $("#divBuffer").hide();
        }
    });

    $("#dlgErrorMessage").dialog({
        autoOpen: false,
        modal: true,
        buttons: {
            Ok: function () {
                $(this).dialog("close");
            }
        }
    });
};
