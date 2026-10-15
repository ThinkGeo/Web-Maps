# ThinkGeo Blazor Web App sample

This is a minimal Blazor Web App host using **Interactive Server** rendering. It demonstrates the existing ThinkGeo Blazor map components in the current project model with three samples: Zoom to Extents, Markers, and Draw and Modify Geometries. It targets .NET 9 to match the existing `samples/blazor` application.

Set a ThinkGeo Cloud API key in configuration before running. For local development, use an environment variable so the key is not committed:

```powershell
$env:ThinkGeo__ApiKey = "your-key"
dotnet run --project .\ThinkGeo.UI.Blazor.WebAppSample.csproj
```

The app will use the URL printed by `dotnet run`. Its navigation lists the three samples. The project references the sibling `thinkgeosdk` checkout, so both repositories must retain their current relative locations. Draw and Modify Geometries writes session drawings under `Data/Temp`; those generated files are ignored by Git.
