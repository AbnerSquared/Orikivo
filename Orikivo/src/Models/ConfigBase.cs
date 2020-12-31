using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Orikivo
{
    /// <summary>
    /// Represents the base for a dynamic configuration class.
    /// </summary>
    public abstract class ConfigBase
    {
        public abstract ConfigBase GetDefault();

        protected virtual string Title => "Configuration";

        public void SetOptionDefault(string name)
        {
            ClassHelper.SetPropertyValue(this, name, ClassHelper.GetPropertyValue(GetDefault(), name));
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
                    result.AppendLine($"\n> 🛠️ **Remarks**");
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
                // TODO: Re-position tooltips to be handled by the localization helpers
                var tooltips = new List<string>
                {
                    "If an ID is unspecified, you can use their name instead as input.",
                    "Type `config <id>` to view more details about a specific option.",
                    "Type `config <id> <value>` to update the value of a specific option."
                };

                result.AppendLine(Format.Tooltip(tooltips));
                result.AppendLine();
            }

            result.AppendLine($"> 🔩 **{Title}**");

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

        public string SetOrUpdateValue(string id, string value)
        {
            PropertyInfo option = ClassHelper.GetProperty(this, id);

            if (option == null)
            {
                return Format.Warning("An unknown option was specified.");
            }

            Type type = GetOptionType(id);
            var clamp = option.GetCustomAttribute<ClampAttribute>();
            bool useFlags = type.GetCustomAttribute<FlagsAttribute>() != null;

            var panel = new StringBuilder();
            panel.AppendLine($"> **{GetOptionName(id)}**");

            switch (value)
            {
                case "--default":
                    SetOptionDefault(id);
                    panel.AppendLine("> The specified option has been reset.");
                    break;

                case "--min":
                    {
                        if (type != typeof(int))
                        {
                            panel.AppendLine("> This method can only be used on a `Number` with a specified minimum range.");
                        }
                        else if (clamp == null || !clamp.HasMin)
                        {
                            panel.AppendLine("> This `Number` does not have a specified minimum range.");
                        }
                        else
                        {
                            SetOption(id, clamp.Min);
                            panel.AppendLine("> The specified option has been set to its lowest possible value.");
                        }

                        break;
                    }

                case "--max":
                    {
                        if (type != typeof(int))
                        {
                            panel.AppendLine("> This method can only be used on a `Number` with a specified maximum range.");
                        }
                        else if (clamp == null)
                        {
                            panel.AppendLine("> This `Number` does not have a specified maximum range.");
                        }
                        else
                        {
                            SetOption(id, clamp.Max);
                            panel.AppendLine("> The specified option has been set to its highest possible value.");
                        }

                        break;
                    }

                case "--none":
                    {
                        if (!type.IsEnum || !useFlags)
                        {
                            panel.AppendLine("> This method can only be used on a `Flag`.");
                        }
                        else if (!Enum.TryParse(type, "0", out object e))
                        {
                            panel.AppendLine("> An error occurred while attempted to clear all flags.");
                        }
                        else
                        {
                            SetOption(id, e);
                            panel.AppendLine("> Cleared all flags.");
                        }

                        break;
                    }

                case "--all":
                    {
                        if (!type.IsEnum || !useFlags)
                        {
                            panel.AppendLine("> This method can only be used on a `Flag`.");
                        }
                        else if (!Enum.TryParse(type, $"{type.GetEnumValues().Cast<Enum>().Select(Convert.ToInt64).Sum()}", out object e))
                        {
                            panel.AppendLine("> An error occurred while attempting to activate all flags.");
                        }
                        else
                        {
                            SetOption(id, e);
                            panel.AppendLine("> Activated all flags.");
                        }

                        break;
                    }

                default:
                    if (Parser.TryParse(type, value, out object result))
                    {
                        if (type.IsEnum)
                        {
                            long flagValue = Convert.ToInt64(result);
                            if (flagValue < 0)
                            {
                                panel.AppendLine("> Flags cannot be negative.");
                                break;
                            }

                            long partialSum = EnumUtils.GetFlags(result).Select(Convert.ToInt64).Sum();

                            if (flagValue > 0)
                            {
                                if (flagValue - partialSum > 0)
                                {
                                    panel.AppendLine("> The flag summation contains an invalid flag.");
                                    break;
                                }
                            }
                        }
                        if (type == typeof(string) && result is string s)
                        {
                            if (s.Contains("\n"))
                            {
                                panel.AppendLine("> The specified value cannot contain any line breaks.");
                                break;
                            }

                            if (clamp != null)
                            {
                                if (s.Length > clamp.Max)
                                {
                                    panel.AppendLine($"> The specified value cannot be larger than `{clamp.Max}`.");
                                    break;
                                }
                            }
                        }
                        if (type == typeof(int) && result is int i)
                        {
                            if (clamp != null)
                            {
                                if (clamp.HasMin && (i < clamp.Min || i > clamp.Max))
                                {
                                    panel.AppendLine($"> The specified value is out of range (`{clamp.Min} to {clamp.Max}`).");
                                    break;
                                }

                                if (i > clamp.Max)
                                {
                                    panel.AppendLine($"> The specified value cannot be larger than `{clamp.Max}`.");
                                    break;
                                }
                            }
                        }

                        SetOption(id, result);
                        panel.Append($"> The specified value has been set to `{WriteOptionValue(result)}`.");
                    }
                    else
                    {
                        panel.AppendLine("> The specified value could not be parsed.");

                        if (type.IsEnum)
                        {
                            panel.AppendLine();
                            List<string> names = type.GetEnumNames().ToList();
                            List<long> values = type.GetEnumValues().Cast<object>().Select(Convert.ToInt64).ToList();
                            List<string> groups = names.Join(values,
                                a => names.IndexOf(a),
                                b => values.IndexOf(b),
                                (a, b) => $"{a} = {b}")
                                .ToList();

                            if (groups.Any())
                            {
                                panel.AppendLine($"> **Values**\n```cs");
                                panel.AppendJoin(",\n", groups);
                                panel.AppendLine("```");
                            }
                        }
                    }

                    break;
            }

            return panel.ToString();
        }
    }
}
