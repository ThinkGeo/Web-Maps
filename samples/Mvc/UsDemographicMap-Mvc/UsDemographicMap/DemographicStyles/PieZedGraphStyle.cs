using System.Collections.Generic;
using ThinkGeo.MapSuite.Drawing;
using ThinkGeo.MapSuite.Styles;

namespace ThinkGeo.MapSuite.USDemographicMap
{
    public class PieZedGraphStyle : ZedGraphStyle
    {
        private Dictionary<string, GeoColor> pieSlices;

        public PieZedGraphStyle()
            : base()
        { }

        public Dictionary<string, GeoColor> PieSlices
        {
            get
            {
                if (pieSlices == null)
                {
                    pieSlices = new Dictionary<string, GeoColor>();
                }
                return pieSlices;
            }
        }
    }
}