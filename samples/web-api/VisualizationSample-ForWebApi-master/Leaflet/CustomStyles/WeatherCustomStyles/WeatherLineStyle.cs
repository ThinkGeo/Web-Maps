using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace Visualization
{
    public class WeatherLineStyle : LineStyle
    {
        private GeoImage geoImage;
        private int imageSpacing;
        private LineStyle lineStyle;
        private ImageDirection imageDirection;

        private WeatherLineStyle()
        { }

        public WeatherLineStyle(LineStyle lineStyle, GeoImage GeoImage, int imageSpacing, ImageDirection imageDirection)
        {
            this.lineStyle = lineStyle;
            this.geoImage = GeoImage;
            this.imageSpacing = imageSpacing;
            this.imageDirection = imageDirection;
        }

        /// <summary>
        /// Gets or sets the line style of the weather front end.
        /// </summary>
        public LineStyle LineStyle
        {
            get { return lineStyle; }
            set { lineStyle = value; }
        }

        /// <summary>
        /// Gets the image for temperature display.
        /// </summary>
        public GeoImage GeoImage
        {
            get { return geoImage; }
            set { geoImage = value; }
        }

        /// <summary>
        /// Gets or sets the image spacing in screen coordinate between each GeoImage on the line.
        /// </summary>
        public int ImageSpacing
        {
            get { return imageSpacing; }
            set { imageSpacing = value; }
        }

        /// <summary>
        /// Gets or sets the direction where the image display according to the weather front end line.
        /// </summary>
        public ImageDirection Direction
        {
            get { return imageDirection; }
            set { imageDirection = value; }
        }

        protected override void DrawCore(IEnumerable<Feature> features, GeoCanvas canvas, Collection<SimpleCandidate> labelsInThisLayer, Collection<SimpleCandidate> labelsInAllLayers)
        {
            PointStyle pointStyle = new PointStyle(geoImage);

            foreach (Feature feature in features)
            {
                MultilineShape lineShape = (MultilineShape)feature.GetShape();
                lineStyle.Draw(new BaseShape[] { lineShape }, canvas, labelsInThisLayer, labelsInAllLayers);

                List<Vertex> allVertices = lineShape.Lines.SelectMany(l => l.Vertices).ToList();

                double totalDistance = 0;
                for (int i = 0; i < allVertices.Count - 1; i++)
                {
                    PointShape pointShape1 = new PointShape(allVertices[i]);
                    PointShape pointShape2 = new PointShape(allVertices[i + 1]);

                    LineShape tempLineShape = new LineShape();
                    tempLineShape.Vertices.Add(allVertices[i]);
                    tempLineShape.Vertices.Add(allVertices[i + 1]);

                    double angle = GetAngleFromTwoVertices(allVertices[i], allVertices[i + 1]);
                    // Left side
                    if (imageDirection == ImageDirection.Left)
                    {
                        if (angle >= 270)
                        {
                            angle -= 180;
                        }
                    }
                    // Right side
                    else
                    {
                        if (angle <= 90)
                        {
                            angle += 180;
                        }
                    }
                    pointStyle.RotationAngle = (float)angle;

                    float screenDistance = ExtentHelper.GetScreenDistanceBetweenTwoWorldPoints(canvas.CurrentWorldExtent, pointShape1, pointShape2, canvas.Width, canvas.Height);
                    double currentDistance = Math.Round(pointShape1.GetDistanceTo(pointShape2, canvas.MapUnit, DistanceUnit.Meter), 2);
                    double worldInterval = (currentDistance * imageSpacing) / screenDistance;
                    while (totalDistance <= currentDistance)
                    {
                        PointShape tempPointShape = tempLineShape.GetPointOnALine(StartingPoint.FirstPoint, totalDistance, canvas.MapUnit, DistanceUnit.Meter);
                        pointStyle.Draw(new BaseShape[] { tempPointShape }, canvas, labelsInThisLayer, labelsInAllLayers);
                        totalDistance = totalDistance + worldInterval;
                    }

                    totalDistance = totalDistance - currentDistance;
                }
            }
        }

        public static double GetAngleFromTwoVertices(Vertex firstVertex, Vertex secondVertex)
        {
            double tangentAlpha = (secondVertex.Y - firstVertex.Y) / (secondVertex.X - firstVertex.X);
            double Peta = Math.Atan(tangentAlpha);

            double alpha = 0;
            if (secondVertex.X > firstVertex.X)
            {
                alpha = 90 + (Peta * (180 / Math.PI));
            }
            else if (secondVertex.X < firstVertex.X)
            {
                alpha = 270 + (Peta * (180 / Math.PI));
            }
            else
            {
                if (secondVertex.Y > firstVertex.Y) alpha = 0;
                if (secondVertex.Y < firstVertex.Y) alpha = 180;
            }

            double offset = -90;
            if (firstVertex.X > secondVertex.X)
            {
                offset = 90;
            }

            return alpha + offset;
        }
    }
}
