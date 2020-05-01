# Blazor Guide

## Quick Start: Display a Simple Map

We will begin by creating a **Blazor App** as the client to consuming the service in your favorite editor.  Next, we will walk you through adding the required packages and getting a map on the default form.  Then, we will add some code to show a nice looking background map, and finally, add some custom data to the map and style it.  After reading this, you will be in a good position to look over the [How Do I Sample](samples/blazor) and explore our other features.

![alt text](assets/quickstart_webmaps_blazor_screenshot.png "Simple Map")

### Step 1: Setup a New Project

In your editor of choice you need to create a **Blazor App** project.  Please see your editor's instructions on how to create these projects.  We have included a guide in Visual Studio below.

[Visual Studio 2019 Example - Blazor App](https://docs.microsoft.com/en-us/aspnet/core/blazor/get-started?view=aspnetcore-3.1&tabs=visual-studio&viewFallbackFrom=aspnetcore-3.0)

### Step 2: Add NuGet Packages

You will need to install **ThinkGeo.UI.Blazor** in **Blazor App** project.  We highly suggest you use your editors [built in NuGet package manager](https://docs.microsoft.com/en-us/nuget/quickstart/) if possible.  If you're not using an IDE you can [install it via the the dotnet CLI](https://docs.microsoft.com/en-us/nuget/consume-packages/install-use-packages-dotnet-cli) from inside your project folder where where your project file exists.

```shell
Install-Package ThinkGeo.UI.Blazor
```

### Step 3: Create a world map

- **Add using Reference**
Open ~/Imports.razor and add 2 using directives to reference the Map components.
    ```csharp
    @using ThinkGeo.Core 
    @using ThinkGeo.UI.Blazor
    ```


- **Add stylesheet and JavaScript references**
Open the ~/Pages/_Host.cshtml and add the stylesheet and javascript references of ThinkGeo Web For Blazor from CDN:
    ```html
        <head>
            ...
            <link href="https://cdn.thinkgeo.com/blazor/1.0.0/blazor.css" rel="stylesheet" />
        </head>
     
        <body>
            ...
            <script src="https://cdn.thinkgeo.com/blazor/1.0.0/blazor.js"></script>
        </body>
    ```
- **Add ThinkGeo.UI.Blazor map component**
Open ~/Pages/Index.razor file and add a ThinkGeo.UI.Blazor.Map component as following:

    ```html
    @page "/"
     
    <!--You can control the width and height in CSS too, if you like. -->
    <MapView Id="demomap" MapUnit="@ThinkGeo.Core.GeographyUnit.Meter" Zoom="3" Width="800" Height="600">
        <OverlaysSetting>
            <ThinkGeoCloudRasterMapsOverlay Id="RasterOverlay" MapType="ThinkGeo.Core.ThinkGeoCloudRasterMapsMapType.Aerial" ApiKey="YOUR CLOUD API KEY"></ThinkGeoCloudRasterMapsOverlay>
        </OverlaysSetting>
    </MapView>
    ```

**NOTE:** It requires an APIKey of ThinkGeo Cloud, and you can get the API Key from the registration email. If you don't have an account of ThinkGeo Cloud yet, please refer to [ThinkGeo Cloud Client Keys](https://wiki.thinkgeo.com/wiki/thinkgeo_cloud_client_keys_guideline) to get your ThinkGeo Cloud account.

The first time you run your **ThinkGeo.UI.Blazor** project, you will be presented with ThinkGeo's Product Center which will create and manage your licenses for all of ThinkGeo's products. Create a new account to begin a 60-day free evaluation.

1. Run the application in Debug mode.

1. Click the "Create a new Account?" link.

1. Fill out your name, email address, password and company name and click register.

1. Check your email and click the "Active Your Account" link.

1. Return to Product Center and login using the credentials your just created and hit "Continue Debugging" button.

You should now see your map with our Cloud Maps layer!

### Step 4: Prepare spatial data required

Download the required spatial data from [GitLab](/samples/data) and put it into "Data" directory locating at the root project.

### Step 6: Add a Point Data Layer

Add point capital `FeatureLayer` to the map by adding one more `LayerOverlay` in MapView's  `OverlaysSetting`:

```html
<!--You can control the width and height in CSS too, if you like. -->
<MapView Id="demomap" MapUnit="@ThinkGeo.Core.GeographyUnit.Meter" Zoom="3" Width="800" Height="600">
    <OverlaysSetting>
        <ThinkGeoCloudRasterMapsOverlay Id="RasterOverlay" MapType="ThinkGeo.Core.ThinkGeoCloudRasterMapsMapType.Aerial" ApiKey="YOUR CLOUD API KEY"></ThinkGeoCloudRasterMapsOverlay>
        <!--Add one more Overlay for hosting capital FeatureLayer. -->
        <LayerOverlay Id="capitalOverlay" Layers="@layers"></LayerOverlay>
    </OverlaysSetting>
</MapView>
```

Create the `FeatureLayer` with the data "capital.shp" saved in "Data" directory:

```csharp
@code {
    GeoCollection<Layer> layers = new GeoCollection<Layer>();

    protected override void OnInitialized()
    {
        // Create the point layer for capitals.
        ShapeFileFeatureLayer capitalFeatureLayer = new ShapeFileFeatureLayer("./Data/capital.shp");
        // Create the "Mercator projection" and apply it to the layer to match the background map.
        capitalFeatureLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(Projection.GetWgs84ProjString(), Projection.GetSphericalMercatorProjString());

        // Create the point style for positions.
        capitalFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle(PointSymbolType.Circle, 4, new GeoSolidBrush(GeoColor.FromHtml("#21FF00")), new GeoPen(GeoColor.FromHtml("#ffffff"), 1));
        // Create the labeling style for capitals.
        capitalFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = new TextStyle("CITY_NAME", new GeoFont("Arial", 14), new GeoSolidBrush(GeoColor.FromHtml("#21FF00")));
        capitalFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle.YOffsetInPixel = 5;
        capitalFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        // Set DrawingMarginPercentage to a proper value to avoid some labels are cut-off
        capitalFeatureLayer.DrawingMarginInPixel = 300;

        layers.Add(capitalFeatureLayer);
    }
}
```

### Step 7: Build and launch the blazor app

After build and run the `blazor app` in browser, you should now see your map with our Cloud Maps layer and capital labels. 
