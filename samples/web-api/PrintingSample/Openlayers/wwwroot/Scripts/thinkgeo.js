
// Initialize access id.
var accessId = guid();

// Initialize map.
var map = new ol.Map({
    target: 'map',
    controls: ol.control.defaults({ attribution: false, rotate: false }).extend(
    [new ol.control.Attribution({ collapsible: false })]),
    view: new ol.View({ center: [0, 0], zoom: 17 })
});

// Load printer overlay.
var printerSource = new ol.source.XYZ({
    url: getRootPath() + 'Printing/LoadPrinterOverlay/{z}/{x}/{y}/' + accessId,
    maxZoom: 19,
    tileLoadFunction: function (imageTile, src) { imageTile.getImage().src = src + '?t=' + new Date().getTime(); }
});

// WebApi controller create page png befor draw map.
var url = getRootPath() + 'Printing/PrepareImage/' + accessId;
$.get(url, function () { map.addLayer(new ol.layer.Tile({ source: printerSource })); });

// Update map printer layer extent by pan and zoom.
$(".updateMapExtent").click(function () { UpdatePrintInformation($(this).val(), "None"); });

// Add or remove printer layer.
$("#printerLayers div").click(function () {
    var selected = "false";
    if ($(this).attr("class") == "selected") {
        $(this).attr("class", "unselected");
    } else {
        $(this).attr("class", "selected");
        selected = "true";
    }
    UpdatePrintInformation($(this).attr("key"), selected);
});

// Export page to file.
$("#exportPanel input").click(function () {
    var exportType = $(this).val();

    var url = getRootPath() + 'Printing/Export/' + accessId + '/' + exportType;
    if (exportType == "To Printer") {
        // Print page.
        var content = "<html><head><script>function printmap(){window.print();}</script></head><body onload='printmap()'><img src='" + url + "' /></body></html>";
        var pwa = window.open("about:blank", "_new");
        pwa.document.open();
        pwa.document.write(content);
        pwa.document.close();
    } else {
        // Export png or pdf.
        window.location.href = url;
    }
});

// Update printer layers information.
function UpdatePrintInformation(key, value) {
    var url = getRootPath() + 'Printing/UpdatePrintingInfo/' + accessId + '/' + key + '/' + value;
    $.get(url, function () {
        url = getRootPath() + 'Printing/PrepareImage/' + accessId;
        $.get(url, function () {
            // Redraw map after WebApi controller update page png.
            printerSource.setUrl(getRootPath() + 'Printing/LoadPrinterOverlay/{z}/{x}/{y}/' + accessId);
        });
    });
}

// Add a utility methods to get guid. 
function guid() {
    function s4() { return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1); }
    return (s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4()).toLowerCase();
}
