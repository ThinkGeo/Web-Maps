var zoom = 5;
var center = [40.14711, -95.81131];
var topologyCategory = 'LinesTopology';
var selectedTopology = 'LinesEndpointMustBeCoveredByPoints';

LoadIcons();

// Create the map.
var map = new ol.Map({
    target: 'map',
    controls: ol.control.defaults({ attribution: false }).extend(
        [new ol.control.Attribution({
            collapsible: false
        })]),
    view: new ol.View({
        center: ol.proj.transform([-95.81131, 40.14711], 'EPSG:4326', 'EPSG:3857'),
        zoom: 5
    })
});

// Add image buttons for layers and help.
var imgControls = new app.ImagesControl({
    imgs: [
        {
            id: 'lyrOptions',
            src: 'Images/LeftControlBar.png',
            title: 'Show Topology Control Bar',
            callback: function () { $('#leftControlBar').animate({ 'left': '0px' }); }
        },
        {
            id: 'btnInfo',
            src: 'Images/info.png',
            title: 'Show help',
            callback: function () { window.open('http://wiki.thinkgeo.com/wiki/map_suite_webapi_topology', '_blank'); }
        }
    ]
});
map.addControl(imgControls);

// Add ThinkGeoCloudMap as the map's background layer.
var thinkGeoCloudMapsLayer = new ol.layer.Tile({
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
map.addLayer(thinkGeoCloudMapsLayer);

// Add dynamic tile layer to the map, it can be refreshed dynamically
var xyzSource = new ol.source.XYZ({
    url: getRootPath() + 'TopologyValidation/RefreshSourceLayer/' + selectedTopology + '/{z}/{x}/{y}',
    maxZoom: 19
});
xyzSource.tileLoadFunction = function (imageTile, src) {
    imageTile.getImage().src = src + '?t=' + new Date().getTime();;
};
map.addLayer(new ol.layer.Tile({
    source: xyzSource
}));

function LoadIcons() {
    var leftControlBarItems = $(".leftControlBarItem div");
    for (var i = 0; i < leftControlBarItems.length; i++) {
        $(leftControlBarItems[i]).attr("style", "background-image:url(../Images/TopologyIcons/" + leftControlBarItems[i].id + ".png);");
    }
}

//// Refresh sample by selecting sample item.
$(".leftControlBarItem div").click(function () {
    var controlItem = $(".leftControlBarItem div");
    for (var i = 0; i < controlItem.length; i++) {
        $(controlItem[i]).attr("class", "unselected");
    }
    $(this).attr("class", "selected");
    // Disable edit toolbar button
    selectedTopology = $(this).attr('id');
    topologyCategory = $(this).parent().parent().attr("id");
    var selectedTopologyName = $(this).children()[0].innerText;

    // Redraw the map
    $.get("TopologyValidation/GetSelectedDescription/" + selectedTopology, function (descriptionText) {
        $("#header").text(selectedTopologyName);
        $("#detail").text(descriptionText);
    });
    if (selectedTopology === "LinesMustBeLargerThanClusterTolerance" ||
        selectedTopology === "PolygonsMustBeLargerThanClusterTolerance") {
        $("#inputPanel").attr("class", "show");
    } else {
        $("#inputPanel").attr("class", "hidden");
    }
    xyzSource.setUrl(getRootPath() + 'TopologyValidation/RefreshSourceLayer/' + selectedTopology + '/{z}/{x}/{y}');
    map.setView(new ol.View({
        center: ol.proj.transform([parseFloat(center[1]), parseFloat(center[0])], 'EPSG:4326', 'EPSG:3857'),
        zoom: zoom
    }));
});

// Switch the control bar item.
$(".leftControlBarItemTitle").click(function () {
    if ($(this).hasClass("expanding")) {
        $(this).parent().parent().find("ul").hide();
        $(this).removeClass("expanding");
    } else {
        $(this).parent().parent().find("ul").hide();
        $(this).parent().find("ul").show();
        $(this).parent().parent().find(".expanding").removeClass("expanding");
        $(this).addClass("expanding");
    }
});

// Stop the click bubble event to avoid panel collapse when click on the category item.
$(".leftControlBarItemTitle").each(function () {
    this.onclick = function (e) {
        if (e && e.stopPropagation) {
            e.stopPropagation();
        } else {
            e = window.event;
            e.cancelBubble = true;
        }
    };
});

// Perform verification.
$("#btnValidate").click(function () {
    var clusterTolerance;
    if (selectedTopology === "LinesMustBeLargerThanClusterTolerance" ||
        selectedTopology === "PolygonsMustBeLargerThanClusterTolerance") {
        clusterTolerance = inputClusterTolerance.value;
    }
    xyzSource.setUrl(getRootPath() + "TopologyValidation/" + topologyCategory + "/" + selectedTopology + '/' + clusterTolerance + '/{z}/{x}/{y}');
    map.setView(new ol.View({
        center: ol.proj.transform([parseFloat(center[1]), parseFloat(center[0])], 'EPSG:4326', 'EPSG:3857'),
        zoom: zoom
    }));
});

//Show or hide description.
$("#descriptionExpandOrCollapseButton").click(function () {
    if ($(this).hasClass("validationDescriptionExpand")) {
        $(this).removeClass('validationDescriptionExpand');
        $(this).addClass("validationDescriptionCollapse");
        $("#validationDescription").hide();
    } else {
        $(this).removeClass('validationDescriptionCollapse');
        $(this).addClass("validationDescriptionExpand");
        $("#validationDescription").show();
    }
});

// Show or hide left navigation panel.
$("html").click(function () {
    $('#leftControlBar').animate({
        'left': -$('#leftControlBar').width() + 'px'
    });
});