using System;
using System.Collections.ObjectModel;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace HowDoI.Samples.Styles
{
    public partial class GetAGroupOfColorsInHueFamily : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-140, 60, 140, -60);
                Map1.MapUnit = GeographyUnit.DecimalDegree;

                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02.shp"));
                worldLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 243, 239, 228), GeoColor.FromArgb(255, 218, 193, 163), 1));
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                // Draw a feature based on a value and hue family colors.
                Collection<GeoColor> colorsInFamily = GeoColor.GetColorsInHueFamily(GeoColor.StandardColors.Red, 5);
                ValueStyle valueStyle = new ValueStyle();
                valueStyle.ColumnName = "CNTRY_NAME";
                valueStyle.ValueItems.Add(new ValueItem("United States", new AreaStyle(new GeoSolidBrush(colorsInFamily[0]))));
                valueStyle.ValueItems.Add(new ValueItem("China", new AreaStyle(new GeoSolidBrush(colorsInFamily[1]))));
                valueStyle.ValueItems.Add(new ValueItem("Brazil", new AreaStyle(new GeoSolidBrush(colorsInFamily[2]))));
                valueStyle.ValueItems.Add(new ValueItem("Australia", new AreaStyle(new GeoSolidBrush(colorsInFamily[3]))));
                valueStyle.ValueItems.Add(new ValueItem("South Africa", new AreaStyle(new GeoSolidBrush(colorsInFamily[4]))));
                worldLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(valueStyle);
                Map1.StaticOverlay.Layers.Add("WorldLayer", worldLayer);

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
