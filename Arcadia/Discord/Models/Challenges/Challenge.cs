using System;
using System.Collections.Generic;

namespace Arcadia
{
    /// <summary>
    /// Represents a basic objective.
    /// </summary>
    public class Challenge
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public int Difficulty { get; set; } // The higher the number, the harder it is
        // You want the difficulty to increase as the user completes more challenge sets
        // For each set completed, increase the cap of where the difficulty cuts off.

        public CriteriaTriggers Triggers { get; set; }

        public Criterion Criterion { get; internal set; }
    }
}
