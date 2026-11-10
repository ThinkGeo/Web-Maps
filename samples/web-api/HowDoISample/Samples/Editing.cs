using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using ThinkGeo.Core;

namespace ThinkGeo.UI.WebApi.HowDoI.Samples
{
    /// <summary>
    /// Edit features and save them: the browser draws and reshapes Frisco's parks, and the server
    /// writes the changes into a shapefile in one transaction. Every visitor edits a copy of their
    /// own, so nobody sees another's changes.
    /// </summary>
    public class Editing : ISampleGroup
    {
        private static readonly string Copies = Path.Combine(Path.GetTempPath(), "howdoi-edits");
        private static readonly ConcurrentDictionary<string, object> Locks = new();
        private const long MaxCopiesSize = 100 * 1024 * 1024;

        public void Register(OverlayCatalog catalog)
        {
        }

        public void MapEndpoints(IEndpointRouteBuilder app)
        {
            app.MapGet("/samples/editing/parks", (string session) =>
            {
                if (!Valid(session)) return Results.BadRequest();
                lock (Locks.GetOrAdd(session, _ => new object()))
                {
                    var parks = Parks(session, FileAccess.Read);
                    parks.Open();
                    var features = parks.GetAllFeatures(new[] { "NAME", "ACRES" });
                    parks.Close();
                    return Results.Text(GeoJson.Collection(features), "application/json");
                }
            });

            // The changes the browser kept since the last save, written in one transaction: the
            // shapes arrive in degrees, the source converts them to the file's state plane feet.
            app.MapPost("/samples/editing/save", (JsonElement body) =>
            {
                var session = body.GetProperty("session").GetString();
                if (!Valid(session)) return Results.BadRequest();
                lock (Locks.GetOrAdd(session, _ => new object()))
                {
                    var parks = Parks(session, FileAccess.ReadWrite);
                    parks.Open();
                    parks.BeginTransaction();
                    foreach (var added in body.GetProperty("added").EnumerateArray())
                    {
                        var shape = GeoJson.Shape(added.GetProperty("geometry"));
                        parks.AddFeature(new Feature(shape, new Dictionary<string, string> { ["NAME"] = added.GetProperty("name").GetString(), ["ACRES"] = Acres(parks, shape) }));
                    }
                    // A park renamed keeps its shape and its acres; a park reshaped is measured again.
                    foreach (var updated in body.GetProperty("updated").EnumerateArray())
                    {
                        var feature = parks.GetFeatureById(updated.GetProperty("id").GetString(), ReturningColumnsType.AllColumns);
                        var shape = feature.GetShape();
                        if (updated.TryGetProperty("geometry", out var geometry))
                        {
                            shape = GeoJson.Shape(geometry);
                            feature.ColumnValues["ACRES"] = Acres(parks, shape);
                        }
                        feature.ColumnValues["NAME"] = updated.GetProperty("name").GetString();
                        parks.UpdateFeature(new Feature(shape, feature.ColumnValues) { Id = feature.Id });
                    }
                    foreach (var deleted in body.GetProperty("deleted").EnumerateArray())
                    {
                        parks.DeleteFeature(deleted.GetString());
                    }
                    var result = parks.CommitTransaction();
                    parks.Close();
                    RemoveOldCopies();
                    return Results.Json(new { saved = result.TotalSuccessCount, failed = result.TotalFailureCount });
                }
            });

            app.MapPost("/samples/editing/reset", (JsonElement body) =>
            {
                var session = body.GetProperty("session").GetString();
                if (!Valid(session)) return Results.BadRequest();
                lock (Locks.GetOrAdd(session, _ => new object()))
                {
                    var folder = Path.Combine(Copies, session);
                    if (Directory.Exists(folder)) Directory.Delete(folder, true);
                    return Results.Ok();
                }
            });
        }

        // Measured in the file's own state plane feet, not in the map's Web Mercator.
        private static string Acres(FeatureSource parks, BaseShape shape)
        {
            var feet = (AreaBaseShape)parks.ProjectionConverter.ConvertToInternalProjection(shape);
            return feet.GetArea(GeographyUnit.Feet, AreaUnit.Acres).ToString("0.000", CultureInfo.InvariantCulture);
        }

        // The visitor's copy of the parks shapefile, made from the original the first time they come.
        private static ShapeFileFeatureSource Parks(string session, FileAccess access)
        {
            var folder = Path.Combine(Copies, session);
            if (!Directory.Exists(folder))
            {
                RemoveOldCopies();
                Directory.CreateDirectory(folder);
                foreach (var file in Directory.GetFiles(Path.GetDirectoryName(SampleData.Path("Shapefile/Parks.shp")), "Parks.*"))
                {
                    File.Copy(file, Path.Combine(folder, Path.GetFileName(file)));
                }
            }
            Directory.SetLastWriteTimeUtc(folder, DateTime.UtcNow);
            return new ShapeFileFeatureSource(Path.Combine(folder, "Parks.shp"), access) { ProjectionConverter = new ProjectionConverter(2276, 3857) };
        }

        // Copies left alone for a day go, then the oldest ones until all of them fit in 100 MB.
        // A copy someone is reading or saving right now is skipped.
        private static void RemoveOldCopies()
        {
            if (!Directory.Exists(Copies)) return;
            var copies = new DirectoryInfo(Copies).GetDirectories()
                .Select(folder => (Folder: folder, Size: folder.GetFiles().Sum(file => file.Length)))
                .OrderBy(copy => copy.Folder.LastWriteTimeUtc).ToList();
            var total = copies.Sum(copy => copy.Size);
            foreach (var copy in copies)
            {
                if (copy.Folder.LastWriteTimeUtc >= DateTime.UtcNow.AddDays(-1) && total <= MaxCopiesSize) break;
                var gate = Locks.GetOrAdd(copy.Folder.Name, _ => new object());
                if (!Monitor.TryEnter(gate)) continue;
                try { copy.Folder.Delete(true); total -= copy.Size; } catch (IOException) { } finally { Monitor.Exit(gate); }
            }
        }

        private static bool Valid(string session) => session != null && Regex.IsMatch(session, "^[a-z0-9]{16,40}$");
    }
}
