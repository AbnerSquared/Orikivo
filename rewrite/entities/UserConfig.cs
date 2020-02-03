using Discord.Commands;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace Orikivo.Unstable
{
    public class UserConfig
    {
        public string GetPanel()
        {
            StringBuilder panel = new StringBuilder();

            PropertyInfo[] properties = GetType().GetProperties();

            foreach (PropertyInfo property in properties)
            {
                if (property.GetCustomAttribute<ReadIgnoreAttribute>() != null)
                    continue;

                panel.Append("> **");
                panel.Append(property.Name);
                panel.Append("** • `");

                string value = property.GetValue(this, null)?.ToString();
                if (Checks.NotNull(value))
                    panel.Append(value);
                else
                    panel.Append("null");
                panel.AppendLine("`");

                string subtitle = property.GetCustomAttribute<SubtitleAttribute>()?.Subtitle;
                
                if (Checks.NotNull(subtitle))
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
        /// An optional prefix that will only be read for the <see cref="User"/>.
        /// </summary>
        [JsonProperty("prefix")]
        [Aliases("pfx")]
        [Subtitle("An optional property that allows you to set your own personal prefix when using **Orikivo.**")]
        public string Prefix { get; set; }

        /// <summary>
        /// Represents what the <see cref="User"/> wants to ignore when being notified.
        /// </summary>
        [JsonProperty("notifier")]
        [Subtitle("Controls the notifiers that have permission to notify you.")]
        public NotifyDeny Notifier { get; set; }

        [ReadIgnore]
        [JsonProperty("flags")]
        public UserFlag Flag { get; set; } = 0;

        /// <summary>
        /// A marker that determines if tooltips will be shown when executing certain commands.
        /// </summary>
        [JsonIgnore]
        [Subtitle("A marker that determines if tooltips will be shown when executing certain commands.")]
        public bool Tooltips { get => Flag.HasFlag(UserFlag.Tooltips); set => Flag = GetFlagValue(value, Debug); }

        /// <summary>
        /// A marker that labels the <see cref="User"/> as a debug tester.
        /// </summary>
        [JsonIgnore]
        [Subtitle("A marker that labels you as a debug tester.")]
        public bool Debug { get => Flag.HasFlag(UserFlag.Debug); set => Flag = GetFlagValue(Tooltips, value); }
    }
}
