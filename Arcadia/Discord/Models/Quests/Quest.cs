using System;
using System.Collections.Generic;

namespace Arcadia
{
    /// <summary>
    /// Represents a generic objective.
    /// </summary>
    public class Quest
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Summary { get; set; }

        public int Difficulty { get; set; }

        public QuestType Type { get; set; }

        // Amount of monthly quest points given on completion
        // If unspecified, this is automatically assigned based on difficulty
        public long Value { get; set; } = 0;

        public Func<ArcadeUser, bool> ToAssign { get; set; }

        public List<Criterion> Criteria { get; internal set; } = new List<Criterion>();

        public Reward Reward { get; set; }

        public int? MaxAllowed { get; set; }
    }
}
