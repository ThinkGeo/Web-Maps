using System.IO;
using System.Threading.Tasks;

namespace ThinkGeo.UI.Blazor.HowDoI
{
    public class SourceCodeLoader
    {
        public Task<string> LoadRazorCodeAsync(string sample)
        {
            var razorFile = Path.Combine(Directory.GetCurrentDirectory(), "Pages", $"{sample}.razor");
            if (!File.Exists(razorFile)) return Task.FromResult(string.Empty);

            return File.ReadAllTextAsync(razorFile);
        }

        public Task<string> LoadCsharpCodeAsync(string sample)
        {
            var razorFile = Path.Combine(Directory.GetCurrentDirectory(), "Pages", $"{sample}.cs.razor");
            if (!File.Exists(razorFile)) return Task.FromResult(string.Empty);

            return File.ReadAllTextAsync(razorFile);
        }
    }
}
