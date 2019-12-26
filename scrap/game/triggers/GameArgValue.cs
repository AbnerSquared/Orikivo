using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// An object defining the required written value of an argument.
    /// </summary>
    public class GameArgValue
    {
        /// <summary>
        /// Creates an empty argument field.
        /// </summary>
        /// <param name="value">The name that marks the argument as valid.</param>
        public GameArgValue(string value, List<GameCriterion> criteria = null)
        {
            Value = value;
            Criteria = criteria ?? new List<GameCriterion>();
        }

        /// <summary>
        /// Creates an argument field with specified update values.
        /// </summary>
        /// <param name="value">The name that marks the argument as valid.</param>
        /// <param name="onParseSuccess">A collection of attributes to be updated.</param>
        public GameArgValue(string value, List<GameUpdatePacket> onParseSuccess, List<GameCriterion> criteria = null)
        {
            Value = value;
            OnParseSuccess = onParseSuccess;
            Criteria = criteria ?? new List<GameCriterion>();
        }

        /// <summary>
        /// The name that marks the argument as valid.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Criteria that must be met before the user can use this argument field. Can be left empty.
        /// </summary>
        public List<GameCriterion> Criteria { get; }

        /// <summary>
        /// The list of values to update upon this argument field being executed. If left empty, it will default to the main argument update packets.
        /// </summary>
        public List<GameUpdatePacket> OnParseSuccess { get; } = new List<GameUpdatePacket>();
    }
}
