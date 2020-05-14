using System;
using System.Collections.ObjectModel;
using System.Configuration;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WmsServer;

namespace CustomWmsLayerPlugin
{
    public class CustomWmsLayerPlugin : WmsLayerPlugin
    {
        protected override RectangleShape GetBoundingBoxCore(string crs)
        {
            RectangleShape boundingBox = null;
            switch (crs)
            {
                case "EPSG:4326":
                case "CRS:84":
                    boundingBox = new RectangleShape(-180, 90, 180, -90);
                    break;
                case "EPSG:900913":
                case "EPSG:3857":
                    boundingBox = new RectangleShape(-20037508.2314698, 20037508.2314698, 20037508.2314698, -20037508.2314698);
                    break;
                default:
                    throw new ArgumentException("The input CRS is not supported by now, we only support EPSG:4326 and EPSG:900913 and CRS:84.");
            }

            return boundingBox;
        }

        protected override MapConfiguration GetMapConfigurationCore(string style, string crs)
        {
            MapConfiguration mapConfiguration = new MapConfiguration();

            string shapeFile = ConfigurationManager.AppSettings["Countries02"];
            ShapeFileFeatureLayer shapeFileFeatureLayer = new ShapeFileFeatureLayer(shapeFile);

            switch (crs)
            {
                case "EPSG:900913":
                case "EPSG:3857":
                    shapeFileFeatureLayer.FeatureSource.Projection = new Proj4Projection(4326, 3857);
                    break;
                case "EPSG:4326":
                case "CRS:84":
                    break;
                default:
                    throw new ArgumentException("The input CRS is not supported. Currently only EPSG:4326 and EPSG:900913 and CRS:84 are supported.");
            }

            switch (style)
            {
                case "Country1":
                    shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(
                        new GeoPen(new GeoSolidBrush(GeoColor.FromHtml("#0066ff")), 1f),
                        new GeoSolidBrush(GeoColor.FromHtml("#00ccff")));
                    break;
                case "Country2":
                    shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(
                        new GeoPen(new GeoSolidBrush(GeoColor.FromHtml("#669900")), 1f),
                        new GeoSolidBrush(GeoColor.FromHtml("#66ffcc")));
                    break;
            }

            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            mapConfiguration.Layers.Add(shapeFileFeatureLayer);

            return mapConfiguration;
        }

        protected override string GetNameCore()
        {
            return "Countries02";
        }

        protected override Collection<WmsLayerStyle> GetStylesCore()
        {
            Collection<WmsLayerStyle> styles = new Collection<WmsLayerStyle>
            {
                new WmsLayerStyle("Country1"),
                new WmsLayerStyle("Country2")
            };

            return styles;
        }

        protected override Collection<string> GetProjectionsCore()
        {
            Collection<string> projections = new Collection<string>
            {
                "EPSG:3857",
                "EPSG:900913",
                "CRS:84",
                "EPSG:4326"
            };
            return projections;
        }

        protected override GeographyUnit GetGeographyUnitCore(string crs)
        {
            GeographyUnit geographyUnit;
            switch (crs)
            {
                case "EPSG:4326":
                case "CRS:84":
                    geographyUnit = GeographyUnit.DecimalDegree;
                    break;
                case "EPSG:900913":
                case "EPSG:3857":
                    geographyUnit = GeographyUnit.Meter;
                    break;
                default:
                    throw new ArgumentException("The input CRS is not supported now, we only support EPSG:4326 and EPSG:900913 and CRS:84.");
            }

            return geographyUnit;
        }
    }
}
