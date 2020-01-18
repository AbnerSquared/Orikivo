using Discord.WebSocket;

namespace Orikivo
{
    public abstract class MessageFilter
    {
        public abstract bool Judge(SocketMessage message, int index);

        public virtual bool JudgeMany(SocketMessage message, FilterCollection matches, int index)
            => Judge(message, index);
    }
}
