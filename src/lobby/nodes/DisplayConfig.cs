namespace Orikivo
{
    public class DisplayConfig
    {
        public DisplayConfig()
        {
            Title = "New Display";
            DefaultGroupSize = 8;
            SyncKeyLength = 8;
            ShowSyncKey = false;
        }

        public string Title { get; set; }
        public int DefaultGroupSize { get; set; }
        public int SyncKeyLength { get; set; }
        public bool ShowSyncKey { get; set; }
    }
}
