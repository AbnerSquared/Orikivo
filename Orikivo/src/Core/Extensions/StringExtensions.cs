#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Orikivo
{
    public static class StringExtensions
    {
        /// <summary>
        /// Sets the specified casing rule for this current instance.
        /// </summary>
        public static string ToString(this string str, Casing casing)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            return casing switch
            {
                Casing.Upper => str.ToUpper(),
                Casing.Lower => str.ToLower(),
                Casing.Pascal => str.Length >= 2 ? $"{char.ToUpper(str[0])}{str[1..].ToLower()}" : str.Length == 1 ? str.ToUpper() : str,
                _ => str
            };
        }

        /// <summary>
        /// Determines whether the beginning of this string instance matches any of the specified strings.
        /// </summary>
        public static bool StartsWithAny(this string str, params string[] anyOf)
            => anyOf.Any(str.StartsWith);

        /// <summary>
        /// Determines whether the beginning of this string instance matches any of the specified strings.
        /// </summary>
        public static bool StartsWithAny(this string str, out string? match, params string[] anyOf)
        {
             match = anyOf.FirstOrDefault(str.StartsWith);
             return StartsWith(str, anyOf);
        }

        /// <summary>
        /// Determines whether the beginning of this string instance matches any of the specified strings when compared using the specified comparison option.
        /// </summary>
        public static bool StartsWithAny(this string str, StringComparison comparisonType, params string[] anyOf)
            => anyOf.Any(x => str.StartsWith(x, comparisonType));

        /// <summary>
        /// Determines whether the beginning of this string instance matches any of the specified strings when compared using the specified comparison option.
        /// </summary>
        public static bool StartsWithAny(this string str, StringComparison comparisonType, out string? match, params string[] anyOf)
        {
             match = anyOf.FirstOrDefault(x => str.StartsWith(x, comparisonType));
             return StartsWith(str, anyOf, comparisonType);
        }

        /// <summary>
        /// Determines whether the beginning of this string instance matches any of the specified strings when compared using the specified culture.
        /// </summary>
        public static bool StartsWithAny(this string str, bool ignoreCase, CultureInfo? culture, params string[] anyOf)
            => anyOf.Any(x => str.StartsWith(x, ignoreCase, culture));

        /// <summary>
        /// Determines whether the beginning of this string instance matches any of the specified strings when compared using the specified culture.
        /// </summary>
        public static bool StartsWithAny(this string str, bool ignoreCase, CultureInfo? culture, out string? match, params string[] anyOf)
        {
             match = anyOf.FirstOrDefault(x => str.StartsWith(x, ignoreCase, culture));
             return StartsWith(str, anyOf, ignoreCase, culture);
        }

        /// <summary>
        /// Determines whether this string instance starts with any of the specified characters.
        /// </summary>
        public static bool StartsWithAny(this string str, params char[] anyOf)
            => anyOf.Any(str.StartsWith);

        /// <summary>
        /// Determines whether this string instance starts with any of the specified characters.
        /// </summary>
        public static bool StartsWithAny(this string str, out char? match, params char[] anyOf)
        {
             match = anyOf.FirstOrDefault(str.StartsWith);
             return StartsWith(str, anyOf);
        }

        /// <summary>
        /// Determines whether the beginning of this string instance matches any of the specified strings.
        /// </summary>
        public static bool StartsWith(this string str, in IEnumerable<string> anyOf)
            => anyOf.Any(str.StartsWith);

        /// <summary>
        /// Determines whether the beginning of this string instance matches any of the specified strings.
        /// </summary>
        public static bool StartsWith(this string str, in IEnumerable<string> anyOf, out string? match)
        {
             match = anyOf.FirstOrDefault(str.StartsWith);
             return StartsWith(str, anyOf);
        }

        /// <summary>
        /// Determines whether the beginning of this string instance matches any of the specified strings when compared using the specified comparison option.
        /// </summary>
        public static bool StartsWith(this string str, in IEnumerable<string> anyOf, StringComparison comparisonType)
            => anyOf.Any(x => str.StartsWith(x, comparisonType));

        /// <summary>
        /// Determines whether the beginning of this string instance matches any of the specified strings when compared using the specified comparison option.
        /// </summary>
        public static bool StartsWith(this string str, in IEnumerable<string> anyOf, StringComparison comparisonType, out string? match)
        {
             match = anyOf.FirstOrDefault(x => str.StartsWith(x, comparisonType));
             return StartsWith(str, anyOf, comparisonType);
        }

        /// <summary>
        /// Determines whether the beginning of this string instance matches any of the specified strings when compared using the specified culture.
        /// </summary>
        public static bool StartsWith(this string str, in IEnumerable<string> anyOf, bool ignoreCase, CultureInfo? culture)
            => anyOf.Any(x => str.StartsWith(x, ignoreCase, culture));

        /// <summary>
        /// Determines whether the beginning of this string instance matches any of the specified strings when compared using the specified culture.
        /// </summary>
        public static bool StartsWith(this string str, in IEnumerable<string> anyOf, bool ignoreCase, CultureInfo? culture, out string? match)
        {
             match = anyOf.FirstOrDefault(x => str.StartsWith(x, ignoreCase, culture));
             return StartsWith(str, anyOf, ignoreCase, culture);
        }

        /// <summary>
        /// Determines whether this string instance starts with any of the specified characters.
        /// </summary>
        public static bool StartsWith(this string str, in IEnumerable<char> anyOf)
            => anyOf.Any(str.StartsWith);

        /// <summary>
        /// Determines whether this string instance starts with any of the specified characters.
        /// </summary>
        public static bool StartsWith(this string str, in IEnumerable<char> anyOf, out char match)
        {
             match = anyOf.FirstOrDefault(str.StartsWith);
             return StartsWith(str, anyOf);
        }

        /// <summary>
        /// Determines whether the end of this string instance matches any of the specified strings.
        /// </summary>
        public static bool EndsWithAny(this string str, params string[] anyOf)
        => anyOf.Any(str.EndsWith);

        /// <summary>
        /// Determines whether the end of this string instance matches any of the specified strings.
        /// </summary>
        public static bool EndsWithAny(this string str, out string? match, params string[] anyOf)
        {
            match = anyOf.FirstOrDefault(str.EndsWith);
            return EndsWith(str, anyOf);
        }

        /// <summary>
        /// Determines whether the end of this string instance matches any of the specified strings when compared using the specified comparison option.
        /// </summary>
        public static bool EndsWithAny(this string str, StringComparison comparisonType, params string[] anyOf)
            => anyOf.Any(x => str.EndsWith(x, comparisonType));

        /// <summary>
        /// Determines whether the end of this string instance matches any of the specified strings when compared using the specified comparison option.
        /// </summary>
        public static bool EndsWithAny(this string str, StringComparison comparisonType, out string? match, params string[] anyOf)
        {
            match = anyOf.FirstOrDefault(x => str.EndsWith(x, comparisonType));
            return EndsWith(str, anyOf, comparisonType);
        }

        /// <summary>
        /// Determines whether the end of this string instance matches any of the specified strings when compared using the specified culture.
        /// </summary>
        public static bool EndsWithAny(this string str, bool ignoreCase, CultureInfo? culture, params string[] anyOf)
            => anyOf.Any(x => str.EndsWith(x, ignoreCase, culture));

        /// <summary>
        /// Determines whether the end of this string instance matches any of the specified strings when compared using the specified culture.
        /// </summary>
        public static bool EndsWithAny(this string str, bool ignoreCase, CultureInfo? culture, out string? match, params string[] anyOf)
        {
            match = anyOf.FirstOrDefault(x => str.EndsWith(x, ignoreCase, culture));
            return StartsWith(str, anyOf, ignoreCase, culture);
        }

        /// <summary>
        /// Determines whether this string instance ends with any of the specified characters.
        /// </summary>
        public static bool EndsWithAny(this string str, params char[] anyOf)
            => anyOf.Any(str.EndsWith);

        /// <summary>
        /// Determines whether this string instance ends with any of the specified characters.
        /// </summary>
        public static bool EndsWithAny(this string str, out char? match, params char[] anyOf)
        {
            match = anyOf.FirstOrDefault(str.EndsWith);
            return EndsWith(str, anyOf);
        }

        /// <summary>
        /// Determines whether the end of this string instance matches any of the specified strings.
        /// </summary>
        public static bool EndsWith(this string str, in IEnumerable<string> anyOf)
            => anyOf.Any(str.EndsWith);

        /// <summary>
        /// Determines whether the end of this string instance matches any of the specified strings.
        /// </summary>
        public static bool EndsWith(this string str, in IEnumerable<string> anyOf, out string? match)
        {
            match = anyOf.FirstOrDefault(str.EndsWith);
            return EndsWith(str, anyOf);
        }

        /// <summary>
        /// Determines whether the end of this string instance matches any of the specified strings when compared using the specified comparison option.
        /// </summary>
        public static bool EndsWith(this string str, in IEnumerable<string> anyOf, StringComparison comparisonType)
            => anyOf.Any(x => str.EndsWith(x, comparisonType));

        /// <summary>
        /// Determines whether the end of this string instance matches any of the specified strings when compared using the specified comparison option.
        /// </summary>
        public static bool EndsWith(this string str, in IEnumerable<string> anyOf, StringComparison comparisonType, out string? match)
        {
            match = anyOf.FirstOrDefault(x => str.EndsWith(x, comparisonType));
            return EndsWith(str, anyOf, comparisonType);
        }

        /// <summary>
        /// Determines whether the end of this string instance matches any of the specified strings when compared using the specified culture.
        /// </summary>
        public static bool EndsWith(this string str, in IEnumerable<string> anyOf, bool ignoreCase, CultureInfo? culture)
            => anyOf.Any(x => str.EndsWith(x, ignoreCase, culture));

        /// <summary>
        /// Determines whether the end of this string instance matches any of the specified strings when compared using the specified culture.
        /// </summary>
        public static bool EndsWith(this string str, in IEnumerable<string> anyOf, bool ignoreCase, CultureInfo? culture, out string? match)
        {
            match = anyOf.FirstOrDefault(x => str.EndsWith(x, ignoreCase, culture));
            return EndsWith(str, anyOf, ignoreCase, culture);
        }

        /// <summary>
        /// Determines whether this string instance ends with any of the specified characters.
        /// </summary>
        public static bool EndsWith(this string str, in IEnumerable<char> anyOf)
            => anyOf.Any(str.EndsWith);

        /// <summary>
        /// Determines whether this string instance ends with any of the specified characters.
        /// </summary>
        public static bool EndsWith(this string str, in IEnumerable<char> anyOf, out char match)
        {
            match = anyOf.FirstOrDefault(str.EndsWith);
            return EndsWith(str, anyOf);
        }

        /// <summary>
        /// Returns a value indicating whether any of the specified substrings occurs within this string.
        /// </summary>
        public static bool ContainsAny(this string str, params string[] anyOf)
            => anyOf.Any(str.Contains);

        /// <summary>
        /// Returns a value indicating whether any of the specified substrings occurs within this string.
        /// </summary>
        public static bool ContainsAny(this string str, out string? match, params string[] anyOf)
        {
             match = anyOf.FirstOrDefault(str.Contains);
             return Contains(str, anyOf);
        }

        /// <summary>
        /// Returns a value indicating whether any of the specified substrings occurs within this string, using the specified comparison rules.
        /// </summary>
        public static bool ContainsAny(this string str, StringComparison comparisonType, params string[] anyOf)
            => anyOf.Any(x => str.Contains(x, comparisonType));

        /// <summary>
        /// Returns a value indicating whether any of the specified substrings occurs within this string, using the specified comparison rules.
        /// </summary>
        public static bool ContainsAny(this string str, StringComparison comparisonType, out string? match, params string[] anyOf)
        {
             match = anyOf.FirstOrDefault(x => str.Contains(x, comparisonType));
             return Contains(str, anyOf, comparisonType);
        }

        /// <summary>
        /// Returns a value indicating whether any of the specified characters occurs within this string.
        /// </summary>
        public static bool ContainsAny(this string str, params char[] anyOf)
            => anyOf.Any(str.Contains);

        /// <summary>
        /// Returns a value indicating whether any of the specified characters occurs within this string.
        /// </summary>
        public static bool ContainsAny(this string str, out char? match, params char[] anyOf)
        {
             match = anyOf.FirstOrDefault(str.Contains);
             return Contains(str, anyOf);
        }

        /// <summary>
        /// Returns a value indicating whether any of the specified substrings occurs within this string.
        /// </summary>
        public static bool Contains(this string str, in IEnumerable<string> anyOf)
            => anyOf.Any(str.Contains);

        /// <summary>
        /// Returns a value indicating whether any of the specified substrings occurs within this string.
        /// </summary>
        public static bool Contains(this string str, in IEnumerable<string> anyOf, out string? match)
        {
             match = anyOf.FirstOrDefault(str.Contains);
             return Contains(str, anyOf);
        }

        /// <summary>
        /// Returns a value indicating whether any of the specified substrings occurs within this string, using the specified comparison rules.
        /// </summary>
        public static bool Contains(this string str, in IEnumerable<string> anyOf, StringComparison comparisonType)
            => anyOf.Any(x => str.Contains(x, comparisonType));

        /// <summary>
        /// Returns a value indicating whether any of the specified substrings occurs within this string, using the specified comparison rules.
        /// </summary>
        public static bool Contains(this string str, in IEnumerable<string> anyOf, StringComparison comparisonType, out string? match)
        {
             match = anyOf.FirstOrDefault(x => str.Contains(x, comparisonType));
             return Contains(str, anyOf, comparisonType);
        }

        /// <summary>
        /// Returns a value indicating whether any of the specified characters occurs within this string, using the specified comparison rules.
        /// </summary>
        public static bool Contains(this string str, in IEnumerable<char> anyOf, StringComparison comparisonType)
            => anyOf.Any(x => str.Contains(x, comparisonType));

        /// <summary>
        /// Returns a value indicating whether any of the specified characters occurs within this string, using the specified comparison rules.
        /// </summary>
        public static bool Contains(this string str, in IEnumerable<char> anyOf, StringComparison comparisonType, out char? match)
        {
            match = anyOf.FirstOrDefault(x => str.Contains(x, comparisonType));
            return Contains(str, anyOf, comparisonType);
        }

        /// <summary>
        /// Returns a value indicating whether any of the specified characters occurs within this string.
        /// </summary>
        public static bool Contains(this string str, IEnumerable<char> anyOf)
            => anyOf.Any(str.Contains);

        /// <summary>
        /// Returns a value indicating whether any of the specified characters occurs within this string.
        /// </summary>
        public static bool Contains(this string str, in IEnumerable<char> anyOf, out char? match)
        {
             match = anyOf.FirstOrDefault(str.Contains);
             return Contains(str, anyOf);
        }

        /// <summary>
        /// Determines whether any of the specified string objects have the same value as this instance.
        /// </summary>
        public static bool EqualsAny(this string str, params string[] anyOf)
            => anyOf.Any(str.Equals);

        /// <summary>
        /// Determines whether any of the specified string objects have the same value as this instance.
        /// </summary>
        public static bool EqualsAny(this string str, out string? match, params string[] anyOf)
        {
             match = anyOf.FirstOrDefault(str.Equals);
             return Equals(str, anyOf);
        }

        /// <summary>
        /// Determines whether any of the specified string objects have the same value as this instance. A parameter specifies the culture, case, and sort rules used in the comparison.
        /// </summary>
        public static bool EqualsAny(this string str, StringComparison comparisonType, params string[] anyOf)
            => anyOf.Any(x => str.Equals(x, comparisonType));

        /// <summary>
        /// Determines whether any of the specified string objects have the same value as this instance. A parameter specifies the culture, case, and sort rules used in the comparison.
        /// </summary>
        public static bool EqualsAny(this string str, StringComparison comparisonType, out string? match, params string[] anyOf)
        {
             match = anyOf.FirstOrDefault(x => str.Equals(x, comparisonType));
             return Equals(str, anyOf, comparisonType);
        }

        /// <summary>
        /// Determines whether any of the specified characters have the same value as this instance.
        /// </summary>
        public static bool EqualsAny(this string str, params char[] anyOf)
            => anyOf.Any(x => str.Equals(x.ToString()));

        /// <summary>
        /// Determines whether any of the specified characters have the same value as this instance.
        /// </summary>
        public static bool EqualsAny(this string str, out char? match, params char[] anyOf)
        {
             match = anyOf.FirstOrDefault(x => str.Equals(x.ToString()));
             return Equals(str, anyOf);
        }

        /// <summary>
        /// Determines whether any of the specified characters have the same value as this instance. A parameter specifies the culture, case, and sort rules used in the comparison.
        /// </summary>
        public static bool EqualsAny(this string str, StringComparison comparisonType, params char[] anyOf)
            => anyOf.Any(x => str.Equals(x.ToString(), comparisonType));

        /// <summary>
        /// Determines whether any of the specified characters have the same value as this instance. A parameter specifies the culture, case, and sort rules used in the comparison.
        /// </summary>
        public static bool EqualsAny(this string str, StringComparison comparisonType, out char? match, params char[] anyOf)
        {
            match = anyOf.FirstOrDefault(x => str.Equals(x.ToString(), comparisonType));
            return Equals(str, anyOf, comparisonType);
        }

        /// <summary>
        /// Determines whether any of the specified string objects have the same value as this instance.
        /// </summary>
        public static bool Equals(this string str, in IEnumerable<string> anyOf)
            => anyOf.Any(str.Equals);

        /// <summary>
        /// Determines whether any of the specified string objects have the same value as this instance.
        /// </summary>
        public static bool Equals(this string str, in IEnumerable<string> anyOf, out string? match)
        {
             match = anyOf.FirstOrDefault(str.Equals);
             return Equals(str, anyOf);
        }

        /// <summary>
        /// Determines whether any of the specified string objects have the same value as this instance. A parameter specifies the culture, case, and sort rules used in the comparison.
        /// </summary>
        public static bool Equals(this string str, in IEnumerable<string> anyOf, StringComparison comparisonType)
            => anyOf.Any(x => str.Equals(x, comparisonType));

        /// <summary>
        /// Determines whether any of the specified string objects have the same value as this instance. A parameter specifies the culture, case, and sort rules used in the comparison.
        /// </summary>
        public static bool Equals(this string str, in IEnumerable<string> anyOf, StringComparison comparisonType, out string? match)
        {
             match = anyOf.FirstOrDefault(x => str.Equals(x, comparisonType));
             return Equals(str, anyOf, comparisonType);
        }

        /// <summary>
        /// Determines whether any of the specified characters have the same value as this instance.
        /// </summary>
        public static bool Equals(this string str, in IEnumerable<char> anyOf)
            => anyOf.Any(x => str.Equals(x.ToString()));

        /// <summary>
        /// Determines whether any of the specified characters have the same value as this instance.
        /// </summary>
        public static bool Equals(this string str, in IEnumerable<char> anyOf, out char? match)
        {
             match = anyOf.FirstOrDefault(x => str.Equals(x.ToString()));
             return Equals(str, anyOf);
        }

        /// <summary>
        /// Determines whether any of the specified characters have the same value as this instance. A parameter specifies the culture, case, and sort rules used in the comparison.
        /// </summary>
        public static bool Equals(this string str, in IEnumerable<char> anyOf, StringComparison comparisonType)
            => anyOf.Any(x => str.Equals(x.ToString(), comparisonType));

        /// <summary>
        /// Determines whether any of the specified characters have the same value as this instance. A parameter specifies the culture, case, and sort rules used in the comparison.
        /// </summary>
        public static bool Equals(this string str, in IEnumerable<char> anyOf, StringComparison comparisonType, out char? match)
        {
            match = anyOf.FirstOrDefault(x => str.Equals(x.ToString(), comparisonType));
            return Equals(str, anyOf, comparisonType);
        }

        /// <summary>
        /// Returns a new string in which this current instance is escaped.
        /// </summary>
        public static string Escape(this string s)
            => $"\\{s}";

        /// <summary>
        /// Returns a new string in which all occurrences of the specified strings in this current instance are escaped.
        /// </summary>
        public static string Escape(this string s, params char[] escapeChars)
        {
            escapeChars.ForEach(c => s = s.Replace(c.ToString(), c.Escape()));
            return s;
        }

        /// <summary>
        /// Returns a new string in which all occurrences of the specified strings in this current instance are escaped.
        /// </summary>
        public static string Escape(this string s, params string[] args)
        {
            args.ForEach(x => s = s.Replace(x, x.Escape()));
            return s;
        }

        /// <summary>
        /// Returns a new string in which all occurrences of the specified strings in this current instance are removed.
        /// </summary>
        public static string Remove(this string s, params string[] args)
        {
            args.ForEach(x => s = s.Replace(x, string.Empty));
            return s;
        }

        /// <summary>
        /// Returns a new string in which all occurrences of the specified characters in this current instance are removed.
        /// </summary>
        public static string Remove(this string s, params char[] args)
        {
            args.ForEach(c => s = s.Replace(c.ToString(), string.Empty));
            return s;
        }
    }
}
