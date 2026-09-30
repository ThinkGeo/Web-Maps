// What every sample page shares: a MapLibre map on a ThinkGeo Cloud basemap, the overlays the
// server serves, and a few small tools. A page lives at samples/<name>.html; the server's
// routes are one level up.

export const root = new URL('../', location.href).href;
export const apiKey = 'PIbGd76RyHKod99KptWTeb-Jg9JUPEPUBFD3SZJYLDE~';

const R = 6378137;
/** Web Mercator meters to [lng, lat]. The samples give their places in meters, as the SDK does. */
export const toLngLat = (x, y) => [x / R * 180 / Math.PI, (2 * Math.atan(Math.exp(y / R)) - Math.PI / 2) * 180 / Math.PI];
/** [lng, lat] to Web Mercator meters. */
export const toMeters = (lng, lat) => [lng * Math.PI / 180 * R, Math.log(Math.tan(Math.PI / 4 + lat * Math.PI / 360)) * R];
/** The ThinkGeo scale ladder: zoom level 1 is 1:590,591,790, the world in 256 pixels, halving per level. MapLibre's zoom 0 is that level 2. */
const scaleAtZoomZero = 295295895.35;
/** MapLibre's zoom for a map scale, and back; and the meters one pixel covers at a scale or a zoom. */
export const zoomForScale = (scale) => Math.log2(scaleAtZoomZero / scale);
export const scaleForZoom = (zoom) => scaleAtZoomZero / 2 ** zoom;
export const resolutionForScale = (scale) => scale * 0.0254 / 96;
export const resolutionForZoom = (zoom) => resolutionForScale(scaleForZoom(zoom));
export const zoomForResolution = (resolution) => zoomForScale(resolution * 96 / 0.0254);
/** A LngLatBounds as minx,miny,maxx,maxy in degrees, the form the server's routes take. */
export const bboxOf = (map) => { const b = map.getBounds(); return [b.getWest(), b.getSouth(), b.getEast(), b.getNorth()].map(v => v.toFixed(6)).join(','); };
/** A name as it appears in a route: lower case, words joined by dashes - the same rule the server uses. */
export const slug = (name) => name.toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/^-|-$/g, '');

/**
 * The map: a ThinkGeo Cloud vector basemap (light, dark, or none), centred in meters at a
 * ThinkGeo zoom level.
 * The cloud's tiles are asked for with a {key} placeholder in their address; the map fills it.
 */
export function createMap({ container = 'map', center = [-10777700, 3912400], zoom = 12, basemap = 'light', bearing = 0, pitch = 0, background = '#DDE8F0', maxZoom = 20, preserveDrawingBuffer = false } = {}) {
    // The zoom is a ThinkGeo zoom level, the ladder the other galleries count on; MapLibre's is one less.
    const style = basemap
        ? `https://cdn.thinkgeo.com/worldstreets-gl-styles/1.0.0/${basemap}.json`
        : { version: 8, glyphs: 'https://cdn.thinkgeo.com/glyphs/1.0.0/{fontstack}/{range}.pbf', sources: {}, layers: [{ id: 'background', type: 'background', paint: { 'background-color': background } }] };
    const map = new maplibregl.Map({
        container, style, center: toLngLat(center[0], center[1]), zoom: zoom - 1, bearing, pitch, maxZoom, attributionControl: false, preserveDrawingBuffer,
        transformRequest: (url) => ({ url: url.replace('{key}', apiKey).replace('%7Bkey%7D', apiKey) }),
    });
    map.addControl(new maplibregl.AttributionControl({ compact: true }));
    // The map's own loaded() drops back to false while tiles are in flight; this stays true.
    map.once('load', () => { map.tgLoaded = true; });
    window.map = map;   // handy in the console
    map.once('load', () => fallbackTextFonts(map));
    return map;
}

/** Resolves once the map's style is ready to take sources and layers. */
export const ready = (map) => map.tgLoaded || map.loaded() ? Promise.resolve() : new Promise(resolve => map.once('load', resolve));

// The glyph server has Noto Sans, Regular and Bold, and no other face; a style that names another
// stack, or none, would draw no label at all.
function fallbackTextFonts(map) {
    for (const layer of map.getStyle().layers) {
        if (layer.type !== 'symbol') continue;
        const font = map.getLayoutProperty(layer.id, 'text-font');
        if (font === undefined && map.getLayoutProperty(layer.id, 'text-field') !== undefined) {
            map.setLayoutProperty(layer.id, 'text-font', ['Noto Sans Regular']);
        }
    }
}

function fontOf(layer) {
    const layout = layer.layout || (layer.layout = {});
    if (layer.type !== 'symbol' || layout['text-field'] === undefined) return;
    const font = layout['text-font'];
    if (font === undefined) { layout['text-font'] = ['Noto Sans Regular']; return; }
    if (Array.isArray(font)) layout['text-font'] = font.map(name => typeof name === 'string' && !/^Noto Sans (Regular|Bold)$/.test(name) ? 'Noto Sans ' + (/bold|black|heavy|semibold/i.test(name) ? 'Bold' : 'Regular') : name);
}

/**
 * A whole style document made fit for the map: glyphs from ThinkGeo's server when it names
 * none, and every label in a face that server has.
 */
export function normalizeStyle(style) {
    style.version = style.version || 8;
    style.glyphs = 'https://cdn.thinkgeo.com/glyphs/1.0.0/{fontstack}/{range}.pbf';
    for (const layer of style.layers) fontOf(layer);
    return style;
}

/**
 * An overlay of vector tiles the server cuts (or reads from an archive) under tiles/<overlay>/,
 * drawn by the style layers given, which name their source-layer and leave the source to this.
 * Returns a handle that swaps the layers for new ones - the style editor's job.
 */
export function vectorOverlay(map, overlay, layers, { maxzoom, before, sourceId, tiles } = {}) {
    const source = sourceId || overlay;
    const spec = { type: 'vector', tiles: tiles || [`${root}tiles/${overlay}/{z}/{x}/{y}.mvt`], minzoom: 0 };
    if (maxzoom !== undefined) spec.maxzoom = maxzoom;
    map.addSource(source, spec);
    let ids = [];
    const put = (newLayers) => {
        for (const id of ids) if (map.getLayer(id)) map.removeLayer(id);
        ids = [];
        newLayers.forEach((layer, i) => {
            const copy = JSON.parse(JSON.stringify(layer));
            // A layer keeps its own id unless the map already has one by that name.
            copy.id = copy.id && !map.getLayer(copy.id) ? copy.id : `${overlay}-${i}`;
            if (copy.type !== 'background') copy.source = source;
            fontOf(copy);
            map.addLayer(copy, before);
            ids.push(copy.id);
        });
    };
    put(layers);
    return { source, setLayers: put, remove: () => { put([]); if (map.getSource(source)) map.removeSource(source); } };
}

/** An overlay of raster tiles drawn by classic layers on the server, under raster/<overlay>/. */
export function rasterOverlay(map, overlay, { opacity = 1, before, tileSize = 512 } = {}) {
    map.addSource(overlay, { type: 'raster', tiles: [`${root}raster/${overlay}/{z}/{x}/{y}.png`], tileSize });
    map.addLayer({ id: overlay, type: 'raster', source: overlay, paint: { 'raster-opacity': opacity } }, before);
    return { remove: () => { if (map.getLayer(overlay)) map.removeLayer(overlay); if (map.getSource(overlay)) map.removeSource(overlay); } };
}

/**
 * The same layers drawn on the server. The layers, with any image they name taken from the map,
 * are posted to samples/tiles/style; the server draws that document over the overlay's sources
 * into raster tiles under an address of its own, which the reply names, and those tiles are laid
 * on the map. The handle removes them, and knows the address.
 */
export async function serverOverlay(map, overlay, layers, { before } = {}) {
    const images = {};
    const styled = layers.map(layer => { const copy = JSON.parse(JSON.stringify(layer)); fontOf(copy); return copy; });
    for (const layer of styled) {
        for (const key of ['icon-image', 'fill-pattern', 'line-pattern']) {
            const name = (layer.layout && layer.layout[key]) || (layer.paint && layer.paint[key]);
            if (typeof name === 'string' && !images[name] && map.hasImage(name)) images[name] = pictureOf(map.getImage(name));
        }
    }
    const r = await post('samples/tiles/style', { source: overlay, layers: styled, images });
    return { ...rasterOverlay(map, r.id, { before }), id: r.id };
}

// A map image as a PNG data URL, for the server to draw with.
function pictureOf(image) {
    const { width, height, data } = image.data;
    const canvas = document.createElement('canvas');
    canvas.width = width; canvas.height = height;
    canvas.getContext('2d').putImageData(new ImageData(new Uint8ClampedArray(data.buffer, data.byteOffset, data.length), width, height), 0, 0);
    return canvas.toDataURL('image/png');
}

/**
 * The map as it is on screen, as a PNG data URL. The map must be created with
 * preserveDrawingBuffer, and even then the drawing buffer is empty once the map has settled and
 * stopped painting: reading it then gives a black picture. So a repaint is asked for and the
 * canvas read inside the render that follows, while the frame is still there.
 */
export function snapshot(map) {
    return new Promise((resolve) => {
        map.once('render', () => resolve(map.getCanvas().toDataURL('image/png')));
        map.triggerRepaint();
    });
}

/** Features the page holds itself - results of a query, shapes it drew - as a GeoJSON source under the layers given. */
export function geojsonOverlay(map, id, layers, data, { before } = {}) {
    data = data || { type: 'FeatureCollection', features: [] };
    map.addSource(id, { type: 'geojson', data });
    let ids = [];
    const put = (newLayers) => {
        for (const old of ids) if (map.getLayer(old)) map.removeLayer(old);
        ids = [];
        newLayers.forEach((layer, i) => {
            const copy = JSON.parse(JSON.stringify(layer));
            copy.id = copy.id || `${id}-${i}`;
            copy.source = id;
            fontOf(copy);
            map.addLayer(copy, before);
            ids.push(copy.id);
        });
    };
    put(layers);
    const handle = {
        data,
        setData: (fc) => { handle.data = fc; map.getSource(id).setData(fc || { type: 'FeatureCollection', features: [] }); },
        setLayers: put,
        clear: () => handle.setData({ type: 'FeatureCollection', features: [] }),
    };
    return handle;
}

/**
 * Adornment layers drawn on the server as one image the map's size, laid over the map and
 * asked for again whenever the view settles.
 */
export function adornments(map, overlay) {
    const image = document.createElement('img');
    Object.assign(image.style, { position: 'absolute', inset: '0', width: '100%', height: '100%', pointerEvents: 'none' });
    map.getContainer().appendChild(image);
    const refresh = () => {
        const canvas = map.getCanvas(), w = canvas.clientWidth, h = canvas.clientHeight;
        image.src = `${root}adornments/${overlay}?width=${w}&height=${h}&bbox=${viewBox(map, w, h)}&t=${Date.now()}`;
    };
    map.on('moveend', refresh);
    map.on('resize', refresh);
    ready(map).then(refresh);
    return { refresh, remove: () => { map.off('moveend', refresh); map.off('resize', refresh); image.remove(); } };
}

/**
 * The map's view as a north-up box of the screen's size about its centre, in degrees: what the
 * server draws adornments for. The map's own bounds grow when it is turned or tilted, and a
 * scale read off them would be the scale of a bigger map.
 */
export function viewBox(map, w, h) {
    const c = map.getCenter(), [x, y] = toMeters(c.lng, c.lat), r = resolutionForZoom(map.getZoom());
    const sw = toLngLat(x - r * w / 2, y - r * h / 2), ne = toLngLat(x + r * w / 2, y + r * h / 2);
    return [sw[0], sw[1], ne[0], ne[1]].map(v => v.toFixed(6)).join(',');
}

/**
 * A compass drawn on the server - the magnetic declination adornment, in the middle of a small
 * picture of its own for the map's centre - pinned to the top right and turned with the map, so
 * its true north points to true north however the map is turned.
 */
export function compass(map, overlay, size = 170) {
    const image = document.createElement('img');
    Object.assign(image.style, { position: 'absolute', right: '10px', top: '10px', width: size + 'px', height: size + 'px', pointerEvents: 'none', transformOrigin: '50% 50%' });
    map.getContainer().appendChild(image);
    const refresh = () => { image.src = `${root}adornments/${overlay}?width=${size}&height=${size}&bbox=${viewBox(map, size, size)}&t=${Date.now()}`; };
    const turn = () => { image.style.transform = `rotate(${-map.getBearing()}deg)`; };
    map.on('moveend', refresh);
    map.on('rotate', turn);
    ready(map).then(() => { refresh(); turn(); });
    return { refresh, remove: () => { map.off('moveend', refresh); map.off('rotate', turn); image.remove(); } };
}

/**
 * The style.json editor beside a map: the text is parsed as it is typed, and a document that
 * parses is handed to apply(layers); one that does not changes nothing.
 */
export function editor(textarea, status, text, apply) {
    textarea.value = text;
    let pending = 0;
    const run = () => {
        try {
            const parsed = JSON.parse(textarea.value);
            apply(parsed.layers || []);
            status.textContent = 'applied at ' + new Date().toLocaleTimeString();
            status.style.color = 'darkgreen';
        } catch (e) {
            status.textContent = 'not applied - ' + e.message;
            status.style.color = 'firebrick';
        }
    };
    textarea.addEventListener('input', () => { clearTimeout(pending); pending = setTimeout(run, 400); });
    return { apply: run, get text() { return textarea.value; }, set text(v) { textarea.value = v; run(); } };
}

/** The layers of a document as pretty text, the way the editor shows them. */
export const styleText = (layers) => JSON.stringify({ layers }, null, 2);

/** A popup at a place, with HTML in it. */
export function popup(map, lngLat, html, { offset = 12 } = {}) {
    return new maplibregl.Popup({ offset, closeButton: true }).setLngLat(lngLat).setHTML(html).addTo(map);
}

/**
 * Drawing on the map: a point is one click; a line or polygon is clicks for the vertices and a
 * double click to finish. What is being drawn shows as it goes; done(feature) gets the shape as
 * GeoJSON. Returns a function that cancels.
 */
export function draw(map, mode, done) {
    const id = '__drawing';
    if (!map.getSource(id)) {
        map.addSource(id, { type: 'geojson', data: { type: 'FeatureCollection', features: [] } });
        map.addLayer({ id: id + '-fill', type: 'fill', source: id, filter: ['==', ['geometry-type'], 'Polygon'], paint: { 'fill-color': 'rgba(30,90,220,0.15)' } });
        map.addLayer({ id: id + '-line', type: 'line', source: id, paint: { 'line-color': '#1E5ADC', 'line-width': 2, 'line-dasharray': [2, 2] } });
        map.addLayer({ id: id + '-pt', type: 'circle', source: id, filter: ['==', ['geometry-type'], 'Point'], paint: { 'circle-radius': 5, 'circle-color': '#1E5ADC', 'circle-stroke-color': '#fff', 'circle-stroke-width': 2 } });
    }
    const vertices = [];
    const show = (cursor) => {
        const pts = cursor ? vertices.concat([cursor]) : vertices;
        const features = [];
        if (mode === 'polygon' && pts.length >= 3) features.push({ type: 'Feature', geometry: { type: 'Polygon', coordinates: [pts.concat([pts[0]])] }, properties: {} });
        else if (pts.length >= 2) features.push({ type: 'Feature', geometry: { type: 'LineString', coordinates: pts }, properties: {} });
        for (const p of vertices) features.push({ type: 'Feature', geometry: { type: 'Point', coordinates: p }, properties: {} });
        map.getSource(id).setData({ type: 'FeatureCollection', features });
    };
    const finish = () => {
        cancel();
        if (mode === 'point') return done({ type: 'Feature', geometry: { type: 'Point', coordinates: vertices[0] }, properties: {} });
        if (mode === 'line' && vertices.length >= 2) return done({ type: 'Feature', geometry: { type: 'LineString', coordinates: vertices }, properties: {} });
        if (mode === 'polygon' && vertices.length >= 3) return done({ type: 'Feature', geometry: { type: 'Polygon', coordinates: [vertices.concat([vertices[0]])] }, properties: {} });
    };
    const onClick = (e) => { vertices.push([e.lngLat.lng, e.lngLat.lat]); show(); if (mode === 'point') finish(); };
    const onMove = (e) => { if (vertices.length) show([e.lngLat.lng, e.lngLat.lat]); };
    const onDouble = (e) => { e.preventDefault(); finish(); };
    const cancel = () => {
        map.off('click', onClick); map.off('mousemove', onMove); map.off('dblclick', onDouble);
        map.getCanvas().style.cursor = '';
        map.doubleClickZoom.enable();
        map.getSource(id).setData({ type: 'FeatureCollection', features: [] });
    };
    map.doubleClickZoom.disable();
    map.getCanvas().style.cursor = 'crosshair';
    map.on('click', onClick); map.on('mousemove', onMove); map.on('dblclick', onDouble);
    return cancel;
}

/** GET a route of the server, as JSON. */
export async function get(path) {
    const response = await fetch(root + path);
    if (!response.ok) throw new Error(await response.text() || response.statusText);
    return response.json();
}

/** POST JSON to a route of the server, and read JSON back. */
export async function post(path, body) {
    const response = await fetch(root + path, { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify(body || {}) });
    if (!response.ok) throw new Error(await response.text() || response.statusText);
    return response.json();
}

/** Zooms the map to an extent given as [minx, miny, maxx, maxy] in degrees. */
export function fit(map, bounds, padding = 20) {
    map.fitBounds([[bounds[0], bounds[1]], [bounds[2], bounds[3]]], { padding, duration: 600 });
}

/** The status line under a control: a word about what is going on. */
export function say(element, text, ok = true) {
    element.textContent = text;
    element.style.color = ok ? '#555' : 'firebrick';
}
