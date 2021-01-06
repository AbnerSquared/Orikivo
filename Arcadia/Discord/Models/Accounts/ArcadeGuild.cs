using System;
using System.Collections.Generic;
using Discord;
using Newtonsoft.Json;
using Orikivo;

namespace Arcadia
{
    public class ArcadeGuild : BaseGuild
    {
        public ArcadeGuild(IGuild guild)
            : base(guild)
        {
            Balance = 0;
            Exp = 0;
        }

        [JsonConstructor]
        internal ArcadeGuild(ulong id, string name, DateTime createdAt, ulong ownerId, GuildConfig config,
            long balance, ulong exp, Dictionary<string, long> stats, List<QuestData> quests)
            : base(id, name, createdAt, ownerId, config)
        {
            Balance = balance;
            Exp = exp;
            Stats = stats ?? new Dictionary<string, long>();
            Quests = quests ?? new List<QuestData>();
        }

        [JsonProperty("balance")]
        public long Balance { get; internal set; }

        [JsonProperty("exp")]
        public ulong Exp { get; internal set; }

        [JsonProperty("stats")]
        public Dictionary<string, long> Stats { get; internal set; }

        [JsonProperty("quests")]
        public List<QuestData> Quests { get; internal set; }
    }
}
