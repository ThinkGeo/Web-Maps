/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System.Collections.ObjectModel;
using System.Data;
using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Styles;

namespace CSharp_HowDoISamples
{
    public partial class SpatialFunctionsController : Controller
    {
        //
        // GET: /GetAllFeatures/

        public ActionResult GetAllFeatures()
        {
            Map map = new Map("Map1",
                    new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage),
                    new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage));
            map.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#B3C6D4"));
            map.MapUnit = GeographyUnit.Meter;
            map.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            map.CurrentExtent = new RectangleShape(-13914936.3491592, 11753184.6153385, 5565974.53966368, -5780349.22025635);

            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay backgroundOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            map.CustomOverlays.Add(backgroundOverlay);

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(Server.MapPath("~/App_Data/cntry02_3857.shp"));
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.SimpleColors.Transparent, GeoColor.FromArgb(100, GeoColor.SimpleColors.Green));
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay staticOverlay = new LayerOverlay("StaticOverlay");
            staticOverlay.IsBaseOverlay = false;
            staticOverlay.Layers.Add(worldLayer);
            map.CustomOverlays.Add(staticOverlay);
            ViewData["map"] = map;

            DataTable dataTable = BindAllFeaturesToGridView(worldLayer);
            Collection<Country> countries = new Collection<Country>();
            foreach (DataRow row in dataTable.Rows)
            {
                Country country = new Country();
                country.CountryId = row["CountryId"].ToString();
                country.CountryName = row["CountryName"].ToString();
                country.Currency = row["Currency"].ToString();

                countries.Add(country);
            }
            return View(countries);
        }

        private DataTable BindAllFeaturesToGridView(FeatureLayer featureLayer)
        {
            string[] returningColumns = new string[] { "CNTRY_NAME", "CURR_TYPE", "RECID" };
            featureLayer.Open();
            Collection<Feature> allFeaturs = featureLayer.FeatureSource.GetAllFeatures(returningColumns);
            featureLayer.Close();

            return GetDataTableFromFeatures(allFeaturs, returningColumns);
        }

        private static DataTable GetDataTableFromFeatures(Collection<Feature> features, string[] columns)
        {
            DataTable dataTable = new DataTable();
            if (features != null)
            {
                dataTable.Columns.Add("CountryName");
                dataTable.Columns.Add("Currency");
                dataTable.Columns.Add("CountryId");

                foreach (Feature feature in features)
                {
                    AddRow(columns, feature, dataTable);
                }
            }

            return dataTable;
        }

        private static void AddRow(string[] returningColumns, Feature feature, DataTable dataTable)
        {
            DataRow dataRow = dataTable.NewRow();

            for (int i = 0; i < returningColumns.Length; i++)
            {
                dataRow[i] = feature.ColumnValues[returningColumns[i]];
            }

            dataTable.Rows.Add(dataRow);
        }
    }
}
