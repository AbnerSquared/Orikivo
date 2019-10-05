using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{

    // contains configuration info used with a guild for orikivo
    public class OriGuildOptions
    {
        private OriGuildOptions() { }

        [JsonConstructor]
        public OriGuildOptions(string prefix, List<GuildEvent> events, bool? allowEvents, Dictionary<GuildRoleType, ulong> customRoles,
            ulong? systemChannelId, List<ulong> selfAssignRoles, ExceptionLevel exceptionLevel)
        {
            Prefix = prefix; // prevent editing prefix config
            // allow MaxLobbies, which defines the limit to the lobbies that can be open on your server at a time. Default is 1.
            Events = events ?? Default.Events;
            AllowEvents = allowEvents ?? Default.AllowEvents;
            CustomRoles = customRoles ?? Default.CustomRoles;
            SystemChannelId = systemChannelId;
            ExceptionLevel = exceptionLevel;
            SelfAssignRoles = selfAssignRoles ?? Default.SelfAssignRoles;
        }

        public static RoleProperties MuteRoleProperties
        {
            get
            {
                RoleProperties role = new RoleProperties
                {
                    Name = "Muted",
                    Permissions = new GuildPermissions(66560),
                    Mentionable = false,
                    Hoist = false
                };
                return role;
            }
        }

        // static requirements
        public static OriGuildOptions Default
        {
            get
            {
                OriGuildOptions guildOptions = new OriGuildOptions();
                guildOptions.Prefix = null;
                guildOptions.ExceptionLevel = ExceptionLevel.Default;
                guildOptions.Privacy = GuildPrivacyDeny.Internal; // this hides everything except emojis in the guild by default.
                // separate emojis with duplicate names by guild id? by default, you can just use the first instance of an emoji with that name.
                guildOptions.CustomRoles = new Dictionary<GuildRoleType, ulong>();
                
                guildOptions.AllowEvents = false;
                guildOptions.Events = new List<GuildEvent> { new GuildEvent(GuildEventType.UserJoin) { Message = "Welcome to the server, {mention_user}!" } };
                guildOptions.SelfAssignRoles = new List<ulong>();
                return guildOptions;
            }
        }

        // this is used to determine what's public about a guild.
        [JsonProperty("privacy")]
        public GuildPrivacyDeny Privacy { get; set; }
        
        [JsonProperty("prefix")]
        public string Prefix { get; set; }

        // static properties
        [JsonIgnore]
        public bool HasPrefix => !string.IsNullOrWhiteSpace(Prefix);

        [JsonProperty("events")]
        public List<GuildEvent> Events { get; set; }

        [JsonIgnore]
        public IReadOnlyList<GuildEvent> Greetings => Events.Where(x => x.Type == GuildEventType.UserJoin).ToList();

        [JsonProperty("allow_events")]
        public bool AllowEvents { get; set; }

        [JsonProperty("custom_roles")]
        public Dictionary<GuildRoleType, ulong> CustomRoles {get; private set;}

        [JsonIgnore]
        public ulong? MuteRoleId
        {
            get
            {
                return CustomRoles.ContainsKey(GuildRoleType.Muted) ? CustomRoles[GuildRoleType.Muted] : (ulong?) null;
            }
            set
            {
                if (value.HasValue)
                    if (!CustomRoles.TryAdd(GuildRoleType.Muted, value.Value))
                        CustomRoles[GuildRoleType.Muted] = value.Value;
            }
        }

        [JsonIgnore]
        public ulong? TrustRoleId
        {
            get
            {
                return CustomRoles.ContainsKey(GuildRoleType.Trusted) ? CustomRoles[GuildRoleType.Trusted] : (ulong?) null;
            }
            set
            {
                if (value.HasValue)
                    if (!CustomRoles.TryAdd(GuildRoleType.Trusted, value.Value))
                        CustomRoles[GuildRoleType.Trusted] = value.Value;
            }
        }

        [JsonIgnore]
        public ulong? DefaultRoleId
        {
            get
            {
                return CustomRoles.ContainsKey(GuildRoleType.Default) ? CustomRoles[GuildRoleType.Default] : (ulong?) null;
            }
            set
            {
                if (value.HasValue)
                    if (!CustomRoles.TryAdd(GuildRoleType.Default, value.Value))
                        CustomRoles[GuildRoleType.Default] = value.Value;
            }
        }
        [JsonProperty("system_channel_id")]
        public ulong? SystemChannelId { get; set; }

        // a dictionary with the key being the required level, and the value being the role given on reaching the required level
        //[JsonIgnore]
        //public Dictionary<int, ulong> RoleLevelInfo { get; private set; }

        [JsonProperty("self_assign_roles")]
        public List<ulong> SelfAssignRoles { get; private set; } // roles that the people within the guild can give themselves

        public void AddEvent(GuildEventType type, string message)
            => Events.Add(new GuildEvent(type) { Message = message });

        public void AddSelfRole(ulong roleId)
        {
            if (SelfAssignRoles.Contains(roleId))
                throw new Exception("A role of this type already exists.");
            SelfAssignRoles.Add(roleId);
        }

        public void RemoveSelfRole(ulong roleId)
        {
            if (!SelfAssignRoles.Contains(roleId))
                throw new Exception("There is no role that matches this.");
            SelfAssignRoles.Remove(roleId);
        }

        public void UpdateSelfRole(ulong oldRoleId, ulong newRoleId)
        {
            if (SelfAssignRoles.Contains(oldRoleId))
                SelfAssignRoles[SelfAssignRoles.IndexOf(oldRoleId)] = newRoleId;
        }

        [JsonProperty("ex_level")]
        public ExceptionLevel ExceptionLevel { get; set; }
    }
}
