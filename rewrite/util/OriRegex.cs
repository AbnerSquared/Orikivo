using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Orikivo
{
    public static class OriRegex
    {
        // OriHelperService

        // the context parsing pattern needs to be reworked; ^c => (  ^m => {  ^g => [ if this is used for the root context, the closing brackets can be ignored i.e.
        // ] } )
        public static readonly string ContextParsePattern = @"(?:^(?:(?=\d+$)(\d+)$)?$)|(?:^(all|~)(?: )?(?:(?=\d+$)(\d+)$)?$)|(?:^((?:\w+(?:\^m)?\.)*)((?:\w+(?:\^g)? )*)(\w+)(?:(?=\^)\^(c|(?:m|g)(?: (\d+))?$))?(?:(?=\+)\+(\d{1,3}))?( \d+$|~(?:(?: \d+$)|$)|\*(?:(?: \d+$)|$))?(?:(?=\()\((\w+)\)?$)?$)";
        public static readonly string ModuleParsePattern = @"(?:(\w+)(?:\^m)?\.)";
        public static readonly string GroupParsePattern = @"(?:(\w+)(?:\^g)? )";

        // OriStat
        public static readonly string StatParsePattern = @"^(\w+):(\w+)$";

        // Discord
        public static readonly string EmoteParsePattern = @"<a?:\w{2,32}:\d{1,20}>";

        // Lobby Triggers:
        public static readonly string TriggerKeyPattern = @"^(\w+)(?:$| )";
        public static readonly string TriggerParsePatternFormat = @"^{0}((?:(?: \w+)*)?)(?: +)?$";
        public static readonly string TriggerArgParsePattern = @"(?: (\w+))"; // make object parse patterns
        // and append the corresponding object parse values into the trigger parse format.

        // TODO: Create custom parser for Double and Int32.
        public static bool ContainsEmote(string content)
        {
            return new Regex(EmoteParsePattern).Match(content).Success;
        }

        public static bool CaptureEmotes(string content, out List<Emote> capturedEmotes)
        {
            capturedEmotes = new List<Emote>();
            MatchCollection matches = new Regex(EmoteParsePattern).Matches(content);
            foreach (Match match in matches)
                if (match.Success)
                    capturedEmotes.Add(Emote.Parse(match.Value));

            if (capturedEmotes.Count == 0)
                return false;

            return true;
        }

        public static Match ParseContext(string context)
            => new Regex(ContextParsePattern).Match(context);

        public static List<string> GetContextModules(string moduleContext)
            => new Regex(ModuleParsePattern).Matches(moduleContext).Where(x => x.Success).Select(x => x.Groups[1].Value).ToList();

        public static List<string> GetContextGroups(string groupContext)
            => new Regex(GroupParsePattern).Matches(groupContext).Where(x => x.Success).Select(x => x.Groups[1].Value).ToList();

        public static string GetTriggerKey(string context)
            => new Regex(TriggerKeyPattern).Match(context).Groups[0].Value.Trim();

        public static List<string> GetTriggerArgs(string trigger, string context)
        {
            Match match = new Regex(string.Format(TriggerParsePatternFormat, trigger)).Match(context);
            List<string> args = new List<string>();
            Console.WriteLine(match.Groups[0].Value);
            MatchCollection matches = new Regex(TriggerArgParsePattern).Matches(match.Groups[0].Value);

            matches.ToList().ForEach(x => { Console.WriteLine(x.Value.Trim()); args.Add(x.Value.Trim()); });
            return match.Success ? args : null;
        }


    }
}
