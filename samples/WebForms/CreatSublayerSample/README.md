# Create Sublayers Sample for WebForms

### Description
This sample shows you how to use FeatureSource to implement sublayer fuctions.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webforms) for the details.

![Screenshot](https://github.com/ThinkGeo/CreateSublayersSample-ForWebForms/blob/master/Screenshot.gif)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code
```csharp
ShapeFileFeatureLayer featureLayer = ((ShapeFileFeatureLayer)(((LayerOverlay)Map1.CustomOverlays[1]).Layers[0]));
featureLayer.Open();
Collection<Feature> features = featureLayer.FeatureSource.GetAllFeatures(ReturningColumnsType.AllColumns);
featureLayer.Close();

foreach (Feature feature in features)
{
    featureLayer.FeatureIdsToExclude.Add(feature.Id);
}
```
### Getting Help

[Map Suite Web for WebForms Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webforms)

[Map Suite Web for WebForms Product Description](https://thinkgeo.com/ui-controls#web-platforms)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

- [ThinkGeo.MapSuite.Layers.ShapeFileFeatureSource](http://wiki.thinkgeo.com/wiki/api/ThinkGeo.MapSuite.Layers.ShapeFileFeatureSource)

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.

