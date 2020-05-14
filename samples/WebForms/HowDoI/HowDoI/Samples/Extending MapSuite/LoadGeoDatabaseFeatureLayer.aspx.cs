using System;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples.Extending_MapSuite
{
    public partial class LoadGeoDatabaseFeatureLayer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.CurrentExtent = new RectangleShape(2149408.38465815, 246471.365609125, 2204046.63635703, 213231.081162168);
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));

                PersonalGeoDatabaseFeatureLayer worldLayer = new PersonalGeoDatabaseFeatureLayer(MapPath("~/SampleData/world/JORWD6gdb.mdb"), null, null, "Mains");
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.White, 8F, GeoColor.StandardColors.DarkGray, 10F, true);
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                Map1.StaticOverlay.Layers.Add("WorldLayer", worldLayer);
            }
        }
    }
}
