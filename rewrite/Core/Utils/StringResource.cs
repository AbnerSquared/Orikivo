using System;
using System.Collections.Generic;

namespace Orikivo
{
    // TODO: Configure this to derive from Resources.
    public class StringResource
    {
        public static StringResource Get(string key)
        {

            throw new NotImplementedException();
        }

        /// <summary>
        /// The unique identifier for this string.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// The formatter used to modify the appearance of this string. Can be left empty.
        /// </summary>
        public string Formatter { get; }

        /// <summary>
        /// The collection of values for this string across all known languages.
        /// </summary>
        public Dictionary<StringLocale, string[]> Locale { get; }

        /// <summary>
        /// Returns the string correlated to the specified locale.
        /// </summary>
        public string ToString(StringLocale locale)
            => Locale[locale][RandomProvider.Instance.Next(Locale[locale].Length)] ?? throw new KeyNotFoundException();
    }
}
