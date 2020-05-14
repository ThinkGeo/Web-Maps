$(document).ready(function () {
    window.onload = resizeElementHeight();
    $(window).resize(resizeElementHeight);

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

    // set the toggle style for group buttons
    $("#divTrackMode input[type=image]").bind("click", function () {
        var btnImgs = $("#divTrackMode input[type=image]");
        for (var i = 0; i < btnImgs.length; i++) {
            $(btnImgs[i]).attr("class", "");
        }
        $(this).attr("class", "active");
    });

    // set the toggle effect for 2 service mode radio button
    $("input[name='ServiceArea']").change(function () {
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
                $("#btnNormal").trigger("click");
                $(this).dialog("close");
            }
        }
    });
    // Make sure the resize method take effect when doing ajax call
    Sys.Application.remove_load(resizeElementHeight);
    Sys.Application.add_load(resizeElementHeight);
});

var OnMapCreated = function (map) {
    map.events.register("mousemove", map, function (e) {
        var mouseCoordinate = this.getLonLatFromPixel(new OpenLayers.Pixel(e.clientX, e.clientY));
        if (mouseCoordinate) {
            $("#spanMouseCoordinate").text("X:" + mouseCoordinate.lon.toFixed(2) + "  Y: " + mouseCoordinate.lat.toFixed(2));
        }
    });
}

function resizeElementHeight() {
    var documentheight = $(window).height();
    var contentDivH = documentheight - $("#footer").height() - $("#header").height() - 1;
    $("#content-container").height(contentDivH + "px");

    $("#leftContainer").height(contentDivH + "px");
    $("#map-content").height(contentDivH + "px");
    $("#toggle").css("line-height", contentDivH + "px");

    if (contentDivH > 450) {
        $("#queryResultUpdatePanel").height(contentDivH - 450 + "px");
    } else {
        $("#queryResultUpdatePanel").visible = false;
    }

    $("#mapContainer").height(contentDivH + "px");

    // refresh the map.
    Map1.GetOpenLayersMap().updateSize();
}