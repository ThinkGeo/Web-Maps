namespace ThinkGeo.UI.Blazor.HowDoI
{
    public class MenuModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Href { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public string Preicon { get; set; }

        /// <summary>The section of the menu this sample sits under; null for the top.</summary>
        public string Group { get; set; }
    }
}
