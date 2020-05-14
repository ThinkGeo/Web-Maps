using System;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
 

namespace HowDoI
{
    public partial class ConvertAGeoColorToAndFromOleWin32HtmlArgbColors : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-140, 60, 140, -60);
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

                ddlGeoColors_SelectedIndexChanged(ddlGeoColors, EventArgs.Empty);
            }
        }

        protected void ddlGeoColors_SelectedIndexChanged(object sender, EventArgs e)
        {
            string geoColorName = ddlGeoColors.SelectedValue.ToString();
            GeoColor geoColor = GeoColor.GeographicColors.ShallowOcean;
            switch (geoColorName)
            {
                case "ShallowOcean":
                    geoColor = GeoColor.GeographicColors.ShallowOcean;
                    break;
                case "Sand":
                    geoColor = GeoColor.GeographicColors.Sand;
                    break;
                case "Lake":
                    geoColor = GeoColor.GeographicColors.Lake;
                    break;
                case "Silver":
                    geoColor = GeoColor.SimpleColors.Silver;
                    break;
                case "Green":
                    geoColor = GeoColor.SimpleColors.Green;
                    break;
                case "Transparent":
                    geoColor = GeoColor.StandardColors.Transparent;
                    break;
                default:
                    break;
            }
            txtArgb.Text = string.Format("A:{0}  R:{1}  G:{2}  B:{3}", geoColor.AlphaComponent, geoColor.RedComponent, geoColor.GreenComponent, geoColor.BlueComponent);
            txtHTML.Text = GeoColor.ToHtml(geoColor);
            txtOLE.Text = GeoColor.ToOle(geoColor).ToString();
            txtWin32.Text = GeoColor.ToWin32(geoColor).ToString();

            Map1.MapBackground = new GeoSolidBrush(geoColor);
            Map1.StaticOverlay.ClientCache.CacheId = geoColorName;

            //Map1.StaticOverlay.ClientCache.CacheId = geoColorName;
            //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + geoColorName);
        }
    }
}
