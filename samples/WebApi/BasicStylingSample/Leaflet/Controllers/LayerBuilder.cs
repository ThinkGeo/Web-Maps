using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Styles;

namespace BasicStyling.Controllers
{
    public static class LayerBuilder
    {
        private static readonly string baseDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");

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
            GeoColor fillSolidBrushColor = featureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color;
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

            GeoPen symbolPen = targetPointStyle.SymbolPen;
            styles["SymbolPenColor"] = GeoColor.ToHtml(symbolPen.Color);
            styles["SymbolPointPenAlpha"] = symbolPen.Color.AlphaComponent;
            styles["SymbolPointPenWidth"] = symbolPen.Width;

            GeoColor symbolSolidBrushColor = targetPointStyle.SymbolSolidBrush.Color;
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
            BackgroundLayer backgroundLayer = new BackgroundLayer(WorldStreetsAreaStyles.Water().FillSolidBrush);
            layers.Add(backgroundLayer);

            ShapeFileFeatureLayer countriesLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "Countries.shp"));
            countriesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = WorldStreetsAreaStyles.BaseLand();
            countriesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColors.Gray);
            countriesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layers.Add(countriesLayer);

            ShapeFileFeatureLayer lakeLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "lake.shp"));
            lakeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = WorldStreetsAreaStyles.Water();
            lakeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layers.Add(lakeLayer);

            ShapeFileFeatureLayer usStatesLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "USStates.shp"));
            usStatesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = WorldStreetsAreaStyles.BaseLand();
            usStatesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layers.Add(usStatesLayer);

            ShapeFileFeatureLayer highwayNetworkShapeLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "USHighwayNetwork.shp"));
            highwayNetworkShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = WorldStreetsLineStyles.Highway(1);
            highwayNetworkShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layers.Add(highwayNetworkShapeLayer);

            ShapeFileFeatureLayer majorCitiesLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "USMajorCities.shp"));
            majorCitiesLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = WorldStreetsTextStyles.Poi("AREANAME", 7, 7);
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
            AreaStyle areaStyle = new AreaStyle();
            areaStyle.FillSolidBrush = new GeoSolidBrush(new GeoColor(200, GeoColor.SimpleColors.LightOrange));
            areaStyle.OutlinePen = new GeoPen(new GeoSolidBrush(new GeoColor(200, GeoColor.SimpleColors.LightBlue)), 2);
            areaShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = areaStyle;
            areaShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

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
            LineStyle lineStyle = new LineStyle();
            lineStyle.CenterPen = new GeoPen(new GeoColor(255, GeoColor.SimpleColors.PastelGreen), 2);
            lineStyle.InnerPen = new GeoPen(new GeoColor(255, GeoColors.Black), 5);
            lineStyle.OuterPen = new GeoPen(new GeoColor(255, GeoColors.YellowGreen), 10);
            lineShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = lineStyle;
            lineShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

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
            PointStyle pointOfHighSchoolStyle = new PointStyle();
            pointOfHighSchoolStyle.Image = new GeoImage(Path.Combine(baseDirectory, "school.png"));
            pointOfHighSchoolStyle.PointType = PointType.Bitmap;
            pointOfHighSchoolStyle.RotationAngle = 0;

            ImagePointStyleLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = pointOfHighSchoolStyle;
            ImagePointStyleLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

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
            var colors = GeoColor.GetColorsInHueFamily(GeoColor.SimpleColors.Red, 10);

            symblePointLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimplePointStyle(PointSymbolType.Triangle, colors[1], GeoColor.SimpleColors.Blue, 1, 20);
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

            PointStyle hospitalPointStyle = new PointStyle();
            hospitalPointStyle.PointType = PointType.Character;
            hospitalPointStyle.RotationAngle = 0;
            hospitalPointStyle.CharacterIndex = 72;
            GeoFont geoFont = new GeoFont("Arial", 20, DrawingFontStyles.Bold);
            hospitalPointStyle.CharacterFont = geoFont;
            hospitalPointStyle.CharacterSolidBrush = new GeoSolidBrush(GeoColors.Blue);

            characterPointLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = hospitalPointStyle;
            characterPointLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

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
            AreaStyle ZoomLevelAreaStyleFrom01To16 = new AreaStyle();
            ZoomLevelAreaStyleFrom01To16.FillSolidBrush = new GeoSolidBrush(new GeoColor(100, GeoColor.SimpleColors.BrightOrange));
            ZoomLevelAreaStyleFrom01To16.OutlinePen = new GeoPen(new GeoColor(255, GeoColor.SimpleColors.Black));
            ZoomLevelAreaShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = ZoomLevelAreaStyleFrom01To16;
            ZoomLevelAreaShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level16;

            AreaStyle ZoomLevelAreaStyle17 = new AreaStyle();
            ZoomLevelAreaStyle17.FillSolidBrush = new GeoSolidBrush(new GeoColor(100, GeoColor.SimpleColors.Blue));
            ZoomLevelAreaStyle17.OutlinePen = new GeoPen(new GeoColor(255, 0, 0, 0));
            ZoomLevelAreaShapeLayer.ZoomLevelSet.ZoomLevel17.DefaultAreaStyle = ZoomLevelAreaStyle17;

            AreaStyle ZoomLevelAreaStyle18 = new AreaStyle();
            ZoomLevelAreaStyle18.FillSolidBrush = new GeoSolidBrush(new GeoColor(100, GeoColor.SimpleColors.Red));
            ZoomLevelAreaStyle18.OutlinePen = new GeoPen(new GeoColor(255, 0, 0, 0));
            ZoomLevelAreaShapeLayer.ZoomLevelSet.ZoomLevel18.DefaultAreaStyle = ZoomLevelAreaStyle18;

            AreaStyle ZoomLevelAreaStyle19 = new AreaStyle();
            ZoomLevelAreaStyle19.FillSolidBrush = new GeoSolidBrush(new GeoColor(100, GeoColor.SimpleColors.Green));
            ZoomLevelAreaStyle19.OutlinePen = new GeoPen(new GeoColor(255, 0, 0, 0));
            ZoomLevelAreaShapeLayer.ZoomLevelSet.ZoomLevel19.DefaultAreaStyle = ZoomLevelAreaStyle19;

            layers.Add(ZoomLevelAreaShapeLayer);

            ShapeFileFeatureLayer zoomLevelLineLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "ThinkGeoLine.shp"));

            // Initialization four line styles  for different zoomlevel.
            LineStyle zoomLevelLineStyleFrom01To16 = new LineStyle();
            zoomLevelLineStyleFrom01To16.CenterPen = new GeoPen(new GeoColor(255, GeoColor.SimpleColors.DarkOrange), 5);
            zoomLevelLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = zoomLevelLineStyleFrom01To16;
            zoomLevelLineLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level16;

            LineStyle zoomLevelLineStyle17 = new LineStyle();
            zoomLevelLineStyle17.CenterPen = new GeoPen(new GeoColor(255, GeoColor.SimpleColors.DarkBlue), 5);
            zoomLevelLineLayer.ZoomLevelSet.ZoomLevel17.DefaultLineStyle = zoomLevelLineStyle17;

            LineStyle zoomLevelLineStyle18 = new LineStyle();
            zoomLevelLineStyle18.CenterPen = new GeoPen(new GeoColor(255, GeoColor.SimpleColors.LightOrange), 5);
            zoomLevelLineLayer.ZoomLevelSet.ZoomLevel18.DefaultLineStyle = zoomLevelLineStyle18;

            LineStyle zoomLevelLineStyle19 = new LineStyle();
            zoomLevelLineStyle19.CenterPen = new GeoPen(new GeoColor(255, GeoColor.SimpleColors.LightGreen), 5);
            zoomLevelLineLayer.ZoomLevelSet.ZoomLevel19.DefaultLineStyle = zoomLevelLineStyle19;
            layers.Add(zoomLevelLineLayer);

            ShapeFileFeatureLayer ZoomLevelPointLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "ThinkGeoLocation.shp"));
            ZoomLevelPointLayer.Transparency = 200f;

            // Initialization four point styles  for different zoomlevel.
            PointStyle pointStyleForZoomLevelFrom01To16 = new PointStyle();
            pointStyleForZoomLevelFrom01To16.Image = new GeoImage(Path.Combine(baseDirectory, "ThinkGeoLogo.png"));
            pointStyleForZoomLevelFrom01To16.PointType = PointType.Bitmap;
            pointStyleForZoomLevelFrom01To16.RotationAngle = 0;
            ZoomLevelPointLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = pointStyleForZoomLevelFrom01To16;
            ZoomLevelPointLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level16;

            PointStyle pointStyleForZoomLevel17 = new PointStyle();
            pointStyleForZoomLevel17.SymbolPen = new GeoPen(GeoColor.FromArgb(255, GeoColor.SimpleColors.DarkBlue), 8);
            ZoomLevelPointLayer.ZoomLevelSet.ZoomLevel17.DefaultPointStyle = pointStyleForZoomLevel17;

            TextStyle textStyleForZoomLevel18 = new TextStyle("name", (new GeoFont("Arial", 12)), new GeoSolidBrush(GeoColor.SimpleColors.LightRed));
            textStyleForZoomLevel18.HaloPen = new GeoPen(GeoColor.StandardColors.FloralWhite, 5);
            textStyleForZoomLevel18.SplineType = SplineType.ForceSplining;
            PointStyle pointStyleForZoomLevel18 = new PointStyle();
            pointStyleForZoomLevel18.SymbolPen = new GeoPen(GeoColor.FromArgb(255, GeoColor.SimpleColors.LightRed), 8);
            ZoomLevelPointLayer.ZoomLevelSet.ZoomLevel18.CustomStyles.Add(textStyleForZoomLevel18);
            ZoomLevelPointLayer.ZoomLevelSet.ZoomLevel18.CustomStyles.Add(pointStyleForZoomLevel18);

            PointStyle pointStyleForZoomLevel19 = new PointStyle();
            pointStyleForZoomLevel19.SymbolPen = new GeoPen(GeoColor.FromArgb(255, GeoColor.SimpleColors.DarkGreen), 8);
            ZoomLevelPointLayer.ZoomLevelSet.ZoomLevel19.DefaultPointStyle = pointStyleForZoomLevel19;

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
            PointStyle circleStyle = new PointStyle();
            circleStyle.PointType = PointType.Symbol;
            circleStyle.SymbolPen = new GeoPen(new GeoColor(255, GeoColor.SimpleColors.Red), 3);
            circleStyle.SymbolType = PointSymbolType.Circle;
            circleStyle.SymbolSize = 25;

            PointStyle starStyle = new PointStyle();
            starStyle.PointType = PointType.Symbol;
            starStyle.SymbolSolidBrush = new GeoSolidBrush(new GeoColor(255, GeoColor.SimpleColors.Blue));
            starStyle.SymbolType = PointSymbolType.Star;
            starStyle.SymbolSize = 20;
            starStyle.XOffsetInPixel = 1;
            starStyle.YOffsetInPixel = 3;

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
            int fillSolidBrushAlpha = int.Parse(savedStyle["fillSolidBrushAlpha"]);
            string outerPenStr = savedStyle["outerPenColor"];
            int outerPenAlpha = int.Parse(savedStyle["outerPenAlpha"]);
            int outerPenWidth = int.Parse(savedStyle["outerPenWidth"]);

            AreaStyle areaStyle = new AreaStyle();
            areaStyle.FillSolidBrush = new GeoSolidBrush(new GeoColor(fillSolidBrushAlpha, GeoColor.FromHtml(fillSolidBrushStr)));
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
            int centerPenColorAlpha = int.Parse(savedStyle["centerPenColorAlpha"]);
            int centerPenWidth = int.Parse(savedStyle["centerPenWidth"]);

            string outterPenColor = savedStyle["outterPenColor"];
            int outterPenColorAlpha = int.Parse(savedStyle["outterPenColorAlpha"]);
            int outterPenWidth = int.Parse(savedStyle["outterPenWidth"]);

            string innerPenColor = savedStyle["innerPenColor"];
            int innerPenColorAlpha = int.Parse(savedStyle["innerPenColorAlpha"]);
            int innerPenWidth = int.Parse(savedStyle["innerPenWidth"]);
            GeoPen centerPen = new GeoPen(new GeoColor(centerPenColorAlpha, GeoColor.FromHtml(centerPenColor)), centerPenWidth);
            GeoPen outterPen = new GeoPen(new GeoColor(outterPenColorAlpha, GeoColor.FromHtml(outterPenColor)), outterPenWidth);
            GeoPen innerPen = new GeoPen(new GeoColor(innerPenColorAlpha, GeoColor.FromHtml(innerPenColor)), innerPenWidth);

            LineStyle lineStyle = new LineStyle();
            lineStyle.CenterPen = centerPen;
            lineStyle.OuterPen = outterPen;
            lineStyle.InnerPen = innerPen;

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
            int symbolPointPenAlpha = int.Parse(savedStyle["symbolPointPenAlpha"]);
            int symbolPointPenWidth = int.Parse(savedStyle["symbolPointPenWidth"]);

            string symbolPointSolidBrushColor = savedStyle["symbolPointSolidBrushColor"];
            int symbolPointSolidBrushAlpha = int.Parse(savedStyle["symbolPointSolidBrushAlpha"]);

            PointStyle thinkGeoLocationStyle = new PointStyle();
            thinkGeoLocationStyle.PointType = PointType.Symbol;
            thinkGeoLocationStyle.SymbolType = (PointSymbolType)Enum.Parse(typeof(PointSymbolType), symbolPointType, true);
            thinkGeoLocationStyle.SymbolSize = symbolPointSize;
            thinkGeoLocationStyle.RotationAngle = symbolPointRotationAngle;
            thinkGeoLocationStyle.SymbolPen = new GeoPen(new GeoColor(symbolPointPenAlpha, GeoColor.FromHtml(symbolPointPenColor)), symbolPointPenWidth);
            thinkGeoLocationStyle.SymbolSolidBrush = new GeoSolidBrush(new GeoColor(symbolPointSolidBrushAlpha, GeoColor.FromHtml(symbolPointSolidBrushColor)));

            featureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = thinkGeoLocationStyle;
            featureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        }
    }
}