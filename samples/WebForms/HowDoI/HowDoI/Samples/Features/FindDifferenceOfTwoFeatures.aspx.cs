/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples.Features
{
    public partial class FindDifferenceOfTwoFeatures : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-7219610.047343, 9368012.7830705, 8992377.8137632, 963608.69574991);

                // Please input your ThinkGeo Cloud API Key to enable the background map. 
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                Map1.CustomOverlays.Add(backgroundOverlay);

                InMemoryFeatureLayer mapShapeLayer = new InMemoryFeatureLayer();
                mapShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = new AreaStyle(new GeoSolidBrush(new GeoColor(50, 100, 100, 200)));
                mapShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.StandardColors.RoyalBlue;
                mapShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                BaseShape areaShape1 = new RectangleShape(-4452779.63173094, 4865942.27950318, 0, 0);
                areaShape1.Id = "AreaShape1";
                BaseShape areaShape2 = new RectangleShape(-2226389.81586547, 11068715.6593795, 3339584.72379821, 2273030.92698769);
                areaShape2.Id = "AreaShape2";
                mapShapeLayer.InternalFeatures.Add("AreaShape1", new Feature(areaShape1));
                mapShapeLayer.InternalFeatures.Add("AreaShape2", new Feature(areaShape2));

                LayerOverlay dynamicOverlay = new LayerOverlay();
                dynamicOverlay.TileType = TileType.SingleTile;
                dynamicOverlay.Layers.Add("InMemoryFeatureLayer", mapShapeLayer);
                dynamicOverlay.IsBaseOverlay = false;

                Map1.CustomOverlays.Add(dynamicOverlay);
            }
        }

        protected void btnDifference_Click(object sender, EventArgs e)
        {
            InMemoryFeatureLayer mapShapeLayer = (InMemoryFeatureLayer)((LayerOverlay)Map1.CustomOverlays[1]).Layers["InMemoryFeatureLayer"];

            if (mapShapeLayer.InternalFeatures.Count > 1)
            {
                AreaBaseShape targetShape = (AreaBaseShape)mapShapeLayer.InternalFeatures["AreaShape1"].GetShape();

                mapShapeLayer.Open();
                mapShapeLayer.EditTools.BeginTransaction();
                mapShapeLayer.EditTools.GetDifference("AreaShape2", targetShape);
                mapShapeLayer.EditTools.Delete("AreaShape1");
                mapShapeLayer.EditTools.CommitTransaction();
                mapShapeLayer.Close();
                mapShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.FromArgb(100, GeoColor.StandardColors.Blue);
            }
            ((LayerOverlay)Map1.CustomOverlays[1]).Redraw();
        }
    }
}
