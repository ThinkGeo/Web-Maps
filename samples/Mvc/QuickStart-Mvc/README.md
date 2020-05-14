# Quickstart Sample for Mvc

### Description
The Map Suite MVC QuickStart Guide will guide you through the process of creating a sample application and will help you become familiar with Map Suite. The QuickStart Guide supports Map Suite 10.0.0.0 and higher and will show you how to create an ASP.NET MVC application. 

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_mvc) for the details.

![Screenshot](https://github.com/ThinkGeo/QuickstartSample-ForMvc/blob/master/Screenshot.gif)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code

```csharp
 ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(Server.MapPath("~/App_Data/USStates.SHP"));
                AreaStyle areaStyle = new AreaStyle();
                areaStyle.FillSolidBrush = new GeoSolidBrush(GeoColor.FromArgb(255, 233, 232, 214));
                areaStyle.OutlinePen = new GeoPen(GeoColor.FromArgb(255, 156, 155, 154), 2);
                areaStyle.OutlinePen.DashStyle = LineDashStyle.Solid;
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = areaStyle;
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                overlay.Layers.Add(worldLayer);
```

### Getting Help

[Map Suite web for Mvc Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_web_for_mvc)

[Map Suite web for Mvc Product Description](https://thinkgeo.com/ui-controls#web-platforms)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

- [ThinkGeo.MapSuite.Styles.AreaStyle](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.styles.areastyle)
- [ThinkGeo.MapSuite.Styles.PointStyle](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.styles.pointstyle)
- [ThinkGeo.MapSuite.Styles.TextStyle](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.styles.textstyle)

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
