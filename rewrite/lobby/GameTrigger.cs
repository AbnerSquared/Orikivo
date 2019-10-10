using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Orikivo
{
    // needs a major rework
    public class GameTrigger
    {
        public GameTrigger(string name, GameTriggerArg arg = null, GameAttributeUpdate onSuccess = null)
        {
            Name = name; // the name that is used to match with a trigger.
            Arg = arg; // the arg that is required whenever 
            OnSuccess = onSuccess; // to execute whenever the trigger is activated.
        }

        public string Name { get; }
        public GameTriggerArg Arg { get; }
        public GameAttributeUpdate OnSuccess { get; }
        public bool RequiresArg => !(Arg == null);
        public bool CanParse(string context, List<User> users)
            => TryParse(context, users, out TriggerContext parse);
        // pass the list of users in here to ensure parse
        public bool TryParse(string context, List<User> users, out TriggerContext parse)
        {
            Console.WriteLine($"-- (trigger:{Name}) Now attempting parse... --");
            parse = new TriggerContext();
            string keyParser = string.Format(@"^{0}", Name);
            string valueParser = @"((?:(?: \w+)*)?)(?: +)?";
            // if there is an arg specified, attach the value parser; otherwise, just get the key
            string parser = Arg == null ? keyParser + "$" : keyParser + valueParser + "$";
            Regex r = new Regex(parser);
            Match m = r.Match(context);
            if (m.Success)
            {
                Console.WriteLine($"-- (trigger:{Name}) Parse success! --");
                parse.TriggerName = Name;
                if (Arg != null)
                {
                    string obj = m.Groups[0].Value; // the parsed object value.
                    ulong.TryParse(obj, out ulong userId);
                    if (Arg.Type is GameArgType.Message)
                        parse.Value = obj;
                    if (Arg.Type is GameArgType.User)
                    {
                        if (!users.Any(x => x.Name == obj || (x.Id == userId)))
                        {
                            parse = null;
                            return false;
                        }
                        parse.Value = users.First(x => x.Name == obj || (x.Id == userId));
                    }
                    Console.WriteLine($"-- (trigger:{Name}) Arg parse success! --");
                }

                parse.AttributeUpdate = OnSuccess;
                return true;
            }

            parse = null;
            return false;
        }
    }

    public class TriggerContext
    {
        internal TriggerContext() { }
        public string TriggerName { get; internal set; }
        public object Value { get; internal set; } // can be null.

        public GameAttributeUpdate AttributeUpdate { get; internal set; }

    }
}
