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

### Step 3: Create "MapServiceController" by selecting "API Controller - Empty" tempate in `Controller` directory ###

Add the new namespace to the `MainWindow.xaml` exisiting XAML namespaces.

```xml
xmlns:uc1="clr-namespace:ThinkGeo.UI.Wpf;assembly=ThinkGeo.UI.Wpf"
```

Add the control to `MainWindow.xaml` file inside the Grid element.

```xml
<uc1:MapView x:Name="mapView" Loaded="mapView_Loaded"></uc1:MapView>
```

### Step 4: Add Namespaces and the MapViewLoaded Event to `MainWindow.xaml.cs` ###

Add the required usings.

```csharp
using ThinkGeo.Core;
using ThinkGeo.UI.Wpf;
```

Setup the `mapView_Loaded` event which will run when the map control is finsihed being loaded.

```csharp
private void mapView_Loaded(object sender, RoutedEventArgs e)
{
    // Set the Map Unit.
    mapView.MapUnit = GeographyUnit.Meter;
}
```

### Step 5: Add the Background World Overlay ###

Add the code below to the `mapView_Loaded` event of the `MainWindow.xaml.cs`.

```csharp
  // Set the Map Unit.
  mapView.MapUnit = GeographyUnit.Meter;

  // Add a base map overlay.
  var cloudVectorBaseMapOverlay = new ThinkGeoCloudVectorMapsOverlay("USlbIyO5uIMja2y0qoM21RRM6NBXUad4hjK3NBD6pD0~", "f6OJsvCDDzmccnevX55nL7nXpPDXXKANe5cN6czVjCH0s8jhpCH-2A~~", ThinkGeoCloudVectorMapsMapType.Light);
  mapView.Overlays.Add(cloudVectorBaseMapOverlay);

  mapView.CurrentExtent = new RectangleShape(-20000000, 20000000, 20000000, -20000000);
```

### Step 5: Run the Sample & Register for Your Free Evaluation ###

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


