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
    public static class Format
    {
        private const string GROUP_FORMATTING = "##,0.###";
        private static readonly string[] SensitiveCharacters = { "\\", "*", "_", "~", "`", "|", ">" };
        public static readonly string VoiceChannelUrlFormat = "https://discordapp.com/channels/{0}/{1}";

        public static bool IsSensitive(string text)
            => text.ContainsAny("\\", "*", "_", "~", "`", "|", ">");

        // TODO: Use Discord.Net's version of Quote(text)
        public static string Quote(string text)
        {
            if (!Check.NotNull(text))
                return text;

            return $"> {text}";
        }

        public static string BlockQuote(string text)
        {
            if (!Check.NotNull(text))
                return text;

            return $">>> {text}";
        }

        public static string Bold(string text)
            => $"**{text}**";

        public static string Italics(string text)
            => $"*{text}*";

        public static string Underline(string text)
            => $"__{text}__";

        public static string StrikeThrough(string text)
            => $"~~{text}~~";

        public static string Spoiler(string text)
            => $"||{text}||";

        public static string Url(string text, string url)
            => $"[{text}]({url})";

        public static string EscapeUrl(string url)
            => $"<{url}>";

        public static string LineCode(string text)
            => $"`{text}`";

        public static string BlockCode(string text)
            => $"```\n{text}\n```";

        public static string Code(string text, string language = null)
        {
            if (language != null || text.Contains("\n"))
                return $"```{language ?? ""}\n{text}\n```";

            return $"`{text}`";
        }

        public static string Hyperlink(string url)
            => Url(Path.GetFileName(url), url);

        public static string HyperlinkEmote(string parsedEmote, string emoteUrl, string emoteName)
            => $"{Url(parsedEmote, emoteUrl)} {emoteName}";

        public static string Trim(string value, int limit)
            => value.Length > limit ? $"{value[..limit]}..." : value;

        public static string Header(string text, string icon = null, string description = null)
        {
            var header = new StringBuilder("> ");


            if (Check.NotNull(icon))
                header.Append(icon);

            header.Append(Bold(text));

            if (Check.NotNull(description))
                header.Append($"\n> {description}");

            return header.ToString();
        }

        public static string Tooltip(string text)
            => $"> 🛠️ {text}";

        public static string Warning(string text)
            => $"> ⚠️ {text}";



        private static readonly Dictionary<char, char> SuperscriptMap = new Dictionary<char, char>
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

        private static readonly Dictionary<char, char> SubscriptMap = new Dictionary<char, char>
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

        private static readonly Dictionary<string, string> UniquePlurals = new Dictionary<string, string>
        {
            ["foot"] = "feet",
            ["tooth"] = "teeth",
            ["goose"] = "geese",
            ["man"] = "men",
            ["woman"] = "women"
        };

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
                t == typeof(decimal) ? "decimal" :
                t == typeof(string) ? "string" :
                t.Name;

        public static string HumanizeType<T>()
            => HumanizeType(typeof(T));

        public static string Time(DateTime time)
            => time.ToString(@"HH\:mm\:ss");

        public static string Date(DateTime date, char separator = '/')
            => date.ToString($"M{separator}d{separator}yyyy");

        public static string FullTime(DateTime time, char separator = '/')
            => time.ToString($"M{separator}d{separator}yyyy @ HH:mm tt");

        public static string Countdown(TimeSpan remaining)
            => remaining.ToString(@"hh\:mm\:ss");

        

        /// <summary>
        /// Attempts to map all known characters to its subscript variant.
        /// </summary>
        public static string Subscript(string value)
            => SetUnicodeMap(value, UnicodeMap.Subscript);

        private const string CLOCK_1 = "🕐";
        private const string CLOCK_2 = "🕑";
        private const string CLOCK_3 = "🕒";
        private const string CLOCK_4 = "🕓";
        private const string CLOCK_5 = "🕔";
        private const string CLOCK_6 = "🕕";
        private const string CLOCK_7 = "🕖";
        private const string CLOCK_8 = "🕗";
        private const string CLOCK_9 = "🕘";
        private const string CLOCK_10 = "🕙";
        private const string CLOCK_11 = "🕚";
        private const string CLOCK_12 = "🕛";

        public static string GetHourEmote(int hour)
        {
            switch (hour)
            {
                case 11:
                case 23:
                    return CLOCK_11;

                case 10:
                case 22:
                    return CLOCK_10;

                case 9:
                case 21:
                    return CLOCK_9;

                case 8:
                case 20:
                    return CLOCK_8;

                case 7:
                case 19:
                    return CLOCK_7;

                case 6:
                case 18:
                    return CLOCK_6;

                case 5:
                case 17:
                    return CLOCK_5;

                case 4:
                case 16:
                    return CLOCK_4;

                case 3:
                case 15:
                    return CLOCK_3;

                case 2:
                case 14:
                    return CLOCK_2;

                case 1:
                case 13:
                    return CLOCK_1;

                default:
                    return CLOCK_12;
            }
        }


        // 70 => 1.06m
        public static string Counter(double seconds)
        {
            if (seconds < 0)
                seconds *= -1;

            double upper = seconds;
            int i = 1;

            while (upper > GetNextDenominator(i))
            {
                upper /= GetNextDenominator(i);

                if (i < 7)
                    i++;
                else
                    break;
            }

            return $"**{upper:##,0.##}**{GetCounterPrefix(i)}";
        }

        public static string RawCounter(double milliseconds)
        {
            if (milliseconds < 0)
                milliseconds *= -1;

            double upper = milliseconds;
            int i = 0;

            while (upper > GetNextDenominator(i))
            {
                upper /= GetNextDenominator(i);

                if (i < 7)
                    i++;
                else
                    break;
            }

            return $"{upper:##,0.##}{GetCounterPrefix(i)}";
        }

        // 92399 => 92.3k
        public static string Condense(ulong value, out NumberGroup group, bool ignoreZeros = true)
        {
            
            string text = value.ToString();
            int length = text.Length;
            int pre = length % 3;

            group = (NumberGroup)MathF.Floor(length / 3);

            if (group > 0)
                group--;

            if (group < NumberGroup.H || group > NumberGroup.T)
                group = NumberGroup.Null;

            if (length <= 3)
                return text;

            if (pre == 0)
                pre = 3;

            var result = new StringBuilder();
            result.Append(text[..pre]);

            if (pre >= 3)
                return result.ToString();

            int rem = length - (int) group * 3;

            if (ignoreZeros)
            {
                if (rem > 0)
                {
                    if (text[pre] == '0')
                    {
                        if (rem <= 1)
                            return result.ToString();
                        
                        if (text[pre + 1] == '0')
                            return result.ToString();
                    }
                }
            }

            result.Append('.');

            while (pre < 3)
            {
                result.Append(text[pre]);
                pre++;
            }
            
            return result.ToString();
        }

        public static string Ordinal(int i)
        {
            return GetLastDigit(i) switch
            {
                '1' => $"{Separate(i)}st",
                '2' => $"{Separate(i)}nd",
                '3' => $"{Separate(i)}rd",
                _ => $"{Separate(i)}th",
            };
        }

        private static char GetLastDigit(int number)
            => number.ToString()[^1];

        public static string Separate(float f, bool includeDecimals = false)
            => f.ToString(includeDecimals ? GROUP_FORMATTING : GROUP_FORMATTING[..4]);

        public static string Separate(double d, bool includeDecimals = false)
            => d.ToString(includeDecimals ? GROUP_FORMATTING : GROUP_FORMATTING[..4]);

        public static string Separate(decimal d, bool includeDecimals = false)
            => d.ToString(includeDecimals ? GROUP_FORMATTING : GROUP_FORMATTING[..4]);

        public static string Separate(long l)
            => l.ToString(GROUP_FORMATTING);
        
        public static string Separate(int i)
            => i.ToString(GROUP_FORMATTING);

        public static string Separate(ulong u)
            => u.ToString(GROUP_FORMATTING);

        

        public static string Error(string reaction = null, string title = null, string reason = null, string stackTrace = null, bool isEmbedded = false)
        {
            var result = new StringBuilder();

            // NOTE: If the error is embedded, the reaction is placed on the title.
            if (!isEmbedded)
            {
                if (Check.NotNull(reaction))
                    result.AppendLine(Bold(reaction));
            }

            if (Check.NotNull(title))
                result.Append(title);

            if (Check.NotNull(reason))
                result.Append($"```{reason}```");

            if (!Check.NotNull(stackTrace))
                return result.ToString();

            if (Check.NotNull(reason))
                result.AppendLine();

            result.Append(Code(stackTrace, "bf"));

            return result.ToString();
        }

        public static string GetVoiceChannelUrl(ulong guildId, ulong voiceChannelId)
            => string.Format(VoiceChannelUrlFormat, guildId, voiceChannelId);

        public static string SetUnicodeMap(string value, UnicodeMap type)
        {
            Dictionary<char, char> map = type switch
            {
                UnicodeMap.Subscript => SubscriptMap,
                UnicodeMap.Superscript => SuperscriptMap,
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
            {
                if (!unique.Contains(c))
                    unique += c;
            }

            return unique;
        }

        // TODO: Implement all noun forms based on the end of the word
        public static string TryPluralize(string word, int count)
        {
            if (string.IsNullOrWhiteSpace(word))
                return word;

            if (count == 0 || count == 1)
                return word;

            return Pluralize(word);
        }

        // TODO: Handle upper case words
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
            if (UniquePlurals.ContainsKey(word))
                return UniquePlurals[word];

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
                return word[..^suffix.Length] + "ves";
            }

            if (word.EndsWith("y"))
            {
                // ^2 == word.Length - 2
                if (IsConsonant(word[^2]))
                    return word[..^1] + "ies";
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
                return word[..^2] + "i";

            // criterion => criteria
            if (word.EndsWith("on"))
                return word[..^2] + "a";

            // axis => axes
            if (word.EndsWith("is"))
                return word[..^2] + "es";

            // datum => data
            if (word.EndsWith("um"))
                return word[..^2] + "a";
            

            // this might not be right, but it's worth a shot
            if (word.EndsWith("ies"))
                return word;

            return word + "s";
        }

        private static bool IsConsonant(char letter, bool includeY = false)
        {
            if (!char.IsLetter(letter))
                return false;

            if (char.ToLower(letter) == 'y')
                return includeY;

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

            if (char.ToLower(letter) == 'y')
                return includeY;

            return char.ToLower(letter)
                .EqualsAny('a', 'e', 'i', 'o', 'u');
        }

        private static string GetCounterPrefix(int upperIndex)
        {
            return upperIndex switch
            {
                7 => "y",
                6 => "mo",
                5 => "w",
                4 => "d",
                3 => "h",
                2 => "m",
                1 => "s",
                _ => "ms"
            };
        }

        private static double GetNextDenominator(int upperIndex)
        {
            return upperIndex switch
            {
                7 => 12,
                6 => 30.44, // average month
                5 => 7,
                4 => 24,
                0 => 1000,
                _ => 60
            };
        }
    }
}
