using System;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples.DataProviders
{
    public partial class CreateAnInMemoryFeatureLayer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-51, 121, 149, -13);
                Map1.MapUnit = GeographyUnit.DecimalDegree;

                InMemoryFeatureLayer inMemoryLayer = new InMemoryFeatureLayer();
                inMemoryLayer.InternalFeatures.Add("Polygon", new Feature(BaseShape.CreateShapeFromWellKnownData("POLYGON((10 60,40 70,30 85, 10 60))")));
                inMemoryLayer.InternalFeatures.Add("Multipoint", new Feature(BaseShape.CreateShapeFromWellKnownData("MULTIPOINT(12 20, 30 20,40 20, 12 30, 30 30, 40 30)")));
                inMemoryLayer.InternalFeatures.Add("Line", new Feature(BaseShape.CreateShapeFromWellKnownData("LINESTRING(60 60, 70 70,75 60, 80 70, 85 60,95 80)")));
                inMemoryLayer.InternalFeatures.Add("Rectangle", new Feature(new RectangleShape(65, 30, 90, 15)));

                inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.FillSolidBrush.Color = GeoColor.FromArgb(100, GeoColor.StandardColors.RoyalBlue);
                inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.OutlinePen.Color = GeoColor.StandardColors.Blue;
                inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle.OuterPen = new GeoPen(GeoColor.FromArgb(200, GeoColor.StandardColors.Red), 5);
                inMemoryLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle.SymbolPen = new GeoPen(GeoColor.FromArgb(255, GeoColor.StandardColors.Green), 8);
                inMemoryLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                Map1.StaticOverlay.Layers.Add(inMemoryLayer);

                // The following two lines of code enable the client and server caching.
                // If you enable these features it will greatly increase the scalability of your
                // mapping application however there some side effects that may be counter intuitive.
                // Please read the white paper on web caching or the documentation regarding these methods.

                // Map1.StaticOverlay.ClientCache.CacheId = "InMemoryLayer";
                // Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path);
            }
        }
    }
}
