using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a <see cref="Notification"/> cache.
    /// </summary>
    public class Notifier
    {
        /// <summary>
        /// Defines the max limit that a <see cref="Notifier"/> can display at once on a message.
        /// </summary>
        public const int MAX_NOTIFIER_DISPLAY = 4;  // if there is more, do +2 more... after all notifications.
        public Notifier()
        {
            Notifications = new List<Notification>();
        }

        [JsonConstructor]
        internal Notifier(List<Notification> notifications)
        {
            Notifications = notifications;
        }

        [JsonProperty("notifications")]
        public List<Notification> Notifications { get; set; } = new List<Notification>();

        [JsonIgnore]
        public DateTime? LastNotified { get; set; }

        /// <summary>
        /// Appends a new <see cref="Notification"/> to the <see cref="Notifier"/>.
        /// </summary>
        public void Append(string content)
        {
            Notifications.Add(new Notification(content));
        }

        /// <summary>
        /// Returns a <see cref="string"/> that displays all existing notifications.
        /// </summary>
        public string Display()
        {
            StringBuilder notifier = new StringBuilder();

            int i = 0;
            foreach (Notification n in Notifications.OrderByDescending(x => x.SentAt))
            {
                notifier.AppendLine($"> {n.Content} ({OriFormat.Time(n.SentAt)})");

                if (!n.Read)
                    n.Read = true;
            }

            return notifier.ToString();
        }

        // clears all read notifications
        public void ClearRead()
            => Notifications.RemoveAll(x => x.Read);

        // clears all notifications
        public void Clear()
        {
            Notifications.Clear();
        }

        // determines if the notifier can notify
        [JsonIgnore]
        public bool CanNotify => Notifications.Any(x => !x.Read);

        // returns a string with all notifications, and clears the cache.
        // Returns a <see cref="string"/> formatting all existing notifications and clears them.
        /// <summary>
        /// Returns a <see cref="string"/> that summarizes all notifications.
        /// </summary>
        public string Notify()
        {
            // TODO: configure based on remaining space.
            // If there is barely any space, simply do > !100
            StringBuilder notifier = new StringBuilder();

            int i = 0;
            foreach(Notification n in Notifications.Where(x => !x.Read).OrderByDescending(x => x.SentAt))
            {
                if (i < 3)
                {
                    notifier.AppendLine($"> {n.Content}");
                    n.Read = true;
                }
                else
                {
                    int remainder = Notifications.Where(x => !x.Read).Count();

                    if (remainder > 0)
                        notifier.AppendLine($" > and **+{OriFormat.PlaceValue(remainder)}** more!");

                    break;
                }
                i++;
            }

            return notifier.ToString();
        }
    }
}
