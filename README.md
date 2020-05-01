# Web Maps

## Repository Layout

`/api-docs`: An offline version the API documentation HTML pages.

`/samples`: A collection of feature by feature samples.  We suggest you start with the [How Do I Sample](samples/web-api) as it shows dozens of features in one easy to navigate app.

`/assets`: Any assets needed for the readme.md.

`README.md`: A quick start guide to show you how to quickly get up and running.

## Samples

We have a number of samples for both WebAPI and Blazor that show off ThinkGeo Web Maps' full capabilities. You can use these samples as a starting point for your own application, or simply reference them for how to use our controls using best practices.

- [WebAPI samples](samples/wep-api)
- [Blazor samples](samples/blazor)

## Quickstart Guides

- [WebAPI Quickstart](#quick-start-display-a-simple-map-on-webapi)
- [Blazor Quickstart](#quick-start-display-a-simple-map-on-blazor)

## Quick Start: Display a Simple Map on Blazor

We will begin by creating an **ASP.NET Core Web - API** project as the service and a **Blazor App** as the client to consuming the service in your favorite editor.  Next, we will walk you through adding the required packages and getting a map on the default form.  Then, we will add some code to show a nice looking background map, and finally, add some custom data to the map and style it.  After reading this, you will be in a good position to look over the [How Do I Sample](samples) and explore our other features.

![alt text](assets/quickstart_webmaps_shapefile_pointstyle_screenshot.png "Simple Map")

### Step 1: Setup a New Project

In your editor of choice you need to create a **ASP.NET Core Web - API** project and a **Blazor App** project.  Please see your editor's instructions on how to create these projects.  We have included a guide in Visual Studio below.

[Visual Studio 2019 Example - ASP.NET Core Web - API](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-3.1&tabs=visual-studio)

[Visual Studio 2019 Example - Blazor App](https://docs.microsoft.com/en-us/aspnet/core/blazor/get-started?view=aspnetcore-3.1&tabs=visual-studio&viewFallbackFrom=aspnetcore-3.0)

### Step 2: Add NuGet Packages

You will need to install the **ThinkGeo.UI.WebApi** NuGet package in **ASP.NET Core Web - API** project and **ThinkGeo.UI.Blazor** in **Blazor App** project.  We highly suggest you use your editors [built in NuGet package manager](https://docs.microsoft.com/en-us/nuget/quickstart/) if possible.  If you're not using an IDE you can [install it via the the dotnet CLI](https://docs.microsoft.com/en-us/nuget/consume-packages/install-use-packages-dotnet-cli) from inside your project folder where where your project file exists.

**ASP.NET Core Web - API** project

```shell
Install-Package ThinkGeo.UI.WebApi
```

**Blazor App** project

```shell
Install-Package ThinkGeo.UI.Blazor
```

### Step 3: Create "MapServiceController" by selecting "API Controller - Empty" template in `Controller` directory

Add the required usings in `MapServiceController.cs`.

```csharp
using ThinkGeo.Core;
```

Add an action method "GetCapitalData" with code below to serve features saved in demo data "capital.shp", following the [Mercator projection](https://en.wikipedia.org/wiki/Mercator_projection):

```csharp
[Route("getcapitals")]
[HttpGet]
public string GetCapitalData()
{
    // Capital layer
    var capitalSource = new ShapeFileFeatureSource(Path.Combine(AppContext.BaseDirectory, "../../../Data/capital.shp"));

    // Create the "Mercator projection" and apply it to the layer.
    var proj4 = new ProjectionConverter(Projection.GetWgs84ProjString(), Projection.GetSphericalMercatorProjString());
    if (!proj4.IsOpen) proj4.Open();
    capitalSource.ProjectionConverter = proj4;

    // Query all features with all attributes.
    capitalSource.Open();
    var queriedFeatures = capitalSource.GetAllFeatures(ReturningColumnsType.AllColumns);

    // Convert it to GeoJSON collection for return.
    GeometryCollectionShape returnedFeatures = new GeometryCollectionShape(queriedFeatures);
    return returnedFeatures.GetGeoJson();
}
```

### Step 4: Prepare spatial data required

Download the required spatial data from [GitLab](/samples/data) and put it into "Data" directory locating at the root project.

### Step 5: Create the HTML container of mapView

Open `~/Imports.razor` and add 2 using directives to reference the Map components.

```csharp
@using ThinkGeo.Core
@using ThinkGeo.UI.Blazor
```

Open the `~/Pages/_Host.cshtml` and add the stylesheet and javascript references of ThinkGeo Web For Blazor from CDN:

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

Open `~/Pages/Index.razor` file and add a ThinkGeo.UI.Blazor.Map component as following:
@page "/"

```html
<!--You can control the width and height in CSS too, if you like. -->
<MapView Id="demomap" MapUnit="@ThinkGeo.Core.GeographyUnit.Meter" Width="800" Height="600"  Zoom="3" Center="@(new PointShape(1056665.479014, 6066042.564712))">
    <OverlaysSetting>
        <!--Add the background world map-->
        <ThinkGeoCloudRasterMapsOverlay Id="RasterOverlay" MapType="ThinkGeo.Core.ThinkGeoCloudRasterMapsMapType.Light" ApiKey="Input your key"></ThinkGeoCloudRasterMapsOverlay>
    </OverlaysSetting>
</MapView>
```

### Step 6: Add a Point Data Layer

Add point capital `FeatureSource` to the map by adding one more `LayerOverlay` in MapView's  `OverlaysSetting`:

```html
<!--Add layer for hosting the point capital data requested from WebAPI project-->
<ThinkGeo.UI.Blazor.LayerOverlay Id="MapServiceOverlay" Layers="layers"></ThinkGeo.UI.Blazor.LayerOverlay>
```

And create an `InMemoryFeatureLayer` to host the point features pulling from the "GetCapitalData" action for render:

```csharp
@code {
    MapView map;
    GeoCollection<Layer> layers = new GeoCollection<Layer>();

    protected override void OnInitialized()
    {
        // URI of the GetCapitalData service endpoint.
        var geoJsonRequestUri = $"https://localhost:44316/api/MapService/getcapitals";

        // Submit the  request and pull capital point data down in geoJson format.
        var httpClient = new HttpClient();
        var requestBody = httpClient.GetStringAsync(geoJsonRequestUri).Result;
        var capitalPoints = (GeometryCollectionShape)GeometryCollectionShape.CreateShapeFromGeoJson(requestBody);

        // Create an InMemoryFeatureLayer to host the point features for render.
        InMemoryFeatureLayer capitalPointLayer = new InMemoryFeatureLayer();
        foreach (var capitalPoint in capitalPoints.Shapes)
        {
            capitalPointLayer.InternalFeatures.Add(new Feature(capitalPoint));
        }
        capitalPointLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyle.CreateSimpleCircleStyle(GeoColors.Green, 8);
        capitalPointLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

        layers.Add(capitalPointLayer);
    }
}
```

### Step 7: Build and launch the blazor app

To make sure the **ASP.NET Core Web - API** project is running while the `Blazor App` is started, please open the "*.sln" solution property by "right-click solution -> Properties -> Common Properties:Startup Project -> Checked `Multiple startup projects`", and then press "F5" to launch the sample.

The first time you run your application, you will be presented with ThinkGeo's Product Center which will create and manage your licenses for all of ThinkGeo's products. Create a new account to begin a 60-day free evaluation.

1. Run the application in Debug mode.

1. Click the "Create a new Account?" link.

1. Fill out your name, email address, password and company name and click register.

1. Check your email and click the "Active Your Account" link.

1. Return to Product Center and login using the credentials your just created and hit "Continue Debugging" button.

You should now see your map with our Cloud Maps layer!

## Quick Start: Display a Simple Map on WebAPI

We will begin by creating an **ASP.NET Core Web - API** project as the service and a simple HTML-based sample with [Leaflet](https://leafletjs.com/) to consume the service. Next, we will walk you through adding the required packages and getting a Restful map service.  Then, we will add some code to show a nice looking background map, and finally, add some custom data to the map and style it. After reading this, you will be in a good position to look over the [How Do I Sample](samples/web-api) and explore our other features.

![alt text](assets/quickstart_webmaps_webapi_screenshot.png "Simple Map")

### Step 1: Setup a New Project

In your editor of choice you need to create a **ASP.NET Core Web - API** project.  Please see your editor's instructions on how to create this kind of project.  We have included a guide in Visual Studio below.

[Visual Studio 2019 Example - ASP.NET Core Web - API](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-3.1&tabs=visual-studio)

### Step 2: Add NuGet Packages

You will need to install the **ThinkGeo.UI.WebApi** NuGet package in your **ASP.NET Core Web - API** project.  We highly suggest you use your editors [built in NuGet package manager](https://docs.microsoft.com/en-us/nuget/quickstart/) if possible.  If you're not using an IDE you can [install it via the the dotnet CLI](https://docs.microsoft.com/en-us/nuget/consume-packages/install-use-packages-dotnet-cli) from inside your project folder where where your project file exists.

```shell
Install-Package ThinkGeo.UI.WebApi
```

### Step 3: Prepare spatial data required

Download the required spatial data from [GitLab](assets/capital.zip) and extract its contents into "Data" directory located at the root project.

### Step 4: Create the Map API Controller

Create "MapServiceController" by selecting "API Controller - Empty" template in the `Controller` directory

Add the required usings in `MapServiceController.cs`.

```csharp
using ThinkGeo.Core;
```

Add an action method "GetTile" with code below to serve map tiles drawn with map data "capital.shp", following the [Mercator projection](https://en.wikipedia.org/wiki/Mercator_projection):

```csharp
 [Route("{z}/{x}/{y}")]
 [HttpGet]
 public IActionResult GetTile(int z, int x, int y)
 {
     // Create the LayerOverlay for displaying the map.
     LayerOverlay capitalOverlay = new LayerOverlay();

     // Create the point layer for capitals.
     ShapeFileFeatureLayer capitalFeatureLayer = new ShapeFileFeatureLayer(Path.Combine(AppContext.BaseDirectory, "../../../Data/capital.shp"));
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
     capitalOverlay.Layers.Add("street", capitalFeatureLayer);

     using (GeoImage image = new GeoImage(256, 256))
     {
         GeoCanvas geoCanvas = GeoCanvas.CreateDefaultGeoCanvas();
         RectangleShape boundingBox = WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, GeographyUnit.Meter);
         geoCanvas.BeginDrawing(image, boundingBox, GeographyUnit.Meter);

         if (capitalOverlay != null)
         {
             capitalOverlay.Draw(geoCanvas);
         }
         geoCanvas.EndDrawing();
         byte[] imageBytes = image.GetImageBytes(GeoImageFormat.Png);

         return File(imageBytes, "image/png");
     }
 }
```

### Step 5: Create HTML page "default.html" as client to consume the service

Add another pure HTML page "default.html" with following steps to consume the map tiles created previously.

> **NOTE:** The added "default.html" can be in created **ASP.NET Core Web - API** project, but please make sure run it from another place, such as in Visual Studio Code to make sure it doesn't share the same port of map service created.

 **Include Leaflet CSS file in the `<head>` section of your document:**

```html
 <link rel="stylesheet" href="https://unpkg.com/leaflet@1.6.0/dist/leaflet.css" />
```

**Include Leaflet JavaScript file after Leaflet’s CSS:**

```html
 <!-- Make sure you put this AFTER Leaflet's CSS -->
 <script src="https://unpkg.com/leaflet@1.6.0/dist/leaflet.js></script>
```

**Put a `<div>` element with an id of `map` in the html's `<body>`:**

```html
 <div id="map"></div>
```

**Make sure the map container has a defined width and height, for example by setting it in CSS:**

```css
#map { width: 800px; height:600px }
```

**Add a `<script>` element below the map div**

```html
<script>
    // JS code goes here
</script>
```

Now you’re ready to initialize the map and consume the map tile services with following code:

```javascript
// Add the utility methods to L.Util
L.Util.getRootPath = function() {
    var pathArray = location.pathname.split('/');
    var appPath = "/";
    for (var i = 1; i < pathArray.length - 1; i++) {
        appPath += pathArray[i] + "/";
    }

    return appPath === "/" ? "" : appPath;
};

// Create the map.
var map = L.map('map').setView([33.1010, -96.8134], 4);

//
var cloudApiKey = 'YOUR THINKGEO CLOUD MAPS API KEY';
// Add ThinkGeoCloudMaps as the base map.
var thinkgeoCloudMapsLayer = L.tileLayer('https://{s}.thinkgeo.com/api/v1/maps/raster/aerial/x1/3857/512/{z}/{x}/{y}.jpeg?apikey=' + cloudApiKey, {
    subdomains: ['cloud1', 'cloud2', 'cloud3', 'cloud4', 'cloud5', 'cloud6'],
    layers: 'ThinkGeoCloudMaps',
    format: 'image/png',
    styles: 'aerial',
    version: '1.1.1'
});
thinkgeoCloudMapsLayer.addTo(map);

// Add base street layer without any label from the web API controller of this Project.
var capitalLayer = L.tileLayer('https://localhost:44350/mapservice/{z}/{x}/{y}').addTo(map);
```

That’s it! You have a working Leaflet map now.

The first time you run your **ASP.NET Core Web - API** project, you will be presented with ThinkGeo's Product Center which will create and manage your licenses for all of ThinkGeo's products. Create a new account to begin a 60-day free evaluation.

1. Run the application in Debug mode.
1. Click the "Create a new Account?" link.
1. Fill out your name, email address, password and company name and click register.
1. Check your email and click the "Active Your Account" link.
1. Return to Product Center and login using the credentials your just created and hit "Continue Debugging" button.

You should now see your map with our Cloud Maps layer!
