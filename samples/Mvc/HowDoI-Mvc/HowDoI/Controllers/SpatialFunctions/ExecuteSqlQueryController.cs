using System.Collections.ObjectModel;
using System.Data;
using System.Web.Mvc;
using ThinkGeo.MapSuite.Layers;

namespace CSharp_HowDoISamples
{
    public partial class SpatialFunctionsController : Controller
    {
        //
        // GET: /ExecuteSqlQuery/

        public ActionResult ExecuteSqlQuery()
        {
            Collection<Country> countries = new Collection<Country>();
            if (HttpContext.Request.QueryString.Count > 0)
            {
                string sql = ControllerContext.HttpContext.Request.Form["SQLTextBox"].ToString().ToLowerInvariant();

                ShapeFileFeatureSource featureSource = new ShapeFileFeatureSource(Server.MapPath("~/App_Data/cntry02.shp"));
                featureSource.Open();
                DataTable dataTable = featureSource.ExecuteQuery(sql);
                featureSource.Close();

                foreach (DataRow row in dataTable.Rows)
                {
                    Country country = new Country();
                    country.CountryName = row["cntry_name"].ToString();
                    country.Population = row["pop_cntry"].ToString();

                    countries.Add(country);
                }
            }

            return View(countries);
        }
    }
}
