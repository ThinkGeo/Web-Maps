# ThinkGeo Cloud Maps Sample for WebForms

### Description

This sample demonstrates how you can display ThinkGeo Cloud Maps in your Map Suite GIS applications. It will show you how to use the XYZFileBitmapTileCache to improve the performance of map rendering. ThinkGeoCloudMapsOverlay uses the ThinkGeo Cloud XYZ Tile Server as raster map tile server. It supports 5 different map styles: 
- Light
- Dark
- Aerial
- Hybrid
- TransparentBackground

ThinkGeo Cloud Maps support would work in all of the Map Suite controls such as Wpf, Web, MVC, WebApi, Android and iOS.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webforms) for the details.

![Screenshot](https://github.com/ThinkGeo/ThinkGeoCloudMapsSample-ForWebForms/blob/master/Screenshot.gif)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code
```csharp
Map1.MapUnit = GeographyUnit.Meter;
Map1.ZoomLevelSet = ThinkGeoCloudMapsOverlay.GetZoomLevelSet();
Map1.MapTools.OverlaySwitcher.Enabled = true;
Map1.MapTools.OverlaySwitcher.BaseOverlayTitle = "ThinkGeo Cloud Maps:";
Map1.MapTools.OverlaySwitcher.BackgroundColor = GeoColor.StandardColors.DarkSlateGray;

ThinkGeoCloudMapsOverlay lightMap = new ThinkGeoCloudMapsOverlay();
lightMap.Name = "Light";
lightMap.MapType = ThinkGeoCloudMapsMapType.Light;
lightMap.WrapDateline = WrapDatelineMode.WrapDateline;
Map1.CustomOverlays.Add(lightMap);

ThinkGeoCloudMapsOverlay darkMap = new ThinkGeoCloudMapsOverlay();
darkMap.Name = "Dark";
darkMap.MapType = ThinkGeoCloudMapsMapType.Dark;
darkMap.WrapDateline = WrapDatelineMode.WrapDateline;
Map1.CustomOverlays.Add(darkMap);

ThinkGeoCloudMapsOverlay aerialMap = new ThinkGeoCloudMapsOverlay();
aerialMap.Name = "Aerial";
aerialMap.MapType = ThinkGeoCloudMapsMapType.Aerial;
aerialMap.WrapDateline = WrapDatelineMode.WrapDateline;
Map1.CustomOverlays.Add(aerialMap);

ThinkGeoCloudMapsOverlay hybridMap = new ThinkGeoCloudMapsOverlay();
hybridMap.Name = "Hybrid";
hybridMap.MapType = ThinkGeoCloudMapsMapType.Hybrid;
hybridMap.WrapDateline = WrapDatelineMode.WrapDateline;
Map1.CustomOverlays.Add(hybridMap);

ThinkGeoCloudMapsOverlay transparentBackgroundMap = new ThinkGeoCloudMapsOverlay();
transparentBackgroundMap.Name = "Transparent Background";
transparentBackgroundMap.MapType = ThinkGeoCloudMapsMapType.TransparentBackground;
transparentBackgroundMap.WrapDateline = WrapDatelineMode.WrapDateline;
Map1.CustomOverlays.Add(transparentBackgroundMap);

Map1.CurrentExtent = new RectangleShape(-20037508.2314698, 20037508.2314698, 20037508.2314698, -20037508.2314698);
```
### Getting Help

[Map Suite Web for WebForms Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webforms)

[Map Suite Web for WebForms Product Description](https://thinkgeo.com/ui-controls#web-platforms)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

Working...

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
