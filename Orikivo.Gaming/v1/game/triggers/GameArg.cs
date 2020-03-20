using System;
using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// A class defining the argument of a trigger within a game.
    /// </summary>
    public class GameArg
    {
        private GameArg(string name, GameObjectType type, bool isArray = false, List<GameArgValue> values = null, List<GameCriterion> criteria = null, List<GameUpdatePacket> onParseSuccess = null, GameObject defaultValue = null)
        {
            Name = name;
            Type = type;
            IsArray = isArray;
            Values = values;
            Criteria = criteria ?? new List<GameCriterion>();
            OnParseSuccess = onParseSuccess ?? new List<GameUpdatePacket>();
            DefaultValue = defaultValue;
        }

        /// <summary>
        /// Creates a game argument requesting a dynamic object.
        /// </summary>
        /// <param name="name">The name of the argument.</param>
        /// <param name="type">The dynamic object type to be used for the argument.</param>
        public GameArg(string name, GameObjectType type, List<GameCriterion> criteria = null, List<GameUpdatePacket> onParseSuccess = null, bool isArray = false, GameObject defaultValue = null)
            : this(name, type, isArray, null, criteria, onParseSuccess, defaultValue)
        {
            if (type == GameObjectType.Custom)
                throw new Exception("At least one custom argument field must be specified when using this type.");
        }

        /// <summary>
        /// Creates a game argument using custom specified fields.
        /// </summary>
        /// <param name="name">The name of the argument.</param>
        /// <param name="values">A collection of fields that can be specified.</param>
        public GameArg(string name, List<GameArgValue> values, List<GameCriterion> criteria = null, List<GameUpdatePacket> onParseSuccess = null, bool isArray = false, GameObject defaultValue = null)
            : this(name, GameObjectType.Custom, isArray, values, criteria, onParseSuccess, defaultValue) { }
        
        /// <summary>
        /// The name of the argument.
        /// </summary>
        public string Name { get; }

        public string Id => $"arg.{Name}";

        /// <summary>
        /// The object type that the argument requires.
        /// </summary>
        public GameObjectType Type { get; }

        // the default value an arg will be if the arg is left empty.
        public GameObject DefaultValue { get; } = null;
        // If the argument is an array of objects.
        public bool IsArray { get; }
        public bool IsOptional => DefaultValue == null;

        // optional limit to the amount of values that can be added. ignored if left null.
        public int? Capacity { get; }

        // the amount of values that have to be parsed at a minimum.
        public int RequiredValues { get; } = 1;

        /// <summary>
        /// A list of arguments that can be considered as valid upon being written.
        /// </summary>
        public List<GameArgValue> Values { get; }

        public List<GameCriterion> Criteria { get; } = new List<GameCriterion>();

        /// <summary>
        /// A collection of attributes to be updated upon a successful argument being parsed. When using fields, this is dependant on their preference.
        /// </summary>
        public List<GameUpdatePacket> OnParseSuccess { get; } = new List<GameUpdatePacket>();
    }
}
