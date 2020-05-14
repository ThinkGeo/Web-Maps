# Us Earthquake Statistics Sample for Mvc

### Description

The Earthquake Statistics sample template is a statistical report system for earthquakes that have occurred in the past few years across the United States. It can help you generate infographics and analyze the severely afflicted areas, or used as supporting evidence when recommending measures to minimize the damage in future quakes.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_mvc) for the details.

![Screenshot](https://github.com/ThinkGeo/UsEarthquakeStatisticsSample-ForMvc/blob/master/Screenshot.gif)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code

```csharp

LayerOverlay earthquakeOverlay = new LayerOverlay("EarthquakeOverlay");
            //earthquakeOverlay.TileType = TileType.SingleTile;
            earthquakeOverlay.IsVisibleInOverlaySwitcher = false;
            Map1.CustomOverlays.Add(earthquakeOverlay);

            Proj4Projection proj4 = new Proj4Projection();
            proj4.InternalProjectionParametersString = Proj4Projection.GetDecimalDegreesParametersString();
            proj4.ExternalProjectionParametersString = Proj4Projection.GetSphericalMercatorParametersString();

            string dataShapefileFilePath = Server.MapPath(ConfigurationManager.AppSettings["statesPathFileName"]);

            EarthquakeHeatFeatureLayer heatLayer = new EarthquakeHeatFeatureLayer(new ShapeFileFeatureSource(dataShapefileFilePath));
            heatLayer.HeatStyle = new HeatStyle(10, 180, "MAGNITUDE", 0, 12, 100, DistanceUnit.Kilometer);
            heatLayer.FeatureSource.Projection = proj4;
            earthquakeOverlay.Layers.Add("Heat Map", heatLayer);

            ShapeFileFeatureLayer pointLayer = new ShapeFileFeatureLayer(dataShapefileFilePath);
            pointLayer.FeatureSource.Projection = proj4;
            pointLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.StandardColors.Red, 6, GeoColor.StandardColors.White, 1);
            pointLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            pointLayer.IsVisible = false;
            earthquakeOverlay.Layers.Add("Regular Point Map", pointLayer);

            EarthquakeIsoLineFeatureLayer isoLineLayer = new EarthquakeIsoLineFeatureLayer(new ShapeFileFeatureSource(dataShapefileFilePath));
            isoLineLayer.FeatureSource.Projection = proj4;
            isoLineLayer.IsVisible = false;
            earthquakeOverlay.Layers.Add("IsoLines Map", isoLineLayer);


```

### Getting Help

[Map Suite web for Mvc Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_web_for_mvc)

[Map Suite web for Mvc Product Description](https://thinkgeo.com/ui-controls#web-platforms)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

- [ThinkGeo.MapSuite.Mvc.LayerOverlay](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.mvc.layeroverlay)
- [ThinkGeo.MapSuite.Shapes.Proj4Projection](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.shapes.proj4projection)
- [ThinkGeo.MapSuite.Layers.ShapeFileFeatureLayer](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.layers.shapefilefeaturelayer)

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
