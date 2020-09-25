namespace Arcadia
{
    public class UsageContext
    {
        public UsageContext(ArcadeUser user, string input, ItemData data)
        {
            User = user;
            Input = input;
            Data = data;
            Item = ItemHelper.GetItem(data.Id);
        }

        public ArcadeUser User { get; }

        public string Input { get; }

        public ItemData Data { get; }

        public Item Item { get; }
    }
}
