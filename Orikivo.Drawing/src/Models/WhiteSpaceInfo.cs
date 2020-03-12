using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Orikivo.Drawing
{
    /// <summary>
    /// Represents the width of a collection of white-space characters.
    /// </summary>
    public class WhiteSpaceInfo
    {
        public static List<WhiteSpaceInfo> Default
            => new List<WhiteSpaceInfo> { new WhiteSpaceInfo(1, '​'), new WhiteSpaceInfo(4, ' ', '⠀') };

        public static bool IsWhiteSpace(char c)
            => _emptyChars.Contains(c);

        // These are all of the valid empty characters.
        private static readonly char[] _emptyChars =
        {
            '​', /* Zero-Width Space */
            ' ', /* Space */
            '⠀' /* Braille Empty */
        };

        [JsonConstructor]
        public WhiteSpaceInfo(char[] chars, int width)
        {
            Chars = chars;
            Width = width;
        }

        public WhiteSpaceInfo(int width, params char[] chars)
        {
            if (!(chars.Length > 0))
                throw new Exception("At least one white-space character has to be specified.");

            if (chars.Any(x => !IsWhiteSpace(x)))
                throw new Exception($"One of the characters specified is not marked as white-space.");

            Chars = chars;
            Width = width;
        }

        /// <summary>
        /// A collection of <see cref="char"/> values that are validated as white-space.
        /// </summary>
        [JsonProperty("chars")]
        public char[] Chars { get; }

        /// <summary>
        /// The width (in pixels) of every white-space value in <see cref="Chars"/>.
        /// </summary>
        [JsonProperty("width")]
        public int Width { get; }
    }
}
