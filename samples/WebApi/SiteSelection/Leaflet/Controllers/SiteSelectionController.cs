using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebApi;
using ThinkGeo.MapSuite.Routing;

namespace SiteSelection.Controllers
{
	[RoutePrefix ("SiteSelection")]
	public class SiteSelectionController : ApiController
	{
		// The key used to store the search area extent for the specified user.
		private const string SearchAreaKey = "SearchArea";
		// The key used to store the features searched by the condifitions and user.
		private const string SearchedFeatureKey = "SearchedFeature";
		// Categories used to search.
		private static Dictionary<string, Collection<string>> poiTypes;
		// Base overlay.
		private static LayerOverlay poiOverlay;
		private static RoutingEngine routingEngine;

		static SiteSelectionController ()
		{
			// Create the static POI Overlay.
			poiOverlay = new LayerOverlay ();

			Proj4Projection wgs84ToMercatorProj4Projection = new Proj4Projection ();
			wgs84ToMercatorProj4Projection.InternalProjectionParametersString = Proj4Projection.GetWgs84ParametersString ();
			wgs84ToMercatorProj4Projection.ExternalProjectionParametersString = Proj4Projection.GetSphericalMercatorParametersString ();
			wgs84ToMercatorProj4Projection.Open ();

			ShapeFileFeatureLayer friscoBoundaryLayer = new ShapeFileFeatureLayer (InternalHelper.GetFullPath ("App_Data/CityLimitPolygon.shp"));
			friscoBoundaryLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add (new AreaStyle (new GeoPen (GeoColor.SimpleColors.White, 5.5f), new GeoSolidBrush (GeoColor.SimpleColors.Transparent)));
			friscoBoundaryLayer.ZoomLevelSet.ZoomLevel01.CustomStyles.Add (new AreaStyle (new GeoPen (GeoColor.SimpleColors.Red, 1.5f) { DashStyle = LineDashStyle.Dash }, new GeoSolidBrush (GeoColor.SimpleColors.Transparent)));
			friscoBoundaryLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
			friscoBoundaryLayer.FeatureSource.Projection = wgs84ToMercatorProj4Projection;
			poiOverlay.Layers.Add ("friscoBoundary", friscoBoundaryLayer);

			ShapeFileFeatureLayer hotelsLayer = new ShapeFileFeatureLayer (InternalHelper.GetFullPath ("App_Data/POIs/Hotels.shp"));
			hotelsLayer.Transparency = 200f;
			hotelsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle (new GeoImage (InternalHelper.GetFullPath ("Images/Hotel.png")));
			hotelsLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
			hotelsLayer.FeatureSource.Projection = wgs84ToMercatorProj4Projection;
			poiOverlay.Layers.Add ("hotelsLayer", hotelsLayer);

			ShapeFileFeatureLayer medicalFacilitesLayer = new ShapeFileFeatureLayer (InternalHelper.GetFullPath ("App_Data/POIs/Medical_Facilities.shp"));
			medicalFacilitesLayer.Transparency = 200f;
			medicalFacilitesLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle (new GeoImage (InternalHelper.GetFullPath ("Images/DrugStore.png")));
			medicalFacilitesLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
			medicalFacilitesLayer.FeatureSource.Projection = wgs84ToMercatorProj4Projection;
			poiOverlay.Layers.Add ("Medical Facilites", medicalFacilitesLayer);

			ShapeFileFeatureLayer publicFacilitesLayer = new ShapeFileFeatureLayer (InternalHelper.GetFullPath ("App_Data/POIs/Public_Facilities.shp"));
			publicFacilitesLayer.Transparency = 200f;
			publicFacilitesLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle (new GeoImage (InternalHelper.GetFullPath ("Images/public_facility.png")));
			publicFacilitesLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
			publicFacilitesLayer.FeatureSource.Projection = wgs84ToMercatorProj4Projection;
			poiOverlay.Layers.Add (publicFacilitesLayer);

			ShapeFileFeatureLayer restaurantsLayer = new ShapeFileFeatureLayer (InternalHelper.GetFullPath ("App_Data/POIs/Restaurants.shp"));
			restaurantsLayer.Transparency = 200f;
			restaurantsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle (new GeoImage (InternalHelper.GetFullPath (@"Images/restaurant.png")));
			restaurantsLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
			restaurantsLayer.FeatureSource.Projection = wgs84ToMercatorProj4Projection;
			poiOverlay.Layers.Add ("Restaurants", restaurantsLayer);

			ShapeFileFeatureLayer schoolsLayer = new ShapeFileFeatureLayer (InternalHelper.GetFullPath ("App_Data/POIs/Schools.shp"));
			schoolsLayer.Transparency = 200f;
			schoolsLayer.ZoomLevelSet.ZoomLevel10.DefaultPointStyle = new PointStyle (new GeoImage (InternalHelper.GetFullPath (@"Images/school.png")));
			schoolsLayer.ZoomLevelSet.ZoomLevel10.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
			schoolsLayer.FeatureSource.Projection = wgs84ToMercatorProj4Projection;
			poiOverlay.Layers.Add ("Schools", schoolsLayer);

			// Create the Routine Engine. 
			string streetShapeFilePathName = InternalHelper.GetFullPath ("App_Data/Street.shp");
			string streetRtgFilePathName = Path.ChangeExtension (streetShapeFilePathName, ".rtg");

			RtgRoutingSource routingSource = new RtgRoutingSource (streetRtgFilePathName);
			FeatureSource featureSource = new ShapeFileFeatureSource (streetShapeFilePathName);
			featureSource.Projection = wgs84ToMercatorProj4Projection;
			routingEngine = new RoutingEngine (routingSource, featureSource);
			routingEngine.GeographyUnit = GeographyUnit.Meter;

			// Initialise categories
			poiTypes = new Dictionary<string, Collection<string>> ();
			foreach (string layerId in new string[] { "Hotels", "Medical Facilites", "Restaurants", "Schools" }) {
				poiTypes.Add (layerId, GetPoiSubTypes (poiOverlay, layerId));
			}
		}

		[Route ("GetCategories")]
		public string GetCategories ()
		{
			// Return the categories in JSON to client side.
			return JsonConvert.SerializeObject (poiTypes);
		}

		[Route ("baselayer/{z}/{x}/{y}")]
		public HttpResponseMessage GetBaseLayerTile (int z, int x, int y)
		{
			using (Bitmap bitmap = new Bitmap (256, 256)) {
				PlatformGeoCanvas geoCanvas = new PlatformGeoCanvas ();
				RectangleShape boundingBox = WebApiExtentHelper.GetBoundingBoxForXyz (x, y, z, GeographyUnit.Meter);
				geoCanvas.BeginDrawing (bitmap, boundingBox, GeographyUnit.Meter);
				poiOverlay.Draw (geoCanvas);
				geoCanvas.EndDrawing ();

				MemoryStream ms = new MemoryStream ();
				bitmap.Save (ms, ImageFormat.Png);
				HttpResponseMessage msg = new HttpResponseMessage (HttpStatusCode.OK);
				msg.Content = new ByteArrayContent (ms.ToArray ());
				msg.Content.Headers.ContentType = new MediaTypeHeaderValue ("image/png");

				return msg;
			}
		}

		[Route ("Search")]
		[HttpPost]
		public string Search ([FromBody] string parameterString)
		{
			Dictionary<string, string> parameters = JsonConvert.DeserializeObject<Dictionary<string, string>> (parameterString);
			string[] searchPointLatLng = parameters ["searchPoint"].Split (',');
			PointShape searchPoint = InternalHelper.ConvertToSphericalMercator<PointShape> (new PointShape (double.Parse (searchPointLatLng [0]), double.Parse (searchPointLatLng [1])));

			string result = JsonConvert.SerializeObject (new { status = "2", message = "out of restriction area" });
			Feature searchAreaFeature = null;

			// check if the clicked point is in valid area (Frisco City)
			FeatureLayer friscoLayer = (FeatureLayer)poiOverlay.Layers ["friscoBoundary"];
			friscoLayer.Open ();
			if (friscoLayer.QueryTools.GetFeaturesContaining (searchPoint, ReturningColumnsType.NoColumns).Any ()) {
				// Calculate the service area/buffer area and display it on the map
				if (parameters ["searchMode"] == "serviceArea") {
					int drivingTimeInMinutes = Convert.ToInt32 (parameters ["driveTime"], CultureInfo.InvariantCulture);
					searchAreaFeature = new Feature (routingEngine.GenerateServiceArea (searchPoint, new TimeSpan (0, drivingTimeInMinutes, 0), 100, GeographyUnit.Meter));
				} else {
					DistanceUnit distanceUnit = parameters ["distanceUnit"] == "Mile" ? DistanceUnit.Mile : DistanceUnit.Kilometer;
					double distanceBuffer = Convert.ToDouble (parameters ["distanceBuffer"], CultureInfo.InvariantCulture);
					searchAreaFeature = new Feature (searchPoint.Buffer (distanceBuffer, 40, GeographyUnit.Meter, distanceUnit));
				}

				// Search the Pois in calculated service area and display them on map
				ShapeFileFeatureLayer poiLayer = (ShapeFileFeatureLayer)(poiOverlay.Layers [parameters ["category"]]);
				Collection<Feature> featuresInServiceArea = poiLayer.QueryTools.GetFeaturesWithin (searchAreaFeature.GetShape (), ReturningColumnsType.AllColumns);
				List<Feature> filteredQueryFeatures = FilterFeaturesByQueryConfiguration (featuresInServiceArea, parameters ["category"], parameters ["subCategory"].Replace (">~", ">="));

				if (filteredQueryFeatures.Any ()) {
					Collection<object> returnedJsonFeatures = new Collection<object> ();
					foreach (Feature feature in filteredQueryFeatures) {
						PointShape latlng = InternalHelper.ConvertToWgs84<PointShape> (feature.GetShape ());
						returnedJsonFeatures.Add (new { name = feature.ColumnValues ["NAME"], point = string.Format ("{0},{1}", latlng.Y, latlng.X) });
					}

					Feature wgs84Feature = new Feature (InternalHelper.ConvertToWgs84<BaseShape> (searchAreaFeature.GetShape ()));
					result = JsonConvert.SerializeObject (new { status = "0", message = "has features", area = wgs84Feature.GetGeoJson (), features = returnedJsonFeatures });
				} else {
					result = JsonConvert.SerializeObject (new { status = "1", message = "without features" });
				}
			}

			// return the search poi feature information to the client.
			return result;
		}

		private static Collection<string> GetPoiSubTypes (LayerOverlay queryingOverlay, string poiType)
		{
			Collection<string> poiSubTypes = new Collection<string> ();
			poiSubTypes.Add ("All");

			string columnName = InternalHelper.GetDbfColumnByPoiType (poiType);
			if (columnName.Equals ("Hotels")) {
				poiSubTypes.Add ("1 ~ 50");
				poiSubTypes.Add ("50 ~ 100");
				poiSubTypes.Add ("100 ~ 150");
				poiSubTypes.Add ("150 ~ 200");
				poiSubTypes.Add ("200 ~ 300");
				poiSubTypes.Add ("300 ~ 400");
				poiSubTypes.Add ("400 ~ 500");
				poiSubTypes.Add (">= 500");
			} else {
				ShapeFileFeatureLayer inMemoryFeatureLayer = (ShapeFileFeatureLayer)queryingOverlay.Layers [poiType];
				inMemoryFeatureLayer.Open ();
				IEnumerable<string> distinctColumnValues = inMemoryFeatureLayer.FeatureSource.GetDistinctColumnValues (columnName).Select (v => v.ColumnValue);
				foreach (string distinctColumnValue in distinctColumnValues) {
					poiSubTypes.Add (distinctColumnValue);
				}
			}

			return poiSubTypes;
		}

		private List<Feature> FilterFeaturesByQueryConfiguration (Collection<Feature> allFeatures, string category, string subCategory)
		{
			if (subCategory.Equals ("All")) {
				return allFeatures.ToList ();
			} else if (!category.Equals ("Hotels")) {
				return allFeatures.Where (f => f.ColumnValues [InternalHelper.GetDbfColumnByPoiType (category)] == subCategory).ToList ();
			} else {
				string queriedColumn = InternalHelper.GetDbfColumnByPoiType (category);

				List<Feature> queriedPois = new List<Feature> ();
				foreach (Feature feature in allFeatures) {
					int rooms = int.Parse (feature.ColumnValues [queriedColumn], CultureInfo.InvariantCulture);

					string[] values = subCategory.Split ('~');
					if (values.Length >= 2) {
						if (int.Parse (values [0], CultureInfo.InvariantCulture) <= rooms && int.Parse (values [1], CultureInfo.InvariantCulture) >= rooms) {
							queriedPois.Add (feature);
						}
					} else if (values.Length == 1) {
						int maxValue = int.Parse (values [0].TrimStart (new[] { '>', '=', ' ' }), CultureInfo.InvariantCulture);
						if (rooms > maxValue) {
							queriedPois.Add (feature);
						}
					}
				}
				return queriedPois;
			}
		}
	}
}