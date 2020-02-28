# Labeling Sample for WebApi

### Description

This sample shows you how to examine different labeling techniques to make your maps informative.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webapi) for the details.

![Screenshot](https://github.com/ThinkGeo/LabelingSample-ForWebApi/blob/master/ScreenShot.png)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code
```csharp
PlatformGeoCanvas geoCanvas = new PlatformGeoCanvas();
RectangleShape boundingBox = WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, GeographyUnit.Meter);
geoCanvas.BeginDrawing(bitmap, boundingBox, GeographyUnit.Meter);

if (layerOverlay != null)
{
    layerOverlay.Draw(geoCanvas);
}
geoCanvas.EndDrawing();
```
### Getting Help

[Map Suite WebAPI Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webapi)

[Map Suite WebAPI Product Description](https://thinkgeo.com/ui-controls#web-platforms)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:
- [ThinkGeo.MapSuite.Drawing.GeoCanvas](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.drawing.geocanvas)
- [ThinkGeo.MapSuite.Layers.PrinterLayer](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.layers.printerlayer)
- [ThinkGeo.MapSuite.WebApi.LayerOverlay](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.webapi.layeroverlay)

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
