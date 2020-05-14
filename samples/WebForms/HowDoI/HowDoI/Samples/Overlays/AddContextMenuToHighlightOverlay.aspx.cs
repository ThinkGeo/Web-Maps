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
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI
{
    public partial class AddContextMenuToHighlightOverlay : System.Web.UI.Page
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

                // Add ContextMenu to EventLayer
                ContextMenu menuOnEventLayer = new ContextMenu("area", 200);
                ContextMenuItem menu1 = new ContextMenuItem("<a href='http://thinkgeo.com/About/tabid/524/Default.aspx' target='_blank'>About ThinkGeo<a>");
                ContextMenuItem menu2 = new ContextMenuItem("click me");
                menu2.Click += new EventHandler<ContextMenuItemClickEventArgs>(menu2_Click);
                menuOnEventLayer.MenuItems.Add(menu1);
                menuOnEventLayer.MenuItems.Add(menu2);

                Feature feature = new Feature(new RectangleShape(-12269634.2752346, 6446275.84101716, -8908898.84818568, 3503549.84350437));
                Map1.HighlightOverlay.Features.Add("multipolygon", feature);
                Map1.HighlightOverlay.ContextMenu = menuOnEventLayer;
                Map1.HighlightOverlay.HighlightStyle = Map1.HighlightOverlay.Style;
            }
        }

        void menu2_Click(object sender, ContextMenuItemClickEventArgs e)
        {
            string content = String.Format("<span class='popup'>Clicked Location<br/>({0}, {1})</span>", e.Location.X, e.Location.Y);
            if (Map1.Popups.Contains("DefaultPopup"))
            {
                CloudPopup popup = (CloudPopup)Map1.Popups["DefaultPopup"];
                popup.Position = e.Location;
                popup.ContentHtml = String.Format(content, e.Location.X, e.Location.Y);
            }
            else
            {
                CloudPopup popup = new CloudPopup("DefaultPopup", e.Location, String.Format(content, e.Location.X, e.Location.Y));
                popup.AutoSize = true;
                Map1.Popups.Add(popup);
            }
        }
    }
}
