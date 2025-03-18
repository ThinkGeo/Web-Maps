window.DisableMouseWheel = function (mapId) {
    // Retrieve the ThinkGeoMap from window.blazorObjects
    let map = window.blazorObjects[mapId];
    if (!map) {
        console.warn("No map found for id: " + mapId);
        return;
    }

    // Remove the MouseWheelZoom interaction
    map.getInteractions().forEach(function (interaction) {
        if (interaction instanceof ol.interaction.MouseWheelZoom) {
            map.removeInteraction(interaction);
        }
    });

    console.log("Mouse wheel zoom disabled for map:", mapId);
};
