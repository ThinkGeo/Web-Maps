using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace ThinkGeo.MapSuite.Core
{
    [Serializable]
    public class ClassBreakClusterPointStyle : Style
    {
        [Obfuscation(Exclude = true)]
        private int cellSize;
        [Obfuscation(Exclude = true)]
        private TextStyle textSytle;
        [Obfuscation(Exclude = true)]
        private Dictionary<int, PointStyle> classBreakPoints;

        public ClassBreakClusterPointStyle()
            : base()
        {
            cellSize = 100;
            textSytle = new TextStyle();
            classBreakPoints = new Dictionary<int, PointStyle>();
        }

        public Dictionary<int, PointStyle> ClassBreakPoints
        {
            get { return classBreakPoints; }
        }

        /// <summary>
        /// Gets or sets a TextStyle for display the label on the cluster.
        /// </summary>
        public TextStyle TextStyle
        {
            get { return textSytle; }
            set { textSytle = value; }
        }

        /// <summary>
        /// Gets or sets a value in pixel which determines how devide the screen. The smaller the slower it runs.
        /// </summary>
        public int CellSize
        {
            get { return cellSize; }
            set { cellSize = value; }
        }

        /// <summary>
        /// Here in the DrawCore we cluster the features
        /// </summary>
        protected override void DrawCore(IEnumerable<Feature> features, GeoCanvas canvas, Collection<SimpleCandidate> labelsInThisLayer, Collection<SimpleCandidate> labelsInAllLayers)
        {
            //  We get the scale to determine the grid.  This scale property should really be on the Canvas!
            double scale = ExtentHelper.GetScale(canvas.CurrentWorldExtent, canvas.Width, canvas.MapUnit);

            // Setup our grid for clustering the points.  This is where we specify our cell size in pixels
            MapSuiteTileMatrix mapSuiteTileMatrix = new MapSuiteTileMatrix(scale, cellSize, cellSize, canvas.MapUnit);

            // Pass in the current extent to get our grid cells.  All points in these cells will be consolidated
            IEnumerable<TileMatrixCell> tileMatricCells = mapSuiteTileMatrix.GetContainedCells(canvas.CurrentWorldExtent);

            // Create an unused features list, as we add them to clusters we will remove them from here
            // This is just for speed so we don't re-test lots of already associated features
            Dictionary<string, string> unusedFeatures = new Dictionary<string, string>();

            foreach (Feature feature in features)
            {
                if (feature.GetWellKnownType() != WellKnownType.Point && feature.GetWellKnownType() != WellKnownType.Multipoint)
                {
                    continue;
                }
                unusedFeatures.Add(feature.Id, feature.Id);
            }

            // Loop through each cell and find the features that fit inside of it
            foreach (TileMatrixCell cell in tileMatricCells)
            {
                int featureCount = 0;
                MultipointShape tempMultiPointShape = new MultipointShape();
                foreach (Feature feature in features)
                {
                    // Make sure the feature has not been used in another cluster                   
                    if (unusedFeatures.ContainsKey(feature.Id))
                    {
                        // Check if the cell contains the feature
                        if (cell.BoundingBox.Contains(feature.GetBoundingBox()))
                        {
                            featureCount++;
                            unusedFeatures.Remove(feature.Id);
                            if (feature.GetWellKnownType() == WellKnownType.Multipoint)
                            {
                                MultipointShape multipointShape = feature.GetShape() as MultipointShape;
                                foreach (var item in multipointShape.Points)
                                {
                                    tempMultiPointShape.Points.Add(item);
                                }
                            }
                            else
                            {
                                tempMultiPointShape.Points.Add(feature.GetShape() as PointShape);
                            }
                        }
                    }
                }
                if (featureCount > 0)
                {
                    // Add the feature count to the new feature we created.  The feature will be placed
                    // at the center of gravity of all the clustered features of the cell we created.
                    Dictionary<string, string> featureValues = new Dictionary<string, string>();
                    featureValues.Add("FeatureCount", featureCount.ToString(CultureInfo.InvariantCulture));

                    bool isMatch = false;

                    for (int i = 0; i < classBreakPoints.Count - 1; i++)
                    {
                        var startItem = classBreakPoints.ElementAt(i);
                        var endItem = classBreakPoints.ElementAt(i + 1);
                        if (featureCount >= startItem.Key && featureCount < endItem.Key)
                        {
                            // Draw the point shape
                            startItem.Value.Draw(new Feature[] { new Feature(tempMultiPointShape.GetCenterPoint(), featureValues) }, canvas, labelsInThisLayer, labelsInAllLayers);
                            isMatch = true;
                            break;
                        }
                    }
                    if (!isMatch && featureCount >= classBreakPoints.LastOrDefault().Key)
                    {
                        classBreakPoints.LastOrDefault().Value.Draw(new Feature[] { new Feature(tempMultiPointShape.GetCenterPoint(), featureValues) }, canvas, labelsInThisLayer, labelsInAllLayers);
                    }

                    if (featureCount != 1)
                    {
                        // Draw the text style to show how many feaures are consolidated in the cluster
                        textSytle.Draw(new Feature[] { new Feature(tempMultiPointShape.GetCenterPoint(), featureValues) }, canvas, labelsInThisLayer, labelsInAllLayers);
                    }
                }
            }
        }
    }
}
