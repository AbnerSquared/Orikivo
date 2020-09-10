using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Orikivo;
using Orikivo.Text.Pagination;

namespace Arcadia
{
    public class TextSection
    {
        public string Icon { get; set; }

        public string Title { get; set; }

        public virtual string Content { get; set; }

        public string Build()
        {
            var result = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(Title))
                result.AppendLine(Format.Header(Title, Icon, useMarkdown: false));

            if (!string.IsNullOrWhiteSpace(Content))
                result.AppendLine(Content);

            return result.ToString();
        }
    }

    public class TextList : TextSection
    {
        public string Bullet { get; set; }

        public List<string> Values { get; set; } = new List<string>();

        /// <inheritdoc />
        public override string Content => Format.List(Values, Bullet);
    }

    public class TextBody
    {
        public List<string> Tooltips { get; set; } = new List<string>();

        public Header Header { get; set; }

        public List<TextSection> Sections { get; set; } = new List<TextSection>();

        public string Build(bool allowTooltips = true)
            => Locale.BuildMessage(this, allowTooltips);
    }

    public static class Locale
    {
        public const int MaxMessageLength = 2000;

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
            },
            [Headers.Merits] = new Header
            {
                //Icon = "🏆",
                Title = "Merits"
            },
            [Headers.Catalog] = new Header
            {
                Icon = "🗃️",
                Title = "Catalog"
            }
        };

        public static string BuildMessage(TextBody template, bool allowTooltips = true)
        {
            var result = new StringBuilder();

            if (template.Tooltips.Count > 0 && allowTooltips)
                result.AppendLine(Format.Tooltip(template.Tooltips)).AppendLine();

            if (Check.NotNull(template.Header))
                result.AppendLine(BuildHeader(template.Header));

            IEnumerable<string> sections = template?.Sections.Select(x => x.Build()).ToList();

            if (Check.NotNullOrEmpty(sections))
            {
                result.AppendLine();
                result.AppendJoin("\n", sections);
            }

            if (result.Length == 0 || result.Length > MaxMessageLength)
                throw new ArgumentOutOfRangeException(nameof(template), "The specified text body is outside of the specified message range.");

            return result.ToString();
        }

        private static bool IsValidSection(TextSection section)
            => section != null && (!string.IsNullOrWhiteSpace(section.Title) || !string.IsNullOrWhiteSpace(section.Content));

        public static string GetHeaderTitle(string id, string extra = null, string group = null, string icon = null)
        {
            if (!LHeaders.ContainsKey(id))
                return "UNKNOWN_HEADER";

            Header header = LHeaders[id];

            string title = header.Title;

            if (!string.IsNullOrWhiteSpace(group))
                title = $"{header.Title}: {group}";


            return GetHeaderText(title, icon ?? header.Icon, extra: extra);
        }

        public static Header GetOrCreateHeader(string id)
            => LHeaders.ContainsKey(id) ? new Header(LHeaders[id]) : new Header();

        public static string GetHeader(string id, string extra = null, string subtitle = null, string group = null, string icon = null)
        {
            if (!LHeaders.ContainsKey(id))
                return "UNKNOWN_HEADER";

            Header header = LHeaders[id];

            string title = header.Title;

            if (!string.IsNullOrWhiteSpace(group))
                title = $"{header.Title}: {group}";

            return GetHeaderText(title, icon ?? header.Icon, string.IsNullOrWhiteSpace(subtitle) ? header.Subtitle : subtitle, extra);
        }

        public static string BuildHeader(Header header)
            => GetHeaderText(!string.IsNullOrWhiteSpace(header.Group) ? $"{header.Title}: {header.Group}" : header.Title, header.Icon, header.Subtitle, header.Extra);

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
