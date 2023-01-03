using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orikivo
{
    /// <summary>
    /// Represents a <see cref="Notification"/> cache.
    /// </summary>
    public class Notifier
    {
        /// <summary>
        /// Defines the max limit that a <see cref="Notifier"/> can display at once on a message.
        /// </summary>
        public const int NotificationsPerMessage = 3;

        /// <summary>
        /// Initializes a new <see cref="Notifier"/>.
        /// </summary>
        public Notifier()
        {
            Notifications = new List<Notification>();
        }

        /// <summary>
        /// Initializes a new <see cref="Notifier"/> with the specified capacity.
        /// </summary>
        /// <param name="capacity">The maximum amount of notifications that this <see cref="Notifier"/> can store.</param>
        public Notifier(int capacity)
        {
            if (capacity <= 0)
                throw new ArgumentException("The specified capacity cannot be less than or equal to 0", nameof(capacity));

            Notifications = new List<Notification>();
            Capacity = capacity;
        }

        [JsonConstructor]
        internal Notifier(List<Notification> notifications, int capacity)
        {
            Notifications = notifications;
            Capacity = capacity;
        }

        [JsonProperty("notifications")]
        public List<Notification> Notifications { get; set; }

        public int Capacity { get; }

        [JsonIgnore]
        public DateTime? LastNotified { get; set; }

        [JsonIgnore]
        public bool CanNotify => Notifications.Any(x => !x.Read);

        /// <summary>
        /// Appends a new <see cref="Notification"/> to the <see cref="Notifier"/>.
        /// </summary>
        public void Add(string content)
        {
            Notifications.Add(new Notification(content));
        }

        /// <summary>
        /// Returns a <see cref="string"/> that displays all existing notifications.
        /// </summary>
        public string Display()
        {
            var notifier = new StringBuilder();

            foreach (Notification n in Notifications.OrderByDescending(x => x.CreatedAt))
            {
                notifier.AppendLine($"> {n.Content} ({Format.FullTime(n.CreatedAt)})");

                if (!n.Read)
                    n.Read = true;
            }

            return notifier.ToString();
        }

        /// <summary>
        /// Returns a <see cref="string"/> that summarizes all notifications.
        /// </summary>
        public string Notify(int availableLength = -1, bool clearRead = true)
        {
            IEnumerable<Notification> notifications = Notifications
                .Where(x => !x.Read)
                .OrderByDescending(x => x.CreatedAt)
                .ToList();

            if (!Check.NotNullOrEmpty(notifications))
                return "";

            if (notifications.Count() == 1)
            {
                notifications.First().Read = true;

                if (clearRead)
                    Notifications.Remove(notifications.First());

                return Format.Notice(notifications.First().Content);
            }

            var result = new StringBuilder();
            result.Append("> 🔔 **Notifications**");

            int i = 0;

            foreach (Notification notification in notifications)
            {
                if (i >= NotificationsPerMessage || (availableLength > 0 && (availableLength - notification.Content.Length) <= 0))
                    break;

                result.AppendLine($"• {notification.Content}");
                notification.Read = true;

                i++;
            }

            int remainder = notifications.Count() - i;

            if (remainder > 0)
                result.Append($"• and **{remainder:##,0}** more...");

            if (clearRead)
                Notifications.RemoveAll(x => x.Read);

            return result.ToString();
        }
    }
}
