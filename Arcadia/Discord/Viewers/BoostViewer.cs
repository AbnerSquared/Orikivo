using System;
using System.Text;
using Orikivo;

namespace Arcadia.Modules
{
    public static class BoostViewer
    {
        private static string WriteCurrentRates(ArcadeUser user)
        {
            var current = new StringBuilder();

            current.AppendLine($"> **Current Rates**");

            foreach (BoostType type in EnumUtils.GetValues<BoostType>())
                current.AppendLine($"> {WriteCurrentRate(user, type)}");

            return current.ToString();
        }

        public static string WriteCurrentRate(ArcadeUser user, BoostType type)
        {
            float rate = CurrencyHelper.GetBoostMultiplier(user, type);
            return $"{Icons.IconOf(type)} {Format.Percent(rate)}";
        }

        public static string Write(ArcadeUser user)
        {
            var info = new StringBuilder();

            info.AppendLine(Locale.GetHeader(Headers.Booster));
            info.AppendLine();
            info.AppendLine(WriteCurrentRates(user));

            int i = 0;
            foreach (BoostData booster in user.Boosters)
            {
                if (i > 0)
                    info.AppendLine();

                info.Append(WriteEntry(booster));

                i++;
            }

            return info.ToString();
        }

        public static bool IsNegative(float rate, BoostType type)
        {
            //if (type == BoosterType.Debt)
            //    return rate > 0;

            return rate < 0;
        }

        private static string WriteDefaultName(BoostData booster)
        {
            string prefix = IsNegative(booster.Rate, booster.Type) ? "Inhibitor: " : "Booster: ";

            return $"{prefix}{booster.Type}";
        }

        public static string WriteName(BoostData booster)
        {
            if (Check.NotNull(booster.ParentId))
                return ItemHelper.GetItem(booster.ParentId)?.Name ?? WriteDefaultName(booster);

            return WriteDefaultName(booster);
        }

        public static string WriteEffect(BoostData booster)
        {
            return $"{Icons.IconOf(booster.Type)} {Format.Percent(booster.Rate)}";
        }

        public static string WriteEntry(BoostData booster)
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
