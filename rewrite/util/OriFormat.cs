using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    /// <summary>
    /// A formatter tool used to help format simple sentences.
    /// </summary>
    public static class OriFormat
    {
        public static readonly string DebugFormat = "[Debug] -- {0} --";
        public static readonly string VoiceChannelUrlFormat = "https://discordapp.com/channels/{0}/{1}";
        public static readonly string HyperlinkFormat = "[{0}]({1})";
        // ᵃᵇᶜᵈᵉᶠᵍʰᶤʲᵏˡᵐᶯᵒᵖʳˢᵗᵘᵛʷˣʸᶻ (superscript)
        // ₀₁₂₃₄₅₆₇₈₉₊₋₌₍₎ (subscript)
        private static readonly Dictionary<char, char> _subscriptMap = new Dictionary<char, char>
        {
            {'0', '₀'},
            {'1', '₁'},
            {'2', '₂'},
            {'3', '₃'},
            {'4', '₄'},
            {'5', '₅'},
            {'6', '₆'},
            {'7', '₇'},
            {'8', '₈'},
            {'9', '₉'},
            {'+', '₊'},
            {'-', '₋'},
            {'=', '₌'},
            {'(', '₍'},
            {')', '₎'}
        };

        /// <summary>
        /// Attempts to map all known characters to its subscript variant.
        /// </summary>
        public static string Subscript(string value)
            => MapChars(value, CharMap.Subscript);

        public static string CodeBlock(string value, CodeType? type = null)
            => $"```{type?.ToString().ToLower()}\n{value}```";

        public static string CropGameId(string value)
            => value.Length > 8 ? value.Substring(0, 8) + "..." : value;

        public static string Hyperlink(string text, string url)
            => string.Format(HyperlinkFormat, text, url);

        public static string CreateVoiceChannelUrl(ulong guildId, ulong voiceChannelId)
            => string.Format(VoiceChannelUrlFormat, guildId, voiceChannelId);
        private static string MapChars(string value, CharMap mapType)
        {
            Dictionary<char, char> map = null;
            switch(mapType)
            {
                case CharMap.Subscript:
                    map = _subscriptMap;
                    break;
                default:
                    throw new Exception("The specified CharMapType was not found.");
            }
            foreach (char c in value)
                if (map.ContainsKey(c))
                    value = value.Replace(c, map[c]);
            return value;
        }

        // use Regex to single out the {} values.
        /// <summary>
        /// Parses the format of a greeting and replaces all specified keys into their known type.
        /// </summary>
        public static string ParseGreeting(string greeting, IGuild guild, IUser user) // OriCommandContext
            => greeting // ({[A-Za-z._]}) <= Regex format. Regex.Matches(Pattern);
            .Replace("{user}", user.Username)
            .Replace("{mention_user}", user.Mention)
            .Replace("{date}", DateTime.UtcNow.ToString(@"mm-dd-yyyy"));

        // Convert HTML format tags and convert them into Discord markup tags?
        public static string ConvertHtmlTags(string value)
            => throw new NotImplementedException();

        public static string GetNounForm(string word, int count)
            => $"{word}{(count > 1 || count == 0 || count < 0 ? "s" : "")}";

        public static string ReadType<T>()
            => $"**{typeof(T).Name}**";

        public static string ReadUserName(IUser user)
            => $"**{user.Username}**#{user.Discriminator}";

        /// <summary>
        /// Converts the specified number to be read out in place value format.
        /// </summary>
        public static string PlaceValue(double d)
            => d.ToString("##,0.###");

        public static string GetShortTime(double seconds)
        {
            char t = 's';
            double n = seconds;
            if (seconds > (60 * 60)) // if seconds is larger than 1 hour (in seconds)
            {
                n = seconds / (60 * 60);
                t = 'h';
            }
            if (seconds > 60) // if seconds is larger than 1 minute (in seconds)
            {
                n = seconds / 60;
                t = 'm';
            }
            
            return $"**{n}**{t}";
        }

        /// <summary>
        /// Converts the specified value into a JSON-friendly string.
        /// </summary>
        public static string Jsonify(string value)
        {
            StringBuilder sb = new StringBuilder();
            char? lastChar = null;
            foreach(char c in value)
            {
                if (char.IsUpper(c))
                    if (lastChar.HasValue)
                        if (char.IsLower(lastChar.Value))
                        {
                            sb.Append($"_{char.ToLower(c)}");
                            lastChar = c;
                            continue;
                        }

                sb.Append(char.ToLower(c));
                lastChar = c;
            }

            return sb.ToString();
        }

        public static string FormatTextChannelName(string value)
        {
            const int MAX_LENGTH = 100;

            if (value.Length > MAX_LENGTH)
                value = value.Substring(0, MAX_LENGTH);

            // regex (([A-Za-z0-9-_ ])*)
            List<char> limitedChars = new List<char>
            { '`', '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '+', '=',
             '{', '[', '}', ']', '|', '\\', ':', ';', '\'', '"', '<', ',', '>', '.',
             '/', '?', ' ' }; // allow characters: A-Z a-z 0-9 - _ (Valid Unicode)
            List<char> separatorChars = new List<char>
            { ' ', '.', ',' };

            StringBuilder result = new StringBuilder();
            foreach (char c in value)
            {
                if (limitedChars.Contains(c))
                {
                    if (separatorChars.Contains(c))
                        result.Append(c == ' ' ? '-' : '_');
                }
                else
                    result.Append(c);
            }

            return result.ToString();
        }
    }
}
