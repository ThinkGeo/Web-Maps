using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using ThinkGeo.MapSuite.Core;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebApi;

namespace Visualization
{
    public static class OverlayBuilder
    {
        /// <summary>
        /// Gets an overlay applied with ClassBreak style.
        /// </summary>
        public static LayerOverlay GetOverlayWithClassBreakStyle()
        {
            // Get the file path name from its relative path.
			string shpFilePathName = GetFullPath("App_Data/usStatesCensus2010.shp");

            ShapeFileFeatureLayer shapeFileFeatureLayer = new ShapeFileFeatureLayer(shpFilePathName);
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(GetClassBreakStyle());

            // Apply projection to the shape file which is used for display.
            shapeFileFeatureLayer.FeatureSource.Projection = GetProjection();

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(shapeFileFeatureLayer);

            return layerOverlay;
        }

        /// <summary>
        /// Gets an overlay applied with ClusterPoint style.
        /// </summary>
        public static LayerOverlay GetOverlayWithClusterPointStyle()
        {
            // Get the file path name from its relative path.
			string shpFilePathName = GetFullPath("App_Data/usEarthquake.shp");

            ShapeFileFeatureLayer shapeFileFeatureLayer = new ShapeFileFeatureLayer(shpFilePathName);
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(GetClusterPointStyle());

            // Apply projection to the shape file which is used for display.
            shapeFileFeatureLayer.FeatureSource.Projection = GetProjection();

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(shapeFileFeatureLayer);

            return layerOverlay;
        }

        /// <summary>
        /// Gets an overlay applied with a weather custmized style.
        /// </summary>
        public static LayerOverlay GetOverlayWithCustomeStyle()
        {
            // Get the layers which have been applied the customized style.
            Collection<FeatureLayer> styleLayers = GetCustomStyleLayers();

            LayerOverlay layerOverlay = new LayerOverlay();
            for (int i = 0; i < styleLayers.Count; i++)
            {
                string styleId = string.Format(CultureInfo.InvariantCulture, "CustomStyle{0}", i.ToString(CultureInfo.InvariantCulture));
                layerOverlay.Layers.Add(styleId, styleLayers[i]);
            }

            return layerOverlay;
        }

        /// <summary>
        /// Gets an overlay applied with DotDensity style.
        /// </summary>
        public static LayerOverlay GetOverlayWithDotDensityStyle()
        {
            // Get the file path name from its relative path.
			string shpFilePathName = GetFullPath("App_Data/usStatesCensus2010.shp");

            ShapeFileFeatureLayer shapeFileFeatureLayer = new ShapeFileFeatureLayer(shpFilePathName);
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(20, GeoColor.FromHtml("#00e6fe")), GeoColor.StandardColors.Gray));
            // Create the DotDensityStyle
            DotDensityStyle dotDensityStyle = new DotDensityStyle("Population", 0.0000094778167166538189, PointStyles.CreateSimpleCircleStyle(GeoColor.StandardColors.Black, 7));
            dotDensityStyle.CustomPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.FromHtml("#a57431"), 5);
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(dotDensityStyle);
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Apply projection to the shape file which is used for display.
            shapeFileFeatureLayer.FeatureSource.Projection = GetProjection();

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(shapeFileFeatureLayer);
            return layerOverlay;
        }

        /// <summary>
        /// Gets an overlay applied with IsoLine style.
        /// </summary>
        public static LayerOverlay GetOverlayWithIsoLineStyle()
        {
            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(GetDynamicIsoLineLayer());

            return layerOverlay;
        }

        /// <summary>
        /// Gets an overlay applied with Filter style.
        /// </summary>
        public static LayerOverlay GetOverlayWithFilterStyle()
        {
            // Get the file path name from its relative path.
			string shpFilePathName = GetFullPath(@"App_Data/usStatesCensus2010.shp");

            ShapeFileFeatureLayer shapeFileFeatureLayer = new ShapeFileFeatureLayer(shpFilePathName);
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            // Create the FilterStyle.
            FilterStyle filterStyle = new FilterStyle();
            filterStyle.Conditions.Add(new FilterCondition("Population", ">2967297"));
            filterStyle.Styles.Add(AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(130, GeoColor.FromHtml("#ffb74c")), GeoColor.FromHtml("#333333")));
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(filterStyle);

            // Apply projection to the shape file which is used for display
            shapeFileFeatureLayer.FeatureSource.Projection = GetProjection();

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(shapeFileFeatureLayer);
            return layerOverlay;
        }

        /// <summary>
        /// Gets an overlay applied with heatmap style.
        /// </summary>
        public static LayerOverlay GetOverlayWithHeatStyle()
        {
            // Get the file path name from its relative path.
			string shpFilePathName = GetFullPath("App_Data/usEarthquake.shp");

            ShapeFileFeatureLayer shapeFileFeatureLayer = new ShapeFileFeatureLayer(shpFilePathName);
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new HeatStyle(10, 150, "MAGNITUDE", 0, 12, 100, DistanceUnit.Kilometer));
            shapeFileFeatureLayer.DrawingMarginInPixel = 300;

            // Apply projection to the shape file which is used for display
            shapeFileFeatureLayer.FeatureSource.Projection = GetProjection();

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(shapeFileFeatureLayer);
            return layerOverlay;
        }

        /// <summary>
        /// Gets an overlay applied with icon style.
        /// </summary>
        public static LayerOverlay GetOverlayWithIconStyle()
        {
            // Get the file path name from its relative path.
			string shpFilePathName = GetFullPath("App_Data/Vehicles.shp");

            ShapeFileFeatureLayer shapeFileFeatureLayer = new ShapeFileFeatureLayer(shpFilePathName);
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(GetIconStyle());

            // Apply projection to the shape file which is used for display.
            shapeFileFeatureLayer.FeatureSource.Projection = GetProjection();

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(shapeFileFeatureLayer);
            return layerOverlay;
        }

        /// <summary>
        /// Gets an overlay applied with ZedGraph style.
        /// </summary>
        public static LayerOverlay GetOverlayWithZedGraphStyle()
        {
            // Get the file path name from its relative path.
			string shpFilePathName = GetFullPath(@"App_Data/MajorCities.shp");

            ShapeFileFeatureLayer shapeFileFeatureLayer = new ShapeFileFeatureLayer(shpFilePathName);
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            TextStyle textStyle = TextStyles.CreateSimpleTextStyle("AREANAME", "Arial", 10, DrawingFontStyles.Regular, GeoColor.StandardColors.Black, GeoColor.SimpleColors.White, 2, -20, 40);
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(new PieChartStyle());
            shapeFileFeatureLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(textStyle);
            shapeFileFeatureLayer.DrawingMarginInPixel = 100;

            // Apply projection to the shape file which is used for display.
            Proj4Projection Proj4Projection = new Proj4Projection();
            Proj4Projection.InternalProjectionParametersString = Proj4Projection.GetWgs84ParametersString();
            Proj4Projection.ExternalProjectionParametersString = Proj4Projection.GetSphericalMercatorParametersString();
            shapeFileFeatureLayer.FeatureSource.Projection = Proj4Projection;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(shapeFileFeatureLayer);

            return layerOverlay;
        }

        private static ClassBreakStyle GetClassBreakStyle()
        {
            // Define the values which are used as the class breaks.
            double[] classBreakValues = new double[] { 0, 814180.0, 1328361.0, 2059179.0, 2967297.0, 4339367.0, 5303925.0, 6392017.0, 8791894.0 };
            // Create a color family for displying the features.
            Collection<GeoColor> familyColors = GeoColor.GetColorsInQualityFamily(GeoColor.FromArgb(255, 116, 160, 255), GeoColor.FromArgb(255, 220, 52, 56), 10, ColorWheelDirection.CounterClockwise);

            ClassBreakStyle classBreakStyle = new ClassBreakStyle("Population", BreakValueInclusion.IncludeValue);
            for (int i = 0; i < classBreakValues.Length; i++)
            {
                classBreakStyle.ClassBreaks.Add(new ClassBreak(classBreakValues[i], AreaStyles.CreateSimpleAreaStyle(new GeoColor(150, familyColors[i]), GeoColor.FromHtml("#f05133"), 1)));
            }
            return classBreakStyle;
        }

        private static ClassBreakClusterPointStyle GetClusterPointStyle()
        {
            ClassBreakClusterPointStyle clusterPointStyle = new ClassBreakClusterPointStyle();
            clusterPointStyle.CellSize = 65;

            // Create the PointStyle for different class breaks.
            PointStyle pointStyle1 = new PointStyle(PointSymbolType.Circle, new GeoSolidBrush(GeoColor.FromArgb(250, 222, 226, 153)), new GeoPen(GeoColor.FromArgb(100, 222, 226, 153), 5), 8);
            clusterPointStyle.ClassBreakPoints.Add(1, pointStyle1);

            PointStyle pointStyle2 = new PointStyle(PointSymbolType.Circle, new GeoSolidBrush(GeoColor.FromArgb(250, 222, 226, 153)), new GeoPen(GeoColor.FromArgb(100, 222, 226, 153), 8), 15);
            clusterPointStyle.ClassBreakPoints.Add(2, pointStyle2);

            PointStyle pointStyle3 = new PointStyle(PointSymbolType.Circle, new GeoSolidBrush(GeoColor.FromArgb(250, 255, 183, 76)), new GeoPen(GeoColor.FromArgb(100, 255, 183, 76), 10), 25);
            clusterPointStyle.ClassBreakPoints.Add(50, pointStyle3);

            PointStyle pointStyle4 = new PointStyle(PointSymbolType.Circle, new GeoSolidBrush(GeoColor.FromArgb(250, 243, 193, 26)), new GeoPen(GeoColor.FromArgb(100, 243, 193, 26), 15), 35);
            clusterPointStyle.ClassBreakPoints.Add(150, pointStyle4);

            PointStyle pointStyle5 = new PointStyle(PointSymbolType.Circle, new GeoSolidBrush(GeoColor.FromArgb(250, 245, 7, 10)), new GeoPen(GeoColor.FromArgb(100, 245, 7, 10), 15), 40);
            clusterPointStyle.ClassBreakPoints.Add(350, pointStyle5);

            PointStyle pointStyle6 = new PointStyle(PointSymbolType.Circle, new GeoSolidBrush(GeoColor.FromArgb(250, 245, 7, 10)), new GeoPen(GeoColor.FromArgb(100, 245, 7, 10), 20), 50);
            clusterPointStyle.ClassBreakPoints.Add(500, pointStyle6);

            clusterPointStyle.TextStyle = TextStyles.CreateSimpleTextStyle("FeatureCount", "Arail", 10, DrawingFontStyles.Regular, GeoColor.SimpleColors.Black);
            clusterPointStyle.TextStyle.PointPlacement = PointPlacement.Center;

            return clusterPointStyle;
        }

        private static Collection<FeatureLayer> GetCustomStyleLayers()
        {
            Collection<FeatureLayer> featureLayers = new Collection<FeatureLayer>();

            // Create the weather line style.
            LineStyle lineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.FromArgb(255, 50, 0, 249), 4, false);

            InMemoryFeatureLayer inMemoryFeatureLayerColdFront = new InMemoryFeatureLayer();
            // Cold Front Icon Style.
            string[] temperatureIcons = new string[] { GetFullPath(@"App_Data/CustomStyles/offset_circle_red_bl.png"), GetFullPath(@"App_Data/CustomStyles/offset_triangle_blue_revert.png") };
            CustomGeoImageLineStyle coldFrontLineStyle = new CustomGeoImageLineStyle(lineStyle, temperatureIcons.Select(p => new GeoImage(p)), 19, ImageDirection.Right);
            inMemoryFeatureLayerColdFront.ZoomLevelSet.ZoomLevel05.CustomStyles.Add(coldFrontLineStyle);
            inMemoryFeatureLayerColdFront.ZoomLevelSet.ZoomLevel05.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            featureLayers.Add(inMemoryFeatureLayerColdFront);
            // Add features which present the cold points.
            inMemoryFeatureLayerColdFront.InternalFeatures.Add(new Feature(File.ReadAllText(GetFullPath(@"App_Data/CustomStyles/ColdFront2.txt"))));
            inMemoryFeatureLayerColdFront.InternalFeatures.Add(new Feature(File.ReadAllText(GetFullPath(@"App_Data/CustomStyles/ColdFront3.txt"))));

            InMemoryFeatureLayer inMemoryFeatureLayerWarmFront = new InMemoryFeatureLayer();
            // Warm Front Icon Style.
            CustomGeoImageLineStyle warmFrontLineStyle = new CustomGeoImageLineStyle(lineStyle, new GeoImage(GetFullPath(@"App_Data/CustomStyles/offset_circle_blue.png")), 30, ImageDirection.Right);
            inMemoryFeatureLayerWarmFront.ZoomLevelSet.ZoomLevel05.CustomStyles.Add(warmFrontLineStyle);
            inMemoryFeatureLayerWarmFront.ZoomLevelSet.ZoomLevel05.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            featureLayers.Add(inMemoryFeatureLayerWarmFront);
            // Add features which present the warm points.
            LineShape lineShape5 = new LineShape(File.ReadAllText(GetFullPath(@"App_Data/CustomStyles/WarmFront5.txt")).Trim());
            inMemoryFeatureLayerWarmFront.InternalFeatures.Add(new Feature(lineShape5));
            LineShape lineShape6 = new LineShape(File.ReadAllText(GetFullPath(@"App_Data/CustomStyles/WarmFront6.txt")).Trim());
            inMemoryFeatureLayerWarmFront.InternalFeatures.Add(new Feature(lineShape6));
            LineShape lineShape7 = new LineShape(File.ReadAllText(GetFullPath(@"App_Data/CustomStyles/WarmFront7.txt")).Trim());
            inMemoryFeatureLayerWarmFront.InternalFeatures.Add(new Feature(lineShape7));
            LineShape lineShape8 = new LineShape(File.ReadAllText(GetFullPath(@"App_Data/CustomStyles/WarmFront8.txt")).Trim());
            inMemoryFeatureLayerWarmFront.InternalFeatures.Add(new Feature(lineShape8));

            // Occluded Front Icon Style.
            InMemoryFeatureLayer inMemoryFeatureLayerOccludedFront = new InMemoryFeatureLayer();
            CustomGeoImageLineStyle occludedFrontLineStyle = new CustomGeoImageLineStyle(lineStyle, new[]
                {
                    new GeoImage(GetFullPath(@"App_Data/CustomStyles/offset_triangle_and_circle_blue.png")),
                    new GeoImage(GetFullPath(@"App_Data/CustomStyles/offset_triangle_and_circle_red.png"))
                }, 45, ImageDirection.Right);
            inMemoryFeatureLayerOccludedFront.ZoomLevelSet.ZoomLevel05.CustomStyles.Add(occludedFrontLineStyle);
            inMemoryFeatureLayerOccludedFront.ZoomLevelSet.ZoomLevel05.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            featureLayers.Add(inMemoryFeatureLayerOccludedFront);
            // Add features which present the occluded points.
            LineShape lineShape9 = new LineShape(File.ReadAllText(GetFullPath(@"App_Data/CustomStyles/OccludedFront9.txt")).Trim());
            inMemoryFeatureLayerOccludedFront.InternalFeatures.Add(new Feature(lineShape9));
            LineShape lineShape10 = new LineShape(File.ReadAllText(GetFullPath(@"App_Data/CustomStyles/OccludedFront10.txt")).Trim());
            inMemoryFeatureLayerOccludedFront.InternalFeatures.Add(new Feature(lineShape10));
            LineShape lineShape11 = new LineShape(File.ReadAllText(GetFullPath(@"App_Data/CustomStyles/OccludedFront11.txt")).Trim());
            inMemoryFeatureLayerOccludedFront.InternalFeatures.Add(new Feature(lineShape11));

            // Create the style for "Pressure" values.
            PressureValueStyle pressureValueStyle = new PressureValueStyle();
            pressureValueStyle.ColumnName = "Pressure";

            InMemoryFeatureLayer pressureFeatureLayer = new InMemoryFeatureLayer();
            pressureFeatureLayer.Open();
            pressureFeatureLayer.Columns.Add(new FeatureSourceColumn("Pressure"));
            pressureFeatureLayer.ZoomLevelSet.ZoomLevel05.CustomStyles.Add(pressureValueStyle);
            pressureFeatureLayer.ZoomLevelSet.ZoomLevel05.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            featureLayers.Add(pressureFeatureLayer);

            // Add features which present the "Pressure" points.
            string[] pressurePointStr = File.ReadAllLines(GetFullPath(@"App_Data/Pressure.txt"));
            foreach (string pointStr in pressurePointStr)
            {
                string[] parts = pointStr.Split(',');
                Feature pressurePoint = new Feature(double.Parse(parts[0]), double.Parse(parts[1]));
                pressurePoint.ColumnValues["Pressure"] = parts[2];
                pressureFeatureLayer.InternalFeatures.Add(pressurePoint);
            }

            // Create the style for "wind".
            ClassBreakStyle windClassBreakStyle = new ClassBreakStyle("TEXT");
            WindPointStyle windStyle1 = new WindPointStyle("TEXT", "LEVEL", "ANGLE", GeoColor.FromHtml("#0AF8F8"));
            windClassBreakStyle.ClassBreaks.Add(new ClassBreak(50, new Collection<Style> { windStyle1 }));
            WindPointStyle windStyle2 = new WindPointStyle("TEXT", "LEVEL", "ANGLE", GeoColor.FromHtml("#0FF5B0"));
            windClassBreakStyle.ClassBreaks.Add(new ClassBreak(60, new Collection<Style> { windStyle2 }));
            WindPointStyle windStyle3 = new WindPointStyle("TEXT", "LEVEL", "ANGLE", GeoColor.FromHtml("#F7F70D"));
            windClassBreakStyle.ClassBreaks.Add(new ClassBreak(70, new Collection<Style> { windStyle3 }));
            WindPointStyle windStyle4 = new WindPointStyle("TEXT", "LEVEL", "ANGLE", GeoColor.FromHtml("#FBE306"));
            windClassBreakStyle.ClassBreaks.Add(new ClassBreak(80, new Collection<Style> { windStyle4 }));

            InMemoryFeatureLayer windFeatureLayer = new InMemoryFeatureLayer();
            windFeatureLayer.Open();
            windFeatureLayer.Columns.Add(new FeatureSourceColumn("TEXT"));
            windFeatureLayer.Columns.Add(new FeatureSourceColumn("ANGLE"));
            windFeatureLayer.Columns.Add(new FeatureSourceColumn("LEVEL"));
            windFeatureLayer.ZoomLevelSet.ZoomLevel05.CustomStyles.Add(windClassBreakStyle);
            windFeatureLayer.ZoomLevelSet.ZoomLevel05.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            featureLayers.Add(windFeatureLayer);

            // Add features which present the "wind" points.
            Random random = new Random();
            for (int i = 0; i < 8; i++)
            {
                Feature windFeature = new Feature(random.Next(-13886070, -8906057), random.Next(3382985, 6660597));
                windFeature.ColumnValues["TEXT"] = random.Next(55, 99).ToString();
                windFeature.ColumnValues["ANGLE"] = random.Next(0, 360).ToString();
                windFeature.ColumnValues["LEVEL"] = random.Next(0, 5).ToString();
                windFeatureLayer.InternalFeatures.Add(windFeature);
            }

            return featureLayers;
        }

        private static DynamicIsoLineLayer GetDynamicIsoLineLayer()
        {
            // Define the colors for different earthquake magnitude.
            Collection<GeoColor> colorsOfMagnitude = new Collection<GeoColor>() {
                GeoColor.FromHtml("#FFFFBE"),
                GeoColor.FromHtml("#FDFF9E"),
                GeoColor.FromHtml("#FDFF37"),
                GeoColor.FromHtml("#FDDA04"),
                GeoColor.FromHtml("#FFA701"),
                GeoColor.FromHtml("#FF6F02"),
                GeoColor.FromHtml("#EC0000"),
                GeoColor.FromHtml("#B90000"),
                GeoColor.FromHtml("#850100"),
                GeoColor.FromHtml("#620001"),
                GeoColor.FromHtml("#450005"),
                GeoColor.FromHtml("#2B0804")
            };

            // Get the file path name from its relative path.
			string shpFilePathName = GetFullPath("App_Data/usEarthquake.shp");

            // Create the layer for IsoLine.
            ShapeFileFeatureSource featureSource = new ShapeFileFeatureSource(shpFilePathName);
            Dictionary<PointShape, double> dataPoints = GetDataPoints(featureSource);
            GridInterpolationModel interpolationModel = new InverseDistanceWeightedGridInterpolationModel(3, double.MaxValue);
            DynamicIsoLineLayer isoLineLayer = new DynamicIsoLineLayer(dataPoints, GetClassBreakValues(dataPoints.Values, 12), interpolationModel, IsoLineType.ClosedLinesAsPolygons);

            // Create the style for different level of earthquake magnitude.
            ClassBreakStyle earthquakeMagnitudeBreakStyle = new ClassBreakStyle(isoLineLayer.DataValueColumnName);
            earthquakeMagnitudeBreakStyle.ClassBreaks.Add(new ClassBreak(double.MinValue, new AreaStyle(new GeoPen(GeoColor.FromHtml("#FE6B06"), 1), new GeoSolidBrush(new GeoColor(100, colorsOfMagnitude[0])))));
            for (int i = 0; i < isoLineLayer.IsoLineLevels.Count - 1; i++)
            {
                earthquakeMagnitudeBreakStyle.ClassBreaks.Add(new ClassBreak(isoLineLayer.IsoLineLevels[i + 1], new AreaStyle(new GeoPen(GeoColor.FromHtml("#FE6B06"), 1), new GeoSolidBrush(new GeoColor(100, colorsOfMagnitude[i + 1])))));
            }
            isoLineLayer.CustomStyles.Add(earthquakeMagnitudeBreakStyle);

            TextStyle textStyle = TextStyles.CreateSimpleTextStyle(isoLineLayer.DataValueColumnName, "Arial", 8, DrawingFontStyles.Bold, GeoColor.StandardColors.Black, 0, 0);
            textStyle.HaloPen = new GeoPen(GeoColor.StandardColors.White, 2);
            textStyle.OverlappingRule = LabelOverlappingRule.NoOverlapping;
            textStyle.SplineType = SplineType.StandardSplining;
            textStyle.DuplicateRule = LabelDuplicateRule.UnlimitedDuplicateLabels;
            textStyle.TextLineSegmentRatio = 9999999;
            textStyle.FittingLineInScreen = true;
            textStyle.SuppressPartialLabels = true;
            textStyle.NumericFormat = "{0:0.00}";
            isoLineLayer.CustomStyles.Add(textStyle);

            return isoLineLayer;
        }

        private static Dictionary<PointShape, double> GetDataPoints(FeatureSource featureSource)
        {
            featureSource.Open();
            Dictionary<PointShape, double> features = (from feature in featureSource.GetAllFeatures(new string[] { "LONGITUDE", "LATITIUDE", "MAGNITUDE" })
                                                       where double.Parse(feature.ColumnValues["MAGNITUDE"]) > 0
                                                       select new PointShape
                                                       {
                                                           X = double.Parse(feature.ColumnValues["LONGITUDE"], CultureInfo.InvariantCulture),
                                                           Y = double.Parse(feature.ColumnValues["LATITIUDE"], CultureInfo.InvariantCulture),
                                                           Z = double.Parse(feature.ColumnValues["MAGNITUDE"], CultureInfo.InvariantCulture)
                                                       }).ToDictionary(point => point, point => point.Z);

            return features;
        }

        private static IEnumerable<double> GetClassBreakValues(IEnumerable<double> values, int count)
        {
            // Take an average value for creating class breaks.
            Collection<double> result = new Collection<double>();
            double[] sortedValues = values.OrderBy(v => v).ToArray();
            int classCount = sortedValues.Length / count;
            for (int i = 1; i < count; i++)
            {
                result.Add(sortedValues[i * classCount]);
            }

            return result;
        }

        private static ValueStyle GetIconStyle()
        {
            // Get the file path name from its relative path.
            string imagePath = GetFullPath(@"Images/vehicle");

            ValueStyle valueStyle = new ValueStyle() { ColumnName = "TYPE" };
            // Create the icon style for different type of vehicles.
            for (int i = 1; i <= 7; i++)
            {
                IconStyle iconStyle = new IconStyle(string.Format("{0}{1}.png", imagePath, i.ToString(CultureInfo.InvariantCulture), ".png"), "Type", new GeoFont("Arial", 12, DrawingFontStyles.Bold), new GeoSolidBrush(GeoColor.StandardColors.Black));
                iconStyle.HaloPen = new GeoPen(GeoColor.SimpleColors.White);
                ValueItem valueItem = new ValueItem(i.ToString(CultureInfo.InvariantCulture), iconStyle);

                valueStyle.ValueItems.Add(valueItem);
            }

            return valueStyle;
        }

        private static Proj4Projection GetProjection()
        {
            Proj4Projection Proj4Projection = new Proj4Projection();
            Proj4Projection.InternalProjectionParametersString = Proj4Projection.GetWgs84ParametersString();
            Proj4Projection.ExternalProjectionParametersString = Proj4Projection.GetSphericalMercatorParametersString();

            return Proj4Projection;
        }

        private static string GetFullPath(string relativePath)
        {
            Uri uri = new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
            string folderPath = Path.GetDirectoryName(Path.GetDirectoryName(uri.LocalPath));
            return Path.Combine(folderPath, relativePath);
        }
    }
}