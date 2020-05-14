/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Web.UI;
using ThinkGeo.MapSuite.Geocoding;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.WebForms;

namespace ThinkGeo.MapSuite.HowDoI
{
    public partial class ReverseGeoCodingInTexas : System.Web.UI.Page
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
            // Get the path to the data files and create the Geocoding
            string dataPath = this.GetDataPath("CitystateGeocoding");
            UsaGeocoder usaGeocoder = new UsaGeocoder(dataPath);

            // Open the geocoder, get any matches and close it
            Collection<GeocoderMatch> matchResult;
            try
            {
                usaGeocoder.Open();
                matchResult = usaGeocoder.Match(cboSourceText.Text);
                matchItems = matchResult;
            }
            finally
            {
                usaGeocoder.Close();
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
                if (matchItem.NameValuePairs.ContainsKey("Street"))
                {
                    lstResult.Items.Add(SampleHelper.GetMatchItemText(matchItem, new string[] { "Street", "Zip", "State" }));
                }
            }

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
}
