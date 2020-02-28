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

        // TODO: Create custom parser for Double and Int32.

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

        // TODO: Either scrap or modify custom command parsing.
        /*
        
        // Lobby Triggers:
        public static readonly string TriggerKeyPattern = @"^(\w+)(?:$| )";
        public static readonly string TriggerParsePatternFormat = @"^{0}((?:(?: \w+)*)?)(?: +)?$";
        public static readonly string TriggerArgParsePattern = @"(?: (\w+))"; // make object parse patterns
        // and append the corresponding object parse values into the trigger parse format. 
        
        public static string GetTriggerKey(string context)
            => new Regex(TriggerKeyPattern).Match(context).Groups[0].Value.Trim();

        public static List<string> GetTriggerArgs(string trigger, string context)
        {
            Match match = new Regex(string.Format(TriggerParsePatternFormat, trigger)).Match(context);
            List<string> args = new List<string>();
            Console.WriteLine(match.Groups[0].Value);
            MatchCollection matches = new Regex(TriggerArgParsePattern).Matches(match.Groups[0].Value);

            matches.ToList().ForEach(x =>
            {
                Console.WriteLine(x.Value.Trim());
                args.Add(x.Value.Trim());
            });

            return match.Success ? args : null;
        }
        */
    }
}
