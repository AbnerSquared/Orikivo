﻿using System;
using System.Collections.Generic;
using System.Linq;
using Orikivo;
using Orikivo.Framework;

namespace Arcadia
{
    public class CatalogGenerator
    {
        public CatalogGenerator() {}

        public CatalogGenerator(int size, int maxDiscountsAllowed = 0, int maxSpecialsAllowed = 0)
        {
            if (size <= 0)
                throw new ArgumentException("Cannot initialize a catalog with an empty or negative size.");

            Size = size;
            MaxDiscountsAllowed = maxDiscountsAllowed;
            MaxSpecialsAllowed = maxSpecialsAllowed;
            Entries = new List<CatalogEntry>();
        }

        public CatalogGenerator(int size, List<CatalogEntry> entries, int maxDiscountsAllowed = 0,
            int maxSpecialsAllowed = 0)
        {
            Size = size;

            if (!Check.NotNullOrEmpty(entries))
                throw new Exception("Expected at least entry for the catalog generator but returned empty");

            Entries = entries;
            MaxDiscountsAllowed = maxDiscountsAllowed;
            MaxSpecialsAllowed = maxSpecialsAllowed;
        }

        public int Size { get; internal set; }

        public int MaxDiscountsAllowed { get; internal set; }

        public int MaxSpecialsAllowed { get; internal set; }

        public List<CatalogEntry> Entries { get; internal set; }

        public List<CatalogGroup> Groups { get; internal set; }

        private CatalogGroup GetGroup(string groupId)
            => Groups.FirstOrDefault(x => x.Id == groupId) ?? throw new Exception("Expected to find the specified catalog group but returned null");

        /// <summary>
        /// Generates a new <see cref="ItemCatalog"/>.
        /// </summary>
        public ItemCatalog Generate(long tier = 1)
        {
            int specials = 0;
            int discounts = 0;
            var counters = new Dictionary<string, int>();
            var discountEntries = new Dictionary<string, int>();
            var groupCounters = new Dictionary<string, int>();
            int entries = 0;

            if (Size <= 0)
                throw new ArgumentException("Cannot initialize a catalog with an empty or negative size.");

            while (entries < Size)
            {
                CatalogEntry entry = GetNextEntry(tier, specials, counters, groupCounters);

                if (entry == null)
                {
                    break;
                }

                if (entry.IsSpecial)
                {
                    Logger.Debug($"Special entry applied ({entry.ItemId})");
                    specials++;
                }
                else
                {
                    Logger.Debug($"Next entry loaded ({entry.ItemId})");
                }

                if (entry.DiscountChance > 0 && entry.MaxDiscount.HasValue)
                {
                    if (!discountEntries.ContainsKey(entry.ItemId)
                        && discounts < MaxDiscountsAllowed
                        && RandomProvider.Instance.NextDouble() <= entry.DiscountChance)
                    {
                        // NOTE: Since the upper bound is exclusive, add 1 to the max discount, so that it's possible to land on the specified max
                        int discountToApply = RandomProvider.Instance
                            .Next(entry.MinDiscount ?? CatalogEntry.DefaultMinDiscount, (entry.MaxDiscount ?? CatalogEntry.DefaultMaxDiscount) + 1);

                        discountEntries.Add(entry.ItemId, discountToApply);

                        Logger.Debug($"{discountToApply}% discount applied ({entry.ItemId})");
                        discounts++;
                    }
                }

                if (!ItemHelper.Exists(entry.ItemId))
                    throw new Exception("The specified item ID could not be found.");

                if (!counters.TryAdd(entry.ItemId, 1))
                    counters[entry.ItemId]++;

                if (Check.NotNull(entry.GroupId))
                {
                    if (!groupCounters.TryAdd(entry.GroupId, 1))
                        groupCounters[entry.GroupId]++;
                }

                entries++;
            }

            Logger.Debug($"Compiling catalog with {entries} {Format.TryPluralize("entry", entries)}");

            return new ItemCatalog(counters, discountEntries);
        }

        public int GetBaseWeight()
            => Entries.Sum(x => x.Weight);

        private CatalogEntry[] GetAvailableEntries(long tier, int specials, IReadOnlyDictionary<string, int> counters, IReadOnlyDictionary<string, int> groupCounters)
        {
            return Entries.Where(x =>
                (x.RequiredTier <= 0 || (x.ForceAtTier ? tier == x.RequiredTier : tier >= x.RequiredTier))
                && (!x.IsSpecial || specials < MaxSpecialsAllowed)
                && (!Check.NotNull(x.GroupId) || groupCounters.GetValueOrDefault(x.GroupId, 0) >= GetGroup(x.GroupId).MaxAllowed)
                && (!x.MaxAllowed.HasValue || counters.GetValueOrDefault(x.ItemId, 0) >= x.MaxAllowed)).ToArray();
        }

        private CatalogEntry GetNextEntry(long tier, int specials, IReadOnlyDictionary<string, int> counters, IReadOnlyDictionary<string, int> groupCounters)
        {
            CatalogEntry[] entries = GetAvailableEntries(tier, specials, counters, groupCounters);
            int totalWeight = entries.Sum(x => x.Weight);
            int marker = RandomProvider.Instance.Next(0, totalWeight);
            int weightSum = 0;

            if (totalWeight == 0)
                return null;

            for (int i = 0; i < entries.Length; i++)
            {
                weightSum += entries[i].Weight;

                if (marker <= weightSum)
                    return Entries[i];
            }

            throw new Exception("Weighed marker was out of bounds.");
        }
    }
}
