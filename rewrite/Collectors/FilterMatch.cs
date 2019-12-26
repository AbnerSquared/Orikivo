using Discord.WebSocket;
using System;

namespace Orikivo
{
    public class FilterMatch
    {
        public FilterMatch(SocketMessage message, int index, bool isSuccess)
        {
            Message = message;
            Index = index;
            IsSuccess = isSuccess;
        }

        public int Index { get; }
        public SocketMessage Message { get; }
        public bool IsSuccess { get; }

        public T Convert<T>(Func<FilterMatch, T> converter)
        {
            return converter.Invoke(this);
        }
    }
}
