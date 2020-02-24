using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Orikivo
{
    public enum PlaceValue
    {
        Null = -1, // 0
        H = 0,     // 100
        K = 1,     // 1,000
        M = 2,     // 1,000,000
        B = 3,     // 1,000,000,000
        T = 4      // 1,000,000,000,000
    }

    /// <summary>
    /// A utility class that handles string formatting.
    /// </summary>
    public static class OriFormat
    {
        public static readonly string DebugFormat = "[Debug] -- {0} --";
        public static readonly string VoiceChannelUrlFormat = "https://discordapp.com/channels/{0}/{1}";

        // TODO: You could use the Minic FontFace to utilize a way to draw sub/superscript on larger fonts.
        // ᵃᵇᶜᵈᵉᶠᵍʰᶤʲᵏˡᵐᶯᵒᵖʳˢᵗᵘᵛʷˣʸᶻ (superscript)
        private static readonly Dictionary<char, char> _superscriptMap = new Dictionary<char, char>
        {
            {'a', 'ᵃ'},
            {'b', 'ᵇ'},
            {'c', 'ᶜ'},
            {'d', 'ᵈ'},
            {'e', 'ᵉ'},
            {'f', 'ᶠ'},
            {'g', 'ᵍ'},
            {'h', 'ʰ'},
            {'i', 'ᶤ'},
            {'j', 'ʲ'},
            {'k', 'ᵏ'},
            {'l', 'ˡ'},
            {'m', 'ᵐ'},
            {'n', 'ᶯ'},
            {'o', 'ᵒ'},
            {'p', 'ᵖ'},
            // q
            {'r', 'ʳ'},
            {'s', 'ˢ'},
            {'t', 'ᵗ'},
            {'u', 'ᵘ'},
            {'v', 'ᵛ'},
            {'w', 'ʷ'},
            {'x', 'ˣ'},
            {'y', 'ʸ'},
            {'z', 'ᶻ'},
        };
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

        public static string TypeName(Type t)
            =>

               t == typeof(sbyte) ? "sbyte" :
               t == typeof(byte) ? "byte" :

               t == typeof(short) ? "short" :
               t == typeof(ushort) ? "ushort" :

               t == typeof(int) ? "int" :
               t == typeof(uint) ? "uint" :

               t == typeof(long) ? "long" :
               t == typeof(ulong) ? "ulong" :

               t == typeof(double) ? "double" :
               t == typeof(float) ? "float" :

               t == typeof(string) ? "string" :
               t.Name;

        /// <summary>
        /// Attempts to map all known characters to its subscript variant.
        /// </summary>
        public static string Subscript(string value)
            => SetUnicodeMap(value, UnicodeMap.Subscript);

        public static string GetShortValue(ulong value, out PlaceValue place, bool ignoreZeros = true)
        {
            string text = value.ToString();
            int valueLength = text.Length; // (12)(11)(10),(9)(8)(7),(6)(5)(4),(3)(2)(1)
            int placeValue = (int)MathF.Floor(valueLength / 3);

            // displaySize is 3 => 0.00 || 00.0 || 000
            int leftSize = (valueLength % 3 == 0) ? 3 : valueLength % 3; // 0 = length of 3, 1 = length of 1, 2 = length of 2
            // use the 0.00 value format, and return the letter indicating its placement value (k = thousands, m = millions, b = billions)

            if (leftSize < (int)PlaceValue.H || leftSize > (int)PlaceValue.T)
            {
                place = PlaceValue.Null;
            }
            else
            {
                place = (PlaceValue)placeValue;
            }

            if (valueLength <= 3)
            {
                return text;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(text.Substring(0, leftSize));

            if (leftSize < 3)
            {
                int rem = text.Length - (placeValue * 3);

                if (rem == 0)
                    return sb.ToString();

                int pos = valueLength % 3;
                Console.WriteLine(rem);
                // TODO: Optimize
                if (ignoreZeros)
                {
                    if (rem > 0)
                    {
                        if (text.Substring(pos, 1) == "0")
                        {
                            if (rem > 1)
                            {
                                if (text.Substring(pos + 1, 1) == "0")
                                    return sb.ToString();
                            }
                            else
                                return sb.ToString();
                        }
                    }
                }

                sb.Append('.');

                while (leftSize < 3)
                {
                    string v = text.Substring(pos, 1);
                    sb.Append(v);
                    pos++;
                    leftSize++;
                }

            }

            return sb.ToString();
        }

        public static string HyperlinkEmote(string parsedEmote, string emoteUrl, string emoteName)
            => $"{Format.Url(parsedEmote, emoteUrl)} {emoteName}";

        public static string Code(string value, CodeType? type = null)
            => $"```{type?.ToString().ToLower()}\n{value}```";

        public static string CropGameId(string value)
            => value.Length > 8 ? value.Substring(0, 8) + "..." : value;

        public static string Position(int i)
        {
            string value = Notate(i, false);
            return i switch
            {
                1 => $"{value}st",
                2 => $"{value}nd",
                3 => $"{value}rd",
                _ => $"{value}th",
            };
        }

        private const string POS_NOTATION = "##,0.###";
        public static string Notate(int i, bool includeDecimals = false)
            => i.ToString(includeDecimals ? POS_NOTATION : POS_NOTATION.Substring(0, 5));

        public static string Notate(ulong u, bool includeDecimals = false)
            => u.ToString(includeDecimals ? POS_NOTATION : POS_NOTATION.Substring(0, 5));

        public static string Error(string reaction = null, string title = null, string reason = null, string stackTrace = null, bool isEmbedded = false)
        {
            StringBuilder sb = new StringBuilder();
            // if embedded, the reaction is placed on the title.
            if (!isEmbedded)
                if (Check.NotNull(reaction))
                    sb.AppendLine(Format.Bold(reaction));

            if (Check.NotNull(title))
                sb.Append(title);

            if (Check.NotNull(reason))
                sb.Append($"```{reason}```");

            if (Check.NotNull(stackTrace))
            {
                if (Check.NotNull(reason))
                    sb.AppendLine();

                sb.Append($"```bf\n{stackTrace}```");
            }

            return sb.ToString();
        }

        public static string Hyperlink(string url)
            => Format.Url(Path.GetFileName(url), url);

        public static string GetVoiceChannelUrl(ulong guildId, ulong voiceChannelId)
            => string.Format(VoiceChannelUrlFormat, guildId, voiceChannelId);
        public static string SetUnicodeMap(string value, UnicodeMap type)
        {
            Dictionary<char, char> map = type switch
            {
                UnicodeMap.Subscript => _subscriptMap,
                _ => throw new ArgumentException("The specified UnicodeMap does not exist.")
            };

            foreach (char c in GetUniqueChars(value))
                if (map.ContainsKey(c))
                    value = value.Replace(c, map[c]);

            return value;
        }

        /// <summary>
        /// Returns a string in which each repeated character is removed.
        /// </summary>
        private static string GetUniqueChars(string value)
        {
            string unique = "";

            foreach (char c in value)
                if (!unique.Contains(c))
                    unique += c;

            return unique;
        }

        // TODO: Implement all noun forms based on the end of the word
        public static string GetNounForm(string word, int count)
            => $"{word}{(count > 1 || count == 0 || count < 0 ? $"{(word.EndsWith("h") ? "e" : "")}s" : "")}";

        public static string ReadType<T>()
            => $"**{typeof(T).Name}**";

        public static string Username(IUser user)
            => $"**{user.Username}**#{user.Discriminator}";

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
            
            return $"**{n.ToString("#.##")}**{t}";
        }

        // Might be scrapped.
        /// <summary>
        /// Returns the specified value into a JSON-friendly string.
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

        // TODO: Figure out a better way to filter the symbols utilized.
        public static string TextChannelName(string value)
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
