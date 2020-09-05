using System;
using System.Collections.Generic;
using System.Text;
using Orikivo;

namespace Arcadia
{
    public static class Locale
    {
        private static readonly Dictionary<string, Header> LHeaders = new Dictionary<string, Header>
        {
            [Headers.Browser] = new Header
            {
                Title = "Server Browser",
                Icon = "🔍",
                Subtitle = "View all of the public game servers."
            },
            [Headers.Inventory] = new Header
            {
                Title = "Inventory",
                Icon = "📂",
                Subtitle = "View your contents currently in storage."
            },
            [Headers.Booster] = new Header
            {
                Title = "Boosters",
                Icon = Icons.Booster,
                Subtitle = "View all of your currently active boosters."
            },
            [Headers.Stat] = new Header
            {
                Title = "Stats",
                Icon = "⏱️"
            },
            [Headers.Recipe] = new Header
            {
                Title = "Recipes",
                Icon = "📒"
            },
            [Headers.UserBrowser] = new Header
            {
                Title = "User Browser",
                Subtitle = "Search for a specific user."
            },
            [Headers.GuildBrowser] = new Header
            {
                Title = "Guild Browser",
                Subtitle = "Search for a specific guild."
            }
        };

        public static string GetHeaderTitle(string id, string extra = null, string group = null)
        {
            if (!LHeaders.ContainsKey(id))
                return "UNKNOWN_HEADER";

            Header header = LHeaders[id];

            string title = header.Title;

            if (!string.IsNullOrWhiteSpace(group))
                title = $"{header.Title}: {group}";


            return GetHeaderText(title, header.Icon, extra: extra);
        }

        public static string GetHeader(string id, string extra = null, string subtitle = null, string group = null)
        {
            if (!LHeaders.ContainsKey(id))
                return "UNKNOWN_HEADER";

            Header header = LHeaders[id];

            string title = header.Title;

            if (!string.IsNullOrWhiteSpace(group))
                title = $"{header.Title}: {group}";

            return GetHeaderText(title, header.Icon, string.IsNullOrWhiteSpace(subtitle) ? header.Subtitle : subtitle, extra);
        }

        private static string GetHeaderText(string title, string icon = "", string subtitle = "", string extra = "")
        {
            if (!Check.NotNull(title))
                throw new ArgumentException("Expected header text to have a specified value");

            var header = new StringBuilder("> ");

            if (Check.NotNull(icon))
                header.Append($"{icon} ");

            header.Append($"**{title}**");

            if (Check.NotNull(extra))
                header.Append($" {extra}");

            if (Check.NotNull(subtitle))
                header.Append($"\n> {subtitle}");

            return header.ToString();
        }
    }
}
