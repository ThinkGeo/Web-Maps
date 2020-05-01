# WebAPI Guide

## Quick Start: Display a Simple Map

We will begin by creating an **ASP.NET Core Web - API** project as the service and a simple HTML-based sample with [Leaflets](https://leafletjs.com/) to consume the service in your favorite editor. Next, we will walk you through adding the required packages and getting a Restful map service.  Then, we will add some code to show a nice looking background map, and finally, add some custom data to the map and style it.  After reading this, you will be in a good position to look over the [How Do I Sample](samples/web-api) and explore our other features.

![alt text](assets/quickstart_webmaps_webapi_screenshot.png "Simple Map")

### Step 1: Setup a New Project

In your editor of choice you need to create a **ASP.NET Core Web - API** project.  Please see your editor's instructions on how to create this kind of project.  We have included a guide in Visual Studio below.

[Visual Studio 2019 Example - ASP.NET Core Web - API](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-3.1&tabs=visual-studio)

### Step 2: Add NuGet Packages

You will need to install the **ThinkGeo.UI.WebApi** NuGet package in your **ASP.NET Core Web - API** project.  We highly suggest you use your editors [built in NuGet package manager](https://docs.microsoft.com/en-us/nuget/quickstart/) if possible.  If you're not using an IDE you can [install it via the the dotnet CLI](https://docs.microsoft.com/en-us/nuget/consume-packages/install-use-packages-dotnet-cli) from inside your project folder where where your project file exists.


```shell
Install-Package ThinkGeo.UI.WebApi
```

### Step 3: Create "MapServiceController" by selecting "API Controller - Empty" template in `Controller` directory

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

### Step 4: Prepare spatial data required

Download the required spatial data from [GitLab](/samples/data) and put it into "Data" directory locating at the root project.

### Step 5: Create HTML page "default.html" as client to consume the service

Add another pure THML page "default.html" with following steps to consume the map tiles created previously.  
**NOTE:** The added "default.html" can be in created **ASP.NET Core Web - API** project, but please make sure run it from another place, such as in Visual Studio Code to make sure it doesn't share the same port of map service created. 

 **Include Leaflet CSS file in the head section of your document:**
```html
 <link rel="stylesheet" href="https://unpkg.com/leaflet@1.6.0/dist/leaflet.css" />
```

**Include Leaflet JavaScript file after Leaflet’s CSS:**
```html
 <!-- Make sure you put this AFTER Leaflet's CSS -->
 <script src="https://unpkg.com/leaflet@1.6.0/dist/leaflet.js></script>
```

**Put a div element with a certain id where you want your map to be:**
```html
 <div id="mapdiv"></div>
```

**Make sure the map container has a defined width and height, for example by setting it in CSS:**
```css
#mapdiv { width: 800px; height:600px }
```

Now you’re ready to initialize the map and consume the map tile services with following code:

```javascript
 <script>
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
     var map = L.map('mapDiv').setView([33.1010, -96.8134], 4);

     // Add ThinkGeoCloudMaps as the base map.
     var thinkgeoCloudMapsLayer = L.tileLayer('https://{s}.thinkgeo.com/api/v1/maps/raster/aerial/x1/3857/512/{z}/{x}/{y}.jpeg?apikey=YOUR CLOUD API KEY', {
         subdomains: ['cloud1', 'cloud2', 'cloud3', 'cloud4', 'cloud5', 'cloud6'],
         layers: 'ThinkGeoCloudMaps',
         format: 'image/png',
         styles: 'aerial',
         version: '1.1.1'
     });
     thinkgeoCloudMapsLayer.addTo(map);


     // Add base street layer without any label from the web API controller of this Project.
     var capitalLayer = L.tileLayer('https://localhost:44350/mapservice/{z}/{x}/{y}').addTo(map);
 </script>
```

Make sure all the code is called after the div and leaflet.js inclusion. That’s it! You have a working Leaflet map now.


The first time you run your **ASP.NET Core Web - API** project, you will be presented with ThinkGeo's Product Center which will create and manage your licenses for all of ThinkGeo's products. Create a new account to begin a 60-day free evaluation.

1. Run the application in Debug mode.

1. Click the "Create a new Account?" link.

1. Fill out your name, email address, password and company name and click register.

1. Check your email and click the "Active Your Account" link.

1. Return to Product Center and login using the credentials your just created and hit "Continue Debugging" button.

You should now see your map with our Cloud Maps layer!
