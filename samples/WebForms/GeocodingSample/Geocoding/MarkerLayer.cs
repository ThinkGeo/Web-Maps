using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace ThinkGeo.MapSuite.HowDoI
{
    class MarkerLayer : Layer
    {
        private PointShape markerLocation;
        private string markerFile;
       

        public MarkerLayer(string markerFile)
        {
            this.markerFile = markerFile;
        }

        public PointShape MarkerLocation
        {
            get
            {
                return markerLocation;
            }
            set
            {
                markerLocation = value;
            }
        }

        protected override void DrawCore(GeoCanvas canvas, Collection<SimpleCandidate> labelsInAllLayers)
        {            
            MemoryStream memoryStream = new MemoryStream();
            Bitmap markerImage = new Bitmap(markerFile); 
            markerImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
            if (markerLocation != null)
            {
                canvas.DrawWorldImageWithoutScaling(new GeoImage(memoryStream), markerLocation.X, markerLocation.Y, DrawingLevel.LevelOne);
            }
            canvas.Flush();
        }

    }
}
