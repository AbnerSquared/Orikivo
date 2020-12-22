using System;
using Orikivo;

namespace Arcadia
{
    /// <summary>
    /// Represents the result from an <see cref="Item"/> usage.
    /// </summary>
    public class UsageResult
    {
        /// <summary>
        /// Returns a successful <see cref="UsageResult"/> with a cooldown.
        /// </summary>
        /// <param name="cooldown">The cooldown duration to apply on the <see cref="Item"/> that was used.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="mode">The cooldown mode to apply on the <see cref="Item"/> that was used.</param>
        public static UsageResult FromSuccessCooldown(TimeSpan cooldown, string message = null, CooldownTarget? mode = null)
        {
            return new UsageResult(message, true)
            {
                Cooldown = cooldown,
                CooldownMode = mode
            };
        }

        /// <summary>
        /// Returns a successful <see cref="UsageResult"/> with a cooldown.
        /// </summary>
        /// <param name="cooldown">The cooldown duration to apply on the <see cref="Item"/> that was used.</param>
        /// <param name="message">The message to display.</param>
        /// <param name="mode">The cooldown mode to apply on the <see cref="Item"/> that was used.</param>
        public static UsageResult FromSuccessCooldown(TimeSpan cooldown, Message message = null, CooldownTarget? mode = null)
        {
            return new UsageResult(message, true)
            {
                Cooldown = cooldown,
                CooldownMode = mode
            };
        }

        /// <summary>
        /// Returns a successful <see cref="UsageResult"/>.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public static UsageResult FromSuccess(string message = null)
        {
            return new UsageResult(message, true);
        }

        /// <summary>
        /// Returns a successful <see cref="UsageResult"/>.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public static UsageResult FromSuccess(Message message = null)
        {
            return new UsageResult(message, true);
        }

        /// <summary>
        /// Returns a failed <see cref="UsageResult"/>.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public static UsageResult FromError(string message = null)
        {
            return new UsageResult(message, false);
        }

        /// <summary>
        /// Returns a failed <see cref="UsageResult"/>.
        /// </summary>
        /// <param name="message">The message to display.</param>
        public static UsageResult FromError(Message message = null)
        {
            return new UsageResult(message, false);
        }

        private UsageResult(string content, bool isSuccess)
        {
            Message = new MessageBuilder(content).Build();
            IsSuccess = isSuccess;
        }

        private UsageResult(Message message, bool isSuccess)
        {
            Message = message;
            IsSuccess = isSuccess;
        }

        /// <summary>
        /// Represents the message that specified for this <see cref="UsageResult"/>.
        /// </summary>
        public Message Message { get; }

        /// <summary>
        /// Specifies if this <see cref="UsageResult"/> was successful.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// Represents the cooldown to apply for the <see cref="Item"/> that was used (optional).
        /// </summary>
        public TimeSpan? Cooldown { get; internal set; }

        /// <summary>
        /// Represents the cooldown mode for the <see cref="Item"/> that was used (optional).
        /// </summary>
        public CooldownTarget? CooldownMode { get; internal set; }
    }
}
