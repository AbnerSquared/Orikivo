using Discord.WebSocket;
using System;
using System.Collections.Generic;

namespace Orikivo
{
    public class MessageInfo
    {
        public MessageInfo(SocketMessage message)
        {
            Id = message.Id;
            Author = new UserBaseInfo(message.Author);
        }

        public ulong Id { get; }
        public UserBaseInfo Author { get; }
        public string Content { get; }
        public List<MessageReactionInfo> Reactions { get; }
        public List<MessageMentionInfo> Mentions { get; }
        public DateTime Timestamp { get; }

    }
}