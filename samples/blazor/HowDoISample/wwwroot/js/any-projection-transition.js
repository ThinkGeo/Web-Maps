const transitions = new Map();

export function beginTileTransition(mapId, timeoutMs) {
    const map = window.blazorObjects?.[mapId];
    if (!map || typeof map.on !== "function") return false;

    transitions.get(mapId)?.finish(false);

    let resolveTransition;
    const promise = new Promise(resolve => { resolveTransition = resolve; });
    const sources = map.getLayers().getArray()
        .map(layer => layer.getSource?.())
        .filter(source => source && typeof source.on === "function");
    let pending = 0;
    let started = false;
    let finished = false;
    let settleTimer;
    let timeoutTimer;

    const cleanup = () => {
        map.un("rendercomplete", onComplete);
        for (const source of sources) {
            source.un("tileloadstart", onStart);
            source.un("tileloadend", onEnd);
            source.un("tileloaderror", onEnd);
        }
        clearTimeout(settleTimer);
        clearTimeout(timeoutTimer);
        transitions.delete(mapId);
    };
    const finish = ready => {
        if (finished) return;
        finished = true;
        cleanup();
        resolveTransition(ready);
    };
    const scheduleSettled = () => {
        clearTimeout(settleTimer);
        if (started && pending === 0) {
            // Leave one quiet interval for the next frame to request more tiles.
            settleTimer = setTimeout(() => finish(true), 500);
        }
    };
    const onStart = () => {
        pending++;
        clearTimeout(settleTimer);
    };
    const onEnd = () => {
        pending = Math.max(0, pending - 1);
        scheduleSettled();
    };
    const onComplete = () => {
        if (started) finish(true);
    };

    map.on("rendercomplete", onComplete);
    for (const source of sources) {
        source.on("tileloadstart", onStart);
        source.on("tileloadend", onEnd);
        source.on("tileloaderror", onEnd);
    }
    timeoutTimer = setTimeout(() => finish(false), timeoutMs);
    transitions.set(mapId, { promise, finish, start: () => {
        started = true;
        scheduleSettled();
        map.render();
    } });
    return true;
}

export function waitForTileTransition(mapId) {
    const transition = transitions.get(mapId);
    if (!transition) return Promise.resolve(false);
    transition.start();
    return transition.promise;
}
