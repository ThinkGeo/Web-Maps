/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    a Client ID and Secret. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Collections.ObjectModel;
using System.Web.UI.WebControls;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace CreateSublayers
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.CurrentExtent = new RectangleShape(-14607343.5818934, 7371576.14679691, -6014592.08756057, 1910351.0222467);

                // Please input your ThinkGeo Cloud API Key to enable the background map. 
                ThinkGeoCloudRasterMapsOverlay thinkgeoCloudMapsOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                Map1.CustomOverlays.Add(thinkgeoCloudMapsOverlay);

                ShapeFileFeatureLayer shapeLayer = new ShapeFileFeatureLayer(Server.MapPath("~/App_Data/usStatesCensus2010.shp"));
                ClassBreakStyle classBreakStyle = new ClassBreakStyle("population");
                GeoColor outlineColor = GeoColor.SimpleColors.DarkBlue;
                classBreakStyle.ClassBreaks.Add(new ClassBreak(100000, CreateClassBreakAreaStyle(outlineColor, GeoColor.SimpleColors.Green)));
                classBreakStyle.ClassBreaks.Add(new ClassBreak(1000000, CreateClassBreakAreaStyle(outlineColor, GeoColor.SimpleColors.Yellow)));
                classBreakStyle.ClassBreaks.Add(new ClassBreak(10000000, CreateClassBreakAreaStyle(outlineColor, GeoColor.SimpleColors.Red)));
                shapeLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(classBreakStyle);
                shapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                Map1.DynamicOverlay.Layers.Add(shapeLayer);
            }
        }

        protected void SelectedLayers(object sender, EventArgs e)
        {
            CheckBox layerCheckBox = (CheckBox)sender;
            var maxPopulation = Double.MaxValue;
            var minPopulation = Double.MinValue;
            switch (layerCheckBox.ID)
            {
                case "population1":
                    maxPopulation = 1000000;
                    break;
                case "population2":
                    minPopulation = 1000000;
                    maxPopulation = 10000000;
                    break;
                case "population3":
                    minPopulation = 10000000;
                    break;
            }

            ShapeFileFeatureLayer featureLayer = (ShapeFileFeatureLayer)(Map1.DynamicOverlay.Layers[0]);
            featureLayer.Open();
            if (!layerCheckBox.Checked)
            {
                Collection<Feature> features = featureLayer.FeatureSource.GetAllFeatures(ReturningColumnsType.AllColumns);
                featureLayer.Close();

                foreach (Feature feature in features)
                {
                    var population = Convert.ToDouble(feature.ColumnValues["population"]);
                    if (population > minPopulation && population <= maxPopulation)
                        featureLayer.FeatureIdsToExclude.Add(feature.Id);
                }
            }
            else
            {
                string[] excludeFeatures = new string[featureLayer.FeatureIdsToExclude.Count];
                featureLayer.FeatureIdsToExclude.CopyTo(excludeFeatures, 0);
                featureLayer.FeatureIdsToExclude.Clear();
                foreach (var excludeFeature in excludeFeatures)
                {
                    var tempFeature = featureLayer.FeatureSource.GetFeatureById(excludeFeature, ReturningColumnsType.AllColumns);
                    var population = Convert.ToDouble(tempFeature.ColumnValues["population"]);
                    if (minPopulation > population || population >= maxPopulation)
                        featureLayer.FeatureIdsToExclude.Add(excludeFeature);
                }
            }

            Map1.DynamicOverlay.Redraw();
        }

        private AreaStyle CreateClassBreakAreaStyle(GeoColor outlineColor, GeoColor brushColor)
        {
            GeoSolidBrush geoSolidBrush = new GeoSolidBrush(new GeoColor(100, brushColor));
            AreaStyle areaStyle = new AreaStyle(new GeoPen(outlineColor), geoSolidBrush);
            return areaStyle;
        }
    }
}