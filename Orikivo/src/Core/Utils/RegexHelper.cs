using Discord;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Orikivo
{
    /// <summary>
    /// A utility class that provides common <see cref="Regex"/> methods.
    /// </summary>
    public static class RegexHelper
    {
        /// <summary>
        /// Gets the <see cref="Regex"/> pattern that is used to parse an <see cref="Emote"/>.
        /// </summary>
        public static readonly string EmoteParsePattern = @"<a?:\w{2,32}:\d{1,20}>";

        /// <summary>
        /// Determines if the specified <see cref="string"/> contains a parseable emote.
        /// </summary>
        public static bool ContainsEmote(string content)
            => new Regex(EmoteParsePattern).Match(content).Success;

        /// <summary>
        /// Attempts to capture emotes from a specified <see cref="string"/>.
        /// </summary>
        public static bool TryCaptureEmotes(string content, out List<Emote> emotes)
        {
            emotes = new List<Emote>();

            emotes.AddRange(new Regex(EmoteParsePattern)
                .Matches(content)
                .Where(x => x.Success)
                .Select(x => Emote.Parse(x.Value)));

            return emotes.Count != 0;
        }

        /// <summary>
        /// Returns all captured emotes from a specified <see cref="string"/>.
        /// </summary>
        public static List<Emote> CaptureEmotes(string content)
        {
            var emotes = new List<Emote>();

            emotes.AddRange(new Regex(EmoteParsePattern)
                .Matches(content)
                .Where(x => x.Success)
                .Select(x => Emote.Parse(x.Value)));

            return emotes;
        }
    }
}
