namespace ThinkGeo.UI.WebApi.HowDoI.Samples
{
    /// <summary>
    /// What a group of samples asks of the server: the overlays it serves through the shared
    /// tile routes, and any routes of its own.
    /// </summary>
    public interface ISampleGroup
    {
        void Register(OverlayCatalog catalog);

        void MapEndpoints(IEndpointRouteBuilder app);
    }

    public static class SampleGroups
    {
        public static readonly ISampleGroup[] All =
        {
            new Navigation(),
            new Tiles(),
            new Projections(),
            new Services(),
            new Cloud(),
            new YourData(),
            new Shapes(),
            new Styling(),
            new Dynamic(),
            new Tools(),
            new Extending(),
            new Printing(),
            new GettingStarted(),
            new NauticalCharts(),
            new PrinterLayout(),
        };
    }
}
