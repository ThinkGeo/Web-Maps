using System.Collections.Generic;
using ThinkGeo.MapSuite.Core;
using ZedGraph;

namespace ThinkGeo.MapSuite.USDemographicMap
{
    public class PieZedGraphStyle : ZedGraphStyle
    {
        private Dictionary<string, GeoColor> pieItems;

        public PieZedGraphStyle()
            : base()
        { }

        public Dictionary<string, GeoColor> PieItems
        {
            get
            {
                if (pieItems == null)
                {
                    pieItems = new Dictionary<string, GeoColor>();
                }
                return pieItems;
            }
        }
    }
}