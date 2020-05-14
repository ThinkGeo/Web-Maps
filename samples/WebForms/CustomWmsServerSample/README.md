# Custom Wms Server Sample for ForWebForms
### Description
This sample shows you how to add a custome Wms layer plugin to Wms server. To run this sample correctly, you sholud:
1. Build the CustomWmsLayerPlugin/CustomWmsLayerPlugin.csproj at first, the ouput directory of this project had been redirected to CustomWmsServer/Plugins.
2. Change the key-value pair of "Countries02" to absolute path of "Countires02.shp" in CustomWmsServer/Web.config, you can find the shape file under CustomWmsServer/AppData
3. Build & run the CustomWmsServer/CustomWmsServer.csproj.

Please refer to  [Wiki](https://wiki.thinkgeo.com/wiki/map_suite_web_for_webforms) for the detail of ThinkGeo WebForms.

![Screenshot](https://github.com/ThinkGeo/CustomWmsServerSamples-ForWebForms/blob/master/ScreenShot.gif)

### Requirements

This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code
```csharp
protected override MapConfiguration GetMapConfigurationCore(string style, string crs)
{
    MapConfiguration mapConfiguration = new MapConfiguration();

    string shapeFile = ConfigurationManager.AppSettings["Countries02"];
    ShapeFileFeatureLayer shapeFileFeatureLayer = new ShapeFileFeatureLayer(shapeFile);

    switch (crs)
    {
        case "EPSG:900913":
        case "EPSG:3857":
            shapeFileFeatureLayer.FeatureSource.Projection = new Proj4Projection(4326, 3857);
            break;
        case "EPSG:4326":
        case "CRS:84":
            break;
        default:
            throw new ArgumentException("The input CRS is not supported. Currently only EPSG:4326 and EPSG:900913 and CRS:84 are supported.");
    }

    switch (style)
    {
        case "Country1":
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(
                new GeoPen(new GeoSolidBrush(GeoColor.FromHtml("#0066ff")), 1f),
                new GeoSolidBrush(GeoColor.FromHtml("#00ccff")));
            break;
        case "Country2":
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(
                new GeoPen(new GeoSolidBrush(GeoColor.FromHtml("#669900")), 1f),
                new GeoSolidBrush(GeoColor.FromHtml("#66ffcc")));
            break;
    }

    shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
    mapConfiguration.Layers.Add(shapeFileFeatureLayer);

    return mapConfiguration;
}
```

### Getting Help

- [ThinkGeo Web for WebForms Wiki Resources](https://wiki.thinkgeo.com/wiki/map_suite_web_for_webforms)
- [ThinkGeo Web for WebForms Product Description](https://thinkgeo.com/gis-ui-web)
- [ThinkGeo Community Site](http://community.thinkgeo.com)
- [ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs

This example makes use of the following APIs:

- [ThinkGeo.MapSuite.WmsServer](https://wiki.thinkgeo.com/wiki/thinkgeo.mapsuite.wmsserveredition)

### About Map Suite

Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo

ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
