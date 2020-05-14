# Site Selection Sample for WebApi

### Description

This sample template allows you to view, understand, interpret, and visualize spatial data in many ways that reveal relationships, patterns, and trends. In this example, the user can apply the features of GIS to analyze spatial data to efficiently choose a suitable site for a new retail outlet.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webapi) for the details.

![Screenshot](https://github.com/ThinkGeo/SiteSelectionSample-ForWebApi/blob/master/ScreenShot.png)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code
```csharp
FeatureLayer friscoLayer = (FeatureLayer)poiOverlay.Layers ["friscoBoundary"];
friscoLayer.Open ();
if (friscoLayer.QueryTools.GetFeaturesContaining (searchPoint, ReturningColumnsType.NoColumns).Any ()) {
	// Calculate the service area/buffer area and display it on the map
	if (parameters ["searchMode"] == "serviceArea") {
		int drivingTimeInMinutes = Convert.ToInt32 (parameters ["driveTime"], CultureInfo.InvariantCulture);
		searchAreaFeature = new Feature (routingEngine.GenerateServiceArea (searchPoint, new TimeSpan (0, drivingTimeInMinutes, 0), 100, GeographyUnit.Meter));
				} else {
		DistanceUnit distanceUnit = parameters ["distanceUnit"] == "Mile" ? DistanceUnit.Mile : DistanceUnit.Kilometer;
		double distanceBuffer = Convert.ToDouble (parameters ["distanceBuffer"], CultureInfo.InvariantCulture);
		searchAreaFeature = new Feature (searchPoint.Buffer (distanceBuffer, 40, GeographyUnit.Meter, distanceUnit));
	}

// Search the Pois in calculated service area and display them on map
ShapeFileFeatureLayer poiLayer = (ShapeFileFeatureLayer)(poiOverlay.Layers [parameters ["category"]]);
Collection<Feature> featuresInServiceArea = poiLayer.QueryTools.GetFeaturesWithin (searchAreaFeature.GetShape (), ReturningColumnsType.AllColumns);
List<Feature> filteredQueryFeatures = FilterFeaturesByQueryConfiguration (featuresInServiceArea, parameters ["category"], parameters ["subCategory"].Replace (">~", ">="));

if (filteredQueryFeatures.Any ()) {
	Collection<object> returnedJsonFeatures = new Collection<object> ();
	foreach (Feature feature in filteredQueryFeatures) {
		PointShape latlng = InternalHelper.ConvertToWgs84<PointShape> (feature.GetShape ());
		returnedJsonFeatures.Add (new { name = feature.ColumnValues ["NAME"], point = string.Format ("{0},{1}", latlng.Y, latlng.X) });
					}

			Feature wgs84Feature = new Feature (InternalHelper.ConvertToWgs84<BaseShape> (searchAreaFeature.GetShape ()));
			result = JsonConvert.SerializeObject (new { status = "0", message = "has features", area = wgs84Feature.GetGeoJson (), features = returnedJsonFeatures });
				} else {
					result = JsonConvert.SerializeObject (new { status = "1", message = "without features" });
		}
}
```
### Getting Help

[Map Suite WebAPI Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webapi)

[Map Suite WebAPI Product Description](https://thinkgeo.com/ui-controls#web-platforms)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

- [ThinkGeo.MapSuite.Layers.QueryTools](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.layers.querytools)
- [ThinkGeo.MapSuite.Layers.ShapeFileFeatureLayer](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.layers.shapefilefeaturelayer)
- [ThinkGeo.MapSuite.WebApi.LayerOverlay](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.webapi.layeroverlay)
- [ThinkGeo.MapSuite.Shapes.PointShape](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.shapes.pointshape)

### FAQ
- __Q: How do I make background map work?__  
A: Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require an API Key. These were sent to you via email when you signed up with ThinkGeo, or you can register now at https://cloud.thinkgeo.com. Once you get them, please update the code in the ThinkGeo Cloud URL in "thinkgeo.js" or "index.htm". 

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
