using Discord;
using Discord.WebSocket;
using Orikivo.Storage;
using System;
using System.Linq;

namespace Orikivo
{
    public static class SocketMessageExtension
    {
        /*
        public static bool HasEmoji(this IMessage message)
        {
            string raw = message.Content;

        }
        */

        public static bool HasFile(this SocketMessage message) =>
            message.Attachments.Any();

        public static bool HasImage(this SocketMessage message) =>
            message.Attachments.Any(x => x.Filename.EndsWithAny(".png", ".jpg", ".gif"));

        public static bool Mentions(this SocketMessage message, SocketUser user) =>
            message.MentionedUsers.Contains(user);

        public static bool Mentions(this SocketMessage message, SocketChannel channel) =>
            message.MentionedChannels.Contains(channel);

        public static bool Mentions(this SocketMessage message, SocketRole role) =>
            message.MentionedRoles.Contains(role);

        public static bool HasEmbed(this SocketMessage message) =>
            message.Embeds.Any();

        public static bool HasMentionedRoles(this SocketMessage message) =>
            message.MentionedRoles.Any();

        public static bool HasMentionedChannels(this SocketMessage message) =>
            message.MentionedChannels.Any();

        public static bool HasMentionedUsers(this SocketMessage message) =>
            message.MentionedUsers.Any();

        public static bool MentionsAny(this SocketMessage message) =>
            message.HasMentionedRoles() ||
            message.HasMentionedChannels() ||
            message.HasMentionedUsers();

        //public static List<Attachment> GetAttachments(this SocketMessage message) =>
        //    message.Attachments.
    }
}