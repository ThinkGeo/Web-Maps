export function waitForSource(indicator, mapId, sourceId, timeoutMs, waitForVisibleFeature = false) {
    return new Promise(resolve => {
        const started = Date.now();
        let lastFeatureCheck = 0;
        const timer = setInterval(() => {
            if (!indicator.isConnected) {
                clearInterval(timer);
                resolve(false);
                return;
            }
            if (timeoutMs && Date.now() - started >= timeoutMs) {
                clearInterval(timer);
                resolve(false);
                return;
            }
            const wrapper = window.blazorObjects?.[mapId];
            const map = wrapper?.map;
            if (!map?.getSource(sourceId)) return;

            if (waitForVisibleFeature && Date.now() - lastFeatureCheck >= 1000) {
                lastFeatureCheck = Date.now();
                const layers = (map.getStyle()?.layers ?? [])
                    .filter(layer => layer.source === sourceId)
                    .map(layer => layer.id);
                if (layers.length) {
                    try {
                        if (map.queryRenderedFeatures(undefined, { layers }).length) {
                            clearInterval(timer);
                            resolve(true);
                            return;
                        }
                    } catch {
                        // A style replacement can briefly invalidate the layer IDs.
                    }
                }
            }

            // The map can exist before the server has returned any vector tiles.
            // Wait for MapLibre's rendered, loaded state, including this source.
            if (map.loaded() && map.isSourceLoaded(sourceId)) {
                clearInterval(timer);
                resolve(true);
            }
        }, 150);
    });
}
