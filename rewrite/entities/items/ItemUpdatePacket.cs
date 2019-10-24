namespace Orikivo
{
    public class ItemUpdatePacket
    {
        public string Id { get; }
        public UniqueItemData Unique { get; }
        public int Amount { get; }

        public ItemUpdateMethod Method { get; }
    }
}
