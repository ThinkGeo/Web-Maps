using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebApi;

namespace TopologyValidation
{
    [RoutePrefix("TopologyValidation")]
    public class TopologyController : ApiController
    {
        public TopologyController()
        { }

        /// <summary>
        /// Refreshs source layer.
        /// </summary>
        [Route("RefreshSourceLayer/{topologyType}/{z}/{x}/{y}")]
        [HttpGet]
        public HttpResponseMessage RefreshSourceLayer(string topologyType, int z, int x, int y)
        {
            LayerOverlay layerOverlay = new LayerOverlay();
            SourceDataItem inputData = TopologyHelper.GetSourceDataById(topologyType);
            layerOverlay.Layers.Add(GetFirstInputLayer(inputData.FirstInputFeatures));
            layerOverlay.Layers.Add(GetSecondInputLayer(inputData.SecondInputFeatures));

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Gets selected topology type's description.
        /// </summary>
        [Route("GetSelectedDescription/{topologyType}")]
        [HttpGet]
        public string GetSelectedDescription(string topologyType)
        {
            return TopologyHelper.GetSourceDataById(topologyType).Comment;
        }

        /// <summary>
        /// Validates lines topology.
        /// </summary>
        [Route("LinesTopology/{topologyType}/{clusterTolerance}/{z}/{x}/{y}")]
        [HttpGet]
        public HttpResponseMessage ValidateLinesTopology(string topologyType, string clusterTolerance, int z, int x, int y)
        {
            Collection<Feature> resultFeatures = null;

            SourceDataItem inputData = TopologyHelper.GetSourceDataById(topologyType);
            TopologyType currentTopologyType = (TopologyType)Enum.Parse(typeof(TopologyType), topologyType);

            switch (currentTopologyType)
            {
                case TopologyType.LinesEndpointMustBeCoveredByPoints:
                    resultFeatures = TopologyValidator.LinesEndPointMustBeCoveredByPoints(inputData.FirstInputFeatures, inputData.SecondInputFeatures);
                    break;

                case TopologyType.LinesMustBeCoveredByBoundaryOfPolygons:
                    resultFeatures = TopologyValidator.LinesMustBeCoveredByBoundaryOfPolygons(inputData.FirstInputFeatures, inputData.SecondInputFeatures);
                    break;

                case TopologyType.LinesMustBeCoveredByFeatureClassOfLines:
                    resultFeatures = TopologyValidator.LinesMustBeCoveredByFeatureClassOfLines(inputData.FirstInputFeatures, inputData.SecondInputFeatures);
                    break;

                case TopologyType.LinesMustBeLargerThanClusterTolerance:
                    if (!string.IsNullOrEmpty(clusterTolerance))
                        resultFeatures = TopologyValidator.LinesMustBeLargerThanClusterTolerance(inputData.FirstInputFeatures, Convert.ToDouble(clusterTolerance));
                    break;

                case TopologyType.LinesMustBeSinglePart:
                    resultFeatures = TopologyValidator.LinesMustBeSinglePart(inputData.FirstInputFeatures);
                    break;

                case TopologyType.LinesMustNotHaveDangles:
                    resultFeatures = TopologyValidator.LinesMustNotHaveDangles(inputData.FirstInputFeatures);
                    break;

                case TopologyType.LinesMustNotHavePseudonodes:
                    resultFeatures = TopologyValidator.LinesMustNotHavePseudonodes(inputData.FirstInputFeatures);
                    break;

                case TopologyType.LinesMustNotIntersectOrTouchInterior:
                    resultFeatures = TopologyValidator.LinesMustNotIntersectOrTouchInterior(inputData.FirstInputFeatures);
                    break;

                case TopologyType.LinesMustNotIntersect:
                    resultFeatures = TopologyValidator.LinesMustNotIntersect(inputData.FirstInputFeatures);
                    break;

                case TopologyType.LinesMustNotOverlap:
                    resultFeatures = TopologyValidator.LinesMustNotOverlap(inputData.FirstInputFeatures);
                    break;

                case TopologyType.LinesMustNotOverlapWithLines:
                    resultFeatures = TopologyValidator.LinesMustNotOverlapWithLines(inputData.FirstInputFeatures, inputData.SecondInputFeatures);
                    break;

                case TopologyType.LinesMustNotSelfIntersect:
                    resultFeatures = TopologyValidator.LinesMustNotSelfIntersect(inputData.FirstInputFeatures);
                    break;

                case TopologyType.LinesMustNotSelfOverlap:
                    resultFeatures = TopologyValidator.LinesMustNotSelfOverlap(inputData.FirstInputFeatures);
                    break;
            }

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(GetFirstInputLayer(inputData.FirstInputFeatures));
            layerOverlay.Layers.Add(GetSecondInputLayer(inputData.SecondInputFeatures));
            layerOverlay.Layers.Add(GetResultFeaturesLayer(currentTopologyType, resultFeatures));

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Validates points topology.
        /// </summary>
        [Route("PointsTopology/{topologyType}/{clusterTolerance}/{z}/{x}/{y}")]
        [HttpGet]
        public HttpResponseMessage ValidatePointsTopology(string topologyType, int z, int x, int y)
        {
            Collection<Feature> resultFeatures = null;

            SourceDataItem inputData = TopologyHelper.GetSourceDataById(topologyType);
            TopologyType currentTopologyType = (TopologyType)Enum.Parse(typeof(TopologyType), topologyType);

            switch (currentTopologyType)
            {
                case TopologyType.PointsMustBeCoveredByBoundaryOfPolygons:
                    resultFeatures = TopologyValidator.PointsMustBeCoveredByBoundaryOfPolygons(inputData.FirstInputFeatures, inputData.SecondInputFeatures);
                    break;

                case TopologyType.PointsMustBeCoveredByEndpointOfLines:
                    resultFeatures = TopologyValidator.PointsMustBeCoveredByEndPointOfLines(inputData.FirstInputFeatures, inputData.SecondInputFeatures);
                    break;

                case TopologyType.PointsMustBeCoveredByLines:
                    resultFeatures = TopologyValidator.PointsMustBeCoveredByLines(inputData.FirstInputFeatures, inputData.SecondInputFeatures);
                    break;

                case TopologyType.PointsMustBeProperlyInsidePolygons:
                    resultFeatures = TopologyValidator.PointsMustBeProperlyInsidePolygons(inputData.FirstInputFeatures, inputData.SecondInputFeatures);
                    break;
            }

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(GetFirstInputLayer(inputData.FirstInputFeatures));
            layerOverlay.Layers.Add(GetSecondInputLayer(inputData.SecondInputFeatures));
            layerOverlay.Layers.Add(GetResultFeaturesLayer(currentTopologyType, resultFeatures));

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Validates polygons topology.
        /// </summary>
        [Route("PolygonsTopology/{topologyType}/{clusterTolerance}/{z}/{x}/{y}")]
        [HttpGet]
        public HttpResponseMessage ValidatePolygonsTopology(string topologyType, string clusterTolerance, int z, int x, int y)
        {
            Collection<Feature> resultFeatures = null;

            SourceDataItem inputData = TopologyHelper.GetSourceDataById(topologyType);
            TopologyType currentTopologyType = (TopologyType)Enum.Parse(typeof(TopologyType), topologyType);
            switch (currentTopologyType)
            {
                case TopologyType.PolygonsBoundaryMustBeCoveredByBoundaryOfPolygons:
                    resultFeatures = TopologyValidator.PolygonsBoundaryMustBeCoveredByBoundaryOfPolygons(inputData.FirstInputFeatures, inputData.SecondInputFeatures);
                    break;

                case TopologyType.PolygonsBoundaryMustBeCoveredByLines:
                    resultFeatures = TopologyValidator.PolygonsBoundaryMustBeCoveredByLines(inputData.FirstInputFeatures, inputData.SecondInputFeatures);
                    break;

                case TopologyType.PolygonsMustBeCoveredByFeatureClassOfPolygons:
                    resultFeatures = TopologyValidator.PolygonsMustBeCoveredByFeatureClassOfPolygons(inputData.FirstInputFeatures, inputData.SecondInputFeatures);
                    break;

                case TopologyType.PolygonsMustBeCoveredByPolygons:
                    resultFeatures = TopologyValidator.PolygonsMustBeCoveredByPolygons(inputData.FirstInputFeatures, inputData.SecondInputFeatures);
                    break;

                case TopologyType.PolygonsMustBeLargerThanClusterTolerance:
                    if (!string.IsNullOrEmpty(clusterTolerance))
                        resultFeatures = TopologyValidator.PolygonsMustBeLargerThanClusterTolerance(inputData.FirstInputFeatures, Convert.ToDouble(clusterTolerance));
                    break;

                case TopologyType.PolygonsMustContainPoint:
                    resultFeatures = TopologyValidator.PolygonsMustContainPoint(inputData.FirstInputFeatures, inputData.SecondInputFeatures);
                    break;

                case TopologyType.PolygonsMustCoverEachOther:
                    resultFeatures = TopologyValidator.PolygonsMustCoverEachOther(inputData.FirstInputFeatures, inputData.SecondInputFeatures);
                    break;

                case TopologyType.PolygonsMustNotHaveGaps:
                    resultFeatures = TopologyValidator.PolygonsMustNotHaveGaps(inputData.FirstInputFeatures);
                    break;

                case TopologyType.PolygonsMustNotOverlap:
                    resultFeatures = TopologyValidator.PolygonsMustNotOverlap(inputData.FirstInputFeatures);
                    break;

                case TopologyType.PolygonsMustNotOverlapWithPolygons:
                    resultFeatures = TopologyValidator.PolygonsMustNotOverlapWithPolygons(inputData.FirstInputFeatures, inputData.SecondInputFeatures);
                    break;
            }

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(GetFirstInputLayer(inputData.FirstInputFeatures));
            layerOverlay.Layers.Add(GetSecondInputLayer(inputData.SecondInputFeatures));
            layerOverlay.Layers.Add(GetResultFeaturesLayer(currentTopologyType, resultFeatures));

            return DrawTileImage(layerOverlay, z, x, y);
        }

        /// <summary>
        /// Draws the map and return the image back to client in an HttpResponseMessage.
        /// </summary>
        private HttpResponseMessage DrawTileImage(LayerOverlay layerOverlay, int z, int x, int y)
        {
            using (Bitmap bitmap = new Bitmap(256, 256))
            {
                PlatformGeoCanvas geoCanvas = new PlatformGeoCanvas();
                RectangleShape boundingBox = WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, GeographyUnit.Meter);
                geoCanvas.BeginDrawing(bitmap, boundingBox, GeographyUnit.Meter);
                layerOverlay.Draw(geoCanvas);
                geoCanvas.EndDrawing();

                MemoryStream ms = new MemoryStream();
                bitmap.Save(ms, ImageFormat.Png);

                HttpResponseMessage msg = new HttpResponseMessage(HttpStatusCode.OK);
                msg.Content = new ByteArrayContent(ms.ToArray());
                msg.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

                return msg;
            }
        }

        /// <summary>
        /// Gets first input layer.
        /// </summary>
        private InMemoryFeatureLayer GetFirstInputLayer(Collection<Feature> features)
        {
            InMemoryFeatureLayer firstInputFeatureLayer = new InMemoryFeatureLayer();
            firstInputFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(102, 0, 0, 255), GeoColor.FromArgb(255, 0, 0, 255), 2);
            firstInputFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(102, 0, 0, 255), 2, true);
            firstInputFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.FromArgb(102, 0, 0, 255), 12);
            firstInputFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            if (features.Count > 0)
            {
                foreach (var item in features)
                {
                    firstInputFeatureLayer.InternalFeatures.Add(item);
                }
            }

            return firstInputFeatureLayer;
        }

        /// <summary>
        /// Gets second input layer.
        /// </summary>
        private InMemoryFeatureLayer GetSecondInputLayer(Collection<Feature> features)
        {
            InMemoryFeatureLayer secondInputFeatureLayere = new InMemoryFeatureLayer();
            secondInputFeatureLayere.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(102, 0, 255, 0), GeoColor.FromArgb(255, 0, 255, 0), 2);
            secondInputFeatureLayere.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(102, 0, 255, 0), 2, true);
            secondInputFeatureLayere.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.FromArgb(102, 0, 255, 0), 12);
            secondInputFeatureLayere.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            if (features.Count > 0)
            {
                foreach (var item in features)
                {
                    secondInputFeatureLayere.InternalFeatures.Add(item);
                }
            }

            return secondInputFeatureLayere;
        }

        /// <summary>
        /// Gets result features layer.
        /// </summary>
        private InMemoryFeatureLayer GetResultFeaturesLayer(TopologyType topologyType, Collection<Feature> resultFeatures)
        {
            InMemoryFeatureLayer resultFeatureLayer = new InMemoryFeatureLayer();
            if (topologyType == TopologyType.LinesMustBeLargerThanClusterTolerance ||
                topologyType == TopologyType.PolygonsMustBeLargerThanClusterTolerance)
            {
                resultFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(200, GeoColor.StandardColors.Green), 2, true);
                resultFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(102, 0, 255, 0), GeoColor.FromArgb(255, 0, 0, 255), 2);
            }
            else
            {
                resultFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(new GeoColor(180, GeoColor.StandardColors.Red), 2, true);
                resultFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(new GeoColor(180, GeoColor.StandardColors.Red), 12);
                resultFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(new GeoColor(180, GeoColor.StandardColors.Red), GeoColor.StandardColors.Black);
            }
            resultFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            if (resultFeatures != null && resultFeatures.Count > 0)
            {
                foreach (var item in resultFeatures)
                {
                    resultFeatureLayer.InternalFeatures.Add(item);
                }
            }

            return resultFeatureLayer;
        }
    }
}
