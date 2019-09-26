using Orikivo.Static;
using System.Collections.Generic;
using System;
using System.Linq;
using Discord;

namespace Orikivo
{
    public static class CharacterExtension
    {
        private static List<Dictionary<char, string>> _all = UnicodeDictionary.All;
        private static List<List<char>> _default = UnicodeDictionary.Defaults;
        
        public static string EmojifyCharacter(this char c, bool large = true)
            => $"{c.TryConvertEmoji()}{(large ? UnicodeIndex.Space : UnicodeIndex.Invisible)}";
        
        private static string EmojifyLetter(this char c)
            => $":regional_indicator_{c}:";

        private static string EmojifyNumber(this char c)
            => $":{int.Parse($"{c}").Written()}:";
        
        private static string Written(this int i)
        {
            switch(i)
            {
                case 0: return "zero";
                case 1: return "one";
                case 2: return "two";
                case 3: return "three";
                case 4: return "four";
                case 5: return "five";
                case 6: return "six";
                case 7: return "seven";
                case 8: return "eight";
                case 9: return "nine";
                default: return "zero";
            }
        }

        private static string EmojifySymbol(this char c)
        {
            if (c == '#')
                return ":hash:";
            else if (c == '*')
                return ":asterisk:";
            else if (c == '!')
                return ":grey_exclamation:";
            else if (c == '?')
                return ":grey_question:";
            else
                return "";
        }

        public static Emoji TryConvertEmoji(this char c)
        {
            string r = "";

            if (c.IsLetter())
                r = c.EmojifyLetter();

            else if (c.IsDigit())
                r = c.EmojifyNumber();

            else if (c.IsEmojiSymbol())
                r = c.EmojifySymbol();

            return new Emoji(r);
        }

        public static string ToUnicodeCharacter(this char c, Dictionary<char, string> lib)
        {
            if (lib.ContainsKey(c))
                return lib[c];
            else
            {
                if (Options.DiscordSubstitution)
                    if (_all.Any(x => !x.Equals(lib) && x.ContainsValue($"{c}")) || _default.Any(x => c.EqualsAny(x)))
                        return $"{c}".ToMarker(lib);
                
                return $"{c}";
            }
        }
        
        public static string FromUnicodeCharacter(this string s, Dictionary<char, string> lib)
            => lib.ContainsValue(s) ? $"{lib.First(x => x.Value == s).Key}" : s;

        public static string FromAnyUnicodeCharacter(this string s)
            => _all.Any(x => x.ContainsValue(s)) ? $"{s.FromUnicodeCharacter(_all.First(x => x.ContainsValue(s)))}" : s;
        
        public static bool AreOperators(this char[] a)
            => a.All(x => x.EqualsAny(UnicodeIndex.Operators));

        public static bool IsAlphanumeric(this char c)
            => c.IsDigit() || c.IsLetter();

        public static bool IsSolvable(this char c)
            => c.IsOperator() || c.IsDigit() || c.IsLetter();

        public static bool IsEmojiSymbol(this char c)
            => c.Equals('#') || c.Equals('*') || c.Equals('!') || c.Equals('?');

        public static bool IsOperator(this char c)
            => c.EqualsAny(UnicodeIndex.Operators);

        public static bool IsDigit(this char c)
            => char.IsDigit(c);

        public static bool IsLetter(this char c)
            => char.IsLetter(c);

        public static string Escape(this char c)
            => $"\\{c}";
    }
}