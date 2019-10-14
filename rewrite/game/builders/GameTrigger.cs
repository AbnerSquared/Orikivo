using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Orikivo
{
    /// <summary>
    /// A command object that defines what to do upon being called in a message.
    /// </summary>
    public class GameTrigger
    {
        public GameTrigger(string name, GameArg arg = null, AttributeUpdatePacket onSuccess = null)
        {
            Name = name; // the name that is used to match with a trigger.
            Arg = arg; // the arg that is required whenever
            if (onSuccess != null)
                ToUpdate.Add(onSuccess); // to execute whenever the trigger is activated.
        }

        /// <summary>
        /// The name of the trigger that is required in a message in order to parse.
        /// </summary>
        public string Name { get; }

        public GameArg Arg { get; }

        /// <summary>
        /// A list of attributes to update upon a successful execution.
        /// </summary>
        public List<AttributeUpdatePacket> ToUpdate { get; } = new List<AttributeUpdatePacket>();

        /// <summary>
        /// A list of criteria that need to be met before the command can be executed.
        /// </summary>
        public List<AttributeCriterion> Criteria { get; } = new List<AttributeCriterion>();

        /// <summary>
        /// Returns a value defining if the trigger requires an argument.
        /// </summary>
        public bool RequiresArg => !(Arg == null);

        /// <summary>
        /// Attempts to parse the message sent as a validated trigger.
        /// </summary>
        /// <param name="message">The message that was sent to the channel.</param>
        /// <param name="users"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryParse(string message, List<User> users, out TriggerResult result)
        {
            Console.WriteLine($"-- (trigger:{Name}) Now attempting parse... --");
            result = new TriggerResult();
            string keyParser = string.Format(@"^{0}", Name);
            string valueParser = @"((?:(?: \w+)*)?)(?: +)?";
            // if there is an arg specified, attach the value parser; otherwise, just get the key
            string parser = Arg == null ? keyParser + "$" : keyParser + valueParser + "$";
            Regex r = new Regex(parser);
            Match m = r.Match(message);
            if (m.Success)
            {
                string argField = null;
                Console.WriteLine($"-- (trigger:{Name}) Parse success! --");
                result.TriggerName = Name;
                if (Arg != null)
                {
                    string obj = m.Groups[0].Value; // the parsed object value.
                    ulong.TryParse(obj, out ulong userId);
                    if (Arg.Type is GameArgType.Message)
                        result.Result = obj;
                    if (Arg.Type is GameArgType.User)
                    {
                        if (!users.Any(x => x.Name == obj || (x.Id == userId)))
                        {
                            result = null;
                            return false;
                        }
                        result.Result = users.First(x => x.Name == obj || (x.Id == userId));
                    }
                    if (Arg.Type == GameArgType.Custom)
                    {
                        if (Arg.Fields.Any(x => x.Name == obj))
                        {
                            result.Result = Arg.Fields.First(x => x.Name == obj);
                            argField = result.Result.ToString();
                        }
                    }
                    Console.WriteLine($"-- (trigger:{Name}) Arg parse success! --");
                }

                result.Packets = GetUpdatePackets(argField ?? Arg?.Name);
                return true;
            }

            result = null;
            return false;
        }

        // Gets all of the attributes deriving from the argument successfully passed.
        private List<AttributeUpdatePacket> GetUpdatePackets(string type = null)
        {
            if (RequiresArg)
            {
                if (Arg.Fields.Any(x => x.Name == type))
                {
                    GameArgField field = Arg.Fields.First(x => x.Name == type);
                    return (field.IncludeParentUpdates ? Arg.ToUpdate.Concat(field.ToUpdate).Concat(ToUpdate) : field.ToUpdate.Concat(ToUpdate)).ToList();
                }

                if (Arg.Name == type)
                    return Arg.ToUpdate.Concat(ToUpdate).ToList();
            }

            return ToUpdate;
        }
    }
}
