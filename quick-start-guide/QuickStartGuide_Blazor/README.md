# Blazor Quick Start

This sample follows the [Web Maps Quick Start](https://docs-v15.thinkgeo.com/products/web-maps/quickstart/). It displays the ThinkGeo Cloud Light vector basemap and the WorldCapitals shapefile as white circles with black outlines.

## Run

Install the .NET 10 SDK, then run from this directory:

```sh
dotnet run --project BlazorSample/BlazorSample.csproj
```

Open the localhost URL printed in the terminal. The sample uses ThinkGeo.UI.Blazor 15.0.1. A ThinkGeo runtime license and a ThinkGeo Cloud API key are separate: the Cloud key requests the basemap, while the runtime license controls trial behavior.

## Data and rendering

The five `WorldCapitals` files are directly in `BlazorSample/Data`. The project copies them to the build and publish output. `Home.razor` reads the shapefile with `ShapeFileFeatureSource`, reprojects EPSG:4326 to EPSG:3857, and supplies it to `VectorTileOverlay`. `StyleJson` draws the capital points.

The screenshots in `assets` show the running sample. To check the basemap alone, temporarily omit the `VectorTileOverlay` element from `Home.razor`.
