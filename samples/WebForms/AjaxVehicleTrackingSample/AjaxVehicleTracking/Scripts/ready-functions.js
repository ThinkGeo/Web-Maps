$(document).ready(function () {
    // Initialize the dom elements in the page
    initializePageElements();
    // attach event to the buttons for track shapes
    attachEventsToTrackButtons();
    // init vihecles in the first time.
    refreshVehicle();
    // auto-refresh button
    var autoRefreshTimer = setInterval(autoRefresh, 1000);
    $('#btnAutoRefresh').on('click', function () {
        // attach animation to auto-refresh button
        if ($(this).attr("src") == "Images/AutoRefresh.png") {
            $("#autoRefreshStatus").text("On");
            // let server start reading history
            autoRefreshTimer = setInterval(autoRefresh, 1000);
        } else if (autoRefreshTimer != null) {
            $("#autoRefreshStatus").text("Off");

            $("#btnAutoRefresh").attr("src", "Images/AutoRefresh.png")
            clearInterval(autoRefreshTimer);
            autoRefreshTimer = null;
        }
    });

    // Refresh manually
    $("#btnRefresh").bind("click", function (e) {
        refreshVehicle();
    });

    // Measure Buttons
    $("#divMeasure Img").bind("click", function (e) {
        Map1.SetDrawMode('Normal');
        var command = $(this).attr("alt");
        switch (command) {
            case "Distance":
                Map1.SetMeasureMode('PathMeasure');
                break;
            case "Area":
                Map1.SetMeasureMode('PolygonMeasure');
                break;
            case "Cancel":
                Map1.SetMeasureMode('Normal');
                break;
            default:
                break;
        }
    });
    $("#divMeasure Select").bind("change", function () {
        var displaySystem = $(this).val();
        var measureTools = MapHelper.getMeasureTools();
        for (var i = 0; i < measureTools.length; i++) {
            switch (displaySystem) {
                case "Metric":
                    measureTools[i].displaySystem = "metric";
                    break;
                case "Imperial":
                    measureTools[i].displaySystem = "english";
                    break;
                default:
                    break;
            }
        }
        var args = { "request": CallbackTypes.changeDisplaySystem, "displaySystem": displaySystem };
        $DoCallback(JSON.stringify(args), function (e, context) {
            MapHelper.getAdormentOverlay().redraw(true);
        });
    })
});

function autoRefresh() {
    if ($("#btnAutoRefresh").attr("src") == "Images/AutoRefresh_2.png") {
        $("#btnAutoRefresh").attr("src", "Images/AutoRefresh_1.png")
    } else {
        $("#btnAutoRefresh").attr("src", "Images/AutoRefresh_2.png")
    }
    // refresh vechiles
    refreshVehicle();
};

function refreshVehicle() {
    var args = { "request": CallbackTypes.refreshVihecle };
    $DoCallback(JSON.stringify(args), function (e, context) {
        var jsonVihecles = JSON.parse(e);
        // refresh vihecle panel.
        $("#divVehicleList").empty();
        $("#divVehicleList").html(new vehiclesPanel(jsonVihecles).generateTable());
        // refresh vihecle marker layers.
        var vehicleOverlays = MapHelper.getVehicleOverlays();
        for (var i = 0; i < vehicleOverlays.length; i++) {
            vehicleOverlays[i].redraw(true);
        }

        // zoom to vehicle
        $("#divVehicleList a").bind("click", function (e) {
            var lonlatVal = $(this).parent().find("#lonlat").val().split(',');
            if (lonlatVal.length >= 2) {
                var lonlat = new OpenLayers.LonLat(parseFloat(lonlatVal[0]), parseFloat(lonlatVal[1]));
                Map1.GetOpenLayersMap().zoomToScale(4513.988880);
                Map1.GetOpenLayersMap().panTo(lonlat);
            }
        })
    });
}

var OnMapCreated = function (map) {
    map.events.register("mousemove", map, function (e) {
        var mouseCoordinate = this.getLonLatFromPixel(new OpenLayers.Pixel(e.clientX, e.clientY));
        if (mouseCoordinate) {
            $("#spanMouseCoordinate").text("X:" + mouseCoordinate.lon.toFixed(2) + "  Y: " + mouseCoordinate.lat.toFixed(2));
        }
    });

    // A workaround here to make sure the marker always have a higher zIndex than editOverlay
    var markerOverlays = MapHelper.getVehicleOverlays();
    for (var i = 0; i < markerOverlays.length; i++) {
        markerOverlays[i].setZIndex(750 + i); // 750 is the default zIndex of popup
    }
}

var MapHelper = {
    getEditOverlay: function () {
        var editOverlay = Map1.GetOpenLayersMap().getLayersByName("EditOverlay");
        if (editOverlay.length == 0) {
            Map1.GetMapParser().initDrawControls();
        }
        return Map1.GetOpenLayersMap().getLayersByName("EditOverlay")[0];
    },
    getFenceOverlay: function () {
        var map = Map1.GetOpenLayersMap();
        return map.getLayer("SpatialFenceOverlay");
    },
    getMeasureTools: function () {
        var map = Map1.GetOpenLayersMap();
        return map.getControlsByClass("OpenLayers.Control.Measure");
    },
    getAdormentOverlay: function () {
        var adornmentOverlay = Map1.GetOpenLayersMap().getLayersByName("AdornmentOverlay");
        return adornmentOverlay[0];
    },
    getVehicleOverlays: function () {
        var map = Map1.GetOpenLayersMap();
        return Map1.GetOpenLayersMap().getLayersByClass("OpenLayers.Layer.Markers");
    }
}

function attachEventsToTrackButtons() {
    // Process track mode buttons
    $("#divTrackType Img").bind("click", function (e) {
        var command = $(this).attr("alt");
        switch (command) {
            case "Draw":
                // Back to draw polygon mode and cacel edit geometries
                Map1.SetDrawMode('Polygon');
                // clear all editing fences if it's in editing mode before
                var fenceOverlay = MapHelper.getFenceOverlay();
                if (!fenceOverlay.getVisibility()) {
                    var editOverlay = MapHelper.getEditOverlay();
                    editOverlay.destroyFeatures(editOverlay.features);

                    fenceOverlay.setVisibility(true);
                }
                break;
            case "Edit":
                Map1.SetDrawMode('Modify');
                if (MapHelper.getFenceOverlay().getVisibility() || MapHelper.getEditOverlay().features.length <= 0) {
                    editFences();
                }
                break;
            case "Remove":
                var parser = Map1.GetMapParser();
                if (parser.editOverlay == null || parser.editOverlay.selectedFeatures.length == 0) {
                    // Show no fence selected dialog
                    $("#divMessage").dialog("open");
                } else {
                    $("#divRemoveDialog").dialog("open");
                }
                $(".ui-dialog-titlebar").hide();    // Hide dialog header bar
                break;
            case "Save":
                if (!MapHelper.getFenceOverlay().getVisibility() || MapHelper.getEditOverlay().features.length > 0) {
                    saveFences();
                }
                break;
            case "Cancel":
                $("#btnDrawPolygon").trigger("click");
                break;
            default:
                break;
        }
    })

    // Functions related map
    var editFences = function () {
        var args = { "request": CallbackTypes.editFences };
        $DoCallback(JSON.stringify(args), function (e, context) {
            if (e) {
                var jsonFeatures = $.parseJSON(e);
                var editingFeatures = new Array();
                for (var i = 0; i < jsonFeatures.length; i++) {
                    var spatialFence = new OpenLayers.Feature.Vector(OpenLayers.Geometry.fromWKT(jsonFeatures[i].wkt));
                    spatialFence.fid = jsonFeatures[i].id;

                    editingFeatures.push(spatialFence);
                }

                MapHelper.getEditOverlay().addFeatures(editingFeatures);
                MapHelper.getFenceOverlay().setVisibility(false);
            }
        }, this)
    }
    var saveFences = function () {
        var editOverlay = MapHelper.getEditOverlay();
        var features = new Array();
        for (var i = 0; i < editOverlay.features.length; i++) {
            var geometry = editOverlay.features[i].geometry;
            if (geometry.CLASS_NAME != "OpenLayers.Geometry.Point") {
                var feature = { id: editOverlay.features[i].fid, wkt: geometry.toString() };
                features.push(feature);
            }
        }

        var args = { "trackMode": $("#mapTrackMode").val(), "request": CallbackTypes.saveFences, "features": features };
        $DoCallback(JSON.stringify(args), function (e, context) {
            if (e == "True") {
                var editOverlay = MapHelper.getEditOverlay();
                editOverlay.destroyFeatures(editOverlay.features);

                var fenceOverlay = MapHelper.getFenceOverlay();
                fenceOverlay.setVisibility(true);
                fenceOverlay.redraw(true);
                $("#btnDrawPolygon").trigger("click");
            } else {
                $("#divMessage").html("Failed to save the edit spatial fences.");
                $("#divMessage").dialog("open");
            }
        }, this)
    }

    // Init dialog for tracking shapes
    $("#divRemoveDialog").dialog({
        autoOpen: false,
        modal: true,
        buttons: {
            Yes: function () {
                var parser = Map1.GetMapParser();
                if (parser.editOverlay && parser.editOverlay.selectedFeatures.length > 0) {
                    var selectedFeatures = [];
                    for (var i = 0; i < parser.editOverlay.selectedFeatures.length; i++) {
                        for (var j = 0; j < parser.editOverlay.features.length; j++) {
                            if (parser.editOverlay.features[j].id == parser.editOverlay.selectedFeatures[i].id) {
                                selectedFeatures.push(parser.editOverlay.features[j]);
                            }
                        }
                    }
                    if (parser.paintControls['Modify'].active) {
                        parser.paintControls['Modify'].deactivate();
                    }
                    parser.editOverlay.removeFeatures(selectedFeatures);
                    // Synchronize the edited features to server side
                    saveFences();

                    $(this).dialog("close");
                }
            },
            No: function () {
                // back to previous status
                if (MapHelper.getFenceOverlay().getVisibility()) {
                    // Back to draw edit mode
                    $("#btnDrawPolygon").trigger("click");
                } else {
                    // Back to draw polygon mode
                    $("#btnEditPolygon").trigger("click");
                }
                $(this).dialog("close");
            }
        }
    });
    $("#divMessage").dialog({
        autoOpen: false,
        modal: true,
        buttons: {
            ok: function () {
                // back to previous status
                if (MapHelper.getFenceOverlay().getVisibility()) {
                    // Back to draw edit mode
                    $("#btnDrawPolygon").trigger("click");
                } else {
                    // Back to draw polygon mode
                    $("#btnEditPolygon").trigger("click");
                }
                $(this).dialog("close");
            }
        }
    });
}

function initializePageElements() {
    // Resize map after browser if resized
    var resizeElementHeight = function resizeElementHeight() {
        var documentheight = $(window).height();
        var contentDivH = documentheight - $("#footer").height() - $("#header").height() - 1;
        $("#content-container").height(contentDivH + "px");

        $("#leftContainer").height(contentDivH + "px");
        $("#map-content").height(contentDivH + "px");
        $("#toggle").css("line-height", contentDivH + "px");

        if (contentDivH > 300) {
            $("#divVehicleList").height(contentDivH - 300 + "px");
        } else {
            $("#divVehicleList").visible = false;
        }

        // refresh the map.
        Map1.GetOpenLayersMap().updateSize();
    }
    window.onload = resizeElementHeight();
    $(window).resize(resizeElementHeight);

    // set the toggle style for group buttons
    var resetToggleButtons = function (containerId) {
        var btnImgs = $(containerId).find("img");
        for (var i = 0; i < btnImgs.length; i++) {
            $(btnImgs[i]).bind("click", function () {
                var btnImgs = $(containerId).find("img");
                for (var i = 0; i < btnImgs.length; i++) {
                    $(btnImgs[i]).attr("class", "imgButton");
                }
                $(this).attr("class", "active imgButton");
            })
        };
    };
    resetToggleButtons("#divTrackMode");
    resetToggleButtons("#divTrackType");
    resetToggleButtons("#divMeasure");

    // highlight all img buttons when mouse-over
    var highlightImg = function (imgContainerIds) {
        for (var i = 0; i < imgContainerIds.length; i++) {
            var btnImgs = $(imgContainerIds[i]).find("img");
            for (var j = 0; j < btnImgs.length; j++) {
                $(btnImgs[j]).bind("mouseover", function () {
                    $(this).css("opacity", "0.4");
                });
                $(btnImgs[j]).bind("mouseout", function () {
                    $(this).css("opacity", "1");
                });
            }
        }
    }
    highlightImg(["#divRefresh", "#divTrackMode", "#divTrackType", "#divMeasure"]);

    // attach toggle events to track buttons
    $("#divTrackMode img").bind("click", function (e) {
        switch ($(this).attr("alt")) {
            case "Pan":
                $("#divTrackType").hide();
                $("#divMeasure").hide();
                Map1.SetDrawMode('Normal');
                Map1.SetMeasureMode('Normal');
                break;
            case "Draw":
                $("#divTrackType").show();
                $("#divMeasure").hide();
                Map1.SetDrawMode('Polygon');
                Map1.SetMeasureMode('Normal');
                break;
            case "Measure":
                $("#divTrackType").hide();
                $("#divMeasure").show();
                Map1.SetDrawMode('Normal');
                Map1.SetMeasureMode('PathMeasure');
                break;
            default: break;
        }
    })

    // hide track types button and measure type buttons
    $("#divTrackType").hide();
    $("#divMeasure").hide();

    // Bind toggle button events
    $("#toggle").bind("click", function () {
        if ($("#leftContainer").is(':visible')) {
            $("#collapse").attr("src", "Images/expand.gif");
            $("#map-content").css("width", "99%");
            $("#toggle").css("left", "5px");
            $("#leftContainer").hide();
        }
        else {
            $("#leftContainer").show();
            $("#collapse").attr("src", "Images/collapse.gif");
            $("#map-content").css("width", "80%");
            $("#toggle").css("left", "20%");
        }
        resizeElementHeight();
    });
}

var CallbackTypes = {
    editFences: "editFences",
    saveFences: "saveFences",
    refreshVihecle: "refreshVehicles"
};

function $DoCallback(arg, callbackEvent, context) {
    WebForm_DoCallback("__Page", arg, callbackEvent, context, null, false);
}