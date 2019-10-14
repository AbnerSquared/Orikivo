using System;
using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// A class defining the argument of a trigger within a game.
    /// </summary>
    public class GameArg
    {
        /// <summary>
        /// Creates a game argument requesting a dynamic object.
        /// </summary>
        /// <param name="name">The name of the argument.</param>
        /// <param name="type">The dynamic object type to be used for the argument.</param>
        public GameArg(string name, GameArgType type)
        {
            if (type == GameArgType.Custom)
                throw new Exception("At least one custom argument field must be specified when using this type.");
            Name = name;
            Type = type;
        }

        /// <summary>
        /// Creates a game argument using custom specified fields.
        /// </summary>
        /// <param name="name">The name of the argument.</param>
        /// <param name="fields">A collection of fields that can be specified.</param>
        public GameArg(string name, List<GameArgField> fields)
        {
            Name = name;
            Type = GameArgType.Custom;
        }
        
        /// <summary>
        /// The name of the argument.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The object type that the argument requires.
        /// </summary>
        public GameArgType Type { get; }

        /// <summary>
        /// A list of arguments that can be considered as valid upon being written.
        /// </summary>
        public List<GameArgField> Fields { get; } = new List<GameArgField>();



        /// <summary>
        /// A collection of attributes to be updated upon a successful argument being parsed. When using fields, this is dependant on their preference.
        /// </summary>
        public List<AttributeUpdatePacket> ToUpdate { get; } = new List<AttributeUpdatePacket>();
    }
}
