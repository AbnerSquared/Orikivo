using Discord;
using Discord.WebSocket;
using System;

namespace Orikivo
{
    public class UserInfo : UserBaseInfo
    {
        public UserInfo(SocketUser user) : base(user)
        {
            Activity = new ActivityInfo(user.Activity); // replaces the ActivityBaseInfo variant.
            Flags = new UserFlags(user.IsBot, user.IsWebhook);
            
        }

        public new ActivityInfo Activity { get; }

        public UserFlags Flags { get; }
        
        public UserGuildInfo GuildInfo { get; }
    }

    [Flags]
    public enum UserFlag
    {
        Bot = 1,
        Webhook = 2
    }

    public class UserFlags
    {
        public UserFlags(bool isBot = false, bool isWebhook = false)
        {
            IsBot = isBot;
            IsWebhook = isWebhook;
        }
        public bool IsBot { get; }
        public bool IsWebhook { get; }
    }
}