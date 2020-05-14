/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI
{
    public partial class GetTheDistanceBetweenTwoPointsClicked : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-13064290.945669, 6003560.5647606, -9011293.9803927, 3902459.5429304);

                // Please input your ThinkGeo Cloud API Key to enable the background map.
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                Map1.CustomOverlays.Add(backgroundOverlay);

                InMemoryFeatureLayer pointShapeLayer = new InMemoryFeatureLayer();
                pointShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimplePointStyle(PointSymbolType.Circle, GeoColor.StandardColors.Red, 8);
                pointShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                InMemoryFeatureLayer lineShapeLayer = new InMemoryFeatureLayer();
                lineShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = new LineStyle(new GeoPen(GeoColor.StandardColors.Red, 3));
                lineShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                LayerOverlay dynamicOverlay = new LayerOverlay("DynamicOverlay");
                dynamicOverlay.IsBaseOverlay = false;
                dynamicOverlay.TileType = TileType.SingleTile;
                dynamicOverlay.Layers.Add("pointShapeLayer", pointShapeLayer);
                dynamicOverlay.Layers.Add("lineShapeLayer", lineShapeLayer);
                Map1.CustomOverlays.Add(dynamicOverlay);
            }
        }

        protected void Map1_Click(object sender, MapClickedEventArgs e)
        {
            LayerOverlay dynamicOverlay = (LayerOverlay)Map1.CustomOverlays["DynamicOverlay"];
            InMemoryFeatureLayer pointShapeLayer = (InMemoryFeatureLayer)dynamicOverlay.Layers["pointShapeLayer"];
            Feature newPoint = new Feature(e.Position);
            InMemoryFeatureLayer lineShapeLayer = (InMemoryFeatureLayer)dynamicOverlay.Layers["lineShapeLayer"];

            if (ViewState["StartPoint"] == null)
            {
                pointShapeLayer.InternalFeatures.Clear();
                lineShapeLayer.InternalFeatures.Clear();
            }

            pointShapeLayer.InternalFeatures.Add(newPoint.Id, newPoint);

            if (ViewState["StartPoint"] != null)
            {
                Proj4Projection proj4 = new Proj4Projection(3857, 4326);
                proj4.Open();

                PointShape startPoint = (PointShape)ViewState["StartPoint"];
                startPoint = (PointShape)proj4.ConvertToExternalProjection(startPoint);
                PointShape endPoint = (PointShape)proj4.ConvertToExternalProjection(e.Position);
                MultilineShape line = startPoint.GetShortestLineTo(endPoint, GeographyUnit.DecimalDegree);

                Feature lineFeature = new Feature(line);
                string distanceValue = String.Format("<span class='popup'>{0} Mile</span>", line.GetLength(GeographyUnit.DecimalDegree, DistanceUnit.Mile).ToString("N2"));
                lineShapeLayer.InternalFeatures.Add(lineFeature.Id, lineFeature);

                CloudPopup popup;
                if (Map1.Popups.Contains("DistancePopup"))
                {
                    popup = (CloudPopup)Map1.Popups["DistancePopup"];
                    popup.Position = e.Position;
                    popup.ContentHtml = distanceValue;
                    popup.IsVisible = true;
                }
                else
                {
                    popup = new CloudPopup("DistancePopup", e.Position, distanceValue);
                    Map1.Popups.Add(popup);
                    popup.AutoSize = true;
                }

                ViewState["StartPoint"] = null;
            }
            else
            {
                ViewState["StartPoint"] = e.Position;

                if (Map1.Popups.Contains("DistancePopup"))
                {
                    Map1.Popups["DistancePopup"].IsVisible = false;
                }
            }

            dynamicOverlay.Redraw();
        }
    }
}
