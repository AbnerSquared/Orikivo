namespace Orikivo
{
    // the actual display. tasks can bind their display to a tab upon start.
    /// <summary>
    /// A game display built off of elements.
    /// </summary>
    public class GameTab : ElementCluster
    {
        public GameTab(GameTabProperties properties) : base(properties?.Elements, properties?.Groups, properties?.Capacity)
        {
            Name = properties?.Name ?? "new-tab";
        }

        public string WindowId { get; internal set; }
        public string Name { get; }
        public string Id => $"{(Checks.NotNull(WindowId) ? $"{WindowId}:": "")}tab.{Name}";
        public bool IncludeHeader { get; internal set; } = true;
        public bool IncludeFooter { get; internal set; } = true;
    }
}
