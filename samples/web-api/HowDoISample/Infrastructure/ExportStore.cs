using System.Collections.Concurrent;

namespace ThinkGeo.UI.WebApi.HowDoI
{
    /// <summary>
    /// Files a sample has made for the browser to fetch as ordinary HTTP - a PDF, a picture. Each
    /// is kept under a token for ten minutes and served by the export/{id} endpoint.
    /// </summary>
    public static class ExportStore
    {
        private static readonly ConcurrentDictionary<string, (byte[] Bytes, string ContentType, DateTime MadeAt)> Files = new ConcurrentDictionary<string, (byte[], string, DateTime)>();

        public static string Put(byte[] bytes, string contentType)
        {
            foreach (var stale in Files.Where(pair => pair.Value.MadeAt < DateTime.UtcNow.AddMinutes(-10)).Select(pair => pair.Key).ToList())
            {
                Files.TryRemove(stale, out _);
            }

            var id = Guid.NewGuid().ToString("N");
            Files[id] = (bytes, contentType, DateTime.UtcNow);
            return id;
        }

        public static bool TryGet(string id, out byte[] bytes, out string contentType)
        {
            if (id != null && Files.TryGetValue(id, out var file))
            {
                bytes = file.Bytes;
                contentType = file.ContentType;
                return true;
            }

            bytes = null;
            contentType = null;
            return false;
        }
    }
}
