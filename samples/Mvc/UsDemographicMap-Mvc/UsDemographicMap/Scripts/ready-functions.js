$(document).ready(function () {
    initializePageElements();

    // apply the selected style to server side
    var applyStyleToServer = function (args) {
        var argsInString = JSON.stringify(args);
        $("#clientStatusKeeper").text(argsInString); // keep last request status
        // Remove the not supported charactor in URL
        if (argsInString.indexOf('>=') >= 0) {
            argsInString = argsInString.replace('>=', '$');
        }
        Map1.ajaxCallAction("default", "ApplyStyle", { "args": argsInString }, function () {
            Map1.getDynamicOverlay().redraw(true);
            Map1.getAdornmentOverlay().redraw(true);
        });
    };

    // Apply events to color picker
    var updateMapColorSystem = function (sliderVal) {
        var requestArgs = $("#clientStatusKeeper").text();
        if ($.trim(requestArgs) != '') {
            var args = $.parseJSON(requestArgs);
            args["startColor"] = $('#colorStartselector').val();
            args["endColor"] = $('#colorEndselector').val();
            args["colorDirection"] = $('#slColorWheelDirection').val();
            if (arguments.length > 0 && !isNaN(parseInt(sliderVal)))
                args["sliderValue"] = sliderVal;

            applyStyleToServer(args);
        }
    }
    $("#slColorWheelDirection").change(updateMapColorSystem);
    $('#colorStartselector').colorselector({ callback: updateMapColorSystem });

    // show pie button after actived
    var refreshPieButton = function () {
        var activeIndex = parseInt($("#accordion").accordion("option", "active"));
        var accordionHeaders = $("#accordion h3");
        var accordionContents = $('#accordion ul');
        for (var i = 0; i < accordionHeaders.length; i++) {
            var pieButton = $(accordionHeaders[i]).find("input")[0];
            var categoryItems = $(accordionContents[i]).find("Span");
            if (activeIndex == i && categoryItems.length > 1) {
                $(pieButton).css('visibility', 'visible');
            } else {
                $(pieButton).css('visibility', 'hidden');
            }
        }
    }
    $("#accordion h3").click(refreshPieButton);
    refreshPieButton();

    // Change style and do redraw map
    $("img[class='accordion-content-right']").click(function () {
        var alias = $.trim($(this).parent().find(".accordion-content").text());
        applyStyleToServer({ "style": this.alt, "alias": alias });

        resetConfigurations(this.alt);

        return false;
    });

    // attach checkbox changed event and redraw map
    getActivePieButton = function () {
        var activeIndex = parseInt($("#accordion").accordion("option", "active"));
        return $("input[class='accordion-header-right'][type='image']")[activeIndex];
    };
    $("#accordion ul [type='checkbox']").change(function () {
        var activeIndex = parseInt($("#accordion").accordion("option", "active"));
        // get all the seleceted category items
        var activeContent = $('#accordion ul')[activeIndex];
        var checkboxes = $(activeContent).find("[type='checkbox']");
        var selectedItems = new Array();
        for (var i = 0; i < checkboxes.length; i++) {
            if (checkboxes[i].checked) {
                var itemAlias = $(checkboxes[i]).parent().find("Span").text();
                selectedItems.push($.trim(itemAlias));
            }
        }

        var pieButton = getActivePieButton();
        if (selectedItems.length > 1) {
            applyStyleToServer({ "request": "applyStyle", "style": "Pie", "selectedItems": selectedItems, "startColor": $('#colorStartselector').val() });
            resetConfigurations("Pie");

            $(pieButton).css('visibility', 'visible');
        } else {
            $(pieButton).css('visibility', 'hidden');
        }
        return false;
    });

    // Deal with pie button click event
    $("input[class='accordion-header-right'][type='image']").click(function () {
        // get all the seleceted category items
        var activeIndex = parseInt($("#accordion").accordion("option", "active"));
        var activeContent = $('#accordion ul')[activeIndex];
        var checkboxes = $(activeContent).find("[type='checkbox']");
        var selectedItems = new Array();
        for (var i = 0; i < checkboxes.length; i++) {
            if (checkboxes[i].checked) {
                var itemAlias = $(checkboxes[i]).parent().find("Span").text();
                selectedItems.push($.trim(itemAlias));
                $(checkboxes[i]).css("visibility", "visible");
            }
        }

        applyStyleToServer({ "style": this.alt, "selectedItems": selectedItems, "startColor": $('#colorStartselector').val() });
        resetConfigurations(this.alt);

        return false;
    });

    // set default style as Population thematic
    var activeContent = $('#accordion ul')[0];
    var themeticButton = $(activeContent).find("img")[1];
    $(themeticButton).trigger("click");

    // Init slider bar
    $("#slider").slider({
        min: 1,
        max: 5,
        range: "min",
        value: 3,
        slide: function (event, ui) {
            updateMapColorSystem(ui.value);
        }
    });
});

// All the codes before creating map
var OnMapCreating = function (map) {
    OpenLayers.Map.prototype.getPopup = function (id) {
        var searched = null;
        for (var i = 0; i < map.popups.length; i++) {
            if (map.popups[i].id == id) {
                searched = map.popups[i];
                break;
            }
        }

        return searched;
    }
}

var popupDisplayTimer;
var OnMapCreated = function (map) {
    // Show mouse coordinates on the page bottom
    map.events.register("mousemove", map, function (e) {
        var mouseCoordinate = this.getLonLatFromPixel(new OpenLayers.Pixel(e.clientX, e.clientY));
        $("#spanMouseCoordinate").text("X:" + mouseCoordinate.lon.toFixed(6) + "  Y: " + mouseCoordinate.lat.toFixed(6));
    });

    // Initialize the hover-popup
    var hightlightLayer = map.getLayer("HighlightOverlay");
    hightlightLayer.events.register("mouseout", hightlightLayer, function (e) {
        // Clear timer at first
        window.clearTimeout(popupDisplayTimer);
        if (Map1.getPopup("featureInfoPopup") != null) {
            Map1.getPopup("featureInfoPopup").hide();
        }
    });
    hightlightLayer.events.register("mouseover", hightlightLayer, function (e) {
        var map = Map1.getOpenLayersMap();
        var lonlat = map.getLonLatFromPixel(new OpenLayers.Pixel(e.offsetX, e.offsetY));
        var mouseCoordinate = new OpenLayers.Geometry.Point(lonlat.lon, lonlat.lat);
        var selectedFeature = null;
        for (var i = 0; i < hightlightLayer.features.length; i++) {
            if (hightlightLayer.features[i].geometry.intersects(mouseCoordinate)) {
                selectedFeature = hightlightLayer.features[i];
                break;
            }
        }
        // Clear timer at first
        window.clearTimeout(popupDisplayTimer);

        if (selectedFeature != null) {
            var requestArgs = $("#clientStatusKeeper").text();
            if ($.trim(requestArgs) != '') {
                var args = $.parseJSON(requestArgs);
                args["selectedFeatureId"] = selectedFeature.id;
                popupDisplayTimer = window.setTimeout(function () {
                    Map1.ajaxCallAction("default", "IdentifyFeature", { "args": JSON.stringify(args) }, function (result) {
                        if (result.get_responseData() != "") {
                            var featureInfoPopup = Map1.getPopup("featureInfoPopup");
                            featureInfoPopup.setContentHTML(result.get_responseData());
                            featureInfoPopup.lonlat = lonlat;
                            featureInfoPopup.updatePosition();
                            featureInfoPopup.show();
                        }
                    });
                }, 1000);
            }
        }
    });
}

function initializePageElements() {
    var resizeElementHeight = function () {
        var documentheight = $(window).height();
        var mapDivH = (documentheight - $("#header").height() - $("#footer").height() - 1) + "px";
        $("#map-content").height(mapDivH);
        $("#leftContainer").height(mapDivH);
        $("#toggle").height(mapDivH);
        $("#toggle").css("line-height", mapDivH);

        // refresh the map.
        Map1.getOpenLayersMap().updateSize();
    }

    window.onload = resizeElementHeight();
    $(window).resize(resizeElementHeight);

    // init accordion tree
    $("#accordion").accordion({
        icons: null,
        heightStyle: "content"
    });
    $("#accordion").accordion("option", "active", 0);

    // init color picker and color direction
    var initColorPicker = function (select) {
        var colors = getHtmlKnownColors();
        for (var item in colors) {
            var option = new Option(item, colors[item]);
            $(option).attr("data-color", colors[item]);
            $(select).append(option);
        }
    }
    initColorPicker($("#colorStartselector"));
    initColorPicker($("#colorEndselector"));

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
}

function resetConfigurations(style) {
    switch (style) {
        case "Thematic":
            $('#colorStartselector').colorselector("setColor", "#74a0ff");
            $('#colorEndselector').colorselector("setColor", "#dc3438");
            $("#spanBaseColor").text("Display Start Color:");
            $("#divEndColor").show();
            $("#divColorWheel").show();
            $("#divSlider").hide();
            break;
        case "DotDensity":
            $('#colorStartselector').colorselector("setColor", "#8B0000");
            $("#spanBaseColor").text("Display Color:");
            $("#divEndColor").hide();
            $("#divColorWheel").hide();
            $("#divSlider").show();
            $("#spanSliderTitle").text("DotDensity Unit:");
            break;
        case "ValueCircle":
            $('#colorStartselector').colorselector("setColor", "#ff7f00");
            $("#spanBaseColor").text("Display Color:");
            $("#divEndColor").hide();
            $("#divColorWheel").hide();
            $("#divSlider").show();
            $("#spanSliderTitle").text("MAGNIFICATION:");
            break;
        case "Pie":
            $("#divSlider").hide();
            $("#spanBaseColor").text("Display Color:");
            $('#colorStartselector').colorselector("setColor", "#74a0ff");
            break;
        default:
            break;
    }
}

function getHtmlKnownColors() {
    var colors = {
        "Balck": "#000000", "White": "#FFFFFF", "Aqua": "#00FFFF", "MidnightBlue": "#191970",
        "DodgerBlue": "#1E90FF", "LightSeaGreen": "#20B2AA", "ForestGreen": "#228B22", "SeaGreen": "#2E8B57",
        "DarkSlateGray": "#2F4F4F", "LimeGreen": "#32CD32", "MediumSeaGreen": "#3CB371", "Turquoise": "#40E0D0",
        "SteelBlue": "#4682B4", "DarkSlateBlue": "#483D8B", "MediumTurquoise": "#48D1CC", "DarkOliveGreen": "#556B2F",
        "CadetBlue": "#5F9EA0", "MediumAquaMarine": "#66CDAA", "DimGray": "#696969", "SlateBlue": "#6A5ACD",
        "OliveDrab": "#6B8E23", "SlateGray": "#708090", "LightSlateGray": "#778899", "MediumSlateBlue": "#7B68EE",
        "Aquamarine": "#7FFFD4", "Maroon": "#800000", "Olive": "#808000", "Gray": "#808080",
        "SkyBlue": "#87CEEB", "BlueViolet": "#8A2BE2", "DarkRed": "#8B0000", "SaddleBrown": "#8B4513",
        "DarkSeaGreen": "#8FBC8F", "LightGreen": "#90EE90", "MediumPurple": "#9370DB", "PaleGreen": "#98FB98",
        "YellowGreen": "#9ACD32", "Sienna": "#A0522D", "DarkGray": "#A9A9A9", "LightBlue": "#74a0ff",
        "GreenYellow": "#ADFF2F", "Yellow ": "#FFFF00", "PaleTurquoise": "#AFEEEE", "LightSteelBlue": "#B0C4DE",
        "PowderBlue": "#B0E0E6", "FireBrick": "#B22222", "DarkGoldenRod": "#B8860B", "MediumOrchid": "#BA55D3",
        "RosyBrown": "#BC8F8F", "DarkKhaki": "#BDB76B", "Silver": "#C0C0C0", "MediumVioletRed": "#C71585",
        "IndianRed ": "#CD5C5C", "Peru": "#CD853F", "Chocolate": "#D2691E", "Tan": "#D2B48C", "LightGray": "#D3D3D3",
        "Thistle": "#D8BFD8", "Orchid": "#DA70D6", "GoldenRod": "#DAA520", "PaleVioletRed": "#DB7093", "Red": "#FF0000",
        "LightRed": "#dc3438", "Orange": "#ff7f00"
    };
    return colors;
}