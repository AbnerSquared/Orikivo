using Orikivo;

namespace Arcadia
{
    /// <summary>
    /// Represents the result from an <see cref="Item"/> that was used.
    /// </summary>
    public class UsageResult
    {
        public static UsageResult FromSuccess(string message = null)
        {
            return new UsageResult(message, true);
        }

        public static UsageResult FromError(string message = null)
        {
            return new UsageResult(message, false);
        }

        public static UsageResult FromSuccess(Message message = null)
        {
            return new UsageResult(message, true);
        }

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

        public Message Message { get; }

        public bool IsSuccess { get; }
    }
}
