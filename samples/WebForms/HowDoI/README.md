# How Do I Sample for WebForms

### Description

The "How Do I?" samples collection is a comprehensive set containing dozens of interactive samples.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webforms) for the details.

![Screenshot](https://github.com/ThinkGeo/HowDoISample-ForWebForms/blob/master/Screenshot.gif)

### Requirements

This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code
```csharp
ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02.shp"));
worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 243, 239, 228), GeoColor.FromArgb(255, 218, 193, 163), 1);
worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

ShapeFileFeatureLayer citiesLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/USA/cities_a.shp"));
citiesLayer.ZoomLevelSet.ZoomLevel05.DefaultPointStyle = PointStyles.CreateCompoundCircleStyle(GeoColor.StandardColors.White, 6F, GeoColor.StandardColors.Black, 1F, GeoColor.StandardColors.Black, 3F);
citiesLayer.ZoomLevelSet.ZoomLevel05.DefaultTextStyle = new TextStyle("AREANAME", new GeoFont("Verdana", 9), new GeoSolidBrush(GeoColor.StandardColors.Black));
citiesLayer.ZoomLevelSet.ZoomLevel05.DefaultTextStyle.HaloPen = new GeoPen(GeoColor.StandardColors.White, 2);
citiesLayer.ZoomLevelSet.ZoomLevel05.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

ShapeFileFeatureLayer streetsLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/USA/Austin/austinstreets.shp"));
streetsLayer.ZoomLevelSet.ZoomLevel10.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.White, 3F, GeoColor.StandardColors.DarkGray, 5F, true);
streetsLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

Map1.StaticOverlay.Layers.Add(worldLayer);
Map1.StaticOverlay.Layers.Add(citiesLayer);
Map1.StaticOverlay.Layers.Add(streetsLayer);
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

### FAQ
- __Q: How do I make background map work?__  
A: Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require an API Key. These were sent to you via email when you signed up with ThinkGeo, or you can register now at https://cloud.thinkgeo.com. Once you get them, please update the code in method Page_Load() in partial class of pages.  

### About Map Suite

Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo

ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
