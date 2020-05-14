using System.Collections.ObjectModel;
using System.Globalization;
using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;

namespace CSharp_HowDoISamples
{
    public partial class InteractiveOverlaysController : Controller
    {
        //
        // GET: /AreaOfAFeature/
        public ActionResult AreaOfAFeature()
        {
            return View();
        }

        [MapActionFilter]
        public void GetArea(Map map, Collection<object> args)
        {
            double clickedX = double.Parse(args[0].ToString(), CultureInfo.InvariantCulture);
            double clickedY = double.Parse(args[1].ToString(), CultureInfo.InvariantCulture);
            PointShape point = new PointShape(clickedX, clickedY);

            FeatureLayer worldLayer = (ShapeFileFeatureLayer)((LayerOverlay)map.CustomOverlays["OverLayer"]).Layers["worldLayer"];

            // Find the country the user clicked on.
            worldLayer.Open();
            Collection<Feature> selectedFeatures = worldLayer.QueryTools.GetFeaturesContaining(point, new string[1] { "CNTRY_NAME" });
            worldLayer.Close();

            // Determine the area of the country.
            string contentHtml = @"<div style='color:#0065ce;font-size:10px; font-family:verdana; padding:4px;'>Please click on a country to see how large it is.</div>";
            if (selectedFeatures.Count > 0)
            {
                AreaBaseShape areaShape = (AreaBaseShape)selectedFeatures[0].GetShape();
                Proj4Projection proj4Projection = new Proj4Projection(3857, 4326);
                proj4Projection.Open();
                areaShape = (AreaBaseShape)proj4Projection.ConvertToExternalProjection(areaShape);
                double area = areaShape.GetArea(GeographyUnit.DecimalDegree, AreaUnit.SquareKilometers);
                contentHtml = string.Format(@"<div style='color:#0065ce;font-size:10px; font-family:verdana; padding:4px;'><span style='color:red'>{0}</span> has an area of <span style='color:red'>{1:N0}</span> square kilometers.</div>", selectedFeatures[0].ColumnValues["CNTRY_NAME"], area);
            }

            map.Popups.Clear();

            CloudPopup popup = new CloudPopup("information");
            popup.Position = point;
            popup.AutoSize = true;
            popup.ContentHtml = contentHtml;
            map.Popups.Add(popup);
        }
    }
}
