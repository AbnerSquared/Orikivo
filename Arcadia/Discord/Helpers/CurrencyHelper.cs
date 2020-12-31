using System;
using System.Collections.Generic;
using Orikivo;

namespace Arcadia
{
    public static class CurrencyHelper
    {
        public static long GetBalance(ArcadeUser user, CurrencyType currency)
        {
            return currency switch
            {
                CurrencyType.Chips => user.ChipBalance,
                CurrencyType.Money => user.Balance,
                CurrencyType.Debt => user.Debt,
                CurrencyType.Tokens => user.TokenBalance,
                _ => throw new ArgumentException("Unknown currency type")
            };
        }

        public static string GetName(CurrencyType currency, long value)
        {
            bool isPlural = value != 0 && value > 1;
            return currency switch
            {
                CurrencyType.Chips => Format.TryPluralize("Chip", isPlural),
                CurrencyType.Money => Format.TryPluralize("Orite", isPlural),
                CurrencyType.Debt => "Debt",
                CurrencyType.Tokens => Format.TryPluralize("Token", isPlural),
                _ => "UNKNOWN_CURRENCY"
            };
        }

        public static string WriteCost(long value, CurrencyType currency)
            => value <= 0 ? "**Unknown Cost**" : $"{Icons.IconOf(currency)} **{value:##,0}**"; // {(value == 69 ? "(nice)" : "")}

        public static bool CanAddBooster(BoostData boost)
        {
            return (!boost.ExpiresOn.HasValue || !CooldownHelper.IsExpired(boost.ExpiresOn.Value))
                && (!boost.UsesLeft.HasValue  || boost.UsesLeft > 0);
        }

        public static float GetBoostMultiplier(ArcadeUser user, BoostTarget type)
        {
            float rate = 1f;
            var toRemove = new List<BoostData>();

            foreach (BoostData boost in user.Boosters)
            {
                if (!CanAddBooster(boost))
                    toRemove.Add(boost);

                if (boost.Type != type)
                    continue;

                rate += boost.Rate;
            }

            RemoveBoosts(user, ref toRemove);
            return rate < 0 ? 0 : rate;
        }

        public static long BoostValue(ArcadeUser user, long value, BoostTarget type, bool isNegative = false)
        {
            float rate = 1;

            if (user.Boosters.Count == 0)
                return value;

            var toRemove = new List<BoostData>();
            foreach (BoostData booster in user.Boosters)
            {
                if (!CanAddBooster(booster))
                    toRemove.Add(booster);

                if (booster.Type != type)
                    continue;

                rate += booster.Rate;

                if (booster.UsesLeft.HasValue && --booster.UsesLeft <= 0)
                    toRemove.Add(booster);
            }

            RemoveBoosts(user, ref toRemove);
            return BoostConvert.GetValue(value, rate, isNegative);
        }

        private static void RemoveBoosts(ArcadeUser user, ref List<BoostData> toRemove)
        {
            foreach (BoostData booster in toRemove)
            {
                user.Boosters.Remove(booster);

                if (!string.IsNullOrWhiteSpace(booster.ParentId))
                    continue;

                Item parent = ItemHelper.GetItem(booster.ParentId);
                parent?.Usage?.OnBreak?.Invoke(user);
            }
        }
    }
}
