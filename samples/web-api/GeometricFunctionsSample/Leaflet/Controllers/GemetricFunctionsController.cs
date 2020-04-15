using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Xml.Linq;
using ThinkGeo.Core;
using ThinkGeo.UI.WebApi;

namespace GeometricFunctions.Controllers
{
    [Route("tile")]
    public class GeometricFunctionsController : ControllerBase
    {
        // Load the input features which will be used for geo-processing.
        private static readonly string baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        private static GeoCollection<Feature> inputFeatures = null;

        static GeometricFunctionsController()
        {
            inputFeatures = LoadInputFeatures();
        }

        [Route("Input/{z}/{x}/{y}/{featureIds}")]
        public IActionResult GetInputTile(int z, int x, int y, string featureIds)
        {
            // Get the sample input features by ids passed from client side.
            Feature[] tempFeatures = featureIds.Split(',').Select(id => inputFeatures[id]).ToArray();

            // Draw the map and return the image back to client in an IActionResult.
            return DrawTile(z, x, y, tempFeatures, new Feature[] { });
        }

        [Route("Output/{z}/{x}/{y}/{accessId}")]
        public IActionResult GetOutputTile(int z, int x, int y, string accessId)
        {
            // Get the result of the geoprocessing made by the speicific access id.
            Collection<Feature> outputFeatures = GetGeoprocessingResultFeatures(accessId);

            // Draw the map and return the image back to client in an IActionResult.
            return DrawTile(z, x, y, new Feature[] { }, outputFeatures);
        }

        [Route("Execute/Union/{accessId}")]
        [HttpPost]
        public void Union(string accessId, [FromBody]string[] ids)
        {
            // Get the sample input features by ids passed from client side.
            Feature tempFeatures = Feature.Union(ids.Select(id => inputFeatures[id]));
            SaveFeatures(accessId, new Feature[] { tempFeatures });
        }

        [Route("Execute/Difference/{accessId}")]
        [HttpPost]
        public void Difference(string accessId, [FromBody]Dictionary<string, string> parameters)
        {
            // Get the feature as an element for doing geoprocessing.
            Feature sourceFeature = inputFeatures[parameters["source"]];
            // Get the feature as another element for doing geoprocessing.
            Feature targetFeature = inputFeatures[parameters["target"]];

            // Do Diference geoprocessing.
            Feature resultFeature = sourceFeature.GetDifference(targetFeature);

            SaveFeatures(accessId, new Feature[] { resultFeature });
        }

        [Route("Execute/Buffer/{accessId}")]
        [HttpPost]
        public void Buffer(string accessId, [FromBody]Dictionary<string, string> parameters)
        {
            // Get the buffer distance passed from client side.
            double distance = Convert.ToDouble(parameters["distance"], CultureInfo.InvariantCulture);
            // Get the buffer distance unit passed from client side.
            DistanceUnit distanceUnit = (DistanceUnit)Enum.Parse(typeof(DistanceUnit), parameters["unit"], true);

            // Get the feature for buffer.
            Feature inputFeature = inputFeatures[parameters["id"]];

            // Do buffer.
            Feature bufferedFeature = inputFeature.Buffer(distance, GeographyUnit.Meter, distanceUnit);
            bufferedFeature.ColumnValues["Name"] = "Buffering";

            SaveFeatures(accessId, new Feature[] { inputFeature, bufferedFeature });
        }

        [Route("Execute/Scale/{accessId}")]
        [HttpPost]
        public void Scale(string accessId, [FromBody]Dictionary<string, string> parameters)
        {
            double percentage = Convert.ToDouble(parameters["percentage"], CultureInfo.InvariantCulture);
            // Get the feature for Scaling.
            Feature inputFeature = inputFeatures[parameters["id"]];

            MultipolygonShape polygonShape = inputFeature.GetShape() as MultipolygonShape;
            // Scale up the input feature.
            polygonShape.ScaleUp(percentage);
            Feature scaledFeature = new Feature(polygonShape, inputFeature.ColumnValues);

            SaveFeatures(accessId, new Feature[] { inputFeature, scaledFeature });
        }

        [Route("Execute/Rotate/{accessId}")]
        [HttpPost]
        public void Rotate(string accessId, [FromBody]Dictionary<string, string> parameters)
        {
            // Get the rotation angle passed from client side.
            double degreeAngle = Convert.ToDouble(parameters["angle"], CultureInfo.InvariantCulture);
            // Get the feature for rotate.
            Feature inputFeature = inputFeatures[parameters["id"]];

            BaseShape shape = inputFeature.GetShape();

            // Rotate the input feature.
            Feature rotatedFeature = new Feature(BaseShape.Rotate(shape, shape.GetCenterPoint(), (float)degreeAngle));

            SaveFeatures(accessId, new Feature[] { inputFeature, rotatedFeature });
        }

        [Route("Execute/CenterPoint/{accessId}")]
        [HttpPost]
        public void CenterPoint(string accessId, [FromBody]string id)
        {
            // Get the feature for calculating the center point.
            Feature inputFeature = inputFeatures[id];

            // Get the center point of boundingBox.
            PointShape centerPointShape = inputFeature.GetBoundingBox().GetCenterPoint();
            Feature centerFeature = new Feature(centerPointShape);
            centerFeature.ColumnValues["Display"] = "BoundingBox Center Point";

            // Get the centroid point of the geometry.
            PointShape centroidPointShape = inputFeature.GetShape().GetCenterPoint();
            Feature centroidFeature = new Feature(centroidPointShape);
            centroidFeature.ColumnValues["Display"] = "Polygon Center Point";

            SaveFeatures(accessId, new Feature[] { inputFeature, centerFeature, centroidFeature });
        }

        [Route("Execute/CalculateArea/{accessId}")]
        [HttpPost]
        public void CalculateArea(string accessId, [FromBody]string id)
        {
            // Calculate the area of the geometry.
            AreaBaseShape areaBaseShape = inputFeatures[id].GetShape() as AreaBaseShape;
            double areaInHectares = areaBaseShape.GetArea(GeographyUnit.Meter, AreaUnit.Hectares);
            double areaInAcres = areaBaseShape.GetArea(GeographyUnit.Meter, AreaUnit.Acres);

            // Display the geometry labeling with area.
            string acreaText = string.Format("{0:N3} {1}\r\n{2:N3} {3}", areaInHectares, "Hectares", areaInAcres, "Acres");
            Feature resultFeature = new Feature(areaBaseShape, new Dictionary<string, string> { { "Display", acreaText } });

            SaveFeatures(accessId, new Feature[] { resultFeature });
        }

        [Route("Execute/Simplify/{accessId}")]
        [HttpPost]
        public void Simplify(string accessId, [FromBody]Dictionary<string, string> parameters)
        {
            // Get the feature id used for geoprocessing.
            string featureId = parameters["id"];
            // Get the distance passed from client side.
            double distance = Convert.ToDouble(parameters["distance"], CultureInfo.InvariantCulture);
            // Get the buffer distance unit passed from client side.
            DistanceUnit distanceUnit = (DistanceUnit)Enum.Parse(typeof(DistanceUnit), parameters["unit"], true);

            // Simplify the polygon shape.
            MultipolygonShape polygonShape = inputFeatures[featureId].GetShape() as MultipolygonShape;
            polygonShape = polygonShape.Simplify(GeographyUnit.Meter, distance, distanceUnit, SimplificationType.DouglasPeucker);

            Feature simplifiedFeature = new Feature(polygonShape);
            simplifiedFeature.ColumnValues["Display"] = "SimplifiedPolygon";
            simplifiedFeature.ColumnValues["Name"] = "SimplifiedPolygon";

            SaveFeatures(accessId, new Feature[] { simplifiedFeature });
        }

        [Route("Execute/Split/{accessId}")]
        [HttpPost]
        public void Split(string accessId, [FromBody]Dictionary<string, string> parameters)
        {
            // Get the id of polygon shape used for split.
            string polygonId = parameters["polygonId"];
            // Get the id of the line used for splitting the polygon shape.
            string lineId = parameters["lineId"];

            // Get the polygon shape used for split.
            PolygonShape polygonShape = inputFeatures[polygonId].GetShape() as PolygonShape;
            // Get the line for splitting the polygon shape.
            LineShape lineShape = inputFeatures[lineId].GetShape() as LineShape;

            Collection<Feature> splitedFeatures = new Collection<Feature>();
            MultipolygonShape resultPolygon = Split(polygonShape, lineShape);
            for (int i = 0; i < resultPolygon.Polygons.Count; i++)
            {
                Feature feature = new Feature(resultPolygon.Polygons[i]);
                feature.ColumnValues["Display"] = "Subcommunity" + (i + 1).ToString(CultureInfo.InvariantCulture);
                feature.ColumnValues["Name"] = feature.ColumnValues["Display"];
                splitedFeatures.Add(feature);
            }

            SaveFeatures(accessId, splitedFeatures);
        }

        [Route("Execute/CalculateShortestLine/{accessId}")]
        [HttpPost]
        public void CalculateShortestLine(string accessId, [FromBody]Dictionary<string, string> parameters)
        {
            // Get the 2 shapes passed from client side for calculating the shortest distance.
            BaseShape shape1 = inputFeatures[parameters["id1"]].GetShape();
            BaseShape shape2 = inputFeatures[parameters["id2"]].GetShape();

            // Calculate the shortest distance.
            MultilineShape resultShape = shape1.GetShortestLineTo(shape2, GeographyUnit.Meter);
            double distance = shape1.GetDistanceTo(shape2, GeographyUnit.Meter, DistanceUnit.Feet);

            Feature shortestLine = new Feature(resultShape);
            shortestLine.ColumnValues["Display"] = string.Format("Distance is {0:N2} feet.", distance);

            SaveFeatures(accessId, new Feature[] { inputFeatures[parameters["id1"]], inputFeatures[parameters["id2"]], shortestLine });
        }

        [Route("Execute/CalculateLength/{accessId}")]
        [HttpPost]
        public void CalculateLength(string accessId, [FromBody]string id)
        {
            // Get the line shape used for calculating the length.
            LineShape lineShape = inputFeatures[id].GetShape() as LineShape;
            // Calculate the length of the line shape.
            double length = lineShape.GetLength(GeographyUnit.Meter, DistanceUnit.Feet);

            Feature lengthFeature = new Feature(lineShape);
            lengthFeature.ColumnValues["Display"] = string.Format("Length is {0:N0} feet.", length);
            lengthFeature.ColumnValues["Name"] = "Length";

            SaveFeatures(accessId, new Feature[] { inputFeatures[id], lengthFeature });
        }

        [Route("Execute/LineOnLine/{accessId}")]
        [HttpPost]
        public void LineOnLine(string accessId, [FromBody]Dictionary<string, string> parameters)
        {
            // Get the feature id passed from client side.
            string id = parameters["id"];
            // Get the first distance from the start vertex of the line.
            double startingDistance = Convert.ToDouble(parameters["startDistance"], CultureInfo.InvariantCulture);
            // Get the second distance from the start vertex of the line.
            double distance = Convert.ToDouble(parameters["distance"], CultureInfo.InvariantCulture);
            // Get the distance unit passed from client side.
            DistanceUnit distanceUnit = (DistanceUnit)Enum.Parse(typeof(DistanceUnit), parameters["unit"], true);

            LineShape lineShape = (LineShape)inputFeatures[id].GetShape();
            // Clip a line on the line between start and end distance.
            LineBaseShape resultLineShape = lineShape.GetLineOnALine(StartingPoint.FirstPoint, startingDistance, distance, GeographyUnit.Meter, distanceUnit);

            Feature resultFeature = new Feature(resultLineShape);
            resultFeature.ColumnValues["Name"] = "GetLineOnLineResult";

            SaveFeatures(accessId, new Feature[] { inputFeatures[id], resultFeature });
        }

        [Route("Execute/Clip/{accessId}")]
        [HttpPost]
        public void Clip(string accessId, [FromBody]Dictionary<string, string> parameters)
        {
            Collection<Feature> resultFeatures = new Collection<Feature>();
            // Get the feature for clipping other features.
            Feature clippingFeature = inputFeatures[parameters["id"]];
            // Get the features which will be clipped by a specified polygon.
            string clippingSourceIds = parameters["clippingSourceIds"];
            Feature[] clippingSourceFeatures = clippingSourceIds.Split(',').Select(tempId => inputFeatures[tempId]).ToArray();
            foreach (var feature in clippingSourceFeatures)
            {
                // Clip the feature.
                Feature processedFeature = feature.GetIntersection(clippingFeature);
                if (processedFeature != null)
                {
                    processedFeature.ColumnValues["Name"] = "ClippingResult";
                    resultFeatures.Add(processedFeature);
                }
            }

            SaveFeatures(accessId, resultFeatures);
        }

        [Route("Execute/ConvexHull/{accessId}")]
        [HttpPost]
        public void ConvexHull(string accessId, [FromBody]Dictionary<string, string> parameters)
        {
            // Create a MultiPointShape for calculating the convex hull of it.
            MultipointShape multipointShape = new MultipointShape();
            string pointIds = parameters["pointIds"];
            foreach (var feature in pointIds.Split(',').Select(id => inputFeatures[id]))
            {
                multipointShape.Points.Add((PointShape)feature.GetShape());
            }
            // Calculate the context hull of points.
            PolygonShape pointsContextHull = new PolygonShape(multipointShape.ConvexHull());

            // Create a polygon shape for calculating the convex hull of it.
            string polygonId = parameters["polygonId"];
            MultipolygonShape multipolygonShape = inputFeatures[polygonId].GetShape() as MultipolygonShape;

            // Calculate the context hull of the polygon shape.
            PolygonShape polygonContextHull = new PolygonShape(multipolygonShape.GetConvexHull());

            SaveFeatures(accessId, new Feature[] { new Feature(pointsContextHull), new Feature(polygonContextHull) });
        }

        [Route("Execute/Snapping/{accessId}")]
        [HttpPost]
        public void Snapping(string accessId, [FromBody]Dictionary<string, string> parameters)
        {
            Collection<Feature> resultFeatures = new Collection<Feature>();
            // Get a MultiLineShape for snapping.
            MultilineShape multilineShape = inputFeatures[parameters["sourceId"]].GetShape() as MultilineShape;
            // Display the output features.
            resultFeatures.Add(new Feature(multilineShape));

            Feature[] allFeatures = new Feature[] { inputFeatures[parameters["targetId"]] };
            Collection<Feature> matchFeatures = new Collection<Feature>();
            // Get the fetaures which are in a specific distance from the MultiLineShape.
            foreach (var item in allFeatures)
            {
                double tempDistance = multilineShape.GetShortestLineTo(item, GeographyUnit.Meter).GetLength(GeographyUnit.Meter, DistanceUnit.Feet);
                if (tempDistance < 100)
                {
                    matchFeatures.Add(item);
                }
            }
            foreach (var feature in matchFeatures)
            {
                Collection<Vertex> vertices = new Collection<Vertex>();

                PointShape resultShape = multilineShape.GetClosestPointTo(feature, GeographyUnit.Meter);
                MultilineShape tempMultilineShape = feature.GetShape() as MultilineShape;
                if (tempMultilineShape != null)
                {
                    double offsetX = resultShape.X - tempMultilineShape.Lines[0].Vertices[0].X;
                    double offsetY = resultShape.Y - tempMultilineShape.Lines[0].Vertices[0].Y;
                    vertices.Add(new Vertex(resultShape));

                    double x = offsetX + tempMultilineShape.Lines[0].Vertices[1].X;
                    double y = offsetY + tempMultilineShape.Lines[0].Vertices[1].Y;
                    vertices.Add(new Vertex(x, y));
                }

                resultFeatures.Add(new Feature(new MultilineShape(new LineShape[] { new LineShape(vertices) })));
            }

            foreach (var feature in allFeatures)
            {
                if (!matchFeatures.Contains(feature))
                {
                    resultFeatures.Add(feature);
                }
            }

            AddPolygonsToFeatureLayer(resultFeatures.Skip(1).ToArray(), resultFeatures);

            SaveFeatures(accessId, resultFeatures);
        }

        [Route("Execute/EnvelopBoundingbox/{accessId}")]
        [HttpPost]
        public void CalculateEnvelope(string accessId, [FromBody]string id)
        {
            Feature envelope = new Feature(inputFeatures[id].GetBoundingBox());
            envelope.ColumnValues["Name"] = "EnvelopeResult";

            SaveFeatures(accessId, new Feature[] { envelope });
        }

        private IActionResult DrawTile(int z, int x, int y, IEnumerable<Feature> inputFeature, IEnumerable<Feature> outputFeature)
        {
            LayerOverlay layerOverlay = new LayerOverlay();

            InMemoryFeatureLayer inputFeatureLayer = InitInputLayer();
            InMemoryFeatureLayer outputFeatureLayer = InitOutputLayer();
            foreach (var item in inputFeature)
            {
                inputFeatureLayer.InternalFeatures.Add(item);
            }
            foreach (var item in outputFeature)
            {
                outputFeatureLayer.InternalFeatures.Add(item);
            }

            layerOverlay.Layers.Add(inputFeatureLayer);
            layerOverlay.Layers.Add(outputFeatureLayer);

            // Draw the map and return the image back to client in an IActionResult.
            using (GeoImage image = new GeoImage(256, 256))
            {
                GeoCanvas geoCanvas = GeoCanvas.CreateDefaultGeoCanvas();
                RectangleShape boundingBox = WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, GeographyUnit.Meter);
                geoCanvas.BeginDrawing(image, boundingBox, GeographyUnit.Meter);
                layerOverlay.Draw(geoCanvas);
                geoCanvas.EndDrawing();

                byte[] imageBytes = image.GetImageBytes(GeoImageFormat.Png);

                return File(imageBytes, "image/png");
            }
        }

        private static InMemoryFeatureLayer InitOutputLayer()
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
            defaultValueItem.CustomStyles.Add(new PointStyle(PointSymbolType.Circle, 8, new GeoSolidBrush(GeoColor.FromArgb(255, 255, 248, 172))));
            defaultValueItem.CustomStyles.Add(new AreaStyle(new GeoPen(GeoColors.Black, 3), new GeoSolidBrush(GeoColor.FromArgb(100, 0, 147, 221))));

            // Give different style for different geoprocessing.
            valueStyle.ValueItems.Add(defaultValueItem);
            valueStyle.ValueItems.Add(new ValueItem("GetLineOnLineResult", LineStyle.CreateSimpleLineStyle(GeoColor.FromArgb(200, 146, 203, 252), 5f, GeoColors.Black, 6f, true)));
            valueStyle.ValueItems.Add(new ValueItem("Buffering", new AreaStyle(new GeoSolidBrush(GeoColor.FromArgb(140, 255, 155, 13)))));
            valueStyle.ValueItems.Add(new ValueItem("ClippingResult", new AreaStyle(new GeoPen(GeoColors.Black, 1), new GeoSolidBrush(new GeoColor(160, 255, 248, 172)))));
            valueStyle.ValueItems.Add(new ValueItem("SnappingBuffer", AreaStyle.CreateSimpleAreaStyle(GeoColors.Transparent, GeoColors.Black)));
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

        private static InMemoryFeatureLayer InitInputLayer()
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
            defaultValueItem.CustomStyles.Add(PointStyle.CreateSimplePointStyle(PointSymbolType.Circle, GeoColors.Transparent, GeoColors.Black, 12));
            defaultValueItem.CustomStyles.Add(new LineStyle(new GeoPen(GeoColor.FromArgb(180, 255, 155, 13), 5)));

            valueStyle.ValueItems.Add(defaultValueItem);
            valueStyle.ValueItems.Add(new ValueItem("Community", new AreaStyle(new GeoPen(GeoColors.Black, 3), new GeoSolidBrush(GeoColor.FromArgb(100, 0, 147, 221)))));
            valueStyle.ValueItems.Add(new ValueItem("SnappingBuffer", AreaStyle.CreateSimpleAreaStyle(GeoColors.Transparent, GeoColors.Black)));
            valueStyle.ValueItems.Add(new ValueItem("ClippingSource", new AreaStyle(new GeoPen(GeoColors.Black, 1), new GeoSolidBrush(new GeoColor(160, 255, 248, 172)))));

            PointStyle firePointStyle = new PointStyle();
            firePointStyle.PointType = PointType.Image;
            firePointStyle.Image = new GeoImage(Path.Combine(baseDirectory, "Images", "fire.png"));
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

        private static GeoCollection<Feature> LoadInputFeatures()
        {
            GeoCollection<Feature> features = new GeoCollection<Feature>();
            XElement xElement = XElement.Load(Path.Combine(baseDirectory, "App_Data", "inputFeatures.xml"));
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

        private void AddPolygonsToFeatureLayer(IEnumerable<Feature> features, Collection<Feature> layerFeatures)
        {
            lock (features)
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
                            layerFeatures.Add(tempFeature);
                        }
                    }
                }
            }
        }

        private static MultipolygonShape Split(PolygonShape polygonShape, LineShape lineShape)
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

        private static void SaveFeatures(string accessId, IEnumerable<Feature> features)
        {
            string jsonFilePath = Path.Combine(baseDirectory, "App_Data", "Temp", string.Format("{0}.json", accessId));

            using (StreamWriter sw = new StreamWriter(jsonFilePath, false))
            {
                foreach (Feature feature in features)
                {
                    sw.WriteLine(feature.GetGeoJson());
                }
            }
        }

        private static Collection<Feature> GetGeoprocessingResultFeatures(string accessId)
        {
            string jsonFilePath = Path.Combine(baseDirectory, "App_Data", "Temp", string.Format("{0}.json", accessId));

            if (System.IO.File.Exists(jsonFilePath))
            {
                Collection<Feature> features = new Collection<Feature>();
                string[] geoJsons = System.IO.File.ReadAllLines(jsonFilePath);
                foreach (string geojson in geoJsons)
                {
                    features.Add(Feature.CreateFeatureFromGeoJson(geojson));
                }
                return features;
            }
            return null;
        }
    }
}