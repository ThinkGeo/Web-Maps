# Edit Overlay Styles Sample for WebForms

### Description

This Web project demonstrates how to control the styles of the **EditOverlay**, for both the default style and the editing style. In order to accomplish this, we use javascript code. Look at the code in the *script* tag to see how we can control the fill color, the opacity, the border width, and other attributes of the features in the **EditOverlay**.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webforms) for the details.

![Screenshot](https://github.com/ThinkGeo/EditOverlayStylesSample-ForWebForms/blob/master/Screenshot.gif)

### Requirements

This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code
```csharp
Feature editFeature = new Feature(polygonShape);
Map1.EditOverlay.Features.Add(editFeature);

Map1.EditOverlay.TrackMode = TrackMode.Edit;
Map1.EditOverlay.EditSettings.IsDraggable = true;
Map1.EditOverlay.EditSettings.IsReshapable = false;
Map1.EditOverlay.EditSettings.IsResizable = false;
Map1.EditOverlay.EditSettings.IsRotatable = false;
```
### Getting Help

[Map Suite Web for WebForms Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webforms)

[Map Suite Web for WebForms Product Description](https://thinkgeo.com/ui-controls#web-platforms)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs

This example makes use of the following APIs:

- [ThinkGeo.MapSuite.WebForms.EditSettings](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.webforms.editsettings)
- [ThinkGeo.MapSuite.Shapes.Feature](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.shapes.feature)
- [ThinkGeo.MapSuite.WebForms.TrackMode](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.webforms.trackmode)

### About Map Suite

Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo

ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
