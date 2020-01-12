using Newtonsoft.Json;

namespace Orikivo.Unstable
{
    public class UserConfig
    {
        public UserConfig() { }

        [JsonConstructor]
        internal UserConfig(string prefix, NotifyDeny notifier, bool tooltips, bool debug)
        {
            Prefix = prefix;
            Notifier = notifier;
            Tooltips = tooltips;
            Debug = debug;
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

        /// <summary>
        /// A marker that determines if tooltips will be shown when executing certain commands.
        /// </summary>
        [JsonProperty("tooltips")]
        public bool Tooltips { get; set; }

        /// <summary>
        /// A marker that labels the <see cref="User"/> has a debugger.
        /// </summary>
        [JsonProperty("debug")]
        public bool Debug { get; set; }
    }
}
