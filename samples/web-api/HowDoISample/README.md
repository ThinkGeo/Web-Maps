# ThinkGeo Web Maps for WebAPI - How Do I samples

The map is drawn in the browser by MapLibre GL; an ASP.NET Core Web API on ThinkGeo.UI.WebApi and
ThinkGeo.Core serves what the browser cannot make itself:

- **vector tiles** cut from any feature source the SDK reads - shapefiles, databases, WFS and OGC
  API services, NOAA feeds, features built in code - at `tiles/{overlay}/{z}/{x}/{y}.mvt`
  (`VectorTileOverlay`), or handed out of an MBTiles / PMTiles archive;
- **raster tiles** drawn by classic layers and styles at `raster/{overlay}/{z}/{x}/{y}.png`
  (`LayerOverlay.GetTileImage`), WMS and WMTS included;
- **adornments** - legend, scale line and bar, magnetic declination, logo - drawn as one picture
  the size of the map at `adornments/{overlay}?width&height&bbox` (`AdornmentOverlay`);
- **answers**: geometry, spatial queries, topology rules, projections, ThinkGeo Cloud services,
  interpolated fields, PDF and image export - each a small JSON route under `samples/...`.

Every sample is one page under `wwwroot/samples/` and, when it needs the server, one class under
`Samples/`; the gallery shows both beside the map. The Legacy group carries the classic WebAPI
samples over onto the same routes.

## Run it

```
dotnet run
```

then open the address it prints. The sample data is the Blazor gallery's, linked in through the
project file and copied next to the binaries on build; the Legacy group's own data lives under
`Data/Legacy`.
