using System;
using System.IO;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;
 
namespace HowDoI.Samples.Extending_MapSuite
{
    public partial class LoadSdfFeatureLayer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.DecimalDegree;
                Map1.CurrentExtent = new RectangleShape(-87.7649869909628, 43.7975200004804, -87.6955215108997, 43.6913981287878);
                Map1.MapBackground = new GeoSolidBrush(GeoColor.GeographicColors.ShallowOcean);
                
                WorldMapKitWmsWebOverlay worldMapKitOverlay = new WorldMapKitWmsWebOverlay();
                Map1.CustomOverlays.Add(worldMapKitOverlay);

                SdfFeatureLayer worldLayer = new SdfFeatureLayer(MapPath("~/SampleData/world/Sheboygan_CityLimits.sdf"), null);
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(100, GeoColor.SimpleColors.Green), GeoColor.SimpleColors.Green);
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                LayerOverlay staticOverlay = new LayerOverlay();
                staticOverlay.IsBaseOverlay = false;
                staticOverlay.Layers.Add("WorldLayer", worldLayer);
                Map1.CustomOverlays.Add(staticOverlay);
            }
        }
    }
}