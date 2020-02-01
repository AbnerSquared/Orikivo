using Newtonsoft.Json;

namespace Orikivo.Unstable
{
    public class UserConfig
    {
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
        public string Prefix { get; set; }

        /// <summary>
        /// Represents what the <see cref="User"/> wants to ignore when being notified.
        /// </summary>
        [JsonProperty("notifier")]
        public NotifyDeny Notifier { get; set; }

        [JsonProperty("flags")]
        public UserFlag Flag { get; set; } = 0;

        /// <summary>
        /// A marker that determines if tooltips will be shown when executing certain commands.
        /// </summary>
        [JsonIgnore]
        public bool Tooltips { get => Flag.HasFlag(UserFlag.Tooltips); set => Flag = GetFlagValue(value, Debug); }

        /// <summary>
        /// A marker that labels the <see cref="User"/> as a debug tester.
        /// </summary>
        [JsonIgnore]
        public bool Debug { get => Flag.HasFlag(UserFlag.Debug); set => Flag = GetFlagValue(Tooltips, value); }
    }
}
