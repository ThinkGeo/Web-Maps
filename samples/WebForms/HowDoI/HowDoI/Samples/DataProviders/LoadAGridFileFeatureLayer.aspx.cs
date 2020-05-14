using System;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples.DataProviders
{
    public partial class LoadAGridFileFeatureLayer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-96.1156129546119, 29.9548238510752, -96.105810851851, 29.9474722740045);
                Map1.MapUnit = GeographyUnit.DecimalDegree;

                ClassBreakStyle gridClassBreakStyle = new ClassBreakStyle("CellValue");
                gridClassBreakStyle.ClassBreaks.Add(new ClassBreak(double.MinValue, new AreaStyle(new GeoSolidBrush(GeoColor.SimpleColors.Transparent))));
                gridClassBreakStyle.ClassBreaks.Add(new ClassBreak(0, new AreaStyle(new GeoSolidBrush(GeoColor.FromArgb(100, GeoColor.SimpleColors.Black)))));
                gridClassBreakStyle.ClassBreaks.Add(new ClassBreak(6.83, new AreaStyle(new GeoSolidBrush(GeoColor.StandardColors.Snow))));
                gridClassBreakStyle.ClassBreaks.Add(new ClassBreak(7.0, new AreaStyle(new GeoSolidBrush(GeoColor.StandardColors.Silver))));
                gridClassBreakStyle.ClassBreaks.Add(new ClassBreak(7.08, new AreaStyle(new GeoSolidBrush(GeoColor.StandardColors.Yellow))));
                gridClassBreakStyle.ClassBreaks.Add(new ClassBreak(7.15, new AreaStyle(new GeoSolidBrush(GeoColor.StandardColors.Blue))));
                gridClassBreakStyle.ClassBreaks.Add(new ClassBreak(7.21, new AreaStyle(new GeoSolidBrush(GeoColor.StandardColors.Green))));
                gridClassBreakStyle.ClassBreaks.Add(new ClassBreak(7.54, new AreaStyle(new GeoSolidBrush(GeoColor.StandardColors.Red))));

                GridFeatureLayer gridFeatureLayer = new GridFeatureLayer(MapPath("~/SampleData/world/PhValues.grd"));
                gridFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(gridClassBreakStyle);
                gridFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                ClassBreakStyle shapeFileClassBreakStyle = new ClassBreakStyle("PH");
                shapeFileClassBreakStyle.ClassBreaks.Add(new ClassBreak(double.MinValue, new PointStyle(PointSymbolType.Circle, new GeoSolidBrush(GeoColor.SimpleColors.Transparent), new GeoPen(GeoColor.StandardColors.Black), 10)));
                shapeFileClassBreakStyle.ClassBreaks.Add(new ClassBreak(0, new PointStyle(PointSymbolType.Circle, new GeoSolidBrush(GeoColor.FromArgb(255, 255, 0, 0)), new GeoPen(GeoColor.StandardColors.Black), 10)));
                shapeFileClassBreakStyle.ClassBreaks.Add(new ClassBreak(6.83, new PointStyle(PointSymbolType.Circle, new GeoSolidBrush(GeoColor.FromArgb(255, 255, 128, 0)), new GeoPen(GeoColor.StandardColors.Black), 10)));
                shapeFileClassBreakStyle.ClassBreaks.Add(new ClassBreak(7.0, new PointStyle(PointSymbolType.Circle, new GeoSolidBrush(GeoColor.FromArgb(255, 245, 210, 10)), new GeoPen(GeoColor.StandardColors.Black), 10)));
                shapeFileClassBreakStyle.ClassBreaks.Add(new ClassBreak(7.08, new PointStyle(PointSymbolType.Circle, new GeoSolidBrush(GeoColor.FromArgb(255, 255, 255, 0)), new GeoPen(GeoColor.StandardColors.Black), 10)));
                shapeFileClassBreakStyle.ClassBreaks.Add(new ClassBreak(7.15, new PointStyle(PointSymbolType.Circle, new GeoSolidBrush(GeoColor.FromArgb(255, 224, 251, 132)), new GeoPen(GeoColor.StandardColors.Black), 10)));
                shapeFileClassBreakStyle.ClassBreaks.Add(new ClassBreak(7.21, new PointStyle(PointSymbolType.Circle, new GeoSolidBrush(GeoColor.FromArgb(255, 128, 255, 128)), new GeoPen(GeoColor.StandardColors.Black), 10)));
                shapeFileClassBreakStyle.ClassBreaks.Add(new ClassBreak(7.54, new PointStyle(PointSymbolType.Circle, new GeoSolidBrush(GeoColor.FromArgb(255, 0, 255, 0)), new GeoPen(GeoColor.StandardColors.Black), 10)));

                ShapeFileFeatureLayer phLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/sampleph.shp"));
                phLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(shapeFileClassBreakStyle);
                phLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                Map1.StaticOverlay.Layers.Add(gridFeatureLayer);
                Map1.StaticOverlay.Layers.Add(phLayer);

                // The following two lines of code enable the client and server caching.
                // If you enable these features it will greatly increase the scalability of your
                // mapping application however there some side effects that may be counter intuitive.
                // Please read the white paper on web caching or the documentation regarding these methods.

                // Map1.StaticOverlay.ClientCache.CacheId = "WorldOverlay";
                // Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path);
            }
        }
    }
}
