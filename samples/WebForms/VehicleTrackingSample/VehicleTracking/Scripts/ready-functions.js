var autoRefreshTimer;
$(document).ready(function () {
    initializePageElements();

    // auto-refresh button
    autoRefreshTimer = setInterval(autoRefreshUI, 1000);

    // attach toggle events to track buttons
    $("#divTrackMode input[type='image']").bind("click", function (e) {
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
                Map1.SetMeasureMode('Normal');
                break;
            case "Measure":
                $("#divTrackType").hide();
                $("#divMeasure").show();
                Map1.SetDrawMode('Normal');
                break;
            default: break;
        }
        resetDefaultStatus();
        return false;
    });

    // Remove selected feature on client at first and then synchronize to server
    $("#ibtnDeleteSpatialFences").bind("click", function (e) {
        var parser = Map1.GetMapParser();
        if (parser.editOverlay == null || parser.editOverlay.selectedFeatures.length == 0) {
            showDialog("divMessage", "Please choose a fence at first.");
        } else {
            showDialog("divRemoveDialog", "Are you sure you want to delete the spatial fence you have selected?");
        }
        $(".ui-dialog-titlebar").hide();    // Hide dialog header bar
        return false;
    });

    // Init dialog for tracking shapes
    $("#divRemoveDialog").dialog({
        autoOpen: false,
        modal: true,
        buttons: {
            Yes: function () {
                var parser = Map1.GetMapParser();
                // remove the selected features in client side.
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
                    parser.editOverlay.destroyFeatures(selectedFeatures);
                    $(this).dialog("close");
                    // do a partial postback to save the edited/deleted shapes to server side
                    $("#btnDeleteSpatialFence").trigger("click");
                }
            },
            No: function () {
                $(this).dialog("close");
            }
        }
    });
});

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
    getAdormentOverlay: function () {
        var adornmentOverlay = Map1.GetOpenLayersMap().getLayersByName("AdornmentOverlay");
        return adornmentOverlay[0];
    },
    getVehicleOverlays: function () {
        var map = Map1.GetOpenLayersMap();
        return Map1.GetOpenLayersMap().getLayersByClass("OpenLayers.Layer.Markers");
    }
}

function zoomInToVehicle(lon, lat) {
    Map1.GetOpenLayersMap().moveTo(new OpenLayers.LonLat(lon, lat), 16);
    return false;
}

function changeAutoRefreshButton(element) {
    // attach animation to auto-refresh button
    if ($(element).attr("src") == "Images/autorefresh.png") {
        autoRefreshTimer = setInterval(autoRefreshUI, 1000);
    } else {
        $("#ibtnAutoRefreshButton").attr("src", "Images/autorefresh.png")
        clearInterval(autoRefreshTimer);
        autoRefreshTimer = null;
    }
}

function autoRefreshUI() {
    if ($("#ibtnAutoRefreshButton").attr("src") == "Images/AutoRefresh_2.png") {
        $("#ibtnAutoRefreshButton").attr("src", "Images/AutoRefresh_1.png")
    } else {
        $("#ibtnAutoRefreshButton").attr("src", "Images/AutoRefresh_2.png")
    }
};

function initializePageElements() {

    // apply toggle button effect to a group buttons
    var applyToggleEffectToButtons = function (containerId) {
        var btnImgs = $(containerId).find("input[type='image']");
        for (var i = 0; i < btnImgs.length; i++) {
            $(btnImgs[i]).bind("click", function () {
                var btnImgs = $(containerId).find("input[type='image']");
                for (var i = 0; i < btnImgs.length; i++) {
                    $(btnImgs[i]).attr("class", "imgButton");
                }
                $(this).attr("class", "active imgButton");
            })
        };
    };
    applyToggleEffectToButtons("#divTrackMode");
    applyToggleEffectToButtons("#divTrackType");
    applyToggleEffectToButtons("#divMeasure");

    // apply highlighted effect to buttons when mouse-over
    var highlightImg = function (imgContainerIds) {
        for (var i = 0; i < imgContainerIds.length; i++) {
            var btnImgs = $(imgContainerIds[i]).find("input[type='image']");

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

    // hide track types button and measure type buttons
    $("#divTrackType").hide();
    $("#divMeasure").hide();

    // Implement collapse effect to toggle button
    $("#toggle img").bind("click", function () {
        if ($("#leftContainer").is(':visible')) {
            $(this).attr("src", "Images/expand.gif");
            $("#map-content").css("width", "99%");
            $("#toggle").css("left", "5px");
            $("#leftContainer").hide();
        }
        else {
            $("#leftContainer").show();
            $(this).attr("src", "Images/collapse.gif");
            $("#map-content").css("width", "80%");
            $("#toggle").css("left", "20%");
        }
        resizeElementHeight();
    });

    var resizeElementHeight = function () {
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

    $("#divMessage").dialog({
        autoOpen: false,
        modal: true,
        buttons: {
            ok: function () {
                $(this).dialog("close");
            }
        }
    });
}

function savedSuccessfully() {
    showDialog("divMessage", "Saved Successfully.");
}

function savedFailed() {
    showDialog("divMessage","Saved Failed.");
}

function showDialog(id, html) {
    $("#" + id).html(html);
    $("#" + id).dialog("open");
    $(".ui-dialog-titlebar").hide();    // Hide dialog header bar
}

function hideDialog(id) {
    $("#" + id).dialog("close");
}

function resetDefaultStatus() {
    var btnImgs = $("#divTrackType").find("input[type='image']");
    for (var i = 0; i < btnImgs.length; i++) {
        $(btnImgs[i]).attr("class", "imgButton");
    }
    var btnImgs = $("#divMeasure").find("input[type='image']");
    for (var i = 0; i < btnImgs.length; i++) {
        $(btnImgs[i]).attr("class", "imgButton");
    }
}