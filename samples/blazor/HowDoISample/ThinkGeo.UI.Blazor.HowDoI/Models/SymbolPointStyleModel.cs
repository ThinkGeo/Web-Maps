using ThinkGeo.Core;

namespace ThinkGeo.UI.Blazor.HowDoI
{
    public class SymbolPointStyleModel
    {
        public PointSymbolType Type { get; set; }

        public float Size { get; set; }

        public float RotationAngle { get; set; }

        public string PenColor { get; set; }

        public byte PenAlpha { get; set; }

        public float PenWidth { get; set; }

        public string BrushColor { get; set; }

        public byte BrushAlpha { get; set; }
    }
}
