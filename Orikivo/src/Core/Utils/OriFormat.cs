using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Orikivo
{
    /// <summary>
    /// A utility class that handles string formatting.
    /// </summary>
    public static class OriFormat
    {
        public static readonly string VoiceChannelUrlFormat = "https://discordapp.com/channels/{0}/{1}";

        // TODO: You could use the Minic FontFace to utilize a way to draw sub/superscript on larger fonts.
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

        public static string Time(DateTime time)
            => time.ToString("M/d/yyyy @ HH:mm tt");

        public static string ShowTime(DateTime time)
            => time.ToString(@"hh\:mm\:ss");

        public static string HumanizeType(Type t)
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

        public static string ShortenValue(ulong value, out PlaceValue place, bool ignoreZeros = true)
        {
            string text = value.ToString();
            int length = text.Length;
            int placeValue = (int)MathF.Floor(length / 3);

            if (placeValue > 0)
                placeValue--;

            int preDecimalSize = length % 3;

            if (preDecimalSize == 0)
                preDecimalSize = 3;

            int postDecimalSize = 3 - preDecimalSize;

            if (placeValue < (int)Orikivo.PlaceValue.H
                || placeValue > (int)Orikivo.PlaceValue.T)
            {
                place = Orikivo.PlaceValue.Null;
            }
            else
            {
                place = (PlaceValue)placeValue;
            }

            if (length <= 3)
            {
                return text;
            }

            var result = new StringBuilder();
            result.Append(text[0..preDecimalSize]);

            if (preDecimalSize == 3)
                return result.ToString();

            if (preDecimalSize < 3)
            {
                int pos = length % 3;
                int rem = length - (placeValue * 3);
                if (ignoreZeros)
                {
                    if (rem > 0)
                    {
                        if (text[preDecimalSize] == '0')
                        {
                            if (rem > 1)
                            {
                                if (text[preDecimalSize + 1] == '0')
                                    return result.ToString();
                            }
                            else
                                return result.ToString();
                        }
                    }
                }

                result.Append('.');

                int i = preDecimalSize;

                while (i < 3)
                {
                    result.Append(text[pos]);
                    pos++;
                    i++;
                }
            }

            return result.ToString();
        }

        public static string HyperlinkEmote(Emote emote)
            => HyperlinkEmote(emote.ToString(), emote.Url, emote.Name);

        public static string HyperlinkEmote(string parsedEmote, string emoteUrl, string emoteName)
            => $"{Format.Url(parsedEmote, emoteUrl)} {emoteName}";

        public static string Code(string value, CodeType? type = null)
            => $"```{type?.ToString().ToLower()}\n{value}```";

        // this crops a string with ... if the length of the string is past the specified limit
        public static string Crop(string value, int limit)
            => value.Length > limit ? value.Substring(0, limit) + "..." : value;

        public static string GetPosition(int i)
        {
            string value = PlaceValue(i, false);
            return i switch
            {
                1 => $"{value}st",
                2 => $"{value}nd",
                3 => $"{value}rd",
                _ => $"{value}th",
            };
        }

        private const string POS_NOTATION = "##,0.###";

        public static string PlaceValue(int i, bool includeDecimals = false)
            => i.ToString(includeDecimals ? POS_NOTATION : POS_NOTATION.Substring(0, 5));

        public static string PlaceValue(ulong u, bool includeDecimals = false)
            => u.ToString(includeDecimals ? POS_NOTATION : POS_NOTATION.Substring(0, 5));

        public static string Error(string reaction = null, string title = null, string reason = null, string stackTrace = null, bool isEmbedded = false)
        {
            var sb = new StringBuilder();
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
                UnicodeMap.Superscript => _superscriptMap,
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
        public static string GetUniqueChars(string value)
        {
            string unique = "";

            foreach (char c in value)
                if (!unique.Contains(c))
                    unique += c;

            return unique;
        }

        // TODO: Implement all noun forms based on the end of the word
        public static string TryPluralize(string word, int count)
        {
            if (string.IsNullOrWhiteSpace(word))
                return word;

            string result = word;

            if (count > 1 || count <= 0)
            {
                result = Pluralize(word);
            }

            return result;
        }

        private static bool IsConsonant(char letter, bool includeY = false)
        {
            if (!char.IsLetter(letter))
                return false;

            if (includeY)
                if (char.ToLower(letter) == 'y')
                    return true;

            return char.ToLower(letter)
                .EqualsAny('b', 'c', 'd', 'f',
                'g', 'h', 'j', 'k', 'l', 'm',
                'n', 'p', 'q', 'r', 's', 't',
                'v', 'w', 'x', 'z');
        }

        private static bool IsVowel(char letter, bool includeY = false)
        {
            if (!char.IsLetter(letter))
                return false;

            if (includeY)
                if (char.ToLower(letter) == 'y')
                    return true;

            return char.ToLower(letter)
                .EqualsAny('a', 'e', 'i', 'o', 'u');
        }

        private static Dictionary<string, string> uniquePlurals => new Dictionary<string, string>
        {
            ["foot"] = "feet",
            ["tooth"] = "teeth",
            ["goose"] = "geese",
            ["man"] = "men",
            ["woman"] = "women"
        };

        // this also has to handle possible capital letters.
        private static string Pluralize(string word, bool isFormal = false)
        {
            // no point in trying if the word doesn't exist.
            if (string.IsNullOrWhiteSpace(word))
                return word;

            // this is where special exception case words are handled
            // all words here are ignored if specified
            if (word.EqualsAny("sheep", "fish", "moose", "swine", "buffalo", "shrimp", "trout"))
                return word;

            // likewise, this one is a bit harder to specify
            // for now, we can just use a dictionary look-up
            // try to make a method for pluralization in relation to -oot -ooth -an, etc.
            if (uniquePlurals.ContainsKey(word))
                return uniquePlurals[word];

            // if the specified word is shorter than 3 letters, just return it with an s.
            if (word.Length < 3)
                return word + "s";


            // regular nouns => word + s
            // if singular noun ends in -s, -ss, -sh, -ch, -x, -z => word + es
            if (word.EndsWithAny("s", "ss", "sh", "ch", "x", "z"))
            {
                // index => indices (IF FORMAL)
                if (word.EndsWithAny("ex", "ix"))
                    if (isFormal)
                        return word[0..^2] + "ices";

                // in some cases, singular nouns ending in -s or -z require doubling the -s/-z before adding -es
                if (word.EndsWithAny("s", "z"))
                    return word + word.Last() + "es";

                return word + "es";
            }

            if (word.EndsWithAny(out string suffix, "f", "fe"))
            {
                return word[0..(word.Length - suffix.Length)] + "ves";
            }

            if (word.EndsWith("y"))
            {
                // ^2 == word.Length - 2
                if (IsConsonant(word[^2]))
                    return word[0..^1] + "ies";
            }

            if (word.EndsWith("o"))
            {
                // this is where implementing exceptions for specific words come into play.
                return word + "es";
            }

            // radius => radii
            if (word.EndsWith("us"))
                // word[0..^2] == word.Substring(0, word.Length - 2)
                // this is selecting the range of characters FROM 0 TO word.Length - 2
                return word[0..^2] + "i";

            // criterion => criteria
            if (word.EndsWith("on"))
                return word[0..^2] + "a";

            // axis => axes
            if (word.EndsWith("is"))
                return word[0..^2] + "es";

            // datum => data
            if (word.EndsWith("um"))
                return word[0..^2] + "a";
            

            // this might not be right, but it's worth a shot
            if (word.EndsWith("ies"))
                return word;

            return word + "s";
        }

        public static string HumanizeType<T>()
            => HumanizeType(typeof(T));

        public static string Username(IUser user)
            => $"**{user.Username}**#{user.Discriminator}";

        public static string GetRawTime(double milliseconds)
        {
            string t = "ms";
            double n = 0;

            if (milliseconds > (60 * 60 * 60))
            {
                n = milliseconds / (60 * 60 * 60);
                t = "h";
            }
            if (milliseconds > (60 * 60))
            {
                n = milliseconds / (60 * 60);
                t = "m";
            }
            if (milliseconds > 60)
            {
                n = milliseconds / 60;
                t = "s";
            }
            
            if (n > 0)
                return $"{n.ToString("#.##")}{t}";

            return $"{milliseconds.ToString()}{t}";
        }

        public static string GetShortTime(double seconds)
        {
            if (seconds < 0)
                seconds *= -1;

            char t = 's';
            double n = seconds;
            if (seconds > (60 * 60)) // if seconds is larger than 1 hour (in seconds)
            {
                n = seconds / (60 * 60);
                t = 'h';
            }
            else if (seconds > 60) // if seconds is larger than 1 minute (in seconds)
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
            var result = new StringBuilder();
            char? lastChar = null;
            foreach(char c in value)
            {
                if (char.IsUpper(c))
                    if (lastChar.HasValue)
                        if (char.IsLower(lastChar.Value))
                        {
                            result.Append($"_{char.ToLower(c)}");
                            lastChar = c;
                            continue;
                        }

                result.Append(char.ToLower(c));
                lastChar = c;
            }

            return result.ToString();
        }

        // TODO: Figure out a better way to filter the symbols utilized.
        public static string TextChannelName(string value)
        {
            const int MAX_LENGTH = 100;

            if (value.Length > MAX_LENGTH)
                value = value.Substring(0, MAX_LENGTH);

            // regex (([A-Za-z0-9-_ ])*)
            var limitedChars = new List<char>
            { '`', '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '+', '=',
             '{', '[', '}', ']', '|', '\\', ':', ';', '\'', '"', '<', ',', '>', '.',
             '/', '?', ' ' }; // allow characters: A-Z a-z 0-9 - _ (Valid Unicode)
            var separatorChars = new List<char>
            { ' ', '.', ',' };

            var result = new StringBuilder();
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
