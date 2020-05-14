using System;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace HowDoI
{
    public partial class ChangeTheWidthAndColorOfALine : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.CurrentExtent = new RectangleShape(-97.745827547760484, 30.297694742808115, -97.728208518132988, 30.285123327073894);
                Map1.MapUnit = GeographyUnit.DecimalDegree;

                ShapeFileFeatureLayer streetLayer = new ShapeFileFeatureLayer(Server.MapPath("~/SampleData/USA/Austin/austinstreets.shp"));
                streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.White, 8F, GeoColor.StandardColors.DarkGray, 10F, true);
                streetLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                Map1.StaticOverlay.Layers.Add("Austin", streetLayer);

                // The following two lines of code enable the client and server caching.
                // If you enable these features it will greatly increase the scalability of your
                // mapping application however there some side effects that may be counter intuitive.
                // Please read the white paper on web caching or the documentation regarding these methods.

                //Map1.StaticOverlay.ClientCache.CacheId = "WorldOverlay";
                //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path);
            }
        }

        protected void btnWider_Click(object sender, EventArgs e)
        {
            ShapeFileFeatureLayer streetLayer = (ShapeFileFeatureLayer)Map1.StaticOverlay.Layers["Austin"];
            streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Width += 2;
            streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.OuterPen.Width += 2;

            //Map1.StaticOverlay.ClientCache.CacheId = string.Format("{0}{1}", streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Width.ToString(), GeoColor.ToHtml(streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Color));
            //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + string.Format("{0}{1}", streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Width.ToString(), GeoColor.ToHtml(streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Color)));
            Map1.StaticOverlay.Redraw();
        }

        protected void btnNarrow_Click(object sender, EventArgs e)
        {
            ShapeFileFeatureLayer streetLayer = (ShapeFileFeatureLayer)Map1.StaticOverlay.Layers["Austin"];
            if (streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.OuterPen.Width > 2)
            {
                streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Width -= 2;
                streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.OuterPen.Width -= 2;
            }

            //Map1.StaticOverlay.ClientCache.CacheId = string.Format("{0}{1}", streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Width.ToString(), GeoColor.ToHtml(streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Color));
            //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + string.Format("{0}{1}", streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Width.ToString(), GeoColor.ToHtml(streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Color)));
            Map1.StaticOverlay.Redraw();
        }

        protected void btnLineColorYellow_Click(object sender, EventArgs e)
        {
            ShapeFileFeatureLayer streetLayer = (ShapeFileFeatureLayer)Map1.StaticOverlay.Layers["Austin"];
            streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Color = GeoColor.StandardColors.LightYellow;

            //Map1.StaticOverlay.ClientCache.CacheId = string.Format("{0}{1}", streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Width.ToString(), GeoColor.ToHtml(streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Color));
            //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + string.Format("{0}{1}", streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Width.ToString(), GeoColor.ToHtml(streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Color)));
            Map1.StaticOverlay.Redraw();
        }

        protected void btnLineColorPink_Click(object sender, EventArgs e)
        {
            ShapeFileFeatureLayer streetLayer = (ShapeFileFeatureLayer)Map1.StaticOverlay.Layers["Austin"];
            streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Color = GeoColor.StandardColors.LightPink;

            //Map1.StaticOverlay.ClientCache.CacheId = string.Format("{0}{1}", streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Width.ToString(), GeoColor.ToHtml(streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Color));
            //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + string.Format("{0}{1}", streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Width.ToString(), GeoColor.ToHtml(streetLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.InnerPen.Color)));
            Map1.StaticOverlay.Redraw();
        }
    }
}
