using System;
using System.Collections.Generic;
using System.Linq;
using Orikivo;

namespace Arcadia
{
    public class LootContainer
    {
        public LootTable Loot { get; set; }

        public Item Next()
        {
            int totalWeight = Loot.Entries.Sum(x => x.Weight);
            int marker = RandomProvider.Instance.Next(0, totalWeight);
            int weightSum = 0;

            if (totalWeight == 0)
                return null;

            string choice = Loot.Entries.First().ItemId;

            for (int i = 0; i < Loot.Entries.Count; i++)
            {
                weightSum += Loot.Entries[i].Weight;

                if (marker <= weightSum)
                {
                    choice = Loot.Entries[i].ItemId;
                    break;
                }
            }

            return ItemHelper.GetItem(choice);
        }

        public Dictionary<string, int> Next(int count)
        {
            if (count <= 0)
                throw new Exception("Invalid count specified, expected at least 1");

            var results = new Dictionary<string, int>();

            int totalWeight = Loot.Entries.Sum(x => x.Weight);

            if (totalWeight == 0)
                return results;

            int rolls = 0;

            while (rolls < count)
            {
                int marker = RandomProvider.Instance.Next(0, totalWeight);
                int weightSum = 0;

                for (int i = 0; i < Loot.Entries.Count; i++)
                {
                    weightSum += Loot.Entries[i].Weight;

                    if (marker <= weightSum)
                    {
                        if (!results.TryAdd(Loot.Entries[i].ItemId, 1))
                        {
                            results[Loot.Entries[i].ItemId]++;
                        }

                        break;
                    }
                }

                rolls++;
            }

            return results;
        }
    }
}