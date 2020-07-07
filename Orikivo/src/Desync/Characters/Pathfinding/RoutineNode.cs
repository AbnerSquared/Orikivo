using System;

namespace Orikivo.Desync
{

    // A RoutineNode will implement PathNode instead, as a Node is for one specific day.
    /// <summary>
    /// Represents a specific location at which a <see cref="Routine"/> stops at.
    /// </summary>
    public class RoutineNode : PathNode
    {
        // does the character instantly go to the specified location?
        public bool Instant { get; set; } = false;

        // a character is stopped if someone wants to talk to them

        // if <= 0, this character cannot talk.
        // if unspecified, this character can always talk
        public TimeSpan MaxHoldTime { get; set; }

        // represents the duration at which this character will remain at this location
        // travel time is subtracted from this.
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// If specified, the duration at which the <see cref="Character"/> remains is randomized by the specified offset, which represents the upper bound.
        /// </summary>
        public double LengthRandomOffset { get; set; }

        // when the character arrives at the specified location
        // what are they doing?
        public CharacterAction Action { get; set; }
    }
}
