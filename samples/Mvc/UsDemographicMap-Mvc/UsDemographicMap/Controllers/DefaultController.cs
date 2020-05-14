/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Xml.Linq;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.USDemographicMap;

namespace MapSuiteUsDemographicMap.Controllers
{
    public class DefaultController : Controller
    {
        public ActionResult Index()
        {
            // Initialize map
            Map map = new Map("Map1", new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage), new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage));
            InitializeMap(map);
            // Load categories from xml file
            ViewData["Categories"] = LoadCategories();

            return View(map);
        }

        [MapActionFilter]
        public string IdentifyFeature(Map map, GeoCollection<object> args)
        {
            string callbackResult = string.Empty;
            if (map != null)
            {
                JsonCallbackRequest callbackRequest = JsonSerializer.Deserialize<JsonCallbackRequest>(args[0].ToString());
                callbackResult = GetIdentifiedFeatureInfo(callbackRequest, map);
            }

            return callbackResult;
        }

        [MapActionFilter]
        public string ApplyStyle(Map map, GeoCollection<object> args)
        {
            string callbackResult = string.Empty;
            if (map != null)
            {
                // Replace the "$=" with ">=", because the ">=" is not allowed in AJAX Content
                string jsonParameters = args[0].ToString();
                if (jsonParameters.Contains("$"))
                {
                    jsonParameters = jsonParameters.Replace("$", ">=");
                }

                JsonCallbackRequest callbackRequest = JsonSerializer.Deserialize<JsonCallbackRequest>(jsonParameters);
                callbackResult = UpdateMapAndLegend(callbackRequest, map).ToString();
            }

            return callbackResult;
        }

        private string GetIdentifiedFeatureInfo(JsonCallbackRequest callbackRequest, Map map)
        {
            // Get the requested sub-categories
            Category selectedCategory = null;
            Collection<CategoryItem> selectedCategoryItems = new Collection<CategoryItem>();
            if (callbackRequest.StyleBuildType == DemographicStyleBuilderType.PieChart)
            {
                selectedCategoryItems = GetCategoryItemByAlias(callbackRequest.SelectedCategoryItems, ref selectedCategory);
            }
            else
            {
                selectedCategoryItems = GetCategoryItemByAlias(new Collection<string>() { callbackRequest.RequestColumnAlias }, ref selectedCategory);
            }

            // Get the selected columns
            Collection<string> selectedColumns = new Collection<string>();
            foreach (CategoryItem categoryItem in selectedCategoryItems)
            {
                selectedColumns.Add(categoryItem.ColumnName);
            }
            selectedColumns.Add("Name");

            // Find the identified feature with specified columns
            ShapeFileFeatureLayer statesLayer = map.DynamicOverlay.Layers["usStatesLayer"] as ShapeFileFeatureLayer;
            statesLayer.Open();
            Feature identifiedFeature = statesLayer.FeatureSource.GetFeatureById(callbackRequest.SelectedFeatureId, selectedColumns);
            ((CloudPopup)map.Popups["featureInfoPopup"]).Position = identifiedFeature.GetShape().GetCenterPoint();

            // Format the inner-html of the popup
            StringBuilder popupHtml = new StringBuilder("<table>");
            popupHtml.Append(string.Format("<tr><td class='popupTitle'>{0}</td></tr>", identifiedFeature.ColumnValues["Name"]));
            popupHtml.Append("<tr><td><div class='hrLine'></div></td></tr>");
            for (int i = 0; i < selectedColumns.Count - 1; i++)
            {
                string formatedString = DemographicStyleTextFormatter.GetFormatedString(selectedColumns[i], double.Parse(identifiedFeature.ColumnValues[selectedColumns[i]]));
                popupHtml.Append(string.Format("<tr class='popupText'><td>{0}</td></tr>", formatedString));
            }
            popupHtml.Append("</table>");

            return popupHtml.ToString();
        }

        private bool UpdateMapAndLegend(JsonCallbackRequest callbackRequest, Map map)
        {
            // Get the requested sub-categories
            Category selectedCategory = null;
            Collection<CategoryItem> selectedCategoryItems = new Collection<CategoryItem>();
            if (callbackRequest.StyleBuildType == DemographicStyleBuilderType.PieChart)
            {
                selectedCategoryItems = GetCategoryItemByAlias(callbackRequest.SelectedCategoryItems, ref selectedCategory);
            }
            else
            {
                selectedCategoryItems = GetCategoryItemByAlias(new Collection<string>() { callbackRequest.RequestColumnAlias }, ref selectedCategory);
            }

            DemographicStyleBuilder demographicStyle = null;
            switch (callbackRequest.StyleBuildType)
            {
                case DemographicStyleBuilderType.PieChart:
                    demographicStyle = new PieChartDemographicStyleBuilder();
                    foreach (CategoryItem categoryItem in selectedCategoryItems)
                    {
                        ((PieChartDemographicStyleBuilder)demographicStyle).SelectedColumnAliases.Add(categoryItem.Alias);
                    }
                    if (!string.IsNullOrEmpty(callbackRequest.StartColor))
                    {
                        ((PieChartDemographicStyleBuilder)demographicStyle).Color = GeoColor.FromHtml(callbackRequest.StartColor);
                    }
                    break;
                case DemographicStyleBuilderType.Thematic:
                    demographicStyle = new ThematicDemographicStyleBuilder();
                    if (!string.IsNullOrEmpty(callbackRequest.StartColor))
                    {
                        ((ThematicDemographicStyleBuilder)demographicStyle).StartColor = GeoColor.FromHtml(callbackRequest.StartColor);
                    }
                    if (!string.IsNullOrEmpty(callbackRequest.EndColor))
                    {
                        ((ThematicDemographicStyleBuilder)demographicStyle).EndColor = GeoColor.FromHtml(callbackRequest.EndColor);
                    }
                    if (!string.IsNullOrEmpty(callbackRequest.ColorWheelDirection))
                    {
                        ((ThematicDemographicStyleBuilder)demographicStyle).ColorWheelDirection = callbackRequest.ColorWheelDirection.Equals("Clockwise") ? ColorWheelDirection.Clockwise : ColorWheelDirection.CounterClockwise;
                    }
                    break;
                case DemographicStyleBuilderType.DotDensity:
                    demographicStyle = new DotDensityDemographicStyleBuilder();
                    if (!string.IsNullOrEmpty(callbackRequest.StartColor))
                    {
                        ((DotDensityDemographicStyleBuilder)demographicStyle).Color = GeoColor.FromHtml(callbackRequest.StartColor);
                    }
                    if (callbackRequest.SliderValue > 0)
                    {
                        ((DotDensityDemographicStyleBuilder)demographicStyle).DotDensityValue = 50 * (callbackRequest.SliderValue / 3.0);
                    }
                    break;
                case DemographicStyleBuilderType.ValueCircle:
                    demographicStyle = new ValueCircleDemographicStyleBuilder();
                    if (!string.IsNullOrEmpty(callbackRequest.StartColor))
                    {
                        ((ValueCircleDemographicStyleBuilder)demographicStyle).Color = GeoColor.FromHtml(callbackRequest.StartColor);
                    }
                    if (callbackRequest.SliderValue > 0)
                    {
                        ((ValueCircleDemographicStyleBuilder)demographicStyle).RadiusRatio = callbackRequest.SliderValue / 3.0;
                    }
                    break;
                default:
                    break;
            }

            foreach (CategoryItem categoryItem in selectedCategoryItems)
            {
                demographicStyle.SelectedColumns.Add(categoryItem.ColumnName);
            }

            ShapeFileFeatureLayer statesLayer = map.DynamicOverlay.Layers["usStatesLayer"] as ShapeFileFeatureLayer;
            statesLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Clear();
            Style selectedStyle = demographicStyle.GetStyle(statesLayer.FeatureSource);
            statesLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(selectedStyle);
            statesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            // update the legend based on the selected style
            UpdateLegend(demographicStyle, map);

            return true;
        }

        private Collection<CategoryItem> GetCategoryItemByAlias(Collection<string> requestAliases, ref Category queriedCategory)
        {
            Collection<CategoryItem> queriedItems = new Collection<CategoryItem>();
            Dictionary<string, Category> categories = LoadCategories();
            foreach (var category in categories)
            {
                foreach (var item in category.Value.Items)
                {
                    if (requestAliases.Contains(item.Alias))
                    {
                        queriedCategory = category.Value;
                        queriedItems.Add(item);
                    }
                }
            }

            return queriedItems;
        }

        private void InitializeMap(Map map)
        {
            map.MapUnit = GeographyUnit.Meter;
            map.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            map.MapTools.Logo.Enabled = true;
            map.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
            map.CurrentExtent = new RectangleShape(-14268781.3528224, 7745550.26928538, -7693973.9278447, 2295596.96842637);

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            map.CustomOverlays.Add(backgroundOverlay);

            // us states layer
            ShapeFileFeatureLayer statesLayer = new ShapeFileFeatureLayer(Server.MapPath(ConfigurationManager.AppSettings["UsShapefilePath"]));
            ThematicDemographicStyleBuilder selectedStyle = new ThematicDemographicStyleBuilder(new Collection<string>() { "Population" });
            statesLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(selectedStyle.GetStyle(statesLayer.FeatureSource));
            statesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            map.DynamicOverlay.Layers.Add("usStatesLayer", statesLayer);

            // highlight layers
            map.HighlightOverlay.HighlightStyle = new FeatureOverlayStyle(GeoColor.FromArgb(150, GeoColor.FromHtml("#449FBC")), GeoColor.FromHtml("#014576"), 1);
            map.HighlightOverlay.Style = new FeatureOverlayStyle(GeoColor.SimpleColors.Transparent, GeoColor.SimpleColors.Transparent, 0);
            statesLayer.Open();
            foreach (Feature feature in statesLayer.FeatureSource.GetAllFeatures(ReturningColumnsType.NoColumns))
            {
                map.HighlightOverlay.Features.Add(feature.Id, feature);
            }
            statesLayer.Close();

            // Add hover-popup
            CloudPopup featureinfoPopup = new CloudPopup("featureInfoPopup", new PointShape(-10981377.6403336, 4658770.71698388), "State Info"); // make the popup out of map view temporarily
            featureinfoPopup.AutoSize = true;
            map.Popups.Add(featureinfoPopup);

            // Add Legend adorment overlay
            LegendAdornmentLayer legendAdornmentLayer = new LegendAdornmentLayer();
            legendAdornmentLayer.Location = AdornmentLocation.LowerLeft;
            legendAdornmentLayer.XOffsetInPixel = 10;
            legendAdornmentLayer.Title = new LegendItem();
            legendAdornmentLayer.Title.ImageJustificationMode = LegendImageJustificationMode.JustifyImageRight;
            legendAdornmentLayer.Title.TopPadding = 10;
            legendAdornmentLayer.Title.BottomPadding = 10;
            legendAdornmentLayer.Title.TextStyle = new TextStyle("Population", new GeoFont("Segoe UI", 12), new GeoSolidBrush(GeoColor.SimpleColors.Black));
            map.AdornmentOverlay.Layers.Add(legendAdornmentLayer);

            ThematicDemographicStyleBuilder initDemographicStyle = new ThematicDemographicStyleBuilder(new Collection<string>() { "Population" });
            UpdateLegend(initDemographicStyle, map);
        }

        private void UpdateLegend(DemographicStyleBuilder styleBuilder, Map map)
        {
            LegendAdornmentLayer legendAdornmentLayer = map.AdornmentOverlay.Layers[0] as LegendAdornmentLayer;
            legendAdornmentLayer.LegendItems.Clear();

            if (styleBuilder is ThematicDemographicStyleBuilder)
            {
                AddThematicLegendItems(styleBuilder, legendAdornmentLayer, map);
            }
            else if (styleBuilder is DotDensityDemographicStyleBuilder)
            {
                AddDotDensityLegendItems(styleBuilder, legendAdornmentLayer, map);
            }
            else if (styleBuilder is ValueCircleDemographicStyleBuilder)
            {
                AddValueCircleLegendItems(styleBuilder, legendAdornmentLayer, map);
            }
            else if (styleBuilder is PieChartDemographicStyleBuilder)
            {
                AddPieGraphLegendItems(styleBuilder, legendAdornmentLayer, map);
            }

            legendAdornmentLayer.ContentResizeMode = LegendContentResizeMode.Fixed;
            legendAdornmentLayer.Height = GetLegendHeight(legendAdornmentLayer);
            legendAdornmentLayer.Width = GetLegendWidth(legendAdornmentLayer);
        }

        private void AddPieGraphLegendItems(DemographicStyleBuilder styleBuilder, LegendAdornmentLayer legendAdornmentLayer, Map map)
        {
            ShapeFileFeatureLayer statesLayer = map.DynamicOverlay.Layers["usStatesLayer"] as ShapeFileFeatureLayer;
            PieZedGraphStyle zedGraphStyle = (PieZedGraphStyle)styleBuilder.GetStyle(statesLayer.FeatureSource);

            foreach (KeyValuePair<string, GeoColor> item in zedGraphStyle.PieSlices)
            {
                LegendItem legendItem = new LegendItem();
                legendItem.ImageWidth = 20;
                legendItem.TextRightPadding = 5;
                legendItem.RightPadding = 5;
                legendItem.ImageStyle = new AreaStyle(new GeoSolidBrush(item.Value));
                legendItem.TextStyle = new TextStyle(item.Key, new GeoFont("Segoe UI", 10), new GeoSolidBrush(GeoColor.SimpleColors.Black));
                legendAdornmentLayer.LegendItems.Add(legendItem);
            }
        }

        private void AddValueCircleLegendItems(DemographicStyleBuilder styleBuilder, LegendAdornmentLayer legendAdornmentLayer, Map map)
        {
            ShapeFileFeatureLayer statesLayer = map.DynamicOverlay.Layers["usStatesLayer"] as ShapeFileFeatureLayer;
            ValueCircleStyle valueCircleStyle = (ValueCircleStyle)styleBuilder.GetStyle(statesLayer.FeatureSource);

            int[] circleAreas = new int[] { 160, 320, 640, 1280 };
            foreach (int circleArea in circleAreas)
            {
                LegendItem legendItem = new LegendItem();
                double radius = Math.Sqrt(circleArea / Math.PI);
                legendItem.ImageStyle = new PointStyle(PointSymbolType.Circle, new GeoSolidBrush(valueCircleStyle.InnerColor), new GeoPen(new GeoSolidBrush(valueCircleStyle.OuterColor)), (int)(radius * 2));
                AreaStyle maskStyle = new AreaStyle(new GeoPen(GeoColor.StandardColors.LightGray, 1), new GeoSolidBrush(GeoColor.SimpleColors.Transparent));
                legendItem.ImageMask = maskStyle;
                legendItem.ImageWidth = 48;
                legendItem.TextTopPadding = 16;
                legendItem.TextRightPadding = 5;
                legendItem.BottomPadding = 16;
                legendItem.TopPadding = 16;
                legendItem.RightPadding = 5;

                double drawingRadius = circleArea / valueCircleStyle.DrawingRadiusRatio * valueCircleStyle.BasedScale / valueCircleStyle.DefaultZoomLevel.Scale;
                double ratio = (valueCircleStyle.MaxValidValue - valueCircleStyle.MinValidValue) / (valueCircleStyle.MaxCircleAreaInDefaultZoomLevel - valueCircleStyle.MinCircleAreaInDefaultZoomLevel);
                double resultValue = (drawingRadius - valueCircleStyle.MinCircleAreaInDefaultZoomLevel) * ratio + valueCircleStyle.MinValidValue;

                string text = DemographicStyleTextFormatter.GetFormatedStringForLegendItem(valueCircleStyle.ColumnName, resultValue);
                legendItem.TextStyle = new TextStyle(text, new GeoFont("Segoe UI", 10), new GeoSolidBrush(GeoColor.SimpleColors.Black));

                legendAdornmentLayer.LegendItems.Add(legendItem);
            }
        }

        private void AddDotDensityLegendItems(DemographicStyleBuilder styleBuilder, LegendAdornmentLayer legendAdornmentLayer, Map map)
        {
            ShapeFileFeatureLayer statesLayer = map.DynamicOverlay.Layers["usStatesLayer"] as ShapeFileFeatureLayer;
            CustomDotDensityStyle dotDensityStyle = (CustomDotDensityStyle)styleBuilder.GetStyle(statesLayer.FeatureSource);

            int[] pointCounts = new int[] { 5, 10, 20, 50 };
            foreach (int pointCount in pointCounts)
            {
                LegendItem legendItem = new LegendItem();
                legendItem.ImageMask = new AreaStyle(new GeoPen(GeoColor.StandardColors.LightGray, 1), new GeoSolidBrush(GeoColor.SimpleColors.Transparent));
                legendItem.ImageWidth = 48;
                legendItem.TextTopPadding = 16;
                legendItem.TextRightPadding = 5;
                legendItem.BottomPadding = 16;
                legendItem.TopPadding = 16;
                legendItem.RightPadding = 5;
                CustomDotDensityStyle legendDotDensityStyle = (CustomDotDensityStyle)dotDensityStyle.CloneDeep();
                legendDotDensityStyle.DrawingPointsNumber = pointCount;
                legendItem.ImageStyle = legendDotDensityStyle;

                string text = string.Format(CultureInfo.InvariantCulture, "{0:0.####}", DemographicStyleTextFormatter.GetFormatedStringForLegendItem(dotDensityStyle.ColumnName, (pointCount / dotDensityStyle.PointToValueRatio)));
                legendItem.TextStyle = new TextStyle(text, new GeoFont("Segoe UI", 10), new GeoSolidBrush(GeoColor.SimpleColors.Black));

                legendAdornmentLayer.LegendItems.Add(legendItem);
            }
        }

        private void AddThematicLegendItems(DemographicStyleBuilder styleBuilder, LegendAdornmentLayer legendAdornmentLayer, Map map)
        {
            ShapeFileFeatureLayer statesLayer = map.DynamicOverlay.Layers["usStatesLayer"] as ShapeFileFeatureLayer;
            ClassBreakStyle thematicStyle = (ClassBreakStyle)styleBuilder.GetStyle(statesLayer.FeatureSource);

            for (int i = 0; i < thematicStyle.ClassBreaks.Count; i++)
            {
                LegendItem legendItem = new LegendItem();

                if (i < thematicStyle.ClassBreaks.Count)
                {
                    legendItem.ImageStyle = thematicStyle.ClassBreaks[i].DefaultAreaStyle;
                    legendItem.ImageWidth = 20;
                    legendItem.TextRightPadding = 5;
                    legendItem.RightPadding = 5;

                    string text = string.Empty;
                    if (i != thematicStyle.ClassBreaks.Count - 1)
                    {
                        text = string.Format("{0:#,0.####} ~ {1:#,0.####}",
                            DemographicStyleTextFormatter.GetFormatedStringForLegendItem(thematicStyle.ColumnName, thematicStyle.ClassBreaks[i].Value),
                            DemographicStyleTextFormatter.GetFormatedStringForLegendItem(thematicStyle.ColumnName, thematicStyle.ClassBreaks[i + 1].Value));
                    }
                    else
                    {
                        text = string.Format("> {0:#,0.####}",
                            DemographicStyleTextFormatter.GetFormatedStringForLegendItem(thematicStyle.ColumnName, thematicStyle.ClassBreaks[i].Value));
                    }
                    legendItem.TextStyle = new TextStyle(text, new GeoFont("Segoe UI", 10), new GeoSolidBrush(GeoColor.SimpleColors.Black));
                }
                legendAdornmentLayer.LegendItems.Add(legendItem);
            }
        }

        public float GetLegendWidth(LegendAdornmentLayer legendAdornmentLayer)
        {
            PlatformGeoCanvas gdiPlusGeoCanvas = new PlatformGeoCanvas();
            LegendItem title = legendAdornmentLayer.Title;
            float width = gdiPlusGeoCanvas.MeasureText(title.TextStyle.TextColumnName, new GeoFont("Segoe UI", 12)).Width
               + title.ImageWidth + title.ImageRightPadding + title.ImageLeftPadding + title.TextRightPadding + title.TextLeftPadding + title.LeftPadding + title.RightPadding;

            foreach (LegendItem legendItem in legendAdornmentLayer.LegendItems)
            {
                float legendItemWidth = gdiPlusGeoCanvas.MeasureText(legendItem.TextStyle.TextColumnName, new GeoFont("Segoe UI", 10)).Width
                    + legendItem.ImageWidth + legendItem.ImageRightPadding + legendItem.ImageLeftPadding + legendItem.TextRightPadding + legendItem.TextLeftPadding + legendItem.LeftPadding + legendItem.RightPadding;
                if (width < legendItemWidth)
                {
                    width = legendItemWidth;
                }
            }
            return width;
        }

        public float GetLegendHeight(LegendAdornmentLayer legendAdornmentLayer)
        {
            PlatformGeoCanvas gdiPlusGeoCanvas = new PlatformGeoCanvas();
            LegendItem title = legendAdornmentLayer.Title;
            float legendHeight = Math.Max(gdiPlusGeoCanvas.MeasureText(title.TextStyle.TextColumnName, new GeoFont("Segoe UI", 12)).Height, title.ImageHeight);
            legendHeight = Math.Max(legendHeight, title.Height);
            float height = legendHeight + Math.Max(title.ImageTopPadding, title.TextTopPadding) + title.TopPadding + Math.Max(title.ImageBottomPadding, title.TextBottomPadding) + title.BottomPadding;

            foreach (LegendItem legendItem in legendAdornmentLayer.LegendItems)
            {
                float itemLegendHeight = Math.Max(gdiPlusGeoCanvas.MeasureText(legendItem.TextStyle.TextColumnName, new GeoFont("Segoe UI", 10)).Height, legendItem.ImageHeight);
                float itemHeight = itemLegendHeight + Math.Max(legendItem.ImageTopPadding, legendItem.TextTopPadding) + legendItem.TopPadding + Math.Max(legendItem.ImageBottomPadding, legendItem.TextBottomPadding) + legendItem.BottomPadding;

                height += itemHeight;
            }
            return height;
        }

        private Dictionary<string, Category> LoadCategories()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + ConfigurationManager.AppSettings["CategoryFilePath"];
            var xDoc = XDocument.Load(path);
            var elements = from category in xDoc.Element("DemographicMap").Elements("Category")
                           select category;

            Dictionary<string, Category> categories = new Dictionary<string, Category>();
            foreach (var element in elements)
            {
                Category category = new Category();
                category.Title = element.Attribute("name").Value;
                category.CategoryImage = string.Format(@"Content\{0}", element.Attribute("icon").Value);

                foreach (var item in element.Elements("item"))
                {
                    CategoryItem categoryItem = new CategoryItem();
                    categoryItem.ColumnName = item.Element("columnName").Value;
                    categoryItem.Alias = item.Element("alias").Value;
                    category.Items.Add(categoryItem);
                }

                categories.Add(category.Title, category);
            }

            return categories;
        }
    }
}
