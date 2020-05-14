# Vehicle Tracking Sample for Mvc

### Description

The Vehicle Tracking sample template gives you a head start on your next tracking project. With a working code example to draw from, you can spend more of your time implementing the features you care about and less time thinking about how to accomplish the basic functionality of a tracking system.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_mvc) for the details.

![Screenshot](https://github.com/ThinkGeo/VehicleTrackingSample-ForMvc/blob/master/Screenshot.gif)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code

```csharp
// Add the vehicle's latest position
                Feature latestPositionFeature = new Feature(currentVehicle.Value.Location.GetLocationPointShape().GetWellKnownBinary(), currentVehicle.Value.VehicleName);
                latestPositionFeature.ColumnValues["DateTime"] = currentVehicle.Value.Location.DateTime.ToString();
                latestPositionFeature.ColumnValues["IsCurrentPosition"] = "IsCurrentPosition";
                latestPositionFeature.ColumnValues["Speed"] = currentVehicle.Value.Location.Speed.ToString(CultureInfo.InvariantCulture);

                Location projectedCurrentLocation = ProjectLocation(currentVehicle.Value.Location);
                latestPositionFeature.ColumnValues["Longitude"] = projectedCurrentLocation.Longitude.ToString("N6", CultureInfo.InvariantCulture);
                latestPositionFeature.ColumnValues["Latitude"] = projectedCurrentLocation.Latitude.ToString("N6", CultureInfo.InvariantCulture);
                latestPositionFeature.ColumnValues["VehicleName"] = currentVehicle.Value.VehicleName;
                latestPositionFeature.ColumnValues["Duration"] = currentVehicle.Value.SpeedDuration.ToString(CultureInfo.InvariantCulture);
                vehiclesHistoryOverlay.FeatureSource.InternalFeatures.Add(latestPositionFeature);

                vehiclesHistoryOverlay.FeatureSource.Close();
```

### Getting Help

[Map Suite web for Mvc Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_web_for_mvc)

[Map Suite web for Mvc Product Description](https://thinkgeo.com/ui-controls#web-platforms)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

- [ThinkGeo.MapSuite.Mvc.InMemoryMarkerOverlay](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.mvc.inmemorymarkeroverlay)
- [ThinkGeo.MapSuite.Mvc.ValueMarkerStyle](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.mvc.valuemarkerstyle)
- [ThinkGeo.MapSuite.Mvc.OpenStreetMapOverlay](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.mvc.openstreetmapoverlay)

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
