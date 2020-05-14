using System;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;

namespace HowDoI.Samples.Extending_MapSuite
{
    public partial class UseMicrosoftMapsLayer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                //Map1.MapUnit = GeographyUnit.Meter;
                //Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                //Map1.CurrentScale = Map1.ClientZoomLevelScales[3];

                //string applicationID = "Your ApplicationID";
                //string cacheDirectory = @"c:\temp\";
                //BingMapsLayer worldLayer = new BingMapsLayer(applicationID, BingMapsMapType.Road, cacheDirectory);

                //Map1.StaticOverlay.Layers.Add("WorldLayer", worldLayer);
            }
        }
    }
}
