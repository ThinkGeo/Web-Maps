using ThinkGeo.Core;

namespace ThinkGeo.UI.Blazor.HowDoI
{
    public class StylingModel
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public PointShape Center { get; set; }

        public int Zoom { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public GeoCollection<Layer> Layers { get; set; }
    }
}
