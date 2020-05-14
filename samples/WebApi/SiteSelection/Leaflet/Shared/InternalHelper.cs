using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Layers;
using ThinkGeo.MapSuite.Shapes;
using ThinkGeo.MapSuite.Styles;
using ThinkGeo.MapSuite.WebApi;

namespace SiteSelection
{
    public static class InternalHelper
    {
        private static Dictionary<string, string> poiColumns;
        private static Proj4Projection projection;

        public static string GetDbfColumnByPoiType(string poiCategory)
        {
            if (poiColumns == null)
            {
                poiColumns = new Dictionary<string, string>();
                poiColumns.Add("Hotels", "Hotels");
                poiColumns.Add("Medical Facilites", "TYPE");
                poiColumns.Add("Restaurants", "FoodType");
                poiColumns.Add("Schools", "TYPE");
            }

            return poiColumns[poiCategory];
        }

        public static string GetFullPath(string fileName)
        {
            Uri uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            string rootDirectory = Path.GetDirectoryName(Path.GetDirectoryName(uri.LocalPath));
            return Path.Combine(rootDirectory, fileName); ;
        }

        public static T ConvertToWgs84<T>(BaseShape baseShape) where T : BaseShape
        {
            InitializeProjection();
            return projection.ConvertToInternalProjection(baseShape) as T;
        }

        public static T ConvertToSphericalMercator<T>(BaseShape baseShape) where T : BaseShape
        {
            InitializeProjection();
            return projection.ConvertToExternalProjection(baseShape) as T;
        }

        private static void InitializeProjection()
        {
            if (projection == null)
            {
                projection = new Proj4Projection();
                projection.InternalProjectionParametersString = Proj4Projection.GetWgs84ParametersString();
                projection.ExternalProjectionParametersString = Proj4Projection.GetSphericalMercatorParametersString();
                projection.Open();
            }
        }
    }
}