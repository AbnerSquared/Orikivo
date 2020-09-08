using System.Collections.Generic;
using Orikivo;

namespace Arcadia
{
    public class Reward
    {
        public Reward()
        {
            ItemIds = new Dictionary<string, int>();
            Money = 0;
            Exp = 0;
        }
        //public static IEnumerable<string> GetNames(Reward reward)
        //{

        //}

        public Dictionary<string, int> ItemIds { get; set; }
        public long Money { get; set; }
        public long Exp { get; set; }

        public int Count => GetCount();

        private int GetCount()
        {
            int count = ItemIds?.Count ?? 0;

            if (Money > 0)
                count++;

            if (Exp > 0)
                count++;

            return count;
        }

        public void Apply(ArcadeUser user)
        {
            if (Money > 0)
                user.Give(Money);

            // if (Exp > 0)
            //    user.Exp += Exp;

            if (!Check.NotNullOrEmpty(ItemIds))
                return;

            foreach ((string itemId, int amount) in ItemIds)
                ItemHelper.GiveItem(user, itemId, amount);
        }
    }
}