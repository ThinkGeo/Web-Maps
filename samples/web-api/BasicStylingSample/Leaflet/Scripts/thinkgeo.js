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
var map = L.map('map').setView([40.14711, -95.81131], 4);

// Add image buttons for layers, settings, help etc.
L.imageButtons({
    imgs: [
        {
            src: 'images/layers.png',
            id: 'lyrOptions',
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
            css: 'leaflet-disabled',
            callback: function () {
                loadStyleDlg();
                showStyleDlg();
            }
        },
        {
            src: 'images/info.png',
            id: 'btnInfo',
            title: 'Show help',
            callback: function () { window.open('http://wiki.thinkgeo.com/wiki/map_suite_styling', '_blank'); }
        }
    ]
}).addTo(map);

// Add ThinkGeo Cloud Maps as the map's background layer. 
var thinkgeoCloudMapsLayer = L.tileLayer('https://{s}.thinkgeo.com/api/v1/maps/raster/light/x1/3857/256/{z}/{x}/{y}.png?apikey=ThinkGeo Cloud API Key', {
    subdomains: ['cloud1', 'cloud2', 'cloud3', 'cloud4', 'cloud5', 'cloud6'],
    layers: 'ThinkGeoCloudMaps',
    format: 'image/png',
    styles: 'Light',
    version: '1.1.1'
})
    // --------------------------------------------------------------------------------------
    // Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    // an API Key. The following function is just for reminding you to input the key. 
    // Feel free to remove this function after the key was input. 
    // --------------------------------------------------------------------------------------
    .on('tileloadstart', function (e) {
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
    })
    .addTo(map);

// Initialize default value.
var selectedLayer = 'PredefinedStyles';

// Get access id.
var accessId = guid();

// Add dynamic tile layer to the map, it can be refreshed dynamically
var shapeLayer = L.dynamicLayer(L.Util.getRootPath() + '/BasicStyling/PredefinedStyles/{z}/{x}/{y}/' + accessId, { unloadInvisibleTiles: true, reuseTiles: false }).addTo(map);

// Load style editing page.
var loadStyleDlg = function () {
    var url = L.Util.getRootPath() + '/BasicStyling/GetStyle/' + selectedLayer + '/' + accessId;
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
            this.bgmask = L.DomUtil.create('div', 'bg-mask');
            $('body').append(this.bgmask);
            var offsetY = -$('#editPanel').height() / 2;
            $('#editPanel').css('margin-top', offsetY + 'px');
            $("#editPanel").slideToggle("fast");
            break;
        default:
            break;
    }
}

// Hide edit dialog.
function hideStyleDlg() {
    $('.bg-mask').remove();
    $('#editPanel').slideToggle("fast");
    $(".editContent").hide();
}

// Save the style modified.
$('#btnSave').click(function () {
    var paras;
    switch (selectedLayer) {
        case 'AreaStyle':
            paras = {
                fillSolidBrushColor: document.getElementById("areaStyleFillSolidBrushColorText").value,
                fillSolidBrushAlpha: document.getElementById("areaStyleFillSolidBrushAlpha").value,
                outerPenColor: document.getElementById("areaStyleOutlinePenColorText").value,
                outerPenAlpha: document.getElementById("areaStyleOutlinePenAlpha").value,
                outerPenWidth: document.getElementById("areaStyleOutlinePenWidth").value
            };
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
                symbolPointSolidBrushAlpha: document.getElementById("symbolPointSolidBrushAlpha").value,
            };
            break;
        default:
            break;
    }
    var url = L.Util.getRootPath() + 'BasicStyling/UpdateStyle/' + selectedLayer + '/' + accessId;
    SettingPost(url, paras);
});

// Send setting style request.
function SettingPost(url, paras) {
    $.post(url, { '': JSON.stringify(paras) }, function (data) {
        if (data) {
            hideStyleDlg();
            shapeLayer.setUrl(L.Util.getRootPath() + 'BasicStyling/' + selectedLayer + '/{z}/{x}/{y}/' + accessId);
            shapeLayer.redraw();
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

    // Redraw the map.
    selectedLayer = $(this).attr("id");
    var center = $(this).attr('center').split(',');
    var zoom = $(this).attr('zoom');
    $('#styleDescription').text($(this).attr('description'));

    if ($(this).attr('hasSetting') === 'true') {
        $("#btnConfig").removeClass('leaflet-disabled');
    } else {
        $("#btnConfig").addClass('leaflet-disabled');
    }

    map.removeLayer(thinkgeoCloudMapsLayer);
    if (selectedLayer !== "PredefinedStyles") {
        map.addLayer(thinkgeoCloudMapsLayer);
        thinkgeoCloudMapsLayer.bringToBack();
    }

    shapeLayer.setUrl(L.Util.getRootPath() + 'BasicStyling/' + selectedLayer + '/{z}/{x}/{y}/' + accessId);
    map.setView(L.latLng(center[0], center[1]), zoom);
    shapeLayer.redraw();
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
