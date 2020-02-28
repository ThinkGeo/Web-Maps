# Projection Sample for WebApi

### Description

Learn about Map Projection and how to apply it to your data.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webapi) for the details.

![Screenshot](https://github.com/ThinkGeo/ProjectionSample-ForWebApi/blob/master/ScreenShot.png)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code
```csharp
string countriesFilePath = string.Format(@"{0}/Countries02.shp", baseDirectory);
ShapeFileFeatureLayer countriesLayer = new ShapeFileFeatureLayer(countriesFilePath);
countriesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColor.SimpleColors.Green, 2);
countriesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
LayerOverlay layerOverlay = new LayerOverlay();
layerOverlay.Layers.Add(countriesLayer);

Proj4Projection decimalDegreeToMeter = new Proj4Projection();
decimalDegreeToMeter.InternalProjectionParametersString = Proj4Projection.GetWgs84ParametersString();
decimalDegreeToMeter.ExternalProjectionParametersString = Proj4Projection.GetSphericalMercatorParametersString();
decimalDegreeToMeter.Open();
```
### Getting Help

[Map Suite WebAPI Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webapi)

[Map Suite WebAPI Product Description](https://thinkgeo.com/ui-controls#web-platforms)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

- [ThinkGeo.MapSuite.Drawing.GeoCanvas](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.drawing.geocanvas)
- [ThinkGeo.MapSuite.Layers.ShapeFileFeatureLayer](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.layers.shapefilefeaturelayer)
- [ThinkGeo.MapSuite.WebApi.LayerOverlay](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.webapi.layeroverlay)

### FAQ
- __Q: How do I make background map work?__  
A: Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require an API Key. These were sent to you via email when you signed up with ThinkGeo, or you can register now at https://cloud.thinkgeo.com. Once you get them, please update the code in the ThinkGeo Cloud URL in "thinkgeo.js" or "index.htm". 

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
