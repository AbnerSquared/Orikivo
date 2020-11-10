using System.Collections.Generic;
using System.Linq;

namespace Arcadia
{
    /// <summary>
    /// Represents emoticon-based text.
    /// </summary>
    public class Icon
    {
        private readonly string DefaultValue = "EMPTY_ICON";

        internal Icon() { }

        public Icon(string fallback)
        {
            Fallback = fallback;
            Aliases = new List<string>(0);
        }

        /// <summary>
        /// Represents the default Unicode string.
        /// </summary>
        public string Value { get; internal set; }

        /// <summary>
        /// Represents the fallback Unicode string if custom icons are disabled.
        /// </summary>
        public string Fallback { get; internal set; }

        public IReadOnlyList<string> Aliases { get; internal set; }

        public override string ToString()
            => Value ?? Fallback ?? DefaultValue;

        public string ToString(bool useCustomIcons)
            => useCustomIcons ? Value ?? Fallback : Fallback;

        public bool Equals(string value)
        {
            if (!string.IsNullOrWhiteSpace(Value))
            {
                if (Value == value)
                    return true;
            }

            if (!string.IsNullOrWhiteSpace(Fallback) && Fallback == value)
                return true;

            return Aliases.Contains(value);
        }

        public static implicit operator Icon(string value)
            => new Icon
            {
                Fallback = value
            };

        public static implicit operator string(Icon icon)
            => icon?.ToString();
    }
}
