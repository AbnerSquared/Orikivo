
using Orikivo.Desync;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo
{
    // represents a exchange performed at a market
    // this is used to quickly revert and summarize transactions.
    public class MarketHistory
    {
        public MarketHistory(Dictionary<string, int> itemIds, ulong value)
        {
            ItemIds = itemIds;
            Value = value;
        }

        public Dictionary<string, int> ItemIds { get; }
        public int Count => ItemIds.Values.Sum();
        public ulong Value { get; }

        // adds the items, and takes the money.
        public void ApplyBuy(User user, string marketId = null)
        {
            foreach (KeyValuePair<string, int> item in ItemIds)
            {
                if (!user.Husk.Backpack.ItemIds.TryAdd(item.Key, item.Value))
                    user.Husk.Backpack.ItemIds[item.Key] += item.Value;

                if (Check.NotNull(marketId))
                {
                    user.Brain.Catalogs[marketId].ItemIds[item.Key] -= item.Value;
                }
            }

            

            user.Take((long)Value);
        }

        // takes the items, and gives the money.
        public void ApplySell(User user)
        {
            foreach (KeyValuePair<string, int> item in ItemIds)
            {
                user.Husk.Backpack.ItemIds[item.Key] -= item.Value;

                if (user.Husk.Backpack.ItemIds[item.Key] == 0)
                    user.Husk.Backpack.ItemIds.Remove(item.Key);
            }

            user.Give((long)Value);

        }
    }
}
