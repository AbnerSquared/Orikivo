using System.Collections.Generic;
using Newtonsoft.Json;

namespace Orikivo
{
    public class GuildConfig : ConfigBase
    {
        public static readonly GuildConfig Default = new GuildConfig
        {
            Prefix = null,
            AllowPrefixes = true,
            AllowCooldownNotices = true,
            DefaultRoleId = null,
            TrustedRoleId = null,
            GameServerLimit = 3
        };

        public override ConfigBase GetDefault()
            => Default;

        protected override string Title => "Guild Configuration";

        public GuildConfig()
        {
            Prefix = null;
            SystemRoles = new Dictionary<SystemRole, ulong>();
        }

        [JsonConstructor]
        internal GuildConfig(string prefix,bool allowPrefixes, int gameServerLimit, bool allowCooldownNotices,
            Dictionary<SystemRole, ulong> systemRoles)
        {
            Prefix = prefix;
            AllowPrefixes = allowPrefixes;
            GameServerLimit = gameServerLimit;
            AllowCooldownNotices = allowCooldownNotices;
            SystemRoles = systemRoles ?? new Dictionary<SystemRole, ulong>();
        }

        [JsonProperty("prefix")]
        [Aliases("pfx")]
        [Description("A value that will define the default prefix for all users in this server.")]
        public string Prefix { get; set; }

        [JsonProperty("allow_prefixes")]
        [Id("allow_prefixes")]
        [Title("Allow Prefixes")]
        [Description("Determines if prefixes are allowed in this server.")]
        public bool AllowPrefixes { get; set; }

        [JsonProperty("game_server_limit")]
        [Id("game_server_limit")]
        [Title("Game Server Limit")]
        [Description("Determines the total amount of games that can be handled in this server.")]
        public int GameServerLimit { get; set; } = 3;

        [JsonProperty("allow_cooldown_notices")]
        [Id("allow_cooldown_notices")]
        [Title("Allow Cooldown Notices")]
        [Description("Determines if command cooldowns are displayed to users.")]
        public bool AllowCooldownNotices { get; set; }

        [Ignore]
        [JsonProperty("system_roles")]
        public Dictionary<SystemRole, ulong> SystemRoles { get; }

        [JsonIgnore]
        [Id("default_role_id")]
        [Title("Default Role ID")]
        [Description("Represents the ID of the default role that a user will be given when joining this server.")]
        public ulong? DefaultRoleId
        {
            get => SystemRoles.ContainsKey(SystemRole.Default)
                ? SystemRoles[SystemRole.Default]
                : (ulong?) null;
            set
            {
                if (!value.HasValue)
                    return;

                if (!SystemRoles.TryAdd(SystemRole.Default, value.Value))
                    SystemRoles[SystemRole.Default] = value.Value;
            }
        }

        [JsonIgnore]
        [Id("trusted_role_id")]
        [Title("Trusted Role ID")]
        [Description("Represents the ID of the role that grants a user access to higher level commands.")]
        public ulong? TrustedRoleId
        {
            get => SystemRoles.ContainsKey(SystemRole.Trusted)
                ? SystemRoles[SystemRole.Trusted]
                : (ulong?) null;
            set
            {
                if (!value.HasValue)
                    return;

                if (!SystemRoles.TryAdd(SystemRole.Trusted, value.Value))
                    SystemRoles[SystemRole.Trusted] = value.Value;
            }
        }
    }
}
