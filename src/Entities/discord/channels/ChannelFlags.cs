namespace Orikivo
{
    public class ChannelFlags
    {
        public ChannelFlags(bool isNsfw = false)
        {
            IsNsfw = isNsfw;
        }
        public bool IsNsfw { get; }
        //public bool IsNews { get; }
    }
}