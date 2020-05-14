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
    public partial class AddAdditionalCustomPropertiesAndMethods : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.CurrentExtent = new RectangleShape(-131.22, 55.05, -54.03, 16.91);
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.MapUnit = GeographyUnit.DecimalDegree;

                AdministrationShapeFileFeatureLayer worldLayer = new AdministrationShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02.shp"), SecurityLevel.AverageUsageLevel1);
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 243, 239, 228), GeoColor.FromArgb(255, 218, 193, 163), 1);

                AdministrationShapeFileFeatureLayer statesLayer = new AdministrationShapeFileFeatureLayer(MapPath("~/SampleData/USA/STATES.SHP"), SecurityLevel.AverageUsageLevel2);
                statesLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 243, 239, 228), GeoColor.FromArgb(255, 218, 193, 163), 1);
                statesLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                LayerOverlay staticOverlay = new LayerOverlay("StaticOverlay");
                staticOverlay.IsBaseOverlay = false;
                staticOverlay.Layers.Add(worldLayer);
                staticOverlay.Layers.Add(statesLayer);

                Map1.CustomOverlays.Add(staticOverlay);             
            }
        }

        protected void ddlSecurityLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            LayerOverlay staticOverlay = (LayerOverlay)Map1.CustomOverlays["StaticOverlay"];

            foreach (Layer layer in staticOverlay.Layers)
            {
                if (layer.Name != "WorldMapKitLayer")
                {
                    layer.IsVisible = true;
                    SecurityLevel securityLevel = ((AdministrationShapeFileFeatureLayer)layer).SecurityLevel;

                    if (ddlSecurityLevel.SelectedValue == "AverageUsageLevel1" && securityLevel == SecurityLevel.AverageUsageLevel2)
                    {
                        layer.IsVisible = false;
                    }
                    else if (ddlSecurityLevel.SelectedValue == "AverageUsageLevel2" && securityLevel == SecurityLevel.AverageUsageLevel1)
                    {
                        layer.IsVisible = false;
                    }
                }
            }

            //Map1.StaticOverlay.ClientCache.CacheId = ddlSecurityLevel.SelectedValue;
            //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + ddlSecurityLevel.SelectedValue);
            staticOverlay.Redraw();    
        }
    }

    public enum SecurityLevel
    {
        AdministrativeLevel = 0,
        AverageUsageLevel1 = 1,
        AverageUsageLevel2 = 2
    }

    [Serializable]
    public class AdministrationShapeFileFeatureLayer : ShapeFileFeatureLayer
    {
        private SecurityLevel securityLevel;

        public AdministrationShapeFileFeatureLayer(string pathFileName)
            : base(pathFileName)
        {
            securityLevel = SecurityLevel.AdministrativeLevel;
        }

        public AdministrationShapeFileFeatureLayer(string pathFileName, SecurityLevel securityLevel)
            : base(pathFileName)
        {
            this.securityLevel = securityLevel;
        }

        public SecurityLevel SecurityLevel
        {
            get
            {
                return securityLevel;
            }
            set
            {
                securityLevel = value;
            }
        }
    }
}
