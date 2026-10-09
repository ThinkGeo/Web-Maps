const flights = new Map();
const earthRadiusMeters = 6378137;

function lengthInMeters(a, b) {
    const radians = Math.PI / 180;
    const dLat = (b[1] - a[1]) * radians;
    const dLon = (b[0] - a[0]) * radians;
    const latA = a[1] * radians;
    const latB = b[1] * radians;
    const haversine = Math.sin(dLat / 2) ** 2 + Math.cos(latA) * Math.cos(latB) * Math.sin(dLon / 2) ** 2;
    return 2 * earthRadiusMeters * Math.asin(Math.min(1, Math.sqrt(haversine)));
}

function bearing(a, b) {
    const radians = Math.PI / 180;
    const latA = a[1] * radians;
    const latB = b[1] * radians;
    const dLon = (b[0] - a[0]) * radians;
    const y = Math.sin(dLon) * Math.cos(latB);
    const x = Math.cos(latA) * Math.sin(latB) - Math.sin(latA) * Math.cos(latB) * Math.cos(dLon);
    return (Math.atan2(y, x) / radians + 360) % 360;
}

function positionAtDistance(state) {
    let segment = state.cumulative.length - 2;
    for (let index = 0; index < state.cumulative.length - 1; index++) {
        if (state.distance <= state.cumulative[index + 1]) {
            segment = index;
            break;
        }
    }

    const start = state.points[segment];
    const end = state.points[segment + 1];
    const segmentLength = state.cumulative[segment + 1] - state.cumulative[segment];
    const ratio = segmentLength ? (state.distance - state.cumulative[segment]) / segmentLength : 0;
    return {
        lon: start[0] + (end[0] - start[0]) * ratio,
        lat: start[1] + (end[1] - start[1]) * ratio,
        heading: bearing(start, end)
    };
}

function update(state, forceHud = false) {
    const position = positionAtDistance(state);
    state.marker.setLngLat([position.lon, position.lat]);
    state.marker.setRotation(position.heading);

    const now = performance.now();
    if (!forceHud && now - state.lastHud < 100) return;
    state.lastHud = now;

    const longitude = ((position.lon + 180) % 360 + 360) % 360 - 180;
    const positionElement = document.getElementById("dateline-position");
    const progressElement = document.getElementById("dateline-progress");
    if (positionElement) {
        const hemisphere = longitude < 0 ? "W" : "E";
        positionElement.textContent = `lon ${position.lon.toFixed(2)}  (${Math.abs(longitude).toFixed(2)} ${hemisphere})\nlat  ${position.lat.toFixed(2)}  hdg ${Math.round(position.heading)}`;
    }
    if (progressElement) {
        progressElement.textContent = `${Math.round(state.distance / 1000).toLocaleString()} of ${Math.round(state.total / 1000).toLocaleString()} km (${Math.round(state.distance / state.total * 100)}%)`;
    }
}

function frame(state, now) {
    if (!state.running) return;
    state.distance = Math.min(state.total, state.startDistance + (now - state.startedAt) * state.total / state.durationMs);
    update(state);
    if (state.distance >= state.total) {
        state.running = false;
        update(state, true);
        state.dotNetReference.invokeMethodAsync("FlightFinished").catch(() => {});
    } else {
        state.frameId = requestAnimationFrame(time => frame(state, time));
    }
}

export function initializeFlight(mapId, points, imageUrl, dotNetReference, durationMs) {
    disposeFlight(mapId);
    const map = window.blazorObjects?.[mapId]?.map;
    if (!map || !window.maplibregl?.Marker || points?.length < 2) return false;

    const cumulative = [0];
    for (let index = 1; index < points.length; index++) {
        cumulative.push(cumulative[index - 1] + lengthInMeters(points[index - 1], points[index]));
    }
    const total = cumulative[cumulative.length - 1];
    if (!total) return false;

    const element = document.createElement("img");
    element.src = imageUrl;
    element.width = 54;
    element.height = 54;
    element.alt = "Flight from Los Angeles to Shanghai";
    element.style.pointerEvents = "none";
    const marker = new window.maplibregl.Marker({
        element,
        anchor: "center",
        rotationAlignment: "map",
        pitchAlignment: "map"
    }).setLngLat(points[0]).addTo(map);

    const state = {
        points, cumulative, total, marker, dotNetReference, durationMs,
        distance: 0, startDistance: 0, startedAt: 0, lastHud: 0,
        running: false, frameId: 0
    };
    flights.set(mapId, state);
    update(state, true);
    return true;
}

export function toggleFlight(mapId) {
    const state = flights.get(mapId);
    if (!state) return false;
    if (state.running) {
        state.running = false;
        cancelAnimationFrame(state.frameId);
        return false;
    }
    if (state.distance >= state.total) state.distance = 0;
    state.startDistance = state.distance;
    state.startedAt = performance.now();
    state.running = true;
    state.frameId = requestAnimationFrame(time => frame(state, time));
    return true;
}

export function resetFlight(mapId) {
    const state = flights.get(mapId);
    if (!state) return;
    state.running = false;
    cancelAnimationFrame(state.frameId);
    state.distance = 0;
    update(state, true);
}

export function disposeFlight(mapId) {
    const state = flights.get(mapId);
    if (!state) return;
    state.running = false;
    cancelAnimationFrame(state.frameId);
    state.marker.remove();
    flights.delete(mapId);
}
