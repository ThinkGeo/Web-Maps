﻿using System;
using System.IO;
using System.Web.Mvc;
using ThinkGeo.Core;
using ThinkGeo.UI.WebApi;

namespace MvcSample.Areas.Home.Controllers
{
    public class HomeController : Controller
    {
        // GET: /Home/Index/
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Load ShapeFileFeatureLayer.
        /// </summary>
        [HttpGet]
        public ActionResult GetShapeFileTile(int z, int x, int y)
        {
            var baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");

            string shpFilePathName = string.Format(@"{0}/ShapeFile/USStates.shp", baseDirectory);
            ShapeFileFeatureLayer shapeFileFeatureLayer = new ShapeFileFeatureLayer(shpFilePathName);
            shapeFileFeatureLayer.Name = "shapeFile";
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush = new GeoSolidBrush(new GeoColor(50, GeoColors.Orange));
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColors.Black);
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(shapeFileFeatureLayer);

            return DrawTileImage(layerOverlay, z, x, y);
        }


        /// <summary>
        /// Draw the map and return the image back to client in an IActionResult.
        /// </summary>
        private ActionResult DrawTileImage(LayerOverlay layerOverlay, int z, int x, int y)
        {
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
    }
}