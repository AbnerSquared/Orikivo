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

        public Item Next()
        {
            int totalWeight = Entries.Sum(x => x.Weight);
            int marker = RandomProvider.Instance.Next(0, totalWeight);
            int weightSum = 0;

            if (totalWeight == 0)
                return null;

            string choice = Entries.First().ItemId;

            foreach (LootEntry entry in Entries)
            {
                weightSum += entry.Weight;

                if (marker > weightSum)
                    continue;

                choice = entry.ItemId;
                break;
            }

            return ItemHelper.GetItem(choice);
        }

        public Reward Next(int count)
        {
            if (count <= 0)
                throw new Exception("Invalid count specified, expected at least 1");

            var results = new Dictionary<string, int>();
            long money = 0;

            int totalWeight = Entries.Sum(x => x.Weight);

            if (totalWeight == 0)
            {
                return new Reward
                {
                    Money = money,
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
                        money += entry.Money;
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
                Money = money,
                ItemIds = results
            };
        }
    }
}
