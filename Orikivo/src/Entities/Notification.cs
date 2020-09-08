using Newtonsoft.Json;
using System;

namespace Orikivo
{
    /// <summary>
    /// Represents a notice.
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Initializes a new <see cref="Notification"/> with the specified content.
        /// </summary>
        /// <param name="content">The content to display for this <see cref="Notification"/>.</param>
        public Notification(string content)
        {
            if (!string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("The specified content cannot be null or consist of only whitespace characters", nameof(content));

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
        /// Gets or sets a <see cref="string"/> that represents the content that this <see cref="Notification"/> will display.
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; }

        /// <summary>
        /// Represents the <see cref="DateTime"/> at which this <see cref="Notification"/> was sent.
        /// </summary>
        [JsonProperty("sent_at")]
        public DateTime SentAt { get; }

        /// <summary>
        /// Represents the read status of this <see cref="Notification"/>.
        /// </summary>
        [JsonProperty("read")]
        public bool Read { get; set; }
    }
}
