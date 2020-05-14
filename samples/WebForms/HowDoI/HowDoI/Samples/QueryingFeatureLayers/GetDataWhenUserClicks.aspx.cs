/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Web.UI;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebForms;

namespace HowDoI.Samples.Querying_Vector_Layers
{
    public partial class GetDataWhenUserClicks : System.Web.UI.Page
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

                ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(MapPath("~/SampleData/world/cntry02_3857.shp"));
                worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.SimpleColors.Transparent, GeoColor.FromArgb(100, GeoColor.SimpleColors.Green));
                worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

                LayerOverlay staticOverlay = new LayerOverlay("StaticOverlay");
                staticOverlay.IsBaseOverlay = false;
                staticOverlay.Layers.Add("WorldLayer", worldLayer);
                Map1.CustomOverlays.Add(staticOverlay);
            }
        }

        protected void Map1_Click(object sender, MapClickedEventArgs e)
        {
            LayerOverlay staticOverlay = (LayerOverlay)Map1.CustomOverlays["StaticOverlay"];
            FeatureLayer worldLayer = (FeatureLayer)staticOverlay.Layers["WorldLayer"];
            worldLayer.Open();
            Collection<Feature> selectedFeatures = worldLayer.QueryTools.GetFeaturesContaining(e.Position, new string[2] { "CNTRY_NAME", "POP_CNTRY" });
            worldLayer.Close();

            if (selectedFeatures.Count > 0)
            {
                FeaturesGridView.DataSource = GetDataTableFromFeatureSourceColumns(selectedFeatures);
                FeaturesGridView.DataBind();
            }
        }

        private static DataTable GetDataTableFromFeatureSourceColumns(Collection<Feature> features)
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("CountryName");
            dataTable.Columns.Add("Population");

            foreach (Feature feature in features)
            {
                AddRow(feature, dataTable);
            }

            return dataTable;
        }

        private static void AddRow(Feature feature, DataTable dataTable)
        {
            DataRow dataRow = dataTable.NewRow();

            dataRow[0] = feature.ColumnValues["CNTRY_NAME"];
            dataRow[1] = feature.ColumnValues["POP_CNTRY"];

            dataTable.Rows.Add(dataRow);
        }
    }
}
