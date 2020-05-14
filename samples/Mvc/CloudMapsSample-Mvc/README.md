# ThinkGeo Cloud Maps Sample for Mvc

### Description

This sample demonstrates how you can display ThinkGeo Cloud Maps in your Map Suite GIS applications. It will show you how to use the XYZFileBitmapTileCache to improve the performance of map rendering. ThinkGeoCloudMapsOverlay uses the ThinkGeo Cloud XYZ Tile Server as raster map tile server. It supports 5 different map styles: 
- Light
- Dark
- Aerial
- Hybrid
- TransparentBackground

ThinkGeo Cloud Maps support would work in all of the Map Suite controls such as Wpf, WinForms, Web, WebApi, Android and iOS.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_mvc) for the details.

![Screenshot](https://github.com/ThinkGeo/ThinkGeoCloudMapsSample-ForMvc/blob/master/Screenshot.gif)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code
```csharp
Map map = new Map("Map1",
    new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage),
    new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage));

map.MapUnit = GeographyUnit.Meter;
map.ZoomLevelSet = ThinkGeoCloudMapsOverlay.GetZoomLevelSet();
map.MapTools.OverlaySwitcher.Enabled = true;
map.MapTools.OverlaySwitcher.BaseOverlayTitle = "ThinkGeo Cloud Maps:";
map.MapTools.OverlaySwitcher.BackgroundColor = GeoColor.StandardColors.DarkSlateGray;

ThinkGeoCloudMapsOverlay lightMap = new ThinkGeoCloudMapsOverlay();
lightMap.Name = "Light";
lightMap.WrapDateline = WrapDatelineMode.WrapDateline;
lightMap.MapType = ThinkGeoCloudMapsMapType.Light;
map.CustomOverlays.Add(lightMap);

ThinkGeoCloudMapsOverlay darkMap = new ThinkGeoCloudMapsOverlay();
darkMap.Name = "Dark";
darkMap.WrapDateline = WrapDatelineMode.WrapDateline;
darkMap.MapType = ThinkGeoCloudMapsMapType.Dark;
map.CustomOverlays.Add(darkMap);

ThinkGeoCloudMapsOverlay aerialMap = new ThinkGeoCloudMapsOverlay();
aerialMap.Name = "Aerial";
aerialMap.WrapDateline = WrapDatelineMode.WrapDateline;
aerialMap.MapType = ThinkGeoCloudMapsMapType.Aerial;
map.CustomOverlays.Add(aerialMap);

ThinkGeoCloudMapsOverlay hybridMap = new ThinkGeoCloudMapsOverlay();
hybridMap.Name = "Hybrid";
hybridMap.WrapDateline = WrapDatelineMode.WrapDateline;
hybridMap.MapType = ThinkGeoCloudMapsMapType.Hybrid;
map.CustomOverlays.Add(hybridMap);

ThinkGeoCloudMapsOverlay transparentBackgroundMap = new ThinkGeoCloudMapsOverlay();
transparentBackgroundMap.Name = "Transparent Background";
transparentBackgroundMap.WrapDateline = WrapDatelineMode.WrapDateline;
transparentBackgroundMap.MapType = ThinkGeoCloudMapsMapType.TransparentBackground;
map.CustomOverlays.Add(transparentBackgroundMap);

map.CurrentExtent = new RectangleShape(-13086298.60, 7339062.72, -8111177.75, 2853137.62);

return View(map);
```
### Getting Help

[Map Suite Web for Mvc Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_web_for_mvc)

[Map Suite Web for Mvc Product Description](https://thinkgeo.com/ui-controls#mvc-platforms)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

Working...

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
