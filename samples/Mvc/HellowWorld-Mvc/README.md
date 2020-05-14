# Hello World Sample for Mvc

### Description
This sample shows you how to get started building your first application with the Map Suite Web for Mvc 10.0.0.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_mvc) for the details.

![Screenshot](https://github.com/ThinkGeo/HelloWorldSample-ForMvc/blob/master/Screenshot.gif)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code

```csharp

<div style="height:80%">
    @{ Html.ThinkGeo().Map(Model).Render(); }
</div>

public ActionResult Index()
{
    Map map = new Map("Map1");
    map.Width = new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage);
    map.Height = new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage);
    map.MapUnit = GeographyUnit.Meter;
    map.ZoomLevelSet = ThinkGeoCloudMapsOverlay.GetZoomLevelSet();
    map.CurrentExtent = new RectangleShape(-13086298.60, 7339062.72, -8111177.75, 2853137.62);
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

### FAQ
- __Q: How do I make background map work?__  
A: Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require an API Key. These were sent to you via email when you signed up with ThinkGeo, or you can register now at https://cloud.thinkgeo.com. Once you get them, please update the code in Index.cshtml.  

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
