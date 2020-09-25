using System.Collections.Generic;

namespace Arcadia
{
    public class Icon
    {
        // The base custom icon to use
        public string Value { get; set; }
        // The icon the use in case the custom emojis are not allowed
        public string Fallback { get; set; }

        public List<string> Aliases { get; set; } = new List<string>();

        public static implicit operator Icon(string value)
            => new Icon
            {
                Fallback = value
            };

        public string ToString(bool useCustomIcons)
            => useCustomIcons ? Value ?? Fallback : Fallback;

        public override string ToString()
            => Value ?? Fallback ?? "EMPTY_ICON";

        public bool Equals(string value)
        {
            if (!string.IsNullOrWhiteSpace(Value))
            {
                if (Value == value)
                    return true;
            }

            if (!string.IsNullOrWhiteSpace(Fallback) && Fallback == value)
                return true;

            if (Aliases.Contains(value))
                return true;

            return false;
        }
    }
}