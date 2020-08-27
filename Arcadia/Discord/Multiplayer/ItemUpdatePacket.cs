namespace Arcadia.Multiplayer
{
    public class ItemUpdatePacket
    {
        public ItemUpdatePacket(string id, int amount = 1)
        {
            Id = id;
            Amount = amount;
        }

        public string Id { get; set; }
        public int Amount { get; set; }
    }
}