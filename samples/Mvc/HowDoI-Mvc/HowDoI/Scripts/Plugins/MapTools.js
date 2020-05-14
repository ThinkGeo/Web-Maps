var MapTools = Class.Create();
Class.Extent(MapTools.prototype, {
    initialize: function (mapObj) {
    },

    createMeasureMaptool: function () {
        return new OpenLayers.Control.Measure();
    },

    createTouchMapTool: function () {
        return new OpenLayers.Control.TouchNavigation();
    },

    createAnimationPan: function () {
        return new OpenLayers.Control.AnimationPan();
    },

    createOverlaySwitcher: function () {
        var control = new OpenLayers.Control.LayerSwitcher();

        return control;
    },

    createMouseMapTool: function () {
        return new OpenLayers.Control.Navigation();
    },

    createMouseCoordinateMapTool: function (formatType) {
        var control = new OpenLayers.Control.MousePosition();
        control.showType = formatType;

        return control;
    },

    createMiniMap: function () {
        var control = new OpenLayers.Control.OverviewMap();

        return control;
    },

    createPanZoom: function () {
        var control = new OpenLayers.Control.PanZoom();
        return control;
    },

    createPanZoomBar: function () {
        var control = new OpenLayers.Control.PanZoomBar();
        control.zoomWorldIcon = true;
        return control;
    },

    createScaleLine: function () {
        return new OpenLayers.Control.ScaleLine();
    },

    createLogo: function () {
        return new OpenLayers.Control.Logo();
    },

    createKeyboardMapTool: function () {
        return new OpenLayers.Control.KeyboardDefaults();
    }
})