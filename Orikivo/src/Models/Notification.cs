using System;

namespace Orikivo
{
    public class Notification
    {
        public Notification(string content, NotificationType type = 0) : this(content, DateTime.UtcNow, false, type) { }

        internal Notification(string content, DateTime createdAt, bool read, NotificationType type)
        {
            Requires.NotNull(content, nameof(content));

            Content = content;
            CreatedAt = createdAt;
            Read = read;
            Type = type;
        }

        public string Content { get; }

        public DateTime CreatedAt { get; }

        public bool Read { get; set; }

        public NotificationType Type { get; set; }
    }
}
