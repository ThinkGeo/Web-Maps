$(document).ready(function () {
    window.onload = resizeElementHeight;
    $(window).resize(resizeElementHeight);

    Map1.events.register("mousemove", Map1, function (e) {
        var mouseCoordinate = this.getLonLatFromPixel(new OpenLayers.Pixel(e.clientX, e.clientY));
        $("#spanMouseCoordinate").text("X:" + mouseCoordinate.lon.toFixed(5) + "  Y: " + mouseCoordinate.lat.toFixed(5));
    });

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
    resetToggleButtons("#divEditPanel");
    resetToggleButtons("#divMeasure");

    // attach toggle events to track buttons
    $("#divTrackMode img").bind("click", function (e) {
        switch ($(this).attr("command")) {
            case "Pan":
                $("#divEditPanel").hide();
                $("#divMeasure").hide();
                Map1.setDrawMode('Normal');
                Map1.setMeasureMode('Normal');
                break;
            case "Draw":
                $("#divEditPanel").show();
                $("#divMeasure").hide();
                Map1.setDrawMode('Polygon');
                Map1.setMeasureMode('Normal');
                break;
            case "Measure":
                $("#divEditPanel").hide();
                $("#divMeasure").show();
                Map1.setDrawMode('Normal');
                Map1.setMeasureMode('PathMeasure');
                break;
            default: break;
        }
    })

    // hide track types button and measure type buttons
    $("#divEditPanel").hide();
    $("#divMeasure").hide();

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

    $('#btnRefresh').on('click', function () {
        refreshVehicles();
    });


    // Process track mode buttons
    $("#divEditPanel Img").bind("click", function (e) {
        var command = $(this).attr("command");
        switch (command) {
            case "Polygon":
                // Back to draw polygon mode and cacel edit geometries
                Map1.setDrawMode('Polygon');
                // clear all editing fences if it's in editing mode before
                var fenceOverlay = MapHelper.getFenceOverlay();
                if (!fenceOverlay.getVisibility()) {
                    var editOverlay = MapHelper.getEditOverlay();
                    editOverlay.destroyFeatures(editOverlay.features);

                    fenceOverlay.setVisibility(true);
                }
                break;
            case "RemovePolygon":
                var parser = Map1.getMapParser();
                if (parser.editOverlay == null || parser.editOverlay.selectedFeatures.length == 0) {
                    // Show no fence selected dialog
                    $("#divMessage").dialog("open");
                } else {
                    $("#divRemoveDialog").dialog("open");
                }
                $(".ui-dialog-titlebar").hide();    // Hide dialog header bar
                break;
            case "Clear":
                Map1.clearEditingFeatures();
                $("#btnDrawPolygon").trigger("click");
                break;
            default:
                break;
        }
    })

    // Measure Buttons
    $("#divMeasure Img").bind("click", function (e) {
        Map1.setDrawMode('Normal');
        var command = $(this).attr("command");
        switch (command) {
            case "Distance":
                Map1.setMeasureMode('PathMeasure');
                break;
            case "Area":
                Map1.setMeasureMode('PolygonMeasure');
                break;
            case "Cancel":
                Map1.setMeasureMode('Normal');
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
    })

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

    // Init dialog for tracking shapes
    $("#divRemoveDialog").dialog({
        autoOpen: false,
        modal: true,
        buttons: {
            Yes: function () {
                var parser = Map1.getMapParser();
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
                    Map1.setDrawMode('Polygon');
                    Map1.setMeasureMode('Normal');
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
});

var resizeElementHeight = function resizeElementHeight() {
    var documentheight = $(window).height();
    var contentDivH = documentheight - $("#footer").height() - $("#header").height() - 1;
    $("#content-container").height(contentDivH + "px");

    $("#leftContainer").height(contentDivH + "px");
    $("#map-content").height(contentDivH + "px");
    $("#toggle").css("line-height", contentDivH + "px");

    if (contentDivH > 400) {
        $("#divVehicleList").height(contentDivH - 400 + "px");
    } else {
        $("#divVehicleList").visible = false;
    }

    // refresh the map.
    Map1.updateSize();
}

var MapHelper = {
    getEditOverlay: function () {
        var editOverlay = Map1.getLayersByName("EditOverlay");
        if (editOverlay.length == 0) {
            Map1.initDrawControls();
        }
        return Map1.getLayer("EditOverlay");
    },
    getFenceOverlay: function () {
        return Map1.getLayer("SpatialFenceOverlay");
    },
    getMeasureTools: function () {
        return Map1.getControlsByClass("OpenLayers.Control.Measure");
    },
    getAdormentOverlay: function () {
        return Map1.getLayer("AdornmentOverlay");
    },
    getVehicleOverlay: function () {
        return Map1.getLayer("VehicleOverlay");
    },
    getVehicleHistoryOverlay: function () {
        return Map1.getLayer("VehicleHistoryOverlay");
    }
}

function autoRefresh() {
    if ($("#btnAutoRefresh").attr("src") == "Images/AutoRefresh_2.png") {
        $("#btnAutoRefresh").attr("src", "Images/AutoRefresh_1.png")
    } else {
        $("#btnAutoRefresh").attr("src", "Images/AutoRefresh_2.png")
    }
    // refresh vechiles
    refreshVehicles();
};

function refreshVehicles() {
    Map1.ajaxCallAction("Default", "DisplayVehicles", null, function (result) {
        $('#divVehicleList').empty().html(result.get_responseData());
        Map1.getLayer("VehicleOverlay").redraw(true);
        Map1.getLayer("VehicleHistoryOverlay").redraw(true);

        // zoom to vehicle
        $("#tbVehiclelist a").bind("click", function (e) {
            var lonlatVal = $(this).parent().find("#lonlat").val().split(',');
            if (lonlatVal.length >= 2) {
                var lonlat = new OpenLayers.LonLat(parseFloat(lonlatVal[0]), parseFloat(lonlatVal[1]));
                Map1.zoomToScale(4513.988880);
                Map1.panTo(lonlat);
            }
        })
    });
}

function onEditFencesRequesting(e) {
    if (MapHelper.getFenceOverlay().getVisibility() || MapHelper.getEditOverlay().features.length <= 0) {
        e.cancel = false;
    }
    else {
        e.cancel = true;
    }
}

function onEditFencesRequested(e) {
    var jsonFeatures = JSON.parse(e.get_responseData());
    if (jsonFeatures) {
        var editingFeatures = new Array();
        for (var i = 0; i < jsonFeatures.length; i++) {
            var spatialFence = new OpenLayers.Feature.Vector(OpenLayers.Geometry.fromWKT(jsonFeatures[i].Wkt));
            spatialFence.fid = jsonFeatures[i].Id;

            editingFeatures.push(spatialFence);
        }

        Map1.setDrawMode('Modify');
        MapHelper.getEditOverlay().addFeatures(editingFeatures);
        MapHelper.getFenceOverlay().setVisibility(false);
    }
}

function getSaveFencesParameters() {
    var editOverlay = MapHelper.getEditOverlay();
    var features = new Array();
    for (var i = 0; i < editOverlay.features.length; i++) {
        var geometry = editOverlay.features[i].geometry;
        if (geometry.CLASS_NAME != "OpenLayers.Geometry.Point") {
            var feature = { id: editOverlay.features[i].fid, wkt: geometry.toString() };
            features.push(feature);
        }
    }

    return { args: JSON.stringify(features) };
}

function onSaveFencesRequested(e) {
    Map1.clearEditingFeatures();
    MapHelper.getFenceOverlay().setVisibility(true);
    var result = JSON.parse(e.get_responseData());
    if (result.status != "True") {
        $("#divMessage").html("Failed to save the edit spatial fences.");
        $("#divMessage").dialog("open");
    }
    else {
        $("#btnDrawPolygon").trigger("click");
    }
}
