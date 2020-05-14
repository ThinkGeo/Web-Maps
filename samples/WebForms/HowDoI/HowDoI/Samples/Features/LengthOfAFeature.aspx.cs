using System;
using System.Collections.ObjectModel;
using System.Globalization;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples.Features
{
    public partial class LengthOfAFeature : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-97.766, 30.291, -97.755, 30.286);
                Map1.MapUnit = GeographyUnit.DecimalDegree;

                ShapeFileFeatureLayer roadLayer = new ShapeFileFeatureLayer(Server.MapPath(@"~\SampleData\USA\Austin\austinstreets.shp"));
                roadLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.White, 9.2F, GeoColor.StandardColors.DarkGray, 12.2F, true);
                roadLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                ShapeFileFeatureLayer roadLabelLayer = new ShapeFileFeatureLayer(Server.MapPath(@"~\SampleData\USA\Austin\austinstreets.shp"));
                roadLabelLayer.ZoomLevelSet.ZoomLevel01.DefaultTextStyle = WorldStreetsTextStyles.GeneralPurpose("FENAME",9);
                roadLabelLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                InMemoryFeatureLayer streetLayer = new InMemoryFeatureLayer();
                streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.White, 9.2F, GeoColor.StandardColors.DarkGray, 12.2F, true);
                streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Color = GeoColor.FromArgb(20, 60, 180, 60);
                streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.OuterPen.Color = GeoColor.GeographicColors.DeepOcean;
                streetLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                Map1.StaticOverlay.Layers.Add("RoadLayer", roadLayer);
                Map1.DynamicOverlay.Layers.Add("StreetLayer", streetLayer);
                Map1.DynamicOverlay.Layers.Add("RoadLabelLayer", roadLabelLayer);

                // The following two lines of code enable the client and server caching.
                // If you enable these features it will greatly increase the scalability of your
                // mapping application however there some side effects that may be counter intuitive.
                // Please read the white paper on web caching or the documentation regarding these methods.

                //Map1.StaticOverlay.ClientCache.CacheId = "WorldOverlay";
                //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path);
            }
        }

        protected void Map1_Click(object sender, MapClickedEventArgs e)
        {
            FeatureLayer worldLayer = (FeatureLayer)Map1.StaticOverlay.Layers["RoadLayer"];
            InMemoryFeatureLayer streetLayer = (InMemoryFeatureLayer)Map1.DynamicOverlay.Layers["StreetLayer"];
            streetLayer.InternalFeatures.Clear();

            worldLayer.Open();
            Collection<Feature> selectedFeatures = worldLayer.QueryTools.GetFeaturesNearestTo(e.Position, GeographyUnit.DecimalDegree, 1, new string[1] { "FENAME" });
            worldLayer.Close();

            CloudPopup popup;
            if (Map1.Popups.Count == 0)
            {
                popup = new CloudPopup("Popup", e.Position, string.Empty);
                popup.AutoSize = true;
                Map1.Popups.Add(popup);
            }
            else
            {
                popup = (CloudPopup)Map1.Popups["Popup"];
                popup.Position = e.Position;
            }

            if (selectedFeatures.Count > 0)
            {
                LineBaseShape lineShape = (LineBaseShape)selectedFeatures[0].GetShape();
                double length = lineShape.GetLength(GeographyUnit.DecimalDegree, DistanceUnit.Meter);
                string contentHtml = "<span style='color:red'>{0}</span> has a length of <span style='color:red'>{1:N0}</span> meters.";
                string information = string.Format(contentHtml, selectedFeatures[0].ColumnValues["FENAME"].Trim(), length);
                popup.ContentHtml = "<div style='font-size:10px; font-family:verdana; padding:4px;'>" + information + "</div>";

                streetLayer.InternalFeatures.Add("Street", new Feature(lineShape));
                Map1.DynamicOverlay.Redraw();
            }
        }
    }
}
