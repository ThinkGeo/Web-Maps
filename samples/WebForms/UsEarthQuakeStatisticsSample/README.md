# Us Earthquake Statistics Sample for WebForms

### Description

The Earthquake Statistics sample template is a statistical report system for earthquakes that have occurred in the past few years across the United States. It can help you generate infographics and analyze the severely afflicted areas, or used as supporting evidence when recommending measures to minimize the damage in future quakes.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webforms) for the details.

![Screenshot](https://github.com/ThinkGeo/UsEarthquakeStatisticsSample-ForWebForms/blob/master/Screenshot.gif)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code
```csharp
LayerOverlay trackShapeOverlay = Map1.CustomOverlays["TrackShapeOverlay"] as LayerOverlay;
InMemoryFeatureLayer trackShapeLayer = trackShapeOverlay.Layers["TrackShapeLayer"] as InMemoryFeatureLayer;
trackShapeLayer.InternalFeatures.Clear();
trackShapeOverlay.Redraw();

LayerOverlay queryResultMarkerOverlay = Map1.CustomOverlays["QueryResultMarkerOverlay"] as LayerOverlay;
InMemoryFeatureLayer markerMemoryLayer = queryResultMarkerOverlay.Layers["MarkerMemoryLayer"] as InMemoryFeatureLayer;
markerMemoryLayer.InternalFeatures.Clear();
InMemoryFeatureLayer markerMemoryHighLightLayer = queryResultMarkerOverlay.Layers["MarkerMemoryHighLightLayer"] as InMemoryFeatureLayer;
markerMemoryHighLightLayer.InternalFeatures.Clear();
queryResultMarkerOverlay.Redraw();
```
### Getting Help

[Map Suite Web for WebForms Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webforms)

[Map Suite Web for WebForms Product Description](https://thinkgeo.com/ui-controls#web-platforms)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:
- [ThinkGeo.MapSuite.WebForms.LayerOverlay](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.webforms.layeroverlay)
- [ThinkGeo.MapSuite.Layers.ShapeFileFeatureLayer](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.layers.shapefilefeaturelayer)
- [ThinkGeo.MapSuite.Drawing.GeoColor](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.drawing.geocolor)
- [ThinkGeo.MapSuite.Layers.InMemoryFeatureLayer](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.layers.inmemoryfeaturelayer)

### FAQ
- __Q: How do I make background map work?__  
A: Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require an API Key. These were sent to you via email when you signed up with ThinkGeo, or you can register now at https://cloud.thinkgeo.com. Once you get them, please update the code in method AddBaseMapLayers() in Default.aspx.cs.  

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
