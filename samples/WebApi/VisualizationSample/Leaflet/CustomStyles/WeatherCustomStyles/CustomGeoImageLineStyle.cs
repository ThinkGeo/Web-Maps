using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebApi;

namespace Visualization
{
    public class CustomGeoImageLineStyle : LineStyle
    {
        private LineStyle lineStyle;
        private int imageSpacing;
        private Collection<GeoImage> images;
        private ImageDirection imageDirection;

        private CustomGeoImageLineStyle()
        { }

        public CustomGeoImageLineStyle(LineStyle lineStyle, GeoImage geoImage, int imageSpacing, ImageDirection imageDirection)
            : this(lineStyle, new Collection<GeoImage> { geoImage }, imageSpacing, imageDirection)
        { }

        public CustomGeoImageLineStyle(LineStyle lineStyle, IEnumerable<GeoImage> geoImages, int imageSpacing, ImageDirection imageDirection)
        {
            this.imageDirection = imageDirection;
            this.imageSpacing = imageSpacing;
            this.lineStyle = lineStyle;
            this.images = new Collection<GeoImage>();
            foreach (var geoImage in geoImages)
            {
                this.images.Add(geoImage);
            }
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
        /// Gets a collection of images for temperature display.
        /// </summary>
        public Collection<GeoImage> Images
        {
            get { return images; }
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
            PointStyle[] pointStyles = images.Select(geoImage =>
            {
                return new PointStyle(geoImage) { DrawingLevel = DrawingLevel.LevelThree };
            }).ToArray();

            //  Loop the features for display.
            foreach (Feature feature in features)
            {
                LineShape lineShape = (LineShape)feature.GetShape();
                lineStyle.Draw(new BaseShape[] { lineShape }, canvas, labelsInThisLayer, labelsInAllLayers);

                int index = 0;
                double totalDistance = 0;
                for (int i = 0; i < lineShape.Vertices.Count - 1; i++)
                {
                    PointShape pointShape1 = new PointShape(lineShape.Vertices[i]);
                    PointShape pointShape2 = new PointShape(lineShape.Vertices[i + 1]);

                    LineShape tempLineShape = new LineShape();
                    tempLineShape.Vertices.Add(lineShape.Vertices[i]);
                    tempLineShape.Vertices.Add(lineShape.Vertices[i + 1]);

                    double angle = WeatherLineStyle.GetAngleFromTwoVertices(lineShape.Vertices[i], lineShape.Vertices[i + 1]);
                    // Left side
                    if (imageDirection == ImageDirection.Left)
                    {
                        if (angle >= 270)
                        {
                            angle = angle - 180;
                        }
                    }
                    // Right side
                    else
                    {
                        if (angle <= 90)
                        {
                            angle = angle + 180;
                        }
                    }

                    foreach (var pointStyle in pointStyles)
                    {
                        pointStyle.RotationAngle = (float)angle;
                    }

                    float screenDistance = ExtentHelper.GetScreenDistanceBetweenTwoWorldPoints(canvas.CurrentWorldExtent, pointShape1, pointShape2, canvas.Width, canvas.Height);
                    double currentDistance = Math.Round(pointShape1.GetDistanceTo(pointShape2, canvas.MapUnit, DistanceUnit.Meter), 2);
                    double worldInterval = (currentDistance * imageSpacing) / screenDistance;
                    while (totalDistance <= currentDistance)
                    {
                        PointStyle pointStyle = pointStyles[index % pointStyles.Length];
                        PointShape tempPointShape = tempLineShape.GetPointOnALine(StartingPoint.FirstPoint, totalDistance, canvas.MapUnit, DistanceUnit.Meter);
                        pointStyle.Draw(new BaseShape[] { tempPointShape }, canvas, labelsInThisLayer, labelsInAllLayers);
                        totalDistance = totalDistance + worldInterval;
                        index++;
                    }

                    totalDistance = totalDistance - currentDistance;
                }
            }
        }
    }
}