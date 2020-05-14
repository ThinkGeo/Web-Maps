using System;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace HowDoI
{
    public partial class LoadAStandardImageWithWorldFile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapBackground = new GeoSolidBrush(GeoColor.StandardColors.White);
                Map1.CurrentExtent = new RectangleShape(-80.5, 68.9, 80, -60.43);
                Map1.MapUnit = GeographyUnit.DecimalDegree;

                NativeImageRasterLayer gdiPlusImageLayer = new NativeImageRasterLayer(MapPath(@"~\SampleData\World\World.tif"));
                gdiPlusImageLayer.UpperThreshold = double.MaxValue;
                gdiPlusImageLayer.LowerThreshold = 0;
                Map1.StaticOverlay.Layers.Add(gdiPlusImageLayer);

                // The following two lines of code enable the client and server caching.
                // If you enable these features it will greatly increase the scalability of your
                // mapping application however there some side effects that may be counter intuitive.
                // Please read the white paper on web caching or the documentation regarding these methods.

                //Map1.StaticOverlay.ClientCache.CacheId = "WorldOverlay";
                //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path);
            }
        }
    }
}
