﻿using System;
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
        public GameTrigger(string name, GameArg arg = null, List<GameCriterion> criteria = null, List<GameUpdatePacket> onParseSuccess = null)
        {
            Name = name; // the name that is used to match with a trigger.
            Arg = arg; // the arg that is required whenever
            Criteria = criteria ?? new List<GameCriterion>();
            OnParseSuccess = onParseSuccess ?? new List<GameUpdatePacket>();
        }

        /// <summary>
        /// The name of the trigger that is required in a message in order to parse.
        /// </summary>
        public string Name { get; }

        public string Id => $"trigger.{Name}";

        // an optional argument for the trigger to require.
        public GameArg Arg { get; } = null;

        /// <summary>
        /// A list of attributes to update upon a successful execution.
        /// </summary>
        public List<GameUpdatePacket> OnParseSuccess { get; } = new List<GameUpdatePacket>();

        /// <summary>
        /// A list of criteria that need to be met before the command can be executed.
        /// </summary>
        public List<GameCriterion> Criteria { get; } = new List<GameCriterion>();

        /// <summary>
        /// Returns a value defining if the trigger requires an argument.
        /// </summary>
        public bool ContainsArg => !(Arg == null);

        /// <summary>
        /// Attempts to parse the message sent as a validated trigger.
        /// </summary>
        public bool TryParse(GameTriggerContext context, out GameTriggerResult result)
        {
            Console.WriteLine($"-- (trigger:{Name}) Now attempting parse... --");
            result = new GameTriggerResult();
            string keyParser = string.Format(@"^{0}", Name);
            string valueParser = @"((?:(?: \w+)*)?)(?: +)?";
            // if there is an arg specified, attach the value parser; otherwise, just get the key
            string parser = ContainsArg ? keyParser + valueParser + "$" : keyParser + "$";
            Regex r = new Regex(parser);
            Match m = r.Match(context.Message);
            if (m.Success)
            {
                Console.WriteLine($"-- {Id} Successfully parsed a trigger. --");
                result.TriggerId = Id;
                if (ContainsArg)
                {
                    result.ArgId = Arg.Id;
                    string argMessage = m.Groups[0].Value;

                    if (Arg.IsOptional && !Checks.NotNull(argMessage))
                    {
                        result.Objects.Add(Arg.DefaultValue);
                        // You don't add the Arg.OnParseSuccess packets on a default value.
                        result.Packets.AddRange(OnParseSuccess);
                        return true;
                    }
                    
                    if (Arg.IsArray)
                    {
                        if (TryParseArgs(context.Data, argMessage, out List<GameObject> objects))
                        {
                            result.Objects.AddRange(objects);
                            result.Packets.AddRange(OnParseSuccess);
                            if (Arg.Type is GameObjectType.Custom)
                                foreach (List<GameUpdatePacket> packetGroup in Arg.Values.Where(x => objects.Select(y => y.Value).Contains(x.Value)).Select(x => x.OnParseSuccess).ToList())
                                    result.Packets.AddRange(packetGroup);
                            result.Packets.AddRange(Arg.OnParseSuccess);
                            return true;
                        }
                    }
                    else
                    {
                        if (TryParseArg(context.Data, argMessage, out GameObject obj))
                        {
                            result.Objects.Add(obj);
                            result.Packets.AddRange(OnParseSuccess);
                            result.Packets.AddRange(Arg.OnParseSuccess);
                            return true;
                        }
                    }

                    result.Objects.Clear();
                    result.Packets.Clear();
                    result.Error = TriggerParseError.InvalidArg;
                    return false;
                }

                // if there's no arguments specified, just return the default.
                result.Packets.AddRange(OnParseSuccess);
                return true;
            }

            result.Objects.Clear();
            result.Packets.Clear();
            result.Error = TriggerParseError.InvalidTrigger;
            return false;
        }

        private bool TryParseArgs(GameTaskData data, string message, out List<GameObject> objects)
        {
            objects = new List<GameObject>();
            if (Arg.IsArray)
            {
                string[] values = message.Split(',');

                if (!(values.Length <= Arg.Capacity && values.Length >= Arg.RequiredValues))
                    return false;

                foreach (string context in values)
                    if (TryParseArg(data, context.Trim(), out GameObject obj))
                        objects.Add(obj);

                if (!(objects.Count <= Arg.Capacity && objects.Count >= Arg.RequiredValues))
                    return false;

                return true;
            }

            return false;
        }

        // Attempts to parse an argument.
        private bool TryParseArg(GameTaskData data, string message, out GameObject obj)
        {
            message = message.Trim();
            obj = null;

            switch(Arg.Type)
            {
                case GameObjectType.User:
                    Player search = data.Players.Find(x => x.Name == message);
                    if (search != null)
                    {
                        obj = new GameObject(GameObjectType.User, $"user.{search.UserId}");
                        return true;
                    }
                    if (ulong.TryParse(message, out ulong id))
                    {
                        if (data.Players.Any(x => x.UserId == id))
                        {
                            obj = new GameObject(GameObjectType.User, $"user.{id}");
                            return true;
                        }
                    }
                    return false;

                case GameObjectType.String:
                    if (Checks.NotNull(message))
                        obj = new GameObject(GameObjectType.String, message);
                    else
                        obj = new GameObject(GameObjectType.String, "");

                    return true;

                case GameObjectType.Custom:
                    GameArgValue value = Arg.Values.Find(x => x.Value == message);
                    if (value != null)
                    {
                        obj = new GameObject(GameObjectType.Custom, $"{Arg.Id}:{value}");
                        return true;
                    }
                    return false;
            }

            return false;
        }
    }
}