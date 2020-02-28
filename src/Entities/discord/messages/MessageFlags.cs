namespace Orikivo
{
    public class MessageFlags
    {
        public MessageFlags(bool isTts = false, bool isPinned = false)
        {
            IsTts = isTts;
            IsPinned = isPinned;
        }
        public bool IsTts { get; }
        public bool IsPinned { get; }
    }
}