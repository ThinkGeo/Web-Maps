using System;
using System.Collections.ObjectModel;
using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Styles;

namespace CSharp_HowDoISamples
{
    public partial class LayersFeatureSourcesController : Controller
    {
        //
        // GET: /StopCertainFeaturesFromDrawing/

        public ActionResult StopCertainFeaturesFromDrawing()
        {
            Map map = new Map("Map1",
             new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage),
             new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage));
            map.MapBackground = new GeoSolidBrush(GeoColor.FromHtml("#E5E3DF"));
            map.CurrentExtent = new RectangleShape(-140, 60, 140, -60);
            map.MapUnit = GeographyUnit.DecimalDegree;

            ShapeFileFeatureLayer worldLayer = new ShapeFileFeatureLayer(Server.MapPath("~/App_Data/cntry02.shp"));
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(255, 243, 239, 228), GeoColor.FromArgb(255, 218, 193, 163), 1);
            worldLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle.RequiredColumnNames.Add("POP_CNTRY");
            worldLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            worldLayer.DrawingFeatures += new EventHandler<DrawingFeaturesEventArgs>(worldLayer_DrawingFeatures);

            map.StaticOverlay.Layers.Add(worldLayer);

            return View(map);
        }

        void worldLayer_DrawingFeatures(object sender, DrawingFeaturesEventArgs e)
        {
            Collection<Feature> featuresToDrawn = new Collection<Feature>();
            foreach (Feature feature in e.FeaturesToDraw)
            {
                double population = Convert.ToDouble(feature.ColumnValues["POP_CNTRY"]);
                if (population > 10000000)
                {
                    featuresToDrawn.Add(feature);
                }
            }

            e.FeaturesToDraw.Clear();
            foreach (Feature feature in featuresToDrawn)
            {
                e.FeaturesToDraw.Add(feature);
            }
        }

    }
}
