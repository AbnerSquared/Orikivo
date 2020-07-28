using System.Collections.Generic;
using Newtonsoft.Json;

namespace Orikivo
{
    public class GuildConfig
    {
        public GuildConfig()
        {
            Prefix = null;
            SystemRoles = new Dictionary<SystemRole, ulong>();
        }

        [JsonConstructor]
        internal GuildConfig(string prefix, Dictionary<SystemRole, ulong> systemRoles)
        {
            Prefix = prefix;
            SystemRoles = systemRoles;
        }

        [JsonProperty("prefix")]
        [Aliases("pfx")]
        [Description("A value that will define the default prefix for all users in this server.")]
        public string Prefix { get; set; }

        [Ignore]
        [JsonProperty("system_roles")]
        public Dictionary<SystemRole, ulong> SystemRoles { get; }

        [JsonIgnore]
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
