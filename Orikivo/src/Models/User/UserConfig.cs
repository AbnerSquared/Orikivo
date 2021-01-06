using Newtonsoft.Json;
using Orikivo.Text;

namespace Orikivo
{
    /// <summary>
    /// Represents the configuration for a <see cref="User"/>.
    /// </summary>
    public class UserConfig : ConfigBase
    {
        public static readonly UserConfig Default = new UserConfig
        {
            Notifier = 0,
            Prefix = null,
            Tooltips = true,
            Language = Language.English
        };

        public override ConfigBase GetDefault()
            => Default;

        private static UserFlag GetFlagValue(bool tooltips, bool debug)
        {
            UserFlag flag = 0;

            if (tooltips)
                flag |= UserFlag.Tooltips;

            if (debug)
                flag |= UserFlag.Debug;

            return flag;
        }

        public UserConfig()
        {
            Notifier = NotifyAllow.Merit | NotifyAllow.Cooldown | NotifyAllow.OfferAccepted | NotifyAllow.OfferInbound | NotifyAllow.GiftInbound | NotifyAllow.Invite | NotifyAllow.Level | NotifyAllow.Research | NotifyAllow.Daily | NotifyAllow.ItemInbound;
            Prefix = null;
            Tooltips = true;
            Language = Language.English;
        }

        [JsonConstructor]
        internal UserConfig(string prefix, NotifyAllow notifier, UserFlag flag, Language language)
        {
            Prefix = prefix;
            Notifier = notifier;
            Flag = flag;
            Language = language == 0 ? Default.Language : language;
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
        /// Gets or sets a marker that determines if tooltips will be shown when executing certain commands.
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

        [Id("language")]
        [JsonProperty("language")]
        [Title("Language")]
        [Description("Determines the text language of this bot.")]
        [Details("This sets the base language for most text values in **Orikivo Arcade**. The excluded values are ones that are required as command input and argument names. An example would be typing a query for the leaderboard command, typed as `leaderboard money`.")]
        public Language Language { get; set; }

        public bool CanNotify(NotifyAllow notifier)
        {
            return Notifier.HasFlag(notifier);
        }
    }
}
