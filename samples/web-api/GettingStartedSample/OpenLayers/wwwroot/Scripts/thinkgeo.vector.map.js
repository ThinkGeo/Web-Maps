/*===========================================================================*/
// Vector Tiles
// Sample map by ThinkGeo
// 
//   1. ThinkGeo Cloud API Key
//   2. Map Control Setup
//   3. Changing the Map Style
//   4. ThinkGeo Map Icon Fonts
//   5. Tile Loading Event Handlers
/*===========================================================================*/


/*---------------------------------------------*/
// 1. ThinkGeo Cloud API Key
/*---------------------------------------------*/

// First, let's define our ThinkGeo Cloud API key, which we'll use to
// authenticate our requests to the ThinkGeo Cloud API.  Each API key can be
// restricted for use only from a given web domain or IP address.  To create your
// own API key, you'll need to sign up for a ThinkGeo Cloud account at
// https://cloud.thinkgeo.com.
const apiKey = 'PIbGd76RyHKod99KptWTeb-Jg9JUPEPUBFD3SZJYLDE~';


/*---------------------------------------------*/
// 2. Map Control Setup
/*---------------------------------------------*/

// Now, we'll create the two different styles of layers for our map. 
// The two styles of layers use ThinkGeo Cloud Maps Vector Tile service to 
// display the detailed light style street map and dark style street map.
// For more info, see our wiki: 
// https://wiki.thinkgeo.com/wiki/thinkgeo_cloud_maps_vector_tiles
let light = new ol.mapsuite.VectorTileLayer('https://cdn.thinkgeo.com/worldstreets-styles/3.0.0/light.json', {
    apiKey: apiKey,
    layerName: 'light'
});

let dark = new ol.mapsuite.VectorTileLayer('https://cdn.thinkgeo.com/worldstreets-styles/3.0.0/dark.json', {
    apiKey: apiKey,
    visible: false,
    layerName: 'dark'
});

// This function will create and initialize our interactive map.
// We'll call it later when our POI icon font has been fully downloaded,
// which ensures that the POI icons display as intended.
let map;
let initializeMap = () => {
    map = new ol.Map({
        renderer: 'webgl',
        loadTilesWhileAnimating: true,
        loadTilesWhileInteracting: true,

        // Add our two previously-defined ThinkGeo Cloud Vector Tile layers to the map.
        layers: [light, dark],
        // States that the HTML tag with id="map" should serve as the container for our map.
        target: 'map',
        // Create a default view for the map when it starts up.
        view: new ol.View({

            // Center the map on the United States and start at zoom level 3.
            center: ol.proj.fromLonLat([-96.79620, 32.79423]),
            maxResolution: 40075016.68557849 / 512,
            zoom: 3,
            minZoom: 2,
            maxZoom: 19
        })
    });

    // Add a button to the map that lets us toggle full-screen display mode.
    map.addControl(new ol.control.FullScreen());
}


/*---------------------------------------------*/
// 3. Changing the Map Style
/*---------------------------------------------*/

const changeLayer = (e) => {
    let layers = map.getLayers().getArray();
    for (let i = 0; i < layers.length; i++) {
        if (layers[i].get("layerName") == e.target.getAttribute("value")) {
            layers[i].setVisible(true);
        } else {
            layers[i].setVisible(false);
        }
    }
}

// When click the different styles button, render the relevant style map.
document.addEventListener('DOMContentLoaded', () => {
    document.getElementById('wrap').addEventListener('click', (e) => {
        const nodeList = document.querySelectorAll(".thumb");
        for (let node of nodeList) {
            node.style.borderColor = 'transparent';
        }
        if (e.target.nodeName == 'DIV') {
            e.target.style.borderColor = '#ffffff';
            changeLayer(e);
        }
    })
})


/*---------------------------------------------*/
// 4. ThinkGeo Map Icon Fonts
/*---------------------------------------------*/

// Finally, we'll load the Map Icon Fonts using ThinkGeo's WebFont loader. 
// The loaded Icon Fonts will be used to render POI icons on top of the map's 
// background layer.  We'll initalize the map only once the font has been 
// downloaded.  For more info, see our wiki: 
// https://wiki.thinkgeo.com/wiki/thinkgeo_iconfonts 
WebFont.load({
    custom: {
        families: ["vectormap-icons"],
        urls: ["https://cdn.thinkgeo.com/vectormap-icons/2.0.0/vectormap-icons.css"],
        testStrings: {
            'vectormap-icons': '\ue001'
        }
    },
    // The "active" property defines a function to call when the font has
    // finished downloading.  Here, we'll call our initializeMap method.
    active: initializeMap
});


/*---------------------------------------------*/
// 5. Tile Loading Event Handlers
/*---------------------------------------------*/

// These events allow you to perform custom actions when 
// a map tile encounters an error while loading.
const errorLoadingTile = () => {
    const errorModal = document.querySelector('#error-modal');
    if (errorModal.classList.contains('hide')) {
        // Show the error tips when Tile loaded error.
        errorModal.classList.remove('hide');
    }
}

const setLayerSourceEventHandlers = (layer) => {
    let layerSource = layer.getSource();
    layerSource.on('tileloaderror', function () {
        errorLoadingTile();
    });
}

setLayerSourceEventHandlers(light);
setLayerSourceEventHandlers(dark);

document.querySelector('#error-modal button').addEventListener('click', () => {
    document.querySelector('#error-modal').classList.add('hide');
})


// Add image buttons for locate, full extent and help.
var imgControls = new app.ImagesControl({
    imgs: [
        {
            src: 'Images/location-inactive.png',
            title: 'Locate',
            callback: function () {
                locate(this, 'location.png', 'location-inactive.png');
            }
        },
        {
            src: 'Images/full-extent.png',
            title: 'Zoom to full extent',
            callback: function () {
                map.setView(new ol.View({ center: [-4.9158, -4.2187], zoom: 2 }));
            }
        },
        {
            src: 'Images/info.png',
            title: 'Help',
            callback: function () { window.open('http://wiki.thinkgeo.com/wiki/Map_Suite_WebAPI_Edition_Samples_Getting_Started', '_blank'); }
        }
    ]
});
map.addControl(imgControls);

// Hook up the mouse-move event for showing current scale, zoomlevel and center point.
map.on('moveend', function (e) {
    var mapView = this.getView();
    var zoom = mapView.getZoom();
    var scale = app.getScale(zoom);
    var center = ol.proj.transform(mapView.getCenter(), 'EPSG:3857', 'EPSG:4326');

    $('#scale').text('Scale: 1:' + Math.round(scale).toLocaleString());
    $('#zoom').text('Zoom: ' + zoom);
    $('#lat').text('Latitude: ' + center[0].toPrecision(6));
    $('#lng').text('Longitude: ' + center[1].toPrecision(6));
});

// Create the popup based on the HTML elements.
var container = document.getElementById('popup');
var content = document.getElementById('popup-content');
var closer = document.getElementById('popup-closer');
// Hook up a click event to the "close" div for hiding the popup.
closer.onclick = function () {
    container.style.display = 'none';
    closer.blur();
    return false;
};
// Create an overlay to anchor the popup to the map.
var overlay = new ol.Overlay({
    element: container
});
map.addOverlay(overlay);

// Hook up the map.click event to show a popup with current latitude/longitude.
map.on('click', function (e) {
    var coordinate = e.coordinate;
    var center = ol.proj.transform(coordinate, 'EPSG:3857', 'EPSG:4326');

    overlay.setPosition(coordinate);
    content.innerHTML = 'Latitude:' + center[1].toFixed(4) + '<br/>Longitude:' + center[0].toFixed(4);
    container.style.display = 'block';
});

// Add context-menu to the map
var menuContainer = document.getElementById('contextMenu');
var menuContent = document.getElementById('contextMenu-content');
var menuOverlay = new ol.Overlay({
    element: menuContainer
});
map.addOverlay(menuOverlay);
map.getViewport().oncontextmenu = function (e) {
    e.preventDefault();

    var offsetx = e.clientX + 50;
    var offsety = e.clientY + 150;
    var menuPoint = map.getCoordinateFromPixel([offsetx, offsety]);
    menuOverlay.setPosition(menuPoint);
    menuContainer.style.display = 'block';
};
map.on('click', function () {
    menuContainer.style.display = "none";
});

// Get Geolocation.
var geolocation = new ol.Geolocation({
    projection: map.getView().getProjection(),
    tracking: false,
    trackingOptions: {
        maximumAge: 10000,
        enableHighAccuracy: true,
        timeout: 600000
    }
});

var source = new ol.source.Vector();
var vector = new ol.layer.Vector({
    source: source,
    style: new ol.style.Style({
        fill: new ol.style.Fill({
            color: 'rgba(19, 106, 236, .3)'
        }),
        stroke: new ol.style.Stroke({
            color: '#136AEC',
            width: 2
        }),
        image: new ol.style.Circle({
            radius: 7,
            fill: new ol.style.Fill({
                color: '#2A93EE'
            })
        })
    })
});
map.addLayer(vector);

// Listen to position changes and show it on the map
var accuracyFeature = new ol.Feature();
var positionFeature = new ol.Feature();
geolocation.on('change', function (evt) {
    var location = geolocation.getPosition();
    var accuracy = geolocation.getAccuracy();

    accuracyFeature = new ol.Feature(new ol.geom.Circle(location, accuracy));
    positionFeature = new ol.Feature(new ol.geom.Point(location));
    source.addFeatures([accuracyFeature, positionFeature]);

    map.getView().fitExtent(accuracyFeature.getGeometry().getExtent(), map.getSize());
});
geolocation.on('error', function (e) {
    $('#locateError').text('Failed to locate your position.');
});

// Change the style of "locate" icon on toolbar between active and deactive.
locate = function (locateImg, activeImg, deactiveImg) {
    if ($(locateImg).hasClass('active')) {
        geolocation.setTracking(false);
        source.clear();

        locateImg.src = 'Images/' + deactiveImg;
        $(locateImg).removeClass('active');
    }
    else {
        geolocation.setTracking(true); // start position tracking

        locateImg.src = 'Images/' + activeImg;
        $(locateImg).addClass('active');
    }
};

var centerHere = function () {
    map.getView().setCenter(menuOverlay.getPosition());

    menuContainer.style.display = "none";
};

var zoom = function (zoomnum) {
    map.getView().setZoom(zoomnum);
    map.getView().setCenter(menuOverlay.getPosition());

    menuContainer.style.display = "none";
};