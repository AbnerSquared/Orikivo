namespace Orikivo
{
    public class StatUpdateInfo
    {
        public string Key { get; set; } // the name of the stat
        public StatUpdateType Type { get; set; } // the method of how the value written is used when updating.
        public int Value { get; set; } // the value to update the stat with
    }
}
