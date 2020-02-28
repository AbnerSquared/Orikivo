using Discord.WebSocket;

namespace Orikivo
{
    public class MessageMentionInfo : EntityInfo
    {
        public MessageMentionInfo(SocketUser user) : base(user)
        {
            Type = MentionType.User;
        }

        public MessageMentionInfo(SocketRole role): base(role)
        {
            Type = MentionType.Role;
        }

        public MentionType Type { get; }
    }
}