using System.Collections.Generic;
using System.Text;
using Arcadia.Services;
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

        public Dictionary<string, int> ItemIds { get; set; }
        public long Money { get; set; }
        public ulong Exp { get; set; }

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

            
            if (Exp > 0)
                user.GiveExp(Exp);

            if (!Check.NotNullOrEmpty(ItemIds))
                return;

            foreach ((string itemId, int amount) in ItemIds)
                ItemHelper.GiveItem(user, itemId, amount);
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            if (Money > 0)
                result.AppendLine($"> {CurrencyHelper.WriteCost(Money, CurrencyType.Money)} {CurrencyHelper.GetName(CurrencyType.Money, Money)}");

            if (Exp > 0)
                result.AppendLine($"> {Icons.Exp} **{Exp:##,0}** Experience");

            if (Check.NotNullOrEmpty(ItemIds))
            {
                foreach ((string itemId, int amount) in ItemIds)
                    result.AppendLine($"> {ItemHelper.GetPreview(itemId, amount)}");
            }

            return result.ToString();
        }
    }
}