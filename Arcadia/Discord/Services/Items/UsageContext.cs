namespace Arcadia
{
    public class UsageContext
    {
        public UsageContext(ArcadeUser user, string input, ItemData data = null)
        {
            User = user;
            Input = input;
            Data = data;
        }

        public ArcadeUser User { get; }

        public string Input { get; }

        public ItemData Data { get; }
    }
}
