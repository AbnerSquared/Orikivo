using System;
using System.Text;
using Orikivo;

namespace Arcadia.Modules
{
    public static class BoostHelper
    {
        private static string WriteCurrentRates(ArcadeUser user)
        {
            var current = new StringBuilder();

            current.AppendLine($"> **Current Rates**");

            foreach (BoosterType type in EnumUtils.GetValues<BoosterType>())
                current.AppendLine($"> {WriteCurrentRate(user, type)}");

            return current.ToString();
        }

        public static string WriteCurrentRate(ArcadeUser user, BoosterType type)
        {
            float rate = ItemHelper.GetBoostMultiplier(user, type);
            return $"{Icons.IconOf(type)} {Format.Percent(rate)}";
        }

        private static string WriteHeader()
        {
            return $"> {Icons.Booster} **Boosters**\n> View all of your currently active boosters.";
        }

        public static string Write(ArcadeUser user)
        {
            var info = new StringBuilder();

            info.AppendLine(WriteHeader());
            info.AppendLine();
            info.AppendLine(WriteCurrentRates(user));

            int i = 0;
            foreach (BoosterData booster in user.Boosters)
            {
                if (i > 0)
                    info.AppendLine();

                info.Append(WriteEntry(booster));

                i++;
            }

            return info.ToString();
        }

        public static bool IsNegative(float rate, BoosterType type)
        {
            //if (type == BoosterType.Debt)
            //    return rate > 0;

            return rate < 0;
        }

        private static string WriteDefaultName(BoosterData booster)
        {
            string prefix = IsNegative(booster.Rate, booster.Type) ? "Inhibitor: " : "Booster: ";

            return $"{prefix}{booster.Type}";
        }

        private static string WriteName(BoosterData booster)
        {
            if (Check.NotNull(booster.ParentId))
                return ItemHelper.GetItem(booster.ParentId)?.Name ?? WriteDefaultName(booster);

            return WriteDefaultName(booster);
        }

        public static string WriteEffect(BoosterData booster)
        {
            return $"{Icons.IconOf(booster.Type)} {Format.Percent(booster.Rate)}";
        }

        public static string WriteEntry(BoosterData booster)
        {
            var info = new StringBuilder();

            info.AppendLine($"> **{WriteName(booster)}**");
            info.AppendLine($"> {WriteEffect(booster)}");

            if (booster.ExpiresOn.HasValue)
                info.AppendLine(WriteExpiry(booster.ExpiresOn.Value));

            if (booster.UsesLeft.HasValue)
                info.AppendLine(WriteUseCounter(booster.UsesLeft.Value));

            return info.ToString();
        }

        private static string WriteUseCounter(int usesLeft)
        {
            return $"**Breaks in {usesLeft:##,0} {Format.TryPluralize("use", usesLeft)}**";
        }

        private static string WriteExpiry(DateTime expiresOn)
        {
            return $"**Expires in {Format.Counter((DateTime.UtcNow - expiresOn).TotalSeconds, false)}**";
        }
    }
}
