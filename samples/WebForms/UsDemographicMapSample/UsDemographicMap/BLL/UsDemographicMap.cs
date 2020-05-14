using System.Collections.ObjectModel;
using System.Web.UI;
using ThinkGeo.MapSuite.Core;
using ThinkGeo.MapSuite.WebEdition;
using System.Configuration;

namespace ThinkGeo.MapSuite.USDemographicMap
{
    public class UsDemographicMap
    {
        private Map mapControl;
        private Page mainPage;
        private ShapeFileFeatureLayer statesLayer;
        private ThematicDemographicStyle selectedStyle;

        private UsDemographicMap()
        { }

        public UsDemographicMap(Map map, Page page)
        {
            this.mapControl = map;
            this.mainPage = page;
        }

        public Map MapControl
        {
            get { return mapControl; }
        }

        public Page MainPage
        {
            get { return mainPage; }
        }

        public ShapeFileFeatureLayer StatesLayer
        {
            get { return statesLayer; }
        }

        public ThematicDemographicStyle SelectedStyle
        {
            get { return selectedStyle; }
            set { selectedStyle = value; }
        }

        public void Initialize()
        {
            MapControl.MapUnit = GeographyUnit.DecimalDegree;
            MapControl.MapTools.Logo.Enabled = true;
            MapControl.MapBackground.BackgroundBrush = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
            MapControl.CurrentExtent = new RectangleShape(-128.17864375, 56.9286546875, -69.11614375, 20.1903734375);

            // Display base-map
            WorldMapKitWmsWebOverlay worldMapKitOverlay = new WorldMapKitWmsWebOverlay();
            MapControl.CustomOverlays.Add(worldMapKitOverlay);

            // Display US states with selected style
            statesLayer = new ShapeFileFeatureLayer(MainPage.Server.MapPath(ConfigurationManager.AppSettings["UsShapefilePath"]));

            statesLayer.Open();
            Collection<string> basedColumns = new Collection<string>() { "Pop" };
            SelectedStyle = new ThematicDemographicStyle(basedColumns);

            statesLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(SelectedStyle.GetStyle(statesLayer.FeatureSource));
            statesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            MapControl.DynamicOverlay.Layers.Add("usStatesLayer", statesLayer);

            // Display highlight overlay
            statesLayer.Open();
            MapControl.HighlightOverlay.HighlightStyle = new FeatureOverlayStyle(GeoColor.FromArgb(100, GeoColor.StandardColors.LightBlue), GeoColor.StandardColors.White, 1);
            MapControl.HighlightOverlay.Style = new FeatureOverlayStyle(GeoColor.SimpleColors.Transparent, GeoColor.SimpleColors.Transparent, 0);
            foreach (Feature feature in statesLayer.FeatureSource.GetAllFeatures(ReturningColumnsType.NoColumns))
            {
                MapControl.HighlightOverlay.Features.Add(feature.Id, feature);
            }
            statesLayer.Close();
        }
    }
}