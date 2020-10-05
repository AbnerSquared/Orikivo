using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using Orikivo.Desync;

namespace Orikivo
{
    public class GuildConfig
    {
        public static readonly GuildConfig Default = new GuildConfig
        {
            Prefix = null,
            AllowPrefixes = true,
            AllowCooldownNotices = true,
            DefaultRoleId = null,
            TrustedRoleId = null,
            GameServerLimit = 5
        };

        public void SetOptionDefault(string name)
        {
            ClassHelper.SetPropertyValue(this, name, ClassHelper.GetPropertyValue(Default, name));
        }

        public Type GetOptionType(string name)
        {
            return ClassHelper.GetPropertyType(this, name);
        }

        public object GetOption(string name)
        {
            return ClassHelper.GetPropertyValue(this, name);
        }

        public void SetOption(string name, object value)
        {
            ClassHelper.SetPropertyValue(this, name, value);
        }

        public string WriteOptionValue(object value)
        {
            if (value == null)
                return "N/A";

            Type type = value.GetType();

            if (type.IsEnum && value is Enum e)
            {
                long sum = Convert.ToInt64(e);
                bool useFlags = type.GetCustomAttribute<FlagsAttribute>() != null;

                if (sum == type.GetEnumValues().Cast<Enum>().Select(Convert.ToInt64).Sum())
                {
                    return $"{sum} (All)";
                }

                // if (!useFlags && !Enum.GetNames(type).Contains(e.ToString()))
                //    return $"{sum}{(sum == 0 ? " (None)" : "")}";

                string names = sum == 0 ? "None" : useFlags ? string.Join(", ", EnumUtils.GetFlags(value).Select(x => x.ToString())) : e.ToString();

                return $"{sum} ({names})";
            }

            return value.ToString();
        }

        public string ViewOption(string optionId, bool allowTooltips = true)
        {
            PropertyInfo option = ClassHelper.GetProperty(this, optionId);

            if (option == null)
                return Format.Warning("Unknown option specified.");

            return ViewOption(option, allowTooltips);
        }

        public string GetOptionName(string optionId)
        {
            PropertyInfo option = ClassHelper.GetProperty(this, optionId);

            return option?.GetCustomAttribute<TitleAttribute>()?.Name ?? option?.Name ?? optionId;
        }

        public string ViewOption(PropertyInfo option, bool allowTooltips = true)
        {
            var result = new StringBuilder();
            ClampAttribute clamp = option.GetCustomAttribute<ClampAttribute>();
            string id = option.GetCustomAttribute<IdAttribute>()?.Id ?? option.Name.ToLower();
            string title = option.GetCustomAttribute<TitleAttribute>()?.Name ?? option.Name;
            string summary = option.GetCustomAttribute<DescriptionAttribute>()?.Content;
            string value = WriteOptionValue(option.GetValue(this, null));
            string valueType = WriteOptionType(option);

            Type type = option.GetValue(this, null)?.GetType();

            if (allowTooltips)
            {
                string tipBase = $"config {id}";
                List<string> tooltips = option.GetCustomAttribute<TooltipAttribute>()?.Tips.ToList() ?? new List<string>();

                bool useFlags = type?.GetCustomAttribute<FlagsAttribute>() != null;

                if (clamp != null && type != null && type == typeof(int))
                {
                    if (clamp.HasMin)
                        tooltips.Add($"Type `{tipBase} --min` to set the lowest possible value.");

                    tooltips.Add($"Type `{tipBase} --max` to set the highest possible value.");
                }

                if (type != null && type.IsEnum)
                {
                    if (useFlags)
                    {
                        tooltips.Add($"Type `{tipBase} --clear` to clear all specified values.");
                        tooltips.Add($"Type `{tipBase} --all` to set all possible values.");
                        tooltips.Add($"You can chain values together by typing the sum of each flag you want to toggle.");
                        tooltips.Add($"Specifying an active flag will disable it.");
                    }
                }

                tooltips.Add($"Type `{tipBase} --default` to revert to its default value.");
                tooltips.Add($"Type `{tipBase} <value>` to update its current value.");

                result.AppendLine(Format.Tooltip(tooltips));
                result.AppendLine();
            }

            result.AppendLine($"> **{title}**");

            if (!string.IsNullOrWhiteSpace(summary))
                result.AppendLine($"> {summary}");

            result.AppendLine($"> Current Value: `{value}`");
            result.AppendLine($"> Value Type: `{valueType}`");

            if (clamp != null)
            {
                if (!clamp.HasMin)
                {
                    if (type == typeof(string))
                    {
                        result.AppendLine($"> Max Allowed Length: `{clamp.Max}`");
                    }
                    else
                    {
                        result.AppendLine($"> Limit: `{clamp.Max}`");
                    }
                }
                else
                {
                    result.AppendLine($"> Value Range: `{clamp.Min} to {clamp.Max}`");
                }
            }

            if (type != null && type.IsEnum)
            {
                List<string> names = type.GetEnumNames().ToList();
                List<long> values = type.GetEnumValues().Cast<object>().Select(Convert.ToInt64).ToList();

                IEnumerable<string> groups = names.Join(values,
                    a => names.IndexOf(a),
                    b => values.IndexOf(b),
                    (a, b) => $"{a} = {b}");

                if (groups.Any())
                {
                    result.AppendLine($"\n> **Values**\n```cs");
                    result.AppendJoin(",\n", groups);
                    result.Append("```");
                }
            }

            if (allowTooltips)
            {
                IEnumerable<string> details = option.GetCustomAttribute<DetailsAttribute>()?.Details;

                if (details?.Any() ?? false)
                {
                    result.AppendLine($"\n> 🛠️ **Details**");
                    result.AppendJoin("\n", details.Select(x => $"• {x}"));
                }
            }

            return result.ToString();
        }

        private string WriteOptionType(PropertyInfo option)
        {
            object value = option.GetValue(this);

            if (value is null)
                return "Unspecified";

            if (value is string)
                return "Text";

            if (value is int)
                return "Number";

            if (value is bool)
                return "Boolean";

            if (value.GetType().IsEnum)
            {
                if (value.GetType().GetCustomAttribute<FlagsAttribute>() != null)
                    return "Flag";

                return "Flag (Single)";
            }

            return "Object";
        }

        public string Display(bool allowTooltips = true)
        {
            var result = new StringBuilder();

            List<PropertyInfo> properties = GetType()
                .GetProperties()
                .Where(x => x.GetCustomAttribute<IgnoreAttribute>() == null)
                .ToList();

            if (allowTooltips && properties.Any())
            {
                var tooltips = new List<string>
                {
                    "If an ID is unspecified, you can use their name instead as input.",
                    "Type `config <id>` to view more details about a specific option.",
                    "Type `config <id> <value>` to update the value of a specific option."
                };

                result.AppendLine(Format.Tooltip(tooltips));
                result.AppendLine();
            }

            result.AppendLine("> 🔩 **Guild Configuration**");

            if (!properties.Any())
            {
                result.AppendLine("> There are no options to configure at this time.");
                return result.ToString();
            }

            foreach (PropertyInfo option in properties)
            {
                string id = option.GetCustomAttribute<IdAttribute>()?.Id;
                string title = option.GetCustomAttribute<TitleAttribute>()?.Name ?? option.Name;
                string summary = option.GetCustomAttribute<DescriptionAttribute>()?.Content;
                string value = WriteOptionValue(option.GetValue(this, null));

                result.AppendLine();

                if (!string.IsNullOrWhiteSpace(id))
                    result.AppendLine($"> `{id}`");

                result.AppendLine($"> **{title}**: `{value}`");

                if (!string.IsNullOrWhiteSpace(summary))
                    result.AppendLine($"> {summary}");
            }

            return result.ToString();
        }

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
