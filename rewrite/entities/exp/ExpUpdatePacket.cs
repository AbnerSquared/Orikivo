namespace Orikivo
{
    public class ExpUpdatePacket
    {
        public ulong Amount { get; }
        public ExpType Type { get; }
        public UpdateMethod Method { get; }
    }
}
