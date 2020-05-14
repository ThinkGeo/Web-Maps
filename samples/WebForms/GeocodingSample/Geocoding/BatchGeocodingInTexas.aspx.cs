/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using ThinkGeo.MapSuite.Geocoding;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.WebForms;

namespace ThinkGeo.MapSuite.HowDoI
{
    public partial class BatchGeocodingInTexas : System.Web.UI.Page
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

                LoadDataSource();
            }
        }

        protected void btnBatchGeoCode_Click(object sender, EventArgs e)
        {
            // Define a Stopwatch to measure the batch duration
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            // Define a int to record the success count of batching Geocoding
            int successCount = 0;

            // Get the path of the data files and create the Geocoding
            var dataPath = this.GetDataPath("CitystateGeocoding");
            UsaGeocoder usaGeoCoder = new UsaGeocoder(dataPath);
            DataTable resultTable = new DataTable();
            resultTable.Columns.Add("Key");
            resultTable.Columns.Add("Value");
            Collection<GeocoderMatch> matchedItems = new Collection<GeocoderMatch>();

            //Open the geocoder and call the match method then close it
            try
            {
                usaGeoCoder.Open();
                for (int rowIndex = 0; rowIndex < DataSource.Length; rowIndex++)
                {
                    string[] parts = DataSource[rowIndex].Split(',');
                    string address = parts[0];
                    string zip = parts[1];
                    Collection<GeocoderMatch> result = usaGeoCoder.Match(address, zip);

                    if (result.Count > 0)
                    {
                        // If the NameValuePairs contains the key Street it means we found match on street,
                        // then we add it to the result DataGrid.
                        if (result[0].NameValuePairs.ContainsKey("Street"))
                        {
                            successCount++;
                            matchedItems.Add(result[0]);
                            resultTable.Rows.Add(new object[] { address, SampleHelper.GetMatchItemText(result[0], new string[] { "CentroidPoint" }) });
                        }
                    }

                }
                matchItems = matchedItems;
                dataGridViewDetail.DataSource = resultTable;
                dataGridViewDetail.DataBind();
            }
            finally
            {
                usaGeoCoder.Close();
            }

            stopWatch.Stop();

            PopulateBatchResults(successCount, stopWatch.ElapsedMilliseconds);
        }

        private void LoadDataSource()
        {
            string dataPath = Path.Combine(this.GetDataPath("CitystateGeocoding"));
            DataSource = File.ReadAllLines(Path.Combine(dataPath, "BatchGeocoderData.txt"));
            txtTotalRecordCount.Text = string.Format(CultureInfo.InvariantCulture, "{0:0,0}", DataSource.Length, CultureInfo.InvariantCulture);
            DataTable resultTable = new DataTable();
            resultTable.Columns.Add("Key");
            resultTable.Columns.Add("Value");
            foreach (string item in DataSource)
            {
                string address = item.Split(',')[0];
                resultTable.Rows.Add(new object[] { address, string.Empty });
            }
            dataGridViewDetail.DataSource = resultTable;
            dataGridViewDetail.DataBind();
        }

        private void PopulateBatchResults(int successCount, double duration)
        {
            txtSuccessRate.Text = string.Format(CultureInfo.InvariantCulture, "{0:0.0%}", (double)successCount / (double)DataSource.Length);
            txtTotalTime.Text = string.Format(CultureInfo.InvariantCulture, "{0:0.00 sec}", duration / (double)1000);
            txtTimePerRec.Text = string.Format(CultureInfo.InvariantCulture, "{0:0.00 ms}", duration / (double)successCount);
            txtRecPerSecond.Text = string.Format(CultureInfo.InvariantCulture, "{0:0}", (double)successCount / duration * 1000);

            //dataGridViewDetail.Rows[0].Cells[0].Selected = true;
            //dataGridViewDetail_CellClick(this, new DataGridViewCellEventArgs(0, 0));
        }

        protected void dataGridViewDetail_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Attributes.Add("onmouseover", "this.style.cursor='hand';this.style.backgroundColor='lightpink';");
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor='';");
                e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackEventReference(dataGridViewDetail, "Select$" + e.Row.RowIndex.ToString()));
            }
        }

        protected void dataGridViewDetail_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (dataGridViewDetail.SelectedIndex >= 0 && dataGridViewDetail.SelectedIndex < matchItems.Count)
            {
                GeocoderMatch matchItem = matchItems[dataGridViewDetail.SelectedIndex];

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
        }

        protected override void Render(HtmlTextWriter writer)
        {
            for (int i = 0; i < dataGridViewDetail.Rows.Count; i++)
            {
                Page.ClientScript.RegisterForEventValidation(new System.Web.UI.PostBackOptions(dataGridViewDetail, "Select$" + i.ToString()));
            }

            base.Render(writer);
        }

        protected string[] DataSource
        {
            get
            {
                if (Session["DataSource"] == null)
                {
                    return new string[] { };
                }
                else
                {
                    return Session["DataSource"] as string[];
                }
            }
            set
            {
                Session["DataSource"] = value;
            }
        }

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


    }
}
