using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ThinkGeo.UI.Blazor.HowDoI
{
    public class SourceCodeLoader
    {
        public async Task<string> LoadRazorCodeAsync(string sample)
        {
            if (string.IsNullOrWhiteSpace(sample) || !Regex.IsMatch(sample, "^[A-Za-z0-9_]+$"))
                return string.Empty;

            var root = Directory.GetCurrentDirectory();
            var folders = new[]
            {
                Path.Combine(root, "Pages"),
                Path.GetFullPath(Path.Combine(root, "..", "HowDoISample.Client", "Pages")),
                Path.Combine(root, "ClientPages")
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
