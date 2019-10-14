using System.Collections.Generic;

namespace Orikivo
{
    /// <summary>
    /// An object defining the required written value of an argument.
    /// </summary>
    public class GameArgField
    {
        /// <summary>
        /// Creates an empty argument field.
        /// </summary>
        /// <param name="name">The name that marks the argument as valid.</param>
        public GameArgField(string name, List<AttributeCriterion> criteria = null)
        {
            Name = name;
            Criteria = criteria ?? new List<AttributeCriterion>();
        }

        /// <summary>
        /// Creates an argument field with specified update values.
        /// </summary>
        /// <param name="name">The name that marks the argument as valid.</param>
        /// <param name="toUpdate">A collection of attributes to be updated.</param>
        public GameArgField(string name, List<AttributeUpdatePacket> toUpdate, List<AttributeCriterion> criteria = null, bool includeParentUpdates = true)
        {
            Name = name;
            ToUpdate = toUpdate;
            Criteria = criteria ?? new List<AttributeCriterion>();
            IncludeParentUpdates = includeParentUpdates;
        }

        /// <summary>
        /// The name that marks the argument as valid.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// A value defining if the main argument updates should be included upon a successful execution.
        /// </summary>
        public bool IncludeParentUpdates { get; } = true;

        /// <summary>
        /// Criteria that must be met before the user can use this argument field. Can be left empty.
        /// </summary>
        public List<AttributeCriterion> Criteria { get; }

        /// <summary>
        /// The list of values to update upon this argument field being executed. If left empty, it will default to the main argument update packets.
        /// </summary>
        public List<AttributeUpdatePacket> ToUpdate { get; } = new List<AttributeUpdatePacket>();
    }
}
