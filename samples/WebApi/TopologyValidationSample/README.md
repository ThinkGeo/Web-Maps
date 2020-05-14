# Topology Validation Sample for WebApi

### Description

This sample shows simple ways to validate topology rules. 

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webapi) for the details.

![Screenshot](https://github.com/ThinkGeo/TopologyValidationSample-ForWebApi/blob/master/ScreenShot.png)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code

```csharp
switch (currentTopologyType)
{
    case TopologyType.PointsMustBeCoveredByBoundaryOfPolygons:
         resultFeatures = TopologyValidator.PointsMustBeCoveredByBoundaryOfPolygons(inputData.FirstInputFeatures, inputData.SecondInputFeatures);
         break;

    case TopologyType.PointsMustBeCoveredByEndpointOfLines:
         resultFeatures = TopologyValidator.PointsMustBeCoveredByEndPointOfLines(inputData.FirstInputFeatures, inputData.SecondInputFeatures);
         break;

    case TopologyType.PointsMustBeCoveredByLines:
         resultFeatures = TopologyValidator.PointsMustBeCoveredByLines(inputData.FirstInputFeatures, inputData.SecondInputFeatures);
         break;

    case TopologyType.PointsMustBeProperlyInsidePolygons:
         resultFeatures = TopologyValidator.PointsMustBeProperlyInsidePolygons(inputData.FirstInputFeatures, inputData.SecondInputFeatures);
         break;
}
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
- [ThinkGeo.MapSuite.Shapes.TopologyValidator](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.shapes.topologyvalidator)

### FAQ
- __Q: How do I make background map work?__  
A: Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require an API Key. These were sent to you via email when you signed up with ThinkGeo, or you can register now at https://cloud.thinkgeo.com. Once you get them, please update the code in the ThinkGeo Cloud URL in "thinkgeo.js" or "index.htm". 

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
