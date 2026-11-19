# Web API Quick Start

This sample follows the [Web Maps Quick Start](https://docs-v15.thinkgeo.com/products/web-maps/quickstart/): MapLibre draws the ThinkGeo Cloud Light vector basemap and WorldCapitals as white circles with black outlines.

## Run locally

Install the .NET 10 SDK. From this directory:

```sh
dotnet run --project WebApiSample/WebApiSample.csproj
```

Open the localhost URL printed in the terminal. The page is served from `WebApiSample/wwwroot/index.html`; no separate frontend server or CORS configuration is needed. The included ThinkGeo Cloud key is for testing only. Replace it with your own for production; it is separate from a ThinkGeo runtime license.

## Data and tiles

The five WorldCapitals files are directly inside `WebApiSample/Data`, and are copied to build and publish output. `Program.cs` reprojects the shapefile from EPSG:4326 to EPSG:3857 and keeps one `VectorTileOverlay` for tile requests. `/tiles/capitals/{z}/{x}/{y}` returns gzipped Mapbox vector tiles, or HTTP 204 for an empty tile. MapLibre styles the `WorldCapitals` source layer in the browser.

## Docker

Run from this directory:

```sh
docker build -f WebApiSample/Dockerfile -t thinkgeo-webapi-quickstart .
docker run --rm -p 8080:8080 thinkgeo-webapi-quickstart
```

Open http://localhost:8080. The image runs as the non-root `app` user. ThinkGeo license checks happen during the image build; see the documentation's license guide for container build licensing.
