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
    public partial class GetColumnData : System.Web.UI.Page
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
                staticOverlay.Layers.Add(worldLayer);
                Map1.CustomOverlays.Add(staticOverlay);

                PopulateTable(worldLayer);
            }
        }

        private void PopulateTable(FeatureLayer featureLayer)
        {
            featureLayer.Open();
            Collection<FeatureSourceColumn> allColumns = featureLayer.QueryTools.GetColumns();
            featureLayer.Close();

            FeaturesGridView.DataSource = GetDataTableFromFeatureSourceColumns(allColumns);
            FeaturesGridView.DataBind();
        }

        private static DataTable GetDataTableFromFeatureSourceColumns(Collection<FeatureSourceColumn> featureSourceColumns)
        {
            DataTable dataTable = new DataTable();

            dataTable.Columns.Add("ColumnName");
            dataTable.Columns.Add("MaxLength");
            dataTable.Columns.Add("TypeName");

            foreach (FeatureSourceColumn column in featureSourceColumns)
            {
                AddRow(column, dataTable);
            }

            return dataTable;
        }

        private static void AddRow(FeatureSourceColumn featureSourceColumn, DataTable dataTable)
        {
            DataRow dataRow = dataTable.NewRow();

            dataRow[0] = featureSourceColumn.ColumnName;
            dataRow[1] = featureSourceColumn.MaxLength;
            dataRow[2] = featureSourceColumn.TypeName;

            dataTable.Rows.Add(dataRow);
        }
    }
}
