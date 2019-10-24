namespace Orikivo
{
    public class StatUpdatePacket
    {
        public string Id { get; }
        public UpdateMethod Method { get; }
        public int Amount { get; }
    }
}
