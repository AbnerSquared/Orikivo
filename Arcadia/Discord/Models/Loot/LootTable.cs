using System;
using System.Collections.Generic;
using System.Linq;
using Orikivo;

namespace Arcadia
{
    public class LootTable
    {
        public string Id { get; set; }

        public List<LootEntry> Entries { get; set; } = new List<LootEntry>();

        public Reward Next(int count)
        {
            if (count <= 0)
                throw new Exception("Invalid count specified, expected at least 1");

            var results = new Dictionary<string, int>();
            var wagers = new Dictionary<CurrencyType, long>();

            int totalWeight = Entries.Sum(x => x.Weight);

            if (totalWeight == 0)
            {
                return new Reward
                {
                    ItemIds = results
                };
            }

            int rolls = 0;

            while (rolls < count)
            {
                int marker = RandomProvider.Instance.Next(0, totalWeight);
                int weightSum = 0;

                foreach (LootEntry entry in Entries)
                {
                    weightSum += entry.Weight;

                    if (marker > weightSum)
                        continue;

                    if (entry.Money > 0)
                    {
                        if (!wagers.TryAdd(entry.Currency, entry.Money))
                        {
                            wagers[entry.Currency] += entry.Money;
                        }
                    }
                    else if (!results.TryAdd(entry.ItemId, 1))
                    {
                        results[entry.ItemId]++;
                    }

                    break;
                }

                rolls++;
            }

            return new Reward
            {
                Wagers = wagers,
                ItemIds = results
            };
        }
    }
}
