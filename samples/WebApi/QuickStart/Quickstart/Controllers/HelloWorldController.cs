using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using ThinkGeo.Core;
using ThinkGeo.UI.WebApi;

namespace Quickstart.Controllers
{
    [RoutePrefix("HelloWorld")]
    public class HelloWorldController : ApiController
    {
        [Route("tile/{z}/{x}/{y}")]
        [HttpGet]
        public HttpResponseMessage GetTile(int z, int x, int y)
        {
            LayerOverlay layerOverlay = new LayerOverlay();

            // Create a new Layer and pass the path to a Shapefile into its constructor.
            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(HttpContext.Current.Server.MapPath("~/App_Data/cntry02.shp"));
            ProjectionConverter proj4 = new ProjectionConverter(Projection.GetWgs84ProjString(), Projection.GetSphericalMercatorProjString());
            if (!proj4.IsOpen) proj4.Open();
            worldLayer.FeatureSource.ProjectionConverter = proj4;

            // Set the worldLayer to use a preset Style. Since AreaStyles.Country1 has a YellowGreen background and Black border, our worldLayer will have the same render style. 
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = CreateCustomAreaStyle();

            // This setting will apply from ZoomLevel01 to ZoomLevel20, which means the map will be rendered in the same style, no matter how far we zoom in or out. 
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Capital layer
            ShapeFileFeatureLayer capitalLayer = new ShapeFileFeatureLayer(HttpContext.Current.Server.MapPath("~/App_Data/capital.shp"));
            capitalLayer.FeatureSource.ProjectionConverter = proj4;
            // We can customize our own Style. Here we pass in a color and a size.
            capitalLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyle.CreateSimpleCircleStyle(GeoColors.White, 7, GeoColors.Brown);
            // The Style we set here is applied from ZoomLevel01 to ZoomLevel05. That means if we zoom in a bit more, this particular style will no longer be visible.
            capitalLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level05;

            PointStyle customPointStyle = CreateCustomPointStyle();
            capitalLayer.ZoomLevelSet.ZoomLevel06.DefaultPointStyle = customPointStyle;
            // The Style we set here is applied from ZoomLevel06 to ZoomLevel20. That means if we zoom out a bit more, this particular style will no longer be visible.
            capitalLayer.ZoomLevelSet.ZoomLevel06.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // We can customize our own TextStyle. Here we pass in the font, the size, the style and the color.
            capitalLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = TextStyle.CreateSimpleTextStyle("CITY_NAME", "Arial", 8, DrawingFontStyles.Italic, GeoColors.Black, 3, 3);
            // The TextStyle we set here is applied from ZoomLevel01 to ZoomLevel05. 
            capitalLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level05;

            capitalLayer.ZoomLevelSet.ZoomLevel06.DefaultTextStyle = CreateCustomTextStyle("CITY_NAME");
            // The TextStyle we set here is applied from ZoomLevel06 to ZoomLevel20.
            capitalLayer.ZoomLevelSet.ZoomLevel06.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            // Change the number below (to 0, for example) to better understand how this works.

            layerOverlay.Layers.Add(worldLayer);
            layerOverlay.Layers.Add(capitalLayer);

            return DrawLayerOverlay(layerOverlay, z, x, y);
        }

        [Route("tile/{layerId}/{z}/{x}/{y}")]
        [HttpGet]
        public HttpResponseMessage GetTile(string layerId, int z, int x, int y)
        {
            LayerOverlay layerOverlay = new LayerOverlay();
            ShapeFileFeatureLayer shapeFileFeatureLayer;

            if (layerId.ToLowerInvariant() == "usstates")
            {
                // States layer
                shapeFileFeatureLayer = new ShapeFileFeatureLayer(HttpContext.Current.Server.MapPath("~/App_Data/USStates.shp"));
                shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = CreateCustomAreaStyle();
            }
            else
            {
                // Cities layer
                shapeFileFeatureLayer = new ShapeFileFeatureLayer(HttpContext.Current.Server.MapPath("~/App_Data/cities_a.shp"));
                shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = CreateCustomPointStyle();
                shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = CreateCustomTextStyle("AREANAME");
            }

            ProjectionConverter proj4 = new ProjectionConverter(Projection.GetWgs84ProjString(), Projection.GetSphericalMercatorProjString());
            if (!proj4.IsOpen) proj4.Open();
            shapeFileFeatureLayer.FeatureSource.ProjectionConverter = proj4;
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layerOverlay.Layers.Add(shapeFileFeatureLayer);

            return DrawLayerOverlay(layerOverlay, z, x, y);
        }

        private HttpResponseMessage DrawLayerOverlay(LayerOverlay layerOverlay, int z, int x, int y)
        {
            using (GeoImage bitmap = new GeoImage(256, 256))
            {
                GeoCanvas geoCanvas = GeoCanvas.CreateDefaultGeoCanvas();
                RectangleShape boundingBox = WebApiExtentHelper.GetBoundingBoxForXyz(x, y, z, GeographyUnit.Meter);
                geoCanvas.BeginDrawing(bitmap, boundingBox, GeographyUnit.Meter);
                layerOverlay.Draw(geoCanvas);
                geoCanvas.EndDrawing();

                MemoryStream ms = new MemoryStream();
                bitmap.Save(ms, GeoImageFormat.Png);

                HttpResponseMessage msg = new HttpResponseMessage(HttpStatusCode.OK);
                msg.Content = new ByteArrayContent(ms.ToArray());
                msg.Content.Headers.ContentType = new MediaTypeHeaderValue("image/png");

                return msg;
            }
        }

        private AreaStyle CreateCustomAreaStyle()
        {
            AreaStyle areaStyle = new AreaStyle();
            areaStyle.FillBrush = new GeoSolidBrush(GeoColor.FromArgb(255, 233, 232, 214));
            areaStyle.OutlinePen = new GeoPen(GeoColor.FromArgb(255, 118, 138, 69), 1);
            areaStyle.OutlinePen.DashStyle = LineDashStyle.Solid;
            return areaStyle;
        }

        private PointStyle CreateCustomPointStyle()
        {
            PointStyle pointStyle = new PointStyle();
            pointStyle.SymbolType = PointSymbolType.Square;
            pointStyle.FillBrush = new GeoSolidBrush(GeoColors.White);
            pointStyle.OutlinePen = new GeoPen(GeoColors.Black, 1);
            pointStyle.SymbolSize = 6;

            PointStyle stackStyle = new PointStyle();
            stackStyle.SymbolType = PointSymbolType.Square;
            stackStyle.FillBrush = new GeoSolidBrush(GeoColors.Maroon);
            stackStyle.OutlinePen = new GeoPen(GeoColors.Transparent, 0);
            stackStyle.SymbolSize = 2;

            pointStyle.CustomPointStyles.Add(stackStyle);
            return pointStyle;
        }

        private TextStyle CreateCustomTextStyle(string columnName)
        {
            GeoFont font = new GeoFont("Arial", 9, DrawingFontStyles.Bold);
            GeoSolidBrush txtBrush = new GeoSolidBrush(GeoColors.Maroon);
            TextStyle textStyle = new TextStyle(columnName, font, txtBrush);
            textStyle.XOffsetInPixel = 0;
            textStyle.YOffsetInPixel = -6;

            return textStyle;
        }
    }
}