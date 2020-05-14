using System;
using System.Globalization;
using System.Text;
using System.Web.Mvc;
using ThinkGeo.MapSuite;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Mvc;

namespace CSharp_HowDoISamples_for_Debug
{
    public partial class StylesController : Controller
    {
        //
        // GET: /ConvertAGeoColorToAndFromOleWin32HtmlArgbColors/

        public ActionResult GeoColorConversion()
        {
            return View();
        }

        [MapActionFilter]
        public string ColorConversion(Map map, GeoCollection<object> args)
        {
            string resultString = String.Empty;

            string geoColorName = args[0] as string;
            GeoColor geoColor = GeoColor.GeographicColors.ShallowOcean;
            switch (geoColorName)
            {
                case "ShallowOcean":
                    geoColor = GeoColor.GeographicColors.ShallowOcean;
                    break;
                case "Sand":
                    geoColor = GeoColor.GeographicColors.Sand;
                    break;
                case "Lake":
                    geoColor = GeoColor.GeographicColors.Lake;
                    break;
                case "Silver":
                    geoColor = GeoColor.SimpleColors.Silver;
                    break;
                case "Green":
                    geoColor = GeoColor.SimpleColors.Green;
                    break;
                case "Transparent":
                    geoColor = GeoColor.StandardColors.Transparent;
                    break;
                default:
                    break;
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(CultureInfo.InvariantCulture, "{0}|", GeoColor.ToOle(geoColor).ToString());
            builder.AppendFormat(CultureInfo.InvariantCulture, "{0}|", GeoColor.ToHtml(geoColor));
            builder.AppendFormat(CultureInfo.InvariantCulture, "{0}|", GeoColor.ToWin32(geoColor).ToString());
            builder.AppendFormat(CultureInfo.InvariantCulture, "{0}|", string.Format("A:{0}  R:{1}  G:{2}  B:{3}", geoColor.AlphaComponent, geoColor.RedComponent, geoColor.GreenComponent, geoColor.BlueComponent));

            map.MapBackground = new GeoSolidBrush(geoColor);
            map.StaticOverlay.ClientCache.CacheId = geoColorName;

            resultString = builder.ToString();
            return resultString;
        }
    }
}
