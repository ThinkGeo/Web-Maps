/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Web.UI;
using ThinkGeo.MapSuite.Geocoding;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.WebForms;

namespace ThinkGeo.MapSuite.HowDoI
{
    public partial class CreateDatabaseMatchingPlugIn : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                // Setup the map unit and set the Texas extent
                map1.MapUnit = GeographyUnit.Meter;
                map1.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
                map1.CurrentExtent = new RectangleShape(-13086298.60, 7339062.72, -8111177.75, 2853137.62);

                // Please input your ThinkGeo Cloud API Key to enable the background map.
                ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
                map1.BackgroundOverlay = backgroundOverlay;

                // Setup the marker layer and add it to the map            
                MarkerLayer markerLayer = new MarkerLayer(this.GetMarkerPath());
                map1.DynamicOverlay.Layers.Add(markerLayer);

            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // Create a generic geocoder that will house the match plugin
            var dataPath = this.GetDataPath("AccessMatchingPlugin");
            Geocoder geocoder = new Geocoder(dataPath);

            // Create our Text matching plugin and add it to the geocoder
            AccessMatchingPlugIn accessMatchingPlugin = new AccessMatchingPlugIn("county", Path.Combine(dataPath, "County.mdb"));
            geocoder.MatchingPlugIns.Add(accessMatchingPlugin);

            //Open the geocoder and call the match method then close it
            Collection<GeocoderMatch> matchResult;
            try
            {
                geocoder.Open();
                matchResult = geocoder.Match(cboSourceText.Text);
            }
            finally
            {
                geocoder.Close();
            }

            PopulateAddressResultList(matchResult);
        }

        private void PopulateAddressResultList(Collection<GeocoderMatch> matchResult)
        {
            // Clear the results
            lstResult.Items.Clear();
            dataGridViewDetail.DataSource = null;

            // Load the matching items into the grid

            foreach (GeocoderMatch matchItem in matchResult)
            {
                if (matchItem.NameValuePairs.ContainsKey("County") && matchItem.NameValuePairs.ContainsKey("State"))
                {
                    lstResult.Items.Add(SampleHelper.GetMatchItemText(matchItem, new string[] { "County", "State" }));
                }
            }
            matchItems = matchResult;

            // If we find addresses then select the first one to zoom in, if not then say we did not find any
            if (lstResult.Items.Count > 0)
            {
                lstResult.SelectedIndex = 0;
                lstResult_SelectedIndexChanged(this, new EventArgs());
            }
            else
            {
                ScriptManager.RegisterStartupScript(UpdatePanel1, UpdatePanel1.GetType(), "messageBox", "window.alert('No results found!')", true);

            }
        }

        protected void lstResult_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Populate the address grid based on the selected address
            GeocoderMatch matchItem = matchItems[lstResult.SelectedIndex];

            DataTable matchTable = new DataTable();
            matchTable.Columns.Add("Key");
            matchTable.Columns.Add("Value");
            foreach (KeyValuePair<string, string> item in matchItem.NameValuePairs)
            {
                matchTable.Rows.Add(new object[] { item.Key, item.Value });
            }
            dataGridViewDetail.DataSource = matchTable;
            dataGridViewDetail.DataBind();

            // Set the marker location to the address selected
            MarkerLayer markerLayer = map1.DynamicOverlay.Layers[0] as MarkerLayer;
            Proj4Projection proj4 = new Proj4Projection(4326, 3857);
            proj4.Open();

            var location = new PointShape(matchItem.NameValuePairs["CentroidPoint"]);
            markerLayer.MarkerLocation = (PointShape)proj4.ConvertToExternalProjection(location);

            // Set the extent around the address and refresh the map
            var nexExtent = new RectangleShape(matchItem.NameValuePairs["BoundingBox"]);
            map1.CurrentExtent = proj4.ConvertToExternalProjection(nexExtent);
        }

        #region "Fields"

        private Collection<GeocoderMatch> matchItems
        {
            get
            {
                if (Session["MatchItems"] == null)
                {
                    return new Collection<GeocoderMatch>();
                }
                else
                {
                    return Session["MatchItems"] as Collection<GeocoderMatch>;
                }
            }
            set
            {
                Session["MatchItems"] = value;
            }
        }

        #endregion
    }

    public class AccessMatchingPlugIn : MatchingPlugin, IDisposable
    {
        private string pathAndFileName;
        private string tableName;
        private string connectionString;
        private OleDbConnection oleDbConnection;

        public AccessMatchingPlugIn() :
            this(string.Empty, string.Empty)
        {
        }

        public AccessMatchingPlugIn(string tableName, string pathFileName)
        {
            this.pathAndFileName = pathFileName;
            this.tableName = tableName;
            this.connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + pathFileName + @";User Id=admin;Password=;";

        }

        public string TableName
        {
            get
            {
                return this.tableName;
            }
            set
            {
                this.tableName = value;
            }
        }

        public string AccessConnectionString
        {
            get
            {
                return this.connectionString;
            }
            set
            {
                this.connectionString = value;
            }
        }

        public string PathFileName
        {
            get
            {
                return pathAndFileName;
            }
            set
            {
                pathAndFileName = value;
            }
        }

        protected override void OpenCore()
        {
            oleDbConnection = new OleDbConnection(connectionString);
            oleDbConnection.Open();
        }

        protected override void CloseCore()
        {
            oleDbConnection.Close();
        }

        protected override Collection<GeocoderMatch> MatchCore(string sourceText)
        {
            string str = "select * from " + tableName + " where county ='" + sourceText + "'";
            if (oleDbConnection.State == ConnectionState.Closed)
            {
                oleDbConnection.Open();
            }
            Collection<GeocoderMatch> matchItems = new Collection<GeocoderMatch>();
            OleDbCommand oleCmd = new OleDbCommand(str, oleDbConnection);
            oleCmd.CommandTimeout = int.MaxValue;
            OleDbDataReader oleReader = oleCmd.ExecuteReader();
            while (oleReader.Read())
            {
                Dictionary<string, string> values = new Dictionary<string, string>();
                values.Add("County", oleReader["county"].ToString());
                values.Add("State", oleReader["State"].ToString());
                values.Add("CentroidPoint", oleReader["CentroidPoint"].ToString());
                values.Add("BoundingBox", oleReader["BoundingBox"].ToString());
                matchItems.Add(new GeocoderMatch(values));
            }
            return matchItems;
        }


        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (oleDbConnection != null)
                {
                    oleDbConnection.Dispose();
                }
            }
        }

        #endregion
    }
}
