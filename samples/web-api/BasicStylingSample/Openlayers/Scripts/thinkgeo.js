//  Initialize color pickers
$(document).ready(function () {
    $('.colorSelector').ColorPicker({
        onShow: function (colpkr) {
            $(colpkr).fadeIn(500);
            return false;
        },
        onHide: function (colpkr) {
            $(colpkr).fadeOut(500);
            return false;
        },
        onChange: function (hsb, hex, rgb, el) {
            $(el).css('backgroundColor', '#' + hex);
            $(el).siblings("input").val('#' + hex);
        }
    });
});

// Initialize the map.
var map = new ol.Map({
    target: 'map',
    controls: ol.control.defaults({ attribution: false }).extend(
        [new ol.control.Attribution({
            collapsible: false
        })]),
    view: new ol.View({
        center: ol.proj.transform([-95.81131, 40.14711], 'EPSG:4326', 'EPSG:3857'),
        zoom: 4
    })
});

// Add image buttons for layers, edit, help etc.
var imgControls = new app.ImagesControl({
    imgs: [
            {
                id: 'lyrOptions',
                src: 'images/layers.png',
                title: 'Show layers',
                callback: function () {
                    $('#leftPanel').animate({
                        'left': '0px'
                    });
                }
            },
            {
                src: 'images/gear.png',
                id: 'btnConfig',
                title: 'Show style settings',
                css: 'openlayers-disabled',
                callback: function () {
                    loadStyleDlg();
                    showStyleDlg();
                }
            },
            {
                id: 'btnInfo',
                src: 'images/info.png',
                title: 'Show help',
                callback: function () { window.open('http://wiki.thinkgeo.com/wiki/map_suite_styling', '_blank'); }
            }
    ]
});
map.addControl(imgControls);

// Added ThinkGeoCloudMaps as the background map. 
var thinkgeoCloudMapsLayer = new ol.layer.Tile({
    source: new ol.source.XYZ({
        url: 'https://cloud{1-6}.thinkgeo.com/api/v1/maps/raster/light/x1/3857/512/{z}/{x}/{y}.png?apikey=ThinkGeo Cloud API Key',
        maxZoom: 19,
        tileSize: 512,
        params:
        {
            'LAYERS': 'ThinkGeoCloudMaps',
            'VERSION': '10.4.0',
            'STYLE': 'Light'
        },

        // --------------------------------------------------------------------------------------
        // Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
        // an API Key. The following function is just for reminding you to input the key. 
        // Feel free to remove this function after the key was input. 
        // --------------------------------------------------------------------------------------
        tileLoadFunction: function (imageTile, src) {
            fetch(src).then((response) => {
                if (response.status === 401) {
                    var canvas = document.createElement("canvas");
                    canvas.width = 512;
                    canvas.height = 512;
                    var context = canvas.getContext("2d");
                    context.font = "14px Arial";
                    context.strokeText("Backgrounds for this sample are", 256, 100);
                    context.strokeText("powered by ThinkGeo Cloud Maps and", 256, 120);
                    context.strokeText("require an API Key. This was sent", 256, 140);
                    context.strokeText("to you via email when you signed up", 256, 160);
                    context.strokeText("with ThinkGeo, or you can register", 256, 180);
                    context.strokeText("now at https://cloud.thinkgeo.com", 256, 200);
                    var url = canvas.toDataURL("image/png", 1);
                    imageTile.getImage().src = url;
                }
                else {
                    response.blob().then((blob) => {
                        if (blob) {
                            imageTile.getImage().src = URL.createObjectURL(blob);
                        }
                        else {
                            imageTile.getImage().src = "";
                        }
                    });
                }
            });
        }
    })
});


// Initialize default value .
var selectedLayer = 'PredefinedStyles';

// Get access id.
var accessId = guid();


// Add dynamic tile layer to the map, it can be refreshed dynamically
var xyzSource = new ol.source.XYZ({
    url: getRootPath() + '/BasicStyling/PredefinedStyles/{z}/{x}/{y}/' + accessId,
    maxZoom: 19,
    attributions: [new ol.Attribution({
        html: '<a href="http://thinkgeo.com/map-suite-developer-gis/world-map-kit-sdk/">ThinkGeo</a> | &copy; <a href="http://osm.org/copyright">OpenStreetMap</a> contributors <a href="http://www.openstreetmap.org/copyright">ODbL</a>'
    })]
});
xyzSource.tileLoadFunction = function (imageTile, src) {
    imageTile.getImage().src = src + '?t=' + new Date().getTime();
};
map.addLayer(new ol.layer.Tile({
    source: xyzSource
}));

// Load style editing page.
var loadStyleDlg = function () {

    var url = getRootPath() + '/BasicStyling/GetStyle/' + selectedLayer + '/' + accessId;
    switch (selectedLayer) {
        case 'AreaStyle':
            $("#areaStyleEdit").show();
            $('#settingTitle').text('Area Style Edit Settings');
            $.get(url, function (data) {
                if (data) {
                    $('#areaStyleFillSolidBrushColorText').val(data.FillSolidBrushColor);
                    $('#areaStyleFillSolidBrushColorSelector').css('background-color', data.FillSolidBrushColor);
                    $('#areaStyleFillSolidBrushColorSelector').ColorPickerSetColor(data.FillSolidBrushColor);
                    $('#areaStyleFillSolidBrushAlpha').val(data.FillSolidBrushAalpha);

                    $('#areaStyleOutlinePenColorText').val(data.OutlinePenColor);
                    $('#areaStyleOutlinePenColorSelector').css('background-color', data.OutlinePenColor);
                    $('#areaStyleOutlinePenColorSelector').ColorPickerSetColor(data.OutlinePenColor);
                    $('#areaStyleOutlinePenAlpha').val(data.OutlinePenAalpha);
                    $('#areaStyleOutlinePenWidth').val(data.OutlinePenWidth);
                }
                else {
                    alert('ERROR: Get current setting error');
                }
            });
            break;
        case 'LineStyle':
            $("#lineStyleEdit").show();
            $('#settingTitle').text('Line Style Edit Settings');
            $.get(url, function (data) {
                if (data) {
                    $('#lineStyleCenterPenColorText').val(data.CenterPenColor);
                    $('#lineStyleCenterPenColorSelector').css('background-color', data.CenterPenColor);
                    $('#lineStyleCenterPenColorSelector').ColorPickerSetColor(data.CenterPenColor);
                    $('#lineStyleCenterPenAlpha').val(data.CenterPenAlpha);
                    $('#lineStyleCenterPenWidth').val(data.CenterPenWidth);

                    $('#lineStyleOuterPenColorText').val(data.OuterPenColor);
                    $('#lineStyleOuterPenColorSelector').css('background-color', data.OuterPenColor);
                    $('#lineStyleOuterPenColorSelector').ColorPickerSetColor(data.OuterPenColor);
                    $('#lineStyleOuterPenAlpha').val(data.OuterPenAlpha);
                    $('#lineStyleOuterPenWidth').val(data.OuterPenWidth);

                    $('#lineStyleInnerPenColorText').val(data.InnerPenColor);
                    $('#lineStyleInnerPenColorSelector').css('background-color', data.InnerPenColor);
                    $('#lineStyleInnerPenColorSelector').ColorPickerSetColor(data.InnerPenColor);
                    $('#lineStyleInnerPenAlpha').val(data.InnerPenAlpha);
                    $('#lineStyleInnerPenWidth').val(data.InnerPenWidth);
                }
                else {
                    alert('ERROR: Get current setting error');
                }
            });
            break;
        case 'SymbolPoint':
            $("#symbolPointEdit").show();
            $('#settingTitle').text('Symbol Point Style Edit Settings');
            $.get(url, function (data) {
                if (data) {
                    $("#symbolPointType option").eq(data.SymbolPointType).prop("selected", true);
                    $('#symbolPointSize').attr('value', data.SymbolPointSize);
                    $('#symbolPointRotationAngle').val(data.symbolPointRotationAngle);

                    $('#symbolPointPenColorText').val(data.SymbolPenColor);
                    $('#symbolPointPenColorSelector').css('background-color', data.SymbolPenColor);
                    $('#symbolPointPenColorSelector').ColorPickerSetColor(data.SymbolPenColor);
                    $('#symbolPointPenAlpha').val(data.SymbolPointPenAlpha);
                    $('#symbolPointPenWidth').val(data.SymbolPointPenWidth);

                    $('#symbolPointSolidBrushColorText').val(data.SymbolSolidBrushColor);
                    $('#symbolPointSolidBrushColorSelector').css('background-color', data.SymbolSolidBrushColor);
                    $('#symbolPointSolidBrushColorSelector').ColorPickerSetColor(data.SymbolSolidBrushColor);
                    $('#symbolPointSolidBrushAlpha').val(data.SymbolSolidBrushAlpha);
                } else {
                    alert('ERROR: Get current setting error');
                }
            });
            break;
        default:
            break;
    }
};

// Show edit dialog.
function showStyleDlg() {
    switch (selectedLayer) {
        case 'AreaStyle':
        case 'LineStyle':
        case 'SymbolPoint':
            $('#bg-mask').show();
            var offsetY = -$('#editPanel').height()/2;
            $('#editPanel').css('margin-top', offsetY + 'px');
            $('#editPanel').slideToggle("fast");
            break;
        default:
            break;
    }
}

// Hide edit dialog.
function hideStyleDlg() {
    $('#bg-mask').hide();
    $('#editPanel').slideToggle("fast");
    $(".editContent").hide();
}

// Save the style modified.
$('#btnSave').click(function () {
    var paras;
    var url;
    switch (selectedLayer) {
        case 'AreaStyle':
            paras = {
                fillSolidBrushColor: document.getElementById("areaStyleFillSolidBrushColorText").value,
                fillSolidBrushAlpha: document.getElementById("areaStyleFillSolidBrushAlpha").value,
                outerPenColor: document.getElementById("areaStyleOutlinePenColorText").value,
                outerPenAlpha: document.getElementById("areaStyleOutlinePenAlpha").value,
                outerPenWidth: document.getElementById("areaStyleOutlinePenWidth").value
            };
            url = getRootPath() + 'BasicStyling/UpdateStyle/AreaStyle/' + accessId;
            break;
        case 'LineStyle':
            paras = {
                centerPenColor: document.getElementById("lineStyleCenterPenColorText").value,
                centerPenColorAlpha: document.getElementById("lineStyleCenterPenAlpha").value,
                centerPenWidth: document.getElementById("lineStyleCenterPenWidth").value,

                outterPenColor: document.getElementById("lineStyleOuterPenColorText").value,
                outterPenColorAlpha: document.getElementById("lineStyleOuterPenAlpha").value,
                outterPenWidth: document.getElementById("lineStyleOuterPenWidth").value,

                innerPenColor: document.getElementById("lineStyleInnerPenColorText").value,
                innerPenColorAlpha: document.getElementById("lineStyleInnerPenAlpha").value,
                innerPenWidth: document.getElementById("lineStyleInnerPenWidth").value
            };
            url = getRootPath() + 'BasicStyling/UpdateStyle/LineStyle/' + accessId;
            break;
        case 'SymbolPoint':
            paras = {
                symbolPointType: document.getElementById("symbolPointType").value,
                symbolPointSize: document.getElementById("symbolPointSize").value,
                symbolPointRotationAngle: document.getElementById("symbolPointRotationAngle").value,
                symbolPointPenColor: document.getElementById("symbolPointPenColorText").value,
                symbolPointPenAlpha: document.getElementById("symbolPointPenAlpha").value,
                symbolPointPenWidth: document.getElementById("symbolPointPenWidth").value,
                symbolPointSolidBrushColor: document.getElementById("symbolPointSolidBrushColorText").value,
                symbolPointSolidBrushAlpha: document.getElementById("symbolPointSolidBrushAlpha").value
            };
            url = getRootPath() + 'BasicStyling/UpdateStyle/SymbolPoint/' + accessId;
            break;
        default:
            break;
    }
    SettingPost(url, paras);
});

// Send setting style request.
function SettingPost(url, paras) {
    $.post(url, { '': JSON.stringify(paras) }, function (data) {
        if (data) {
            hideStyleDlg();
            xyzSource.setUrl(getRootPath() + 'BasicStyling/' + selectedLayer + '/{z}/{x}/{y}/' + accessId);
        } else {
            alert("error:Invalid parameter");
        }
    });
}

// Cancel editing style.
$('#btnCancel').click(function () { hideStyleDlg(); });

// Refresh sample by selecting sample item.
$("#stylingOptions div").bind("click", function () {
    var styles = $("#stylingOptions div");
    for (var i = 0; i < styles.length; i++) {
        $(styles[i]).attr("class", "unselected");
    }
    $(this).attr("class", "selected");

    selectedLayer = $(this).attr("id");
    var center = $(this).attr('center').split(',');
    var zoom = $(this).attr('zoom');
    $('#styleDescription').text($(this).attr('description'));

    if ($(this).attr('hasSetting') === 'true') {
        $("#btnConfig").removeClass('openlayers-disabled');
    } else {
        $("#btnConfig").addClass('openlayers-disabled');
    }
    map.removeLayer(thinkgeoCloudMapsLayer);
    if (selectedLayer !== "PredefinedStyles") {
        map.addLayer(thinkgeoCloudMapsLayer);
    }
    // Redraw the map
    xyzSource.setUrl(getRootPath() + 'BasicStyling/' + selectedLayer + '/{z}/{x}/{y}/' + accessId);
    map.addLayer(new ol.layer.Tile({
        source: xyzSource
    }));
    map.setView(new ol.View({
        center: ol.proj.transform([parseFloat(center[1]), parseFloat(center[0])], 'EPSG:4326', 'EPSG:3857'),
        zoom: zoom
    }));
});

// Show or hide left navigation panel.
$("html").click(function () {
    $('#leftPanel').animate({
        'left': -$('#leftPanel').width() + 'px'
    });
});

// Add a utility methods to get guid. 
function guid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
    }
    return (s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4()).toLowerCase();
}