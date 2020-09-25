using System;
using System.Collections.Generic;

namespace Arcadia
{
    public class GuildQuest
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Summary { get; set; }

        public QuestDifficulty Difficulty { get; set; }

        public QuestType Type { get; set; }
        public Func<ArcadeGuild, bool> ToAssign { get; set; }

        public List<VarCriterion> Criteria { get; internal set; } = new List<VarCriterion>();
    }
}