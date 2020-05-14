using System;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples.Extending_MapSuite
{
    public partial class DrawUsingFleeBooleanStyle : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.DecimalDegree;
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-153.48592410231, 84.290584806933, 137.78360714769, -66.705508943067);

                // Highlight the countries that are land locked and have a population greater than 10 million
                string expression = "(ToInt32(POP_CNTRY)>10000000) AND (ToChar(LANDLOCKED)='Y')";
                FleeBooleanStyle landLockedCountryStyle = new FleeBooleanStyle(expression);

                // You can access the static methods on these types.  We use this
                // to access the Convert.Toxxx methods to convert variable types
                landLockedCountryStyle.StaticTypes.Add(typeof(System.Convert));

                // The math class might be handy to include but in this sample we do not use it
                //landLockedCountryStyle.StaticTypes.Add(typeof(System.Math));

                landLockedCountryStyle.ColumnVariables.Add("POP_CNTRY");
                landLockedCountryStyle.ColumnVariables.Add("LANDLOCKED");

                landLockedCountryStyle.CustomTrueStyles.Add(new AreaStyle( new GeoPen(GeoColor.FromArgb(255, 118, 138, 69),1), new GeoSolidBrush(GeoColor.SimpleColors.Yellow)));
                landLockedCountryStyle.CustomFalseStyles.Add(AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69)));

                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02.shp"), GeoFileReadWriteMode.Read);
                worldLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add(landLockedCountryStyle);
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                LayerOverlay staticOverlay = new LayerOverlay("StaticOverlay");
                staticOverlay.Layers.Add(worldLayer);

                Map1.CustomOverlays.Add(staticOverlay);
            }
        }
    }
}
