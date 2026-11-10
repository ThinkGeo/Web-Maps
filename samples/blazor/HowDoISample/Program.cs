using Microsoft.AspNetCore.SignalR;
using ThinkGeo.UI.Blazor.HowDoI;
using ThinkGeo.UI.Blazor.HowDoI.Client;
using ThinkGeo.UI.Blazor.HowDoI.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.Configure<HubOptions>(options => options.MaximumReceiveMessageSize = 16 * 1024 * 1024);
builder.Services.AddSingleton<MenuService>();
builder.Services.AddSingleton<DemographicMapService>();
builder.Services.AddSingleton<SourceCodeLoader>();
builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapGet("/export/{id}", (string id) =>
    ExportStore.TryGet(id, out var bytes, out var contentType)
        ? Results.File(bytes, contentType)
        : Results.NotFound());

app.MapGet("/source/{sample}", async (string sample, SourceCodeLoader loader) =>
{
    var source = await loader.LoadRazorCodeAsync(sample);
    return source.Length > 0 ? Results.Text(source, "text/plain") : Results.NotFound();
});

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ClientAssemblyMarker).Assembly);

app.Run();
