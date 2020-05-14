using System;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace HowDoI
{
    public partial class ChangeTheFillAndOutlineColor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-131.22, 55.05, -54.03, 16.91);
                Map1.MapUnit = GeographyUnit.DecimalDegree;

                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02.shp"));
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 243, 239, 228), GeoColor.FromArgb(255, 218, 193, 163), 1);
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                Map1.StaticOverlay.Layers.Add("WorldLayer", worldLayer);

                // The following two lines of code enable the client and server caching.
                // If you enable these features it will greatly increase the scalability of your
                // mapping application however there some side effects that may be counter intuitive.
                // Please read the white paper on web caching or the documentation regarding these methods.

                //Map1.StaticOverlay.ClientCache.CacheId = "WorldOverlay";
                //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path);
            }
        }

        protected void btnFillColorYellow_Click(object sender, EventArgs e)
        {
            ShapeFileFeatureLayer worldLayer = (ShapeFileFeatureLayer)Map1.StaticOverlay.Layers["WorldLayer"];
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.StandardColors.LightGoldenrodYellow;

            //Map1.StaticOverlay.ClientCache.CacheId = String.Format("{0}{1}", "LightGoldenrodYellow", GeoColor.ToHtml(worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color));
            //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + String.Format("{0}{1}", "/LightGoldenrodYellow", GeoColor.ToHtml(worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color)));
            Map1.StaticOverlay.Redraw();
        }

        protected void btnFillColorGreen_Click(object sender, EventArgs e)
        {
            ShapeFileFeatureLayer worldLayer = (ShapeFileFeatureLayer)Map1.StaticOverlay.Layers["WorldLayer"];
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.StandardColors.LightGreen;

            //Map1.StaticOverlay.ClientCache.CacheId = String.Format("{0}{1}", "LightGreen", GeoColor.ToHtml(worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color));
            //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + String.Format("{0}{1}", "/LightGreen", GeoColor.ToHtml(worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color)));
            Map1.StaticOverlay.Redraw();
        }

        protected void btnOutLineColorGray_Click(object sender, EventArgs e)
        {
            ShapeFileFeatureLayer worldLayer = (ShapeFileFeatureLayer)Map1.StaticOverlay.Layers["WorldLayer"];
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.StandardColors.Gray;

            //Map1.StaticOverlay.ClientCache.CacheId = String.Format("{0}{1}", "Gray", String.Format("{0}{1}", "Black", GeoColor.ToHtml(worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color)));
            //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + String.Format("{0}{1}", "/Gray", String.Format("{0}{1}", "Black", GeoColor.ToHtml(worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color))));
            Map1.StaticOverlay.Redraw();
        }

        protected void btnOutlineColorBlack_Click(object sender, EventArgs e)
        {
            ShapeFileFeatureLayer worldLayer = (ShapeFileFeatureLayer)Map1.StaticOverlay.Layers["WorldLayer"];
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.StandardColors.Black;

            //Map1.StaticOverlay.ClientCache.CacheId = String.Format("{0}{1}", "Black", GeoColor.ToHtml(worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color));
            //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + String.Format("{0}{1}", "/Black", GeoColor.ToHtml(worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color)));
            Map1.StaticOverlay.Redraw();
        }
    }
}
