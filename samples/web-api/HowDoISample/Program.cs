using System.Text;
using ThinkGeo.UI.WebApi.HowDoI;
using ThinkGeo.UI.WebApi.HowDoI.Samples;

// Shapefile attribute tables can be in a Windows code page; the provider has to be registered
// before the first one is read.
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<OverlayCatalog>();

var app = builder.Build();
app.UseDefaultFiles();
// A page or script is asked for again each time and answered 304 when unchanged, so an edit
// shows on the next load; a browser left to itself keeps a page for a while.
app.UseStaticFiles(new StaticFileOptions { OnPrepareResponse = context => context.Context.Response.Headers.CacheControl = "no-cache" });

// Every sample group names the overlays it serves and the routes of its own.
var catalog = app.Services.GetRequiredService<OverlayCatalog>();
foreach (var group in SampleGroups.All)
{
    group.Register(catalog);
    group.MapEndpoints(app);
}
app.MapGalleryEndpoints();

app.Run();
