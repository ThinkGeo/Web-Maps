# Display Nautical Charts Sample for WebApi


### Description

This sample shows how you can use Map Suite WebAPI Edition to display nautical charts in your web application.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webapi) for the details.

![Screenshot](https://github.com/ThinkGeo/DisplayNauticalChartsSample-ForWebApi/blob/master/Screenshot.png)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code
```csharp
private readonly string nauticalChartsFilePath = HttpContext.Current.Server.MapPath("~/App_Data/US4IL10M.000");
private static NauticalChartsFeatureLayer nauticalChartsFeatureLayer;

[Route("tile/{z}/{x}/{y}")]
[HttpGet]
public HttpResponseMessage GetTile(int z, int x, int y)
{
    if (nauticalChartsFeatureLayer == null)
        nauticalChartsFeatureLayer = new NauticalChartsFeatureLayer(nauticalChartsFilePath);
    lock (nauticalChartsFeatureLayer)
    {
        nauticalChartsFeatureLayer.IsDepthContourTextVisible = true;
        nauticalChartsFeatureLayer.IsLightDescriptionVisible = true;
        nauticalChartsFeatureLayer.StylingType = NauticalChartsStylingType.EmbeddedStyling;
        nauticalChartsFeatureLayer.SymbolTextDisplayMode = NauticalChartsSymbolTextDisplayMode.None;
        nauticalChartsFeatureLayer.DisplayCategory = NauticalChartsDisplayCategory.All;
        nauticalChartsFeatureLayer.DefaultColorSchema = NauticalChartsDefaultColorSchema.DayBright;
        nauticalChartsFeatureLayer.SymbolDisplayMode = NauticalChartsSymbolDisplayMode.Simplified;
        nauticalChartsFeatureLayer.BoundaryDisplayMode = NauticalChartsBoundaryDisplayMode.Plain;
        nauticalChartsFeatureLayer.SafetyDepthInMeter = NauticalChartsFeatureLayer.ConvertDistanceToMeters(28, NauticalChartsDepthUnit.Meter);
        nauticalChartsFeatureLayer.ShallowDepthInMeter = NauticalChartsFeatureLayer.ConvertDistanceToMeters(3, NauticalChartsDepthUnit.Meter);
        nauticalChartsFeatureLayer.DeepDepthInMeter = NauticalChartsFeatureLayer.ConvertDistanceToMeters(10, NauticalChartsDepthUnit.Meter);
        nauticalChartsFeatureLayer.SafetyContourDepthInMeter = NauticalChartsFeatureLayer.ConvertDistanceToMeters(10, NauticalChartsDepthUnit.Meter);
        nauticalChartsFeatureLayer.DrawingMode = NauticalChartsDrawingMode.Optimized;
        nauticalChartsFeatureLayer.IsFullLightLineVisible = true;
        nauticalChartsFeatureLayer.IsMetaObjectsVisible = false;
    }

    LayerOverlay layerOverlay = new LayerOverlay();
    layerOverlay.Layers.Add(nauticalChartsFeatureLayer);

    return DrawLayerOverlay(layerOverlay, z, x, y);
}
```

### Getting Help

[Map Suite WebAPI Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webapi)

[Map Suite WebAPI Product Description](https://thinkgeo.com/ui-controls#web-platforms)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

- [ThinkGeo.MapSuite.Drawing.PlatformGeoCanvas](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.drawing.PlatformGeoCanvas)
- [ThinkGeo.MapSuite.Layers.NauticalChartsFeatureLayer](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.layers.printerlayer)
- [ThinkGeo.MapSuite.WebApi.LayerOverlay](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.webapi.layeroverlay)

### FAQ
- __Q: How do I make background map work?__  
A: Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require an API Key. These were sent to you via email when you signed up with ThinkGeo, or you can register now at https://cloud.thinkgeo.com. Once you get them, please update the code in the ThinkGeo Cloud URL in "thinkgeo.js" or "index.htm". 

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
