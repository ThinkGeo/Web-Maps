using System;
using System.IO;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.WebForms;

namespace DisplayMapBySpecifiedBoundary
{
    public partial class Main : System.Web.UI.Page
    {
        private RectangleShape boundingBox;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;

                string sourceShapeFile = MapPath(@"App_Data\MMR_adm0_900913.shp");
                ShapeFileFeatureLayer boundaryLayer = new ShapeFileFeatureLayer(sourceShapeFile);
                boundaryLayer.Open();
                boundingBox = boundaryLayer.GetBoundingBox();
                Map1.CurrentExtent = boundingBox;

                LayerOverlay layerOverlay = new LayerOverlay();
                OpenStreetMapLayer osmLayer = new OpenStreetMapLayer();
                osmLayer.SendingWebRequest += OsmLayer_SendingWebRequest;
                layerOverlay.Layers.Add(osmLayer);

                string coverShapeFile = sourceShapeFile.Replace(".shp", "_Cover.shp");
                if (!File.Exists(coverShapeFile))
                    GenerateCoverShapeFile(sourceShapeFile, coverShapeFile);

                ShapeFileFeatureLayer coverLayer = new ShapeFileFeatureLayer(coverShapeFile);
                coverLayer.DrawingQuality = DrawingQuality.HighSpeed;
                coverLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                coverLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color = GeoColors.White;
                coverLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                layerOverlay.Layers.Add(coverLayer);

                Map1.CustomOverlays.Add(layerOverlay);
            }
        }

        private void OsmLayer_SendingWebRequest(object sender, SendingWebRequestEventArgs e)
        {
            var requestCell = e.WebRequest.RequestUri.AbsolutePath.Split('.')[0].Split('/');
            int x = int.Parse(requestCell[2]);
            int y = int.Parse(requestCell[3]);
            int z = int.Parse(requestCell[1]);

            double zoomLevelScale = Map1.ZoomLevelSet.GetZoomLevels()[z].Scale;
            double resolution = zoomLevelScale * 0.0254000508 / 96;
            double cellWidth = 256 * resolution;
            double cellHeight = 256 * resolution;
            RectangleShape maxExtent = new RectangleShape(-20037508, 20037508, 20037508, -20037508);
            double minX = maxExtent.UpperLeftPoint.X + cellWidth * x;
            double maxY = maxExtent.UpperLeftPoint.Y - cellHeight * y;
            double maxX = minX + cellWidth;
            double minY = maxY - cellHeight;

            if (boundingBox.LowerLeftPoint.X > maxX || boundingBox.LowerLeftPoint.Y > maxY
                || boundingBox.UpperRightPoint.X < minX || boundingBox.UpperRightPoint.Y < minY)
                e.Cancel = true;
        }

        private void GenerateCoverShapeFile(string sourceShapeFile, string tagetShapeFile)
        {
            Feature coverFeature = new Feature(new RectangleShape(-20037508.34, 20037508.34, 20037508.34, -20037508.34));
            ShapeFileFeatureLayer shapeFileLayer = new ShapeFileFeatureLayer(sourceShapeFile);
            shapeFileLayer.Open();
            var sourceFeatures = shapeFileLayer.FeatureSource.GetAllFeatures(ReturningColumnsType.AllColumns);
            foreach (var sourceFeature in sourceFeatures)
                coverFeature = coverFeature.GetDifference(sourceFeature);

            DbfColumn dbfColumn = new DbfColumn("Id", DbfColumnType.IntegerInBinary, 4, 0);
            ShapeFileFeatureSource.CreateShapeFile(ShapeFileType.Polygon, tagetShapeFile, new[] { dbfColumn });
            ShapeFileFeatureLayer coverShapeFileLayer = new ShapeFileFeatureLayer(tagetShapeFile, GeoFileReadWriteMode.ReadWrite);
            coverShapeFileLayer.FeatureSource.Open();
            coverShapeFileLayer.FeatureSource.BeginTransaction();
            coverShapeFileLayer.FeatureSource.AddFeature(coverFeature);
            coverShapeFileLayer.FeatureSource.CommitTransaction();
            coverShapeFileLayer.FeatureSource.Close();
        }
    }
}