using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Blazor.HowDoI
{
    internal class GeometricFunctionHelper
    {
        public static GeoCollection<Feature> LoadInputFeatures()
        {
            GeoCollection<Feature> features = new GeoCollection<Feature>();
            XElement xElement = XElement.Load(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "appdata", "inputFeatures.xml"));
            var featureXElements = xElement.Descendants("Feature").ToArray();
            foreach (var featureXElement in featureXElements)
            {
                if (!string.IsNullOrEmpty(featureXElement.Value) && featureXElement.Value.Trim().Length > 0)
                {
                    Feature feature = new Feature(featureXElement.Value);
                    foreach (var xAttribute in featureXElement.Attributes())
                    {
                        feature.ColumnValues[xAttribute.Name.LocalName] = xAttribute.Value;
                    }
                    features.Add(featureXElement.Attribute("id").Value, feature);
                }
            }
            return features;
        }

        public static InMemoryFeatureLayer InitOutputLayer()
        {
            InMemoryFeatureLayer outputLayer = new InMemoryFeatureLayer();
            outputLayer.Open();
            outputLayer.Columns.Add(new FeatureSourceColumn("Name", "character", 100));
            outputLayer.Columns.Add(new FeatureSourceColumn("Display", "character", 100));

            ValueStyle valueStyle = new ValueStyle();
            valueStyle.ColumnName = "Name";
            ValueItem defaultValueItem = new ValueItem();
            defaultValueItem.Value = "";
            defaultValueItem.CustomStyles.Add(new LineStyle(new GeoPen(GeoColor.FromArgb(180, 255, 155, 13), 5)));
            defaultValueItem.CustomStyles.Add(new PointStyle(PointSymbolType.Circle, 8, new GeoSolidBrush(GeoColor.FromArgb(255, 255, 248, 172)), GeoPens.Black));
            defaultValueItem.CustomStyles.Add(new AreaStyle(new GeoPen(GeoColors.Green, 3), new GeoSolidBrush(GeoColor.FromArgb(100, 0, 147, 221))));

            // Give different style for different geoprocessing.
            valueStyle.ValueItems.Add(defaultValueItem);
            valueStyle.ValueItems.Add(new ValueItem("GetLineOnLineResult", new LineStyle(new GeoPen(GeoColor.FromArgb(200, 146, 203, 252), 5f), new GeoPen(GeoColors.Black, 6f))));
            valueStyle.ValueItems.Add(new ValueItem("Buffering", new AreaStyle(new GeoSolidBrush(GeoColor.FromArgb(140, 255, 155, 13)))));
            valueStyle.ValueItems.Add(new ValueItem("ClippingResult", new AreaStyle(new GeoPen(GeoColors.Black, 1), new GeoSolidBrush(new GeoColor(160, 255, 248, 172)))));
            valueStyle.ValueItems.Add(new ValueItem("SnappingBuffer", new AreaStyle(new GeoSolidBrush(GeoColors.Transparent))));
            valueStyle.ValueItems.Add(new ValueItem("EnvelopeResult", new AreaStyle(new GeoPen(GeoColor.FromArgb(255, 255, 155, 13), 3), new GeoSolidBrush(new GeoColor(160, 255, 248, 172)))));
            valueStyle.ValueItems.Add(new ValueItem("SimplifiedPolygon", new AreaStyle(new GeoPen(GeoColor.FromArgb(255, 255, 155, 13), 2), new GeoSolidBrush(GeoColor.FromArgb(140, 255, 155, 13)))));
            valueStyle.ValueItems.Add(new ValueItem("Subcommunity1", new AreaStyle(new GeoPen(GeoColors.Gray, 3), new GeoSolidBrush(GeoColor.FromArgb(140, 255, 155, 13)))));
            valueStyle.ValueItems.Add(new ValueItem("Subcommunity2", new AreaStyle(new GeoPen(GeoColors.Gray, 3), new GeoSolidBrush(GeoColor.FromArgb(150, 255, 204, 1)))));
            valueStyle.ValueItems.Add(new ValueItem("Length", new TextStyle("Display", new GeoFont("Arial", 10), new GeoSolidBrush(GeoColors.Black)) { ForceHorizontalLabelForLine = true, TextLineSegmentRatio = 5, YOffsetInPixel = 230 }));

            outputLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(valueStyle);
            outputLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new TextStyle("Display", new GeoFont("Arial", 10), new GeoSolidBrush(GeoColors.Black)));

            outputLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            return outputLayer;
        }

        public static InMemoryFeatureLayer InitInputLayer()
        {
            InMemoryFeatureLayer inputLayer = new InMemoryFeatureLayer();
            inputLayer.Open();
            inputLayer.Columns.Add(new FeatureSourceColumn("Name", "character", 100));
            inputLayer.Columns.Add(new FeatureSourceColumn("Display", "character", 100));

            ValueStyle valueStyle = new ValueStyle();
            valueStyle.ColumnName = "Name";

            ValueItem defaultValueItem = new ValueItem();
            defaultValueItem.Value = "";
            defaultValueItem.CustomStyles.Add(new AreaStyle(new GeoPen(GeoColors.Black, 3), new GeoSolidBrush(GeoColor.FromArgb(100, 0, 147, 221))));
            defaultValueItem.CustomStyles.Add(new PointStyle(PointSymbolType.Circle, 12, new GeoSolidBrush(GeoColors.Transparent), GeoPens.Black));
            defaultValueItem.CustomStyles.Add(new LineStyle(new GeoPen(GeoColor.FromArgb(180, 255, 155, 13), 5)));

            valueStyle.ValueItems.Add(defaultValueItem);
            valueStyle.ValueItems.Add(new ValueItem("Community", new AreaStyle(new GeoPen(GeoColors.Black, 3), new GeoSolidBrush(GeoColor.FromArgb(100, 0, 147, 221)))));
            valueStyle.ValueItems.Add(new ValueItem("SnappingBuffer", new AreaStyle(GeoPens.Black, new GeoSolidBrush(GeoColors.Transparent))));
            valueStyle.ValueItems.Add(new ValueItem("ClippingSource", new AreaStyle(new GeoPen(GeoColors.Black, 1), new GeoSolidBrush(new GeoColor(160, 255, 248, 172)))));

            PointStyle firePointStyle = new PointStyle();
            firePointStyle.PointType = PointType.Image;
            firePointStyle.Image = new GeoImage(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "fire.png"));
            valueStyle.ValueItems.Add(new ValueItem("FirePoint", firePointStyle));

            ValueStyle valueStyle1 = new ValueStyle();
            valueStyle1.ColumnName = "Name";

            valueStyle1.ValueItems.Add(new ValueItem("Clipping", new AreaStyle(new GeoPen(GeoColor.FromArgb(255, 128, 128, 255), 2), new GeoSolidBrush(GeoColor.FromArgb(100, 146, 203, 252)))));

            inputLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(valueStyle);
            inputLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(valueStyle1);
            inputLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new TextStyle("Display", new GeoFont("Arial", 10), new GeoSolidBrush(GeoColors.Black)));

            inputLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            return inputLayer;
        }

        public static IEnumerable<GeometricFunctionModel> GetGeometricFunctions()
        {
            var menusFile = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "appdata", "geometricfunctions.json");
            return JsonConvert.DeserializeObject<List<GeometricFunctionModel>>(File.ReadAllText(menusFile));
        }

        public static IEnumerable<Feature> GetSnappingBufferFeatures(IEnumerable<Feature> features)
        {
            foreach (var feature in features)
            {
                MultilineShape multilineShape = feature.GetShape() as MultilineShape;
                if (multilineShape != null)
                {
                    foreach (var vertex in multilineShape.Lines.SelectMany(l => l.Vertices))
                    {
                        double distanceInMeter = Conversion.ConvertMeasureUnits(100, DistanceUnit.Feet, DistanceUnit.Meter);
                        EllipseShape ellipseShape = new EllipseShape(new PointShape(vertex), Math.Round(distanceInMeter, 2));
                        Feature tempFeature = new Feature(ellipseShape);
                        tempFeature.ColumnValues["Name"] = "SnappingBuffer";
                        yield return tempFeature;
                    }
                }
            }
        }

        public static MultipolygonShape Split(PolygonShape polygonShape, LineShape lineShape)
        {
            MultipolygonShape resultShape = new MultipolygonShape();
            MultipointShape intersectionMultiPoint = lineShape.GetCrossing(polygonShape.OuterRing);

            if (intersectionMultiPoint.Points.Count == 2)
            {
                PolygonShape polygonShape1 = GetPolygonForSplit(polygonShape, lineShape, false);
                PolygonShape polygonShape2 = GetPolygonForSplit(polygonShape, lineShape, true);
                resultShape.Polygons.Add(polygonShape1);
                resultShape.Polygons.Add(polygonShape2);
            }
            return resultShape;
        }

        private static PolygonShape GetPolygonForSplit(PolygonShape processedPolygon, LineShape processedLineShape, bool changeOrder)
        {
            MultipointShape intersectionMultiPoint = new MultipointShape();
            PolygonShape orderedPolygon = processedPolygon;

            LineShape lineShape = new LineShape((processedLineShape.GetIntersection(orderedPolygon.OuterRing)).Lines[0].GetWellKnownText());
            if (changeOrder)
            {
                List<Vertex> verticesToReverse = new List<Vertex>(lineShape.Vertices);
                verticesToReverse.Reverse();
                lineShape.Vertices.Clear();
                foreach (Vertex vertice in verticesToReverse)
                {
                    lineShape.Vertices.Add(vertice);
                }
            }

            foreach (Vertex vertex in lineShape.Vertices)
            {
                intersectionMultiPoint.Points.Add(new PointShape(vertex));
            }

            if (intersectionMultiPoint.Points.Count >= 2)
            {
                PolygonShape resultPolygonShape = new PolygonShape();
                RingShape resultOuterRing = SplitRing(orderedPolygon.OuterRing, intersectionMultiPoint.Points);
                resultPolygonShape.OuterRing = resultOuterRing;

                foreach (RingShape innerRing in orderedPolygon.InnerRings)
                {
                    MultipointShape innerIntersectionMultiPoint = lineShape.GetCrossing(innerRing);
                    if (innerIntersectionMultiPoint.Points.Count == 2)
                    {
                        RingShape resultInnerRing = SplitRing(innerRing, innerIntersectionMultiPoint.Points);
                        if (resultPolygonShape.Contains(resultInnerRing))
                        {
                            resultPolygonShape.InnerRings.Add(resultInnerRing);
                        }
                    }
                    else
                    {
                        if (resultPolygonShape.Contains(innerRing))
                        {
                            resultPolygonShape.InnerRings.Add(innerRing);
                        }
                    }
                }
                return resultPolygonShape;
            }
            return null;
        }

        private static RingShape SplitRing(RingShape processedRing, Collection<PointShape> intersectionPointShapes)
        {
            RingShape resultRingShape = new RingShape();
            PointShape intersectionPointShape1 = intersectionPointShapes[0];
            PointShape intersectionPointShape2 = intersectionPointShapes[intersectionPointShapes.Count - 1];

            int i = 0;
            int totalPointNumber = processedRing.Vertices.Count;
            while (i < totalPointNumber - 1)
            {
                int indexA = i + 1;
                if (DoesPointShapeBelongToLineSegment(intersectionPointShape1, new PointShape(processedRing.Vertices[i]), new PointShape(processedRing.Vertices[indexA])))
                {
                    resultRingShape.Vertices.Add(new Vertex(intersectionPointShape1));
                    if (DoesPointShapeBelongToLineSegment(intersectionPointShape2, new PointShape(processedRing.Vertices[i]), new PointShape(processedRing.Vertices[indexA])))
                    {
                        for (int shapeIndex = intersectionPointShapes.Count - 1; shapeIndex > 0; shapeIndex--)
                        {
                            resultRingShape.Vertices.Add(new Vertex(intersectionPointShapes[shapeIndex]));
                        }
                        resultRingShape.Vertices.Add(new Vertex(intersectionPointShape1));
                    }
                    else
                    {
                        for (int j = i + 1; j <= processedRing.Vertices.Count - 1; j++)
                        {
                            //- 1
                            int indexB = j + 1;

                            if (j < processedRing.Vertices.Count - 1)
                            {
                                if (DoesPointShapeBelongToLineSegment(intersectionPointShape2, new PointShape(processedRing.Vertices[j]), new PointShape(processedRing.Vertices[indexB])))
                                {
                                    resultRingShape.Vertices.Add(processedRing.Vertices[j]);

                                    for (int shapeIndex = intersectionPointShapes.Count - 1; shapeIndex > 0; shapeIndex--)
                                    {
                                        resultRingShape.Vertices.Add(new Vertex(intersectionPointShapes[shapeIndex]));
                                    }
                                    resultRingShape.Vertices.Add(new Vertex(intersectionPointShape1));
                                    break;
                                }
                                else
                                {
                                    resultRingShape.Vertices.Add(processedRing.Vertices[j]);
                                }
                            }
                            else
                            {
                                for (int k = 0; k < i; k++)
                                {
                                    if (DoesPointShapeBelongToLineSegment(intersectionPointShape2, new PointShape(processedRing.Vertices[k]), new PointShape(processedRing.Vertices[(k + 1)])))
                                    {
                                        resultRingShape.Vertices.Add(processedRing.Vertices[k]);
                                        for (int shapeIndex = intersectionPointShapes.Count - 1; shapeIndex > 0; shapeIndex--)
                                        {
                                            resultRingShape.Vertices.Add(new Vertex(intersectionPointShapes[shapeIndex]));
                                        }
                                        resultRingShape.Vertices.Add(new Vertex(intersectionPointShape1));
                                        break;
                                    }
                                    else
                                    {
                                        resultRingShape.Vertices.Add(processedRing.Vertices[k]);
                                    }
                                }
                                break;
                            }
                        }
                    }

                    return resultRingShape;
                }
                i = i + 1;
            }
            return null;
        }

        private static bool DoesPointShapeBelongToLineSegment(PointShape pointShape, PointShape linePointShape1, PointShape linePointShape2)
        {
            bool result = false;
            if ((pointShape.X == linePointShape1.X & pointShape.Y == linePointShape1.Y) | (pointShape.X == linePointShape2.X & pointShape.Y == linePointShape2.Y))
            {
                result = true;
            }
            else
            {
                if (linePointShape1.X != linePointShape2.X)
                {
                    double a = (linePointShape2.Y - linePointShape1.Y) / (linePointShape2.X - linePointShape1.X);
                    double b = linePointShape1.Y - (a * linePointShape1.X);

                    if (Math.Round(pointShape.Y, 5) == Math.Round((a * pointShape.X) + b, 5) & pointShape.X >= Math.Min(linePointShape1.X, linePointShape2.X) & pointShape.X <= Math.Max(linePointShape1.X, linePointShape2.X))
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                else
                {
                    if (pointShape.X == linePointShape1.X & (pointShape.Y >= Math.Min(linePointShape1.Y, linePointShape2.Y) & pointShape.Y <= Math.Max(linePointShape1.Y, linePointShape2.Y)))
                    {
                        result = true;
                    }
                    else
                    { result = false; }
                }
            }
            return result;
        }

    }
}
