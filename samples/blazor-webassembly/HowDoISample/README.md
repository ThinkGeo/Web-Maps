# ThinkGeo Web UI for Blazor WebAssembly Samples

All 21 samples from the former `samples/blazor/HowDoISample.Client` project, running entirely in the browser as a
standalone Blazor WebAssembly app. These pages were moved to the server gallery in commit `e2e86dd`, before the
client project was removed in `609dfa4`. This port uses their current server-gallery implementations, with server
render modes removed, and includes their images, marker model and flight animation module.

The navigation includes map navigation, overview and extent restriction; markers, animated markers and legacy
popups; raster and vector map providers; 3D perspective; globe and dateline navigation; feature projection;
advanced labels, clustering, layer toggles and label insertion; topology validation; radar animation;
measurement; and PNG export. The larger server gallery remains in `../../blazor/HowDoISample`.

Map services still require internet access. Requests originate in the browser, so custom providers must allow
cross-origin requests (CORS). The radar sample downloads frames directly from the Iowa Environmental Mesonet.

ThinkGeo draws with SkiaSharp, whose native library is linked into the app's WebAssembly, so this app needs the
`wasm-tools` workload:

```
dotnet workload install wasm-tools
dotnet run
```

Run these commands from this directory with the .NET 10 SDK. No ASP.NET server API or SignalR connection is needed.
