using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.UI;
using ThinkGeo.MapSuite.Geocoding;

namespace ThinkGeo.MapSuite.HowDoI
{
    internal static class SampleHelper
    {
        private static string DataRootPath = ConfigurationManager.AppSettings["RootDirectory"];

        public static string GetDataPath(this Page page, string path)
        {
            return Path.Combine(page.MapPath("~"), DataRootPath, path);
        }

        public static string GetDataRootPath(this Page page)
        {
            return Path.Combine(page.MapPath("~"), DataRootPath);
        }

        public static string GetMarkerPath(this Page page)
        {
            string path = "SampleFramework/Images/marker_blue.gif";
            return Path.Combine(page.MapPath("~"), path);
        }

        internal static string GetMatchItemText(GeocoderMatch matchItem, IEnumerable<string> keyList)
        {
            StringBuilder keyValuesBuilder = new StringBuilder();
            foreach (string item in keyList)
            {
                keyValuesBuilder.Append(string.Format(CultureInfo.InvariantCulture, "{0} ", matchItem.NameValuePairs[item]));
            }

            return keyValuesBuilder.ToString();
        }
    }
}
