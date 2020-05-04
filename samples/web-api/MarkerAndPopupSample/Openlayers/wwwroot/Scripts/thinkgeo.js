var thinkgeoApiKey = 'PIbGd76RyHKod99KptWTeb-Jg9JUPEPUBFD3SZJYLDE~';

// Initialize the map.
var map = new ol.Map({
    interactions: ol.interaction.defaults().extend([new app.Drag()]),
    target: 'map',
	renderer: 'webgl',
    loadTilesWhileAnimating: true,
    loadTilesWhileInteracting: true,
    controls: ol.control.defaults({ attribution: false, rotate: false }).extend(
        [new ol.control.Attribution({
            collapsible: false
        })]),
    view: new ol.View({
        center: ol.proj.transform([-96.8099904169396, 33.1282951274428], 'EPSG:4326', 'EPSG:3857'),
        zoom: 17
    })
});

// Add ThinkgeoVectorTileLayer as the map's background layer.
map.addLayer(new ol.mapsuite.VectorTileLayer('https://cdn.thinkgeo.com/worldstreets-styles/3.0.0/light.json', {
    apiKey: thinkgeoApiKey,
    layerName: 'light'
}));

// Add image buttons for layers, help etc.
var imgControls = new app.ImagesControl({
    imgs: [
        {
            id: 'layerOptions',
            src: 'Images/layers.png',
            title: 'Show layers',
            callback: function () {
                $('#leftPanel').animate({
                    'left': '0px'
                });
            }
        },
        {
            id: 'btnInfo',
            src: 'Images/info.png',
            title: 'Show help',
            callback: function () { window.open('http://wiki.thinkgeo.com/wiki/map_suite_webapi_markers_and_popups', '_blank'); }
        }
    ]
});
map.addControl(imgControls);

//Added marker 
var iconFeature = new ol.Feature({
    geometry: new ol.geom.Point(ol.proj.transform([-96.8099871260614, 33.1283121091316],
        'EPSG:4326', 'EPSG:3857')),
    name: 'ThinkGeo Headquarters <a href="http://www.thinkgeo.com" target="_blank">http://www.thinkgeo.com</a>',
});

var iconStyle = new ol.style.Style({
    image: new ol.style.Icon(({
        anchor: [0.5, 1],
        opacity: 0.75,
        src: 'Images/map_marker.png'
    }))
});

iconFeature.setStyle(iconStyle);

var vectorSource = new ol.source.Vector({
    features: [iconFeature]
});

var vectorLayer = new ol.layer.Vector({
    source: vectorSource
});

map.addLayer(vectorLayer);

//Added popup
var container = document.getElementById('popup');
var content = document.getElementById('popup-content');
var closer = document.getElementById('popup-closer');

var popupOverlay = new ol.Overlay(({
    element: container,
    autoPan: true,
    autoPanAnimation: {
        duration: 250
    }
}));

closer.onclick = function () {
    $('#popup').slideToggle("fast");
    closer.blur();
    return false;
};

map.addOverlay(popupOverlay);

//Added marker with popup 
var marker = new ol.Feature({
    geometry: new ol.geom.Point(ol.proj.transform([-96.8099871260614, 33.1283121091316],
        'EPSG:4326', 'EPSG:3857')),
    name: 'ThinkGeo Headquarters <a href="http://www.thinkgeo.com" target="_blank">http://www.thinkgeo.com</a>',
});

var iconStyleForMarkerWithPopup = new ol.style.Style({
    image: new ol.style.Icon(({
        anchor: [0.5, 1],
        opacity: 0.75,
        src: 'Images/marker.png'
    }))
});

marker.setStyle(iconStyleForMarkerWithPopup);

var vectorSourceForMarkerWithPopup = new ol.source.Vector({
    features: [marker]
});

var vectorLayerForMarkerWithPopup = new ol.layer.Vector({
    source: vectorSourceForMarkerWithPopup
});
vectorLayerForMarkerWithPopup.setVisible(false);
map.addLayer(vectorLayerForMarkerWithPopup);

var selected = 'Marker';
map.on('click', function (evt) {
    var coordinate = evt.coordinate;
    var markerFeature = map.forEachFeatureAtPixel(evt.pixel, function (feature) { return feature; });

    if (selected === 'Marker') {
        if (!markerFeature) { iconFeature.setGeometry(new ol.geom.Point(coordinate)); }
    }
    else {
        if (markerFeature) {
            if ($('#popup').css('display') === "none") {
                coordinate = markerFeature.getGeometry().getCoordinates();
                updatePopupContent(coordinate);
            }
            $('#popup').slideToggle("fast");
        } else {
            $('#popup').css('display', 'block');
            popupOverlay.setPosition(coordinate);
            marker.setGeometry(new ol.geom.Point(coordinate));

            updatePopupContent(coordinate);
        }
    }
});

function updatePopupContent(coordinate) {
    var latlng = ol.proj.transform(coordinate, 'EPSG:3857', 'EPSG:4326');
    var longtitude = latlng[0].toFixed(7);
    var latitude = latlng[1].toFixed(7);
    content.innerHTML = '<p>Your marker is at:</p><p>Latitude: ' + latitude + '<br/>Longtitude:' + longtitude + '</p>';
}

$('#leftPanelOptions div').bind('click', function () {
    var options = $('#leftPanelOptions div');
    for (var i = 0; i < options.length; i++) {
        $(options[i]).attr('class', 'unselected');
    }
    $(this).attr('class', 'selected');

    //Refresh map extent
    map.setView(new ol.View({
        center: ol.proj.transform([-96.8099904169396, 33.1282951274428], 'EPSG:4326', 'EPSG:3857'),
        zoom: 17
    }));
    //Refresh the descrition
    $('#description').text($(this).attr('description'));

    selected = $(this).attr('id');
    if (selected === 'Marker') {
        $('#popup').css('display', 'none');
        popupOverlay.setPosition(undefined);
        vectorLayerForMarkerWithPopup.setVisible(false);

        vectorLayer.setVisible(true);
        iconFeature.setGeometry(new ol.geom.Point([-10776836.6105256, 3912344.07403825]));
    }
    else {
        vectorLayer.setVisible(false);

        vectorLayerForMarkerWithPopup.setVisible(true);
        marker.setGeometry(new ol.geom.Point([-10776836.6105256, 3912344.07403825]));
        $('#popup').css('display', 'block');
        popupOverlay.setOffset([0, -41]);
        content.innerHTML = marker.get('name');
        popupOverlay.setPosition([-10776836.6105256, 3912344.07403825]);
    }
});

$('html').click(function () {
    $('#leftPanel').animate({
        'left': -$('#leftPanel').width() + 'px'
    });
});