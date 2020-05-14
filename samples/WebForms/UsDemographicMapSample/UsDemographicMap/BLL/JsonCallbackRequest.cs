using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace ThinkGeo.MapSuite.USDemographicMap
{
    [DataContract]
    public class JsonCallbackRequest
    {
        private string request;
        private string requestStyle;
        private string requestColumnAlias;
        private Collection<string> selectedCategoryItems;
        private string startColor;
        private string endColor;
        private string colorWheelDirection;
        private int sliderValue;
        private string selectedFeatureId;

        public JsonCallbackRequest()
        {
        }

        [DataMember(Name = "request")]
        public string Request
        {
            get { return request; }
            set { request = value; }
        }

        public DemographicStyleBuilderType StyleBuildType
        {
            get
            {
                DemographicStyleBuilderType type = DemographicStyleBuilderType.Thematic;
                switch (requestStyle.ToLowerInvariant())
                {
                    case "pie":
                        type = DemographicStyleBuilderType.PieChart;
                        break;
                    case "thematic":
                        type = DemographicStyleBuilderType.Thematic;
                        break;
                    case "dotdensity":
                        type = DemographicStyleBuilderType.DotDensity;
                        break;
                    case "valuecircle":
                        type = DemographicStyleBuilderType.ValueCircle;
                        break;
                }
                return type;
            }
        }

        [DataMember(Name = "style")]
        private string RequestStyle
        {
            get { return requestStyle; }
            set { requestStyle = value; }
        }

        [DataMember(Name = "alias")]
        public string RequestColumnAlias
        {
            get { return requestColumnAlias; }
            set { requestColumnAlias = value; }
        }

        [DataMember(Name = "selectedItems")]
        public Collection<string> SelectedCategoryItems
        {
            get { return selectedCategoryItems; }
            internal set { selectedCategoryItems = value; }
        }

        [DataMember(Name = "startColor")]
        public string StartColor
        {
            get { return startColor; }
            set { startColor = value; }
        }

        [DataMember(Name = "endColor")]
        public string EndColor
        {
            get { return endColor; }
            set { endColor = value; }
        }

        [DataMember(Name = "colorDirection")]
        public string ColorWheelDirection
        {
            get { return colorWheelDirection; }
            set { colorWheelDirection = value; }
        }

        [DataMember(Name = "sliderValue")]
        public int SliderValue
        {
            get { return sliderValue; }
            set { sliderValue = value; }
        }

        [DataMember(Name = "selectedFeatureId")]
        public string SelectedFeatureId
        {
            get { return selectedFeatureId; }
            set { selectedFeatureId = value; }
        }
    }
}