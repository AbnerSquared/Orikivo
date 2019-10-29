using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    public class GuildOptions
    {
        private const int MAX_GAME_LIMIT = 4;

        private GuildOptions() { }

        [JsonConstructor]
        public GuildOptions(
            string prefix,
            List<GuildEventData> events,
            bool? allowEvents,
            Dictionary<GuildRole, ulong> customRoles,
            ulong? systemChannelId,
            List<ulong> selfAssignRoles,
            ExceptionSeverity exceptionLevel, List<GuildCommand> commands)
        {
            Prefix = prefix; // prevent editing prefix config
            // allow MaxLobbies, which defines the limit to the lobbies that can be open on your server at a time. Default is 1.
            Events = events ?? Default.Events;
            UseEvents = allowEvents ?? Default.UseEvents;
            SystemRoles = customRoles ?? Default.SystemRoles;
            Commands = commands ?? Default.Commands;
            SystemChannelId = systemChannelId;
            Exceptions = exceptionLevel;
            SelfRoles = selfAssignRoles ?? Default.SelfRoles;
        }

        public static GuildOptions Default
        {
            get
            {
                GuildOptions guildOptions = new GuildOptions();
                guildOptions.Prefix = null;
                guildOptions.Exceptions = ExceptionSeverity.Default;
                guildOptions.PrivacyDeny = GuildPrivacyDeny.Internal; // this hides everything except emojis in the guild by default.
                // separate emojis with duplicate names by guild id? by default, you can just use the first instance of an emoji with that name.
                guildOptions.SystemRoles = new Dictionary<GuildRole, ulong>();
                guildOptions.Commands = new List<GuildCommand>();
                guildOptions.UseEvents = false;
                guildOptions.Events = new List<GuildEventData> { new GuildEventData(GuildEvent.UserJoin) { Message = "Welcome to the server, {mention_user}!" } };
                guildOptions.SelfRoles = new List<ulong>();
                return guildOptions;
            }
        }

        /// <summary>
        /// Determines what is denied for others to see about your guild (Discord-side).
        /// </summary>
        [JsonProperty("privacy")]
        public GuildPrivacyDeny PrivacyDeny { get; set; }

        /// <summary>
        /// Specifies how many games a guild can have open at a time. The upper bound is 4.
        /// </summary>
        [JsonProperty("max_games")]
        public int MaxGameCount { get; internal set; }

        /// <summary>
        /// Defines if the guild utilizes the report system. If disabled, this hides all report commands and hides all report tracking in the 'help' command.
        /// </summary>
        [JsonProperty("reporting")]
        public bool Reporting { get; internal set; }
        
        /// <summary>
        /// The new prefix to use within this guild. If left empty, it defaults to <see cref="OriGlobal.DEFAULT_PREFIX"/>.
        /// </summary>
        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        [JsonProperty("ex_level")]
        public ExceptionSeverity Exceptions { get; set; }

        /// <summary>
        /// The ID of the channel that Orikivo will send event calls to. If left empty, it will attempt to use the system channel set on Discord.
        /// </summary>
        [JsonProperty("system_channel_id")]
        public ulong? SystemChannelId { get; set; }

        [JsonProperty("commands")]
        public List<GuildCommand> Commands { get; private set; }

        /// <summary>
        /// Defines if the guild will execute any of the specified event packets when an event is fired. If disabled, it will not send anything.
        /// </summary>
        [JsonProperty("use_events")]
        public bool UseEvents { get; set; }

        /// <summary>
        /// A collection of events that write a body of text upon matching with an event that is called.
        /// </summary>
        [JsonProperty("events")]
        public List<GuildEventData> Events { get; set; }

        /// <summary>
        /// Specifies an embedder to integrate with a called event. If left empty, it will simply send itself as a message. (Note: Writing key shortcuts also work here)
        /// </summary>
        [JsonProperty("event_embedder")]
        public Embedder EventEmbedder { get; set; }

        /// <summary>
        /// Roles that are integrated with services such as muting, trusting, or default entry roles.
        /// </summary>
        [JsonProperty("system_roles")]
        public Dictionary<GuildRole, ulong> SystemRoles { get; private set; }

        /// <summary>
        /// All <see cref="IRole"/> objects that a user can automatically earn upon reaching the specified level bound to the role.
        /// </summary>
        [JsonProperty("tier_roles")] // int => OriUser.GetLevelForGuild(guildId), ulong => RoleId
        public Dictionary<int, ulong> TierRoles { get; set; }

        /// <summary>
        /// All <see cref="IRole"/> values that a user can assign themselves to.
        /// </summary>
        [JsonProperty("self_roles")] // ulong => RoleId
        public List<ulong> SelfRoles { get; private set; }

        [JsonIgnore]
        public ulong? MuteRoleId
        {
            get
            {
                return SystemRoles.ContainsKey(GuildRole.Muted) ?
                    SystemRoles[GuildRole.Muted] :
                    (ulong?) null;
            }
            set
            {
                if (value.HasValue)
                    if (!SystemRoles.TryAdd(GuildRole.Muted, value.Value))
                        SystemRoles[GuildRole.Muted] = value.Value;
            }
        }

        [JsonIgnore]
        public ulong? TrustRoleId
        {
            get
            {
                return SystemRoles.ContainsKey(GuildRole.Trusted) ?
                    SystemRoles[GuildRole.Trusted] :
                    (ulong?) null;
            }
            set
            {
                if (value.HasValue)
                    if (!SystemRoles.TryAdd(GuildRole.Trusted, value.Value))
                        SystemRoles[GuildRole.Trusted] = value.Value;
            }
        }

        [JsonIgnore]
        public ulong? DefaultRoleId
        {
            get
            {
                return SystemRoles.ContainsKey(GuildRole.Default) ?
                    SystemRoles[GuildRole.Default] :
                    (ulong?) null;
            }
            set
            {
                if (value.HasValue)
                    if (!SystemRoles.TryAdd(GuildRole.Default, value.Value))
                        SystemRoles[GuildRole.Default] = value.Value;
            }
        }

        [JsonIgnore]
        public bool HasPrefix => Checks.NotNull(Prefix);

        [JsonIgnore]
        public IReadOnlyList<GuildEventData> Greetings => Events.Where(x => x.Type == GuildEvent.UserJoin).ToList();

        public void AddEvent(GuildEvent type, string message, string imageUrl = null)
            => Events.Add(new GuildEventData(type) { Message = message, ImageUrl = imageUrl });

        public void AddSelfRole(ulong roleId)
        {
            if (!SelfRoles.Contains(roleId))
                SelfRoles.Add(roleId);
        }

        public void RemoveSelfRole(ulong roleId)
        {
            if (SelfRoles.Contains(roleId))
                SelfRoles.Remove(roleId);
        }

        public void UpdateSelfRole(ulong oldRoleId, ulong newRoleId)
        {
            if (SelfRoles.Contains(oldRoleId))
                SelfRoles[SelfRoles.IndexOf(oldRoleId)] = newRoleId;
        }
    }
}
