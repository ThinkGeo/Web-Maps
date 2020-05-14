# RoutingSample-ForWebForms

### Description

[Sample Data Download](http://wiki.thinkgeo.com/wiki/_media/routing/routingdata_3857.zip)

The Map Suite Routing “How Do I?” solution offers a series of useful how-to examples for using the Map Suite Routing extension. The bundled solution comes with a small set of routable street data from Austin, TX and demonstrates simple routing, avoiding specific areas, getting turn-by-turn directions, optimizing for the Traveling Salesman Problem, and much more. Full source code is included in both C# and VB.NET languages; simply select your preferred language to download the associated solution.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webforms) for the details.

![Screenshot](https://github.com/ThinkGeo/RoutingSample-ForWebForms/blob/master/Screenshot.gif)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code
```csharp
Feature feature = featureSource.GetFeatureById(roadId, ReturningColumnsType.NoColumns);

LineBaseShape sourceShape = (LineBaseShape)feature.GetShape();
Collection<LineShape> lines = ConvertLineBaseShapeToLines(sourceShape);

List<string> adjacentIDs = new List<string>();
foreach (LineShape line in lines)
{
    Collection<Feature> features = featureSource.GetFeaturesInsideBoundingBox(line.GetBoundingBox(), ReturningColumnsType.NoColumns);
    foreach (Feature tempFeature in features)
    {
        BaseShape tempShape = tempFeature.GetShape();
        if (feature.Id != tempFeature.Id && line.Intersects(tempShape))
        {
            adjacentIDs.Add(tempFeature.Id);
        }
    }
}
```
### Getting Help

[Map Suite Web for WebForms Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webforms)

[Map Suite Web for WebForms Product Description](https://thinkgeo.com/web)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

- [ThinkGeo.MapSuite.Shapes.Feature](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.shapes.feature)
- [ThinkGeo.MapSuite.Shapes.LineBaseShape](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.shapes.linebaseshape)
- [ThinkGeo.MapSuite.Layers.FeatureSource](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.layers.featuresource)
- [ThinkGeo.MapSuite.Shapes.BaseShape](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.shapes.baseshape)

### FAQ
- __Q: How do I make background map work?__  
A: Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require an API Key. These were sent to you via email when you signed up with ThinkGeo, or you can register now at https://cloud.thinkgeo.com. Once you get them, please update the code in method Page_Load() in partial class of pages.  

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
