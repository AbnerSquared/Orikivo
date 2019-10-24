namespace Orikivo
{
    public class UpgradeUpdatePacket
    {
        public string Id { get; }
        public int Amount { get; }
        public UpdateMethod Method { get; }
    }
}
