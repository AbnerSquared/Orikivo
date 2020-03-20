using Newtonsoft.Json;
using System;

namespace Orikivo.Desync
{
    /// <summary>
    /// Represents a reminder for a <see cref="User"/>.
    /// </summary>
    public class Notification
    {
        public Notification(string content)
        {
            Content = content;
            SentAt = DateTime.UtcNow;
            Read = false;
        }

        [JsonConstructor]
        internal Notification(string content, DateTime sentAt, bool read)
        {
            Content = content;
            SentAt = sentAt;
            Read = read;
        }

        /// <summary>
        /// Represents the content that the <see cref="Notification"/> will display.
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; }

        /// <summary>
        /// Represents the <see cref="DateTime"/> at which the <see cref="Notification"/> was sent.
        /// </summary>
        [JsonProperty("sent_at")]
        public DateTime SentAt { get; set; }

        /// <summary>
        /// Determines if the <see cref="Notification"/> was read.
        /// </summary>
        [JsonProperty("read")]
        public bool Read { get; set; }
    }
}
