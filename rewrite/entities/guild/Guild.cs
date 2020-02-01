using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Orikivo.Unstable
{
    public class Guild
    {
        public Guild(SocketGuild guild)
        {
            Id = guild.Id;
            Name = guild.Name;
            CreatedAt = DateTime.UtcNow;
            OwnerId = guild.OwnerId;
            Balance = 0;
            Exp = 0;
            Objectives = new List<GuildObjective>();
            Events = new List<GuildEvent>();
            Commands = new List<GuildCommand>();
            Config = new GuildConfig();
        }

        [JsonConstructor]
        internal Guild(ulong id, string name, DateTime createdAt, ulong ownerId,
            ulong balance, ulong exp, List<GuildObjective> objectives, List<GuildEvent> events,
            List<GuildCommand> commands, GuildConfig config)
        {
            Id = id;
            Name = name;
            CreatedAt = createdAt;
            OwnerId = ownerId;
            Balance = balance;
            Exp = exp;
            Objectives = objectives ?? new List<GuildObjective>();
            Events = events ?? new List<GuildEvent>();
            Commands = commands ?? new List<GuildCommand>();
            Config = config ?? new GuildConfig();
        }

        [JsonProperty("id")]
        public ulong Id { get; }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; }

        [JsonProperty("owner_id")]
        public ulong OwnerId { get; private set; }
        
        [JsonProperty("balance")]
        public ulong Balance { get; internal set; }
        
        [JsonProperty("exp")]
        public ulong Exp { get; internal set; }
        
        [JsonProperty("objectives")]
        public List<GuildObjective> Objectives { get; internal set; }
        
        [JsonProperty("events")]
        public List<GuildEvent> Events { get; internal set; }
        
        [JsonProperty("commands")]
        public List<GuildCommand> Commands { get; internal set; }

        [JsonProperty("config")]
        public GuildConfig Config { get; private set; }

        // used to update information for a guild that can't be edited from orikivo.
        public void Update(SocketGuild guild)
        {
            if (OwnerId != guild.OwnerId)
                OwnerId = guild.OwnerId;

            if (Name != guild.Name)
                Name = guild.Name;
        }
    }
}
