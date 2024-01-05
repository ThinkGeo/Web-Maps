// Create the map.
var map = L.map('map').setView([41.8781, -87.6298], 11);


getRootPath = function () {
    var pathArray = location.pathname.split('/');
    var appPath = "/";
    for (var i = 1; i < pathArray.length - 1; i++) {
        appPath += pathArray[i] + "/";
    }

    return appPath === "/" ? "" : appPath;
};

// Add ThinkGeoCloudMaps as the map's background. 
var thinkgeoCloudMapsLayer = L.tileLayer('https://{s}.thinkgeo.com/api/v1/maps/raster/light/x1/3857/256/{z}/{x}/{y}.png?apikey=PIbGd76RyHKod99KptWTeb-Jg9JUPEPUBFD3SZJYLDE~', {
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
            context.strokeText("Backgrounds for this sample are", 256, 100);
            context.strokeText("powered by ThinkGeo Cloud Maps and", 256, 120);
            context.strokeText("require an API Key. This was sent", 256, 140);
            context.strokeText("to you via email when you signed up", 256, 160);
            context.strokeText("with ThinkGeo, or you can register", 256, 180);
            context.strokeText("now at https://cloud.thinkgeo.com", 256, 200);
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

var shapeLayer = L.dynamicLayer(L.Util.getRootPath() + '/DisplayNauticalChart/tile/{z}/{x}/{y}', { unloadInvisibleTiles: true, reuseTiles: false }).addTo(map);
// Show or hide loading image.
shapeLayer.on('loading', function () {
    $('#loadingImage').show();
});
shapeLayer.on('load', function () {
    $('#loadingImage').hide();
});




