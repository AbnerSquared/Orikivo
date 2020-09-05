
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
        public void ApplyBuy(Husk husk, HuskBrain brain, ref ulong balance, string marketId = null)
        {
            foreach ((string key, int value) in ItemIds)
            {
                if (!husk.Backpack.ItemIds.TryAdd(key, value))
                    husk.Backpack.ItemIds[key] += value;

                if (!string.IsNullOrWhiteSpace(marketId))
                {
                    brain.Catalogs[marketId].ItemIds[key] -= value;
                }
            }

            balance -= Value;
        }

        // takes the items, and gives the money.
        public void ApplySell(Husk husk, HuskBrain brain, ref ulong balance)
        {
            foreach ((string key, int value) in ItemIds)
            {
                husk.Backpack.ItemIds[key] -= value;

                if (husk.Backpack.ItemIds[key] == 0)
                    husk.Backpack.ItemIds.Remove(key);
            }

            balance += Value;

        }
    }
}
