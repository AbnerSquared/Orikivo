using Newtonsoft.Json;
using System;
using System.Reflection;
using System.Text;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents the configuration for a <see cref="User"/>.
    /// </summary>
    public class UserConfig
    {
        /// <summary>
        /// Attempts to get the <see cref="Type"/> that exists to the specified option. If no matching properties were found, this is returned as null.
        /// </summary>
        public Type GetOptionType(string name)
        {
            return ClassEditor.GetPropertyType(this, name);
        }


        /// <summary>
        /// Attempts to get the value from the specified option. If no matching properties were found, this is returned as null.
        /// </summary>
        public object GetOption(string name)
        {
            return ClassEditor.GetPropertyValue(this, name);
        }

        /// <summary>
        /// Attempts to set the specified option to the specified value. If no option could be found, this does nothing.
        /// </summary>
        public void SetOption(string name, object value)
        {
            ClassEditor.SetPropertyValue(this, name, value);
        }

        // TODO: Move this method into a separate class that handles formatting.
        public string Display()
        {
            var panel = new StringBuilder();

            PropertyInfo[] properties = GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.GetCustomAttribute<IgnoreAttribute>() != null)
                    continue;

                panel.Append("> **");
                panel.Append(property.Name);
                panel.Append("** • `");

                string value = property.GetValue(this, null)?.ToString();
                if (Check.NotNull(value))
                    panel.Append(value);
                else
                    panel.Append("null");
                panel.AppendLine("`");

                string subtitle = property.GetCustomAttribute<DescriptionAttribute>()?.Content;
                
                if (Check.NotNull(subtitle))
                    panel.AppendLine($"> {subtitle}");

                panel.AppendLine();
            }

            return panel.ToString();
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
        internal UserConfig(string prefix, NotifyDeny notifier, UserFlag flag)
        {
            Prefix = prefix;
            Notifier = notifier;
            Flag = flag;
        }

        /// <summary>
        /// Gets or sets the <see cref="string"/> that will represent the prefix for the <see cref="User"/> when executing commands.
        /// </summary>
        [JsonProperty("prefix")]
        [Aliases("pfx")]
        [Description("An optional property that allows you to set your own personal prefix when using **Orikivo.**")]
        public string Prefix { get; set; }

        /// <summary>
        /// Represents what the <see cref="User"/> wants to ignore when being notified.
        /// </summary>
        [JsonProperty("notifier")]
        [Description("Controls the notifiers that have permission to notify you.")]
        public NotifyDeny Notifier { get; set; }

        [Ignore]
        [JsonProperty("flags")]
        public UserFlag Flag { get; set; } = 0;

        /// <summary>
        /// Gets or sets marker that determines if tooltips will be shown when executing certain commands.
        /// </summary>
        [JsonIgnore]
        [Description("A marker that determines if tooltips will be shown when executing certain commands.")]
        public bool Tooltips
        {
            get => Flag.HasFlag(UserFlag.Tooltips);
            set => Flag = GetFlagValue(value, Debug);
        }

        /// <summary>
        /// Gets or sets a marker that labels the <see cref="User"/> as a debug tester.
        /// </summary>
        [JsonIgnore]
        [Description("A marker that labels you as a debug tester.")]
        public bool Debug
        {
            get => Flag.HasFlag(UserFlag.Debug);
            set => Flag = GetFlagValue(Tooltips, value);
        }
    }
}
