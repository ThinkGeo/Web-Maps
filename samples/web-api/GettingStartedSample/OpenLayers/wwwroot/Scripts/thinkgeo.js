var thinkgeoApiKey = 'PIbGd76RyHKod99KptWTeb-Jg9JUPEPUBFD3SZJYLDE~';

let map;
//Create the openlayers map.
let initializeMap = function () {
    map = new ol.Map({
        target: 'map',
        renderer: 'webgl',
        loadTilesWhileAnimating: true,
        loadTilesWhileInteracting: true,

        controls: [
            new ol.control.Zoom(),
            new ol.control.Attribution({
                collapsible: false
            })
        ],
        layers: [
            new ol.mapsuite.VectorTileLayer('https://cdn.thinkgeo.com/worldstreets-styles/3.0.0/light.json', {
                apiKey: thinkgeoApiKey,
                layerName: 'light'
            })
        ],
        // set the default extent
        view: new ol.View({
            center: [-10777396.499651, 4821690.0604384],
            zoom: 4
        })
    });

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
        var scale = app.getMapScale(map);
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

        map.getView().fit(
            accuracyFeature.getGeometry().getExtent(),
            {
                maxZoom: 16,
                size: map.getSize()
            }
        );
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
};
 

// The loaded Icon Fonts will be used to render POI icons on top of the map's 
// background layer.  We'll initalize the map only once the font has been 
// downloaded.  For more info, see our wiki: 
// https://wiki.thinkgeo.com/wiki/thinkgeo_iconfonts 
WebFont.load({
    custom: {
        families: ["vectormap-icons"],
        urls: ["https://cdn.thinkgeo.com/vectormap-icons/3.0.0/vectormap-icons.css"],
        testStrings: {
            'vectormap-icons': '\ue001'
        }
    },
    // The "active" property defines a function to call when the font has
    // finished downloading.  Here, we'll call our initializeMap method.
    active: initializeMap
});
