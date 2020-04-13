using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using ThinkGeo.Core;
using ThinkGeo.UI.WebApi;

namespace Projection.Controllers
{
    [Route("Projection")]
    public class ProjectionController : ControllerBase
    {
        private static readonly string baseDirectory;
        private static readonly LayerOverlay customProjectionOverlay;
        private static readonly LayerOverlay rotaionProjectionOverlay;

        static ProjectionController()
        {
            baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "App_Data");

            // Initialize custom and rotation projection overlay.
            customProjectionOverlay = InitializeCustomProjectionOverlay();
            rotaionProjectionOverlay = InitializeRotaionProjectionOverlay();
        }

        /// <summary>
        /// Loads countries layer with different GeographyUnit.
        /// </summary>
        [Route("LoadCountriesLayer/{geographyUnit}/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult LoadCountriesLayer(int z, int x, int y, string geographyUnit)
        {
            string countriesFilePath = string.Format(@"{0}/Countries02.shp", baseDirectory);
            ShapeFileFeatureLayer countriesLayer = new ShapeFileFeatureLayer(countriesFilePath);
            countriesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColors.Green, 2);
            countriesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(countriesLayer);

            // Change projection by map unit.
            GeographyUnit mapUnit = GeographyUnit.DecimalDegree;
            if (geographyUnit == "Mercator")
            {
                // Initialize projection from decimal degree to meter.
                countriesLayer.FeatureSource.ProjectionConverter = new ProjectionConverter(4326, 3857);
                mapUnit = GeographyUnit.Meter;
            }

            return DrawTileImage(layerOverlay, mapUnit, z, x, y);
        }

        /// <summary>
        /// Loads countries layer with rotation projection.
        /// </summary>
        [Route("LoadRotationLayers/{angle}/{coordinateX}/{coordinateY}/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult LoadRotationLayers(int z, int x, int y, double angle, double coordinateX, double coordinateY)
        {
            // Creates rotation projection.
            RotationProjectionConverter projectionConverter = new RotationProjectionConverter(angle);
            projectionConverter.PivotCenter = new PointShape(coordinateX, coordinateY);

            UpdateRotationProjection(projectionConverter);

            return DrawTileImage(rotaionProjectionOverlay, GeographyUnit.Meter, z, x, y);
        }

        /// <summary>
        /// Gets projection information.
        /// </summary>
        [Route("InformationDataRequest/{epsgId}")]
        [HttpGet]
        public Dictionary<string, object> InformationDataRequest(int epsgId)
        {
            Dictionary<string, object> respond = new Dictionary<string, object>();

            // Get projection's Unit and Proj4String. 
            string epsgParameters = ThinkGeo.Core.Projection.GetProjStringByEpsgSrid(epsgId);
            string unit = ThinkGeo.Core.Projection.GetGeographyUnitFromProj(epsgParameters).ToString();
            respond.Add("Unit", unit);
            respond.Add("Proj4String", epsgParameters);

            return respond;
        }

        /// <summary>
        /// Loads countries layer with custom projection EPSG2163.
        /// </summary>
        [Route("LoadCustomProjectionLayer/{z}/{x}/{y}")]
        [HttpGet]
        public IActionResult LoadCustomProjectionLayer(int z, int x, int y)
        {
            return DrawTileImage(customProjectionOverlay, GeographyUnit.Meter, z, x, y);
        }

        /// <summary>
        /// Initialize custom projection overlay.
        /// </summary>
        /// <returns></returns>
        private static LayerOverlay InitializeCustomProjectionOverlay()
        {
            LayerOverlay layerOverlay = new LayerOverlay();
            // Add background lyer.
            BackgroundLayer backgroundLayer = new BackgroundLayer(new GeoSolidBrush(new GeoColor(255, 160, 207, 235)));
            layerOverlay.Layers.Add(backgroundLayer);

            // Add custom projection layer.
            string countriesFilePath = string.Format(@"{0}/Countries02.shp", baseDirectory);
            ShapeFileFeatureLayer countriesLayer = new ShapeFileFeatureLayer(countriesFilePath);
            countriesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoColors.Transparent), new GeoSolidBrush(new GeoColor(255, 250, 247, 243)));
            countriesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColors.Green, 2);
            countriesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;


            // Initialize projection from decimal degree to EPSG2163.
            ProjectionConverter decimalDegreeToEpsg2163 = new ProjectionConverter();
            decimalDegreeToEpsg2163.InternalProjection = new ThinkGeo.Core.Projection(4326);
            decimalDegreeToEpsg2163.ExternalProjection = new ThinkGeo.Core.Projection(ThinkGeo.Core.Projection.GetProjStringByEpsgSrid(2163));
            countriesLayer.FeatureSource.ProjectionConverter = decimalDegreeToEpsg2163;

            layerOverlay.Layers.Add(countriesLayer);

            return layerOverlay;
        }

        /// <summary>
        /// Initialize ratation projection overlay.
        /// </summary>
        /// <returns></returns>
        private static LayerOverlay InitializeRotaionProjectionOverlay()
        {
            LayerOverlay layerOverlay = new LayerOverlay();

            // Add background lyer.
            BackgroundLayer backgroundLayer = new BackgroundLayer(new GeoSolidBrush(new GeoColor(255, 160, 207, 235)));
            layerOverlay.Layers.Add(backgroundLayer);

            // Add countries layer.
            string countriesFilePath = string.Format(@"{0}/Countries.shp", baseDirectory);
            ShapeFileFeatureLayer countriesLayer = new ShapeFileFeatureLayer(countriesFilePath);
            countriesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoColors.Transparent), new GeoSolidBrush(new GeoColor(255, 250, 247, 243)));
            countriesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColors.Green, 2);
            countriesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add(countriesLayer);

            // Add lake layer.
            ShapeFileFeatureLayer lakeLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "Lake.shp"));
            lakeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoColors.Transparent, 0), new GeoSolidBrush(new GeoColor(255, 160, 207, 235)));
            lakeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add(lakeLayer);

            // Add highway layer.
            ShapeFileFeatureLayer highwayNetworkShapeLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "USHighwayNetwork.shp"));
            highwayNetworkShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = new LineStyle(new GeoPen(new GeoColor(255, 255, 222, 190), 1) { StartCap = DrawingLineCap.Round, EndCap = DrawingLineCap.Round });
            highwayNetworkShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add(highwayNetworkShapeLayer);

            // Add major cities layer
            ShapeFileFeatureLayer majorCitiesLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "USMajorCities.shp"));
            majorCitiesLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = new TextStyle("AREANAME", new GeoFont("Verdana", 7), new GeoSolidBrush(new GeoColor(255, 102, 102, 102)))
            {
                HaloPen = new GeoPen(new GeoColor(200, 255, 255, 255), 3),
                DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels,
                ForceLineCarriage = true,
                OverlappingRule = LabelOverlappingRule.NoOverlapping,
                GridSize = 0,
                TextPlacement = TextPlacement.Lower,
                YOffsetInPixel = 7
            };

            majorCitiesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add(majorCitiesLayer);

            return layerOverlay;
        }

        /// <summary>
        /// Update the layer's projection by rotation projection.
        /// </summary>
        /// <param name="projectionConverter"></param>
        private void UpdateRotationProjection(RotationProjectionConverter projectionConverter)
        {
            foreach (var layer in rotaionProjectionOverlay.Layers.OfType<FeatureLayer>())
            {
                layer.FeatureSource.ProjectionConverter = projectionConverter;
            }
        }

        /// <summary>
        /// Draws the map and return the image back to client in an HttpResponseMessage. 
        /// </summary>
        private IActionResult DrawTileImage(LayerOverlay layerOverlay, GeographyUnit geographyUnit, int z, int x, int y)
        {
            using (GeoImage image = new GeoImage(256, 256))
            {
                GeoCanvas geoCanvas = GeoCanvas.CreateDefaultGeoCanvas();
                RectangleShape boundingBox = WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, geographyUnit);
                geoCanvas.BeginDrawing(image, boundingBox, geographyUnit);
                layerOverlay.Draw(geoCanvas);
                geoCanvas.EndDrawing();

                byte[] imageBytes = image.GetImageBytes(GeoImageFormat.Png);

                return File(imageBytes, "image/png");
            }
        }
    }
}
