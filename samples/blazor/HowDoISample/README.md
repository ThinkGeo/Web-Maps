# How Do I Sample for Blazor

### Description

The “How Do I?” samples collection is a comprehensive set containing dozens of interactive samples. Available in C# (Blazor), these samples are designed to hit all the highlights of ThinkGeo Blazor UIs, from simply adding a layer to a map to performing vehicle tracking and applying a thematic style. Consider this collection your “encyclopedia” of all the ThinkGeo basics and a great starting place for new users.

Please refer to [ThinkGeo Web UI for Blazor](https://thinkgeo.com/gis-ui-web) for the details.

![Screenshot](https://github.com/ThinkGeo/HowDoISample-ForBlazor/blob/master/Screenshot.gif)

### About the Code
Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require an API Key. The following function is just for reminding you to input the key.
```csharp
    <MapView Id="map" MapUnit="@ThinkGeo.Core.GeographyUnit.Meter" ProgressiveZooming="true"
         Center="@(new PointShape(-11037792.463030849, 4953010.053845501))"
         Zoom="5">
        <OverlaysSetting>
            <ThinkGeoCloudVectorMapsOverlay Id="VectorOverlay"
                                            ApiKey=""
                                            MapType="mapType" />
        </OverlaysSetting>
    </MapView>

@code{
    Map map;
    protected override void OnInitialized()
    {
        // Initialize here ...
    }
}
```

### Getting Help

[ThinkGeo Community Site](https://community.thinkgeo.com)

[ThinkGeo Web Site](https://www.thinkgeo.com)

### FAQ
- __Q: How do I make background map work?__  
A: Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require a Client ID and Secret. These were sent to you via email when you signed up with ThinkGeo, or you can register now at https://cloud.thinkgeo.com. Once you get them, please update the code in property ApiKey="" in RazorControl partial classes.  

### About ThinkGeo
ThinkGeo is a GIS (Geographic Information Systems) company founded in 2004 and located in Frisco, TX. Our clients are in more than 40 industries including agriculture, energy, transportation, government, engineering, software development, and defense. 
