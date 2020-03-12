using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Orikivo
{
    /// <summary>
    /// A utility class that provides common <see cref="Regex"/> methods.
    /// </summary>
    public static class OriRegex
    {
        /// <summary>
        /// Gets the <see cref="Regex"/> pattern that is used to parse stats (unused).
        /// </summary>
        public static readonly string StatParsePattern = @"^(\w+):(\w+)$";

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
        /// <param name="emotes">A collection of captured emotes.</param>
        public static bool TryCaptureEmotes(string content, out List<Emote> emotes)
        {
            emotes = new List<Emote>();
            MatchCollection matches = new Regex(EmoteParsePattern).Matches(content);

            foreach (Match match in matches)
                if (match.Success)
                    emotes.Add(Emote.Parse(match.Value));

            if (emotes.Count == 0)
                return false;

            return true;
        }

        /// <summary>
        /// Returns all captured emotes from a specified <see cref="string"/>.
        /// </summary>
        public static List<Emote> CaptureEmotes(string content)
        {
            List<Emote> emotes = new List<Emote>();

            foreach (Match match in new Regex(EmoteParsePattern).Matches(content))
                if (match.Success)
                    emotes.Add(Emote.Parse(match.Value));

            return emotes;
        }
    }
}
