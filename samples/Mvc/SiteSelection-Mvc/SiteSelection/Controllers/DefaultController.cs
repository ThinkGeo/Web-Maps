/*===========================================
    Backgrounds for this sample are powered by ThinkGeo Cloud Maps and require
    An API Key. These were sent to you via email when you signed up
    with ThinkGeo, or you can register now at https://cloud.thinkgeo.com.
===========================================*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Routing;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.SiteSelection.Properties;
using ThinkGeo.MapSuite.Styles;

namespace ThinkGeo.MapSuite.SiteSelection
{
    public class DefaultController : Controller
    {
        private static RoutingEngine routingEngine;
        // GET: /Default/
        public ActionResult Index()
        {
            // Initialize the Map
            Map map = new Map("Map1");
            map.Width = new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage);
            map.Height = new System.Web.UI.WebControls.Unit(100, System.Web.UI.WebControls.UnitType.Percentage);
            map.MapUnit = GeographyUnit.Meter;
            map.ZoomLevelSet = new ThinkGeoCloudMapsZoomLevelSet();
            map.MapTools.OverlaySwitcher.Enabled = true;
            map.MapTools.OverlaySwitcher.BaseOverlayTitle = "ThinkGeo Cloud Maps: ";
            map.CurrentExtent = new RectangleShape(-10788072.2328016, 3923530.35787306, -10769555.4090135, 3906993.24816589);

            AddOverlays(map);

            return View(map);
        }

        [MapActionFilter]
        public JsonResult GetCategories(Map map, GeoCollection<object> args)
        {
            // Initialise categories
            Dictionary<string, Collection<string>> poiTypes = new Dictionary<string, Collection<string>>();
            foreach (string layerId in new[] { "Hotels", "Medical Facilites", "Restaurants", "Schools" })
            {
                poiTypes.Add(layerId, GetPoiSubTypes(map.CustomOverlays["PoisOverlay"] as LayerOverlay, layerId));
            }
            // Return the categories in JSON to client side.
            return Json(poiTypes);
        }

        [MapActionFilter]
        public void Clear(Map map, GeoCollection<object> args)
        {
            // Clear search poi markers.
            map.MarkerOverlay.FeatureSource.Clear();

            // Clear the service area.
            LayerOverlay queriedOverlay = map.CustomOverlays["QueriedOverlay"] as LayerOverlay;
            InMemoryFeatureLayer serviceAreaLayer = queriedOverlay.Layers["serviceArea"] as InMemoryFeatureLayer;
            serviceAreaLayer.InternalFeatures.Clear();

            // Clear the plot point.
            InMemoryMarkerOverlay plotPointOverlay = map.CustomOverlays["DrawnPointOverlay"] as InMemoryMarkerOverlay;
            plotPointOverlay.FeatureSource.Clear();
        }

        [MapActionFilter]
        public JsonResult SearchSimilarSites(Map map, GeoCollection<object> args)
        {
            string[] searchPointLatLng = args["searchPoint"].ToString().Split(',');
            PointShape searchPoint = new PointShape(double.Parse(searchPointLatLng[0]), double.Parse(searchPointLatLng[1]));

            object result = new { status = "2", message = "out of restriction area" };
            Feature searchAreaFeature = null;

            // check if the clicked point is in valid area (Frisco City)
            FeatureLayer friscoLayer = (FeatureLayer)(map.CustomOverlays["RestrictOverlay"] as LayerOverlay).Layers["RestrictLayer"];
            friscoLayer.Open();
            if (friscoLayer.QueryTools.GetFeaturesContaining(searchPoint, ReturningColumnsType.NoColumns).Any())
            {
                // Calculate the service area/buffer area and display it on the map
                if (args["searchMode"].ToString().Equals("serviceArea", StringComparison.InvariantCultureIgnoreCase))
                {
                    int drivingTimeInMinutes = Convert.ToInt32(args["driveTime"].ToString(), CultureInfo.InvariantCulture);
                    searchAreaFeature = new Feature(routingEngine.GenerateServiceArea(searchPoint, new TimeSpan(0, drivingTimeInMinutes, 0), 100, GeographyUnit.Meter));
                }
                else
                {
                    DistanceUnit distanceUnit = args["distanceUnit"].ToString() == "Mile" ? DistanceUnit.Mile : DistanceUnit.Kilometer;
                    double distanceBuffer = Convert.ToDouble(args["distanceBuffer"].ToString(), CultureInfo.InvariantCulture);
                    searchAreaFeature = new Feature(searchPoint.Buffer(distanceBuffer, 40, GeographyUnit.Meter, distanceUnit));
                }

                // Search the Pois in calculated service area and display them on map
                LayerOverlay poiOverlay = map.CustomOverlays["PoisOverlay"] as LayerOverlay;
                ShapeFileFeatureLayer poiLayer = (ShapeFileFeatureLayer)(poiOverlay.Layers[args["category"].ToString()]);
                poiLayer.Open();
                Collection<Feature> featuresInServiceArea = poiLayer.QueryTools.GetFeaturesWithin(searchAreaFeature.GetShape(), ReturningColumnsType.AllColumns);
                List<Feature> filteredQueryFeatures = FilterFeaturesByQueryConfiguration(featuresInServiceArea, args["category"].ToString(), args["subCategory"].ToString().Replace(">~", ">="));

                if (filteredQueryFeatures.Any())
                {
                    Collection<object> returnedJsonFeatures = new Collection<object>();
                    foreach (Feature feature in filteredQueryFeatures)
                    {
                        PointShape latlng = feature.GetShape() as PointShape;
                        returnedJsonFeatures.Add(new { name = feature.ColumnValues["NAME"], point = string.Format("{0},{1}", latlng.Y, latlng.X) });
                    }

                    // Add into plot marker overlay.
                    InMemoryMarkerOverlay drawPointOverlay = map.CustomOverlays["DrawnPointOverlay"] as InMemoryMarkerOverlay;
                    drawPointOverlay.FeatureSource.Clear();
                    drawPointOverlay.FeatureSource.BeginTransaction();
                    drawPointOverlay.FeatureSource.AddFeature(new Feature(searchPoint));
                    drawPointOverlay.FeatureSource.CommitTransaction();

                    // Add into markerOvelray.
                    AddMarkers(filteredQueryFeatures, map);

                    Feature wgs84Feature = new Feature(searchAreaFeature.GetShape());
                    // Add into queried overlay.
                    LayerOverlay queriedOverlay = map.CustomOverlays["QueriedOverlay"] as LayerOverlay;
                    InMemoryFeatureLayer serviceAreaLayer = (queriedOverlay.Layers["serviceArea"] as InMemoryFeatureLayer);
                    serviceAreaLayer.Open();
                    serviceAreaLayer.InternalFeatures.Clear();
                    serviceAreaLayer.InternalFeatures.Add(wgs84Feature);

                    result = new { status = "0", message = "has features", features = returnedJsonFeatures };
                }
                else
                {
                    result = new { status = "1", message = "without features" };
                }
            }

            // return the search poi feature information to the client.
            return Json(result);
        }

        private void AddMarkers(List<Feature> features, Map map)
        {
            if (features.Count <= 0)
            {
                return;
            }

            StringBuilder popupHtml = new StringBuilder("<table>");
            popupHtml.Append("<tr><td colspan='2' class='popupTitle'>[#NAME#]</td></tr>");
            popupHtml.Append("<tr class='popupText'><td>ADDR:</td><td>[#ADDRESS#]</td></tr>");
            popupHtml.Append("<tr><td colspan='2'><div class='hrLine'></div></td></tr>");

            map.MarkerOverlay.FeatureSource.Open();
            map.MarkerOverlay.Columns.Clear();
            foreach (var columnValue in features[0].ColumnValues)
            {
                if (!columnValue.Key.Equals("NAME", StringComparison.OrdinalIgnoreCase) && !columnValue.Key.Equals("ADDRESS", StringComparison.OrdinalIgnoreCase))
                {
                    popupHtml.Append(string.Format("<tr class='vehicleTxt'><td>{0} : </td><td>[#{0}#]</td></tr>", columnValue.Key));
                }
                map.MarkerOverlay.Columns.Add(new FeatureSourceColumn(columnValue.Key));
            }
            popupHtml.Append("</table>");
            map.MarkerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.ContentHtml = popupHtml.ToString();
            map.MarkerOverlay.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            map.MarkerOverlay.FeatureSource.Clear();
            foreach (Feature feature in features)
            {
                map.MarkerOverlay.FeatureSource.InternalFeatures.Add(feature);
            }
        }

        private static Collection<string> GetPoiSubTypes(LayerOverlay poiOverlay, string poiType)
        {
            Collection<string> poiSubTypes = new Collection<string>();
            poiSubTypes.Add("All");

            string columnName = InternalHelper.GetDbfColumnByPoiType(poiType);
            if (columnName.Equals("Hotels"))
            {
                poiSubTypes.Add("1 ~ 50");
                poiSubTypes.Add("50 ~ 100");
                poiSubTypes.Add("100 ~ 150");
                poiSubTypes.Add("150 ~ 200");
                poiSubTypes.Add("200 ~ 300");
                poiSubTypes.Add("300 ~ 400");
                poiSubTypes.Add("400 ~ 500");
                poiSubTypes.Add(">= 500");
            }
            else
            {
                ShapeFileFeatureLayer inMemoryFeatureLayer = (ShapeFileFeatureLayer)poiOverlay.Layers[poiType];
                inMemoryFeatureLayer.Open();
                IEnumerable<string> distinctColumnValues = inMemoryFeatureLayer.FeatureSource.GetDistinctColumnValues(columnName).Select(v => v.ColumnValue);
                foreach (string distinctColumnValue in distinctColumnValues)
                {
                    poiSubTypes.Add(distinctColumnValue);
                }
            }

            return poiSubTypes;
        }

        private List<Feature> FilterFeaturesByQueryConfiguration(Collection<Feature> allFeatures, string category, string subCategory)
        {
            if (subCategory.Equals("All"))
            {
                return allFeatures.ToList();
            }
            else if (!category.Equals("Hotels"))
            {
                return allFeatures.Where(f => f.ColumnValues[InternalHelper.GetDbfColumnByPoiType(category)] == subCategory).ToList();
            }
            else
            {
                string queriedColumn = InternalHelper.GetDbfColumnByPoiType(category);

                List<Feature> queriedPois = new List<Feature>();
                foreach (Feature feature in allFeatures)
                {
                    int rooms = int.Parse(feature.ColumnValues[queriedColumn], CultureInfo.InvariantCulture);

                    string[] values = subCategory.Split('~');
                    if (values.Length >= 2)
                    {
                        if (int.Parse(values[0], CultureInfo.InvariantCulture) <= rooms && int.Parse(values[1], CultureInfo.InvariantCulture) >= rooms)
                        {
                            queriedPois.Add(feature);
                        }
                    }
                    else if (values.Length == 1)
                    {
                        int maxValue = int.Parse(values[0].TrimStart(new[] { '>', '=', ' ' }), CultureInfo.InvariantCulture);
                        if (rooms > maxValue)
                        {
                            queriedPois.Add(feature);
                        }
                    }
                }
                return queriedPois;
            }
        }

        private void AddOverlays(Map map)
        {
            // Please input your ThinkGeo Cloud API Key to enable the background map.
            ThinkGeoCloudRasterMapsOverlay lightMapOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            lightMapOverlay.Name = "Light";
            lightMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Light;
            map.CustomOverlays.Add(lightMapOverlay);

            ThinkGeoCloudRasterMapsOverlay darkMapOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            darkMapOverlay.Name = "Dark";
            darkMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Dark;
            map.CustomOverlays.Add(darkMapOverlay);

            ThinkGeoCloudRasterMapsOverlay aerialMapOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            aerialMapOverlay.Name = "Aerial";
            aerialMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Aerial;
            map.CustomOverlays.Add(aerialMapOverlay);

            ThinkGeoCloudRasterMapsOverlay hybridMapOverlay = new ThinkGeoCloudRasterMapsOverlay("ThinkGeo Cloud API Key");
            hybridMapOverlay.Name = "Hybrid";
            hybridMapOverlay.MapType = ThinkGeoCloudRasterMapsMapType.Hybrid;
            map.CustomOverlays.Add(hybridMapOverlay);

            LayerOverlay poisOverlay = new LayerOverlay("PoisOverlay");
            poisOverlay.IsBaseOverlay = false;
            poisOverlay.IsVisibleInOverlaySwitcher = false;
            map.CustomOverlays.Add(poisOverlay);

            // POI Overlay
            Proj4Projection proj4 = new Proj4Projection();
            proj4.InternalProjectionParametersString = Proj4Projection.GetDecimalDegreesParametersString();
            proj4.ExternalProjectionParametersString = Proj4Projection.GetSphericalMercatorParametersString();
            proj4.Open();

            ShapeFileFeatureLayer hotelsLayer = new ShapeFileFeatureLayer(Server.MapPath(ConfigurationManager.AppSettings["HotelsShapeFilePathName"]), GeoFileReadWriteMode.Read);
            hotelsLayer.Name = Resource.Hotels;
            hotelsLayer.Transparency = 120f;
            hotelsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(new GeoImage(Server.MapPath("~/Content/Images/Hotel.png")));
            hotelsLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            hotelsLayer.FeatureSource.Projection = proj4;
            poisOverlay.Layers.Add(hotelsLayer.Name, hotelsLayer);

            ShapeFileFeatureLayer medicalFacilitesLayer = new ShapeFileFeatureLayer(Server.MapPath(ConfigurationManager.AppSettings["MedicalFacilitiesShapeFilePathName"]), GeoFileReadWriteMode.Read);
            medicalFacilitesLayer.Name = Resource.MedicalFacilites;
            medicalFacilitesLayer.Transparency = 120f;
            medicalFacilitesLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(new GeoImage(Server.MapPath("~/Content/Images/DrugStore.png")));
            medicalFacilitesLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            medicalFacilitesLayer.FeatureSource.Projection = proj4;
            poisOverlay.Layers.Add(medicalFacilitesLayer.Name, medicalFacilitesLayer);

            ShapeFileFeatureLayer publicFacilitesLayer = new ShapeFileFeatureLayer(Server.MapPath(ConfigurationManager.AppSettings["PublicFacilitiesShapeFilePathName"]), GeoFileReadWriteMode.Read);
            publicFacilitesLayer.Name = Resource.PublicFacilites;
            publicFacilitesLayer.Transparency = 120f;
            publicFacilitesLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(new GeoImage(Server.MapPath("~/Content/Images/public_facility.png")));
            publicFacilitesLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            publicFacilitesLayer.FeatureSource.Projection = proj4;
            poisOverlay.Layers.Add(publicFacilitesLayer.Name, publicFacilitesLayer);

            ShapeFileFeatureLayer restaurantsLayer = new ShapeFileFeatureLayer(Server.MapPath(ConfigurationManager.AppSettings["RestaurantsShapeFilePathName"]), GeoFileReadWriteMode.Read);
            restaurantsLayer.Name = Resource.Restaurants;
            restaurantsLayer.Transparency = 120f;
            restaurantsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(new GeoImage(Server.MapPath("~/Content/Images/restaurant.png")));
            restaurantsLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            restaurantsLayer.FeatureSource.Projection = proj4;
            poisOverlay.Layers.Add(restaurantsLayer.Name, restaurantsLayer);

            ShapeFileFeatureLayer schoolsLayer = new ShapeFileFeatureLayer(Server.MapPath(ConfigurationManager.AppSettings["SchoolsShapeFilePathName"]), GeoFileReadWriteMode.Read);
            schoolsLayer.Name = Resource.Schools;
            schoolsLayer.Transparency = 120f;
            schoolsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle(new GeoImage(Server.MapPath("~/Content/Images/school.png")));
            schoolsLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            schoolsLayer.FeatureSource.Projection = proj4;
            poisOverlay.Layers.Add(schoolsLayer.Name, schoolsLayer);

            // Restrict area Overlay
            ShapeFileFeatureLayer restrictedLayer = new ShapeFileFeatureLayer(Server.MapPath(ConfigurationManager.AppSettings["RestrictedShapeFilePathName"]), GeoFileReadWriteMode.Read);
            AreaStyle extentStyle = new AreaStyle();
            extentStyle.CustomAreaStyles.Add(new AreaStyle(new GeoSolidBrush(GeoColor.SimpleColors.Transparent)) { OutlinePen = new GeoPen(GeoColor.SimpleColors.White, 5.5f) });
            extentStyle.CustomAreaStyles.Add(new AreaStyle(new GeoSolidBrush(GeoColor.SimpleColors.Transparent)) { OutlinePen = new GeoPen(GeoColor.SimpleColors.Red, 1.5f) { DashStyle = LineDashStyle.Dash } });
            restrictedLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = extentStyle;
            restrictedLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            restrictedLayer.FeatureSource.Projection = proj4;

            LayerOverlay restrictOverlay = new LayerOverlay("RestrictOverlay");
            restrictOverlay.IsBaseOverlay = false;
            restrictOverlay.IsVisibleInOverlaySwitcher = false;
            restrictOverlay.Layers.Add("RestrictLayer", restrictedLayer);
            map.CustomOverlays.Add(restrictOverlay);

            // Queried Service Overlay
            InMemoryFeatureLayer serviceAreaLayer = new InMemoryFeatureLayer();
            GeoColor serviceAreaGeoColor = new GeoColor(120, GeoColor.FromHtml("#1749c9"));
            serviceAreaLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(serviceAreaGeoColor, GeoColor.FromHtml("#fefec1"), 2, LineDashStyle.Solid);
            serviceAreaLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            InMemoryFeatureLayer highlightFeatureLayer = new InMemoryFeatureLayer();
            highlightFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay queriedOverlay = new LayerOverlay("QueriedOverlay");
            queriedOverlay.IsBaseOverlay = false;
            queriedOverlay.IsVisibleInOverlaySwitcher = false;
            queriedOverlay.TileType = TileType.SingleTile;
            queriedOverlay.Layers.Add("highlightFeatureLayer", highlightFeatureLayer);
            queriedOverlay.Layers.Add("serviceArea", serviceAreaLayer);
            map.CustomOverlays.Add(queriedOverlay);

            // Marker Overlay
            map.MarkerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage = new WebImage("/Content/Images/selectedHalo.png") { ImageOffsetX = -16, ImageOffsetY = -16 };
            map.MarkerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.BorderWidth = 1;
            map.MarkerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.BorderColor = GeoColor.StandardColors.Gray;
            map.MarkerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.Popup.AutoSize = true;
            map.MarkerOverlay.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            map.MarkerOverlay.IsVisibleInOverlaySwitcher = false;

            // Drawn Point
            InMemoryMarkerOverlay markerOverlay = new InMemoryMarkerOverlay("DrawnPointOverlay");
            markerOverlay.FeatureSource.InternalFeatures.Add(new Feature(new PointShape(-10776838.0796536, 3912346.50475619)));     // Add a initial point for query
            markerOverlay.IsVisibleInOverlaySwitcher = false;
            markerOverlay.ZoomLevelSet.ZoomLevel01.DefaultMarkerStyle.WebImage = new WebImage("/Content/Images/drawPoint.png") { ImageOffsetX = -16, ImageOffsetY = -32 };
            markerOverlay.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            map.CustomOverlays.Add(markerOverlay);

            // Legend Layer
            LegendAdornmentLayer legendlayer = new LegendAdornmentLayer { Height = 135, Location = AdornmentLocation.LowerRight };
            map.AdornmentOverlay.Layers.Add(legendlayer);

            LegendItem hotelsLayeritem = new LegendItem();
            hotelsLayeritem.ImageStyle = hotelsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle;
            hotelsLayeritem.TextStyle = WorldStreetsTextStyles.GeneralPurpose(Resource.Hotels, 10);
            legendlayer.LegendItems.Add("hotels", hotelsLayeritem);

            LegendItem medicalFacilitesLayeritem = new LegendItem();
            medicalFacilitesLayeritem.ImageStyle = medicalFacilitesLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle;
            medicalFacilitesLayeritem.TextStyle = WorldStreetsTextStyles.GeneralPurpose(Resource.MedicalFacilites, 10);

            legendlayer.LegendItems.Add("medicalFacilites", medicalFacilitesLayeritem);

            LegendItem publicFacilitesLayeritem = new LegendItem();
            publicFacilitesLayeritem.ImageStyle = publicFacilitesLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle;
            publicFacilitesLayeritem.TextStyle = WorldStreetsTextStyles.GeneralPurpose(Resource.PublicFacilites, 10);

            legendlayer.LegendItems.Add("publicFacilites", publicFacilitesLayeritem);

            LegendItem restaurantsLayeritem = new LegendItem();
            restaurantsLayeritem.ImageStyle = restaurantsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle;
            restaurantsLayeritem.TextStyle = WorldStreetsTextStyles.GeneralPurpose(Resource.Restaurants, 10);

            legendlayer.LegendItems.Add("restaurants", restaurantsLayeritem);

            LegendItem schoolsLayeritem = new LegendItem();
            schoolsLayeritem.ImageStyle = schoolsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle;
            schoolsLayeritem.TextStyle = WorldStreetsTextStyles.GeneralPurpose(Resource.Schools, 10);

            legendlayer.LegendItems.Add("schools", schoolsLayeritem);

            // Scale bar layer
            ScaleBarAdornmentLayer scaleBarAdormentLayer = new ScaleBarAdornmentLayer();
            scaleBarAdormentLayer.Location = AdornmentLocation.LowerLeft;
            scaleBarAdormentLayer.XOffsetInPixel = 10;
            map.AdornmentOverlay.Layers.Add("ScaleBar", scaleBarAdormentLayer);
            map.AdornmentOverlay.IsVisibleInOverlaySwitcher = false;

            // Create the Routine Engine. 
            string streetShapeFilePathName = Server.MapPath(ConfigurationManager.AppSettings["StreetShapeFilePathName"]);
            string streetRtgFilePathName = Path.ChangeExtension(streetShapeFilePathName, ".rtg");

            RtgRoutingSource routingSource = new RtgRoutingSource(streetRtgFilePathName);
            FeatureSource featureSource = new ShapeFileFeatureSource(streetShapeFilePathName);
            featureSource.Projection = proj4;
            routingEngine = new RoutingEngine(routingSource, featureSource);
            routingEngine.GeographyUnit = GeographyUnit.Meter;
        }
    }
}
