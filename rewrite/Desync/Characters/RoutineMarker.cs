﻿using System;

namespace Orikivo.Unstable
{
    /// <summary>
    /// Represents a specific location at which a <see cref="Routine"/> stops at.
    /// </summary>
    public class RoutineMarker
    {
        /// <summary>
        /// The destination of the <see cref="RoutineMarker"/>. If nothing is specified, the <see cref="Npc"/> will be hidden for the specified duration.
        /// </summary>
        public Locator Destination { get; set; }

        /// <summary>
        /// The duration at which the <see cref="Npc"/> remains at the specified destination.
        /// </summary>
        public TimeSpan Length { get; set; }

        /// <summary>
        /// If specified, the duration at which the <see cref="Npc"/> remains is randomized by the specified offset, which represents the upper bound.
        /// </summary>
        public double LengthRandomOffset { get; set; }

        /// <summary>
        /// A <see cref="bool"/> that defines if travel time is applied to the <see cref="Npc"/> using this <see cref="RoutineMarker"/>. If set to true, the <see cref="Npc"/> will teleport to their destination.
        /// </summary>
        public bool Instant { get; set; }
    }
}
