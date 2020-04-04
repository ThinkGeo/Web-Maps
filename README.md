# Web Maps

## Introduction

  Welcome, we're glad you're here!  If you're new to ThinkGeo's Web Maps we suggest that you start by taking a quick look below.  This will introduce you to getting a nice looking map up and running with some external data and styling.  After reviewing this we strongly reccomend that you open the [How Do I Sample](samples).  It's packed with dozens of examples covering nearly everything you can do with the control.

**Recap**

1. Take a quick look at the code below.
2. Open the [How Do I Sample](samples) and explore all our features. 

## Samples

  We have a number of samples and the are 

[check out HowDoI samples](samples)

## Display a Simple Map

We will begin by creating a **ASP.NET Core Web - API** project as the service and a *Blazor App* as the client to consuming the service in your favorite editor.  Next we will walk you through adding the required packages and getting a map on the default form.  Next we will add some code to show a nice looking background map and finally add some custom which will be styled and labeled.  After reading this you will be in a good position to look over the [How Do I Sample](samples) and explore our other features.

 Intro: what you'll be doing, and priming them on Product Center

### Step 1: Setup a New Project ###

  In your editor of choice you need to create a **ASP.NET Core Web - API** project and a **Balzor App** project.  Please see your editor's instructions on how to create these projects.  We have included a guide in Visual Studio below.  

[Visual Studio 2019 Example - ASP.NET Core Web - API](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-3.1&tabs=visual-studio)
[Visual Studio 2019 Example - Blazor App](https://docs.microsoft.com/en-us/aspnet/core/tutorials/build-your-first-blazor-app?view=aspnetcore-3.1)

### Step 2: Add NuGet Packages ###

You will need to install the **ThinkGeo.UI.WebApi** NuGet package in **ASP.NET Core Web - API** project and **ThinkGeo.UI.Blazor** in **Blazor App** project.  We highly suggest you use your editors [built in NuGet package manager](https://docs.microsoft.com/en-us/nuget/quickstart/) if possible.  If you're not using an IDE you can [install it via the the dotnet CLI](https://docs.microsoft.com/en-us/nuget/consume-packages/install-use-packages-dotnet-cli) from inside your project folder where where your project file exists.

**ASP.NET Core Web - API** project
```shell
dotnet add package ThinkGeo.UI.WebApi
```

**Blazor App** project
```shell
dotnet add package ThinkGeo.UI.Blazor
```

#### ASP.NET Core Web - API Project ####

### Step 3: Create "MapServiceController" by selecting "API Controller - Empty" tempate in `Controller` directory ###

Add the required usings in `MapServiceController.cs`.

```csharp
using ThinkGeo.Core;
using ThinkGeo.UI.WebApi;
```

Add an empty action method "GetTile" to serve the map tile images, following z/x/y tile system describling the earth in [Mercator projection](https://en.wikipedia.org/wiki/Mercator_projection)):

```csharp
[Route("tile/{z}/{x}/{y}")]
[HttpGet]
public HttpResponseMessage GetTile(int z, int x, int y)
{
    
}
```

### Step 4: Prepare spatial data required. ###

Download the required spatial data from [GitLab](/samples/Data) and put it into "Data" directory locating at the root project. 

### Step 5: Create the Background World Overlay ###

Add the code below to the `GetTile` action method in  `MapServiceController.cs`.

```csharp
 LayerOverlay layerOverlay = new LayerOverlay();
 ThinkGeoCloudVectorMapsLayer worldstreetsLayer = new ThinkGeoCloudVectorMapsLayer("R4RaogtPoU3aTomla-2EErvZT9OOEJBRI5OrRGeHUyk~", "sdC-p_eh9i1Q95j9LCTxHn9jldT57YVsvHJLvCmI8WAtDxd8Yn1CBg~~", ThinkGeoCloudVectorMapsMapType.Light);
 layerOverlay.Layers.Add(worldstreetsLayer);

 // Set the tile width and height as 256 pixel. A customized TileMatrix is required for other values. 
 var tileWidth = 256;
 var tileHeight = 256;

 // Set the Map Unit.
 var tileMapUnit = GeographyUnit.Meter;

 // Draw the LayerOverlay to the image and serve the clients as "image/png".
 using (GeoImage bitmap = new GeoImage(tileWidth, tileHeight))
 {
     GeoCanvas geoCanvas = GeoCanvas.CreateDefaultGeoCanvas();
     RectangleShape boundingBox = WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, tileMapUnit);
     geoCanvas.BeginDrawing(bitmap, boundingBox, tileMapUnit);
     layerOverlay.Draw(geoCanvas);
     geoCanvas.EndDrawing();
     MemoryStream ms = new MemoryStream();
     bitmap.Save(ms, GeoImageFormat.Png);
     HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
     response.Content = new ByteArrayContent(ms.ToArray());
     response.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
     return response;
 }
```

#### Blazor App Project ####


### Step 6: Run the Sample & Register for Your Free Evaluation ###

The first time you run your application, you will be presented with ThinkGeo's Product Center which will create and manage your licenses for all of ThinkGeo's products. Create a new account to begin a 60-day free evaluation. 

1. Run the application in Debug mode.

1. Click the "Create a new Account?" link.

1. Fill out your name, email address, password and company name and click register.

1. Check your email and click the "Active Your Account" link.

1. Return to Product Center and login using the credentials your just created and hit "Continue Debugging" button.

You should now see your map with our Cloud Maps layer!

### Step 6: Add a Point Data Layer ###

Add `FeatureSource` to map

```csharp
var capitolLayer ...
```

### Step 6: Add Styling and Labeling to the Points ###

Add styling to feature layer

```csharp
var capitolStyle ...
```


