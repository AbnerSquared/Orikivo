using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace Orikivo
{
    public enum CharMapType
    {
        Subscript = 1
    }

    // used to fancify text and whatknot.
    public static class OriFormat
    {
        public static string DebugFrame = "[Debug] -- {0} --";
        // ᵃᵇᶜᵈᵉᶠᵍʰᶤʲᵏˡᵐᶯᵒᵖʳˢᵗᵘᵛʷˣʸᶻ (superscript)
        // ₀₁₂₃₄₅₆₇₈₉₊₋₌₍₎ (subscript)
        private static Dictionary<char, char> _subscriptMap = new Dictionary<char, char>
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

        public static string Subscript(string value)
            => MapChars(value, CharMapType.Subscript);

        public static string CropGameId(string value)
            => value.Length > 8 ? value.Substring(0, 8) + "..." : value;
        private static string MapChars(string value, CharMapType mapType)
        {
            Dictionary<char, char> map = null;
            switch(mapType)
            {
                case CharMapType.Subscript:
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
        public static string ParseGreeting(string greeting, IUser user) // OriCommandContext
            => greeting
            .Replace("{user}", user.Username)
            .Replace("{mention_user}", user.Mention)
            .Replace("{date}", DateTime.UtcNow.ToString(@"mm-dd-yyyy"));

        public static string ConvertHtmlTags(string value)
        {
            StringBuilder sb = new StringBuilder();
            return sb.ToString();
        }
        public static string GetNounForm(string word, int count)
            => $"{word}{(count > 1 || count == 0 || count < 0 ? "s" : "")}";

        public static string ReadType<T>()
            => $"**{typeof(T).Name}**";

        public static string GetUserName(IUser user)
            => $"**{user.Username}**#{user.Discriminator}";

        public static string PlaceValue(double d)
            => d.ToString("##,0.###");

        public static string GetShortTime(double d)
        {
            char t = 's';
            if (d > (60 * 60))
            {
                t = 'h';
                return $"**{d / (60 * 60)}**{t}";
            }
            if (d > 60)
            {
                t = 'm';
                return $"**{d / 60}**{t}";
            }
            
            return $"**{d}**{t}";
        }
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
            int maxLen = 100;

            if (string.IsNullOrWhiteSpace(value))
                throw new Exception("The value used to format cannot be empty.");

            if (value.Length > maxLen)
                value = value.Substring(0, maxLen);

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
