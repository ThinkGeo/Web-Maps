# Us Demographic Map Sample for WebForms

### Description

The Demographic and Lifestyle sample template gives you a head start on your statistics project, which includes details about race, age, gender, land usage, and more for all the states in U.S. The template contains pre-styled layers that can be used as-is, or as the foundation for adding your own map notes and layers.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webforms) for the details.

![Screenshot](https://github.com/ThinkGeo/UsDemographicMapSample-ForWebForms/blob/master/Screenshot.gif)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code
```csharp
ShapeFileFeatureLayer statesLayer = Map1.DynamicOverlay.Layers["usStatesLayer"] as ShapeFileFeatureLayer;
statesLayer.Open();
statesLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
Styles.Style selectedStyle = demographicStyle.GetStyle(statesLayer.FeatureSource);
statesLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(selectedStyle);
statesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

Map1.DynamicOverlay.Redraw();
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
A: Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require an API Key. These were sent to you via email when you signed up with ThinkGeo, or you can register now at https://cloud.thinkgeo.com. Once you get them, please update the code in method InitializeMapControl() in partial class of pages.  

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
