using System;
using System.Drawing;
using System.Web.UI.WebControls;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace HowDoI.Samples
{
    public partial class SetBackgroundColor : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#94aac6"));
                Map1.CurrentExtent = new RectangleShape(-140, 60, 140, -60);
                Map1.MapUnit = GeographyUnit.DecimalDegree;

                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02.shp"));
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 243, 239, 228), GeoColor.FromArgb(255, 218, 193, 163), 1);
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                Map1.StaticOverlay.Layers.Add(worldLayer);

                // The following two lines of code enable the client and server caching.
                // If you enable these features it will greatly increase the scalability of your
                // mapping application however there some side effects that may be counter intuitive.
                // Please read the white paper on web caching or the documentation regarding these methods.

                //Map1.StaticOverlay.ClientCache.CacheId = "WorldOverlay";
                //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path);
            }
        }

        protected void ChangeBgColor(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Color backgroundColor = button.BackColor;
            Map1.MapBackground = new GeoSolidBrush(GeoColor.FromArgb(backgroundColor.A, backgroundColor.R, backgroundColor.G, backgroundColor.B));

            //Map1.StaticOverlay.ClientCache.CacheId = "WorldOverlay" + backgroundColor.Name;
            //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path + "/" + backgroundColor.Name);
            Map1.StaticOverlay.Redraw();
        }
    }
}
