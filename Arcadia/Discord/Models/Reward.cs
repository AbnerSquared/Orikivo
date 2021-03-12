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
            Wagers = new Dictionary<CurrencyType, long>();
            Money = 0;
            Exp = 0;
        }

        public Reward(IEnumerable<Reward> rewards)
        {
            var itemIds = new Dictionary<string, int>();
            var wagers = new Dictionary<CurrencyType, long>();
            long money = 0;
            ulong exp = 0;

            foreach (Reward reward in rewards)
            {
                if (reward == null)
                    continue;

                money += reward.Money;
                exp += reward.Exp;
                itemIds.AddOrConcat(reward.ItemIds);
                wagers.AddOrConcat(reward.Wagers);
            }

            ItemIds = itemIds;
            Money = money;
            Exp = exp;
        }

        public Dictionary<string, int> ItemIds { get; set; }
        public Dictionary<CurrencyType, long> Wagers { get; set; }
        public long Money
        {
            get
            {
                Wagers.TryGetValue(CurrencyType.Money, out long money);
                return money;
            }
            set
            {
                if (value <= 0 && Wagers.ContainsKey(CurrencyType.Money))
                    Wagers.Remove(CurrencyType.Money);
                else if (!Wagers.TryAdd(CurrencyType.Money, value))
                    Wagers[CurrencyType.Money] = value;
            }
        }
        public ulong Exp { get; set; }

        public int Count => GetCount();

        private int GetCount()
        {
            int count = ItemIds?.Count ?? 0;

            count += Wagers?.Count ?? 0;

            if (Money > 0)
                count++;

            if (Exp > 0)
                count++;

            return count;
        }

        /// <summary>
        /// Adds the content from the specified <see cref="Reward"/> to this instance.
        /// </summary>
        public void Add(Reward reward)
        {
            if (reward == null)
                return;

            ItemIds.AddOrConcat(reward.ItemIds);
            Wagers.AddOrConcat(reward.Wagers);

            Money += reward.Money;
            Exp += reward.Exp;
        }

        public void Apply(ArcadeUser user)
        {
            //if (!Check.NotNullOrEmpty(Wagers) && Money > 0)
            //    user.Give(Money);

            if (Exp > 0)
                user.GiveExp(Exp);

            if (!Check.NotNullOrEmpty(Wagers))
            {
                foreach ((CurrencyType currency, long amount) in Wagers)
                {
                    user.Give(amount, currency);
                }
            }

            if (!Check.NotNullOrEmpty(ItemIds))
            {
                foreach ((string itemId, int amount) in ItemIds)
                    ItemHelper.GiveItem(user, itemId, amount);
            }


        }

        public override string ToString()
        {
            var result = new StringBuilder();

            //if (Money > 0)
            //   result.AppendLine($"> {CurrencyHelper.WriteCost(Money, CurrencyType.Money)} {CurrencyHelper.GetName(CurrencyType.Money, Money)}");

            if (Check.NotNullOrEmpty(Wagers))
            {
                foreach ((CurrencyType currency, long amount) in Wagers)
                {
                    result.AppendLine($"> {CurrencyHelper.WriteCost(amount, currency)} {CurrencyHelper.GetName(currency, amount)}");
                }
            }

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