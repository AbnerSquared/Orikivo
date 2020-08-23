using Orikivo;

namespace Arcadia
{
    public class UsageResult
    {
        public UsageResult(string content, bool isSuccess = true)
        {
            Message = new MessageBuilder{ Content = content }.Build();
            IsSuccess = isSuccess;
        }

        public UsageResult(Message message, bool isSuccess)
        {
            Message = message;
            IsSuccess = isSuccess;
        }

        public UsageResult(bool isSuccess)
        {
            IsSuccess = isSuccess;
        }

        public Message Message { get; set; }

        public bool IsSuccess { get; set; }
    }
}
