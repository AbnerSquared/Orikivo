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

        public QuestType Type { get; set; }

        // The requirements needed to assign this quest
        public Func<ArcadeUser, bool> ToAssign { get; set; }

        public List<StatCriterion> Criteria { get; internal set; } = new List<StatCriterion>();

        public Reward Reward { get; set; }

        public int? MaxAllowed { get; set; }
    }

    public class GuildQuest
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Summary { get; set; }

        public QuestDifficulty Difficulty { get; set; }

        public QuestType Type { get; set; }
        public Func<ArcadeGuild, bool> ToAssign { get; set; }

        public List<StatCriterion> Criteria { get; internal set; } = new List<StatCriterion>();
    }
}
