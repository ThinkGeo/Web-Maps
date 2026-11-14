# ThinkGeo Web UI for Blazor WebAssembly Samples

A few of the Blazor samples, running entirely in the browser as a standalone Blazor WebAssembly app. The full set of
samples is the Blazor Server app in `../../blazor/HowDoISample`, which needs nothing beyond the .NET SDK.

ThinkGeo draws with SkiaSharp, whose native library is linked into the app's WebAssembly, so this app needs the
`wasm-tools` workload:

```
dotnet workload install wasm-tools
dotnet run
```
