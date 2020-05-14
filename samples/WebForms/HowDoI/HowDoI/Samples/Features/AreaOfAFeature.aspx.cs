/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Collections.ObjectModel;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples.Features
{
    public partial class AreaOfAFeature : System.Web.UI.Page
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

                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02_3857.shp"));
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.SimpleColors.Transparent, GeoColor.FromArgb(100, GeoColor.SimpleColors.Green));
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                LayerOverlay staticOverlay = new LayerOverlay();
                staticOverlay.IsBaseOverlay = false;
                staticOverlay.Layers.Add("worldLayer", worldLayer);
                Map1.CustomOverlays.Add(staticOverlay);
            }
        }

        protected void Map1_Click(object sender, MapClickedEventArgs e)
        {
            FeatureLayer worldLayer = (ShapeFileFeatureLayer)((LayerOverlay)Map1.CustomOverlays[1]).Layers["worldLayer"];

            // Find the country the user clicked on.
            worldLayer.Open();
            Collection<Feature> selectedFeatures = worldLayer.QueryTools.GetFeaturesContaining(e.Position, new string[1] { "CNTRY_NAME" });
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

            // Determine the area of the country.
            if (selectedFeatures.Count > 0)
            {
                AreaBaseShape areaShape = (AreaBaseShape)selectedFeatures[0].GetShape();
                Proj4Projection proj4 = new Proj4Projection(3857, 4326);
                proj4.Open();
                areaShape = (AreaBaseShape)proj4.ConvertToExternalProjection(areaShape);
                double area = areaShape.GetArea(GeographyUnit.DecimalDegree, AreaUnit.SquareKilometers);
                popup.ContentHtml = string.Format(@"<div style='color:#0065ce;font-size:10px; font-family:verdana; padding:4px;'><span style='color:red'>{0}</span> has an area of <span style='color:red'>{1:N0}</span> square kilometers.</div>", selectedFeatures[0].ColumnValues["CNTRY_NAME"], area);
            }
            else
            {
                popup.ContentHtml = @"<div style='color:#0065ce;font-size:10px; font-family:verdana; padding:4px;'>Please click on a country to see how large it is.</div>";
            }
        }
    }
}
