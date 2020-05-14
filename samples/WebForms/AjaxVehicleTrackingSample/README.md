# Ajax Vehicle Tracking Sample for WebForms

### Description

The Ajax Vehicle Tracking sample template gives you a head start on your next ajax tracking project. With a working code example to draw from, you can spend more of your time implementing the features you care about and less time thinking about how to accomplish the basic functionality of a ajax tracking system.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webforms) for the details.

![Screenshot](https://github.com/ThinkGeo/AjaxVehicleTrackingSample-ForWebForms/blob/master/Screenshot.gif)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code
```csharp
ThinkGeoCloudRasterMapsOverlay lightMapOverlay = new ThinkGeoCloudRasterMapsOverlay();
lightMapOverlay.Name = "Light";
lightMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Light;
Map1.CustomOverlays.Add(lightMapOverlay);

ThinkGeoCloudRasterMapsOverlay darkMapOverlay = new ThinkGeoCloudRasterMapsOverlay();
darkMapOverlay.Name = "Dark";
darkMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Dark;
Map1.CustomOverlays.Add(darkMapOverlay);

ThinkGeoCloudRasterMapsOverlay aerialMapOverlay = new ThinkGeoCloudRasterMapsOverlay();
aerialMapOverlay.Name = "Aerial";
aerialMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Aerial;
Map1.CustomOverlays.Add(aerialMapOverlay);

ThinkGeoCloudRasterMapsOverlay hybridMapOverlay = new ThinkGeoCloudRasterMapsOverlay();
hybridMapOverlay.Name = "Hybrid";
hybridMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Hybrid;
Map1.CustomOverlays.Add(hybridMapOverlay);
```

### Getting Help

[Map Suite Web for WebForms Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webforms)

[Map Suite Web for WebForms Product Description](https://thinkgeo.com/ui-controls#web-platforms)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

- [ThinkGeo.MapSuite.WebForms.LayerOverlay](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.webforms.layeroverlay)
- [ThinkGeo.MapSuite.WebForms.WorldMapKitWmsWebOverlay](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.webforms.worldmapkitwmsweboverlay)
- [ThinkGeo.MapSuite.Drawing.GeoColor](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.drawing.geocolor)
- [ThinkGeo.MapSuite.WebForms.Map](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.webforms.map)

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
