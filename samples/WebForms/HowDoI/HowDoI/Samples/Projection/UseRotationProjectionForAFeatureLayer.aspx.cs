using System;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace HowDoI.Samples.Projection
{
    public partial class UseRotationProjectionForAFeatureLayer : System.Web.UI.Page
    {
        private RotationProjection rotateProjection;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#94aac6"));
                Map1.MapUnit = GeographyUnit.DecimalDegree;

                rotateProjection = new RotationProjection();
                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02.shp"));
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 243, 239, 228), GeoColor.FromArgb(255, 218, 193, 163), 1);
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                worldLayer.FeatureSource.Projection = rotateProjection;
                Map1.StaticOverlay.Layers.Add(worldLayer);

                worldLayer.Open();
                Map1.CurrentExtent = worldLayer.FeatureSource.GetBoundingBox();
                worldLayer.Close();

                // The following two lines of code enable the client and server caching.
                // If you enable these features it will greatly increase the scalability of your
                // mapping application however there some side effects that may be counter intuitive.
                // Please read the white paper on web caching or the documentation regarding these methods.

                //Map1.StaticOverlay.ClientCache.CacheId = "WorldOverlay";
                //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path);
            }
        }

        protected void btnRotateCounterclockwise_Click(object sender, EventArgs e)
        {
            rotateProjection = (RotationProjection)((FeatureLayer)Map1.StaticOverlay.Layers[0]).FeatureSource.Projection;
            rotateProjection.Angle += 45;

            //Map1.StaticOverlay.ClientCache.CacheId = rotateProjection.Angle.ToString();
            //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path + "/" + rotateProjection.Angle.ToString());
            Map1.StaticOverlay.Redraw();
        }

        protected void btnRotateClockwise_Click(object sender, EventArgs e)
        {
            rotateProjection = (RotationProjection)((FeatureLayer)Map1.StaticOverlay.Layers[0]).FeatureSource.Projection;
            rotateProjection.Angle -= 45;

            //Map1.StaticOverlay.ClientCache.CacheId = rotateProjection.Angle.ToString();
            //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path + "/" + rotateProjection.Angle.ToString());
            Map1.StaticOverlay.Redraw();
        }
    }
}
