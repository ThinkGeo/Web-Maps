/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples.NavigateTheMap
{
    public partial class CreateARestrictionLayer1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-7124447.41076951, 7967317.53501591, 12467782.9688466, -8180386.88593525);

                // Please input your ThinkGeo Cloud API Key to enable the background map.
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                Map1.CustomOverlays.Add(backgroundOverlay);

                RestrictionLayer restrictionLayer = new RestrictionLayer();
                restrictionLayer.Zones.Add(new RectangleShape(-1967015.40231714, 4440500.74996535, 6681395.83741228, -4120479.00794152));
                restrictionLayer.RestrictionMode = RestrictionMode.ShowZones;
                restrictionLayer.UpperScale = 250000000;
                restrictionLayer.LowerScale = double.MinValue;

                LayerOverlay staticOverlay = new LayerOverlay("StaticOverlay");
                staticOverlay.IsBaseOverlay = false;
                staticOverlay.Layers.Add("RestrictionLayer", restrictionLayer);
                Map1.CustomOverlays.Add(staticOverlay);

                // The following two lines of code enable the client and server caching.
                // If you enable these features it will greatly increase the scalability of your
                // mapping application however there some side effects that may be counter intuitive.
                // Please read the white paper on web caching or the documentation regarding these methods.

                //Map1.StaticOverlay.ClientCache.CacheId = "WorldOverlay";
                //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path);
            }
        }

        protected void ddlRestrictionStyle_SelectedIndexChanged(object sender, EventArgs e)
        {
            LayerOverlay staticOverlay = (LayerOverlay)Map1.CustomOverlays["StaticOverlay"];
            RestrictionLayer currentRestrictionLayer = (RestrictionLayer)staticOverlay.Layers["RestrictionLayer"];

            if (currentRestrictionLayer.CustomStyles.Count > 0)
            {
                currentRestrictionLayer.CustomStyles.Clear();
            }

            switch (ddlRestrictionStyle.SelectedItem.ToString())
            {
                case "HatchPattern":
                    currentRestrictionLayer.RestrictionStyle = RestrictionStyle.HatchPattern;
                    break;
                case "CircleWithSlashImage":
                    currentRestrictionLayer.RestrictionStyle = RestrictionStyle.CircleWithSlashImage;
                    break;
                case "UseCustomStyles":
                    currentRestrictionLayer.RestrictionStyle = RestrictionStyle.UseCustomStyles;
                    Style customStyle = new AreaStyle(new GeoSolidBrush(new GeoColor(150, GeoColor.StandardColors.Gray)));
                    currentRestrictionLayer.CustomStyles.Add(customStyle);
                    break;
                default:
                    break;
            }

            //Map1.StaticOverlay.ClientCache.CacheId = String.Format("{0}{1}", radHideZones.Text, ddlRestrictionStyle.SelectedItem.Value);
            //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + String.Format("{0}{1}", radHideZones.Text, ddlRestrictionStyle.SelectedItem.Value));
        }

        protected void radShowZones_CheckedChanged(object sender, EventArgs e)
        {
            LayerOverlay staticOverlay = (LayerOverlay)Map1.CustomOverlays["StaticOverlay"];
            RestrictionLayer currentRestrictionLayer = (RestrictionLayer)staticOverlay.Layers["RestrictionLayer"];
            currentRestrictionLayer.RestrictionMode = RestrictionMode.ShowZones;
            lbInfomation.Text = "You can see only Africa because we have added a RestrictionLayer and its mode is ShowZones.";

            //Map1.StaticOverlay.ClientCache.CacheId = String.Format("{0}{1}", radShowZones.Text, ddlRestrictionStyle.SelectedItem.Value);
            //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + String.Format("{0}{1}", radShowZones.Text, ddlRestrictionStyle.SelectedItem.Value));
        }

        protected void radHideZones_CheckedChanged(object sender, EventArgs e)
        {
            LayerOverlay staticOverlay = (LayerOverlay)Map1.CustomOverlays["StaticOverlay"];
            RestrictionLayer currentRestrictionLayer = (RestrictionLayer)staticOverlay.Layers["RestrictionLayer"];
            currentRestrictionLayer.RestrictionMode = RestrictionMode.HideZones;
            lbInfomation.Text = "You can not see Africa because we have added a RestrictionLayer and its mode is HideZones.";

            //Map1.StaticOverlay.ClientCache.CacheId = String.Format("{0}{1}", radHideZones.Text, ddlRestrictionStyle.SelectedItem.Value);
            //Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + String.Format("{0}{1}", radHideZones.Text, ddlRestrictionStyle.SelectedItem.Value));
        }
    }
}
