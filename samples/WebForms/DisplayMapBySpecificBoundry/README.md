# Display Map By Specified Boundary Sample for WebForms

### Description
This sample shows how to display map by specified boundary. It shows how to cancel a request based on its extent to improve performance.  

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webforms) for the details.

![Screenshot](https://github.com/ThinkGeo/DisplayMapBySpecifiedBoundarySample-ForWebForms/blob/master/Screenshot.gif)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code
```csharp
Feature coverFeature = new Feature(new RectangleShape(-20037508.34, 20037508.34, 20037508.34, -20037508.34));
ShapeFileFeatureLayer shapeFileLayer = new ShapeFileFeatureLayer(sourceShapeFile);
shapeFileLayer.Open();
var sourceFeatures = shapeFileLayer.FeatureSource.GetAllFeatures(ReturningColumnsType.AllColumns);
foreach (var sourceFeature in sourceFeatures)
    coverFeature = coverFeature.GetDifference(sourceFeature);

DbfColumn dbfColumn = new DbfColumn("Id", DbfColumnType.IntegerInBinary, 4, 0);
ShapeFileFeatureSource.CreateShapeFile(ShapeFileType.Polygon, tagetShapeFile, new[] { dbfColumn });
ShapeFileFeatureLayer coverShapeFileLayer = new ShapeFileFeatureLayer(tagetShapeFile, GeoFileReadWriteMode.ReadWrite);
coverShapeFileLayer.FeatureSource.Open();
coverShapeFileLayer.FeatureSource.BeginTransaction();
coverShapeFileLayer.FeatureSource.AddFeature(coverFeature);
coverShapeFileLayer.FeatureSource.CommitTransaction();
coverShapeFileLayer.FeatureSource.Close();
```
### Getting Help

[Map Suite Web for WebForms Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webforms)

[Map Suite Web for WebForms Product Description](https://thinkgeo.com/ui-controls#web-platforms)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

- [ThinkGeo.MapSuite.Layers.ShapeFileFeatureLayer](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.layers.shapefilefeaturelayer)
- [ThinkGeo.MapSuite.Layers.OpenStreetMapLayer](http://wiki.thinkgeo.com/wiki/api/ThinkGeo.MapSuite.Layers.OpenStreetMapLayer)
- [ThinkGeo.MapSuite.Shapes.Feature](http://wiki.thinkgeo.com/wiki/api/ThinkGeo.MapSuite.Shapes.Feature)

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.

