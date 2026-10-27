export function waitForSource(indicator, mapId, sourceId) {
    return new Promise(resolve => {
        const timer = setInterval(() => {
            if (!indicator.isConnected) {
                clearInterval(timer);
                resolve();
                return;
            }
            const wrapper = window.blazorObjects?.[mapId];
            const map = wrapper?.map;
            if (!map?.getSource(sourceId)) return;

            // The map can exist before the server has returned any vector tiles.
            // Wait for MapLibre's rendered, loaded state, including this source.
            if (map.loaded() && map.isSourceLoaded(sourceId)) {
                clearInterval(timer);
                resolve();
            }
        }, 150);
    });
}
