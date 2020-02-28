using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a notification cache.
    /// </summary>
    public class Notifier
    {
        public const int MAX_NOTIFIER_DISPLAY = 4;  // if there is more, do +2 more... after all notifications.
        public Notifier() { }

        [JsonConstructor]
        internal Notifier(List<string> notifications)
        {
            Notifications = notifications;
        }

        // TODO: Create NotifyType
        [JsonProperty("notifications")]
        public List<string> Notifications { get; set; }

        public void Append(string notification)
        {
            Notifications.Add(notification);
        }

        // returns a string with all notifications, and clears the cache.
        /// <summary>
        /// Returns a <see cref="string"/> formatting all existing notifications and clears them.
        /// </summary>
        public string Release()
        {
            StringBuilder notifier = new StringBuilder();
            foreach(string notif in Notifications)
            {
                notifier.AppendLine($"> {notif}");
            }

            Notifications.Clear();
            return notifier.ToString();
        }
    }
}
