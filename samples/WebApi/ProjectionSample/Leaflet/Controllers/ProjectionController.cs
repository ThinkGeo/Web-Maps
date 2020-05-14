using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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

namespace Projection.Controllers
{
    [RoutePrefix("Projection")]
    public class ProjectionController : ApiController
    {
        private static readonly string baseDirectory;
        private static readonly LayerOverlay customProjectionOverlay;
        private static readonly LayerOverlay rotaionProjectionOverlay;

        static ProjectionController()
        {
            baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");

            // Initialize custom and rotation projection overlay.
            customProjectionOverlay = InitializeCustomProjectionOverlay();
            rotaionProjectionOverlay = InitializeRotaionProjectionOverlay();
        }

        /// <summary>
        /// Loads countries layer with different GeographyUnit.
        /// </summary>
        [Route("LoadCountriesLayer/{geographyUnit}/{z}/{x}/{y}")]
        [HttpGet]
        public HttpResponseMessage LoadCountriesLayer(int z, int x, int y, string geographyUnit)
        {
            string countriesFilePath = string.Format(@"{0}/Countries02.shp", baseDirectory);
            ShapeFileFeatureLayer countriesLayer = new ShapeFileFeatureLayer(countriesFilePath);
            countriesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColor.SimpleColors.Green, 2);
            countriesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(countriesLayer);

            // Initialize projection from decimal degree to meter.
            Proj4Projection decimalDegreeToMeter = new Proj4Projection();
            decimalDegreeToMeter.InternalProjectionParametersString = Proj4Projection.GetWgs84ParametersString();
            decimalDegreeToMeter.ExternalProjectionParametersString = Proj4Projection.GetSphericalMercatorParametersString();
            decimalDegreeToMeter.Open();

            // Change projection by map unit.
            GeographyUnit mapUnit = GeographyUnit.DecimalDegree;
            if (geographyUnit == "Mercator")
            {
                countriesLayer.FeatureSource.Projection = decimalDegreeToMeter;
                mapUnit = GeographyUnit.Meter;
            }

            return DrawTileImage(layerOverlay, mapUnit, z, x, y);
        }

        /// <summary>
        /// Loads countries layer with rotation projection.
        /// </summary>
        [Route("LoadRotationLayers/{angle}/{coordinateX}/{coordinateY}/{z}/{x}/{y}")]
        [HttpGet]
        public HttpResponseMessage LoadRotationLayers(int z, int x, int y, double angle, double coordinateX, double coordinateY)
        {
            // Creates rotation projection.
            RotationProjection projection = new RotationProjection(angle);
            projection.PivotCenter = new PointShape(coordinateX, coordinateY);
            projection.Open();

            UpdateRotationProjection(projection);

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
            string epsgParameters = Proj4Projection.GetEpsgParametersString(epsgId);
            string unit = Proj4Projection.GetGeographyUnitFromProj4(epsgParameters).ToString();
            respond.Add("Unit", unit);
            respond.Add("Proj4String", epsgParameters);

            return respond;
        }

        /// <summary>
        /// Loads countries layer with custom projection EPSG2163.
        /// </summary>
        [Route("LoadCustomProjectionLayer/{z}/{x}/{y}")]
        [HttpGet]
        public HttpResponseMessage LoadCustomProjectionLayer(int z, int x, int y)
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
            BackgroundLayer backgroundLayer = new BackgroundLayer(WorldStreetsAreaStyles.Water().FillSolidBrush);
            layerOverlay.Layers.Add(backgroundLayer);

            // Add custom projection layer.
            string countriesFilePath = string.Format(@"{0}/Countries02.shp", baseDirectory);
            ShapeFileFeatureLayer countriesLayer = new ShapeFileFeatureLayer(countriesFilePath);
            countriesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = WorldStreetsAreaStyles.BaseLand();
            countriesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColor.SimpleColors.Green, 2);
            countriesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;


            // Initialize projection from decimal degree to EPSG2163.
            Proj4Projection decimalDegreeToEpsg2163 = new Proj4Projection();
            decimalDegreeToEpsg2163.InternalProjectionParametersString = Proj4Projection.GetDecimalDegreesParametersString();
            decimalDegreeToEpsg2163.ExternalProjectionParametersString = Proj4Projection.GetEpsgParametersString(2163);
            decimalDegreeToEpsg2163.Open();

            countriesLayer.FeatureSource.Projection = decimalDegreeToEpsg2163;

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
            BackgroundLayer backgroundLayer = new BackgroundLayer(WorldStreetsAreaStyles.Water().FillSolidBrush);
            layerOverlay.Layers.Add(backgroundLayer);

            // Add countries layer.
            string countriesFilePath = string.Format(@"{0}/Countries.shp", baseDirectory);
            ShapeFileFeatureLayer countriesLayer = new ShapeFileFeatureLayer(countriesFilePath);
            countriesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = WorldStreetsAreaStyles.BaseLand();
            countriesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColor.SimpleColors.Green, 2);
            countriesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add(countriesLayer);

            // Add lake layer.
            ShapeFileFeatureLayer lakeLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "Lake.shp"));
            lakeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = WorldStreetsAreaStyles.Water();
            lakeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add(lakeLayer);

            // Add highway layer.
            ShapeFileFeatureLayer highwayNetworkShapeLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "USHighwayNetwork.shp"));
            highwayNetworkShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = WorldStreetsLineStyles.Highway(1);
            highwayNetworkShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add(highwayNetworkShapeLayer);

            // Add major cities layer
            ShapeFileFeatureLayer majorCitiesLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "USMajorCities.shp"));
            majorCitiesLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = WorldStreetsTextStyles.Poi("AREANAME", 7, 7);
            majorCitiesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add(majorCitiesLayer);

            return layerOverlay;
        }

        /// <summary>
        /// Update the layer's projection by rotation projection.
        /// </summary>
        /// <param name="projection"></param>
        private void UpdateRotationProjection(RotationProjection projection)
        {
            foreach (var layer in rotaionProjectionOverlay.Layers.OfType<FeatureLayer>())
            {
                layer.FeatureSource.Projection = projection;
            }
        }

        /// <summary>
        /// Draws the map and return the image back to client in an HttpResponseMessage. 
        /// </summary>
        private HttpResponseMessage DrawTileImage(LayerOverlay layerOverlay, GeographyUnit geographyUnit, int z, int x, int y)
        {
            using (Bitmap bitmap = new Bitmap(256, 256))
            {
                PlatformGeoCanvas geoCanvas = new PlatformGeoCanvas();
                RectangleShape boundingBox = WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, geographyUnit);
                geoCanvas.BeginDrawing(bitmap, boundingBox, geographyUnit);
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
    }
}
