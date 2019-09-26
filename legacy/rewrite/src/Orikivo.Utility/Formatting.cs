using Discord;
using Orikivo.Static;
using System.Collections.Generic;

namespace Orikivo.Utility
{
    public enum LeaderboardType
    {
        Experience = 1,
        Balance = 2,
        MostHeld = 4,
        Expended = 8,
        MostWon = 16,
        MostLost = 32,
        Debt = 64,
        Mail = 128,
        Votes = 256,
        WinStreaks = 512,
        LossStreaks = 1024,
        Midas = 2048
    }

    public static class Formatting
    {
        public static string DiscordCodeBlock = "```{0}\n{1}\n```";
        public static string DiscordLineBlock = "`{0}`";
        public static string DiscordStrike = "~~{0}~~";
        public static string DiscordBold = "**{0}**";
        public static string DiscordItalic = "*{0}*";
        public static string DiscordUnderline = "__{0}__";
        public static string DiscordBoldItalic = "_**{0}**_";

        public static string ToBalance(ulong value)
        {
            return $"{EmojiIndex.Balance}{value.ToString("##,0").MarkdownBold()}";
        }

        public static string ToMostHeld(OldAccount a)
            => $"{EmojiIndex.MostHeld.Pack(a)}{a.Analytics.MaxHeld.ToPlaceValue().MarkdownBold()}";

        public static string ToDebt(ulong tax)
        {
            return $"{EmojiIndex.Debt}{tax.ToString("##,0").MarkdownBold()}";
        }

        public static string FullBalance(ulong value, ulong tax)
        {
            return $"{ToBalance(value)} **/** {ToDebt(tax)}";
        }

        public static string ToExperience(ulong value)
        {
            return $"**{EmojiIndex.Experience}{value.ToString("##,0")}**";
        }

        public static string ToLevel(long value)
        {
            return $"**[Level null (under construction)]**";
        }

        public static string ToAccount(OldAccount account)
        {
            return $"{ToBalance(account.Balance)}\n{ToExperience(account.Data.Experience)}";
        }
    }
}