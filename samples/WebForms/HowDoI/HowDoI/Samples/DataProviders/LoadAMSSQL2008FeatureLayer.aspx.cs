using System;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples.DataProviders
{
    public partial class LoadAMSSQL2008FeatureLayer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                Map1.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
                Map1.CurrentExtent = new RectangleShape(-140, 60, 140, -60);
                Map1.MapUnit = GeographyUnit.DecimalDegree;

                // The following two lines of code enable the client and server caching.
                // If you enable these features it will greatly increase the scalability of your
                // mapping application however there some side effects that may be counter intuitive.
                // Please read the white paper on web caching or the documentation regarding these methods.

                // Map1.StaticOverlay.ClientCache.CacheId = "WorldOverlay";
                // Map1.StaticOverlay.ServerCache.CacheDirectory = MapPath("~/ImageCache/" + Request.Path);

                // Please modify the visible property of the map in .aspx file if you load the map from MsSQL2008
                // using following code.
                
                // LoadAMapFromMsSQL2008();
            }
        }

        private void LoadAMapFromMsSQL2008()
        {
            string connectString = "User ID=userid;Password=password;Data Source=192.168.0.178/orcl;";
            MsSqlFeatureLayer sql2008Layer = new MsSqlFeatureLayer(connectString, "states", "recid");
            sql2008Layer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 233, 232, 214), GeoColor.FromArgb(255, 118, 138, 69));
            sql2008Layer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            Map1.StaticOverlay.Layers.Add("Sql2008Layer", sql2008Layer);
        }
    }
}