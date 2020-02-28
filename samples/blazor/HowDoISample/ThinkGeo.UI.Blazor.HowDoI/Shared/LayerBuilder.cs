using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.IO;
using ThinkGeo.Core;

namespace ThinkGeo.UI.Blazor.HowDoI
{
    public static class LayerBuilder
    {
        private static readonly string baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data");

        /// <summary>
        /// Gets area style options by style id and access id.
        /// </summary>
        /// <param name="styleId">style id</param>
        /// <param name="accessId">access id</param>
        /// <returns></returns>
        internal static AreaStyleModel GetAreaLayerStyle(string styleId, string accessId)
        {
            // Get the area layer that carries an area style by style id and access id.
            FeatureLayer featureLayer = (FeatureLayer)GetAreaStyleLayer(styleId, accessId);

            // Get the option values of the area style.
            var styles = new AreaStyleModel();
            GeoColor fillSolidBrushColor = ((GeoSolidBrush)featureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillBrush).Color;
            styles.BrushColor = GeoColor.ToHtml(fillSolidBrushColor);
            styles.BrushAlpha = fillSolidBrushColor.AlphaComponent;
            GeoColor outlinePenColor = featureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color;
            styles.OutlinePenColor = GeoColor.ToHtml(outlinePenColor);
            styles.OutlinePenAlpha = outlinePenColor.AlphaComponent;
            styles.OutlinePenWidth = featureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Width;
            return styles;
        }

        /// <summary>
        /// Gets line style options by style id and access id.
        /// </summary>
        /// <param name="styleId">style id</param>
        /// <param name="accessId">access id</param>
        /// <returns></returns>
        internal static LineStyleModel GetLineLayerStyle(string styleId, string accessId)
        {
            // Get the line layer that carries a line style by style id and access id.
            FeatureLayer featureLayer = (FeatureLayer)GetLineStyleLayer(styleId, accessId);

            // Get the option values of the line style.
            var styles = new LineStyleModel();
            GeoPen centerPen = featureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.CenterPen;
            styles.CenterPenColor = GeoColor.ToHtml(centerPen.Color);
            styles.CenterPenAlpha = centerPen.Color.AlphaComponent;
            styles.CenterPenWidth = centerPen.Width;

            GeoPen innerPen = featureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen;
            styles.InnerPenColor = GeoColor.ToHtml(innerPen.Color);
            styles.InnerPenAlpha = innerPen.Color.AlphaComponent;
            styles.InnerPenWidth = innerPen.Width;

            GeoPen outerPen = featureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.OuterPen;
            styles.OuterPenColor = GeoColor.ToHtml(outerPen.Color);
            styles.OuterPenAlpha = outerPen.Color.AlphaComponent;
            styles.OuterPenWidth = outerPen.Width;
            return styles;
        }

        /// <summary>
        /// Gets symbol point style  options by style id and access id.
        /// </summary>
        /// <param name="styleId">style id</param>
        /// <param name="accessId">access id</param>
        /// <returns></returns>
        internal static SymbolPointStyleModel GetSymbolPointLayerStyle(string styleId, string accessId)
        {
            // Get the point layer that carries a symbol point style by style id and access id.
            FeatureLayer featureLayer = (FeatureLayer)GetSymbolPointLayer(styleId, accessId);

            // Get the option values of the symbol point style.
            var styles = new SymbolPointStyleModel();
            PointStyle targetPointStyle = featureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle;
            styles.Type = targetPointStyle.SymbolType;
            styles.Size = targetPointStyle.SymbolSize;
            styles.RotationAngle = targetPointStyle.RotationAngle;

            GeoPen symbolPen = targetPointStyle.OutlinePen;
            styles.PenColor = GeoColor.ToHtml(symbolPen.Color);
            styles.PenAlpha = symbolPen.Color.AlphaComponent;
            styles.PenWidth = symbolPen.Width;

            GeoColor symbolSolidBrushColor = ((GeoSolidBrush)targetPointStyle.FillBrush).Color;
            styles.BrushColor = GeoColor.ToHtml(symbolSolidBrushColor);
            styles.BrushAlpha = symbolSolidBrushColor.AlphaComponent;
            return styles;
        }

        /// <summary>
        /// Update style options to tempoary folder for a specific acess id and style id.
        /// </summary>
        /// <param name="styleId">style id</param>
        /// <param name="accessId">access id</param>
        /// <param name="style">style options</param>
        internal static void UpdateLayerStyle<T>(string styleId, string accessId, T style)
        {
            string styleFile = string.Format("{0}_{1}.json", accessId, styleId);
            string styleFilePath = Path.Combine(baseDirectory, "Temp", styleFile);
            if (!Directory.Exists(Path.GetDirectoryName(styleFilePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(styleFilePath));

            if (!File.Exists(styleFilePath))
                File.Create(styleFilePath).Dispose();

            using (StreamWriter streamWriter = new StreamWriter(styleFilePath, false))
            {
                streamWriter.WriteLine(JsonConvert.SerializeObject(style));
            }
        }

        private static AreaStyle CreateSimpleAreaStyle(GeoColor fillBrushColor, GeoColor outlinePenColor, int outlinePenWidth, LineDashStyle borderStyle, float xOffsetInPixel, float yOffsetInPixel)
        {
            AreaStyle returnStyle = new AreaStyle();
            returnStyle.FillBrush = new GeoSolidBrush(fillBrushColor);
            returnStyle.OutlinePen = new GeoPen(outlinePenColor, outlinePenWidth);
            returnStyle.OutlinePen.DashStyle = borderStyle;
            returnStyle.XOffsetInPixel = xOffsetInPixel;
            returnStyle.YOffsetInPixel = yOffsetInPixel;

            return returnStyle;
        }


        /// <summary>
        /// Gets layers for exhibition predefined style.
        /// </summary>
        /// <returns></returns>
        internal static Collection<Layer> GetPredefinedStyleLayers()
        {
            Collection<Layer> layers = new Collection<Layer>();
            BackgroundLayer backgroundLayer = new BackgroundLayer(new GeoSolidBrush(GeoColor.FromArgb(255, 136, 162, 227)));
            layers.Add(backgroundLayer);

            ShapeFileFeatureLayer countriesLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "Countries.shp"));
            countriesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69), 1, LineDashStyle.Solid, 0, 0);
            countriesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen = new GeoPen(GeoColors.Gray);
            countriesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layers.Add(countriesLayer);

            ShapeFileFeatureLayer lakeLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "lake.shp"));
            lakeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = CreateSimpleAreaStyle(GeoColor.FromArgb(255, 136, 162, 227), new GeoColor(), 1, LineDashStyle.Solid, 0, 0);
            lakeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layers.Add(lakeLayer);

            ShapeFileFeatureLayer usStatesLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "USStates.shp"));
            usStatesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 156, 155, 154), 2, LineDashStyle.Solid, 0, 0); ;
            usStatesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layers.Add(usStatesLayer);

            ShapeFileFeatureLayer highwayNetworkShapeLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "USHighwayNetwork.shp"));
            highwayNetworkShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = CreateSimpleLineStyle(GeoColors.Transparent, 1, LineDashStyle.Solid, GeoColor.FromArgb(255, 255, 255, 128), 3.2F, LineDashStyle.Solid, GeoColors.LightGray, 5.2F, LineDashStyle.Solid, true);
            highwayNetworkShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layers.Add(highwayNetworkShapeLayer);

            ShapeFileFeatureLayer majorCitiesLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "USMajorCities.shp"));
            majorCitiesLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = new TextStyle("AREANAME", new GeoFont("Arial", 10), new GeoSolidBrush(GeoColors.Black));
            majorCitiesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layers.Add(majorCitiesLayer);

            return layers;
        }

        private static LineStyle CreateSimpleLineStyle(GeoColor centerlineColor, float centerlineWidth, LineDashStyle centerlineDashStyle, GeoColor innerLineColor, float innerLineWidth, LineDashStyle innerLineDashStyle, GeoColor outerLineColor, float outerLineWidth, LineDashStyle outerLineDashStyle, bool roundCap)
        {

            GeoPen centerPen = new GeoPen(centerlineColor, centerlineWidth);
            centerPen.DashStyle = centerlineDashStyle;
            GeoPen innerPen = new GeoPen(innerLineColor, innerLineWidth);
            innerPen.DashStyle = innerLineDashStyle;
            GeoPen outerPen = new GeoPen(outerLineColor, outerLineWidth);
            outerPen.DashStyle = outerLineDashStyle;

            if (roundCap)
            {
                centerPen.StartCap = DrawingLineCap.Round;
                centerPen.EndCap = DrawingLineCap.Round;
                innerPen.StartCap = DrawingLineCap.Round;
                innerPen.EndCap = DrawingLineCap.Round;
                outerPen.StartCap = DrawingLineCap.Round;
                outerPen.EndCap = DrawingLineCap.Round;
            }

            return new LineStyle(outerPen, innerPen, centerPen);
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
            areaStyle.FillBrush = new GeoSolidBrush(new GeoColor(200, GeoColors.LightOrange));
            areaStyle.OutlinePen = new GeoPen(new GeoSolidBrush(new GeoColor(200, GeoColors.LightBlue)), 2);
            areaShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = areaStyle;
            areaShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Refresh styles if the user has saved styles.
            var savedStyle = GetSavedStyleByAccessId<AreaStyleModel>(styleId, accessId);
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
            lineStyle.CenterPen = new GeoPen(new GeoColor(255, GeoColors.PastelGreen), 2);
            lineStyle.InnerPen = new GeoPen(new GeoColor(255, GeoColors.Black), 5);
            lineStyle.OuterPen = new GeoPen(new GeoColor(255, GeoColors.YellowGreen), 10);
            lineShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = lineStyle;
            lineShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Refresh styles if the user has saved styles.
            var savedStyle = GetSavedStyleByAccessId<LineStyleModel>(styleId, accessId);
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
            pointOfHighSchoolStyle.PointType = PointType.Image;
            pointOfHighSchoolStyle.SymbolSize = 41;
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

            symblePointLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = new PointStyle(PointSymbolType.Triangle, 20, new GeoSolidBrush(GeoColors.Blue));
            symblePointLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // Refresh styles if the user has saved styles.
            var savedStyle = GetSavedStyleByAccessId<SymbolPointStyleModel>(styleId, accessId);
            if (savedStyle != null) UpdateSymbolPointStyle(symblePointLayer, savedStyle);

            return symblePointLayer;
        }

        /// <summary>
        /// Gets layer for exhibition glyph point style.
        /// </summary>
        /// <returns></returns>
        internal static Layer GetGlyphLayer()
        {
            ShapeFileFeatureLayer characterPointLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "HospitalInFrisco.shp"));

            PointStyle hospitalPointStyle = new PointStyle();
            hospitalPointStyle.PointType = PointType.Glyph;
            hospitalPointStyle.RotationAngle = 0;
            hospitalPointStyle.GlyphContent = GeoFont.GetGlyphContent(72);
            GeoFont geoFont = new GeoFont("Arial", 20, DrawingFontStyles.Bold);
            hospitalPointStyle.GlyphFont = geoFont;
            hospitalPointStyle.SymbolSize = 18;
            hospitalPointStyle.FillBrush = new GeoSolidBrush(GeoColors.Blue);

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
            ZoomLevelAreaStyleFrom01To16.FillBrush = new GeoSolidBrush(new GeoColor(100, GeoColors.BrightOrange));
            ZoomLevelAreaStyleFrom01To16.OutlinePen = new GeoPen(new GeoColor(255, GeoColors.Black));
            ZoomLevelAreaShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = ZoomLevelAreaStyleFrom01To16;
            ZoomLevelAreaShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level16;

            AreaStyle ZoomLevelAreaStyle17 = new AreaStyle();
            ZoomLevelAreaStyle17.FillBrush = new GeoSolidBrush(new GeoColor(100, GeoColors.Blue));
            ZoomLevelAreaStyle17.OutlinePen = new GeoPen(new GeoColor(255, 0, 0, 0));
            ZoomLevelAreaShapeLayer.ZoomLevelSet.ZoomLevel17.DefaultAreaStyle = ZoomLevelAreaStyle17;

            AreaStyle ZoomLevelAreaStyle18 = new AreaStyle();
            ZoomLevelAreaStyle18.FillBrush = new GeoSolidBrush(new GeoColor(100, GeoColors.Red));
            ZoomLevelAreaStyle18.OutlinePen = new GeoPen(new GeoColor(255, 0, 0, 0));
            ZoomLevelAreaShapeLayer.ZoomLevelSet.ZoomLevel18.DefaultAreaStyle = ZoomLevelAreaStyle18;

            AreaStyle ZoomLevelAreaStyle19 = new AreaStyle();
            ZoomLevelAreaStyle19.FillBrush = new GeoSolidBrush(new GeoColor(100, GeoColors.Green));
            ZoomLevelAreaStyle19.OutlinePen = new GeoPen(new GeoColor(255, 0, 0, 0));
            ZoomLevelAreaShapeLayer.ZoomLevelSet.ZoomLevel19.DefaultAreaStyle = ZoomLevelAreaStyle19;

            layers.Add(ZoomLevelAreaShapeLayer);

            ShapeFileFeatureLayer zoomLevelLineLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "ThinkGeoLine.shp"));

            // Initialization four line styles  for different zoomlevel.
            LineStyle zoomLevelLineStyleFrom01To16 = new LineStyle();
            zoomLevelLineStyleFrom01To16.CenterPen = new GeoPen(new GeoColor(255, GeoColors.DarkOrange), 5);
            zoomLevelLineLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = zoomLevelLineStyleFrom01To16;
            zoomLevelLineLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level16;

            LineStyle zoomLevelLineStyle17 = new LineStyle();
            zoomLevelLineStyle17.CenterPen = new GeoPen(new GeoColor(255, GeoColors.DarkBlue), 5);
            zoomLevelLineLayer.ZoomLevelSet.ZoomLevel17.DefaultLineStyle = zoomLevelLineStyle17;

            LineStyle zoomLevelLineStyle18 = new LineStyle();
            zoomLevelLineStyle18.CenterPen = new GeoPen(new GeoColor(255, GeoColors.LightOrange), 5);
            zoomLevelLineLayer.ZoomLevelSet.ZoomLevel18.DefaultLineStyle = zoomLevelLineStyle18;

            LineStyle zoomLevelLineStyle19 = new LineStyle();
            zoomLevelLineStyle19.CenterPen = new GeoPen(new GeoColor(255, GeoColors.LightGreen), 5);
            zoomLevelLineLayer.ZoomLevelSet.ZoomLevel19.DefaultLineStyle = zoomLevelLineStyle19;
            layers.Add(zoomLevelLineLayer);

            ShapeFileFeatureLayer ZoomLevelPointLayer = new ShapeFileFeatureLayer(Path.Combine(baseDirectory, "ThinkGeoLocation.shp"));
            ZoomLevelPointLayer.Transparency = 200f;

            // Initialization four point styles  for different zoomlevel.
            PointStyle pointStyleForZoomLevelFrom01To16 = new PointStyle();
            pointStyleForZoomLevelFrom01To16.Image = new GeoImage(Path.Combine(baseDirectory, "ThinkGeoLogo.png"));
            pointStyleForZoomLevelFrom01To16.PointType = PointType.Image;
            pointStyleForZoomLevelFrom01To16.RotationAngle = 0;
            ZoomLevelPointLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = pointStyleForZoomLevelFrom01To16;
            ZoomLevelPointLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level16;

            PointStyle pointStyleForZoomLevel17 = new PointStyle();
            pointStyleForZoomLevel17.OutlinePen = new GeoPen(GeoColor.FromArgb(255, GeoColors.DarkBlue), 8);
            ZoomLevelPointLayer.ZoomLevelSet.ZoomLevel17.DefaultPointStyle = pointStyleForZoomLevel17;

            TextStyle textStyleForZoomLevel18 = new TextStyle("name", new GeoFont("Arial", 12), new GeoSolidBrush(GeoColors.LightRed));
            textStyleForZoomLevel18.HaloPen = new GeoPen(GeoColors.FloralWhite, 5);
            textStyleForZoomLevel18.SplineType = SplineType.ForceSplining;
            PointStyle pointStyleForZoomLevel18 = new PointStyle();
            pointStyleForZoomLevel18.OutlinePen = new GeoPen(GeoColor.FromArgb(255, GeoColors.LightRed), 8);
            ZoomLevelPointLayer.ZoomLevelSet.ZoomLevel18.CustomStyles.Add(textStyleForZoomLevel18);
            ZoomLevelPointLayer.ZoomLevelSet.ZoomLevel18.CustomStyles.Add(pointStyleForZoomLevel18);

            PointStyle pointStyleForZoomLevel19 = new PointStyle();
            pointStyleForZoomLevel19.OutlinePen = new GeoPen(GeoColor.FromArgb(255, GeoColors.DarkGreen), 8);
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
            circleStyle.OutlinePen = new GeoPen(new GeoColor(255, GeoColors.Red), 3);
            circleStyle.SymbolType = PointSymbolType.Circle;
            circleStyle.SymbolSize = 26;

            PointStyle starStyle = new PointStyle();
            starStyle.PointType = PointType.Symbol;
            starStyle.FillBrush = new GeoSolidBrush(new GeoColor(255, GeoColors.Blue));
            starStyle.SymbolType = PointSymbolType.Star;
            starStyle.SymbolSize = 20;

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
        private static T GetSavedStyleByAccessId<T>(string overlayId, string accessId)
        {
            string styleFile = string.Format("{0}_{1}.json", accessId, overlayId);
            string styleFilePath = Path.Combine(baseDirectory, "Temp", styleFile);

            if (File.Exists(styleFilePath))
            {
                string content = File.ReadAllText(styleFilePath);
                return JsonConvert.DeserializeObject<T>(content);
            }

            return default(T);
        }

        /// <summary>
        /// Updates FeatureLayer's area style.
        /// </summary>
        /// <param name="featureLayer">target FeatureLayer</param>
        /// <param name="savedStyle">new area style options</param>
        private static void UpdateAreaStyle(FeatureLayer featureLayer, AreaStyleModel savedStyle)
        {
            string fillSolidBrushStr = savedStyle.BrushColor;
            byte fillSolidBrushAlpha = savedStyle.BrushAlpha;
            string outerPenStr = savedStyle.OutlinePenColor;
            byte outerPenAlpha = savedStyle.OutlinePenAlpha;
            float outerPenWidth = savedStyle.OutlinePenWidth;

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
        private static void UpdateLineStyle(FeatureLayer featureLayer, LineStyleModel savedStyle)
        {
            string centerPenColor = savedStyle.CenterPenColor;
            byte centerPenColorAlpha = savedStyle.CenterPenAlpha;
            float centerPenWidth = savedStyle.CenterPenWidth;

            string outterPenColor = savedStyle.OuterPenColor;
            byte outterPenColorAlpha = savedStyle.OuterPenAlpha;
            float outterPenWidth = savedStyle.OuterPenWidth;

            string innerPenColor = savedStyle.InnerPenColor;
            byte innerPenColorAlpha = savedStyle.InnerPenAlpha;
            float innerPenWidth = savedStyle.InnerPenWidth;
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
        private static void UpdateSymbolPointStyle(FeatureLayer featureLayer, SymbolPointStyleModel savedStyle)
        {

            byte symbolPointPenAlpha = savedStyle.PenAlpha;
            float symbolPointPenWidth = savedStyle.PenWidth;
            string symbolPointPenColor = savedStyle.PenColor;
            string symbolPointSolidBrushColor = savedStyle.BrushColor;
            byte symbolPointSolidBrushAlpha = savedStyle.BrushAlpha;

            PointStyle thinkGeoLocationStyle = new PointStyle();
            thinkGeoLocationStyle.PointType = PointType.Symbol;
            thinkGeoLocationStyle.SymbolType = savedStyle.Type;
            thinkGeoLocationStyle.SymbolSize = savedStyle.Size;
            thinkGeoLocationStyle.RotationAngle = savedStyle.RotationAngle;
            thinkGeoLocationStyle.OutlinePen = new GeoPen(new GeoColor(symbolPointPenAlpha, GeoColor.FromHtml(symbolPointPenColor)), symbolPointPenWidth);
            thinkGeoLocationStyle.FillBrush = new GeoSolidBrush(new GeoColor(symbolPointSolidBrushAlpha, GeoColor.FromHtml(symbolPointSolidBrushColor)));

            featureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = thinkGeoLocationStyle;
            featureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
        }
    }
}