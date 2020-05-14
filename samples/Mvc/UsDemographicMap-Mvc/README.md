# Us Demographic Map Sample for Mvc

### Description

The Demographic and Lifestyle sample template gives you a head start on your statistics project, which includes details about race, age, gender, land usage, and more for all the states in U.S. The template contains pre-styled layers that can be used as-is, or as the foundation for adding your own map notes and layers.

Please refer to [Wiki](http://wiki.thinkgeo.com/wiki/map_suite_web_for_mvc) for the details.

![Screenshot](https://github.com/ThinkGeo/UsDemographicMapSample-ForMvc/blob/master/Screenshot.gif)

### Requirements
This sample makes use of the following NuGet Packages

[MapSuite 10.0.0](https://www.nuget.org/packages?q=ThinkGeo)

### About the Code

```csharp

// us states layer
            ShapeFileFeatureLayer statesLayer = new ShapeFileFeatureLayer(Server.MapPath(ConfigurationManager.AppSettings["UsShapefilePath"]));
            ThematicDemographicStyleBuilder selectedStyle = new ThematicDemographicStyleBuilder(new Collection<string>() { "Population" });
            statesLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(selectedStyle.GetStyle(statesLayer.FeatureSource));
            statesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            map.DynamicOverlay.Layers.Add("usStatesLayer", statesLayer);

            // highlight layers
            map.HighlightOverlay.HighlightStyle = new FeatureOverlayStyle(GeoColor.FromArgb(150, GeoColor.FromHtml("#449FBC")), GeoColor.FromHtml("#014576"), 1);
            map.HighlightOverlay.Style = new FeatureOverlayStyle(GeoColor.SimpleColors.Transparent, GeoColor.SimpleColors.Transparent, 0);
            statesLayer.Open();
            foreach (Feature feature in statesLayer.FeatureSource.GetAllFeatures(ReturningColumnsType.NoColumns))
            {
                map.HighlightOverlay.Features.Add(feature.Id, feature);
            }
            statesLayer.Close();
    }

```

### Getting Help

[Map Suite web for Mvc Wiki Resources](http://wiki.thinkgeo.com/wiki/map_suite_web_for_mvc)

[Map Suite web for Mvc Product Description](https://thinkgeo.com/ui-controls#web-platforms)

[ThinkGeo Community Site](http://community.thinkgeo.com/)

[ThinkGeo Web Site](http://www.thinkgeo.com)

### Key APIs
This example makes use of the following APIs:

- [ThinkGeo.MapSuite.Mvc.CloudPopup](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.mvc.cloudpopup)
- [ThinkGeo.MapSuite.Layers.LegendAdornmentLayer](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.layers.legendadornmentlayer)
- [ThinkGeo.MapSuite.Styles.ClassBreakStyle](http://wiki.thinkgeo.com/wiki/api/thinkgeo.mapsuite.styles.classbreakstyle)

### About Map Suite
Map Suite is a set of powerful development components and services for the .Net Framework.

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense.
