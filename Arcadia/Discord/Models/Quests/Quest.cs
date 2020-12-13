using System;
using System.Collections.Generic;

namespace Arcadia
{
    public class Quest
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Summary { get; set; }

        public QuestDifficulty Difficulty { get; set; }

        // This is used for challenges, find a way to merge with quest difficulties
        internal int InnerDifficulty { get; set; } = 0;

        public QuestType Type { get; set; }

        // Amount of monthly quest points given on completion
        public long Value { get; set; } = 0;

        // This is the trigger call for this quest to be judged
        public CriteriaTriggers Triggers { get; set; }

        // The requirements needed to assign this quest
        public Func<ArcadeUser, bool> ToAssign { get; set; }

        public List<VarCriterion> Criteria { get; internal set; } = new List<VarCriterion>();

        public Reward Reward { get; set; }

        public int? MaxAllowed { get; set; }
    }
}
