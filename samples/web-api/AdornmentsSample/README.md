# Adornments Sample for WebApi

### Description

Learn how to add legends, scale bars, directional arrows, and many more adornments to your map.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webapi) for the details.

![Screenshot](https://github.com/ThinkGeo/AdornmentsSample-ForWebApi/blob/master/Screenshot.gif)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code
```csharp
switch (adornmentType)
{
    case AdornmentsType.ScaleBarAdornment:
        adornmentLayer = new ScaleBarAdornmentLayer();
        break;

    case AdornmentsType.ScaleLineAdornment:
        adornmentLayer = new ScaleLineAdornmentLayer();
        break;

    case AdornmentsType.ScaleTextAdornment:
        adornmentLayer = new ScaleTextAdornmentLayer();
        break;

    case AdornmentsType.LogoAdornment:
        adornmentLayer = BuildLogoAdornmentLayer();
        break;

    case AdornmentsType.GraticuleAdornment:
        adornmentLayer = BuildGraticuleAdornmentLayer();
        break;

    case AdornmentsType.LegendAdornment:
        adornmentLayer = BuildLegendAdornmentLayer();
        break;

    case AdornmentsType.MagneticDeclinationAdornment:
        adornmentLayer = BuildMagneticDeclinationAdornmentLayer();
        break;

    default:
        adornmentLayer = null;
        break;
}
```
### Getting Help

[Map Suite WebAPI Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webapi)

[Map Suite WebAPI Product Description](https://thinkgeo.com/ui-controls#web-platforms)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

- [ThinkGeo.MapSuite.Layers.ScaleBarAdornmentLayer](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.layers.scalebaradornmentlayer)
- [ThinkGeo.MapSuite.Layers.ScaleLineAdornmentLayer](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.layers.scalelineadornmentlayer)
- [ThinkGeo.MapSuite.Layers.ScaleTextAdornmentLayer](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.layers.scaletextadornmentlayer)
- [ThinkGeo.MapSuite.Layers.LogoAdornmentLayer](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.layers.logoadornmentlayer)
- [ThinkGeo.MapSuite.Layers.GraticuleFeatureLayer](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.layers.graticulefeaturelayer)

### FAQ
- __Q: How do I make background map work?__  
A: Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require an API Key. These were sent to you via email when you signed up with ThinkGeo, or you can register now at https://cloud.thinkgeo.com. Once you get them, please update the code in the ThinkGeo Cloud URL in "thinkgeo.js" or "index.htm". 

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
