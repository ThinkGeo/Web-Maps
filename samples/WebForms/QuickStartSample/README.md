# Quickstart Sample for WebForms

### Description
The Map Suite Web QuickStart Guide will guide you through the process of creating a sample application and will help you become familiar with Map Suite.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webforms) for the details.

![Screenshot](https://github.com/ThinkGeo/QuickstartSample-ForWebForms/blob/master/Screenshot.gif)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code
```csharp
ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/App_Data/Countries02.shp"));
AreaStyle areaStyle = new AreaStyle();
areaStyle.FillSolidBrush = new GeoSolidBrush(GeoColor.FromArgb(255, 233, 232, 214));
areaStyle.OutlinePen = new GeoPen(GeoColor.FromArgb(255, 118, 138, 69), 1);
areaStyle.OutlinePen.DashStyle = LineDashStyle.Solid;

worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = areaStyle;
worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

ShapeFileFeatureLayer capitalLayer = new ShapeFileFeatureLayer(MapPath("~/App_Data/WorldCapitals.shp"));
capitalLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.StandardColors.White, 7, GeoColor.StandardColors.Brown);

capitalLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level05;
PointStyle pointStyle = new PointStyle();
pointStyle.SymbolType = PointSymbolType.Square;
pointStyle.SymbolSolidBrush = new GeoSolidBrush(GeoColor.StandardColors.White);
pointStyle.SymbolPen = new GeoPen(GeoColor.StandardColors.Black, 1);
pointStyle.SymbolSize = 6;
```
### Getting Help

[Map Suite Web for WebForms Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webforms)

[Map Suite Web for WebForms Product Description](https://thinkgeo.com/ui-controls#web-platforms)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

- [ThinkGeo.MapSuite.Styles.AreaStyle](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.styles.areastyle)
- [ThinkGeo.MapSuite.Layers.ShapeFileFeatureLayer](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.layers.shapefilefeaturelayer)
- [ThinkGeo.MapSuite.Drawing.GeoColor](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.drawing.geocolor)
- [ThinkGeo.MapSuite.Styles.PointStyle](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.styles.pointstyle)

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.

