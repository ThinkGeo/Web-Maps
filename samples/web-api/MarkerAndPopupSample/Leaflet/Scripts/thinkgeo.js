var map = L.map('map', { zoomAnimation: false }).setView([33.1282851274428, -96.8099704169396], 17);

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

L.imageButtons({
    imgs: [
        {
            src: 'Images/layers.png',
            id: 'layerOptions',
            title: 'Show layers',
            callback: function () {
                $('#leftPanel').animate({
                    'left': '0px'
                });
            }
        },
        {
            src: 'Images/info.png',
            id: 'btnInfo',
            title: 'Show help',
            callback: function () { window.open('http://wiki.thinkgeo.com/wiki/map_suite_webapi_markers_and_popups', '_blank'); }
        }
    ]
}).addTo(map);

var markerIcon = L.icon({
    iconUrl: 'Images/map_marker.png',
    iconSize: [21, 30],
    iconAnchor: [10, 30]
});
var marker = L.marker([33.1282951274428, -96.8099904169396], { 'draggable': true });
marker.setIcon(markerIcon);
marker.addTo(map);
marker.on('drag', function () {
    if (!navigator.userAgent.match(/(iPhone|iPod|Android|ios)/i)) {
        map.off('click', onMapClick);
    }
});
marker.on('click', function () {
    if (!navigator.userAgent.match(/(iPhone|iPod|Android|ios)/i)) {
        map.off('click', onMapClick);
    }
});
marker.on('mouseout', function () {
    map.on('click', onMapClick);
});


var markerWithPopup = L.marker([33.1282951274428, -96.8099904169396], { 'draggable': true });
markerWithPopup.bindPopup('ThinkGeo Headquarters.<br/><a href="http://www.thinkgeo.com" target="_blank">http://www.thinkgeo.com</a>');
markerWithPopup.openPopup();
markerWithPopup.on('drag', function () {
    if (!navigator.userAgent.match(/(iPhone|iPod|Android|ios)/i)) {
        map.off('click', onMapClick);
    }
});
markerWithPopup.on('click', function () {
    if (!navigator.userAgent.match(/(iPhone|iPod|Android|ios)/i)) {
        map.off('click', onMapClick);
    }
});

markerWithPopup.on('mouseout', function () {
    map.on('click', onMapClick);
});

markerWithPopup.on('dragend', function () {
    var latlng = markerWithPopup.getLatLng();
    var latitude = latlng.lat.toFixed(7);
    var longtitude = latlng.lng.toFixed(7);
    markerWithPopup.setPopupContent('<p>Your marker is at:</p><p>Latitude: ' + latitude + '<br/>Longtitude:' + longtitude +
        '</p>');
});

var selected = 'Marker';
function onMapClick(e) {
    if (selected == 'Marker') {
        marker.setLatLng(e.latlng);
    }
    else {
        markerWithPopup.setLatLng(e.latlng);

        var latitude = e.latlng.lat.toFixed(7);
        var longtitude = e.latlng.lng.toFixed(7);
        markerWithPopup.setPopupContent('<p>Your marker is at:</p><p>Latitude: ' + latitude + '<br/>Longtitude:' + longtitude +
            '</p>');
        markerWithPopup.openPopup();
    }
}
map.addEventListener('click', onMapClick, true);

$('#leftPanelOptions div').bind('click', function () {
    var options = $('#leftPanelOptions div');
    for (var i = 0; i < options.length; i++) {
        $(options[i]).attr('class', 'unselected');
    }
    $(this).attr('class', 'selected');
    //Refresh map extent
    map.setView([33.1282951274428, -96.8099904169396], 17);
    //Refresh the descrition
    $('#description').text($(this).attr('description'));

    //Refresh marker and popup
    selected = $(this).attr('id');
    if (selected == 'Marker') {
        markerWithPopup.remove();

        marker.addTo(map);
        marker.setLatLng([33.1282951274428, -96.8099904169396]);
    }
    else {
        marker.remove();

        markerWithPopup.addTo(map);
        markerWithPopup.setPopupContent('ThinkGeo Headquarters.<br/><a href="http://www.thinkgeo.com" target="_blank">http://www.thinkgeo.com</a>');
        markerWithPopup.setLatLng([33.1282951274428, -96.8099904169396]);
        markerWithPopup.openPopup();
    }
});

$('html').click(function () {
    $('#leftPanel').animate({
        'left': -$('#leftPanel').width() + 'px'
    });
});