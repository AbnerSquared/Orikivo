using System;
using System.Collections.Generic;

namespace Orikivo
{
    // TODO: Configure this to derive from Resources.
    /// <summary>
    /// Represents a <see cref="string"/> that can be stored in various languages.
    /// </summary>
    public class TextResource
    {
        /// <summary>
        /// Imports and returns a new <see cref="TextResource"/> (unimplemented).
        /// </summary>
        /// <param name="key">The unique identifier that points to the <see cref="TextResource"/>.</param>
        public static TextResource Import(string key)
            => throw new NotImplementedException();

        /// <summary>
        /// The unique identifier for the <see cref="TextResource"/>.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Gets or sets a formatting string used to modify the appearance of this string (unimplemented).
        /// </summary>
        public string Formatting { get; }

        /// <summary>
        /// The collection of values for this string across all known languages.
        /// </summary>
        public Dictionary<TextLocale, string[]> Locale { get; }

        /// <summary>
        /// Returns a <see cref="string"/> for the specified locale.
        /// </summary>
        public string ToString(TextLocale locale)
            => Locale[locale][RandomProvider.Instance.Next(Locale[locale].Length)] ?? throw new KeyNotFoundException();
    }
}
