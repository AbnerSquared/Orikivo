using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents the configuration for a <see cref="User"/>.
    /// </summary>
    public class UserConfig
    {
        public static readonly UserConfig Default = new UserConfig
        {
            Notifier = 0,
            Prefix = null,
            Tooltips = true
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

                // if (!useFlags && !Enum.GetNames(type).Contains(e.ToString()))
                //    return $"{sum}{(sum == 0 ? " (None)" : "")}";

                string names = sum == 0 ? "None" : useFlags ? string.Join(", ", EnumUtils.GetFlags(value).Select(x => x.ToString())) : e.ToString();

                return $"{sum} ({names})";
            }

            return value.ToString();
        }

        public string ViewOption(string optionId)
        {
            PropertyInfo option = ClassHelper.GetProperty(this, optionId);

            if (option == null)
                return Format.Warning("Unknown option specified.");

            return ViewOption(option);
        }

        public string GetOptionName(string optionId)
        {
            PropertyInfo option = ClassHelper.GetProperty(this, optionId);

            return option?.GetCustomAttribute<TitleAttribute>()?.Name ?? option?.Name ?? optionId;
        }

        public string ViewOption(PropertyInfo option)
        {
            var result = new StringBuilder();
            ClampAttribute clamp = option.GetCustomAttribute<ClampAttribute>();
            string id = option.GetCustomAttribute<IdAttribute>()?.Id ?? option.Name.ToLower();
            string title = option.GetCustomAttribute<TitleAttribute>()?.Name ?? option.Name;
            string summary = option.GetCustomAttribute<DescriptionAttribute>()?.Content;
            string value = WriteOptionValue(option.GetValue(this, null));
            string valueType = WriteOptionType(option);

            Type type = option.GetValue(this, null)?.GetType();

            if (Tooltips)
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

            if (Tooltips)
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

        public string Display()
        {
            var result = new StringBuilder();

            List<PropertyInfo> properties = GetType()
                .GetProperties()
                .Where(x => x.GetCustomAttribute<IgnoreAttribute>() == null)
                .ToList();

            if (Tooltips && properties.Any())
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

            result.AppendLine("> 🔩 **Configuration**");

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

        private static UserFlag GetFlagValue(bool tooltips, bool debug)
        {
            UserFlag flag = 0;

            if (tooltips)
                flag |= UserFlag.Tooltips;

            if (debug)
                flag |= UserFlag.Debug;

            return flag;
        }

        public UserConfig() { }

        [JsonConstructor]
        internal UserConfig(string prefix, NotifyAllow notifier, UserFlag flag)
        {
            Prefix = prefix;
            Notifier = notifier;
            Flag = flag;
        }

        /// <summary>
        /// Gets or sets the <see cref="string"/> that will represent the prefix for the <see cref="User"/> when executing commands.
        /// </summary>
        [JsonProperty("prefix")]
        [Clamp(16)]
        [Description("Represents the prefix that is used to execute commands on **Arcadia**.")]
        [Details("If left unspecified, the default prefix will depend on how the configuration was set for this guild. This can be viewed by typing `guild config`.",
            "Typing `Prefix?` as the new value will require you to execute commands by typing `Prefix?<command>` from now on.")]
        public string Prefix { get; set; }

        /// <summary>
        /// Represents what the <see cref="User"/> wants to ignore when being notified.
        /// </summary>
        [JsonProperty("notifier")]
        [Title("Allowed Notifications")]
        [Id("notifications")]
        [Description("Determines the type of notifications you can receive.")]
        [Details("Typing `1` or `Merit` will set only merits to have the ability to send notifications. If you unlock a merit, a notification is applied.",
            "Typing `6` will set both `Cooldown` and `OfferAccepted`. If a cooldown expires or someone accepts a trade offer you sent, a notification is applied.")]
        public NotifyAllow Notifier { get; set; }

        [Ignore]
        [JsonProperty("flags")]
        public UserFlag Flag { get; set; } = 0;

        [JsonProperty("error_handling")]
        [Title("Error Handling")]
        [Id("error")]
        [Description("Determines how errors are shown to you.")]
        [Details("Typing `0` or `None` will hide any internal details from being shown. This is for users who might not enjoy viewing confusing details.",
            "Typing `1` or `Simple` will only show the most relevant stack trace path. This is useful to quickly see the origin of an error.",
            "Typing `2` or `Full` will display as much of the stack trace as possible.")]
        public StackTraceMode ErrorHandling { get; set; }

        /// <summary>
        /// Gets or sets marker that determines if tooltips will be shown when executing certain commands.
        /// </summary>
        [JsonIgnore]
        [Description("A marker that determines if tooltips will be shown when executing certain commands.")]
        [Details("Tooltips are denoted with the symbol 🛠️.",
            "If this option is disabled, all tooltips will be hidden from every command you execute.")]
        public bool Tooltips
        {
            get => Flag.HasFlag(UserFlag.Tooltips);
            set => Flag = value ? Flag | UserFlag.Tooltips : Flag & ~UserFlag.Tooltips;
        }
    }
}
