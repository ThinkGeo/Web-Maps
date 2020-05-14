# Quickstart Sample for WebApi

### Description
The Map Suite WebAPI illustrated QuickStart Guide will guide you through the process of creating a sample application and will help you become familiar with Map Suite. This edition of the QuickStart Guide supports Map Suite WebAPI 10.0.0.0 and higher and will show you how to create a ASP.NET WebAPI application using this product.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webapi) for the details.

![Screenshot](https://github.com/ThinkGeo/QuickstartSample-ForWebApi/blob/master/Screenshot.PNG)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code
```csharp
LayerOverlay layerOverlay = new LayerOverlay();

ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(HttpContext.Current.Server.MapPath(@"~/App_Data/cntry02.shp"));
Proj4Projection proj4 = new Proj4Projection(Proj4Projection.GetWgs84ParametersString(), Proj4Projection.GetSphericalMercatorParametersString());
if (!proj4.IsOpen) proj4.Open();
worldLayer.FeatureSource.Projection = proj4;
 
worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = CreateCustomAreaStyle();

worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

ShapeFileFeatureLayer capitalLayer = new ShapeFileFeatureLayer(HttpContext.Current.Server.MapPath("~/App_Data/capital.shp"));
capitalLayer.FeatureSource.Projection = proj4;

capitalLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.StandardColors.White, 7, GeoColor.StandardColors.Brown);

capitalLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level05;

PointStyle customPointStyle = CreateCustomPointStyle();
capitalLayer.ZoomLevelSet.ZoomLevel06.DefaultPointStyle = customPointStyle;

capitalLayer.ZoomLevelSet.ZoomLevel06.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

capitalLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = TextStyles.CreateSimpleTextStyle("CITY_NAME", "Arial", 8, DrawingFontStyles.Italic, GeoColor.StandardColors.Black, 3, 3);
capitalLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level05;
capitalLayer.ZoomLevelSet.ZoomLevel06.DefaultTextStyle = CreateCustomTextStyle("CITY_NAME");
capitalLayer.ZoomLevelSet.ZoomLevel06.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
layerOverlay.Layers.Add(worldLayer);
layerOverlay.Layers.Add(capitalLayer);
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
- [ThinkGeo.MapSuite.Shapes.Proj4Projection](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.shapes.proj4projection)
- [ThinkGeo.MapSuite.Styles.PointStyle](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.styles.pointstyle)

### FAQ
- __Q: How do I make background map work?__  
A: Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require an API Key. These were sent to you via email when you signed up with ThinkGeo, or you can register now at https://cloud.thinkgeo.com. Once you get them, please update the code in the ThinkGeo Cloud URL in "thinkgeo.js" or "index.htm". 

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
