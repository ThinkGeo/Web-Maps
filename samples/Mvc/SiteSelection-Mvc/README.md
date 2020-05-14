# Site Selection Sample for Mvc

### Description

The Site Selection sample template allows you to view, understand, interpret, and visualize spatial data in many ways that reveal relationships, patterns, and trends. In the example illustrated, the user can apply the features of GIS to analyze spatial data to efficiently choose a suitable site for a new retail outlet.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_mvc) for the details.

![Screenshot](https://github.com/ThinkGeo/SiteSelectionSample-ForMvc/blob/master/Screenshot.gif)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code

```csharp
   // Create the Routine Engine. 
            string streetShapeFilePathName = Server.MapPath(ConfigurationManager.AppSettings["StreetShapeFilePathName"]);
            string streetRtgFilePathName = Path.ChangeExtension(streetShapeFilePathName, ".rtg");

            RtgRoutingSource routingSource = new RtgRoutingSource(streetRtgFilePathName);
            FeatureSource featureSource = new ShapeFileFeatureSource(streetShapeFilePathName);
            featureSource.Projection = proj4;
            routingEngine = new RoutingEngine(routingSource, featureSource);
            routingEngine.GeographyUnit = GeographyUnit.Meter;
```

### Getting Help

[Map Suite web for Mvc Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_web_for_mvc)

[Map Suite web for Mvc Product Description](https://thinkgeo.com/ui-controls#web-platforms)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

- [ThinkGeo.MapSuite.Layers.ShapeFileFeatureLayer](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.layers.shapefilefeaturelayer)
- [ThinkGeo.MapSuite.Layers.ScaleBarAdornmentLayer](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.layers.scalebaradornmentlayer)

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
