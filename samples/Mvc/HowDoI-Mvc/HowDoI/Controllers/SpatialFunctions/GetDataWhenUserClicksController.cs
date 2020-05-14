using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Layers;

namespace CSharp_HowDoISamples
{
    public partial class SpatialFunctionsController : Controller
    {
        //
        // GET: /GetDataWhenUserClicks/

        public ActionResult GetDataWhenUserClicks()
        {
            return View();
        }

        [MapActionFilter]
        public string GetData(Map map, GeoCollection<object> args)
        {
            double clickX = Convert.ToDouble(args[0].ToString(), CultureInfo.InvariantCulture);
            double clickY = Convert.ToDouble(args[1].ToString(), CultureInfo.InvariantCulture);
            PointShape pointShape = new PointShape(clickX, clickY);
            LayerOverlay staticOverlay = (LayerOverlay)map.CustomOverlays["StaticOverlay"];
            FeatureLayer worldLayer = (FeatureLayer)staticOverlay.Layers["WorldLayer"];
            worldLayer.Open();
            Collection<Feature> selectedFeatures = worldLayer.QueryTools.GetFeaturesContaining(pointShape, new string[2] { "CNTRY_NAME", "POP_CNTRY" });
            worldLayer.Close();

            Country country = new Country();
            if (selectedFeatures.Count > 0)
            {
                country.CountryName = selectedFeatures[0].ColumnValues["CNTRY_NAME"];
                country.Population = selectedFeatures[0].ColumnValues["POP_CNTRY"];
            }

            return country.CountryName + "|" + country.Population;
        }
    }
}
