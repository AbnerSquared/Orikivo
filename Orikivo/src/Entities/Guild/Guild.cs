using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a guild account for <see cref="Orikivo"/>.
    /// </summary>
    public class BaseGuild : IDiscordEntity<SocketGuild>, IJsonEntity
    {
        public BaseGuild(SocketGuild guild)
        {
            Id = guild.Id;
            Name = guild.Name;
            CreatedAt = DateTime.UtcNow;
            OwnerId = guild.OwnerId;
            Config = new GuildConfig();
        }

        [JsonConstructor]
        internal BaseGuild(ulong id, string name, DateTime createdAt, ulong ownerId,
            GuildConfig config)
        {
            Id = id;
            Name = name;
            CreatedAt = createdAt;
            OwnerId = ownerId;
            Config = config ?? new GuildConfig();
        }

        // Barebones property
        [JsonProperty("id")]
        public ulong Id { get; }

        // Barebones property
        [JsonProperty("name")]
        public string Name { get; private set; }

        // Barebones property
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; }

        // Barebones property
        [JsonProperty("owner_id")]
        public ulong OwnerId { get; private set; }
        
        // Arcadia property
        // public ulong Balance { get; internal set; }
        
        // Arcadia property
        // public ulong Exp { get; internal set; }

        // Moderation property
        // public List<GuildEvent> Events { get; internal set; }
        
        // Arcadia property
        // public List<GuildCommand> Commands { get; internal set; }

        // Barebones property
        [JsonProperty("config")]
        public GuildConfig Config { get; private set; }

        // used to update information for a guild that can't be edited from orikivo.
        public void Synchronize(SocketGuild guild)
        {
            if (OwnerId != guild.OwnerId)
                OwnerId = guild.OwnerId;

            if (Name != guild.Name)
                Name = guild.Name;
        }

        public bool GetEntity(BaseSocketClient client, out SocketGuild guild)
        {
            guild = client.GetGuild(Id);
            return guild != null;
        }

        public override bool Equals(object obj)
            => obj is null || obj == null || GetType() != obj.GetType() ?
            false : ReferenceEquals(this, obj) ?
            true : Equals(obj as IJsonEntity);

        public override int GetHashCode()
            => unchecked((int)Id);

        public bool Equals(IJsonEntity obj)
            => Id == obj.Id;
    }
}
