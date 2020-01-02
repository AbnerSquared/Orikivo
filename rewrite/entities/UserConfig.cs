namespace Orikivo.Unstable
{
    public class UserConfig
    {
        /// <summary>
        /// An optional prefix that will only be read for the <see cref="User"/>.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Represents what the <see cref="User"/> wants to ignore when being notified.
        /// </summary>
        public NotifyDeny Notifier { get; set; }
    }
}
