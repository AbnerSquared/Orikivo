using Newtonsoft.Json;
using System;
using Discord;

namespace Orikivo
{
    /// <summary>
    /// Represents a generic guild account.
    /// </summary>
    public class BaseGuild : IJsonEntity
    {
        public BaseGuild(IGuild guild)
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

        [JsonProperty("id")]
        public ulong Id { get; }

        [JsonProperty("name")]
        public string Name { get; private set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; }

        [JsonProperty("owner_id")]
        public ulong OwnerId { get; private set; }

        [JsonProperty("config")]
        public GuildConfig Config { get; private set; }

        public void Synchronize(IGuild guild)
        {
            if (OwnerId != guild.OwnerId)
                OwnerId = guild.OwnerId;

            if (Name != guild.Name)
                Name = guild.Name;
        }

        public override bool Equals(object obj)
            => obj != null && GetType() == obj.GetType() && (ReferenceEquals(this, obj) || Equals(obj as IJsonEntity));

        public override int GetHashCode()
            => unchecked((int) Id);

        public bool Equals(IJsonEntity obj)
            => Id == obj?.Id;
    }
}
