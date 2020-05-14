/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Web.UI;
using ThinkGeo.MapSuite.Geocoding;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.WebForms;

namespace ThinkGeo.MapSuite.HowDoI
{
    public partial class BuildIndexFile : System.Web.UI.Page
    {
        private static string dataPath;

        protected void Page_Load(object sender, EventArgs e)
        {
            dataPath = this.GetDataPath("CreateIndexFileData/citystate.txt");

            if (!Page.IsPostBack)
            {
                BindDataGrid();

                // Setup the map unit and set the Texas extent
                map1.MapUnit = GeographyUnit.Meter;
                map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                map1.CurrentExtent = new RectangleShape(-13086298.60, 7339062.72, -8111177.75, 2853137.62);

                // Please input your ThinkGeo Cloud API Key to enable the background map.
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                map1.BackgroundOverlay = backgroundOverlay;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {

            // Define a string list and add all lines of source data into it
            List<string> cityList = new List<string>();
            cityList.AddRange(File.ReadAllLines(dataPath));

            // Sort the key column To perform the matching high efficiently.
            cityList.Sort(delegate (string lineA, string lineB)
            { return string.Compare(lineA.Split('|')[0], lineB.Split('|')[0], StringComparison.OrdinalIgnoreCase); });

            // Create the MatchDbf object and add records into it
            DbfMatchingPlugin cityDbfMatchingPlugIn = CreateCityDbfMatchingPlugIn();
            try
            {
                cityDbfMatchingPlugIn.Open();

                for (int i = 0; i < cityList.Count; i++)
                {
                    string[] parts = cityList[i].Split('|');
                    string city = parts[0];
                    string state = parts[1];
                    int bb_Cx = int.Parse(parts[2]);
                    int bb_Cy = int.Parse(parts[3]);
                    int bb_Ulx = int.Parse(parts[4]);
                    int bb_Uly = int.Parse(parts[5]);
                    int bb_Lrx = int.Parse(parts[6]);
                    int bb_Lry = int.Parse(parts[7]);

                    cityDbfMatchingPlugIn.AddRecord(new object[] { city, state, bb_Cx, bb_Cy, bb_Ulx, bb_Uly, bb_Lrx, bb_Lry });
                }
            }
            finally
            {
                cityDbfMatchingPlugIn.Close();
            }
            ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "messageBox", "window.alert('Build index file done!')", true);
        }

        private DbfMatchingPlugin CreateCityDbfMatchingPlugIn()
        {
            // Define a Collection of DbfColumnDefinition
            Collection<DbfMatchingPluginColumn> columns = new Collection<DbfMatchingPluginColumn>();

            // Add necessary columns into the collection
            columns.Add(new DbfMatchingPluginColumn("ID_City", ThinkGeo.MapSuite.Geocoding.DbfMatchingPluginColumnType.String, 54));
            columns.Add(new DbfMatchingPluginColumn("DT_State", ThinkGeo.MapSuite.Geocoding.DbfMatchingPluginColumnType.String, 2));
            columns.Add(new DbfMatchingPluginColumn("BB_CX", ThinkGeo.MapSuite.Geocoding.DbfMatchingPluginColumnType.Long, 4));
            columns.Add(new DbfMatchingPluginColumn("BB_CY", ThinkGeo.MapSuite.Geocoding.DbfMatchingPluginColumnType.Long, 4));
            columns.Add(new DbfMatchingPluginColumn("BB_ULX", ThinkGeo.MapSuite.Geocoding.DbfMatchingPluginColumnType.Long, 4));
            columns.Add(new DbfMatchingPluginColumn("BB_ULY", ThinkGeo.MapSuite.Geocoding.DbfMatchingPluginColumnType.Long, 4));
            columns.Add(new DbfMatchingPluginColumn("BB_LRX", ThinkGeo.MapSuite.Geocoding.DbfMatchingPluginColumnType.Long, 4));
            columns.Add(new DbfMatchingPluginColumn("BB_LRY", ThinkGeo.MapSuite.Geocoding.DbfMatchingPluginColumnType.Long, 4));

            // Create the MatchDbf object according to the path and the collection of DbfColumnDefinition
            string filePath = Path.ChangeExtension(dataPath, ".dbf");
            DbfMatchingPlugin.CreateDbf(filePath, columns);
            DbfMatchingPlugin matchingPlugIn = new DbfMatchingPlugin(filePath, DbfMatchingPluginReadWriteMode.ReadWrite);
            return matchingPlugIn;
        }

        private void BindDataGrid()
        {
            string[] allLines = File.ReadAllLines(dataPath);
            DataTable table = new DataTable();
            table.Columns.Add(new DataColumn("ID_City"));
            table.Columns.Add(new DataColumn("DT_State"));
            foreach (string item in allLines)
            {
                //dataGridViewDetail.Rows.Add(new object[] { item.Split('|')[0], item.Split('|')[1] });
                table.Rows.Add(new object[] { item.Split('|')[0], item.Split('|')[1] });
            }

            dataGridViewDetail.DataSource = table;
            dataGridViewDetail.DataBind();
        }
    }
}
