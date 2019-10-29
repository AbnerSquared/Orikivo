using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    public class OriGuild : IDiscordEntity<SocketGuild>, IJsonEntity
    {
        [JsonConstructor]
        internal OriGuild(ulong id, ulong? ownerId, string name, DateTime createdAt, GuildOptions options)
        {
            Id = id;
            OwnerId = ownerId ?? 0;
            Name = name;
            CreatedAt = createdAt;
            Options = options;
        }

        public OriGuild(SocketGuild guild)
        {
            Id = guild.Id;
            OwnerId = guild.OwnerId;
            Name = guild.Name;
            CreatedAt = DateTime.UtcNow;
            Options = GuildOptions.Default;
        }

        /// <summary>
        /// The unique identifier for this guild.
        /// </summary>
        [JsonProperty("id")]
        public ulong Id { get; }

        /// <summary>
        /// The ID of the guild's current owner.
        /// </summary>
        [JsonProperty("owner_id")]
        public ulong OwnerId { get; internal set; }


        /// <summary>
        /// The current name of the guild.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; internal set; }
        
        /// <summary>
        /// The time that this guild account was created.
        /// </summary>
        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; }

        /// <summary>
        /// A local guild balance that can utilized to purchase guild mechanics and systems.
        /// </summary>
        [JsonProperty("balance")]
        public ulong Balance { get; private set; }

        /// <summary>
        /// The guild's current global experience.
        /// </summary>
        [JsonProperty("exp")]
        public ulong Exp { get; private set; }

        /// <summary>
        /// Configuration variables that are used when executing methods.
        /// </summary>
        [JsonProperty("options")]
        public GuildOptions Options { get; private set; }

        /// <summary>
        /// All actions that have been executed on users.
        /// </summary>
        [JsonProperty("actions")] // ulong => UserId
        public Dictionary<ulong, Dictionary<GuildAction, DateTime?>> Actions { get; private set; }

        internal void TryUpdateName(string guildName)
        {
            if (Name != guildName)
                Name = guildName;
        }

        internal void TryUpdateOwner(ulong ownerId)
        {
            if (OwnerId != ownerId)
                OwnerId = ownerId;
        }

        public bool HasMuted(ulong userId)
            => Actions.Where(x => x.Value.Any(x => x.Key == GuildAction.Mute && !DateTimeUtils.IsExpired(x.Value))).Any(x => x.Key == userId);

        // TODO: Possibly create EventContext, which would contain OriGuild, SocketGuild, and SocketUser.
        public string Greet(SocketGuild guild, SocketGuildUser user)
            => OriFormat.ParseGreeting(OriRandom.Choose(Options.Greetings ?? GuildOptions.Default.Greetings).Message, new GuildEventContext(this, guild, user));
        
        private void EnsureActionDataFor(ulong userId)
        {
            if (!Actions.ContainsKey(userId))
                Actions[userId] = new Dictionary<GuildAction, DateTime?>();
        }

        public void Mute(ulong userId, double seconds)
        {
            EnsureActionDataFor(userId);

            if (!Actions[userId].TryAdd(GuildAction.Mute, DateTime.UtcNow.AddSeconds(seconds)))
                Actions[userId][GuildAction.Mute] = (Actions[userId][GuildAction.Mute] ?? DateTime.UtcNow).AddSeconds(seconds);
        }

        public void Blacklist(ulong userId)
        {
            EnsureActionDataFor(userId);
            Actions[userId].TryAdd(GuildAction.Blacklist, null);
        }

        public void Unmute(ulong userId)
        {
            if (HasMuted(userId))
                Actions[userId].Remove(GuildAction.Mute);
        }

        public void AddCommand(GuildCommand command)
        {
            if (Options.Commands.Any(x => x.Name.ToLower() == command.Name.ToLower()))
                throw new Exception("This command name conflicts with an existing command.");
            Options.Commands.Add(command);
        }

        public void RemoveCommand(string command)
        {
            Options.Commands.Remove(Options.Commands.First(x => x.Name.ToLower() == command.ToLower()));
        }

        public bool TryGetSocketEntity(BaseSocketClient client, out SocketGuild guild)
        {
            guild = client.GetGuild(Id);
            return guild != null;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj) || obj == null || GetType() != obj.GetType())
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return Equals(obj as IJsonEntity);
        }

        public bool Equals(IJsonEntity obj)
            => Id == obj.Id;

        public override int GetHashCode()
            => unchecked((int)Id);

        public override string ToString()
            => Name;
    }
}
