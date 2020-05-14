$(document).ready(function () {
    initializePageElements();

    $("#btnPanMap").bind("click", function () {
        $("#divTrackShapes input[type=image]").not($(this)).removeClass("active");
    });

    $("#btnClearAll").bind("click", function () {
        $("#divTrackShapes input[type=image]").not($("#btnPanMap")).removeClass("active");
        $("#btnPanMap").addClass("active");

        Map1.setDrawMode("Normal");
        Map1.getEditOverlay().removeAllFeatures();
    });

    // Configuration slider
    var filterEarthquakePoints = function () {
        var queryItems = [];
        var magnitude = { "name": "MAGNITUDE", "min": $("#sliderFortxbMagnitude").slider("values", 0), "max": $("#sliderFortxbMagnitude").slider("values", 1) };
        queryItems.push(magnitude);

        var depth = { "name": "DEPTH_KM", "min": $("#sliderFortxbDepth").slider("values", 0), "max": $("#sliderFortxbDepth").slider("values", 1) };
        queryItems.push(depth);

        var year = { "name": "YEAR", "min": $("#sliderFortxbYear").slider("values", 0), "max": $("#sliderFortxbYear").slider("values", 1) };
        queryItems.push(year);

        $("#loading").show();
        Map1.ajaxCallAction("default", "GetQueryingFeatures", { "configs": JSON.stringify(queryItems) }, displayQueryResult);
    }

    var displaySlideValue = function (element, ui) {
        var valueSpans = $(element).parent().parent().find("span");
        $(valueSpans[0]).text(ui.values[0]);
        $(valueSpans[1]).text(ui.values[1]);
    }
    $("#sliderFortxbMagnitude").slider({
        range: true,
        min: 0,
        max: 12,
        values: [0, 12],
        stop: filterEarthquakePoints,
        slide: function (event, ui) {
            displaySlideValue(this, ui);
        }
    });
    $("#sliderFortxbDepth").slider({
        range: true,
        min: 0,
        max: 300,
        values: [0, 300],
        stop: filterEarthquakePoints,
        slide: function (event, ui) {
            displaySlideValue(this, ui);
        }
    });
    $("#sliderFortxbYear").slider({
        range: true,
        min: 1568,
        max: 2012,
        values: [1568, 2012],
        stop: filterEarthquakePoints,
        slide: function (event, ui) {
            displaySlideValue(this, ui);
        }
    });
});

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

    // set the toggle style for group buttons
    $("#divTrackShapes input[type=image]").bind("click", function () {
        var btnImgs = $("#divTrackShapes input[type=image]");
        for (var i = 0; i < btnImgs.length; i++) {
            $(btnImgs[i]).attr("class", "");
        }
        $(this).attr("class", "active");
    });

    // Bind toggle button events
    $("#toggle img").bind("click", function () {
        if ($("#leftContainer").is(':visible')) {
            $("#map-content").css("width", "99%");
            $("#resultContainer").css("width", "99%");
            $("#toggle").css("left", "5px");
            $("#leftContainer").hide();
            $("#collapse").attr("src", "Images/expand.gif");
        }
        else {
            $("#leftContainer").show();
            $("#map-content").css("width", "80%");
            $("#resultContainer").css("width", "80%");
            $("#toggle").css("left", "20%");
            $("#collapse").attr("src", "Images/collapse.gif");
        }
        resizeElementHeight();
    });
}

var OnMapCreated = function (map) {
    map.events.register("mousemove", map, function (e) {
        var mouseCoordinate = this.getLonLatFromPixel(new OpenLayers.Pixel(e.clientX, e.clientY));
        if (mouseCoordinate) {
            $("#spanMouseCoordinate").text("X:" + mouseCoordinate.lon.toFixed(2) + "  Y: " + mouseCoordinate.lat.toFixed(2));
        }
    });
}

function getChangeLayerTypeArguments(e) {
    return { command: $(e.srcElement).attr("command") };
}

function changeLayerTypeCallback(response) {
    Map1.getAdornmentOverlay().redraw(true);
}

function clearLayerCallback(response) {
        Map1.getLayer('TrackShapeOverlay').redraw(true);
        Map1.getLayer('QueryResultMarkerOverlay').redraw(true);
        $("#resultTable tr:gt(0)").remove();
}

function displayQueryResult(result) {
    // Refresh the corresponding Layer
    Map1.getLayer('QueryResultMarkerOverlay').redraw(true);

    // remove all the lines except the header
    $("#resultTable tr:gt(0)").remove();
    // Display data in the Grid table
    var featuresJsonStr = JSON.parse(result.get_responseData()).features;
    var queryItems = JSON.parse(featuresJsonStr);
    for (var i = 0; i < queryItems.length; i++) {
        var columns = queryItems[i].values;
        var featureId = queryItems[i].id;
        var newRow = $("<tr>");
        newRow.append('<td><input type="image"  id="' + featureId + '" src="/Content/Images/find.png" onclick="zoomToFeature(this)" /></td>');
        newRow.append("<td>" + columns[0].Value + "</td>");
        newRow.append("<td>" + columns[2].Value + "</td>");
        newRow.append("<td>" + columns[3].Value + "</td>");
        newRow.append("<td>" + (columns[4].Value < 0 ? "Unknown" : columns[4].Value) + "</td>");
        newRow.append("<td>" + (columns[5].Value < 0 ? "Unknown" : columns[5].Value) + "</td>");
        newRow.append("<td>" + columns[1].Value + "</td>");
        $("#resultTable > tbody:last").append(newRow);
    }

    $("#loading").hide();
}

function zoomToFeature(element) {
    Map1.ajaxCallAction("default", "ZoomToFeature", { featureId: $(element).attr("id") }, function (result) {
        if (result.get_responseData()) {
            var lonlat = JSON.parse(result.get_responseData());
            Map1.setCenter(new OpenLayers.LonLat(lonlat.x, lonlat.y), 16);
        }
    });
}

function trackShapeFinished(e) {
    var features = JSON.parse(e.features);
    var wkts = [];

    for (var i = 0; i < features.length; i++) {
        wkts.push(features[i].wkt);
    }

    $("#loading").show();
    Map1.ajaxCallAction("default", "GetTrackFeatures", wkts, displayQueryResult);
}

// Override to support showing the drawing radius of circle
var OnMapCreating = function (map) {
    var msDrawCircleLineId = "";
    var msDrawCircleLabelId = "";
    OpenLayers.Handler.RegularPolygon.prototype.move = function (evt) {
        var maploc = this.map.getLonLatFromPixel(evt.xy);
        var point = new OpenLayers.Geometry.Point(maploc.lon, maploc.lat);
        if (this.irregular) {
            var ry = Math.sqrt(2) * Math.abs(point.y - this.origin.y) / 2;
            this.radius = Math.max(this.map.getResolution() / 2, ry);
        } else if (this.fixedRadius) {
            this.origin = point;
        } else {
            this.calculateAngle(point, evt);
            this.radius = Math.max(this.map.getResolution() / 2,
                                   point.distanceTo(this.origin));
        }
        this.modifyGeometry();
        if (this.irregular) {
            var dx = point.x - this.origin.x;
            var dy = point.y - this.origin.y;
            var ratio;
            if (dy == 0) {
                ratio = dx / (this.radius * Math.sqrt(2));
            } else {
                ratio = dx / dy;
            }
            this.feature.geometry.resize(1, this.origin, ratio);
            this.feature.geometry.move(dx / 2, dy / 2);
        }
        this.layer.drawFeature(this.feature, this.style);
        // if it's circle, added the drawing distance and radius
        if (!this.irregular) {
            var pointArray = [];
            pointArray.push(this.origin);
            pointArray.push(point);
            if (msDrawCircleLineId != "" && this.layer.getFeatureById(msDrawCircleLineId)) {
                this.layer.removeFeatures([this.layer.getFeatureById(msDrawCircleLineId)]);
            }
            if (msDrawCircleLabelId != "" && this.layer.getFeatureById(msDrawCircleLabelId)) {
                this.layer.removeFeatures([this.layer.getFeatureById(msDrawCircleLabelId)]);
            }
            var radiusLine = new OpenLayers.Feature.Vector(new OpenLayers.Geometry.LineString(pointArray), null, this.style);
            msDrawCircleLineId = radiusLine.id;
            this.layer.addFeatures([radiusLine]);
            var radiusLabelText = "";
            var radiusLength = radiusLine.geometry.getGeodesicLength(this.layer.map.getProjectionObject());
            var inPerDisplayUnit = OpenLayers.INCHES_PER_UNIT["mi"];
            if (inPerDisplayUnit) {
                var inPerMapUnit = OpenLayers.INCHES_PER_UNIT["m"];
                radiusLength *= (inPerMapUnit / inPerDisplayUnit);
            }
            radiusLabelText = parseFloat(radiusLength).toFixed(4).toString() + 'mi';
            point.distanceTo(this.origin).toString()
            var radiusLabelFeature = new OpenLayers.Feature.Vector(new OpenLayers.Geometry.Point(point.x + 0.1 * this.layer.map.getExtent().getHeight(), point.y - 0.1 * this.layer.map.getExtent().getHeight()), {}, { label: radiusLabelText });
            msDrawCircleLabelId = radiusLabelFeature.id;
            this.layer.addFeatures([radiusLabelFeature]);
        }
    }
}

