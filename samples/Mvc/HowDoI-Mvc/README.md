# How Do I Sample for Mvc

### Description

The "How Do I?" samples collection is a comprehensive set containing a lot of interactive samples.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_mvc) for the details.

![Screenshot](https://github.com/ThinkGeo/HowDoISample-ForMvc/blob/master/Screenshot.gif)

### Requirements

This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code

```csharp

public ActionResult DisplayASimpleMap()
{
    Map map = new Map("Map1",
        new Unit(100, UnitType.Percentage),
        new Unit(100, UnitType.Percentage));

    map.MapUnit = GeographyUnit.Meter;
    map.ZoomLevelSet = ThinkGeoCloudMapsOverlay.GetZoomLevelSet();
    map.CurrentExtent = new RectangleShape(-13939426.6371, 6701997.4056, -7812401.86, 2626987.386962);

    map.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
    map.CustomOverlays.Add(new ThinkGeoCloudMapsOverlay());

    return View(map);
}

```

### Getting Help

[Map Suite web for Mvc Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_web_for_mvc)

[Map Suite web for Mvc Product Description](https://thinkgeo.com/ui-controls#web-platforms)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs

This example makes use of the following APIs:

- [ThinkGeo.MapSuite.Mvc.WorldMapKitWmsWebOverlay](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.mvc.worldmapkitwmsweboverlay)
- [ThinkGeo.MapSuite.Drawing.GeoSolidBrush](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.drawing.geosolidbrush)
- [ThinkGeo.MapSuite.Shapes.RectangleShape](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.shapes.rectangleshape)

### FAQ
- __Q: How do I make background map work?__  
A: Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require an API Key. These were sent to you via email when you signed up with ThinkGeo, or you can register now at https://cloud.thinkgeo.com. Once you get them, please update the code in partial class of Controllers.  


### About Map Suite

Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo

ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
