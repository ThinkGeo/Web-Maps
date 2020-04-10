using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using ThinkGeo.Core;

namespace BasicStyling.Controllers
{
    public static class LayerBuilder
    {
        private static readonly string baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "App_Data");

        /// <summary>
        /// Gets area style options by style id and access id.
        /// </summary>
        /// <param name="styleId">style id</param>
        /// <param name="accessId">access id</param>
        /// <returns></returns>
        internal static Dictionary<string, object> GetAreaLayerStyle(string styleId, string accessId)
        {
            // Get the area layer that carries an area style by style id and access id.
            FeatureLayer featureLayer = (FeatureLayer)GetAreaStyleLayer(styleId, accessId);

            // Get the option values of the area style.
            Dictionary<string, object> styles = new Dictionary<string, object>();
            GeoColor fillSolidBrushColor = (featureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush as GeoSolidBrush).Color;
            styles["FillSolidBrushColor"] = GeoColor.ToHtml(fillSolidBrushColor);
            styles["FillSolidBrushAalpha"] = fillSolidBrushColor.AlphaComponent;
            GeoColor outlinePenColor = featureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color;
            styles["OutlinePenColor"] = GeoColor.ToHtml(outlinePenColor);
            styles["OutlinePenAalpha"] = outlinePenColor.AlphaComponent;
            styles["OutlinePenWidth"] = featureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Width;
            return styles;
        }

        /// <summary>
        /// Gets line style options by style id and access id.
        /// </summary>
        /// <param name="styleId">style id</param>
        /// <param name="accessId">access id</param>
        /// <returns></returns>
        internal static Dictionary<string, object> GetLineLayerStyle(string styleId, string accessId)
        {
            // Get the line layer that carries a line style by style id and access id.
            FeatureLayer featureLayer = (FeatureLayer)GetLineStyleLayer(styleId, accessId);

            // Get the option values of the line style.
            Dictionary<string, object> styles = new Dictionary<string, object>();
            GeoPen centerPen = featureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.CenterPen;
            styles["CenterPenColor"] = GeoColor.ToHtml(centerPen.Color);
            styles["CenterPenAlpha"] = centerPen.Color.AlphaComponent;
            styles["CenterPenWidth"] = centerPen.Width;

            GeoPen innerPen = featureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen;
            styles["InnerPenColor"] = GeoColor.ToHtml(innerPen.Color);
            styles["InnerPenAlpha"] = innerPen.Color.AlphaComponent;
            styles["InnerPenWidth"] = innerPen.Width;

            GeoPen outerPen = featureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.OuterPen;
            styles["OuterPenColor"] = GeoColor.ToHtml(outerPen.Color);
            styles["OuterPenAlpha"] = outerPen.Color.AlphaComponent;
            styles["OuterPenWidth"] = outerPen.Width;
            return styles;
        }

        /// <summary>
        /// Gets symbol point style  options by style id and access id.
        /// </summary>
        /// <param name="styleId">style id</param>
        /// <param name="accessId">access id</param>
        /// <returns></returns>
        internal static Dictionary<string, object> GetSymbolPointLayerStyle(string styleId, string accessId)
        {
            // Get the point layer that carries a symbol point style by style id and access id.
            FeatureLayer featureLayer = (FeatureLayer)GetSymbolPointLayer(styleId, accessId);

            // Get the option values of the symbol point style.
            Dictionary<string, object> styles = new Dictionary<string, object>();
            PointStyle targetPointStyle = featureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle;
            styles["SymbolPointType"] = targetPointStyle.SymbolType;
            styles["SymbolPointSize"] = targetPointStyle.SymbolSize;
            styles["symbolPointRotationAngle"] = targetPointStyle.RotationAngle;

            //GeoPen symbolPen = targetPointStyle.SymbolPen;
            GeoPen symbolPen = targetPointStyle.OutlinePen;
            styles["SymbolPenColor"] = GeoColor.ToHtml(symbolPen.Color);
            styles["SymbolPointPenAlpha"] = symbolPen.Color.AlphaComponent;
            styles["SymbolPointPenWidth"] = symbolPen.Width;

            //GeoColor symbolSolidBrushColor = targetPointStyle.SymbolSolidBrush.Color;
            GeoColor symbolSolidBrushColor = (targetPointStyle.FillBrush as GeoSolidBrush).Color;
            styles["SymbolSolidBrushColor"] = GeoColor.ToHtml(symbolSolidBrushColor);
            styles["SymbolSolidBrushAlpha"] = symbolSolidBrushColor.AlphaComponent;
            return styles;
        }

        /// <summary>
        /// Update style options to tempoary folder for a specific acess id and style id.
        /// </summary>
        /// <param name="styleId">style id</param>
        /// <param name="accessId">access id</param>
        /// <param name="styles">style options</param>
        internal static void UpdateLayerStyle(string styleId, string accessId, Dictionary<string, string> styles)
        {
            string styleFile = string.Format("{0}_{1}.json", accessId, styleId);
            string styleFilePath = Path.Combine(baseDirectory, "Temp", styleFile);

            using (StreamWriter streamWriter = new StreamWriter(styleFilePath, false))
            {
                streamWriter.WriteLine(JsonConvert.SerializeObject(styles));
            }
        }

        /// <summary>
        /// Gets layers for exhibition predefined style.
        /// </summary>
        /// <returns></returns>
        internal static Collection<Layer> GetPredefinedStyleLayers()
        {
            Collection<Layer> layers = new Collection<Layer>();
            BackgroundLayer backgroundLayer = new BackgroundLayer(new GeoSolidBrush(new GeoColor(255, 160, 207, 235)));
            layers.Add(backgroundLayer);

            ShapeFileFeatureLayer countriesLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "Countries.shp"));
            countriesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoColors.Gray), new GeoSolidBrush(new GeoColor(255, 250, 247, 243)));
            countriesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layers.Add(countriesLayer);

            ShapeFileFeatureLayer lakeLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "lake.shp"));
            lakeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoColors.Transparent, 0), new GeoSolidBrush(new GeoColor(255, 160, 207, 235)));
            lakeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layers.Add(lakeLayer);

            ShapeFileFeatureLayer usStatesLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "USStates.shp"));
            usStatesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoPen(GeoColors.Transparent), new GeoSolidBrush(new GeoColor(255, 250, 247, 243)));
            usStatesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layers.Add(usStatesLayer);

            ShapeFileFeatureLayer highwayNetworkShapeLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "USHighwayNetwork.shp"));
            highwayNetworkShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = new LineStyle(new GeoPen(new GeoColor(255, 255, 222, 190), 1) { StartCap = DrawingLineCap.Round, EndCap = DrawingLineCap.Round });
            highwayNetworkShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layers.Add(highwayNetworkShapeLayer);

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
            layers.Add(majorCitiesLayer);

            return layers;
        }

        /// <summary>
        /// Gets layer for exhibition area style by style id and access id.
        /// </summary>
        /// <param name="styleId">style id</param>
        /// <param name="accessId">access id</param>
        /// <returns></returns>
        internal static Layer GetAreaStyleLayer(string styleId, string accessId)
        {
            ShapeFileFeatureLayer areaShapeLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "ThinkGeoArea.shp"));
            areaShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            areaShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle()
            {
                FillBrush = new GeoSolidBrush(new GeoColor(200, GeoColors.LightOrange)),
                OutlinePen = new GeoPen(new GeoSolidBrush(new GeoColor(200, GeoColors.LightBlue)), 2)
            };

            // Refresh styles if the user has saved styles.
            Dictionary<string, string> savedStyle = GetSavedStyleByAccessId(styleId, accessId);
            if (savedStyle != null) UpdateAreaStyle(areaShapeLayer, savedStyle);

            return areaShapeLayer;
        }

        /// <summary>
        /// Gets layer for exhibition line style by style id and access id.
        /// </summary>
        /// <param name="styleId">style id</param>
        /// <param name="accessId">access id</param>
        /// <returns></returns>
        internal static Layer GetLineStyleLayer(string styleId, string accessId)
        {
            ShapeFileFeatureLayer lineShapeLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "MajorRoadsInFrisco.shp"));
            lineShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            lineShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = new LineStyle()
            {
                CenterPen = new GeoPen(new GeoColor(255, GeoColors.PastelGreen), 2),
                InnerPen = new GeoPen(new GeoColor(255, GeoColors.Black), 5),
                OuterPen = new GeoPen(new GeoColor(255, GeoColors.YellowGreen), 10)
            };


            // Refresh styles if the user has saved styles.
            Dictionary<string, string> savedStyle = GetSavedStyleByAccessId(styleId, accessId);
            if (savedStyle != null) UpdateLineStyle(lineShapeLayer, savedStyle);

            return lineShapeLayer;
        }

        /// <summary>
        /// Gets layer for exhibition image point style.
        /// </summary>
        /// <returns></returns>
        internal static Layer GeImagePointStyleLayer()
        {
            ShapeFileFeatureLayer ImagePointStyleLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "HighSchoolInFrisco.shp"));
            ImagePointStyleLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            ImagePointStyleLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle()
            {
                Image = new GeoImage(Path.Combine(baseDirectory, "school.png")),
                PointType = PointType.Image,
                RotationAngle = 0
            };


            return ImagePointStyleLayer;
        }

        /// <summary>
        /// Gets layer for exhibition symbol point style by style id and access id.
        /// </summary>
        /// <param name="styleId">style id</param>
        /// <param name="accessId">access id</param>
        /// <returns></returns>
        internal static Layer GetSymbolPointLayer(string styleId, string accessId)
        {
            ShapeFileFeatureLayer symblePointLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "ThinkGeoLocation.shp"));
            var colors = GeoColor.GetColorsInHueFamily(GeoColors.Red, 10);

            symblePointLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyle.CreateSimplePointStyle(PointSymbolType.Triangle, colors[1], GeoColors.Blue, 1, 20);
            symblePointLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Refresh styles if the user has saved styles.
            Dictionary<string, string> savedStyle = GetSavedStyleByAccessId(styleId, accessId);
            if (savedStyle != null) UpdateSymbolPointStyle(symblePointLayer, savedStyle);

            return symblePointLayer;
        }

        /// <summary>
        /// Gets layer for exhibition character point style.
        /// </summary>
        /// <returns></returns>
        internal static Layer GetCharacterPointLayer()
        {
            ShapeFileFeatureLayer characterPointLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "HospitalInFrisco.shp"));
            characterPointLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            characterPointLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle()
            {
                PointType = PointType.Glyph,
                RotationAngle = 0,
                GlyphContent = GeoFont.GetGlyphContent(72),
                GlyphFont = new GeoFont("Arial", 20, DrawingFontStyles.Bold),
                FillBrush = new GeoSolidBrush(GeoColors.Blue)
            };

            return characterPointLayer;
        }

        /// <summary>
        /// Gets layer for exhibition multiple style.
        /// </summary>
        /// <returns></returns>
        internal static Collection<Layer> GetMultipleStyleLayers()
        {
            Collection<Layer> layers = new Collection<Layer>();

            ShapeFileFeatureLayer ZoomLevelAreaShapeLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "ThinkGeoArea.shp"));

            // Initialization four area styles  for different zoomlevel.
            ZoomLevelAreaShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level16;
            ZoomLevelAreaShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle()
            {
                FillBrush = new GeoSolidBrush(new GeoColor(100, GeoColors.BrightOrange)),
                OutlinePen = new GeoPen(new GeoColor(255, GeoColors.Black))
            };

            // ZoomLevel 17 Style
            ZoomLevelAreaShapeLayer.ZoomLevelSet.ZoomLevel17.DefaultAreaStyle = new AreaStyle()
            {
                FillBrush = new GeoSolidBrush(new GeoColor(100, GeoColors.Blue)),
                OutlinePen = new GeoPen(new GeoColor(255, 0, 0, 0))
            };

            // ZoomLevel 18 Style
            ZoomLevelAreaShapeLayer.ZoomLevelSet.ZoomLevel18.DefaultAreaStyle = new AreaStyle()
            {
                FillBrush = new GeoSolidBrush(new GeoColor(100, GeoColors.Red)),
                OutlinePen = new GeoPen(new GeoColor(255, 0, 0, 0))
            };

            // ZoomLevel 19 Style
            ZoomLevelAreaShapeLayer.ZoomLevelSet.ZoomLevel19.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            ZoomLevelAreaShapeLayer.ZoomLevelSet.ZoomLevel19.DefaultAreaStyle = new AreaStyle()
            {
                FillBrush = new GeoSolidBrush(new GeoColor(100, GeoColors.Green)),
                OutlinePen = new GeoPen(new GeoColor(255, 0, 0, 0))
            };
            layers.Add(ZoomLevelAreaShapeLayer);


            ShapeFileFeatureLayer zoomLevelLineLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "ThinkGeoLine.shp"));

            // ZoolLevel 1-16 style
            zoomLevelLineLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level16;
            zoomLevelLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = new LineStyle()
            {
                CenterPen = new GeoPen(new GeoColor(255, GeoColors.DarkOrange), 5)
            };

            // ZoolLevel 17 style
            zoomLevelLineLayer.ZoomLevelSet.ZoomLevel17.DefaultLineStyle = new LineStyle()
            {
                CenterPen = new GeoPen(new GeoColor(255, GeoColors.DarkBlue), 5)
            };

            // ZoolLevel 18 style
            zoomLevelLineLayer.ZoomLevelSet.ZoomLevel18.DefaultLineStyle = new LineStyle()
            {
                CenterPen = new GeoPen(new GeoColor(255, GeoColors.LightOrange), 5)
            };

            // ZoolLevel 19 style
            zoomLevelLineLayer.ZoomLevelSet.ZoomLevel19.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            zoomLevelLineLayer.ZoomLevelSet.ZoomLevel19.DefaultLineStyle = new LineStyle()
            {
                CenterPen = new GeoPen(new GeoColor(255, GeoColors.LightGreen), 5)
            };
            layers.Add(zoomLevelLineLayer);


            ShapeFileFeatureLayer ZoomLevelPointLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "ThinkGeoLocation.shp"));
            ZoomLevelPointLayer.Transparency = 200f;

            // Initialization four point styles  for different zoomlevel.
            ZoomLevelPointLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level16;
            ZoomLevelPointLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle()
            {
                Image = new GeoImage(Path.Combine(baseDirectory, "ThinkGeoLogo.png")),
                PointType = PointType.Image,
                RotationAngle = 0
            };

            // ZoomLevel 17 Style
            ZoomLevelPointLayer.ZoomLevelSet.ZoomLevel17.DefaultPointStyle = new PointStyle()
            {
                OutlinePen = new GeoPen(GeoColor.FromArgb(255, GeoColors.DarkBlue), 8)
            };

            // ZoomLevel 18 Style
            TextStyle textStyleForZoomLevel18 = new TextStyle("name", (new GeoFont("Arial", 12)), new GeoSolidBrush(GeoColors.LightRed))
            {
                HaloPen = new GeoPen(GeoColors.FloralWhite, 5),
                SplineType = SplineType.ForceSplining
            };

            PointStyle pointStyleForZoomLevel18 = new PointStyle()
            {
                OutlinePen = new GeoPen(GeoColor.FromArgb(255, GeoColors.LightRed), 8)
            };

            ZoomLevelPointLayer.ZoomLevelSet.ZoomLevel18.CustomStyles.Add(textStyleForZoomLevel18);
            ZoomLevelPointLayer.ZoomLevelSet.ZoomLevel18.CustomStyles.Add(pointStyleForZoomLevel18);

            // ZoomLevel 19 Style
            ZoomLevelPointLayer.ZoomLevelSet.ZoomLevel19.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            ZoomLevelPointLayer.ZoomLevelSet.ZoomLevel19.DefaultPointStyle = new PointStyle()
            {
                OutlinePen = new GeoPen(GeoColor.FromArgb(255, GeoColors.DarkGreen), 8)
            };

            layers.Add(ZoomLevelPointLayer);

            return layers;
        }

        /// <summary>
        /// Gets layer for exhibition compound style.
        /// </summary>
        /// <returns></returns>
        internal static Layer GetCompoundStyleLayer()
        {
            ShapeFileFeatureLayer compoundStyleLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "ThinkGeoLocation.shp"));

            // Initialization two point styles for custom style.
            PointStyle circleStyle = new PointStyle()
            {
                PointType = PointType.Symbol,
                OutlinePen = new GeoPen(new GeoColor(255, GeoColors.Red), 3),
                SymbolType = PointSymbolType.Circle,
                SymbolSize = 25
            };

            PointStyle starStyle = new PointStyle()
            {
                PointType = PointType.Symbol,
                FillBrush = new GeoSolidBrush(new GeoColor(255, GeoColors.Blue)),
                SymbolType = PointSymbolType.Star,
                SymbolSize = 20,
                XOffsetInPixel = 1,
                YOffsetInPixel = 3
            };

            compoundStyleLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(circleStyle);
            compoundStyleLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(starStyle);
            compoundStyleLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            return compoundStyleLayer;
        }

        /// <summary>
        /// Gets style options by overlay id and access id.
        /// </summary>
        /// <param name="overlayId">overlay id</param>
        /// <param name="accessId">access id</param>
        /// <returns></returns>
        private static Dictionary<string, string> GetSavedStyleByAccessId(string overlayId, string accessId)
        {
            string styleFile = string.Format("{0}_{1}.json", accessId, overlayId);
            string styleFilePath = Path.Combine(baseDirectory, "Temp", styleFile);

            if (File.Exists(styleFilePath))
            {
                string content = File.ReadAllText(styleFilePath);
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
            }

            return null;
        }

        /// <summary>
        /// Updates FeatureLayer's area style.
        /// </summary>
        /// <param name="featureLayer">target FeatureLayer</param>
        /// <param name="savedStyle">new area style options</param>
        private static void UpdateAreaStyle(FeatureLayer featureLayer, Dictionary<string, string> savedStyle)
        {
            string fillSolidBrushStr = savedStyle["fillSolidBrushColor"];
            byte fillSolidBrushAlpha = byte.Parse(savedStyle["fillSolidBrushAlpha"]);
            string outerPenStr = savedStyle["outerPenColor"];
            byte outerPenAlpha = byte.Parse(savedStyle["outerPenAlpha"]);
            int outerPenWidth = int.Parse(savedStyle["outerPenWidth"]);

            AreaStyle areaStyle = new AreaStyle();
            areaStyle.FillBrush = new GeoSolidBrush(new GeoColor(fillSolidBrushAlpha, GeoColor.FromHtml(fillSolidBrushStr)));
            areaStyle.OutlinePen = new GeoPen(new GeoColor(outerPenAlpha, GeoColor.FromHtml(outerPenStr)), outerPenWidth);
            featureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = areaStyle;
            featureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        }

        /// <summary>
        /// Updates FeatureLayer's line style.
        /// </summary>
        /// <param name="featureLayer">target FeatureLayer</param>
        /// <param name="savedStyle">new line style options</param>
        private static void UpdateLineStyle(FeatureLayer featureLayer, Dictionary<string, string> savedStyle)
        {
            string centerPenColor = savedStyle["centerPenColor"];
            byte centerPenColorAlpha = byte.Parse(savedStyle["centerPenColorAlpha"]);
            int centerPenWidth = int.Parse(savedStyle["centerPenWidth"]);

            string outterPenColor = savedStyle["outterPenColor"];
            byte outterPenColorAlpha = byte.Parse(savedStyle["outterPenColorAlpha"]);
            int outterPenWidth = int.Parse(savedStyle["outterPenWidth"]);

            string innerPenColor = savedStyle["innerPenColor"];
            byte innerPenColorAlpha = byte.Parse(savedStyle["innerPenColorAlpha"]);
            int innerPenWidth = int.Parse(savedStyle["innerPenWidth"]);

            LineStyle lineStyle = new LineStyle();
            lineStyle.CenterPen = new GeoPen(new GeoColor(centerPenColorAlpha, GeoColor.FromHtml(centerPenColor)), centerPenWidth);
            lineStyle.OuterPen = new GeoPen(new GeoColor(outterPenColorAlpha, GeoColor.FromHtml(outterPenColor)), outterPenWidth);
            lineStyle.InnerPen = new GeoPen(new GeoColor(innerPenColorAlpha, GeoColor.FromHtml(innerPenColor)), innerPenWidth);

            featureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = lineStyle;
            featureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        }

        /// <summary>
        /// Updates FeatureLayer's symbol point style.
        /// </summary>
        /// <param name="featureLayer">target FeatureLayer</param>
        /// <param name="savedStyle">new symbol point style options</param>
        private static void UpdateSymbolPointStyle(FeatureLayer featureLayer, Dictionary<string, string> savedStyle)
        {


            string symbolPointType = savedStyle["symbolPointType"];
            int symbolPointSize = int.Parse(savedStyle["symbolPointSize"]);
            float symbolPointRotationAngle = float.Parse(savedStyle["symbolPointRotationAngle"]);

            string symbolPointPenColor = savedStyle["symbolPointPenColor"];
            byte symbolPointPenAlpha = byte.Parse(savedStyle["symbolPointPenAlpha"]);
            int symbolPointPenWidth = int.Parse(savedStyle["symbolPointPenWidth"]);

            string symbolPointSolidBrushColor = savedStyle["symbolPointSolidBrushColor"];
            byte symbolPointSolidBrushAlpha = byte.Parse(savedStyle["symbolPointSolidBrushAlpha"]);


            featureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            featureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle()
            {
                PointType = PointType.Symbol,
                SymbolType = (PointSymbolType)Enum.Parse(typeof(PointSymbolType), symbolPointType, true),
                SymbolSize = symbolPointSize,
                RotationAngle = symbolPointRotationAngle,
                OutlinePen = new GeoPen(new GeoColor(symbolPointPenAlpha, GeoColor.FromHtml(symbolPointPenColor)), symbolPointPenWidth),
                FillBrush = new GeoSolidBrush(new GeoColor(symbolPointSolidBrushAlpha, GeoColor.FromHtml(symbolPointSolidBrushColor)))
            };
        }
    }
}