using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Mvc;
using ThinkGeo.MapSuite.Shapes;

namespace CSharp_HowDoISamples
{
    public partial class InteractiveMapController : Controller
    {
        public ActionResult InteractWithMap()
        {
            Collection<HotelInfomation> hotels = GetHotels();

            return View(hotels);
        }

        [MapActionFilter]
        public void HighlighHotel(Map map, GeoCollection<object> args)
        {
            if (map != null)
            {
                string featureId = args["featureid"].ToString();

                LayerOverlay dynamicOverlay = (LayerOverlay)map.CustomOverlays["dynamic"];
                ShapeFileFeatureLayer shapeLayer = dynamicOverlay.Layers["points"] as ShapeFileFeatureLayer;
                shapeLayer.Open();
                Feature feature = shapeLayer.FeatureSource.GetFeatureById(featureId, ReturningColumnsType.NoColumns);

                if (feature != null)
                {
                    InMemoryFeatureLayer highlightLayer = dynamicOverlay.Layers["highlight"] as InMemoryFeatureLayer;
                    highlightLayer.Open();
                    highlightLayer.InternalFeatures.Clear();
                    highlightLayer.InternalFeatures.Add(feature);
                }
            }
        }

        [MapActionFilter]
        public string HighlighPoint(Map map, GeoCollection<object> args)
        {
            string featureId = string.Empty;
            if (map != null)
            {
                PointShape pointShape = new PointShape(Convert.ToDouble(args[0]), Convert.ToDouble(args[1]));
                LayerOverlay dynamicOverlay = (LayerOverlay)map.CustomOverlays["dynamic"];
                ShapeFileFeatureLayer shapeLayer = dynamicOverlay.Layers["points"] as ShapeFileFeatureLayer;
                shapeLayer.Open();
                pointShape = (PointShape)shapeLayer.FeatureSource.Projection.ConvertToInternalProjection(pointShape);

                ShapeFileFeatureSource hotelFeatureSource = new ShapeFileFeatureSource(Server.MapPath("~/App_Data/Hotels.shp"));
                hotelFeatureSource.Open();

                Feature feature = hotelFeatureSource.GetFeaturesNearestTo(pointShape, map.MapUnit, 1, ReturningColumnsType.NoColumns)[0];
                if (feature != null)
                {
                    featureId = feature.Id;
                    InMemoryFeatureLayer highlightLayer = dynamicOverlay.Layers["highlight"] as InMemoryFeatureLayer;
                    highlightLayer.Open();
                    highlightLayer.InternalFeatures.Clear();
                    highlightLayer.InternalFeatures.Add(feature);
                }
            }
            return featureId;
        }

        private Collection<HotelInfomation> GetHotels()
        {
            Collection<HotelInfomation> hotels = new Collection<HotelInfomation>();
            ShapeFileFeatureSource hotelFeatureSource = new ShapeFileFeatureSource(Server.MapPath("~/App_Data/Hotels.shp"));
            hotelFeatureSource.Open();
            Collection<Feature> features = hotelFeatureSource.GetAllFeatures(ReturningColumnsType.AllColumns);
            foreach (var item in features)
            {
                HotelInfomation hotel = new HotelInfomation();
                hotel.FeatureId = item.Id;
                foreach (var keyValuePair in item.ColumnValues)
                {
                    switch (keyValuePair.Key.ToLowerInvariant())
                    {
                        case "name":
                            hotel.Name = keyValuePair.Value;
                            break;
                        case "address":
                            hotel.Address = keyValuePair.Value;
                            break;
                        case "rooms":
                            hotel.Rooms = int.Parse(keyValuePair.Value, CultureInfo.InvariantCulture);
                            break;
                        default:
                            break;
                    }
                }
                hotels.Add(hotel);
            }
            return hotels;
        }
    }
}
