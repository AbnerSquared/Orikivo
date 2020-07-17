using Discord.WebSocket;
using Newtonsoft.Json;
using System;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a generic user account.
    /// </summary>
    public class BaseUser : IDiscordEntity<SocketUser>, IJsonEntity
    {
        public BaseUser(SocketUser user)
        {
            Id = user.Id;
            Username = user.Username;
            Discriminator = user.Discriminator;
            CreatedAt = DateTime.UtcNow;
            Config = new UserConfig();
        }

        [JsonConstructor]
        public BaseUser(ulong id, string username, string discriminator, DateTime createdAt, UserConfig config)
        {
            Id = id;
            Username = username;
            Discriminator = discriminator;
            CreatedAt = createdAt;
            Config = config;
        }

        [JsonProperty("id")]
        public ulong Id { get; }

        [JsonProperty("username")]
        public string Username { get; protected set; }

        [JsonProperty("discriminator")]
        public string Discriminator { get; protected set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; }

        [JsonProperty("config")]
        public UserConfig Config { get; }

        public bool GetEntity(BaseSocketClient client, out SocketUser user)
        {
            user = client.GetUser(Id);
            return user != null;
        }

        public void Synchronize(SocketUser user)
        {
            if (Id != user.Id)
                throw new Exception("The user specified must have the same matching ID as the account.");

            Username = user.Username;
            Discriminator = user.Discriminator;
        }

        public override bool Equals(object obj)
            => obj is null || obj == null || GetType() != obj.GetType() ?
            false : ReferenceEquals(this, obj) ?
            true : Equals(obj as IJsonEntity);

        public bool Equals(IJsonEntity obj)
            => Id == obj.Id;

        public override int GetHashCode()
            => unchecked((int)Id);

        public override string ToString()
            => $"{Username}#{Discriminator}";
    }
}

