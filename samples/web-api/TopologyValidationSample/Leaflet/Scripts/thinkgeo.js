var zoom = 5;
var center = [40.14711, -95.81131];
var topologyCategory = 'LinesTopology';
var selectedTopology = 'LinesEndpointMustBeCoveredByPoints';

LoadIcons();

// Create the map.
var map = L.map('map').setView(center, zoom);

// Add image buttons for layers and help.
L.imageButtons({
    imgs: [
        {
            src: 'Images/LeftControlBar.png',
            id: 'topologyOptions',
            title: 'Show Topology Control Bar',
            callback: function () {
                $('#leftControlBar').animate({
                    'left': '0px'
                });
            }
        },
        {
            src: 'Images/info.png',
            id: 'btnInfo',
            title: 'Show help',
            callback: function () { window.open('http://wiki.thinkgeo.com/wiki/map_suite_webapi_topology', '_blank'); }
        }
    ]
}).addTo(map);

// Add ThinkGeoCloudMaps as the map's background layer.
var thinkgeoCloudMapsLayer = L.tileLayer('https://{s}.thinkgeo.com/api/v1/maps/raster/light/x1/3857/256/{z}/{x}/{y}.png?apikey=ThinkGeo Cloud API Key', {
    subdomains: ['cloud1', 'cloud2', 'cloud3', 'cloud4', 'cloud5', 'cloud6'],
    layers: 'ThinkGeoCloudMaps',
    format: 'image/png',
    styles: 'Light',
    version: '1.1.1'
});

// --------------------------------------------------------------------------------------
// Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
// an API Key. The following function is just for reminding you to input the key. 
// Feel free to remove this function after the key was input. 
// --------------------------------------------------------------------------------------
thinkgeoCloudMapsLayer.on('tileloadstart', function (e) {
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
});
thinkgeoCloudMapsLayer.addTo(map);

function LoadIcons() {
    var leftControlBarItems = $(".leftControlBarItem div");
    for (var i = 0; i < leftControlBarItems.length; i++) {
        $(leftControlBarItems[i]).attr("style", "background-image:url(../Images/TopologyIcons/" + leftControlBarItems[i].id + ".png);");
    }
}

// Add dynamic tile layer to the map, it can be refreshed dynamically.
var layer = L.dynamicLayer(L.Util.getRootPath() + 'TopologyValidation/RefreshSourceLayer/' + selectedTopology + '/{z}/{x}/{y}', { unloadInvisibleTiles: true, reuseTiles: false }).addTo(map);
// Show or hide loading image.
layer.on('loading', function () { $('#loadingImage').show(); });
layer.on('load', function () { $('#loadingImage').hide(); });

// Refresh topology rule sample by selecting item.
$('.leftControlBarItem div').click(function () {
    var leftControlBarItems = $(".leftControlBarItem div");
    for (var i = 0; i < leftControlBarItems.length; i++) {
        $(leftControlBarItems[i]).attr("class", "unselected");
    }
    $(this).attr("class", "selected");
    // Disable edit toolbar button
    selectedTopology = $(this).attr('id');
    topologyCategory = $(this).parent().parent().attr("id");
    var selectedTopologyName = $(this).children()[0].innerText;

    // Redraw the map, if no osm background layer, then add it.
    if (!map.hasLayer(thinkgeoCloudMapsLayer)) {
        map.addLayer(thinkgeoCloudMapsLayer);
        thinkgeoCloudMapsLayer.bringToBack();
    }

    // Update description for selected topology.
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
    layer.setUrl(L.Util.getRootPath() + "TopologyValidation/RefreshSourceLayer/" + selectedTopology + '/{z}/{x}/{y}');
    map.setView(L.latLng(center[0], center[1]), zoom);
    layer.redraw();
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

// Perform topology validation.
$("#btnValidate").click(function () {
    var clusterTolerance;
    if (selectedTopology === "LinesMustBeLargerThanClusterTolerance" ||
        selectedTopology === "PolygonsMustBeLargerThanClusterTolerance") {
        clusterTolerance = inputClusterTolerance.value;
    }
    layer.setUrl(L.Util.getRootPath() + "TopologyValidation/" + topologyCategory + "/" + selectedTopology + '/' + clusterTolerance + '/{z}/{x}/{y}');
    map.setView(L.latLng(center[0], center[1]), zoom);
    layer.redraw();
});

// Show or hide topology validation description.
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
    $('#leftControlBar').animate({ 'left': -$('#leftControlBar').width() + 'px' });
});