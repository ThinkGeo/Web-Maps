using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace ThinkGeo.UI.Blazor.HowDoI
{
    public class SourceCodeLoader
    {
        private readonly string contentRoot;

        public SourceCodeLoader(IWebHostEnvironment environment)
        {
            contentRoot = environment.ContentRootPath;
        }

        public async Task<string> LoadRazorCodeAsync(string sample)
        {
            if (string.IsNullOrWhiteSpace(sample) || !Regex.IsMatch(sample, "^[A-Za-z0-9_]+$"))
                return string.Empty;

            var folders = new[]
            {
                Path.Combine(contentRoot, "Pages"),
                Path.GetFullPath(Path.Combine(contentRoot, "..", "HowDoISample.Client", "Pages")),
                Path.Combine(contentRoot, "ClientPages")
            };
            foreach (var folder in folders)
            {
                var file = Path.Combine(folder, sample + ".razor");
                if (File.Exists(file)) return await File.ReadAllTextAsync(file);
            }
            return string.Empty;
        }
    }
}
