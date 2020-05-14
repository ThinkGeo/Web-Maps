/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Text;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI
{
    public partial class SetTheHoverPopupOfAMarker : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapUnit = GeographyUnit.Meter;
                Map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-13064290.945669, 6003560.5647606, -9011293.9803927, 3902459.5429304);

                // Please input your ThinkGeo Cloud API Key to enable the background map.
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                Map1.CustomOverlays.Add(backgroundOverlay);

                StringBuilder contentHtml = new StringBuilder();
                contentHtml.Append("<div style='padding:10px; font-size:10px; font-family:verdana;'><img alt='' align='left'")
                    .Append("src='../../theme/default/samplepic/lawrencecity.jpg'/>&nbsp;&nbsp;Lawrence is a city in Northeastern Kansas")
                    .Append("in the United States. Lawrence serves as the county seat of Douglas County, Kansas. Located 41 miles west ")
                    .Append("of Kansas City, Lawrence is situated along the banks of the Kansas (Kaw) and Wakarusa Rivers. It is considered ")
                    .Append("governmentally independent and is the principal city within the Lawrence, Kansas, Metropolitan Statistical Area, ")
                    .Append("which encompasses all of Douglas County. As of the 2000 census, the city had a population of 80,098, making it ")
                    .Append("the sixth largest city in Kansas. 2006 estimates[3] place the city's population at 89,110. A quintessential")
                    .Append("college town, Lawrence is home to The University of Kansas and Haskell Indian Nations University.<br/>")
                    .Append("&nbsp;&nbsp;<a href='http://en.wikipedia.org/wiki/Lawrence%2C_Kansas' ")
                    .Append("target='_blank'><br/>more about Lawrence city...</a></div>");

                InMemoryMarkerOverlay markerOverlay = new InMemoryMarkerOverlay("MarkerOverlay");
                markerOverlay.FeatureSource.InternalFeatures.Add("Lawrence", new Feature(-10606642.4210281, 4715031.64277965));
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage = new WebImage("../../theme/default/img/marker_green.gif", 21, 25);
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.ContentHtml = contentHtml.ToString();
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.Width = 450;
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.Height = 290;
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.IsVisible = false;
                markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.AutoPan = true;
                markerOverlay.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
                Map1.CustomOverlays.Add(markerOverlay);
            }
        }
    }
}
