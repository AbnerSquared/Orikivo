namespace Orikivo
{
    // container for display content.
    public class GameDisplay
    {
        // defines what display is correlated to what game state it's meant for.
        public GameChannel Type { get; }
        public string SyncKey { get; private set; }
    }
}
