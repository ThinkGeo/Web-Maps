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
    public partial class DistanceBetweenTwoPoints : System.Web.UI.Page
    {
        private readonly PointShape fromPoint = new PointShape(-77.00, 38.90);
        private readonly PointShape toPoint = new PointShape(-0.110378, 51.497309);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-8956259.3203343, 7303601.534613, 7255728.5407719, -1100802.5527076);

                // Please input your ThinkGeo Cloud API Key to enable the background map. 
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                Map1.CustomOverlays.Add(backgroundOverlay);


                InMemoryFeatureLayer mapShapeLayer = new InMemoryFeatureLayer();
                mapShapeLayer.FeatureSource.Projection = new Proj4Projection(4326, 3857);
                mapShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateCompoundCircleStyle(GeoColor.StandardColors.White, 8F, GeoColor.StandardColors.Black, 1F, GeoColor.StandardColors.Black, 5F);
                mapShapeLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.OliveDrab, 3, true);
                mapShapeLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                mapShapeLayer.InternalFeatures.Add("US", new Feature(fromPoint));
                mapShapeLayer.InternalFeatures.Add("BRITAN", new Feature(toPoint));

                LayerOverlay dynamicOverlay = new LayerOverlay();
                dynamicOverlay.TileType = TileType.SingleTile;
                dynamicOverlay.IsBaseOverlay = false;
                dynamicOverlay.Layers.Add("InMemoryFeatureLayer", mapShapeLayer);
                Map1.CustomOverlays.Add(dynamicOverlay);
            }
        }

        protected void btnGetDistance_Click(object sender, EventArgs e)
        {
            InMemoryFeatureLayer mapShapeLayer = (InMemoryFeatureLayer)((LayerOverlay)(Map1.CustomOverlays[1])).Layers["InMemoryFeatureLayer"];

            if (!mapShapeLayer.InternalFeatures.Contains("Distance"))
            {
                MultilineShape line = fromPoint.GetShortestLineTo(toPoint, GeographyUnit.DecimalDegree);
                mapShapeLayer.InternalFeatures.Add("Distance", new Feature(line));
            }
            ((LayerOverlay)Map1.CustomOverlays[1]).Redraw();
            double distance = fromPoint.GetDistanceTo(toPoint, GeographyUnit.DecimalDegree, DistanceUnit.Kilometer);
            DistanceLabel.Text = string.Format("The distance between the two points is <span style='color:red'>{0:N4}</span> km.", distance);
        }
    }
}
