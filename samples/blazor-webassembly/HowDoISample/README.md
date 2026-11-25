# ThinkGeo Web UI for Blazor WebAssembly Samples

Ten focused samples running entirely in the browser as a standalone Blazor WebAssembly app.

The gallery covers Map Navigation, Vector Map Providers, Raster Map Providers, Render Points, Lines and Polygons,
Advanced Labels, Markers & Popups, Cluster Points, Project Features, Measure Distance and Area, and PNG export.
Markers & Popups includes static and animated markers, removal, animation controls and rich detail popups.
Advanced Labels includes seven lessons, including multi-column town labels. The basic styling lesson uses small
in-memory datasets. The larger server gallery is in `../../blazor/HowDoISample`.

The app uses the server gallery's header, a flat list of sample names, sample descriptions and resizable Code
panel. The index route opens Map Navigation. These 10 samples appear in the menu; `wwwroot/menus.json`
and the sample Razor sources are embedded at build time so the gallery and Code panel need no server endpoints.
The sidebar follows direct links and browser history, and collapses behind the navigation button on small screens.

Map services still require internet access. Requests originate in the browser, so custom providers must allow
cross-origin requests (CORS).

ThinkGeo draws with SkiaSharp, whose native library is linked into the app's WebAssembly, so this app needs the
`wasm-tools` workload:

```
dotnet workload install wasm-tools
dotnet run
```

Run these commands from this directory with the .NET 10 SDK. No ASP.NET server API or SignalR connection is needed.
