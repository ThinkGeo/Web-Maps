# Building 3D JS Sample for WebApi

### Description
This project shows to create a simulated 3D buildings and use world map kit server as background map.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webapi) for the details.

![Screenshot](https://github.com/ThinkGeo/Building3DJSSample-ForWebApi/blob/master/Screenshot.png)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code
```csharp
var map = new OpenLayers.Map("map");
var wkt_url = ["http://worldmapkit1.thinkgeo.com/CachedWmsServer/WmsServer.axd",
         "http://worldmapkit2.thinkgeo.com/CachedWmsServer/WmsServer.axd",
         "http://worldmapkit3.thinkgeo.com/CachedWmsServer/WmsServer.axd",
         "http://worldmapkit4.thinkgeo.com/CachedWmsServer/WmsServer.axd",
         "http://worldmapkit5.thinkgeo.com/CachedWmsServer/WmsServer.axd",
         "http://worldmapkit6.thinkgeo.com/CachedWmsServer/WmsServer.axd"];
var wkt_params = {
    EXCEPTIONS: "application/vnd.ogc.se_inimage",
    LAYERS: "OSMWorldMapKitLayer",
    STYLES: "WorldMapKitDefaultStyle"
};
var wkt_option = {
    name: "wkt",
    projection: new OpenLayers.Projection("EPSG:900913"),
    resolutions: [156543.02800009822,
        78271.51400004911,
        39135.757000024554,
        19567.878500012277,
        9783.939250006139,
        4891.969625003069,
        2445.9848125015346,
        1222.9924062507673,
        611.4962031253837,
        305.74810156269183,
        152.87405078134591,
        76.43702539067296,
        38.21851269533648,
        19.10925634766824,
        9.55462817383412,
        4.77731408691706,
        2.38865704345853,
        1.194328521729265,
        0.5971642608646325,
        0.29858213043231624],
}
var ol_wkt = new OpenLayers.Layer.WMS(wkt_option.name, wkt_url, wkt_params, wkt_option);
map.addLayers([ol_wkt]);
map.setCenter([-10047547.366026, 4669691.6437984], 17);
var osmb = new OSMBuildings(map).load();
delete (osmb.maxExtent);
osmb.addOptions({ maxExtent: new OpenLayers.Bounds(-20037507.80, -20037508.64, 20037508.84, 20037508.08) });
```
### Getting Help

[Map Suite WebAPI Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_web_for_webapi)

[Map Suite WebAPI Product Description](https://thinkgeo.com/ui-controls#web-platforms)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

### FAQ
- __Q: How do I make background map work?__  
A: Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require an API Key. These were sent to you via email when you signed up with ThinkGeo, or you can register now at https://cloud.thinkgeo.com. Once you get them, please update the code in the ThinkGeo Cloud URL in "thinkgeo.js" or "index.htm". 

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
